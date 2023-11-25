using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace RouteRandom
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class RouteRandomBase : BaseUnityPlugin
    {
        public const string ModGUID = "stormytuna.RouteRandom";
        public const string ModName = "Route Random";
        public const string ModVersion = "1.0.0";

        public static ManualLogSource Log = BepInEx.Logging.Logger.CreateLogSource(ModGUID);
        public static RouteRandomBase Instance;

        private readonly Harmony harmony = new Harmony(ModGUID);

        void Awake() {
            if (Instance is null) {
                Instance = this;
            }

            Log.LogInfo("Route Random has awaken!");

            LoadConfigs();

            harmony.PatchAll();
        }

        #region Config

        public static ConfigEntry<bool> ConfigAllowMildWeather;
        public static ConfigEntry<bool> ConfigAllowDustCloudsWeather;
        public static ConfigEntry<bool> ConfigAllowRainyWeather;
        public static ConfigEntry<bool> ConfigAllowStormyWeather;
        public static ConfigEntry<bool> ConfigAllowFoggyWeather;
        public static ConfigEntry<bool> ConfigAllowFloodedWeather;
        public static ConfigEntry<bool> ConfigAllowEclipsedWeather;
        public static ConfigEntry<bool> ConfigAllowCostlyPlanets;

        private void LoadConfigs() {
            ConfigAllowMildWeather = Config.Bind("Allowed Weathers",
                                                 "AllowMildWeather",
                                                 true,
                                                 "Whether or not to allow the 'Mild' weather to be chosen by the 'route random' command");

            ConfigAllowDustCloudsWeather = Config.Bind("Allowed Weathers",
                                                       "AllowDustCloudsWeather",
                                                       false,
                                                       "Whether or not to allow the 'Dust Clouds' weather to be chosen by the 'route random' command");

            ConfigAllowRainyWeather = Config.Bind("Allowed Weathers",
                                                       "AllowRainyWeather",
                                                       false,
                                                       "Whether or not to allow the 'Rainy' weather to be chosen by the 'route random' command");

            ConfigAllowStormyWeather = Config.Bind("Allowed Weathers",
                                                       "AllowStormyWeather",
                                                       false,
                                                       "Whether or not to allow the 'Stormy' weather to be chosen by the 'route random' command");

            ConfigAllowFoggyWeather = Config.Bind("Allowed Weathers",
                                                       "AllowFoggyWeather",
                                                       false,
                                                       "Whether or not to allow the 'Foggy' weather to be chosen by the 'route random' command");

            ConfigAllowFloodedWeather = Config.Bind("Allowed Weathers",
                                                       "AllowFloodedWeather",
                                                       false,
                                                       "Whether or not to allow the 'Flooded' weather to be chosen by the 'route random' command");

            ConfigAllowEclipsedWeather = Config.Bind("Allowed Weathers",
                                                       "AllowEclipsedWeather",
                                                       false,
                                                       "Whether or not to allow the 'Eclipsed' weather to be chosen by the 'route random' command");

            ConfigAllowCostlyPlanets = Config.Bind("General",
                                                   "AllowCostlyPlanets",
                                                   false,
                                                   "Whether or not to allow costly planets (85-Rend, 7-Dine, 8-Titan). NOTE: You will still be prompted to pay the fee to fly there, enable the MakeCostlyPlanetsFree option to avoid that");
        }

        #endregion
    }
}
