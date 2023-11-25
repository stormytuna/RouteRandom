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

        private static readonly Random rand = new Random();

        private static readonly TerminalKeyword randomKeyword = new TerminalKeyword {
            word = "random",
            name = "Random",
            defaultVerb = routeKeyword
        };

        private static readonly TerminalNode routeRandomNode = new TerminalNode {
            name = "routeRandom"
        };

        [HarmonyPostfix, HarmonyPatch("Awake")]
        public static void AddNewTerminalWords(Terminal __instance) {
            try {
                routeKeyword = __instance.terminalNodes.allKeywords.First(kw => kw.name == "Route");

                __instance.terminalNodes.allKeywords = __instance.terminalNodes.allKeywords.AddToArray(randomKeyword);
                routeKeyword.compatibleNouns = routeKeyword.compatibleNouns.AddToArray(new CompatibleNoun {
                    noun = randomKeyword,
                    result = routeRandomNode
                });
            } catch {
                RouteRandomBase.Log.LogError("Failed to add 'Random' keyword!");
            }
        }

        [HarmonyPostfix, HarmonyPatch("ParsePlayerSentence")]
        public static TerminalNode RouteToRandomPlanet(TerminalNode __result, Terminal __instance) {
            if (__result.name == routeRandomNode.name) {
                // TODO: DRY this out
                // TODO: Keyword to ignore all weather
                // TODO: Config for any weather to ignore always
                // TODO: Config to remove cost of costly planets
                // TODO: Config to ignore costly planets

                // Seems like all actual moons have buyRerouteToMoon set to -2
                List<CompatibleNoun> routePlanetNodes = routeKeyword.compatibleNouns.Where(noun => noun.result.buyRerouteToMoon == -2 && noun.result.itemCost == 0).ToList();

                int randomIndex = rand.Next(routePlanetNodes.Count);
                return routePlanetNodes[randomIndex].result;
            }

            return __result;
        }
    }
}
