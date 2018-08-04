using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Runes
{
    /// <summary>
    /// Adds the market value from socketed runes.
    /// </summary>
    public class StatPart_SocketableMarketValue : StatPart
    {
        public float SocketableMarketValue(SocketComp socketable)
        {
            float result = 0f;

            foreach(Thing rune in socketable.socketedThings)
            {
                result += rune.MarketValue;
            }

            return result;
        }

        public override string ExplanationPart(StatRequest req)
        {
            if (req.HasThing && req.Thing.TryGetComp<SocketComp>() is SocketComp socketable && SocketableMarketValue(socketable) is float marketvalue && marketvalue > 0f)
            {
                return "RunesSocketedRunesMarketValue".Translate() + ": " + marketvalue;
            }

             return null;
        }

        public override void TransformValue(StatRequest req, ref float val)
        {
            if (req.HasThing && req.Thing.TryGetComp<SocketComp>() is SocketComp socketable)
            {
                val += SocketableMarketValue(socketable);
            }
        }
    }
}
