﻿using BepInEx;
using Jotunn.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XStorage.RPC;

namespace XStorage
{
    [BepInDependency(Jotunn.Main.ModGuid)]
    [BepInPlugin(Mod.Info.GUID, Mod.Info.Name, Mod.Info.Version)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Patch)]
    public class XStorage : BaseUnityPlugin
    {
        public const string Key_ContainerName = Mod.Info.Name + "_Name";
        public const string Key_SkipMark = Mod.Info.Name + "_SkipMark";

        #region Unity Events
        /// <summary>
        /// https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "MonoBehaviour.Awake is called when the script instance is being loaded.")]
        private void Awake()
        {
            // Hello, world!
            Log.Debug("oooh chesty!");

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
            RPCManager.Register();
        }
        #endregion

        #region Container functions
        public static void OpenNearbyContainers(Container container)
        {
            var nearbyContainers = FindNearbyContainers(container);
            if (nearbyContainers.Count > 0)
            {
                Log.Info($"Found {nearbyContainers.Count} extra chests near `{container.GetXStorageNameOrDefault()}`");

                var player = Player.m_localPlayer;
                foreach (Container nearbyContainer in nearbyContainers)
                {
                    Log.Debug($"Calling .Interact() on extra container `{nearbyContainer.GetXStorageNameOrDefault()}`");
                    nearbyContainer.Interact(player, false, false);
                }
            }
        }

        public static List<Container> FindNearbyContainers(Container container)
        {
            var maxDistance = XConfig.Instance.NearbyChestRadius.Value;
            var maxOpenChests = XConfig.Instance.MaxOpenChests.Value;

            var nearbyContainers =
                GameObject.FindObjectsOfType<Container>()
                    .Where(c =>
                            c != container &&
                            !PanelManager.Instance.ContainsPanel(c) &&
                            c.IsPlacedByPlayer() &&
                            !c.IsInUse() &&
                            c.Distance(container) <= maxDistance
                    )
                    .OrderByDescending(c => c.GetInventory().GetTotalWeight())
                    .ToList();

            if (maxOpenChests > 0)
            {
                nearbyContainers = nearbyContainers.Take(maxOpenChests).ToList();
            }

            return nearbyContainers;
        }
        #endregion

        #region Name functions
        public static void SetXStorageName(ZDOID containerId, string newName)
        {
            ZDOMan.instance.GetZDO(containerId).Set(XStorage.Key_ContainerName, newName);
        }

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
            Log.Debug($"Updating `{container.m_name}`: `{newName}`");
            container.m_name = newName;
            container.GetInventory().m_name = newName;
        }
        #endregion

        #region Inventory functions
        public static bool MoveItemToSuitableContainer(InventoryGrid grid, ItemDrop.ItemData item)
        {
            var itemName = item.m_shared.m_name;
            var worldLevel = item.m_worldLevel;

            var containerWithStackSpace = FindSuitableContainer(itemName, worldLevel);
            if (!containerWithStackSpace)
            {
                return false;
            }

            var itemNameLocalised = Localization.instance.Localize(item.m_shared.m_name);
            Log.Debug($"Moving `{itemNameLocalised}` to `{containerWithStackSpace.GetXStorageNameOrDefault()}`");

            containerWithStackSpace.GetInventory().MoveItemToThis(grid.GetInventory(), item);
            return true;
        }

        private static Container FindSuitableContainer(string itemName, float worldLevel)
        {
            var itemNameLocalised = Localization.instance.Localize(itemName);

            Log.Debug($"Looking for containers containing `{itemNameLocalised}`");

            var allContainers = new List<Container>
            {
                InventoryGui.instance.m_currentContainer
            };
            allContainers.AddRange(PanelManager.Instance.GetContainerList());

            var candidates = allContainers
                .Where(c => c.HasRoomFor(itemName, worldLevel))
                .OrderByDescending(c => c.GetInventory().CountItems(itemName));

            if (candidates.Any())
            {
                Log.Debug($"Found {candidates.Count()} container(s) containing `{itemNameLocalised}` with room for more");
                return candidates.First();
            }

            return null;
        }
        #endregion
    }
}
