using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Runes
{
    [StaticConstructorOnStartup]
    public static class PostDefPatcher
    {
        static PostDefPatcher()
        {
            if (Prefs.DevMode)
            {
                Log.Message("==Patching in Runes to all ThingDefs==");
            }

            //Patch all weapons and apparel.
            foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs)
            {
                if(RuneUtility.ValidRuneTargetDef(thingDef))
                {
                    if(!thingDef.comps.Any(props => props is CompProperties_Socket))
                    {
                        thingDef.comps.Add(new CompProperties_Socket());

                        if (Prefs.DevMode)
                        {
                            Log.Message("Patched: '" + thingDef.defName + "'");
                        }
                    }
                    else
                    {
                        if (Prefs.DevMode)
                        {
                            Log.Message("[X] Already got 'CompProperties_Socket' the Def: '" + thingDef.defName + "'");
                        }
                    }
                }
            }
        }
    }
}
