using BepInEx;
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

            harmony.PatchAll();
        }
    }
}
