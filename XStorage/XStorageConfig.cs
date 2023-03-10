using BepInEx.Configuration;
using System;
using XStorage.GUI;

namespace XStorage
{
    public class XStorageConfig
    {
        public const string ZdoProperty_ContainerName = "XStorage_Name";
        public const string ZdoProperty_GridSize = "XStorage_Size";

        ////////////////////////////
        //// Singleton instance ////
        private static readonly Lazy<XStorageConfig> lazy = new Lazy<XStorageConfig>(() => new XStorageConfig());
        public static XStorageConfig Instance { get { return lazy.Value; } }
        ////////////////////////////

        //private ConfigFile configFile;
        public ConfigEntry<int> MaxColumns;
        public ConfigEntry<int> MaxRows;
        public ConfigEntry<GridSize.ExpandPreference> ExpandPreference;
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
            //this.configFile = configFile;

            // Add Nexus ID to config for Nexus Update Check (https://www.nexusmods.com/valheim/mods/102)
            configFile.Bind("General", "NexusID", Mod.Info.NexusId, "Nexus mod ID for updates (do not change)");

            MaxColumns = configFile.Bind("UI", "MaxColumns", 2, "The maximum amount of columns XStorage can expand the containers panel to.");
            MaxRows = configFile.Bind("UI", "MaxRows", 3, "The maximum amount of rows XStorage can expand the containers panel to.");
            ExpandPreference = configFile.Bind("UI", "ExpandPreference", GridSize.ExpandPreference.ColumnsFirst, "Determines how XStorage expands the containers panel.");
            //ChestChaining = configFile.Bind("Algorithm", "ChestChaining", true, "Recursively open chests near the chest that was opened.");

            //this.configFile.ConfigReloaded += LocalConfigChanged;
            //this.configFile.SettingChanged += LocalConfigChanged;
        }
    }
}
