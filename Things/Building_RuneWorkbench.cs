using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Runes
{
    /// <summary>
    /// Building for runework.
    /// </summary>
    public class Building_RuneWorkbench : Building, IRuneBillHolder
    {
        public RuneBillstack billStack = new RuneBillstack();

        public RuneBillstack BillStack => billStack;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Deep.Look(ref billStack, "billStack");
        }

        public void TickAction()
        {
            billStack.Cleanup();
        }

        public override void Tick()
        {
            base.Tick();
            TickAction();
        }

        public override void TickLong()
        {
            base.TickLong();
            TickAction();
        }

        public override void TickRare()
        {
            base.TickRare();
            TickAction();
        }
    }
}
