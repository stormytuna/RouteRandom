using HarmonyLib;
using TMPro;
using UnityEngine.Video;

namespace RouteRandom.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    public class StartOfRoundPatch
    {
        [HarmonyPostfix, HarmonyPatch("SetMapScreenInfoToCurrentLevel")]
        public static void HideMapScreenInfo(VideoPlayer ___screenLevelVideoReel, TextMeshProUGUI ___screenLevelDescription) {
            if (RouteRandomBase.ConfigHidePlanet.Value) {
                ___screenLevelVideoReel.enabled = false;
                ___screenLevelDescription.text = "Orbiting: [REDACTED]\nPopulation: Unknown\nConditions: Unknown\nFauna: Unknown\nWeather: Unknown";
            }
        }
    }
}
