using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using XStorage.GUI;

namespace XStorage
{
    public class Config
    {
        public const string ZdoProperty_ContainerName = "XStorage_Name";
        public const string ZdoProperty_GridSize = "XStorage_Size";

        ////////////////////////////
        //// Singleton instance ////
        private static readonly Lazy<Config> lazy = new Lazy<Config>(() => new Config());
        public static Config Instance { get { return lazy.Value; } }
        ////////////////////////////

        private ConfigFile configFile;


        // TODO move to global config class and read from configfile etc
        private static int MaxRows = 3;
        private static int MaxColumns = 3;
        public static readonly GridSize MaxSize = new GridSize(MaxRows, MaxColumns);

        /// <summary>
        /// Load the config file, and track the settings inside it
        /// </summary>
        /// <param name="configFile">The config file being loaded</param>
        public void LoadLocalConfig(ConfigFile configFile)
        {
            //this.configFile = configFile;
            //ReloadLocalConfig();

            //this.configFile.ConfigReloaded += LocalConfigChanged;
            //this.configFile.SettingChanged += LocalConfigChanged;

            //if (XPortal.IsServer())
            //{
            //    Server = Local;
            //}
        }
    }
}
