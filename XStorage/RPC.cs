using System;

namespace XStorage
{
    internal static class RPC
    {
        #region RPC Names
        private const string RPC_RENAMEREQUEST = "XStorage_RenameRequest";
        #endregion

        private static long GetServerPeerId()
        {
            return ZRoutedRpc.instance.GetServerPeerID();
        }

        /// <summary>
        /// Register our RPCs with ZRoutedRpc, so that the game knows which function to call when these messages arrive
        /// </summary>
        public static void RegisterRPCs()
        {
            ZRoutedRpc.instance.Register<ZDOID, string>(RPC_RENAMEREQUEST, new Action<long, ZDOID, string>(RPC_RenameRequest));
        }

        /// <summary>
        /// Send the server a request to rename a container
        /// </summary>
        /// <param name="containerId">The ZDOID of the container being renamed</param>
        /// <param name="newName">The new name of the container</param>
        public static void SendRenameRequestToServer(ZDOID containerId, string newName)
        {
            Jotunn.Logger.LogDebug($"Requesting server to rename `{containerId}` to `{newName}`");
            ZRoutedRpc.instance.InvokeRoutedRPC(GetServerPeerId(), RPC_RENAMEREQUEST, containerId, newName);
        }

        /// <summary>
        /// A client wishes to rename a container
        /// </summary>
        /// <param name="sender">The client that wishes to rename a container</param>
        /// <param name="containerId">The ZDOID of the container being renamed</param>
        /// <param name="newName">The new name of the container</param>
        private static void RPC_RenameRequest(long sender, ZDOID containerId, string newName)
        {
            if (!Environment.IsServer())
            {
                Jotunn.Logger.LogDebug($"{sender} wants to rename container `{containerId}` to `{newName}`, but I am not the server");
                return;
            }

            Jotunn.Logger.LogDebug($"{sender} wants to rename container `{containerId}` to `{newName}`");
            ZDOMan.instance.GetZDO(containerId).Set(XStorageConfig.ZdoProperty_ContainerName, newName);
        }
    }
}
