using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Runes
{
    /// <summary>
    /// Determines how a recipe plays out in a RuneRecipeDef.
    /// </summary>
    public abstract class RuneRecipeWorker
    {
        /// <summary>
        /// Does the GUI for the Bill.
        /// </summary>
        /// <param name="inRect">Space to work with.</param>
        /// <param name="bill">The Bill to do it on.</param>
        public abstract void DoWorkerGUI(Rect inRect, RuneBill bill);

        /// <summary>
        /// Extra conditions for a pawn doing this recipe.
        /// </summary>
        /// <param name="pawn">Prospecting pawn doing this recipe.</param>
        /// <param name="recipe">Recipe to do it on.</param>
        /// <returns>True if all criterias are met.</returns>
        public abstract bool ExtraPawnCriterias(Pawn pawn, RuneBill bill);

        /// <summary>
        /// Can this Bill be started?
        /// </summary>
        /// <param name="bill">The Bill to check.</param>
        /// <returns>True if it can.</returns>
        public abstract bool CanStartBill(RuneBill bill);

        /// <summary>
        /// Finishes the Bill. Doing the resulting action.
        /// </summary>
        /// <param name="bill">The Bill being finished.</param>
        /// <param name="pawn">Pawn doing the finishing touch.</param>
        public abstract void FinishBill(RuneBill bill, Pawn pawn);
    }
}
