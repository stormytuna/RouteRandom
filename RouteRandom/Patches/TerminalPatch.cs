using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RouteRandom.Helpers;
using Random = System.Random;

namespace RouteRandom.Patches;

[HarmonyPatch(typeof(Terminal))]
public class TerminalPatch
{
    private static readonly TerminalNode noSuitablePlanetsNode = new() {
        name = "NoSuitablePlanets",
        displayText = "\nNo suitable planets found.\nConsider route random.\n\n\n",
        clearPreviousText = true
    };

    private static readonly TerminalNode hidePlanetHackNode = new() {
        name = "HidePlanetHack",
        displayText = "\nRouting autopilot to [REDACTED].\nYour new balance is [playerCredits].\n\nPlease enjoy your flight.\n\n\n",
        clearPreviousText = true
        // buyRerouteToMoon and itemCost fields are set on the fly before returning this node
        // Least obtrusive way I could find to hide the route chosen but still actually go there
    };

    private static TerminalKeyword routeKeyword;

    private static TerminalKeyword randomKeyword;
    private static TerminalKeyword randomFilterWeatherKeyword;

    private static CompatibleNoun routeRandomCompatibleNoun;
    private static CompatibleNoun routeRandomFilterWeatherCompatibleNoun;

    private static readonly Random rand = new();

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Terminal.Awake))]
    public static void AddNewTerminalWords(Terminal __instance) {
        try {
            routeKeyword = __instance.GetKeyword("Route");

            randomKeyword = new TerminalKeyword {
                word = "random",
                name = "Random",
                defaultVerb = routeKeyword,
                compatibleNouns = Array.Empty<CompatibleNoun>()
            };
            randomFilterWeatherKeyword = new TerminalKeyword {
                word = "randomfilterweather",
                name = "RandomFilterWeather",
                defaultVerb = routeKeyword,
                compatibleNouns = Array.Empty<CompatibleNoun>()
            };

            routeRandomCompatibleNoun = new CompatibleNoun {
                noun = randomKeyword,
                result = new TerminalNode {
                    name = "routeRandom",
                    buyRerouteToMoon = -1,
                    terminalOptions = Array.Empty<CompatibleNoun>()
                }
            };
            routeRandomFilterWeatherCompatibleNoun = new CompatibleNoun {
                noun = randomFilterWeatherKeyword,
                result = new TerminalNode {
                    name = "routeRandomFilterWeather",
                    buyRerouteToMoon = -1,
                    terminalOptions = Array.Empty<CompatibleNoun>()
                }
            };

            TerminalKeyword moonsKeyword = __instance.GetKeyword("Moons");
            moonsKeyword.specialKeywordResult.displayText +=
                "* Random   //   Routes you to a random moon, regardless of weather conditions\n* RandomFilterWeather   //   Routes you to a random moon, filtering out disallowed weather conditions\n\n";

            __instance.AddKeywords(randomKeyword, randomFilterWeatherKeyword);
            __instance.AddCompatibleNounsToKeyword("Route", routeRandomCompatibleNoun, routeRandomFilterWeatherCompatibleNoun);
        } catch (Exception e) {
            RouteRandomBase.Log.LogError("Failed to add Terminal keywords and compatible nouns!");
            RouteRandomBase.Log.LogError(e);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Terminal.ParsePlayerSentence))]
    public static TerminalNode RouteToRandomPlanet(TerminalNode __result, Terminal __instance) {
        if (__result is null || __instance is null) {
            RouteRandomBase.Log.LogInfo($"Terminal node was null? ({__result is null})");
            RouteRandomBase.Log.LogInfo($"Terminal was null? ({__instance is null})");
            return __result;
        }

        bool choseRouteRandom = __result.name == "routeRandom";
        bool choseRouteRandomFilterWeather = __result.name == "routeRandomFilterWeather";
        if (!choseRouteRandom && !choseRouteRandomFilterWeather) {
            RouteRandomBase.Log.LogInfo($"Didn't choose random or randomfilterweather (chose {__result.name})");
            return __result;
        }

        // .Distinct check here as Dine was registered twice for some reason? Didn't bother looking into why :P
        List<CompatibleNoun> routePlanetNodes = routeKeyword.compatibleNouns.Where(noun => noun.ResultIsRealMoon() && noun.ResultIsAffordable()).Distinct(new CompatibleNounComparer()).ToList();
        RouteRandomBase.Log.LogInfo($"Moons before filtering: {routePlanetNodes.Count}");

        if (choseRouteRandomFilterWeather) {
            foreach (CompatibleNoun compatibleNoun in routePlanetNodes.ToList()) {
                TerminalNode confirmNode = compatibleNoun.result.GetNodeAfterConfirmation();
                SelectableLevel moonLevel = StartOfRound.Instance.levels[confirmNode.buyRerouteToMoon];
                if (!WeatherIsAllowed(moonLevel.currentWeather)) {
                    routePlanetNodes.Remove(compatibleNoun);
                }
            }

            RouteRandomBase.Log.LogInfo($"Moons after filtering weather: {routePlanetNodes.Count}");
        }


        if (RouteRandomBase.ConfigDifferentPlanetEachTime.Value) {
            routePlanetNodes.RemoveAll(rpn => rpn.result.GetNodeAfterConfirmation().NodeRoutesToCurrentOrbitedMoon());
            RouteRandomBase.Log.LogInfo($"Moons after filtering orbited moon: {routePlanetNodes.Count}");
        }

        // Almost never happens, but sanity check
        if (routePlanetNodes.Count <= 0) {
            RouteRandomBase.Log.LogInfo("No suitable moons found D:");
            return noSuitablePlanetsNode;
        }

        TerminalNode chosenNode = rand.NextFromCollection(routePlanetNodes).result;
        RouteRandomBase.Log.LogInfo($"Chosen moon: {chosenNode.name}");

        if (RouteRandomBase.ConfigRemoveCostOfCostlyPlanets.Value) {
            if (TerminalHelper.TryMakeRouteMoonNodeFree(chosenNode, out TerminalNode freeNode)) {
                chosenNode = freeNode;
            }

            RouteRandomBase.Log.LogInfo("Made moon free!");
        }

        if (RouteRandomBase.ConfigHidePlanet.Value) {
            TerminalNode confirmationNode = chosenNode.GetNodeAfterConfirmation();
            hidePlanetHackNode.buyRerouteToMoon = confirmationNode.buyRerouteToMoon;
            hidePlanetHackNode.itemCost = RouteRandomBase.ConfigRemoveCostOfCostlyPlanets.Value ? 0 : confirmationNode.itemCost;
            RouteRandomBase.Log.LogInfo("Hidden moon!");
            return hidePlanetHackNode;
        }

        return RouteRandomBase.ConfigSkipConfirmation.Value ? chosenNode.GetNodeAfterConfirmation() : chosenNode;
    }

    private static bool WeatherIsAllowed(LevelWeatherType weatherType) {
        return weatherType switch {
            LevelWeatherType.None => RouteRandomBase.ConfigAllowMildWeather.Value,
            LevelWeatherType.DustClouds => RouteRandomBase.ConfigAllowDustCloudsWeather.Value,
            LevelWeatherType.Rainy => RouteRandomBase.ConfigAllowRainyWeather.Value,
            LevelWeatherType.Stormy => RouteRandomBase.ConfigAllowStormyWeather.Value,
            LevelWeatherType.Foggy => RouteRandomBase.ConfigAllowFoggyWeather.Value,
            LevelWeatherType.Flooded => RouteRandomBase.ConfigAllowFloodedWeather.Value,
            LevelWeatherType.Eclipsed => RouteRandomBase.ConfigAllowEclipsedWeather.Value,
            _ => false
        };
    }
}

internal class CompatibleNounComparer : EqualityComparer<CompatibleNoun>
{
    public override bool Equals(CompatibleNoun x, CompatibleNoun y) => x?.result.name.Equals(y?.result.name, StringComparison.InvariantCultureIgnoreCase) ?? false;

    public override int GetHashCode(CompatibleNoun obj) => obj.result.GetHashCode();
}
