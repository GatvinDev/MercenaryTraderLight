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
    [HarmonyPatch(typeof(ProcMissionRecord), "get_PowerCost")]
    public static class MissionPowerCostOverrides {
        public static void Postfix(ProcMissionRecord __instance, ref int __result)
        {
            if (__instance.ProcMissionType == ProceduralMissionType.RaiderCapture) {
                __result = 500;
            }else if (__instance.ProcMissionType == ProceduralMissionType.Espionage) {
                __result = 50;
            }
        }
    }
}
