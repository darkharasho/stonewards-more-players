using System.Reflection;
using HarmonyLib;
using Mirror;
using UnityEngine;

namespace MorePlayers.Patches
{
    [HarmonyPatch(typeof(DifficultyManager), nameof(DifficultyManager.OnStartServer))]
    internal static class ResetPlayerCountPatch
    {
        private static void Prefix() => Scaling.RealPlayerCount = 0;
    }

    // Combat scaling (syncPlayerFactor) is synced to clients, so it can be set directly. Forge costs are
    // computed on each client from syncMaxPlayerCountReached, so a higher ForgeCostMultiplier reports a
    // bigger player count instead, and combat scaling is then put back on the real count.
    [HarmonyPatch(typeof(DifficultyManager), "SetPlayerCount")]
    internal static class DifficultyScalingPatch
    {
        private static readonly AccessTools.FieldRef<DifficultyManager, float> BaseCombatScaling =
            AccessTools.FieldRefAccess<DifficultyManager, float>("baseCombatScaling");
        private static readonly AccessTools.FieldRef<DifficultyManager, float> CombatScalingPerPlayer =
            AccessTools.FieldRefAccess<DifficultyManager, float>("combatScalingPerPlayer");
        private static readonly AccessTools.FieldRef<DifficultyManager, float> BaseForgeScaling =
            AccessTools.FieldRefAccess<DifficultyManager, float>("baseForgeScaling");
        private static readonly AccessTools.FieldRef<DifficultyManager, float> ForgeScalingPerPlayer =
            AccessTools.FieldRefAccess<DifficultyManager, float>("forgeScalingPerPlayer");
        private static readonly MethodInfo Recalculate = AccessTools.Method(typeof(DifficultyManager), "Recalculate");

        private static void Prefix(DifficultyManager __instance, ref int playerCount)
        {
            if (!NetworkServer.active) return;
            if (playerCount > Scaling.RealPlayerCount) Scaling.RealPlayerCount = playerCount;
            playerCount = ReportedForgeCount(__instance, playerCount);
        }

        private static void Postfix(DifficultyManager __instance)
        {
            if (!NetworkServer.active || Scaling.ExtraPlayers == 0) return;

            float factor = (BaseCombatScaling(__instance) + CombatScalingPerPlayer(__instance) * Scaling.RealPlayerCount)
                * Plugin.CombatDifficultyMultiplier.Value;
            if (Mathf.Approximately(__instance.NetworksyncPlayerFactor, factor)) return;
            __instance.NetworksyncPlayerFactor = factor;
            Recalculate.Invoke(__instance, null);
        }

        private static int ReportedForgeCount(DifficultyManager manager, int playerCount)
        {
            float multiplier = Plugin.ForgeCostMultiplier.Value;
            if (playerCount <= Scaling.VanillaMaxPlayers || multiplier <= 1f) return playerCount;

            float baseForge = BaseForgeScaling(manager);
            float perPlayer = ForgeScalingPerPlayer(manager);
            if (perPlayer <= 0f) return playerCount;
            float target = (baseForge + perPlayer * playerCount) * multiplier;
            return Mathf.Max(playerCount, Mathf.RoundToInt((target - baseForge) / perPlayer));
        }
    }
}
