using Jotunn.Managers;

namespace XStorage
{
    internal static class Environment
    {
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
    }
}
