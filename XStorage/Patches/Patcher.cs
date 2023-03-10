using HarmonyLib;

namespace XStorage.Patches
{
    internal static class Patcher
    {
        #region Harmony
        private static readonly Harmony patcher = new Harmony(Mod.Info.HarmonyGUID);
        public static void Patch() => patcher.PatchAll();
        public static void Unpatch() => patcher?.UnpatchSelf();
        #endregion
    }
}
