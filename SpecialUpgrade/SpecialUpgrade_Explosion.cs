using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Runes
{
    public class SpecialUpgrade_Explosion : SpecialUpgrade
    {
        public float radius = 1.5f;
        public DamageDef damageDef;
        public int damageAmount = -1;
        public int scaleToDamage = 25;
        public float maximumScalingFactor = 1.5f;

        public override bool StacksWith(SpecialUpgrade other)
        {
            return other != null && other.GetType() == GetType();
        }

        public override string GetDescriptionPart(RuneComp rune)
        {
            StringBuilder builder = new StringBuilder();
            if (damageDef != null)
                builder.AppendLine("    " + "RunesSpecialUpgradeExplosionDamageType".Translate() + ": " + damageDef.LabelCap);
            builder.AppendLine("    " + "RunesSpecialUpgradeExplosionRadius".Translate() + ": " + radius);
            if(damageAmount > -1f)
                builder.AppendLine("    " + "RunesSpecialUpgradeExplosionDamage".Translate() + ": " + damageAmount);
            if (scaleToDamage != 0)
            {
                builder.AppendLine("    " + "RunesSpecialUpgradeExplosionScaleToDamage".Translate() + ": " + scaleToDamage);
                if (maximumScalingFactor != -1f)
                {
                    builder.AppendLine("    " + "RunesSpecialUpgradeExplosionMaximumScalingFactor".Translate() + ": " + maximumScalingFactor.ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Absolute));
                }
            }
            return builder.ToString();
        }

        public override void Event_Projectile_Impact(RuneComp rune, Projectile projectile, Thing hitThing, Thing launcher, Thing weapon)
        {
            //Log.Message("launcher: " + launcher.ToString());
            int finalDamage = damageAmount;
            float finalRadius = radius;

            if (scaleToDamage != 0)
            {
                float damageScale = (float)projectile.def.projectile.GetDamageAmount(weapon) / (float)scaleToDamage;
                if(maximumScalingFactor != -1f && damageScale > maximumScalingFactor)
                {
                    damageScale = maximumScalingFactor;
                }

                finalDamage = (int)Math.Ceiling(damageAmount * damageScale);
                finalRadius = (float)Math.Ceiling(finalRadius * damageScale);
            }

            GenExplosion.DoExplosion(projectile.Position, launcher.Map, finalRadius, damageDef, launcher,
                damAmount: finalDamage);
        }
    }
}
