using MasterModeReloaded.Common.ID;
using MasterModeReloaded.Content.NPCs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MasterModeReloaded {
    public class MasterModeReloaded : Mod {

        public const bool DebugMode = false;

        public static List<MMRAI> ListOfMMRAI;

        #region Loading
        public override void Load() {

            #region Shaders
            if (Main.netMode != NetmodeID.Server) {
                /*Ref<Effect> verticalRef = new Ref<Effect>((Effect)GetEffect("Effects/Filters/VerticalMirror"));
                Filters.Scene["VerticalMirror"] = new Filter(new ScreenShaderData(verticalRef, "VerticallyMirror"), EffectPriority.Medium);
                Filters.Scene["VerticalMirror"].Load();*/
            }
            #endregion

            #region Detours
            //So the PreVanillaAI() method is called before the vanilla AI without touching PreAI()
            On.Terraria.NPC.VanillaAI += NPC_VanillaAI;
            #endregion
        }

        public override void Unload() {
            #region Static Resets
            ListOfMMRAI = null;
            #endregion
        }

        public override void PostSetupContent() {
            ListOfMMRAI = new List<MMRAI>();

            Type[] typesOfMMRAI = Assembly.GetExecutingAssembly().GetTypes().Where(m => m.IsSubclassOf(typeof(MMRAI))).ToArray();
            foreach (Type mmrType in typesOfMMRAI) {
                ListOfMMRAI.Add((MMRAI)mmrType.GetConstructor(Type.EmptyTypes).Invoke(null));
            }
        }
        #endregion

        #region Netcode
        public override void HandlePacket(BinaryReader reader, int whoAmI) {
            PacketID messageType = (PacketID)reader.ReadByte();
            switch (messageType) {
                case PacketID.SyncModdedNPCAI:
                    if (Main.netMode == NetmodeID.MultiplayerClient) {
                        int npcIndex = reader.ReadInt32();
                        for (int i = 0; i < NPC.maxAI; i++) {
                            Main.npc[npcIndex].GetGlobalNPC<MMRGlobalNPC>().moddedAI[i] = reader.ReadSingle();
                        }
                        Main.npc[npcIndex].aiStyle = reader.ReadInt32();
                    }
                    break;
                default:
                    Main.NewText($"Unexpected packet type recieved: {messageType}");
                    break;
            }
        }
        #endregion

        #region Detour Methods
        private void NPC_VanillaAI(On.Terraria.NPC.orig_VanillaAI orig, NPC self) {
            MMRGlobalNPC globalNPC = self.GetGlobalNPC<MMRGlobalNPC>();
            if (NPCLoader.PreAI(self) && Main.masterMode) {
                globalNPC.currentMMRAI?.PreVanillaAI(self);
            }
            orig(self);
        }
        #endregion
    }
}