using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace MasterModeReloaded.NPCs.BossAI {
    public class Retinazer : MMRAI {

        public Retinazer(NPC npc) {
            currentNPC = npc;
        }

        public override void PreVanillaAI(NPC npc) {
            //Will enter second phase at 60% health instead of 40%
            if (npc.ai[0] == 0f && npc.life < npc.lifeMax * 0.6f) {
                npc.ai[0] = 1f;
                npc.ai[1] = 0f;
                npc.ai[2] = 0f;
                npc.ai[3] = 0f;
                npc.netUpdate = true;
            }

        }

        public override void AI(NPC npc) {
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                int spazIndex = NPC.FindFirstNPC(NPCID.Spazmatism);
                if (spazIndex != -1) {
                    NPC spazmatism = Main.npc[spazIndex];
                    if (npc.life > spazmatism.life) {
                        npc.life = spazmatism.life;
                        NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
                    }
                }
                else {
                    npc.StrikeNPCNoInteraction(npc.life + 1, 0, 0);
                    NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
                }
            }
        }

        public override void PostAI(NPC npc) {
        }
    }
}
