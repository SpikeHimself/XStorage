using BepInEx.Configuration;
using System;
using UnityEngine;
using XStorage.GUI;

namespace XStorage
{
    public class XConfig
    {


        ////////////////////////////
        //// Singleton instance ////
        private static readonly Lazy<XConfig> lazy = new Lazy<XConfig>(() => new XConfig());
        public static XConfig Instance { get { return lazy.Value; } }
        ////////////////////////////

        private ConfigFile configFile;
        private ConfigEntry<int> MaxColumns;
        private ConfigEntry<int> MaxRows;
        public ConfigEntry<int> NearbyChestRadius;
        public ConfigEntry<int> MaxOpenChests;
        private ConfigEntry<float> PanelScale;

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
            NearbyChestRadius = configFile.Bind("Algorithm", "NearbyChestRange", 4, "The radius in meters within which to look for nearby chests. Setting this too high will cause performance issues.");
            MaxOpenChests = configFile.Bind("General", "MaxOpenChests", 0, "The maximum amount of chests to open at once. 0 or less means infinite.");
            PanelScale = configFile.Bind("UI", "PanelScale", 1f, "The relative size of XStorage's panel. Can be any value between 0.5 and 1.5, where 0.5 = 50%, 1 = 100%, and 1.5 = 150%");

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

        #region Panel Scale
        public float GetPanelScale()
        {
            var panelScale = PanelScale.Value;
            panelScale = Mathf.Clamp(panelScale, 0.5f, 1.5f);
            PanelScale.Value = panelScale;
            return panelScale;
        }
        #endregion
    }
}
