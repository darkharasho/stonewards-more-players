using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace MorePlayers
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "com.darkharasho.stonewards.moreplayers";
        public const string PluginName = "MorePlayers";
        public const string PluginVersion = "0.1.0";

        internal static ManualLogSource Log;
        internal static ConfigEntry<int> MaxPlayers;

        private Harmony _harmony;

        private void Awake()
        {
            Log = Logger;

            MaxPlayers = Config.Bind("General", "MaxPlayers", 8, new ConfigDescription(
                "Most players allowed in a hosted session, including the host. The game's default is 4. Only the host needs this mod; takes effect the next time you host.",
                new AcceptableValueRange<int>(1, 64),
                new ConfigurationManagerAttributes { DispName = "Max players" }));

            _harmony = new Harmony(PluginGuid);
            _harmony.PatchAll();
            Log.LogInfo($"{PluginName} {PluginVersion} loaded (max players {MaxPlayers.Value})");
        }

        private void OnDestroy() => _harmony?.UnpatchSelf();
    }
}
