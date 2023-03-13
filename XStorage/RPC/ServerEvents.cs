namespace XStorage.RPC
{
    internal static class ServerEvents
    {
        /// <summary>
        /// A client wishes to rename a container
        /// </summary>
        /// <param name="sender">The client that wishes to rename a container</param>
        /// <param name="containerId">The ZDOID of the container being renamed</param>
        /// <param name="newName">The new name of the container</param>
        internal static void RPC_RenameRequest(long sender, ZDOID containerId, string newName)
        {
            if (!Environment.IsServer)
            {
                Jotunn.Logger.LogDebug($"{sender} wants to rename container `{containerId}` to `{newName}`, but I am not the server");
                return;
            }

            Jotunn.Logger.LogDebug($"{sender} wants to rename container `{containerId}` to `{newName}`");
            XStorage.SetXStorageName(containerId, newName);
        }
    }
}
