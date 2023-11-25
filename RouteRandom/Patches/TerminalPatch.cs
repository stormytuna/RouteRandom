using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RouteRandom.Helpers;

namespace RouteRandom.Patches
{
    [HarmonyPatch(typeof(Terminal))]
    public class TerminalPatch
    {
        private static TerminalKeyword routeKeyword;

        private static readonly TerminalKeyword randomKeyword = new TerminalKeyword {
            word = "random",
            name = "Random",
            defaultVerb = routeKeyword
        };
        private static readonly TerminalKeyword randomWithWeatherKeyword = new TerminalKeyword {
            word = "randomwithweather",
            name = "RandomWithWeather",
            defaultVerb = routeKeyword
        };

        private static readonly CompatibleNoun routeRandomCompatibleNoun = new CompatibleNoun {
            noun = randomKeyword,
            result = new TerminalNode { name = "routeRandom" }
        };
        private static readonly CompatibleNoun routeRandomWithWeatherCompatibleNoun = new CompatibleNoun {
            noun = randomWithWeatherKeyword,
            result = new TerminalNode { name = "routeRandomWithWeather" }
        };

        private static readonly TerminalNode noSuitablePlanetsNode = new TerminalNode {
            name = "NoSuitablePlanets",
            displayText = "\nNo suitable planets found...\nConsider route randomwithweather.\n\n",
            clearPreviousText = true
        };
        private static TerminalNode routeRendNodeFree;
        private static TerminalNode routeDineNodeFree;
        private static TerminalNode routeTitanNodeFree;

        private static readonly Random rand = new Random();

        [HarmonyPostfix, HarmonyPatch("Awake")]
        public static void AddNewTerminalWords(Terminal __instance) {
            try {
                routeKeyword = __instance.terminalNodes.allKeywords.First(kw => kw.name == "Route");

                TerminalNode routeRendNode = routeKeyword.compatibleNouns.First(cn => cn.result.name == "85route").result;
                routeRendNodeFree = TerminalHelper.MakeRouteMoonNodeFree(routeRendNode, "85routefree");
                TerminalNode routeDineNode = routeKeyword.compatibleNouns.First(cn => cn.result.name == "7route").result;
                routeDineNodeFree = TerminalHelper.MakeRouteMoonNodeFree(routeDineNode, "7routefree");
                TerminalNode routeTitanNode = routeKeyword.compatibleNouns.First(cn => cn.result.name == "8route").result;
                routeTitanNodeFree = TerminalHelper.MakeRouteMoonNodeFree(routeTitanNode, "8routefree");

                __instance.AddKeywords(randomKeyword, randomWithWeatherKeyword);
                __instance.AddCompatibleNounsToKeyword("Route", routeRandomCompatibleNoun, routeRandomWithWeatherCompatibleNoun);
            } catch {
                RouteRandomBase.Log.LogError("Failed to add Terminal keywords and compatible nouns!");
            }
        }
        
        [HarmonyPostfix, HarmonyPatch("ParsePlayerSentence")]
        public static TerminalNode RouteToRandomPlanet(TerminalNode __result, Terminal __instance) {
            bool choseRouteRandom = __result.name == routeRandomCompatibleNoun.result.name;
            if (choseRouteRandom || __result.name == routeRandomWithWeatherCompatibleNoun.result.name) {
                // TODO: Add the 'random' and 'randomwithweather' strings to the 'moons' command screen

                List<CompatibleNoun> routePlanetNodes = routeKeyword.compatibleNouns.Where(noun => noun.ResultIsRealMoon() && noun.ResultIsAffordable()).ToList();

                if (choseRouteRandom) {
                    foreach (CompatibleNoun routePlanetNode in routePlanetNodes.ToList()) {
                        LevelWeatherType weather = RoutePlanetNameToWeatherType(routePlanetNode.result.name, __instance.moonsCatalogueList);
                        if (!WeatherIsAllowed(weather)) {
                            routePlanetNodes.Remove(routePlanetNode);
                        }
                    }
                }

                // Almost never happens, but sanity check
                if (routePlanetNodes.Count <= 0) {
                    RouteRandomBase.Log.LogMessage("Couldn't find a planet with suitable weather!");
                    return noSuitablePlanetsNode;
                }

                TerminalNode chosenNode = rand.NextFromCollection(routePlanetNodes).result;

                return RouteRandomBase.ConfigRemoveCostOfCostlyPlanets.Value ? TryGetFreeNodeForCostlyPlanetNode(chosenNode) : chosenNode;
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

        private static LevelWeatherType RoutePlanetNameToWeatherType(string routePlanetName, SelectableLevel[] moonCatalogue) {
            switch (routePlanetName) {
                case "220route":
                    return moonCatalogue[1].currentWeather;
                case "56route":
                    return moonCatalogue[2].currentWeather;
                case "21route":
                    return moonCatalogue[3].currentWeather;
                case "61route":
                    return moonCatalogue[4].currentWeather;
                case "85route":
                    return moonCatalogue[5].currentWeather;
                case "7route":
                    return moonCatalogue[6].currentWeather;
                case "8route":
                    return moonCatalogue[7].currentWeather;
                default:
                    return LevelWeatherType.None;
            }
        }

        private static TerminalNode TryGetFreeNodeForCostlyPlanetNode(TerminalNode node) {
            switch (node.name) {
                case "85route":
                    return routeRendNodeFree;
                case "7route":
                    return routeDineNodeFree;
                case "8route":
                    return routeTitanNodeFree;
                default:
                    return node;
            }
        }
    }
}
