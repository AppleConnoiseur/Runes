using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Runes
{
    /// <summary>
    /// Socketable component. Holds runes.
    /// </summary>
    public class SocketComp : ThingComp, IThingHolder
    {
        /// <summary>
        /// Things which are socketed in the item.
        /// </summary>
        public ThingOwner socketedThings;

        /// <summary>
        /// How many sockets this item got.
        /// </summary>
        public int availableSockets = 1;

        /// <summary>
        /// Runes which are socketed in the item.
        /// </summary>
        public IEnumerable<RuneComp> SocketedRunes
        {
            get
            {
                foreach(Thing thing in socketedThings)
                {
                    if (thing.TryGetComp<RuneComp>() is RuneComp rune)
                        yield return rune;
                }
            }
        }

        /// <summary>
        /// Whether or not this can fit any more runes in it.
        /// </summary>
        public bool CanBeSocketed
        {
            get
            {
                return availableSockets + SocketProps.bonusSockets > 0;
            }
        }

        /// <summary>
        /// Slots gained or lost from quality.
        /// </summary>
        public int SlotsFromQuality
        {
            get
            {
                int result = 0;
                if(parent.TryGetComp<CompQuality>() is CompQuality quality)
                {
                    if(quality.Quality <= QualityCategory.Poor)
                    {
                        result = -1;
                    }
                    else if(quality.Quality == QualityCategory.Masterwork)
                    {
                        result = 1;
                    }
                    else if (quality.Quality == QualityCategory.Legendary)
                    {
                        result = 2;
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Total amount slots which can be socketed.
        /// </summary>
        public int TotalAmountOfSlots
        {
            get
            {
                return availableSockets + SlotsFromQuality + SocketProps.bonusSockets;
            }
        }

        /// <summary>
        /// Total amount of slots that are free.
        /// </summary>
        public int FreeSlots
        {
            get
            {
                return TotalAmountOfSlots - socketedThings.Count;
            }
        }

        /// <summary>
        /// Socket component properties.
        /// </summary>
        public CompProperties_Socket SocketProps
        {
            get
            {
                return props as CompProperties_Socket;
            }
        }

        public SocketComp()
        {
            socketedThings = new ThingOwner<Thing>(this, false, LookMode.Deep);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look(ref availableSockets, "availableSockets");
            Scribe_Deep.Look(ref socketedThings, "socketedThings", this, false, LookMode.Deep);
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            //None
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return socketedThings;
        }

        public bool SocketRune(Thing rune)
        {
            if(FreeSlots > 0)
            {
                rune.DeSpawn();
                socketedThings.TryAddOrTransfer(rune);
                rune.TryGetComp<RuneComp>()?.Notify_RuneInserted(this);

                return true;
            }

            return false;
        }

        public Thing RemoveRune(Thing rune)
        {
            if(socketedThings.Contains(rune))
            {
                rune.TryGetComp<RuneComp>()?.Notify_RuneRemoved(this);
                return socketedThings.Take(rune);
            }

            return null;
        }

        public virtual void AddSocket(Pawn pawn = null)
        {
            availableSockets++;
        }

        public virtual void RemoveSocket(Pawn pawn = null)
        {
            availableSockets--;
            if (availableSockets < 0)
            {
                availableSockets = 0;
            }
        }

        public override string TransformLabel(string label)
        {
            if (socketedThings.Count <= 0)
            {
                return base.TransformLabel(label);
            }

            var runePrefixes = SocketedRunes?.Where(rune => rune.RuneProps.prefix != null);
            var runeSufffixes = SocketedRunes?.Where(rune => rune.RuneProps.suffix != null);

            if(runePrefixes != null)
            {
                RuneComp resultRune = runePrefixes.OrderByDescending(rune => rune.RuneProps.labelPower).FirstOrDefault();
                if(resultRune != null)
                {
                    label = resultRune.RuneProps.prefix + " " + label;
                }
            }

            if (runePrefixes != null)
            {
                RuneComp resultRune = runeSufffixes.OrderByDescending(rune => rune.RuneProps.labelPower).FirstOrDefault();
                if (resultRune != null)
                {
                    if(label.Contains('('))
                    {
                        label = label.Insert(label.LastIndexOf('(') - 1, " " +resultRune.RuneProps.suffix);
                    }
                    else
                    {
                        label = label + resultRune.RuneProps.suffix;
                    }
                }
            }

            return label;
        }

        public override string GetDescriptionPart()
        {
            if (socketedThings.Count <= 0)
                return null;

            StringBuilder builder = new StringBuilder();

            builder.AppendLine("RunesSocketableLabel".Translate());
            if (SocketProps.bonusSockets > 0)
            {
                builder.AppendLine("    " + "RunesSocketableBonusSockets".Translate() + ": " + SocketProps.bonusSockets);
            }
            if (SlotsFromQuality != 0)
            {
                builder.AppendLine("    " + "RunesSocketableSlotsQuality".Translate() + ": " + GenText.ToStringByStyle(SlotsFromQuality, ToStringStyle.Integer, ToStringNumberSense.Offset));
            }
            builder.AppendLine("    " + "RunesSocketableTotalAmountOfSockets".Translate() + ": " + TotalAmountOfSlots);
            builder.AppendLine("    " + "RunesSocketableFreeSockets".Translate() + ": " + FreeSlots);

            if(SocketedRunes.Count() > 0)
            {
                builder.AppendLine();
                builder.AppendLine();
            }

            foreach (RuneComp rune in SocketedRunes)
            {
                builder.AppendLine(rune.GetRuneDescription());
            }

            return builder.ToString();
        }
    }
}
