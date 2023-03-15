using BepInEx.Configuration;
using System;
using UnityEngine;
using XStorage.GUI;

namespace XStorage
{
    public class XConfig
    {
        public const string Key_ContainerName = Mod.Info.Name + "_Name";

        ////////////////////////////
        //// Singleton instance ////
        private static readonly Lazy<XConfig> lazy = new Lazy<XConfig>(() => new XConfig());
        public static XConfig Instance { get { return lazy.Value; } }
        ////////////////////////////

        private ConfigFile configFile;
        public ConfigEntry<int> MaxColumns;
        public ConfigEntry<int> MaxRows;
        public ConfigEntry<int> NearbyChestRadius;
        //public ConfigEntry<bool> ChestChaining;

        public GridSize MaxSize
        {
            get
            {
                return new GridSize(MaxColumns.Value, MaxRows.Value);
            }
        }

        /// <summary>
        /// Load the config file, and track the settings inside it
        /// </summary>
        /// <param name="configFile">The config file being loaded</param>
        public void LoadLocalConfig(ConfigFile configFile)
        {
            this.configFile = configFile;

            // Add Nexus ID to config for Nexus Update Check (https://www.nexusmods.com/valheim/mods/102)
            configFile.Bind("General", "NexusID", Mod.Info.NexusId, "Nexus mod ID for updates (do not change)");

            MaxColumns = configFile.Bind("UI", "MaxColumns", 2, "The maximum amount of columns XStorage can expand the containers panel to.");
            MaxRows = configFile.Bind("UI", "MaxRows", 3, "The maximum amount of rows XStorage can expand the containers panel to.");
            //ChestChaining = configFile.Bind("Algorithm", "ChestChaining", true, "Recursively open chests near the chest that was opened.");
            NearbyChestRadius = configFile.Bind("Algorithm", "NearbyChestRange", 4, "The radius in meters within which to look for nearby chests. Setting this too high will cause performance issues.");

            // Bind the entry of an arbitrary gridsize so that the description is saved in the config file.
            BindPanelPosition(GridSize.OneByOne);

            //this.configFile.ConfigReloaded += LocalConfigChanged;
            //this.configFile.SettingChanged += LocalConfigChanged;
        }

        #region Panel Position
        private ConfigEntry<Vector2> BindPanelPosition(GridSize gridSize)
        {
            var key = $"Position_{gridSize}";
            return configFile.Bind("Panel Position", key, Vector2.zero, $"The position on the screen when the panel is sized `{gridSize}`");
        }

        public void SavePanelPosition(GridSize gridSize, Vector2 position)
        {
            var panelPosition = BindPanelPosition(gridSize);
            panelPosition.Value = position;
            configFile.Save();
        }

        public Vector2 GetPanelPosition(GridSize gridSize)
        {
            var panelPosition = BindPanelPosition(gridSize);
            return panelPosition.Value;
        }
        #endregion
    }
}
