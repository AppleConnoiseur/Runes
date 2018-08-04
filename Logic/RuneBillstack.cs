using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Runes
{
    /// <summary>
    /// Controls how bills are handled on the stack for runes.
    /// </summary>
    public class RuneBillstack : IExposable
    {
        public List<RuneBill> bills = new List<RuneBill>();

        public void ExposeData()
        {
            Scribe_Collections.Look(ref bills, "bills", LookMode.Deep);
            if(Scribe.mode == LoadSaveMode.LoadingVars)
            {
                foreach(RuneBill bill in bills)
                {
                    bill.billstack = this;
                }
            }
        }

        public void AddBill(RuneBill bill)
        {
            bill.billstack = this;
            bills.Add(bill);
        }

        public void RemoveBill(RuneBill bill)
        {
            bill.Notify_Removed();
            bills.Remove(bill);
        }

        public void RearrangeBill(RuneBill bill)
        {
            int oldIndex = bills.IndexOf(bill);

            bills.Remove(bill);
            bills.Insert(oldIndex + bill.moveBill, bill);
            bill.moveBill = 0;
        }

        public void Cleanup()
        {
            foreach(RuneBill bill in bills)
            {
                bill.Cleanup();
            }
        }
    }
}
