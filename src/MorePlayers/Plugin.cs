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
        public const string PluginVersion = "0.2.0";

        internal static ManualLogSource Log;
        internal static ConfigEntry<int> MaxPlayers;
        internal static ConfigEntry<float> EnemyCountPerExtraPlayer;
        internal static ConfigEntry<float> ResourceYieldPerExtraPlayer;
        internal static ConfigEntry<float> CombatDifficultyMultiplier;
        internal static ConfigEntry<float> ForgeCostMultiplier;

        private Harmony _harmony;

        private void Awake()
        {
            Log = Logger;

            MaxPlayers = Config.Bind("General", "MaxPlayers", 8, new ConfigDescription(
                "Most players allowed in a hosted session, including the host. The game's default is 4. Only the host needs this mod; takes effect the next time you host.",
                new AcceptableValueRange<int>(1, 64),
                new ConfigurationManagerAttributes { DispName = "Max players" }));

            EnemyCountPerExtraPlayer = Config.Bind("Scaling", "EnemyCountPerExtraPlayer", 0.25f, new ConfigDescription(
                "Extra enemies per wave for each player beyond 4, as a fraction of the wave. 0.25 means 8 players fight twice as many enemies. 0 turns it off. Bosses and other single spawns are never multiplied.",
                new AcceptableValueRange<float>(0f, 2f),
                new ConfigurationManagerAttributes { DispName = "Enemy count per extra player" }));

            ResourceYieldPerExtraPlayer = Config.Bind("Scaling", "ResourceYieldPerExtraPlayer", 0.15f, new ConfigDescription(
                "Extra mining, enemy drop and barrel yield for each player beyond 4. 0.15 means 8 players get 1.6x. 0 turns it off.",
                new AcceptableValueRange<float>(0f, 2f),
                new ConfigurationManagerAttributes { DispName = "Resource yield per extra player" }));

            CombatDifficultyMultiplier = Config.Bind("Scaling", "CombatDifficultyMultiplier", 1f, new ConfigDescription(
                "Multiplies the game's own enemy strength scaling once there are more than 4 players. 1 leaves it as vanilla.",
                new AcceptableValueRange<float>(1f, 5f),
                new ConfigurationManagerAttributes { DispName = "Combat difficulty multiplier" }));

            ForgeCostMultiplier = Config.Bind("Scaling", "ForgeCostMultiplier", 1f, new ConfigDescription(
                "Multiplies the game's own forge cost scaling once there are more than 4 players. 1 leaves it as vanilla. Applied in steps of about one player's worth of cost.",
                new AcceptableValueRange<float>(1f, 5f),
                new ConfigurationManagerAttributes { DispName = "Forge cost multiplier" }));

            _harmony = new Harmony(PluginGuid);
            _harmony.PatchAll();
            Log.LogInfo($"{PluginName} {PluginVersion} loaded (max players {MaxPlayers.Value})");
        }

        private void OnDestroy() => _harmony?.UnpatchSelf();
    }
}
