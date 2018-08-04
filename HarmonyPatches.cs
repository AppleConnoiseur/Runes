using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Verse;

namespace Runes
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        //public static FieldInfo int_StatWorker_stat;

        static HarmonyPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("chjees.runes");

            //Patch StatWorkers
            {
                //Value
                Type type = typeof(StatWorker);
                MethodInfo method = type.GetMethod("GetValueUnfinalized");
                HarmonyMethod patchMethod = new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(Patch_StatWorker_GetValueUnfinalized)));
                harmony.Patch(method, null, patchMethod);
            }
            {
                //Explanation
                Type type = typeof(StatWorker);
                MethodInfo method = type.GetMethod("GetExplanationUnfinalized");
                HarmonyMethod patchMethod = new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(Patch_StatWorker_GetExplanationUnfinalized)));
                harmony.Patch(method, null, patchMethod);
            }

            //Patch PawnCapacityUtility
            {
                //CalculateCapacityLevel
                Type type = typeof(PawnCapacityUtility);
                MethodInfo method = type.GetMethod("CalculateCapacityLevel");
                HarmonyMethod patchMethod = new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(Patch_PawnCapacityUtility_CalculateCapacityLevel)));
                harmony.Patch(method, null, patchMethod);
            }

            //Patch HealthCardUtility
            {
                //GetPawnCapacityTip
                Type type = typeof(HealthCardUtility);
                MethodInfo method = type.GetMethod("GetPawnCapacityTip");
                HarmonyMethod patchMethod = new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(Patch_HealthCardUtility_GetPawnCapacityTip)));
                harmony.Patch(method, null, patchMethod);
            }

            //Patch PawnRenderer
            {
                //DrawEquipmentAiming
                Type type = typeof(PawnRenderer);
                MethodInfo method = type.GetMethod("DrawEquipmentAiming");
                HarmonyMethod patchMethod = new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(Patch_PawnRenderer_DrawEquipmentAiming)));
                harmony.Patch(method, null, patchMethod);
            }

            //Graphic
            {
                //DrawWorker
                Type type = typeof(Graphic);
                MethodInfo method = type.GetMethod("DrawWorker");
                HarmonyMethod patchMethod = new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(Patch_Graphic_DrawWorker)));
                harmony.Patch(method, null, patchMethod);
            }

            //Graphic_StackCount
            {
                //DrawWorker
                Type type = typeof(Graphic_StackCount);
                MethodInfo method = type.GetMethod("DrawWorker");
                HarmonyMethod patchMethod = new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(Patch_Graphic_DrawWorker)));
                harmony.Patch(method, null, patchMethod);
            }

            //Graphic_RandomRotated
            {
                //DrawWorker
                Type type = typeof(Graphic_RandomRotated);
                MethodInfo method = type.GetMethod("DrawWorker");
                HarmonyMethod patchMethod = new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(Patch_Graphic_RandomRotated_DrawWorker)));
                harmony.Patch(method, null, patchMethod);
            }

            //Verb_LaunchProjectile
            {
                //Projectile
                Type type = typeof(Verb_LaunchProjectile);
                PropertyInfo property = type.GetProperty("Projectile");
                MethodInfo method = property.GetGetMethod();
                HarmonyMethod patchMethod = new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(Patch_Verb_LaunchProjectile_GetProjectile)));
                harmony.Patch(method, patchMethod, null);
            }

            //Patch Projectile
            {
                //Explanation
                Type type = typeof(Projectile);
                MethodInfo method = type.GetMethod("Impact", BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance);
                HarmonyMethod patchMethod = new HarmonyMethod(typeof(HarmonyPatches).GetMethod(nameof(Patch_Projectile_Impact)));
                harmony.Patch(method, null, patchMethod);
            }

            harmony.PatchAll();
        }

        public static bool Patch_Verb_LaunchProjectile_GetProjectile(ref ThingDef __result, Verb_LaunchProjectile __instance)
        {
            if (__instance.EquipmentSource != null)
            {
                SocketComp comp = __instance.EquipmentSource.GetComp<SocketComp>();
                if (comp != null && comp.SocketedRunes.FirstOrDefault(runeComp => runeComp.RuneProps.replacedProjectile != null) is RuneComp rune)
                {
                    __result = rune.RuneProps.replacedProjectile;
                    return false;
                }
            }

            return true;
        }

        public static void Patch_Graphic_RandomRotated_DrawWorker(ref Graphic __instance, ref Vector3 loc, ref Rot4 rot, Thing thing, float extraRotation, float ___maxAngle)
        {
            float num = 0f;
            if (thing != null)
            {
                num = -___maxAngle + (float)(thing.thingIDNumber * 542) % (___maxAngle * 2f);
            }
            num += extraRotation;
            RuneUtility.DrawRuneAttachments(thing, ref loc, num, false, __instance, rot);
        }

        public static void Patch_Graphic_DrawWorker(ref Vector3 loc, Thing thing, float extraRotation)
        {
            RuneUtility.DrawRuneAttachments(thing, ref loc, extraRotation);
        }

        public static void Patch_PawnRenderer_DrawEquipmentAiming(Thing eq, ref Vector3 drawLoc, float aimAngle)
        {
            RuneUtility.DrawRuneAttachments(eq, ref drawLoc, aimAngle);
        }

        public static void Patch_HealthCardUtility_GetPawnCapacityTip(ref string __result, Pawn pawn, PawnCapacityDef capacity)
        {
            List<PawnCapacityUtility.CapacityImpactor> list = new List<PawnCapacityUtility.CapacityImpactor>();
            PawnCapacityUtility.CalculateCapacityLevel(pawn.health.hediffSet, capacity, list);
            if (list.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] is CapacityImpactorRune)
                    {
                        stringBuilder.AppendLine(string.Format("  {0}", list[i].Readable(pawn)));
                    }
                }

                __result += stringBuilder.ToString();
            }
        }

        public static void Patch_PawnCapacityUtility_CalculateCapacityLevel(ref float __result, HediffSet diffSet, PawnCapacityDef capacity, List<PawnCapacityUtility.CapacityImpactor> impactors)
        {
            IEnumerable<RuneComp> runes = diffSet.pawn.AllRunesOnPawn();
            if(runes != null)
            {
                foreach(RuneComp rune in runes)
                {
                    if(!rune.RuneProps.capMods.NullOrEmpty())
                    {
                        float num = __result;
                        if(num > 0f)
                        {
                            float num2 = 99999f;
                            float num3 = 1f;
                            foreach (PawnCapacityModifier pawnCapacityModifier in rune.RuneProps.capMods)
                            {
                                if (pawnCapacityModifier.capacity == capacity)
                                {
                                    num += pawnCapacityModifier.offset;
                                    num3 *= pawnCapacityModifier.postFactor;
                                    if (pawnCapacityModifier.setMax < num2)
                                    {
                                        num2 = pawnCapacityModifier.setMax;
                                    }
                                    if (impactors != null)
                                    {
                                        impactors.Add(new CapacityImpactorRune()
                                        {
                                            rune = rune
                                        });
                                    }
                                }
                            }
                            num *= num3;
                            num = Mathf.Min(num, num2);
                            __result = num;
                        }
                    }
                }
            }
        }

        public static void Patch_Projectile_Impact(Projectile __instance, Thing hitThing, Thing ___launcher)
        {
            //Thing launcher = (Thing)AccessTools.Field(typeof(Projectile), "launcher").GetValue(__instance);
            if(___launcher != null)
            {
                //Log.Message("Patch: launcher: " + launcher.ToString());
                //Output: Patch: launcher: Cyrus
                if(___launcher is Pawn pawn)
                {
                    Thing weapon = pawn.equipment.AllEquipmentListForReading.FirstOrDefault(eqThing => eqThing.def == (ThingDef)AccessTools.Field(typeof(Projectile), "equipmentDef").GetValue(__instance));
                    if(weapon != null)
                    {
                        if (weapon.TryGetComp<SocketComp>() is SocketComp socketable)
                        {
                            foreach (RuneComp rune in socketable.SocketedRunes)
                            {
                                if (rune.RuneProps.special is SpecialUpgrade special)
                                {
                                    special.Event_Projectile_Impact(rune, __instance, hitThing, ___launcher, weapon);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void Patch_StatWorker_GetValueUnfinalized(StatWorker __instance, ref float __result, ref StatRequest req, bool applyPostProcess, StatDef ___stat)
        {
            //StatDef stat = (StatDef)AccessTools.Field(typeof(StatWorker), "stat").GetValue(__instance);

            if (req.Thing is ThingWithComps thing && thing.TryGetComp<SocketComp>() is SocketComp socketable)
            {
                foreach(RuneComp rune in socketable.SocketedRunes)
                {
                    if(rune.RuneProps.statOffsets.FirstOrDefault(mod => mod.stat == ___stat) is StatModifier statModifer)
                    {
                        __result += statModifer.value;
                    }
                }
            }
        }

        public static void Patch_StatWorker_GetExplanationUnfinalized(StatWorker __instance, ref string __result, ref StatRequest req, ToStringNumberSense numberSense, StatDef ___stat)
        {
            //StatDef stat = (StatDef)AccessTools.Field(typeof(StatWorker), "stat").GetValue(__instance);

            if (req.Thing is ThingWithComps thing && thing.TryGetComp<SocketComp>() is SocketComp socketable)
            {
                StringBuilder builder = new StringBuilder(__result);
                bool statChanged = false;

                foreach (RuneComp rune in socketable.SocketedRunes)
                {
                    if (rune.RuneProps.statOffsets.FirstOrDefault(mod => mod.stat == ___stat) is StatModifier statModifer)
                    {
                        if(!statChanged)
                        {
                            builder.AppendLine();
                            builder.AppendLine();
                            builder.AppendLine("Runes");
                        }

                        builder.AppendLine("    " + rune.parent.LabelCapNoCount + ": " + statModifer.value.ToStringByStyle(statModifer.stat.ToStringStyleUnfinalized, ToStringNumberSense.Offset));
                        statChanged = true;
                    }
                }

                if(statChanged)
                {
                    __result = builder.ToString().TrimEndNewlines();
                }
            }
        }
    }
}
