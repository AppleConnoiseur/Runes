using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Runes
{
    /// <summary>
    /// Worker that inserts runes into a item.
    /// </summary>
    public class RuneRecipeWorker_InsertRune : RuneRecipeWorker_ManipulateRune
    {
        public override void Notify_ItemAdded(RuneBill bill)
        {
            if(bill.runeToManipulate != null && bill.runeToManipulate.TryGetComp<RuneComp>() is RuneComp rune && !rune.CanBeSocketedInto(bill.itemToBeManipulated))
            {
                bill.runeToManipulate = null;
            }
        }

        public override void FinishBill(RuneBill bill, Pawn pawn)
        {
            SocketComp socket = bill.itemToBeManipulated.TryGetComp<SocketComp>();
            if (socket != null)
            {
                socket.SocketRune(bill.runeToManipulate);
            }
        }

        public override bool ItemTargetingCondition(SocketComp socketable, RuneBill bill)
        {
            return socketable.CanBeSocketed && socketable.FreeSlots > 0;
        }

        public override bool RuneTargetingCondition(Thing thing, RuneBill bill)
        {
            return thing.TryGetComp<RuneComp>() is RuneComp rune && rune.CanBeSocketedInto(bill.itemToBeManipulated);
        }
    }
}
