using HarmonyLib;
using MGSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MercenaryTrader
{
    [HarmonyPatch(typeof(QuestlineSystem), "ApplyQuestlineStep")]
    public static class FactionStrategyOverride
    {

        public static Boolean Prefix(FactionQuestlineRecord nextRecord, string previousId, SpaceTime spaceTime, Faction faction, StoryTriggers storyTriggers, Factions factions, Stations stations)
        {


            faction.QuestlineId = nextRecord.Id;
            if (faction.QuestlineId.Contains("endgame_loop"))
            {
                int rank = 1;
                foreach (Faction value in factions.Values.OrderByDescending((Faction f) => f.Power))
                {
                    if (!factions.IsEnabledFaction(value)) { continue; }
                    if (value.Id.Equals(faction.Id)) { break; }
                    rank++;
                }
                Debug.Log("Setting Strategy for: " + faction.Id);
                Debug.Log("Rank: " + rank);
                Debug.Log("Power: " + faction.Power);
                faction.CurrentStrategy = selectStrategy(rank, faction.Power);
            }
            else
            {
                faction.CurrentStrategy = DropManager.GenerateDrop(nextRecord.Strategies, null);
            }
            
            FactionStrategyRecord record = Data.FactionStrategies.GetRecord(faction.CurrentStrategy);
            storyTriggers.TimeEventRemove(previousId);
            storyTriggers.TimeEventAdd(nextRecord.Id, spaceTime.Time);
            if (record.FailConditions.Count > 0 && QuestlineSystem.IsConditionsPassed(factions, stations, spaceTime, storyTriggers, faction, record.FailConditions))
            {
                QuestlineSystem.AdvanceQuestline(spaceTime, faction, factions, storyTriggers, stations, success: false);
            }
            else
            {
                QuestlineSystem.PrepareProgressData(faction, spaceTime);
            }
            return false;
        }

        public static String selectStrategy(int rank, int power) {
            //for each faction, we want to know their total power and their rank
            // if they're in the bottom 3rd they go on defense.
            // if they're in the top 3rd, and over 3k power go expand.
            // otherwise scout
            if (rank <= 6 && power > 4000) {
                return "Expansion";
            }
            if (rank > 10)
            {
                return "Defense";
            }
            return "Scouting";
        }
    }
}
