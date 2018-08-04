using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Runes
{
    /// <summary>
    /// Contains information neccesary to perform a operation on a item with runes.
    /// </summary>
    [StaticConstructorOnStartup]
    public class RuneBill : IExposable
    {
        public static readonly Texture2D ReorderUp = ContentFinder<Texture2D>.Get("UI/Buttons/ReorderUp", true);

        public static readonly Texture2D ReorderDown = ContentFinder<Texture2D>.Get("UI/Buttons/ReorderDown", true);

        public static readonly Texture2D DeleteX = ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true);

        /// <summary>
        /// The pawn which is assigned to this bill.
        /// </summary>
        public Pawn assignedPawn;

        /// <summary>
        /// The item which is to be manipulated.
        /// </summary>
        public Thing itemToBeManipulated;

        /// <summary>
        /// The (optional) rune to manipulate the item with.
        /// </summary>
        public Thing runeToManipulate;

        /// <summary>
        /// Is the bill paused?
        /// </summary>
        public bool paused = false;

        /// <summary>
        /// If true this bill will be deleted.
        /// </summary>
        public bool deleted = false;

        /// <summary>
        /// If not 0 it will move the bill up or down.
        /// </summary>
        public int moveBill = 0;

        /// <summary>
        /// The Bill stack we belong to.
        /// </summary>
        [Unsaved]
        public RuneBillstack billstack;

        /// <summary>
        /// The recipe this Bill does.
        /// </summary>
        public RuneRecipeDef recipeDef;

        static RuneBill()
        {

        }

        public void ExposeData()
        {
            Scribe_References.Look(ref assignedPawn, "assignedPawn");
            Scribe_References.Look(ref itemToBeManipulated, "itemToBeManipulated");
            Scribe_References.Look(ref runeToManipulate, "itemToBeSocketed");
            Scribe_Defs.Look(ref recipeDef, "recipeDef");
            Scribe_Values.Look(ref paused, "paused");
        }

        /// <summary>
        /// Can this pawn do this Bill?
        /// </summary>
        /// <param name="pawn">Prospecting pawn.</param>
        /// <returns>True if they can, false if not.</returns>
        public virtual bool CanPawnDoBill(Pawn pawn)
        {
            if(recipeDef.workSkill != null)
            {
                return pawn.skills.GetSkill(recipeDef.workSkill).Level >= recipeDef.minimumSkillLevel && recipeDef.Worker.ExtraPawnCriterias(pawn, this);
            }

            return recipeDef.Worker.ExtraPawnCriterias(pawn, this);
        }

        /// <summary>
        /// Does the GUI for the Bill.
        /// </summary>
        /// <param name="inRect">Space to work with.</param>
        public virtual void DoGUI(Rect inRect)
        {
            //Let the worker do its GUI
            if (recipeDef != null)
            {
                recipeDef.Worker.DoWorkerGUI(inRect, this);
            }

            //Add our own bits and bobs.
            if(billstack.bills.IndexOf(this) > 0)
            {
                //Move Up
                Rect moveRect = new Rect(inRect.x, inRect.y, 24f, 24f);

                if (Widgets.ButtonImage(moveRect, ReorderUp))
                {
                    moveBill = -1;
                    SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                }
                TooltipHandler.TipRegion(moveRect, "ReorderBillUpTip".Translate());
            }

            if (billstack.bills.IndexOf(this) < billstack.bills.Count - 1)
            {
                //Move Down
                Rect moveRect = new Rect(inRect.x, inRect.y + 24f, 24f, 24f);

                if (Widgets.ButtonImage(moveRect, ReorderDown))
                {
                    moveBill = 1;
                    SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                }
                TooltipHandler.TipRegion(moveRect, "ReorderBillDownTip".Translate());
            }

            Rect deleteRect = new Rect(inRect.xMax - 40f, inRect.y, 24f, 24f);
            if (Widgets.ButtonImage(deleteRect, DeleteX, Color.white, Color.white * GenUI.SubtleMouseoverColor))
            {
                //this.billStack.Delete(this);
                deleted = true;
                SoundDefOf.Click.PlayOneShotOnCamera(null);
            }
        }

        /// <summary>
        /// Periodic cleaning on the bill. Remove destroyed items and such.
        /// </summary>
        public void Cleanup()
        {
            if(assignedPawn != null && assignedPawn.Destroyed)
            {
                assignedPawn = null;
            }

            if (itemToBeManipulated != null && itemToBeManipulated.Destroyed)
            {
                itemToBeManipulated = null;
            }

            if (runeToManipulate != null && runeToManipulate.Destroyed)
            {
                runeToManipulate = null;
            }
        }

        public void Notify_Removed()
        {
            assignedPawn = null;
            itemToBeManipulated = null;
            runeToManipulate = null;
        }
    }
}
