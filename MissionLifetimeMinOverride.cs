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
    [HarmonyPatch(typeof(GlobalSettings), "get_MissionMinLifeTimeHours")]
    public static class MissionLifetimeMinOverride {
        public static void Postfix(ref int __result)
        {
            __result = 648;
        }
    }
}
