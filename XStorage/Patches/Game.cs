using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XStorage.Patches
{
    [HarmonyPatch(typeof(Game), nameof(Game.Awake))]
    static class Game_Awake
    {
        /// <summary>
        /// Set Game.isModded as per the request in Game.messageForModders
        /// </summary>
        static void Prefix(ref bool ___isModded)
        {
            ___isModded = true;
        }
    }

    [HarmonyPatch(typeof(Game), nameof(Game.Start))]
    static class Game_Start
    {
        /// <summary>
        /// The game has started!
        /// </summary>
        static void Postfix()
        {
            XStorage.GameStarted();
        }
    }
}
