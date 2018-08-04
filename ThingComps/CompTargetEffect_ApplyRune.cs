using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Runes
{
    public class CompTargetEffect_ApplyRune : CompTargetEffect
    {
        public override void DoEffectOn(Pawn user, Thing target)
        {
            LocalTargetInfo reserved = user.Map.reservationManager.FirstReservationFor(user);
            if(reserved.Thing is Thing thing)
            {
                //Apply it to target.
                if(target.TryGetComp<SocketComp>() is SocketComp socketable)
                {
                    socketable.SocketRune(thing);
                }
            }
        }
    }
}
