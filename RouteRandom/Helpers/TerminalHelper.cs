using System;
using System.Linq;
using HarmonyLib;

namespace RouteRandom.Helpers
{
    public static class TerminalHelper
    {
        public static TerminalKeyword GetKeyword(this Terminal terminal, string keywordName) => terminal.terminalNodes.allKeywords.First(kw => kw.name == keywordName); 

        public static void AddKeyword(this Terminal terminal, TerminalKeyword newKeyword) => terminal.terminalNodes.allKeywords = terminal.terminalNodes.allKeywords.AddToArray(newKeyword);

        public static void AddKeywords(this Terminal terminal, params TerminalKeyword[] newKeywords) {
            foreach (TerminalKeyword newKeyword in newKeywords) {
                terminal.AddKeyword(newKeyword);
            }
        }

        public static void AddCompatibleNounToKeyword(this Terminal terminal, string keywordName, CompatibleNoun newCompatibleNoun) {
            TerminalKeyword keyword = terminal.terminalNodes.allKeywords.FirstOrDefault(kw => kw.name == keywordName) ?? throw new ArgumentException($"Failed to find keyword with name {keywordName}");
            keyword.compatibleNouns = keyword.compatibleNouns.AddToArray(newCompatibleNoun);
        }

        public static void AddCompatibleNounsToKeyword(this Terminal terminal, string keywordName, params CompatibleNoun[] newCompatibleNouns) {
            foreach (CompatibleNoun newCompatibleNoun in newCompatibleNouns) {
                terminal.AddCompatibleNounToKeyword(keywordName, newCompatibleNoun);
            }
        }

        public static bool ResultIsRealMoon(this CompatibleNoun compatibleNoun) => compatibleNoun.result.buyRerouteToMoon == -2;

        public static bool ResultIsAffordable(this CompatibleNoun compatibleNoun) => compatibleNoun.result.itemCost <= 0 || RouteRandomBase.ConfigAllowCostlyPlanets.Value || RouteRandomBase.ConfigRemoveCostOfCostlyPlanets.Value;

        public static TerminalNode MakeRouteMoonNodeFree(TerminalNode routeMoonNode, string name) {
            return new TerminalNode {
                name = name,
                buyRerouteToMoon = -2,
                clearPreviousText = true,
                displayPlanetInfo = routeMoonNode.displayPlanetInfo,
                displayText = routeMoonNode.displayText,
                itemCost = 0,
                overrideOptions = true,
                terminalOptions = routeMoonNode.terminalOptions
            };
        }
    }
}
