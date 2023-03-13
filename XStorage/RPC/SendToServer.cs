namespace XStorage.RPC
{
    internal static class SendToServer
    {
        /// <summary>
        /// Send the server a request to rename a container
        /// </summary>
        /// <param name="containerId">The ZDOID of the container being renamed</param>
        /// <param name="newName">The new name of the container</param>
        internal static void RenameRequest(ZDOID containerId, string newName)
        {
            Jotunn.Logger.LogDebug($"Requesting server to rename `{containerId}` to `{newName}`");
            ZRoutedRpc.instance.InvokeRoutedRPC(Environment.ServerPeerId, RPCManager.RPC_RENAMEREQUEST, containerId, newName);
        }
    }
}
