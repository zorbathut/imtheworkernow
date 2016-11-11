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
            MenuOptionPriority priority = MenuOptionPriority.Medium,
            Action mouseoverGuiAction = null,
            Thing revalidateClickTarget = null,
            float extraPartWidth = 0,
            Func<Rect, bool> extraPartOnGUI = null)
        {
            Action handler = action;
            if (!pawn.CanReserve(target, 1))
            {
                Pawn reserver = Find.Reservations.FirstReserverOf(target, pawn.Faction, true);
                title = title + " (" + "ReservedBy".Translate(new object[] { reserver.LabelShort }) + ")";
                handler = delegate
                {
                    Pawn currentReserver = Find.Reservations.FirstReserverOf(target, pawn.Faction, true);
                    if (currentReserver != null && currentReserver.jobs != null)
                    {
                        currentReserver.jobs.EndCurrentJob(JobCondition.InterruptForced);
                    }
                    action();
                };
            }

            list.Add(new FloatMenuOption(title, handler, priority, mouseoverGuiAction, revalidateClickTarget, extraPartWidth, extraPartOnGUI));
        }
    }
}
