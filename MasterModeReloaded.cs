using MasterModeReloaded.Enums;
using MasterModeReloaded.NPCs;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MasterModeReloaded {
    public class MasterModeReloaded : Mod {
        public override void Load() {
            #region Detours
            On.Terraria.NPC.VanillaAI += NPC_VanillaAI;
            #endregion
        }

        #region Netcode
        public override void HandlePacket(BinaryReader reader, int whoAmI) {
            PacketType messageType = (PacketType)reader.ReadByte();
            switch (messageType) {
                case PacketType.SyncModdedAI:
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
            /*MMRGlobalNPC globalNPC = self.GetGlobalNPC<MMRGlobalNPC>().SpecificGlobalNPCInstance;
            if (globalNPC.PreAI(self)) {
                globalNPC.PreVanillaAI(self);
            }*/
            orig(self);
        }
        #endregion
    }
}