using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Runes
{
    public class CompTargetable_Rune : CompTargetable
    {
        protected override bool PlayerChoosesTarget => true;

        public override IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null)
        {
            yield return targetChosenByPlayer;
            yield break;
        }

        protected override TargetingParameters GetTargetingParameters()
        {
            return new TargetingParameters
            {
                canTargetPawns = false,
                canTargetBuildings = false,
                canTargetItems = true,
                mapObjectTargetsMustBeAutoAttackable = false,
                validator = ((TargetInfo x) => x.Thing != null && Validator(x.Thing))
            };
        }

        public bool Validator(Thing thing)
        {
            //Log.Message("Validating: " + thing.LabelCap + ", result=" + RuneUtility.ValidRuneTargetDef(thing.def));
            return RuneUtility.ValidRuneTargetDef(thing.def);
        }
    }
}
