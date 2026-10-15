using HarmonyLib;
using Mirror;

namespace MorePlayers.Patches
{
    // maxConnections sizes the Steam lobby (SteamLobby.HostLobby), caps Mirror's server and sets how
    // many rows the session settings screen builds, so raising it is all hosting needs.
    [HarmonyPatch(typeof(StoneWardsNetworkManager), nameof(StoneWardsNetworkManager.Awake))]
    internal static class RaiseMaxConnectionsPatch
    {
        private static void Postfix(StoneWardsNetworkManager __instance) => Apply(__instance);

        internal static void Apply(NetworkManager manager)
        {
            if (manager == null) return;
            manager.maxConnections = Plugin.MaxPlayers.Value;
        }
    }

    // Pick up config changes made in-game before the next lobby is created.
    [HarmonyPatch(typeof(SteamLobby), nameof(SteamLobby.HostLobby))]
    internal static class RefreshBeforeHostPatch
    {
        private static void Prefix() => RaiseMaxConnectionsPatch.Apply(NetworkManager.singleton);
    }

    // The hub's "x / 4" player count label hardcodes 4.
    [HarmonyPatch(typeof(HubPlayersNameHolder), "InitUI")]
    internal static class HubPlayerCountLabelPatch
    {
        private static void Postfix(ref int ___maxPlayerCount)
        {
            if (___maxPlayerCount > 1) ___maxPlayerCount = NetworkManager.singleton.maxConnections;
        }
    }
}
