//using System;

//namespace XStorage
//{
//    internal static class RPC
//    {
//        #region RPC Names
//        private const string RPC_RENAMEREQUEST = Mod.Info.Name + "_RenameRequest";
//        #endregion

//        /// <summary>
//        /// Register our RPCs with ZRoutedRpc, so that the game knows which function to call when these messages arrive
//        /// </summary>
//        internal static void RegisterRPCs()
//        {
//            ZRoutedRpc.instance.Register(RPC_RENAMEREQUEST, new Action<long, ZDOID, string>(Events.Server.RPC_RenameRequest));
//        }

//        /// <summary>
//        /// Send the server a request to rename a container
//        /// </summary>
//        /// <param name="containerId">The ZDOID of the container being renamed</param>
//        /// <param name="newName">The new name of the container</param>
//        internal static void SendRenameRequestToServer(ZDOID containerId, string newName)
//        {
//            Jotunn.Logger.LogDebug($"Requesting server to rename `{containerId}` to `{newName}`");
//            ZRoutedRpc.instance.InvokeRoutedRPC(Environment.ServerPeerId, RPC_RENAMEREQUEST, containerId, newName);
//        }

//        #region Events
//        internal static class Events
//        {
//            internal static class Server
//            {
//                /// <summary>
//                /// A client wishes to rename a container
//                /// </summary>
//                /// <param name="sender">The client that wishes to rename a container</param>
//                /// <param name="containerId">The ZDOID of the container being renamed</param>
//                /// <param name="newName">The new name of the container</param>
//                internal static void RPC_RenameRequest(long sender, ZDOID containerId, string newName)
//                {
//                    if (!Environment.IsServer)
//                    {
//                        Jotunn.Logger.LogDebug($"{sender} wants to rename container `{containerId}` to `{newName}`, but I am not the server");
//                        return;
//                    }

//                    Jotunn.Logger.LogDebug($"{sender} wants to rename container `{containerId}` to `{newName}`");
//                    XStorage.SetXStorageName(containerId, newName);
//                }
//            }
//        }
//        #endregion
//    }
//}
