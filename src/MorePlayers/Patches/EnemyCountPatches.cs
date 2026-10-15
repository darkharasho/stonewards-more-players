using HarmonyLib;
using Mirror;
using UnityEngine;

namespace MorePlayers.Patches
{
    // Swaps each wave for a copy with bigger count ranges before it spawns, so the wave's total, its
    // progress bar and its end check all use the scaled numbers.
    [HarmonyPatch(typeof(EnemyManager), "SpawnCurrentWave")]
    internal static class EnemyCountPatch
    {
        private static EnemyWaveDataSO _lastClone;

        private static void Prefix(ref EnemyWaveDataSO _Wave)
        {
            if (!NetworkServer.active || _Wave == null || _Wave.Enemies == null) return;
            float multiplier = 1f + Plugin.EnemyCountPerExtraPlayer.Value * Scaling.ExtraPlayers;
            if (multiplier <= 1f) return;

            EnemyWaveDataSO clone = Object.Instantiate(_Wave);
            var enemies = clone.Enemies;
            for (int i = 0; i < enemies.Count; i++)
            {
                var entry = enemies[i];
                // Single spawns (bosses, elites) stay single.
                if (entry.countRange.y <= 1f) continue;
                int min = Scaling.RandomRound(entry.countRange.x * multiplier);
                int max = Mathf.Max(min, Scaling.RandomRound(entry.countRange.y * multiplier));
                entry.countRange = new Vector2(min, max);
                enemies[i] = entry;
            }

            // The previous wave has finished spawning by now, so its copy is no longer read.
            if (_lastClone != null) Object.Destroy(_lastClone);
            _lastClone = clone;
            _Wave = clone;
        }
    }
}
