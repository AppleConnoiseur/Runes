using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace Runes
{
    /// <summary>
    /// Job driver for removing runes from sockets.
    /// </summary>
    public class JobDriver_RemoveRune : JobDriver_ManipulateRune
    {
        public override IEnumerable<Toil> MakeWorkToils(Toil endToil)
        {
            yield return Toils_General.Wait(100).WithProgressBarToilDelay(TargetIndex.A, false);
            Toil workToil = new Toil();
            workToil.initAction = delegate ()
            {
                SocketComp socket = Item.TryGetComp<SocketComp>();
                if (socket != null)
                {
                    socket.RemoveRune(RuneThing);
                }
            };
            yield return workToil;
        }
    }
}
