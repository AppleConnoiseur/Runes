using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Runes
{
    public static class RuneUtility
    {
        public static bool ValidRuneTargetDef(ThingDef thingDef)
        {
            return thingDef.stackLimit <= 1 && (thingDef.equipmentType == EquipmentType.Primary || thingDef.thingClass == typeof(Apparel));
        }

        public static IEnumerable<RuneComp> AllRunesOnPawn(this Pawn pawn)
        {
            if (pawn == null)
                yield break;

            if (pawn.equipment != null)
            {
                foreach(Thing weapon in pawn.equipment.AllEquipmentListForReading)
                {
                    if(weapon.TryGetComp<SocketComp>() is SocketComp socketable)
                    {
                        foreach(RuneComp rune in socketable.SocketedRunes)
                        {
                            yield return rune;
                        }
                    }
                }
            }
            if (pawn.apparel != null)
            {
                foreach (Thing apparel in pawn.apparel.WornApparel)
                {
                    if (apparel.TryGetComp<SocketComp>() is SocketComp socketable)
                    {
                        foreach (RuneComp rune in socketable.SocketedRunes)
                        {
                            yield return rune;
                        }
                    }
                }
            }
        }

        public static void DrawRuneAttachments(Thing eq, ref Vector3 drawLoc, float aimAngle, bool useAimingRotation = true, Graphic graphic = null, Rot4 rot = new Rot4())
        {
            SocketComp socketable = eq.TryGetComp<SocketComp>();
            if (socketable == null)
            {
                return;
            }

            //Vanilla routines.
            Mesh mesh = null;
            float num = aimAngle;

            if(useAimingRotation)
            {
                num = aimAngle - 90f;

                if (aimAngle > 20f && aimAngle < 160f)
                {
                    mesh = MeshPool.plane10;
                    num += eq.def.equippedAngleOffset;
                }
                else if (aimAngle > 200f && aimAngle < 340f)
                {
                    mesh = MeshPool.plane10Flip;
                    num -= 180f;
                    num -= eq.def.equippedAngleOffset;
                }
                else
                {
                    mesh = MeshPool.plane10;
                    num += eq.def.equippedAngleOffset;
                }
                num %= 360f;
            }
            else
            {
                mesh = graphic.MeshAt(rot);
            }

            //Custom drawing.
            Vector3 runeDrawLoc = drawLoc + new Vector3(0f, 0.01f, 0f);

            foreach (RuneComp rune in socketable.SocketedRunes)
            {
                if (rune.OverlayGraphic != null)
                {
                    Material matSingle = rune.OverlayGraphic.MatSingle;
                    Graphics.DrawMesh(mesh, runeDrawLoc, Quaternion.AngleAxis(num, Vector3.up), matSingle, 0);
                }
            }
        }
    }
}
