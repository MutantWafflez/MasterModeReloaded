using MasterModeReloaded.Content.NPCs;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace MasterModeReloaded.Common {
    /// <summary>
    /// Class that holds all the Detours and IL Edits of MMR.
    /// </summary>
    public static class Patches {

        public static void ApplyDetourPatches() {
            //So the PreVanillaAI() method is called before the vanilla AI without touching PreAI()
            On.Terraria.NPC.VanillaAI += NPC_VanillaAI;

            //So Vanilla interprets the EOL as in her Vanilla Phase 2 in MMR's Phase 2/3
            On.Terraria.NPC.AI_120_HallowBoss_IsInPhase2 += NPC_AI_120_HallowBoss_IsInPhase2;
        }

        public static void ApplyILPatches() {
            //EOL MMR Phase 2 visuals
            IL.Terraria.Main.DrawNPCDirect_HallowBoss += Main_DrawNPCDirect_HallowBoss;
        }

        #region Detour Methods
        private static void NPC_VanillaAI(On.Terraria.NPC.orig_VanillaAI orig, NPC self) {
            MMRGlobalNPC globalNPC = self.GetGlobalNPC<MMRGlobalNPC>();
            if (NPCLoader.PreAI(self) && Main.masterMode) {
                globalNPC.currentMMRAI?.PreVanillaAI(self);
            }
            orig(self);
        }

        private static bool NPC_AI_120_HallowBoss_IsInPhase2(On.Terraria.NPC.orig_AI_120_HallowBoss_IsInPhase2 orig, NPC self) {
            return self.ai[3] == 1f || self.ai[3] == 3f || self.ai[3] >= 4f;
        }
        #endregion

        #region IL Methods
        private static void Main_DrawNPCDirect_HallowBoss(ILContext il) {
            ILCursor c = new ILCursor(il);

            byte Num4LocalValue = 19;
            byte Color3LocalValue = 27;

            //Changes Red to the color the EOL shines in her MMR phases
            if (c.TryGotoNext(i => i.MatchCallvirt(typeof(NPC).GetProperty(nameof(NPC.Opacity)).GetMethod))) {
                c.Index += 4;

                ILLabel label = c.DefineLabel();

                c.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_1);
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldfld, typeof(NPC).GetField(nameof(NPC.ai), BindingFlags.Public | BindingFlags.Instance));
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4_3);
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldelem_R4);
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldc_R4, 4f);
                c.Emit(Mono.Cecil.Cil.OpCodes.Blt_Un_S, label);

                c.Emit(Mono.Cecil.Cil.OpCodes.Call, typeof(Color).GetProperty(nameof(Color.Red), BindingFlags.Public | BindingFlags.Static).GetMethod);
                c.Emit(Mono.Cecil.Cil.OpCodes.Stloc_S, Color3LocalValue);

                c.Emit(Mono.Cecil.Cil.OpCodes.Ldloca_S, Color3LocalValue);
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4_0);
                c.Emit(Mono.Cecil.Cil.OpCodes.Call, typeof(Color).GetProperty(nameof(Color.A), BindingFlags.Public | BindingFlags.Instance).SetMethod);

                c.Emit(Mono.Cecil.Cil.OpCodes.Ldloc_S, Color3LocalValue);
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldc_R4, 0.67f);
                c.Emit(Mono.Cecil.Cil.OpCodes.Call, typeof(Color).GetMethod("op_Multiply", BindingFlags.Public | BindingFlags.Static));
                c.Emit(Mono.Cecil.Cil.OpCodes.Stloc_S, Color3LocalValue);

                c.Emit(Mono.Cecil.Cil.OpCodes.Ldloc_S, Color3LocalValue);
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldloc_S, Num4LocalValue);
                c.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_1);
                c.Emit(Mono.Cecil.Cil.OpCodes.Callvirt, typeof(NPC).GetProperty(nameof(NPC.Opacity)).GetMethod);
                c.Emit(Mono.Cecil.Cil.OpCodes.Mul);
                c.Emit(Mono.Cecil.Cil.OpCodes.Call, typeof(Color).GetMethod("op_Multiply", BindingFlags.Public | BindingFlags.Static));
                c.Emit(Mono.Cecil.Cil.OpCodes.Stloc_S, Color3LocalValue);

                c.MarkLabel(label);
            }

            /* ^This is what this IL is translated to:
             if (rCurrentNPC.ai[3] >= 4f) {
                color3 = Color.Red;
                color3.A = 0;
                color3 *= 0.67f;
                color3 *= num4 * rCurrentNPC.Opacity;
             }
             */
            #endregion
        }
    }
}