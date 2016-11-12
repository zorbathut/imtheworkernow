using RimWorld;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace ImTheWorkerNow
{
    public static class ReservationUtility_Detour
    {
        public static bool CanReserve(this Pawn p, TargetInfo target, int maxPawns = 1)
        {
            if (ITWN.HorrifyingGlobalFakeryToAllowReserve != null && target.Thing == ITWN.HorrifyingGlobalFakeryToAllowReserve)
            {
                return true;
            }
            if (ITWN.HorrifyingGlobalFakeryToPreventReserve != null && target.Thing == ITWN.HorrifyingGlobalFakeryToPreventReserve)
            {
                return false;
            }
            return Find.Reservations.CanReserve(p, target, maxPawns);
        }

        public static bool CanReserveAndReach(this Pawn p, TargetInfo target, PathEndMode peMode, Danger maxDanger, int maxPawns = 1)
        {
            return p.CanReach(target, peMode, maxDanger, false, TraverseMode.ByPawn) && CanReserve(p, target, maxPawns);
        }
    }
}
