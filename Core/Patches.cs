using System;
using System.Linq;
using System.Reflection;
using MasterModeReloaded.Common;
using MasterModeReloaded.Common.ModMenus;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace MasterModeReloaded.Core {

    /// <summary>
    /// Class that holds all the Detours and IL Edits of MMR.
    /// </summary>
    public static class Patches {

        public static void ApplyDetourPatches() {
            //So the PreVanillaAI() method is called before the vanilla AI without touching PreAI() (temporary cause the IL is broken)
            On.Terraria.NPC.VanillaAI += NPC_VanillaAI;

            //So Vanilla interprets the EOL as in her Vanilla Phase 2 in MMR's Phase 2/3
            On.Terraria.NPC.AI_120_HallowBoss_IsInPhase2 += NPC_AI_120_HallowBoss_IsInPhase2;
        }

        public static void ApplyILPatches() {
            //So the PreVanillaAI() method is called before the vanilla AI without touching PreAI()
            //IL.Terraria.NPC.VanillaAI += NPC_VanillaAI;

            //EOL MMR Phase 2 visuals
            IL.Terraria.Main.DrawNPCDirect_HallowBoss += Main_DrawNPCDirect_HallowBoss;

            //Add eclipse in MMR ModMenu
            IL.Terraria.Main.DrawMenu += Main_DrawMenu;
        }

        public static void UnloadDetourPatches() {
            On.Terraria.NPC.VanillaAI -= NPC_VanillaAI;

            On.Terraria.NPC.AI_120_HallowBoss_IsInPhase2 -= NPC_AI_120_HallowBoss_IsInPhase2;
        }

        public static void UnloadILPatches() {
            //IL.Terraria.NPC.VanillaAI -= NPC_VanillaAI;

            IL.Terraria.Main.DrawNPCDirect_HallowBoss -= Main_DrawNPCDirect_HallowBoss;

            IL.Terraria.Main.DrawMenu -= Main_DrawMenu;
        }

        #region Detour Methods

        private static void NPC_VanillaAI(On.Terraria.NPC.orig_VanillaAI orig, NPC self) {
            if (self.GetMMRGlobalNPC().currentMMRAI != null && NPCLoader.PreAI(self) && Main.masterMode) {
                self.GetMMRGlobalNPC().currentMMRAI.PreVanillaAI(self);
            }
            orig(self);
        }

        private static bool NPC_AI_120_HallowBoss_IsInPhase2(On.Terraria.NPC.orig_AI_120_HallowBoss_IsInPhase2 orig, NPC self) {
            return self.ai[3] == 1f || self.ai[3] == 3f || self.ai[3] >= 4f;
        }

        #endregion

        #region IL Methods

        /* Removed until I can figure out what in God's name is wrong with this IL upon reloading
        private static void NPC_VanillaAI(ILContext il) {
            ILCursor c = new ILCursor(il);

            ILLabel falseLabel = c.DefineLabel();

            c.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_0);
            c.EmitDelegate<Func<NPC, bool>>(npc => npc.GetMMRGlobalNPC().currentMMRAI != null && NPCLoader.PreAI(npc) && Main.masterMode);
            c.Emit(Mono.Cecil.Cil.OpCodes.Brfalse_S, falseLabel);

            c.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_0);
            c.EmitDelegate<Func<NPC, MMRGlobalNPC>>(npc => npc.GetMMRGlobalNPC());
            c.Emit(Mono.Cecil.Cil.OpCodes.Ldfld, typeof(MMRGlobalNPC).GetField(nameof(MMRGlobalNPC.currentMMRAI), BindingFlags.Public | BindingFlags.Instance));
            c.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_0);
            c.Emit(Mono.Cecil.Cil.OpCodes.Callvirt, typeof(MMRAI).GetMethod(nameof(MMRAI.PreVanillaAI), BindingFlags.Public | BindingFlags.Instance));

            c.MarkLabel(falseLabel);

            /* ^This is what this IL is translated to (with "this" referring to the given NPC instance):
            if (this.GetMMRGlobalNPC().currentMMRAI != null && NPCLoader.PreAI(this) && Main.masterMode) {
                this.GetMMRGlobalNPC().currentMMRAI.PreVanillaAI(this);
             }
             *
        }
        */

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
        }

        private static void Main_DrawMenu(ILContext il) {
            ILCursor c = new ILCursor(il);

            c.Goto(c.Body.Instructions.Last());

            ILLabel lastInstructionLabel = c.DefineLabel();

            c.EmitDelegate<Func<bool>>(() => {
                return MenuLoader.CurrentMenu.DisplayName == MMRModMenu.PublicDisplayName;
            });
            c.Emit(Mono.Cecil.Cil.OpCodes.Brfalse_S, lastInstructionLabel);

            c.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4_1);
            c.Emit(Mono.Cecil.Cil.OpCodes.Stsfld, typeof(Main).GetField(nameof(Main.eclipse), BindingFlags.Public | BindingFlags.Static));

            c.MarkLabel(lastInstructionLabel);

            /* ^This IL simply translates to:
               if (MenuLoader.CurrentMenu.DisplayName == MMRModMenu.PublicDisplayName) {
                   Main.eclipse = true;
               }
            */
        }

        #endregion
    }
}