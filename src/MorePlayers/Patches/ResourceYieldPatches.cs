using System;
using HarmonyLib;
using Mirror;
using UnityEngine;

namespace MorePlayers.Patches
{
    // Extra copies of mined resources, barrel contents and enemy drops. Pickables are only multiplied
    // inside mining and barrel spawns, so items players drop or take out of storage are never duplicated.
    internal static class ResourceYield
    {
        internal static int ScopeDepth;
        private static bool _spawningExtra;

        internal static void SpawnExtras(Action spawn)
        {
            if (_spawningExtra || !NetworkServer.active) return;
            float multiplier = 1f + Plugin.ResourceYieldPerExtraPlayer.Value * Scaling.ExtraPlayers;
            if (multiplier <= 1f) return;

            int extra = Scaling.RandomRound(multiplier - 1f);
            _spawningExtra = true;
            try
            {
                for (int i = 0; i < extra; i++) spawn();
            }
            finally
            {
                _spawningExtra = false;
            }
        }
    }

    [HarmonyPatch(typeof(DiggingManager), nameof(DiggingManager.ServerTrySpawnResource))]
    internal static class MiningScopePatch
    {
        private static void Prefix() => ResourceYield.ScopeDepth++;
        private static void Finalizer() => ResourceYield.ScopeDepth--;
    }

    [HarmonyPatch(typeof(ItemBarrel), nameof(ItemBarrel.ServerInit))]
    internal static class BarrelScopePatch
    {
        private static void Prefix() => ResourceYield.ScopeDepth++;
        private static void Finalizer() => ResourceYield.ScopeDepth--;
    }

    [HarmonyPatch(typeof(ItemManager), nameof(ItemManager.ServerInstantiatePickableItem))]
    internal static class PickableYieldPatch
    {
        private static void Postfix(ItemManager __instance, PickableItem __result, string _ItemID, int _Count,
            Vector3 _Position, bool forceShow, bool disableVfx)
        {
            if (ResourceYield.ScopeDepth == 0 || __result == null) return;
            Quaternion rotation = __result.transform.rotation;
            ResourceYield.SpawnExtras(() =>
            {
                PickableItem copy = __instance.ServerInstantiatePickableItem(_ItemID, _Count, _Position, forceShow, disableVfx);
                if (copy != null) copy.transform.rotation = rotation;
            });
        }
    }

    [HarmonyPatch(typeof(ItemManager), nameof(ItemManager.ServerInstantiateEnemyDrop))]
    internal static class EnemyDropYieldPatch
    {
        private static void Postfix(ItemManager __instance, PickableItem __result, Vector3 _Position)
        {
            if (__result == null) return;
            ResourceYield.SpawnExtras(() => __instance.ServerInstantiateEnemyDrop(_Position));
        }
    }
}
