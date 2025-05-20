using HarmonyLib;
using MGSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MercenaryTrader
{
    [HarmonyPatch(typeof(FactionSystem), "StrategyUpdate")]
    public static class FactionStrategyOverride
    {
        public static Boolean Prefix(News news, Factions factions, SpaceTime spaceTime, TravelMetadata travelMetadata)
        {
            int rank = 1;
            foreach (Faction value in factions.Values.OrderByDescending((Faction f) => f.Power))
            {
                if (!factions.IsEnabledFaction(value))
                {
                    continue;
                }

                double num = (spaceTime.Time - value.LastStrategyChangedTime).TotalSeconds;
                int num2 = value.NextChangeStrategyHours * 3600;
                if (!(num < (double)num2))
                {
                    FactionStrategy strategy = value.Strategy;
                    while (num >= (double)num2)
                    {
                        value.LastStrategyChangedTime = value.LastStrategyChangedTime.AddSeconds(num2);
                        value.Strategy = selectStrategy(rank, value.Power);
                        num -= (double)num2;
                        value.NextChangeStrategyHours = UnityEngine.Random.Range(value.Record.StrategyDurationMinHours, value.Record.StrategyDurationMaxHours + 1);
                        num2 = value.NextChangeStrategyHours * 3600;
                    }

                    if (strategy != value.Strategy)
                    {
                        NewsEvent newsEvent = new NewsEvent();
                        newsEvent.Factions.Add(value.Id);
                        newsEvent.NewsType = NewsType.FactionStatusChange;
                        newsEvent.Parameters.Add(value.Strategy.ToString());
                        NewsSystem.AddNews(news, spaceTime, travelMetadata, newsEvent);
                    }
                }
                rank++;
            }
            return false;
        }

        public static FactionStrategy selectStrategy(int rank, int power) {
            //for each faction, we want to know their total power and their rank
            // if they're in the bottom 3rd they go on defense.
            // if they're in the top 3rd, and over 3k power go expand.
            // otherwise scout
            if (rank <= 6 && power > 4000) {
                return FactionStrategy.Expansion;
            }
            if (rank > 10)
            {
                return FactionStrategy.Defense;
            }
            return FactionStrategy.Scouting;
        }
    }
}
