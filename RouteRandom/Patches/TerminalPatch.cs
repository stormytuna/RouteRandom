using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RouteRandom.Helpers;
using Random = System.Random;

namespace RouteRandom.Patches
{
    [HarmonyPatch(typeof(Terminal))]
    public class TerminalPatch
    {
        public static TerminalPatch Instance;

        private static readonly TerminalNode noSuitablePlanetsNode = new TerminalNode {
            name = "NoSuitablePlanets",
            displayText = "\nNo suitable planets found.\nConsider route random.\n\n\n",
            clearPreviousText = true
        };
        private static readonly TerminalNode hidePlanetHackNode = new TerminalNode {
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

        private static readonly Random rand = new Random();

        // .DistinctBy doesnt exist in this c# version :(
        private class CompatibleNounComparer : EqualityComparer<CompatibleNoun>
        {
            public override bool Equals(CompatibleNoun x, CompatibleNoun y) {
                var ret = x.result.name.Equals(y.result.name, StringComparison.InvariantCultureIgnoreCase);
                return ret;
            }
            // Not sure why returning obj.GetHashCode() didn't work but this does so...
            public override int GetHashCode(CompatibleNoun obj) => obj.result.GetHashCode();
        }

        [HarmonyPostfix, HarmonyPatch("Awake")]
        public static void AddNewTerminalWords(Terminal __instance) {
            try {
                routeKeyword = __instance.GetKeyword("Route");

                randomKeyword = new TerminalKeyword {
                    word = "random",
                    name = "Random",
                    defaultVerb = routeKeyword,
                };
                randomFilterWeatherKeyword = new TerminalKeyword {
                    word = "randomfilterweather",
                    name = "RandomFilterWeather",
                    defaultVerb = routeKeyword
                };

                routeRandomCompatibleNoun = new CompatibleNoun {
                    noun = randomKeyword,
                    result = new TerminalNode { name = "routeRandom", buyRerouteToMoon = -1 }
                };
                routeRandomFilterWeatherCompatibleNoun = new CompatibleNoun {
                    noun = randomFilterWeatherKeyword,
                    result = new TerminalNode { name = "routeRandomFilterWeather", buyRerouteToMoon = -1 }
                };

                TerminalKeyword moonsKeyword = __instance.GetKeyword("Moons");
                moonsKeyword.specialKeywordResult.displayText += "* Random   //   Routes you to a random moon, regardless of weather conditions\n* RandomFilterWeather   //   Routes you to a random moon, filtering out disallowed weather conditions\n\n";

                __instance.AddKeywords(randomKeyword, randomFilterWeatherKeyword);
                __instance.AddCompatibleNounsToKeyword("Route", routeRandomCompatibleNoun, routeRandomFilterWeatherCompatibleNoun);
            } catch {
                RouteRandomBase.Log.LogError("Failed to add Terminal keywords and compatible nouns!");
            }
        }

        [HarmonyPostfix, HarmonyPatch("ParsePlayerSentence")]
        public static TerminalNode RouteToRandomPlanet(TerminalNode __result, Terminal __instance) {
            bool choseRouteRandom = __result.name == "routeRandom";
            bool choseRouteRandomFilterWeather = __result.name == "routeRandomFilterWeather";
            if (choseRouteRandom || choseRouteRandomFilterWeather) {
                // .Distinct check here as Dine was registered twice for some reason? Didn't bother looking into why :P
                List<CompatibleNoun> routePlanetNodes = routeKeyword.compatibleNouns.Where(noun => noun.ResultIsRealMoon() && noun.ResultIsAffordable()).Distinct(new CompatibleNounComparer()).ToList();

                if (choseRouteRandomFilterWeather) {
                    foreach (CompatibleNoun compatibleNoun in routePlanetNodes.ToList()) {
                        var confirmNode = compatibleNoun.result.GetNodeAfterConfirmation();
                        var moonLevel = StartOfRound.Instance.levels[confirmNode.buyRerouteToMoon];
                        if (!WeatherIsAllowed(moonLevel.currentWeather)) {
                            routePlanetNodes.Remove(compatibleNoun);
                        }
                    }
                }

                if (RouteRandomBase.ConfigDifferentPlanetEachTime.Value) {
                    routePlanetNodes.RemoveAll(rpn => rpn.result.GetNodeAfterConfirmation().NodeRoutesToCurrentOrbitedMoon());
                }

                // Almost never happens, but sanity check
                if (routePlanetNodes.Count <= 0) {
                    return noSuitablePlanetsNode;
                }

                TerminalNode chosenNode = rand.NextFromCollection(routePlanetNodes).result;

                if (RouteRandomBase.ConfigRemoveCostOfCostlyPlanets.Value) {
                    if (TerminalHelper.TryMakeRouteMoonNodeFree(chosenNode, out var freeNode)) {
                        chosenNode = freeNode;
                    }
                }

                if (RouteRandomBase.ConfigHidePlanet.Value) {
                    TerminalNode confirmationNode = chosenNode.GetNodeAfterConfirmation();
                    hidePlanetHackNode.buyRerouteToMoon = confirmationNode.buyRerouteToMoon;
                    hidePlanetHackNode.itemCost = RouteRandomBase.ConfigRemoveCostOfCostlyPlanets.Value ? 0 : confirmationNode.itemCost;
                    return hidePlanetHackNode;
                }

                return RouteRandomBase.ConfigSkipConfirmation.Value ? chosenNode.GetNodeAfterConfirmation() : chosenNode;
            }

            return __result;
        }

        private static bool WeatherIsAllowed(LevelWeatherType weatherType) {
            switch (weatherType) {
                case LevelWeatherType.None:
                    return RouteRandomBase.ConfigAllowMildWeather.Value;
                case LevelWeatherType.DustClouds:
                    return RouteRandomBase.ConfigAllowDustCloudsWeather.Value;
                case LevelWeatherType.Rainy:
                    return RouteRandomBase.ConfigAllowRainyWeather.Value;
                case LevelWeatherType.Stormy:
                    return RouteRandomBase.ConfigAllowStormyWeather.Value;
                case LevelWeatherType.Foggy:
                    return RouteRandomBase.ConfigAllowFoggyWeather.Value;
                case LevelWeatherType.Flooded:
                    return RouteRandomBase.ConfigAllowFloodedWeather.Value;
                case LevelWeatherType.Eclipsed:
                    return RouteRandomBase.ConfigAllowEclipsedWeather.Value;
                default:
                    return false;
            }
        }
    }
}
