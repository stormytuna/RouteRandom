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
        public const string ModVersion = "1.2.1";

        public static ManualLogSource Log = BepInEx.Logging.Logger.CreateLogSource(ModGUID);
        public static RouteRandomBase Instance;

        private readonly Harmony harmony = new Harmony(ModGUID);

        private void Awake() {
            if (Instance is null) {
                Instance = this;
            }

            Log.LogInfo("Route Random has awoken!");

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
        public static ConfigEntry<bool> ConfigRemoveCostOfCostlyPlanets;
        public static ConfigEntry<bool> ConfigSkipConfirmation;
        public static ConfigEntry<bool> ConfigDifferentPlanetEachTime;
        public static ConfigEntry<bool> ConfigHidePlanet;

        private void LoadConfigs() {
            ConfigAllowMildWeather = Config.Bind("Allowed Weathers",
                                                 "AllowMildWeather",
                                                 true,
                                                 "Whether or not to allow the 'Mild' weather to be chosen by the 'route randomfilterweather' command");

            ConfigAllowDustCloudsWeather = Config.Bind("Allowed Weathers",
                                                       "AllowDustCloudsWeather",
                                                       false,
                                                       "Whether or not to allow the 'Dust Clouds' weather to be chosen by the 'route randomfilterweather' command");

            ConfigAllowRainyWeather = Config.Bind("Allowed Weathers",
                                                       "AllowRainyWeather",
                                                       false,
                                                       "Whether or not to allow the 'Rainy' weather to be chosen by the 'route randomfilterweather' command");

            ConfigAllowStormyWeather = Config.Bind("Allowed Weathers",
                                                       "AllowStormyWeather",
                                                       false,
                                                       "Whether or not to allow the 'Stormy' weather to be chosen by the 'route randomfilterweather' command");

            ConfigAllowFoggyWeather = Config.Bind("Allowed Weathers",
                                                       "AllowFoggyWeather",
                                                       false,
                                                       "Whether or not to allow the 'Foggy' weather to be chosen by the 'route randomfilterweather' command");

            ConfigAllowFloodedWeather = Config.Bind("Allowed Weathers",
                                                       "AllowFloodedWeather",
                                                       false,
                                                       "Whether or not to allow the 'Flooded' weather to be chosen by the 'route randomfilterweather' command");

            ConfigAllowEclipsedWeather = Config.Bind("Allowed Weathers",
                                                       "AllowEclipsedWeather",
                                                       false,
                                                       "Whether or not to allow the 'Eclipsed' weather to be chosen by the 'route randomfilterweather' command");

            ConfigAllowCostlyPlanets = Config.Bind("Costly Planets",
                                                   "AllowCostlyPlanets",
                                                   false,
                                                   "Whether or not to allow costly planets (85-Rend, 7-Dine, 8-Titan). NOTE: You will still be prompted to pay the fee to fly there, enable the MakeCostlyPlanetsFree option to avoid that");

            ConfigRemoveCostOfCostlyPlanets = Config.Bind("Costly Planets",
                                                          "RemoveCostOfCostlyPlanets",
                                                          false,
                                                          "Whether or not to remove the cost of costly planets when they're chosen randomly and allows them to be chosen even when AllowCostlyPlanets is false");

            ConfigSkipConfirmation = Config.Bind("General",
                                                 "SkipConfirmation",
                                                 false,
                                                 "Whether or not to skip the confirmation screen when using 'route random' or 'route randomwithweather' commands");

            ConfigDifferentPlanetEachTime = Config.Bind("General",
                                                "DifferentPlanetEachTime",
                                                false,
                                                "Prevents 'route random' and 'route randomwithweather' commands from choosing the same planet you're on");

            ConfigHidePlanet = Config.Bind("General",
                                           "HidePlanet",
                                           false,
                                           "Hides the planet you get randomly routed to, both in the terminal response and at the helm. NOTE: This will ALWAYS hide the orbited planet (even when selected manually) and will skip the confirmation screen");
        }

        #endregion
    }
}
