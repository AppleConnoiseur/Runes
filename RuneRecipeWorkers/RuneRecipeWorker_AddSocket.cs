using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Runes
{
    public class RuneRecipeWorker_AddSocket : RuneRecipeWorker
    {
        public override bool CanStartBill(RuneBill bill)
        {
            return (bill.itemToBeManipulated != null && !bill.itemToBeManipulated.Destroyed);
        }

        public TargetingParameters GetTargetingParametersForItem(RuneBill bill)
        {
            return new TargetingParameters
            {
                canTargetPawns = false,
                canTargetBuildings = false,
                canTargetItems = true,
                mapObjectTargetsMustBeAutoAttackable = false,
                validator = ((TargetInfo x) => x.Thing != null && x.Thing.TryGetComp<SocketComp>() is SocketComp socketable)
            };
        }

        public override void DoWorkerGUI(Rect inRect, RuneBill bill)
        {
            //Show the items to manipulate.
            float thingWidth = inRect.height * 0.50f;
            float totalWidth = thingWidth * 1f;
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

                if (bill.itemToBeManipulated != null)
                {
                    Widgets.ThingIcon(itemRect, bill.itemToBeManipulated);
                    TooltipHandler.TipRegion(itemRect, bill.itemToBeManipulated.LabelCapNoCount);

                    if (Widgets.ButtonInvisible(itemRect))
                    {
                        //Notify_ItemRemoved(bill);
                        bill.itemToBeManipulated = null;
                    }

                    //Draw skill requirement.
                    Rect skillRequirementRect = new Rect(itemRect);
                    Text.Anchor = TextAnchor.MiddleCenter;
                    Text.Font = GameFont.Tiny;
                    skillRequirementRect.y = itemRect.yMax + 2f;
                    skillRequirementRect.height = Text.LineHeight + 4f;
                    skillRequirementRect.x = inRect.x;
                    skillRequirementRect.width = inRect.width;

                    Widgets.Label(skillRequirementRect, "RunesSkillRequiredToAddSocket".Translate(RequiredSkillLevelToSocket(bill), bill.recipeDef.workSkill.LabelCap));

                    //Reset
                    Text.Anchor = TextAnchor.UpperLeft;
                    Text.Font = GameFont.Small;
                }
                else
                {
                    Text.Anchor = TextAnchor.MiddleCenter;
                    Text.Font = GameFont.Tiny;
                    Widgets.Label(itemRect, "RunesBillManipulateSelectItem".Translate());
                    Text.Anchor = TextAnchor.UpperLeft;
                    Text.Font = GameFont.Small;

                    if (Widgets.ButtonInvisible(itemRect))
                    {
                        Find.Targeter.BeginTargeting(
                        GetTargetingParametersForItem(bill),
                        delegate (LocalTargetInfo target)
                        {
                            bill.itemToBeManipulated = target.Thing;
                            //Notify_ItemAdded(bill);
                        });
                    }
                }

                Widgets.DrawHighlightIfMouseover(itemRect);
            }
        }

        public int RequiredSkillLevelToSocket(RuneBill bill)
        {
            int skillLevelPerSocket = 6;
            if (bill.itemToBeManipulated?.TryGetComp<SocketComp>() is SocketComp socketable)
            {
                return socketable.availableSockets * skillLevelPerSocket;
            }

            return -1;
        }

        public override bool ExtraPawnCriterias(Pawn pawn, RuneBill bill)
        {
            SkillRecord skill = pawn.skills.GetSkill(bill.recipeDef.workSkill);

            return skill.Level >= RequiredSkillLevelToSocket(bill);
        }

        public override void FinishBill(RuneBill bill, Pawn pawn)
        {
            SocketComp socket = bill.itemToBeManipulated.TryGetComp<SocketComp>();
            if (socket != null)
            {
                socket.AddSocket(pawn);
            }
        }
    }
}
