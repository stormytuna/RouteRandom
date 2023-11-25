using System;
using System.Linq;
using HarmonyLib;

namespace RouteRandom.Helpers
{
    public static class TerminalHelper
    {
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
    
        public static bool ResultIsRealMoon(this CompatibleNoun compatibleNoun) {
            return compatibleNoun.result.buyRerouteToMoon == -2;
        }

        public static bool ResultIsAffordable(this CompatibleNoun compatibleNoun) {
            // TODO: Config to remove cost of costly planets
            // TODO: Config to ignore costly planets
            return compatibleNoun.result.itemCost <= 0;
        }
    }
}
