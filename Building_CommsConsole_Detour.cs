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
    public static class Building_CommsConsole_Detour
    {
        public static IEnumerable<FloatMenuOption> GetFloatMenuOptions(this Building_CommsConsole building, Pawn myPawn)
        {
            /*if (!myPawn.CanReserve(building, 1))
            {
                FloatMenuOption item = new FloatMenuOption("CannotUseReserved".Translate(), null, MenuOptionPriority.Medium, null, null, 0f, null);
                return new List<FloatMenuOption>
                {
                    item
                };
            }*/
            if (!myPawn.CanReach(building, PathEndMode.InteractionCell, Danger.Some, false, TraverseMode.ByPawn))
            {
                FloatMenuOption item2 = new FloatMenuOption("CannotUseNoPath".Translate(), null, MenuOptionPriority.Medium, null, null, 0f, null);
                return new List<FloatMenuOption>
                {
                    item2
                };
            }
            if (Find.MapConditionManager.ConditionIsActive(MapConditionDefOf.SolarFlare))
            {
                FloatMenuOption item3 = new FloatMenuOption("CannotUseSolarFlare".Translate(), null, MenuOptionPriority.Medium, null, null, 0f, null);
                return new List<FloatMenuOption>
                {
                    item3
                };
            }
            if (!building.GetFieldViaReflection<CompPowerTrader>("powerComp").PowerOn)
            {
                FloatMenuOption item4 = new FloatMenuOption("CannotUseNoPower".Translate(), null, MenuOptionPriority.Medium, null, null, 0f, null);
                return new List<FloatMenuOption>
                {
                    item4
                };
            }
            if (!myPawn.health.capacities.CapableOf(PawnCapacityDefOf.Talking))
            {
                FloatMenuOption item5 = new FloatMenuOption("CannotUseReason".Translate(new object[]
                {
                    "IncapableOfCapacity".Translate(new object[]
                    {
                        PawnCapacityDefOf.Talking.label
                    })
                }), null, MenuOptionPriority.Medium, null, null, 0f, null);
                return new List<FloatMenuOption>
                {
                    item5
                };
            }
            if (!building.CanUseCommsNow)
            {
                Log.Error(myPawn + " could not use comm console for unknown reason.");
                FloatMenuOption item6 = new FloatMenuOption("Cannot use now", null, MenuOptionPriority.Medium, null, null, 0f, null);
                return new List<FloatMenuOption>
                {
                    item6
                };
            }
            IEnumerable<ICommunicable> enumerable = Find.PassingShipManager.passingShips.Cast<ICommunicable>().Concat(Find.FactionManager.AllFactionsInViewOrder.Cast<ICommunicable>());
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            foreach (ICommunicable commTarget in enumerable)
            {
                Faction faction = commTarget as Faction;
                if (faction == null || !faction.IsPlayer)
                {
                    ICommunicable localCommTarget = commTarget;
                    Action action = delegate
                    {
                        if (commTarget is TradeShip && !Building_OrbitalTradeBeacon.AllPowered().Any<Building_OrbitalTradeBeacon>())
                        {
                            Messages.Message("MessageNeedBeaconToTradeWithShip".Translate(), building, MessageSound.RejectInput);
                            return;
                        }
                        Job job = new Job(JobDefOf.UseCommsConsole, building);
                        job.commTarget = localCommTarget;
                        myPawn.drafter.TakeOrderedJob(job);
                        PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.OpeningComms, KnowledgeAmount.Total);
                    };
                    ITWN.PostMenuOption(list,
                        myPawn,
                        building,
                        "CallOnRadio".Translate(new object[]
                        {
                            localCommTarget.GetCallLabel()
                        }), action);
                }
            }
            return list;
        }
    }
}
