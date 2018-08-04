using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace Runes
{
    /// <summary>
    /// Base class for rune manipulation JobDrivers.
    /// </summary>
    public abstract class JobDriver_ManipulateRune : JobDriver
    {
        /// <summary>
        /// Targeted item do manipulate rune on.
        /// </summary>
        public TargetIndex TargetItem => TargetIndex.A;

        /// <summary>
        /// The rune to manipulate. Can either be on the item or somewhere else.
        /// </summary>
        public TargetIndex TargetRune => TargetIndex.B;

        /// <summary>
        /// Area to do work at.
        /// </summary>
        public TargetIndex TargetWorkBench => TargetIndex.C;

        /// <summary>
        /// Thing item to manipulate.
        /// </summary>
        public Thing Item
        {
            get
            {
                return TargetThingA;
            }
        }

        /// <summary>
        /// Rune Thing to manipulate with.
        /// </summary>
        public Thing RuneThing
        {
            get
            {
                return TargetThingB;
            }
        }

        /// <summary>
        /// Rune component for convenience sake.
        /// </summary>
        public RuneComp Rune
        {
            get
            {
                return TargetThingB.TryGetComp<RuneComp>();
            }
        }

        /// <summary>
        /// Related workbench, if any.
        /// </summary>
        public Building_RuneWorkbench Workbench
        {
            get
            {
                return job.targetC.Thing as Building_RuneWorkbench;
            }
        }

        public override bool TryMakePreToilReservations()
        {
            //Do not bother reserving if its already equipped.
            bool reservedItem = false;
            bool reservedRune = false;

            if (Item != null)
            {
                if (Item.ParentHolder is Map)
                {
                    if (pawn.CanReserveAndReach(Item, PathEndMode.OnCell, Danger.Deadly))
                    {
                        pawn.Reserve(Item, job);
                        reservedItem = true;
                    }
                }
                else
                {
                    reservedItem = true;
                }
            }

            if(RuneThing != null)
            {
                if (RuneThing.ParentHolder is Map)
                {
                    if (pawn.CanReserveAndReach(RuneThing, PathEndMode.OnCell, Danger.Deadly))
                    {
                        pawn.Reserve(RuneThing, job);
                        reservedRune = true;
                    }
                }
                else if (RuneThing.ParentHolder is SocketComp)
                {
                    reservedRune = true;
                }
            }

            return reservedItem && reservedRune;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            bool itemIsInMap = Item.ParentHolder is Map;
            bool runeIsInMap = false;
            if(RuneThing != null)
            {
                runeIsInMap = RuneThing.ParentHolder is Map;
            }

            if (Workbench.def.hasInteractionCell)
            {
                this.AddFailCondition(() => !Workbench.InteractionCell.Standable(Workbench.Map));
            }

            //Add conditions
            if (itemIsInMap)
            {
                this.FailOnDestroyedNullOrForbidden(TargetItem);
            }
            if (runeIsInMap)
            {
                this.FailOnDestroyedNullOrForbidden(TargetRune);
            }
            if(Workbench != null)
            {
                this.FailOnDestroyedNullOrForbidden(TargetWorkBench);
            }

            //Reserve
            if (itemIsInMap)
            {
                yield return Toils_Reserve.Reserve(TargetItem);
            }
            if (runeIsInMap)
            {
                yield return Toils_Reserve.Reserve(TargetRune);
            }
            if (Workbench != null)
            {
                yield return Toils_Reserve.Reserve(TargetWorkBench);
            }

            //Walk and carry as needed.
            Toil endToil = new Toil();

            if (itemIsInMap)
            {
                yield return Toils_Goto.GotoThing(TargetItem, PathEndMode.OnCell);
                yield return Toils_Haul.StartCarryThing(TargetItem);
                if(job.GetTarget(TargetWorkBench).IsValid)
                {
                    if(Workbench.def.hasInteractionCell)
                    {
                        yield return Toils_Goto.GotoCell(Workbench.InteractionCell, PathEndMode.OnCell);
                    }
                    else
                    {
                        yield return Toils_Goto.Goto(TargetWorkBench, PathEndMode.ClosestTouch);
                    }
                    yield return Toils_Haul.PlaceHauledThingInCell(TargetWorkBench, endToil, false);
                }
            }

            if (runeIsInMap)
            {
                yield return Toils_Goto.GotoThing(TargetRune, PathEndMode.OnCell);

                if (!job.GetTarget(TargetWorkBench).IsValid)
                {
                    if(itemIsInMap)
                    {
                        yield return Toils_Haul.PlaceHauledThingInCell(TargetRune, endToil, false);
                    }
                }
                else
                {
                    yield return Toils_Haul.StartCarryThing(TargetRune);
                    if (Workbench.def.hasInteractionCell)
                    {
                        yield return Toils_Goto.GotoCell(Workbench.InteractionCell, PathEndMode.OnCell);
                    }
                    else
                    {
                        yield return Toils_Goto.Goto(TargetWorkBench, PathEndMode.ClosestTouch);
                    }
                    yield return Toils_Haul.PlaceHauledThingInCell(TargetWorkBench, endToil, false);
                }
            }

            //Here do work happen.
            foreach (Toil toil in MakeWorkToils(endToil))
                yield return toil;

            //Failure toil if hauling did not work as intended.
            yield return endToil;
        }

        /// <summary>
        /// Makes Toils depending on the context it is in.
        /// </summary>
        /// <param name="endToil">End Toil to jump to if things go wrong.</param>
        /// <returns>Work related Toils.</returns>
        public abstract IEnumerable<Toil> MakeWorkToils(Toil endToil);
    }
}
