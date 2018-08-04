using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace Runes
{
    public class RuneComp : ThingComp
    {
        public CompProperties_Rune RuneProps
        {
            get
            {
                return props as CompProperties_Rune;
            }
        }

        public Graphic OverlayGraphic
        {
            get
            {
                if(RuneProps.overlayGraphicData != null)
                {
                    return RuneProps.overlayGraphicData.Graphic;
                }

                return null;
            }
        }

        public virtual void Notify_RuneInserted(SocketComp socketable)
        {

        }

        public virtual void Notify_RuneRemoved(SocketComp socketable)
        {

        }

        public bool CanBeSocketedInto(Thing thing)
        {
            //Must.
            if (thing == null)
            {
                return false;
            }
            if (RuneProps.restrictedTo.Count > 0 && !RuneProps.restrictedTo.Contains(thing.def))
            {
                return false;
            }

            //Uniqueness.
            if(RuneProps.uniqueGroup != null && thing.TryGetComp<SocketComp>() is SocketComp socketable && socketable.SocketedRunes != null &&
                socketable.SocketedRunes.Any(rune => rune.RuneProps.uniqueGroup != null && rune.RuneProps.uniqueGroup == RuneProps.uniqueGroup))
            {
                return false;
            }

            //Either or.
            bool validEquipment = false;
            if (RuneProps.socketIntoWeapon && thing.def.equipmentType == EquipmentType.Primary)
            {
                validEquipment = true;
            }
            if (RuneProps.socketIntoApparel && thing.def.thingClass == typeof(Apparel))
            {
                validEquipment = true;
            }

            return validEquipment;
        }

        public string GetRuneDescription()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(parent.LabelCapNoCount);
            //Special upgrade
            if(RuneProps.special != null && RuneProps.special.GetDescriptionPart(this) is string description)
            {
                builder.AppendLine("RunesSpecialCategory".Translate() + ":");
                builder.AppendLine(description.TrimEndNewlines());
            }

            //Projectile replacement
            if (RuneProps.replacedProjectile != null)
            {
                builder.AppendLine("RunesProjectileCategory".Translate() + ":");
                builder.AppendLine("    " + "RunesProjectileReplacement".Translate() + ": " + RuneProps.replacedProjectile.LabelCap);
            }

            //Capacitiy modifiers
            if(RuneProps.capMods.Count > 0)
            {
                builder.AppendLine("RunesCapacityModifierCategory".Translate() + ":");
            }
            foreach (PawnCapacityModifier capacitiyModifer in RuneProps.capMods)
            {
                if(capacitiyModifer.offset != 0f)
                {
                    builder.AppendLine("    " + "HealthOffsetScale".Translate(capacitiyModifer.capacity.LabelCap) + ": " + capacitiyModifer.offset.ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Offset));
                }
                if (capacitiyModifer.setMax != 999f)
                {
                    builder.AppendLine("    " + "HealthFactorMaxImpact".Translate(capacitiyModifer.capacity.LabelCap) + ": " + capacitiyModifer.setMax.ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Absolute));
                }
            }

            //Stat offsets
            if (RuneProps.statOffsets.Count > 0)
            {
                builder.AppendLine("RunesStatOffsetsCategory".Translate() + ":");
            }
            foreach (StatModifier statModifer in RuneProps.statOffsets)
            {
                builder.AppendLine("    " + statModifer.stat.LabelCap + ": " + statModifer.value.ToStringByStyle(statModifer.stat.ToStringStyleUnfinalized, ToStringNumberSense.Offset));
            }

            return builder.ToString().TrimEndNewlines();
        }

        public override string GetDescriptionPart()
        {
            return GetRuneDescription();
        }

        /*public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            if(selPawn.IsColonistPlayerControlled)
            {
                yield return new FloatMenuOption("Socket into something on ground",
                delegate ()
                {
                    Find.Targeter.BeginTargeting(new TargetingParameters
                    {
                        canTargetPawns = false,
                        canTargetBuildings = false,
                        canTargetItems = true,
                        mapObjectTargetsMustBeAutoAttackable = false,
                        validator = ((TargetInfo x) => x.Thing != null && RuneUtility.ValidRuneTargetDef(x.Thing.def))
                    },
                    delegate (LocalTargetInfo target)
                    {
                        //Give job.
                        selPawn.jobs.TryTakeOrderedJob(
                            new Job(RuneJobDefOf.Runes_InsertRune, target, parent)
                            {
                                count = 1
                            });
                    });
                });

                foreach(Thing thing in selPawn.equipment.AllEquipmentListForReading)
                {
                    yield return new FloatMenuOption("Socket into '" + thing.LabelCapNoCount + "'.",
                        delegate()
                        {
                            //Give job.
                            selPawn.jobs.TryTakeOrderedJob(
                                new Job(RuneJobDefOf.Runes_InsertRune, thing, parent)
                                {
                                    count = 1
                                });
                        });
                }

                foreach (Thing thing in selPawn.apparel.WornApparel)
                {
                    yield return new FloatMenuOption("Socket into '" + thing.LabelCapNoCount + "'.",
                        delegate ()
                        {
                            //Give job.
                            selPawn.jobs.TryTakeOrderedJob(
                                new Job(RuneJobDefOf.Runes_InsertRune, thing, parent)
                                {
                                    count = 1
                                });
                        });
                }
            }
        }*/
    }
}
