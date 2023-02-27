using HarmonyLib;

namespace XStorage
{
    internal static class Patches
    {
        #region Harmony
        private static readonly Harmony patcher;

        static Patches()
        {
            patcher = new Harmony(XStorage.PluginGUID  + ".harmony");
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
                if (container)
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
                if (!InventoryGui.instance.m_currentContainer)
                {
                    // InventoryGui does not have a container yet, so this one must be the one it's trying to show
                    Jotunn.Logger.LogDebug($"Opening container `{__instance.GetXStorageNameOrDefault()}` into the vanilla UI");
                    return true;
                }

                if (!granted)
                {
                    Jotunn.Logger.LogDebug($"No access to `{__instance.GetXStorageNameOrDefault()}`");
                    return false;
                }

                Jotunn.Logger.LogDebug($"Opening `{__instance.GetXStorageNameOrDefault()}` into XStorage panel");
                ContainersPanel.Instance.Show(__instance);
                return false;
            }
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

                ContainersPanel.Instance.Hide();
            }
        }

        [HarmonyPatch(typeof(Container), nameof(Container.Awake))]
        static class Container_Awake
        {
            static void Postfix(Container __instance)
            {
                // Ideally we'd check if this is a container placed by a player, but it appears that during Awake(), the .m_piece property is not initialised yet

                // Add the ContainerTextReceiver component
                if (!__instance.GetComponent<ContainerTextReceiver>())
                {
                    __instance.gameObject.AddComponent<ContainerTextReceiver>();
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
                static bool Prefix(Container __instance, bool alt)
                {
                    if (!alt)
                    {
                        // User is not holding "alt" key (which is `shift` by default), ignore
                        return true;
                    }

                    if (!__instance.IsPlacedByPlayer())
                    {
                        // Don't do anything to containers that weren't placed by a player
                        return true;
                    }

                    // TODO: check ward access

                    var containerTextReceiver = __instance.GetComponent<ContainerTextReceiver>();
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
        #endregion
    }
}
