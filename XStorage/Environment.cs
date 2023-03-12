﻿using Jotunn.Managers;

namespace XStorage
{
    internal static class Environment
    {
        /// <summary>
        /// Are we the Server?
        /// </summary>
        /// <returns>True if ZNet says we are a server</returns>
        public static bool IsServer
        {
            get
            {
                return ZNet.instance != null && ZNet.instance.IsServer();
            }
        }

        /// <summary>
        /// Are we Headless? (dedicated server)
        /// </summary>
        /// <returns>True if SystemInfo.graphicsDeviceType is not set</returns>
        public static bool IsHeadless
        {
            get
            {
                return GUIManager.IsHeadless();
            }
        }

        /// <summary>
        /// Is the Game shutting down? This happens on logout and on quit.
        /// </summary>
        public static bool ShuttingDown
        {
            get
            {
                return Game.instance.m_shuttingDown;
            }
        }
    }
}
