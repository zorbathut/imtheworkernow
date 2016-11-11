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
    public static class FloatMenuMakerMap_Detour
    {
        // well this is a dumpster fire of a decompilation job
        public static void AddDraftedOrders(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
        {
            IntVec3 b = IntVec3.FromVector3(clickPos);
            foreach (TargetInfo current in GenUI.TargetsAt(clickPos, TargetingParameters.ForAttackHostile(), true))
            {
                Verse.TargetInfo c__AnonStorey = current;
                if (pawn.equipment.Primary != null && !pawn.equipment.PrimaryEq.PrimaryVerb.verbProps.MeleeRange)
                {
                    string str;
                    Action rangedAct = FloatMenuUtility.GetRangedAttackAction(pawn, c__AnonStorey, out str);
                    string text = "FireAt".Translate(new object[]
                    {
                        c__AnonStorey.Thing.LabelCap
                    });
                    FloatMenuOption floatMenuOption = new FloatMenuOption();
                    floatMenuOption.priority = MenuOptionPriority.High;
                    if (rangedAct == null)
                    {
                        text = text + " (" + str + ")";
                    }
                    else
                    {
                        floatMenuOption.autoTakeable = true;
                        floatMenuOption.action = delegate
                        {
                            MoteMaker.MakeStaticMote(c__AnonStorey.Thing.DrawPos, ThingDefOf.Mote_FeedbackAttack, 1f);
                            rangedAct();
                        };
                    }
                    floatMenuOption.Label = text;
                    opts.Add(floatMenuOption);
                }
                string str2;
                Action meleeAct = FloatMenuUtility.GetMeleeAttackAction(pawn, c__AnonStorey, out str2);
                Pawn pawn2 = c__AnonStorey.Thing as Pawn;
                string text2;
                if (pawn2 != null && pawn2.Downed)
                {
                    text2 = "MeleeAttackToDeath".Translate(new object[]
                    {
                        c__AnonStorey.Thing.LabelCap
                    });
                }
                else
                {
                    text2 = "MeleeAttack".Translate(new object[]
                    {
                        c__AnonStorey.Thing.LabelCap
                    });
                }
                Thing thing = c__AnonStorey.Thing;
                FloatMenuOption floatMenuOption2 = new FloatMenuOption(string.Empty, null, MenuOptionPriority.High, null, thing, 0f, null);
                if (meleeAct == null)
                {
                    text2 = text2 + " (" + str2 + ")";
                }
                else
                {
                    floatMenuOption2.action = delegate
                    {
                        MoteMaker.MakeStaticMote(c__AnonStorey.Thing.DrawPos, ThingDefOf.Mote_FeedbackAttack, 1f);
                        meleeAct();
                    };
                }
                floatMenuOption2.Label = text2;
                opts.Add(floatMenuOption2);
            }
            if (pawn.RaceProps.Humanlike && pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                foreach (TargetInfo current2 in GenUI.TargetsAt(clickPos, TargetingParameters.ForArrest(pawn), true))
                {
                    TargetInfo dest = current2;
                    if (!((Pawn)dest.Thing).Downed)
                    {
                        if (!pawn.CanReach(dest, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn))
                        {
                            opts.Add(new FloatMenuOption("CannotArrest".Translate() + " (" + "NoPath".Translate() + ")", null, MenuOptionPriority.Medium, null, null, 0f, null));
                        }
                        else
                        {
                            Pawn pTarg = (Pawn)dest.Thing;
                            Action action = delegate
                            {
                                Building_Bed building_Bed = RestUtility.FindBedFor(pTarg, pawn, true, false, false);
                                if (building_Bed == null)
                                {
                                    Messages.Message("CannotArrest".Translate() + ": " + "NoPrisonerBed".Translate(), pTarg, MessageSound.RejectInput);
                                    return;
                                }
                                Job job = new Job(JobDefOf.Arrest, pTarg, building_Bed);
                                job.playerForced = true;
                                job.maxNumToCarry = 1;
                                pawn.drafter.TakeOrderedJob(job);
                                TutorUtility.DoModalDialogIfNotKnown(ConceptDefOf.ArrestingCreatesEnemies);
                            };
                            Thing thing = dest.Thing;
                            ITWN.PostMenuOption(opts,
                                pawn,
                                dest.Thing,
                                "TryToArrest".Translate(new object[] { dest.Thing.LabelCap }),
                                action,
                                revalidateClickTarget: thing);
                        }
                    }
                }
            }
            int num = GenRadial.NumCellsInRadius(2.9f);
            IntVec3 curLoc;
            for (int i = 0; i < num; i++)
            {
                curLoc = GenRadial.RadialPattern[i] + b;
                if (curLoc.Standable())
                {
                    if (curLoc != pawn.Position)
                    {
                        if (!pawn.CanReach(curLoc, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn))
                        {
                            FloatMenuOption item = new FloatMenuOption("CannotGoNoPath".Translate(), null, MenuOptionPriority.Low, null, null, 0f, null);
                            opts.Add(item);
                        }
                        else
                        {
                            Action action2 = delegate
                            {
                                IntVec3 intVec = Pawn_DraftController.BestGotoDestNear(curLoc, pawn);
                                Job job = new Job(JobDefOf.Goto, intVec);
                                job.playerForced = true;
                                pawn.drafter.TakeOrderedJob(job);
                                MoteMaker.MakeStaticMote(intVec, ThingDefOf.Mote_FeedbackGoto, 1f);
                            };
                            opts.Add(new FloatMenuOption("GoHere".Translate(), action2, MenuOptionPriority.Low, null, null, 0f, null)
                            {
                                autoTakeable = true
                            });
                        }
                    }
                    break;
                }
            }
        }
    }
}
