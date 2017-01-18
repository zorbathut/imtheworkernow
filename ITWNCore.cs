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
    public static class ITWN
    {
        public static void PostMenuOption(
            List<FloatMenuOption> list,
            Pawn pawn,
            Thing target,
            string title,
            Action action,
            MenuOptionPriority priority = MenuOptionPriority.Default,
            Action mouseoverGuiAction = null,
            Thing revalidateClickTarget = null,
            float extraPartWidth = 0,
            Func<Rect, bool> extraPartOnGUI = null,
            WorkTypeDef workType = null)
        {
            Action handler = action;
            if (action != null && !pawn.CanReserve(target, 1))
            {
                Pawn reserver = pawn.Map.reservationManager.FirstReserverOf(target, pawn.Faction, true);
                if (reserver == null)
                {
                    // We can't reserve this for some reason, but we also can't kick the current reserver off, so . . . sucks to be you, I guess?
                    Log.Error(string.Format("Inconsistent reservation info for object {0}; {1} can't reserve it, but nobody else seems to be reserving it (confusing). May be a bug with Hospitality, may not be. Punting on the entire issue; please report to the developer of I'm The Worker Now. Thanks!", target, pawn));
                    return;
                }
                else
                {
                    title = title + " (" + "ReservedBy".Translate(new object[] { reserver.LabelShort }) + ")";
                    handler = delegate
                    {
                        Pawn currentReserver = pawn.Map.reservationManager.FirstReserverOf(target, pawn.Faction, true);
                        if (currentReserver != null && currentReserver.jobs != null)
                        {
                            try
                            {
                                ITWN.HorrifyingGlobalFakeryToPreventReserve = target;
                                currentReserver.jobs.EndCurrentJob(JobCondition.InterruptForced);
                            }
                            finally
                            {
                                ITWN.HorrifyingGlobalFakeryToPreventReserve = null;
                            }
                        }
                        Pawn newReserver = pawn.Map.reservationManager.FirstReserverOf(target, pawn.Faction, true);
                        if (newReserver != null)
                        {
                            Log.Error(string.Format("Something went wrong taking over the job {0} on object {1}! Old reserver was {2}, current reserver is {3} (doing job {5}), intended reserver is {4}. {4} will probably not do the job. We apologize for this inconvenience. Please report to the developer of I'm The Worker Now. Thanks!", workType, target, currentReserver, newReserver, pawn, currentReserver.CurJob));
                            return;
                        }
                        else
                        {
                            action();
                        }
                    };
                }
            }

            if (workType != null && pawn.workSettings.GetPriority(workType) == 0)
            {
                title += string.Format(" (not assigned to {0})", workType.gerundLabel);
            }

            if (!list.Any((FloatMenuOption op) => op.Label == title.TrimEnd(new char[0])))
            {
                list.Add(new FloatMenuOption(title, handler, priority, mouseoverGuiAction, revalidateClickTarget, extraPartWidth, extraPartOnGUI));
            }
        }

        public static Thing HorrifyingGlobalFakeryToAllowReserve = null;
        public static Thing HorrifyingGlobalFakeryToPreventReserve = null;
    }
}
