using HarmonyLib;
using Jotunn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XStorage.Components;

namespace XStorage.Patches
{
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
}
