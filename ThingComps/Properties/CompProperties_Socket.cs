using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Runes
{
    public class CompProperties_Socket : CompProperties
    {
        public CompProperties_Socket()
        {
            compClass = typeof(SocketComp);
        }

        /// <summary>
        /// How many bonus sockets this socketable start with.
        /// </summary>
        public int bonusSockets = 0;

        //public VerbProperties superchargedVerb = null;
    }
}
