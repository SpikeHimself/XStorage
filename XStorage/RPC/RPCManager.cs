using System;

namespace XStorage.RPC
{
    internal static class RPCManager
    {
        #region RPC Names
        internal const string RPC_RENAMEREQUEST = Mod.Info.Name + "_RenameRequest";
        #endregion

        /// <summary>
        /// Register our RPCs with ZRoutedRpc, so that the game knows which function to call when these messages arrive
        /// </summary>
        internal static void Register()
        {
            ZRoutedRpc.instance.Register(RPC_RENAMEREQUEST, new Action<long, ZDOID, string>(ServerEvents.RPC_RenameRequest));
        }
    }
}
