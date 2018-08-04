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
    /// Base class for recipes manipulating runes.
    /// </summary>
    public abstract class RuneRecipeWorker_ManipulateRune : RuneRecipeWorker
    {
        public abstract bool ItemTargetingCondition(SocketComp socketable, RuneBill bill);

        public virtual void Notify_ItemRemoved(RuneBill bill)
        {

        }

        public virtual void Notify_ItemAdded(RuneBill bill)
        {

        }

        public TargetingParameters GetTargetingParametersForItem(RuneBill bill)
        {
            return new TargetingParameters
            {
                canTargetPawns = false,
                canTargetBuildings = false,
                canTargetItems = true,
                mapObjectTargetsMustBeAutoAttackable = false,
                validator = ((TargetInfo x) => x.Thing != null && x.Thing.TryGetComp<SocketComp>() is SocketComp socketable && ItemTargetingCondition(socketable, bill))
            };
        }

        public abstract bool RuneTargetingCondition(Thing thing, RuneBill bill);

        public TargetingParameters GetTargetingParametersForRune(RuneBill bill)
        {
            return new TargetingParameters
            {
                canTargetPawns = false,
                canTargetBuildings = false,
                canTargetItems = true,
                mapObjectTargetsMustBeAutoAttackable = false,
                validator = ((TargetInfo x) => x.Thing != null && RuneTargetingCondition(x.Thing, bill))
            };
        }

        public virtual bool SelectRuneInItem => false;

        public override void DoWorkerGUI(Rect inRect, RuneBill bill)
        {
            //Show the items to manipulate.
            float thingWidth = inRect.height * 0.50f;
            float totalWidth = thingWidth * 3f;
            float centerX = (inRect.width / 2f) - (totalWidth / 2f);
            float centerY = inRect.y + (inRect.height / 2f) - (thingWidth / 2f);

            int item = 0;
            {
                Rect itemRect = new Rect(inRect);
                itemRect.x += centerX + (thingWidth * item);
                itemRect.y = centerY;
                itemRect.width = thingWidth;
                itemRect.height = thingWidth;

                Widgets.DrawWindowBackground(itemRect);

                if(bill.itemToBeManipulated != null)
                {
                    Widgets.ThingIcon(itemRect, bill.itemToBeManipulated);
                    TooltipHandler.TipRegion(itemRect, bill.itemToBeManipulated.LabelCapNoCount);

                    if (Widgets.ButtonInvisible(itemRect))
                    {
                        Notify_ItemRemoved(bill);
                        bill.itemToBeManipulated = null;
                    }
                }
                else
                {
                    Text.Anchor = TextAnchor.MiddleCenter;
                    Text.Font = GameFont.Tiny;
                    Widgets.Label(itemRect, "RunesBillManipulateSelectItem".Translate());
                    Text.Anchor = TextAnchor.UpperLeft;
                    Text.Font = GameFont.Small;

                    if(Widgets.ButtonInvisible(itemRect))
                    {
                        Find.Targeter.BeginTargeting(
                        GetTargetingParametersForItem(bill),
                        delegate (LocalTargetInfo target)
                        {
                            bill.itemToBeManipulated = target.Thing;
                            Notify_ItemAdded(bill);
                        });
                    }
                }

                Widgets.DrawHighlightIfMouseover(itemRect);
            }

            item++;
            {
                Rect itemRect = new Rect(inRect);
                itemRect.x += centerX + (thingWidth * item);
                itemRect.y = centerY;
                itemRect.width = thingWidth;
                itemRect.height = thingWidth;

                //Widgets.DrawRectFast(itemRect, Color.white);

                Text.Font = GameFont.Medium;
                Text.Anchor = TextAnchor.MiddleCenter;

                Widgets.Label(itemRect, "and");

                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.UpperLeft;
            }

            item ++;
            {
                Rect itemRect = new Rect(inRect);
                itemRect.x += centerX + (thingWidth * item);
                itemRect.y = centerY;
                itemRect.width = thingWidth;
                itemRect.height = thingWidth;

                Widgets.DrawWindowBackground(itemRect);

                if (bill.runeToManipulate != null)
                {
                    Widgets.ThingIcon(itemRect, bill.runeToManipulate);
                    TooltipHandler.TipRegion(itemRect, bill.runeToManipulate.LabelCapNoCount);

                    if (Widgets.ButtonInvisible(itemRect))
                    {
                        bill.runeToManipulate = null;
                    }
                }
                else
                {
                    Text.Anchor = TextAnchor.MiddleCenter;
                    Text.Font = GameFont.Tiny;
                    Widgets.Label(itemRect, "RunesBillManipulateSelectRune".Translate());
                    Text.Anchor = TextAnchor.UpperLeft;
                    Text.Font = GameFont.Small;

                    if (Widgets.ButtonInvisible(itemRect))
                    {
                        if(SelectRuneInItem)
                        {
                            if (bill.itemToBeManipulated != null)
                            {
                                SocketComp socketable = bill.itemToBeManipulated.TryGetComp<SocketComp>();
                                if (socketable != null)
                                {
                                    FloatMenuUtility.MakeMenu(
                                        socketable.socketedThings,
                                        (rune) => rune.LabelCapNoCount,
                                        (Thing thing) => delegate ()
                                        {
                                            bill.runeToManipulate = thing;
                                        });
                                }
                            }
                        }
                        else
                        {
                            Find.Targeter.BeginTargeting(
                            GetTargetingParametersForRune(bill),
                            delegate (LocalTargetInfo target)
                            {
                                bill.runeToManipulate = target.Thing;
                            });
                        }
                    }
                }

                Widgets.DrawHighlightIfMouseover(itemRect);
            }
        }

        public override bool ExtraPawnCriterias(Pawn pawn, RuneBill bill)
        {
            return true;
        }

        public override bool CanStartBill(RuneBill bill)
        {
            return (bill.itemToBeManipulated != null && !bill.itemToBeManipulated.Destroyed) && (bill.runeToManipulate != null && !bill.runeToManipulate.Destroyed);
        }
    }
}
