using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse.AI;
using Verse;

namespace Runes
{
    /// <summary>
    /// Gives out work to runemasters to do bills at rune workbenches.
    /// </summary>
    public class WorkGiver_DoRuneBills : WorkGiver_Scanner
    {
        public override PathEndMode PathEndMode => PathEndMode.InteractionCell;

        public override Danger MaxPathDanger(Pawn pawn)
        {
            return Danger.Some;
        }

        public override ThingRequest PotentialWorkThingRequest
        {
            get
            {
                ThingRequest result;
                if (def.fixedBillGiverDefs != null && def.fixedBillGiverDefs.Count == 1)
                {
                    result = ThingRequest.ForDef(def.fixedBillGiverDefs[0]);
                }
                else
                {
                    result = ThingRequest.ForGroup(ThingRequestGroup.PotentialBillGiver);
                }
                return result;
            }
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if(t is IRuneBillHolder billHolder)
            {
                if(t.def.hasInteractionCell)
                {
                    if(t.InteractionCell.Standable(t.Map) && pawn.CanReserveAndReach(t, PathEndMode.InteractionCell, Danger.Some))
                    {
                        if (billHolder.BillStack.bills.FirstOrDefault(bill =>
                        (bill.assignedPawn == null || bill.assignedPawn == pawn) &&
                        bill.CanPawnDoBill(pawn) &&
                        bill.recipeDef.Worker.CanStartBill(bill)) != null)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    if(pawn.CanReserveAndReach(t, PathEndMode.ClosestTouch, Danger.Some))
                    {
                        if (billHolder.BillStack.bills.FirstOrDefault(bill =>
                    (bill.assignedPawn == null || bill.assignedPawn == pawn) &&
                    bill.CanPawnDoBill(pawn) &&
                    bill.recipeDef.Worker.CanStartBill(bill)) != null)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Job jobResult = null;
            if (t is IRuneBillHolder billHolder)
            {
                if (t.def.hasInteractionCell)
                {
                    if (t.InteractionCell.Standable(t.Map) && pawn.CanReserveAndReach(t, PathEndMode.InteractionCell, Danger.Some))
                    {
                        if (billHolder.BillStack.bills.FirstOrDefault(bill =>
                        (bill.assignedPawn == null || bill.assignedPawn == pawn) &&
                        bill.CanPawnDoBill(pawn) &&
                        bill.recipeDef.Worker.CanStartBill(bill)) is RuneBill runeBill)
                        {
                            if (runeBill.assignedPawn == null)
                            {
                                runeBill.assignedPawn = pawn;
                            }

                            jobResult = new Job(RuneJobDefOf.Runes_DoRuneBill, runeBill.itemToBeManipulated, runeBill.runeToManipulate, t)
                            {
                                count = 1
                            };
                            return jobResult;
                        }
                    }
                }
                else
                {
                    if (pawn.CanReserveAndReach(t, PathEndMode.ClosestTouch, Danger.Some))
                    {
                        if (billHolder.BillStack.bills.FirstOrDefault(bill =>
                    (bill.assignedPawn == null || bill.assignedPawn == pawn) &&
                    bill.CanPawnDoBill(pawn) &&
                    bill.recipeDef.Worker.CanStartBill(bill)) is RuneBill runeBill)
                        {
                            if (runeBill.assignedPawn == null)
                            {
                                runeBill.assignedPawn = pawn;
                            }

                            jobResult = new Job(RuneJobDefOf.Runes_DoRuneBill, runeBill.itemToBeManipulated, runeBill.runeToManipulate, t)
                            {
                                count = 1
                            };
                            return jobResult;
                        }
                    }
                }

                //Look if there are unassigned bills to use.
                /*if (billHolder.BillStack.bills.FirstOrDefault(bill =>
                     (bill.assignedPawn == null || bill.assignedPawn == pawn) &&
                     bill.CanPawnDoBill(pawn) &&
                     bill.recipeDef.Worker.CanStartBill(bill)) is RuneBill runeBill)
                {
                    if(runeBill.assignedPawn == null)
                    {
                        runeBill.assignedPawn = pawn;
                    }

                    jobResult = new Job(RuneJobDefOf.Runes_DoRuneBill, runeBill.itemToBeManipulated, runeBill.runeToManipulate, t)
                    {
                        count = 1
                    };
                }*/
            }

            return jobResult;
        }
    }
}
