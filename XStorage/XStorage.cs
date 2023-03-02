using BepInEx;
using Jotunn.Managers;
using Jotunn.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XStorage
{
    [BepInDependency(Jotunn.Main.ModGuid)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Patch)]
    public class XStorage : BaseUnityPlugin
    {
        #region Plugin info
        // This is *the* place to edit plugin details. Everywhere else will be generated based on this info.
        public const string PluginGUID = "yay.spikehimself.xstorage";
        public const string PluginName = "XStorage";
        public const string PluginVersion = "1.0.2";
        public const string PluginDescription = "Open multiple chests at once, rename them, and move items/stacks to the most suitable chest.";
        public const string PluginWebsiteUrl = "https://github.com/SpikeHimself/XStorage";
        public const int PluginNexusId = 2290;
        //public const string PluginBepInVersion = ??
        public const string PluginJotunnVersion = Jotunn.Main.Version;
        #endregion

        public const string ZdoProperty_ContainerName = "XStorage_Name";

        #region Determine Environment
        /// <summary>
        /// Are we the Server?
        /// </summary>
        /// <returns>True if ZNet says we are a server</returns>
        public static bool IsServer()
        {
            return ZNet.instance != null && ZNet.instance.IsServer();
        }

        /// <summary>
        /// Are we Headless? (dedicated server)
        /// </summary>
        /// <returns>True if SystemInfo.graphicsDeviceType is not set</returns>
        public static bool IsHeadless()
        {
            return GUIManager.IsHeadless();
        }
        #endregion

        #region Unity Events
        /// <summary>
        /// https://docs.unity3d.com/ScriptReference/MonoBehaviour.Awake.html
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "MonoBehaviour.Awake is called when the script instance is being loaded.")]
        private void Awake()
        {
            // Hello, world!
            Jotunn.Logger.LogDebug("oooh chesty!");

            // Add Nexus ID to config for Nexus Update Check (https://www.nexusmods.com/valheim/mods/102)
            Config.Bind<int>("General", "NexusID", PluginNexusId, "Nexus mod ID for updates (do not change)");

            // Apply the Harmony patches
            Patches.Patch();
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
            ContainersPanel.Instance.Clear();
            Patches.Unpatch();
        }
        #endregion

        #region Game
        public static void GameStarted()
        {
            ContainersPanel.Instance.Clear();
            RPC.RegisterRPCs();
        }
        #endregion

        #region Container functions
        public static void OpenNearbyContainers(Container container)
        {
            var player = Player.m_localPlayer;
            var nearbyContainers = FindNearbyContainers(container);

            Jotunn.Logger.LogDebug($"Found {nearbyContainers.Count} extra container(s)");
            foreach (Container nearbyContainer in nearbyContainers)
            {
                Jotunn.Logger.LogDebug($"Calling .Interact() on extra container `{nearbyContainer.GetXStorageNameOrDefault()}`");
                nearbyContainer.Interact(player, false, false);
            }
        }

        public static List<Container> FindNearbyContainers(Container container)
        {
            var player = Player.m_localPlayer;
            return GameObject.FindObjectsOfType<Container>()
            .Where(c =>
                    c != container &&
                    c.IsPlacedByPlayer() &&
                    c.Distance(player) < 5f
                )
                .OrderBy(c => c.Distance(player))
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
                RenameContainerAndInventory(container, newName);
            }
        }

        public static void RenameContainerAndInventory(Container container, string newName)
        {
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
            allContainers.AddRange(ContainersPanel.Instance.GetContainers());

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
