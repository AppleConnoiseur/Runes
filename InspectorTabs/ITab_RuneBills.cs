using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Runes
{
    /// <summary>
    /// Displays bills at the rune workbench.
    /// </summary>
    public class ITab_RuneBills : ITab
    {
        public static Vector2 WinSize = new Vector2(400f, 400f);

        public Vector2 billsScrollPosition = new Vector2();

        public override bool IsVisible => SelTable != null;

        protected Building_RuneWorkbench SelTable
        {
            get
            {
                return SelThing as Building_RuneWorkbench;
            }
        }

        public ITab_RuneBills()
        {
            //size = ITab_Bills.WinSize;
            labelKey = "Runes_RuneBills";
            size = WinSize;
        }

        protected override void FillTab()
        {
            Rect mainRect = new Rect(0f, 0f, WinSize.x, WinSize.y);
            float rowY = mainRect.y;

            //Title
            Text.Anchor = TextAnchor.UpperCenter;
            Text.Font = GameFont.Medium;
            Rect titleRect = new Rect(mainRect);
            titleRect.y += Text.LineHeight;
            titleRect.height = Text.LineHeight;
            Widgets.Label(titleRect, "RunesRuneBills".Translate());
            //rowY = Text.LineHeight + 2f;
            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;

            //Bills & Add Bill
            Rect outerBillsRect = new Rect(mainRect);
            outerBillsRect.y = titleRect.yMax + 2f;
            outerBillsRect.height = mainRect.height - outerBillsRect.y - 4f;
            outerBillsRect.x += 4f;
            outerBillsRect.width -= 8f;

            Widgets.DrawWindowBackground(outerBillsRect);

            Rect innerBillsRect = new Rect(outerBillsRect);
            float billHeightSize = outerBillsRect.height / 2.5f;
            innerBillsRect.height = billHeightSize * (1 + SelTable.billStack.bills.Count);

            Widgets.BeginScrollView(outerBillsRect, ref billsScrollPosition, innerBillsRect);

            rowY = outerBillsRect.y;

            //Do Bills
            int index = 0;
            RuneBill billToMove = null;
            RuneBill deleteBill = null;

            foreach (RuneBill bill in SelTable.billStack.bills)
            {
                Rect billRect = new Rect(innerBillsRect);
                billRect.y = rowY;
                billRect.height = billHeightSize;

                if (index % 2 == 0)
                    Widgets.DrawAltRect(billRect);

                Text.Anchor = TextAnchor.UpperLeft;
                Text.Font = GameFont.Small;

                Rect billTitleRect = new Rect(billRect);
                billTitleRect.x += 26f;
                billTitleRect.width -= 26f;
                Widgets.Label(billTitleRect, bill.recipeDef.label);

                if (bill.assignedPawn != null)
                {
                    Text.Anchor = TextAnchor.LowerLeft;
                    Text.Font = GameFont.Tiny;
                    Widgets.Label(billRect, "RunesBillAuthor".Translate(bill.assignedPawn.Name.ToString()));
                    Text.Anchor = TextAnchor.UpperLeft;
                    Text.Font = GameFont.Small;
                }

                //bill.recipeDef.Worker.DoWorkerGUI(billRect, bill);
                bill.DoGUI(billRect);

                if(bill.moveBill != 0)
                {
                    billToMove = bill;
                }

                if(bill.deleted)
                {
                    deleteBill = bill;
                }

                rowY += billHeightSize;
                index++;
            }

            //Do Add Bill
            Rect addBillRect = new Rect(innerBillsRect);
            addBillRect.y = rowY;
            addBillRect.height = billHeightSize;

            if (index % 2 == 0)
                Widgets.DrawAltRect(addBillRect);

            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;

            Widgets.Label(addBillRect, "RunesAddRuneBill".Translate());

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;

            Widgets.DrawHighlightIfMouseover(addBillRect);
            if(Widgets.ButtonInvisible(addBillRect))
            {
                FloatMenuUtility.MakeMenu<RuneRecipeDef>(
                DefDatabase<RuneRecipeDef>.AllDefs,
                (recipeDef) => recipeDef.label,
                (RuneRecipeDef recipeDef) => delegate ()
                {
                    RuneBill bill = new RuneBill();
                    bill.recipeDef = recipeDef;

                    SelTable.billStack.AddBill(bill);
                });
            }

            Widgets.EndScrollView();

            //Manipulate
            if (billToMove != null)
            {
                //SelTable.billStack.bills
                /*int oldIndex = SelTable.billStack.bills.IndexOf(billToMove);

                SelTable.billStack.bills.Remove(billToMove);
                SelTable.billStack.bills.Insert(oldIndex + billToMove.moveBill, billToMove);
                billToMove.moveBill = 0;*/
                SelTable.billStack.RearrangeBill(billToMove);
            }

            //Delete
            if(deleteBill != null)
            {
                SelTable.billStack.RemoveBill(deleteBill);
            }
        }
    }
}
