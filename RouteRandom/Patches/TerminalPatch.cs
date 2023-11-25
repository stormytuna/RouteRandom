using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

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

        private static readonly Random rand = new Random();

        [HarmonyPostfix, HarmonyPatch("Awake")]
        public static void AddNewTerminalWords(Terminal __instance) {
            try {
                routeKeyword = __instance.terminalNodes.allKeywords.First(kw => kw.name == "Route");

                __instance.terminalNodes.allKeywords = __instance.terminalNodes.allKeywords.AddRangeToArray(new TerminalKeyword[] { randomKeyword, randomWithWeatherKeyword });
                routeKeyword.compatibleNouns = routeKeyword.compatibleNouns.AddRangeToArray(new CompatibleNoun[] {
                    routeRandomCompatibleNoun, routeRandomWithWeatherCompatibleNoun
                });
            } catch {
                RouteRandomBase.Log.LogError("Failed to add 'Random' keyword!");
            }
        }

        [HarmonyPostfix, HarmonyPatch("ParsePlayerSentence")]
        public static TerminalNode RouteToRandomPlanet(TerminalNode __result, Terminal __instance) {
            bool choseRouteRandom = __result.name == routeRandomCompatibleNoun.result.name;
            if (choseRouteRandom || __result.name == routeRandomWithWeatherCompatibleNoun.result.name) {
                // TODO: DRY this out
                // TODO: Keyword to allow all weather
                // TODO: Config for any weather to allow
                // TODO: Config to remove cost of costly planets
                // TODO: Config to ignore costly planets
                // TODO: What if we don't have a moon we can choose randomly?

                // Seems like all actual moons have buyRerouteToMoon set to -2
                List<CompatibleNoun> routePlanetNodes = routeKeyword.compatibleNouns.Where(noun => noun.result.buyRerouteToMoon == -2 && noun.result.itemCost == 0).ToList();

                RouteRandomBase.Log.LogInfo(routePlanetNodes);

                if (choseRouteRandom) {
                    foreach (var routePlanetNode in routePlanetNodes.ToList()) {
                        RouteRandomBase.Log.LogInfo(routePlanetNode.result.name);
                        var weather = RoutePlanetNameToWeatherType(routePlanetNode.result.name, __instance.moonsCatalogueList);
                        if (!WeatherIsAllowed(weather)) {
                            routePlanetNodes.Remove(routePlanetNode);
                        }
                    }
                }

                int randomIndex = rand.Next(routePlanetNodes.Count);
                RouteRandomBase.Log.LogInfo(randomIndex);
                return routePlanetNodes[randomIndex].result;
            }

            return __result;
        }

        private static bool WeatherIsAllowed(LevelWeatherType weatherType) {
            switch (weatherType) {
                case LevelWeatherType.None:
                    return true;
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
    }
}
