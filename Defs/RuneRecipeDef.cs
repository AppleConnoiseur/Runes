using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Runes
{
    /// <summary>
    /// Recipe for handling runes.
    /// </summary>
    public class RuneRecipeDef : Def
    {
        /// <summary>
        /// Internal storage of the worker object.
        /// </summary>
        private RuneRecipeWorker workerInt;

        /// <summary>
        /// Recipe worker object.
        /// </summary>
        public RuneRecipeWorker Worker
        {
            get
            {
                if(workerClass != null && workerInt == null)
                {
                    workerInt = (RuneRecipeWorker)Activator.CreateInstance(workerClass);
                }

                return workerInt;
            }
        }

        /// <summary>
        /// The text format for the title of the recipe.
        /// </summary>
        public string reportString = "Manipulate {0} with {1} Recipe";

        /// <summary>
        /// How much work in ticks that are required to finish this recipe.
        /// </summary>
        public int workRequired = 100;

        /// <summary>
        /// Skill used in order to work with this.
        /// </summary>
        public SkillDef workSkill;

        /// <summary>
        /// Optional sound to play while working.
        /// </summary>
        public SoundDef workSound;

        /// <summary>
        /// Minimum skill level in order to do this recipe.
        /// </summary>
        public int minimumSkillLevel = 0;

        /// <summary>
        /// Do this recipe require a rune?
        /// </summary>
        public bool requiresRune = true;

        /// <summary>
        /// What class our worker object is.
        /// </summary>
        public Type workerClass;
    }
}
