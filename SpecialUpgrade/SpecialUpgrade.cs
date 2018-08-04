using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Runes
{
    /// <summary>
    /// Makes special effects happen in runes.
    /// </summary>
    public abstract class SpecialUpgrade
    {
        /// <summary>
        /// If more than one upgrade stacks the final result can give dimnishing returns.
        /// </summary>
        /// <param name="other">Other special upgrade to compare with.</param>
        /// <returns>True if it stacks.</returns>
        public abstract bool StacksWith(SpecialUpgrade other);

        /// <summary>
        /// Event: On Projectile Impact.
        /// </summary>
        /// <param name="rune">Rune associated with the impact.</param>
        /// <param name="projectile">Projectile impacting.</param>
        /// <param name="hitThing">What got hit, if any.</param>
        public virtual void Event_Projectile_Impact(RuneComp rune, Projectile projectile, Thing hitThing, Thing launcher, Thing weapon) {  }

        /// <summary>
        /// Makes a optional description part for this upgrade.
        /// </summary>
        /// <returns>Any extra descriptions.</returns>
        public virtual string GetDescriptionPart(RuneComp rune) { return null; }
    }
}
