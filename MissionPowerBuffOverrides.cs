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
    [HarmonyPatch(typeof(ProcMissionRecord), "get_PowerBuff")]
    public static class MissionPowerBoffOverrides {
        public static void Postfix(ProcMissionRecord __instance, ref int __result)
        {
            if (__instance.ProcMissionType == ProceduralMissionType.Sabotage ||
                __instance.ProcMissionType == ProceduralMissionType.Ritual ||
                __instance.ProcMissionType == ProceduralMissionType.Elimination ||
                __instance.ProcMissionType == ProceduralMissionType.Robbery) {
                __result = 350;
            }else if (__instance.ProcMissionType == ProceduralMissionType.Espionage) {
                __result = 150;
            }
            else if (__instance.ProcMissionType == ProceduralMissionType.RaiderCapture)
            {
                __result = 0;
            }
        }
    }
}
