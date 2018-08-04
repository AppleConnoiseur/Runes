using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace Runes
{
    /// <summary>
    /// Do Bills on a workbench.
    /// </summary>
    public class JobDriver_DoRuneBill : JobDriver_ManipulateRune
    {
        public RuneBill Bill
        {
            get
            {
                return (Workbench as Building_RuneWorkbench).billStack.bills.FirstOrDefault(bill => bill.assignedPawn == pawn);
            }
        }

        public IRuneBillHolder BillHolder
        {
            get
            {
                return Workbench;
            }
        }

        public override void Notify_DamageTaken(DamageInfo dinfo)
        {
            EndJobWith(JobCondition.InterruptForced);
        }

        public override string GetReport()
        {
            return ReportStringProcessed(Bill.recipeDef.reportString);
        }

        public override bool TryMakePreToilReservations()
        {
            RuneBill bill = Bill;
            //Log.Message("Bill: " + bill);

            if (bill != null)
            {
                bool reservedItem = false;
                bool reservedRune = false;

                if (Item != null)
                {
                    if (Item.ParentHolder is Map)
                    {
                        if (pawn.CanReserveAndReach(Item, PathEndMode.OnCell, Danger.Deadly))
                        {
                            pawn.Reserve(Item, job);
                            reservedItem = true;
                        }
                    }
                    else
                    {
                        reservedItem = true;
                    }
                }

                if(bill.recipeDef.requiresRune)
                {
                    if (RuneThing != null)
                    {
                        if (RuneThing.ParentHolder is Map)
                        {
                            if (pawn.CanReserveAndReach(RuneThing, PathEndMode.OnCell, Danger.Deadly))
                            {
                                pawn.Reserve(RuneThing, job);
                                reservedRune = true;
                            }
                        }
                        else if (RuneThing.ParentHolder is SocketComp)
                        {
                            reservedRune = true;
                        }
                    }
                }
                else
                {
                    reservedRune = true;
                }

                /*Log.Message("Bill: " + bill.recipeDef.defName);
                Log.Message("reservedItem: " + reservedItem);
                Log.Message("reservedRune: " + reservedRune);*/

                return reservedItem && reservedRune;
            }

            return false;
        }

        public override IEnumerable<Toil> MakeWorkToils(Toil endToil)
        {
            this.FailOn(() => Bill == null || (Bill.paused || Bill.assignedPawn != pawn));

            yield return Toils_General.Wait(Bill.recipeDef.workRequired, TargetIndex.C).WithProgressBarToilDelay(TargetIndex.C, false).PlaySustainerOrSound(Bill.recipeDef.workSound);
            Toil workToil = new Toil();
            workToil.initAction = delegate ()
            {
                SocketComp socket = Item.TryGetComp<SocketComp>();
                if (socket != null && Bill != null)
                {
                    Bill.recipeDef.Worker.FinishBill(Bill, GetActor());
                    BillHolder.BillStack.RemoveBill(Bill);
                    EndJobWith(JobCondition.Succeeded);
                }
            };
            yield return workToil;
        }
    }
}
