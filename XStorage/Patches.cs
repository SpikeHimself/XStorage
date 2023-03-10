using HarmonyLib;
using Jotunn;
using UnityEngine;
using XStorage.Components;

namespace XStorage
{
    internal static class Patches
    {
        #region Harmony
        private static readonly Harmony patcher;

        static Patches()
        {
            patcher = new Harmony(Mod.Info.HarmonyGUID);
        }
        public static void Patch()
        {
            patcher.PatchAll();
        }

        public static void Unpatch()
        {
            patcher?.UnpatchSelf();
        }
        #endregion

        #region Patch
        [HarmonyPatch(typeof(Game), nameof(Game.Awake))]
        static class Game_Awake
        {
            /// <summary>
            /// Set Game.isModded as per the request in Game.messageForModders
            /// </summary>
            static void Prefix(ref bool ___isModded)
            {
                ___isModded = true;
            }
        }

        [HarmonyPatch(typeof(Game), nameof(Game.Start))]
        static class Game_Start
        {
            /// <summary>
            /// The game has started!
            /// </summary>
            static void Postfix()
            {
                XStorage.GameStarted();
            }
        }

        [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.Show))]
        static class InventoryGui_Show
        {
            static void Postfix(Container container)
            {
                if (container) // && !XStorageConfig.Instance.ChestChaining.Value)
                {
                    Jotunn.Logger.LogDebug($"Showing container `{container.GetXStorageNameOrDefault()}`");
                    XStorage.OpenNearbyContainers(container);
                }
            }
        }

        [HarmonyPatch(typeof(Container), nameof(Container.RPC_OpenRespons))]
        static class Container_RPCOpenRespons
        {
            static bool Prefix(Container __instance, bool granted)
            {
                var containerName = __instance.GetXStorageNameOrDefault();

                if (!InventoryGui.instance.m_currentContainer)
                {
                    // InventoryGui does not have a container yet, so this one must be the one it's trying to show
                    Jotunn.Logger.LogDebug($"Opening container `{containerName}` into the vanilla UI");
                    return true;
                }

                if (!granted)
                {
                    Jotunn.Logger.LogDebug($"No access to container `{containerName}`");
                    return false;
                }

                Jotunn.Logger.LogDebug($"Opening container `{containerName}` into XStorage panel");
                PanelManager.Instance.Show(__instance);
                return false;
            }

            //static void Postfix(Container __instance, bool granted)
            //{
            //    if(!granted)
            //    {
            //        return;
            //    }

            //    if (XStorageConfig.Instance.ChestChaining.Value)
            //    {
            //        XStorage.OpenNearbyContainers(__instance, XStorage.ContainerSearchMethod.NearContainer);
            //    }
            //}
        }

        [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.Hide))]
        static class InventoryGui_Hide
        {
            static void Prefix(InventoryGui __instance)
            {
                if (!__instance.m_animator.GetBool("visible"))
                {
                    return;
                }

                PanelManager.Instance.Hide();
            }
        }

        [HarmonyPatch(typeof(Container), nameof(Container.Awake))]
        static class Container_Awake
        {
            static void Postfix(Container __instance)
            {
                if (!__instance.m_piece)
                {
                    // Don't do anything if this container has no Piece property (i.e. ghost items).
                    Jotunn.Logger.LogDebug($"Ignoring `{__instance.m_name}` (this is fine)");
                    return;
                }

                XStorage.UpdateContainerAndInventoryName(__instance);
            }
        }

        [HarmonyPatch(typeof(Container), nameof(Container.GetHoverText))]
        static class Container_GetHoverText
        {
            static void Prefix(Container __instance)
            {
                if (!__instance.IsPlacedByPlayer())
                {
                    // Don't do anything to containers that weren't placed by a player
                    return;
                }

                XStorage.UpdateContainerAndInventoryName(__instance);
            }

            static void Postfix(Container __instance, ref string __result)
            {
                if (!__instance.IsPlacedByPlayer())
                {
                    // Don't do anything to containers that weren't placed by a player
                    return;
                }

                // Check if we aren't denied access by a Ward
                var pieceNoAccess = Localization.instance.Localize("$piece_noaccess");
                if (!__result.Contains(pieceNoAccess))
                {
                    // Append `[Shift + E] Rename`
                    __result += Localization.instance.Localize("\n[<color=yellow><b>$KEY_AltPlace + $KEY_Use</b></color>] $hud_rename");
                }
            }

            [HarmonyPatch(typeof(Container), nameof(Container.Interact))]
            static class Container_Interact
            {
                static bool Prefix(Container __instance, Humanoid character, bool alt)
                {
                    if (!alt)
                    {
                        // User is not holding "alt" key (which is `shift` by default), ignore
                        return true;
                    }

                    if (!__instance.IsPlacedByPlayer())
                    {
                        // Container was not placed by a player, ignore
                        return true;
                    }

                    if (__instance.m_checkGuardStone && !PrivateArea.CheckAccess(__instance.transform.position))
                    {
                        // Ward is blocking access, abort!
                        return false;
                    }

                    long playerID = Game.instance.GetPlayerProfile().GetPlayerID();
                    if (!__instance.CheckAccess(playerID))
                    {
                        // Player does not have access, abort!
                        character.Message(MessageHud.MessageType.Center, "$msg_cantopen");
                        return false;
                    }

                    var containerTextReceiver = __instance.gameObject.GetOrAddComponent<ContainerTextReceiver>();
                    TextInput.instance.RequestText((TextReceiver)containerTextReceiver, "$hud_rename", sbyte.MaxValue);
                    return false;
                }
            }
        }

        [HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.OnSelectedItem))]
        static class InventoryGui_OnSelectedItem
        {
            static bool Prefix(InventoryGui __instance, InventoryGrid grid, ItemDrop.ItemData item, Vector2i pos, InventoryGrid.Modifier mod)
            {
                if (grid != InventoryGui.instance.m_playerGrid)
                {
                    // The grid being clicked is the container grid, not the player inventory: Ignore
                    return true;
                }

                if (!__instance.m_currentContainer)
                {
                    // No container opened: Ignore
                    return true;
                }

                if (mod == InventoryGrid.Modifier.Move && !item.m_shared.m_questItem)
                {
                    bool itemMoved = XStorage.MoveItemToSuitableContainer(grid, item);
                    if (itemMoved)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(UITooltip), nameof(UITooltip.OnHoverStart))]
        static class UITooltip_OnHoverStart
        {
            static void Postfix(ref GameObject ___m_tooltip)
            {
                if (___m_tooltip && ___m_tooltip.transform.parent && PanelManager.Instance.IsVisible() &&
                    ___m_tooltip.transform.parent == PanelManager.Instance.RootPanel.ContentPanel.Transform)
                {
                    // Problem 1: The tooltip is only half visible because it's inside a scrollrect
                    //      This is fixed by changing the tooltip's parent to something that's outside of the scrollrect, for example the XStorage root panel

                    // Problem 2: The further you scroll the scrollrect down, the further the tooltip will be from the mouse pointer
                    //      This is fixed by re-instantiating the tooltip.
                    //      Yes, you read that correctly: literally copying the exact same object with the exact same properties will not have that same problem.
                    //      It's Valheim magic. Fifteen hours of my life that I will never get back.

                    var newTooltip = Object.Instantiate(___m_tooltip, PanelManager.Instance.RootPanel.Transform);
                    GameObject.Destroy(___m_tooltip);
                    ___m_tooltip = newTooltip;
                }
            }
        }
        #endregion
    }
}
