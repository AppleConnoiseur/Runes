using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Runes
{
    public class CompProperties_Rune : CompProperties
    {
        public CompProperties_Rune()
        {
            compClass = typeof(RuneComp);
        }

        //Labeling
        /// <summary>
        /// Optional prefix to appear before item names.
        /// </summary>
        public string prefix = null;

        /// <summary>
        /// Optional suffix to appear after the item names.
        /// </summary>
        public string suffix = null;

        /// <summary>
        /// If more than one rune got a prefix or suffix it will pick the one with the highest power.
        /// </summary>
        public int labelPower = 0;

        //Restriction
        /// <summary>
        /// Can this be socketed into a weapon?
        /// </summary>
        public bool socketIntoWeapon = true;

        /// <summary>
        /// Can this be socketed into apparel?
        /// </summary>
        public bool socketIntoApparel = true;

        /// <summary>
        /// Restricts this rune to only these things.
        /// </summary>
        public List<ThingDef> restrictedTo = new List<ThingDef>();

        /// <summary>
        /// There can be only of these socketed at all times in this unique group.
        /// </summary>
        public string uniqueGroup = null;

        //Modifiers
        /// <summary>
        /// Offsets the stats of the thing it is attached to by these.
        /// </summary>
        public List<StatModifier> statOffsets = new List<StatModifier>();

        /// <summary>
        /// Modifies the capacities of the wielder of the weapon or apparel it is socketed into by these.
        /// </summary>
        public List<PawnCapacityModifier> capMods = new List<PawnCapacityModifier>();
        

        /// <summary>
        /// If not null then this rune got a special upgrade.
        /// </summary>
        public SpecialUpgrade special = null;

        /// <summary>
        /// If not null this will replace the projectile of the Verb_LaunchProjectile.
        /// </summary>
        public ThingDef replacedProjectile = null;

        //Graphical
        /// <summary>
        /// Overlay effect for the rune on the weapon.
        /// </summary>
        public GraphicData overlayGraphicData;

        public override void ResolveReferences(ThingDef parentDef)
        {
            base.ResolveReferences(parentDef);

            if (overlayGraphicData != null)
            {
                overlayGraphicData.ResolveReferencesSpecial();
            }
        }
    }
}
