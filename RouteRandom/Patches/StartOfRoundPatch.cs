using HarmonyLib;
using TMPro;
using UnityEngine.Video;

namespace RouteRandom.Patches;

[HarmonyPatch(typeof(StartOfRound))]
public class StartOfRoundPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(StartOfRound.SetMapScreenInfoToCurrentLevel))]
    public static void HideMapScreenInfo(StartOfRound __instance, VideoPlayer ___screenLevelVideoReel, TextMeshProUGUI ___screenLevelDescription) {
        if (__instance.currentLevel.name == "CompanyBuildingLevel" || !RouteRandomBase.ConfigHidePlanet.Value) {
            return;
        }

        ___screenLevelDescription.text = "Orbiting: [REDACTED]\nPopulation: Unknown\nConditions: Unknown\nFauna: Unknown\nWeather: Unknown";
        ___screenLevelVideoReel.enabled = false;
        // For some reason just setting .enabled to false here didn't work, so we also undo the other stuff it sets
        ___screenLevelVideoReel.clip = null;
        ___screenLevelVideoReel.gameObject.SetActive(false);
        ___screenLevelVideoReel.Stop();
    }
}
