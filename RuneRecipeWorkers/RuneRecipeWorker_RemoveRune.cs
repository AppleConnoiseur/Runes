using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Runes
{
    /// <summary>
    /// Worker that removes a rune from a item.
    /// </summary>
    public class RuneRecipeWorker_RemoveRune : RuneRecipeWorker_ManipulateRune
    {
        public override bool SelectRuneInItem => true;

        public override void FinishBill(RuneBill bill, Pawn pawn)
        {
            SocketComp socket = bill.itemToBeManipulated.TryGetComp<SocketComp>();
            if (socket != null)
            {
                Thing rune = socket.RemoveRune(bill.runeToManipulate);

                if(rune != null)
                {
                    //Try to spawn it nearby.
                    GenPlace.TryPlaceThing(rune, pawn.Position, pawn.Map, ThingPlaceMode.Near);
                }
            }
        }

        public override bool ItemTargetingCondition(SocketComp socketable, RuneBill bill)
        {
            return socketable.CanBeSocketed && socketable.SocketedRunes.Count() > 0;
        }

        public override bool RuneTargetingCondition(Thing thing, RuneBill bill)
        {
            return false;
        }

        public override void Notify_ItemRemoved(RuneBill bill)
        {
            bill.runeToManipulate = null;
        }
    }
}
