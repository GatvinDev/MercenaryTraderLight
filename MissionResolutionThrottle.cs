using HarmonyLib;
using MGSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MercenaryTrader
{
    [HarmonyPatch(typeof(MissionSystem), "Update")]
    public static class MissionResolutionThrottle {
        public static Boolean Prefix(Missions missions, Stations stations, News news, SpaceTime spaceTime, PopulationDebugData populationDebugData, TravelMetadata travelMetadata, Factions factions, ItemsPrices itemsPrices, Difficulty difficulty)
        {
            Dictionary<string, float> controlMap = new Dictionary<string, float>();
            int num = missions.Values.Count - 1;
            while (num >= 0 && missions.Values.Count != 0)
            {
                Mission mission = missions.Values[num];
                if (!(mission.ExpireTime > spaceTime.Time))
                {
                    if (mission.IsStoryMission)
                    {
                        MissionSystem.RemoveMission(missions, mission.StationId);
                    }
                    else
                    {
                        Station station = stations.Get(mission.StationId);
                        Faction faction = factions.Get(mission.BeneficiaryFactionId);
                        Faction faction2 = factions.Get(mission.VictimFactionId);
                        float f1Control = GetControlMultiplier(faction,ref controlMap, stations);
                        float f2Control = GetControlMultiplier(faction2,ref controlMap, stations);
                        float num2 = (float)faction.Power / 100f / 100f * f1Control;
                        float num3 = (float)faction2.Power / 100f / 100f * f2Control;
                        float num4 = Mathf.Min(0.99f,Mathf.Max(0.01f,station.Record.CaptureChance + num2 - num3));
                        bool num5 = UnityEngine.Random.Range(0f, 1f) < num4;
                        NewsEvent newsEvent = new NewsEvent
                        {
                            Factions = { faction.Id, faction2.Id },
                            Parameters = { station.Id }
                        };
                        if (num5)
                        {
                            ProcMissionRecord procMissionRecord = Data.ProcMissions.Get(mission.ProcMissionType);
                            newsEvent.NewsType = procMissionRecord.NewsTypeEndGood;
                            MissionSystem.ProcessMissionSuccessActions(stations, spaceTime, populationDebugData, travelMetadata, factions, itemsPrices, difficulty, mission);
                        }
                        else
                        {
                            ProcMissionRecord procMissionRecord2 = Data.ProcMissions.Get(mission.ProcMissionType);
                            newsEvent.NewsType = procMissionRecord2.NewsTypeEndFail;
                            MissionSystem.ProcessMissionFailureActions(stations, spaceTime, travelMetadata, factions, mission);
                            if (travelMetadata.CurrentSpaceObject.Equals(station.SpaceObjectId) && !travelMetadata.IsInTravel)
                            {
                                UI.Get<SpaceHudScreen>().RefreshUIOnArrival(travelMetadata.CurrentSpaceObject);
                            }
                        }

                        NewsSystem.AddNews(news, spaceTime, travelMetadata, newsEvent);
                        MissionSystem.RemoveMission(missions, mission.StationId);
                    }
                }

                num--;
            }
            return false;
        }

        public static float GetControlMultiplier(Faction faction, ref Dictionary<string, float> controlMap, Stations stations) {
            //if (!controlMap.ContainsKey(faction.Id)) {
                int hqCount = 0;
                int assetCount = 0;
                foreach (Station s in stations.Values)
                {
                    if (s.OwnerFactionId.Equals(faction.Id))
                    {
                        assetCount++;
                        if (s.UncapturableByDefault)
                        {
                            hqCount++;
                        }
                    }
                }
                controlMap[faction.Id] = (faction.CurrentTechLevel + hqCount) / (2*Mathf.Max(0.5f, assetCount - (faction.CurrentTechLevel + hqCount)));
                //controlMap[faction.Id] = (faction.CurrentTechLevel) / Mathf.Max(1f, assetCount);
            //}
            return controlMap[faction.Id];
        }
    }
}
