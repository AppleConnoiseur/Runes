using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Runes
{
    public class CapacityImpactorRune : PawnCapacityUtility.CapacityImpactor
    {
        public override string Readable(Pawn pawn)
        {
            return string.Format("{0}", rune.parent.LabelCap);
        }

        public RuneComp rune;
    }
}
