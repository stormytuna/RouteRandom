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

        public static ConfigEntry<bool> ConfigAllowDustCloudsWeather;
        public static ConfigEntry<bool> ConfigAllowRainyWeather;
        public static ConfigEntry<bool> ConfigAllowStormyWeather;
        public static ConfigEntry<bool> ConfigAllowFoggyWeather;
        public static ConfigEntry<bool> ConfigAllowFloodedWeather;
        public static ConfigEntry<bool> ConfigAllowEclipsedWeather;

        void Awake() {
            if (Instance is null) {
                Instance = this;
            }

            // TODO: Maybe DRY-ify this?
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


            Log.LogInfo("Route Random has awaken!");

            harmony.PatchAll();
        }
    }
}
