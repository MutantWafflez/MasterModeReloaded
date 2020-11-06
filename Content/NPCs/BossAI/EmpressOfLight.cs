using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace MasterModeReloaded.Content.NPCs.BossAI {
    public class EmpressOfLight : MMRAI {

        public override int NpcType => NPCID.HallowBoss;

        public EmpressOfLight(NPC npc) : base(npc) { }

        //NOTE: Anytime these comments refer to "Phase 1", they mean EOL's Vanilla Phase 2, since she spawns in her Vanilla Phase 2 in MMR.
        //Any references to Phase 2 is MMR's new Phase 2 (effectively Vanilla Phase 3)
        public override void PreVanillaAI(NPC npc) {
            if (npc.ai[3] < 4f) {
                //Instantly enter Phase 2 upon spawn
                if (npc.ai[3] == 2f) {
                    npc.ai[3] = 3f;
                    npc.netUpdate = true;
                }

                //Empress will cycle her attacks faster depending on her health in Phase 1
                if (npc.ai[0] == 1f && npc.ai[3] == 3f) {
                    npc.ai[1] += 1 - npc.GetLifePercent();
                }

                //Transition to Phase 2
                if (npc.ai[0] == 1f && npc.ai[3] == 3f && npc.GetLifePercent() <= 0.5f) {
                    npc.ai[0] = 10f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 4f;
                    npc.netUpdate = true;
                }
            }
            //Phase 2
            else if (npc.ai[3] == 4f) {
                //Exit out of transition phase
                if (npc.ai[0] == 10f && npc.ai[1] >= 190) {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.aiStyle = -1;
                    npc.netUpdate = true;
                }
            }
            //Last Stand (Phase 3)
            else if (npc.ai[3] == 5f) {

            }
        }

        public override void AI(NPC npc) {
        }

        public override void PostAI(NPC npc) {

        }

    }
}
