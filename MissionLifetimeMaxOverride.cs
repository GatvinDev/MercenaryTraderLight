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
    [HarmonyPatch(typeof(GlobalSettings), "get_MissionMaxLifeTimeHours")]
    public static class MissionLifetimeMaxOverride {
        public static void Postfix(ref int __result)
        {
            __result = 1920;
        }
    }
}
