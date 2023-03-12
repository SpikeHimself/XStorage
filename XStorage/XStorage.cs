using BepInEx;
using Jotunn.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XStorage
{
    [BepInDependency(Jotunn.Main.ModGuid)]
    [BepInPlugin(Mod.Info.GUID, Mod.Info.Name, Mod.Info.Version)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Patch)]
    public class XStorage : BaseUnityPlugin
    {
        #region Unity Events
        /// <summary>
        /// https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "MonoBehaviour.Awake is called when the script instance is being loaded.")]
        private void Awake()
        {
            // Hello, world!
            Jotunn.Logger.LogDebug("oooh chesty!");

            // Load config
            XConfig.Instance.LoadLocalConfig(Config);

            // Apply the Harmony patches
            Patches.Patcher.Patch();
        }

        /// <summary>
        /// https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "MonoBehaviour.Update is called every frame, if the MonoBehaviour is enabled.")]
        private void Update()
        {
        }

        /// <summary>
        /// https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnDestroy.html
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "MonoBehaviour.OnDestroy occurs when a Scene or game ends.")]
        private void OnDestroy()
        {
            PanelManager.Instance.Clear();
            Patches.Patcher.Unpatch();
        }
        #endregion

        #region Game
        public static void GameStarted()
        {
            PanelManager.Instance.Reset();
            RPC.RegisterRPCs();
        }
        #endregion

        #region Container functions
        //public enum ContainerSearchMethod
        //{
        //    NearPlayer,
        //    NearContainer
        //}

        public static void OpenNearbyContainers(Container container) //, ContainerSearchMethod method = ContainerSearchMethod.NearPlayer)
        {
            var player = Player.m_localPlayer;
            var nearbyContainers = FindNearbyContainers(container); //, method);

            Jotunn.Logger.LogDebug($"Found {nearbyContainers.Count} extra container(s)");
            foreach (Container nearbyContainer in nearbyContainers)
            {
                Jotunn.Logger.LogDebug($"Calling .Interact() on extra container `{nearbyContainer.GetXStorageNameOrDefault()}`");
                nearbyContainer.Interact(player, false, false);
            }
        }

        public static List<Container> FindNearbyContainers(Container container) //, ContainerSearchMethod method)
        {
            var maxDistance = XConfig.Instance.NearbyChestRadius.Value;
            
            return GameObject.FindObjectsOfType<Container>()
            .Where(c =>
                    c != container &&
                    !PanelManager.Instance.ContainsPanel(c) &&
                    c.IsPlacedByPlayer() &&
                    !c.IsInUse() &&
                    c.Distance(container) <= maxDistance
                )
                .OrderByDescending(c => c.GetInventory().GetTotalWeight())
                .ToList();
        }
        #endregion

        #region Name functions

        public static void UpdateContainerAndInventoryName(Container container)
        {
            var newName = container.GetXStorageName();
            if (string.IsNullOrEmpty(newName))
            {
                return;
            }

            if (!container.m_name.Equals(newName) || !container.GetInventory().m_name.Equals(newName))
            {
                UpdateContainerAndInventoryName(container, newName);
            }
        }

        public static void UpdateContainerAndInventoryName(Container container, string newName)
        {
            Jotunn.Logger.LogDebug($"Updating `{container.m_name}`: `{newName}`");
            container.m_name = newName;
            container.GetInventory().m_name = newName;
        }
        #endregion

        #region Inventory functions
        public static bool MoveItemToSuitableContainer(InventoryGrid grid, ItemDrop.ItemData item)
        {
            var itemName = item.m_shared.m_name;

            var containerWithStackSpace = FindSuitableContainer(itemName);
            if (!containerWithStackSpace)
            {
                return false;
            }

            var itemNameLocalised = Localization.instance.Localize(item.m_shared.m_name);
            Jotunn.Logger.LogDebug($"Moving `{itemNameLocalised}` to `{containerWithStackSpace.GetXStorageNameOrDefault()}`");

            containerWithStackSpace.GetInventory().MoveItemToThis(grid.GetInventory(), item);
            return true;
        }

        private static Container FindSuitableContainer(string itemName)
        {
            var itemNameLocalised = Localization.instance.Localize(itemName);

            Jotunn.Logger.LogDebug($"Looking for containers containing `{itemNameLocalised}`");

            var allContainers = new List<Container>
            {
                InventoryGui.instance.m_currentContainer
            };
            allContainers.AddRange(PanelManager.Instance.GetContainerList());

            var candidates = allContainers
                .Where(c => c.HasRoomFor(itemName))
                .OrderByDescending(c => c.GetInventory().CountItems(itemName));

            if (candidates.Any())
            {
                Jotunn.Logger.LogDebug($"Found {candidates.Count()} container(s) containing `{itemNameLocalised}` with room for more");
                return candidates.First();
            }

            return null;
        }
        #endregion
    }
}
