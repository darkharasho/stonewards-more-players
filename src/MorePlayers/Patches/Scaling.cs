using System;
using UnityEngine;

namespace MorePlayers.Patches
{
    // Host-side state shared by the scaling patches. Nothing scales until the run has had more than
    // the vanilla 4 players.
    internal static class Scaling
    {
        internal const int VanillaMaxPlayers = 4;

        // Highest real player count this run has reached, like DifficultyManager's own
        // syncMaxPlayerCountReached, which ForgeCostMultiplier inflates.
        internal static int RealPlayerCount;

        internal static int ExtraPlayers => Math.Max(0, RealPlayerCount - VanillaMaxPlayers);

        // Rounds up with probability equal to the fraction, so small amounts still average out right.
        internal static int RandomRound(float value)
        {
            int whole = Mathf.FloorToInt(value);
            return whole + (UnityEngine.Random.value < value - whole ? 1 : 0);
        }
    }
}
