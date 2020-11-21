using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace MasterModeReloaded.Content.NPCs.BossAI {
    public class EmpressOfLight : MMRAI {

        public MethodInfo DrawNPCDirectMethod = typeof(Main).GetMethod(nameof(Main.DrawNPCDirect), BindingFlags.Public | BindingFlags.Instance);

        public override int NpcType => NPCID.HallowBoss;

        public override bool DebugMode => true;

        public EmpressOfLight(NPC npc) : base(npc) { }

        //NOTE: Anytime these comments refer to "Phase 1", they mean EOL's Vanilla Phase 2, since she spawns in her Vanilla Phase 2 in MMR
        //Any references to Phase 2 is MMR's new Phase 2 (effectively Vanilla Phase 3)
        public override void PreVanillaAI(NPC npc) {

            //Continue wing animation in our custom Phases (about 25% faster, though)
            if (npc.aiStyle == -1) {
                if ((npc.localAI[0] += 1.25f) >= 44f) {
                    npc.localAI[0] = 0f;
                }
            }
            //Prevents any Vanilla AI from running in our custom Phases
            else if (npc.aiStyle != -1 && npc.ai[0] != 10 && npc.ai[3] >= 4f) {
                npc.aiStyle = -1;
            }

            if (npc.ai[3] < 4f) {
                //Instantly enter Vanilla Phase 2 upon spawn
                if (npc.ai[3] == 0f || npc.ai[3] == 2f) {
                    npc.ai[3] = npc.ai[3] == 0f ? 1f : 3f;
                    npc.netUpdate = true;
                }

                //Empress will cycle her attacks faster depending on her health in Phase 1
                if (npc.ai[0] == 1f && (npc.ai[3] == 1f || npc.ai[3] == 3f)) {
                    npc.ai[1] += 1 - npc.GetLifePercent();
                }

                //Transition to Phase 2
                if (npc.ai[0] == 1f && (npc.ai[3] == 1f || npc.ai[3] == 3f) && npc.GetLifePercent() <= 0.5f) {
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
                if (npc.ai[0] == 10f && npc.ai[1] >= 180f) {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;
                }
                //Prepare for triangulate attack (create two clones then form a triangle between the three)
                if (npc.ai[0] == 0f && npc.ai[1] <= 90f) {
                    npc.ai[1]++;
                    if ((npc.alpha += 5) > 255) {
                        npc.alpha = 255;
                    }
                    npc.velocity *= 0.99f;
                }
                //Transition to triangulate attack
                else if (npc.ai[0] == 0f && npc.ai[1] > 90f) {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.velocity *= 0f;
                    npc.TargetClosest();
                    npc.Teleport(Main.player[npc.target].position - new Vector2(0, 16 * 20), -1);
                    SoundEngine.PlaySound(SoundID.Item160, npc.Center);
                    npc.netUpdate = true;
                }
                //Reppearance phase for triangulation attack
                else if (npc.ai[0] == 1f && npc.ai[1] <= 90f) {
                    npc.ai[1]++;
                    if ((npc.alpha -= 5) < 0) {
                        npc.alpha = 0;
                    }

                }
            }
            //Last Stand (Phase 3)
            else if (npc.ai[3] == 5f) {

            }
        }

        public override void AI(NPC npc) {
            if (npc.ai[3] == 4f && (npc.ai[0] == 1f || npc.ai[0] == 2f)) {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
                NPC leftNPC = new NPC();
                leftNPC.CloneDefaults(npc.type);
                leftNPC.ai = (float[])npc.ai.Clone();
                leftNPC.position += new Vector2(-16 * 20, -16 * 40);
                leftNPC.alpha = npc.alpha;
                NPC rightNPC = new NPC();
                rightNPC.CloneDefaults(npc.type);
                rightNPC.ai = (float[])npc.ai.Clone();
                rightNPC.position += new Vector2(16 * 20, -16 * 40);
                rightNPC.alpha = npc.alpha;
                DrawNPCDirectMethod.Invoke(Main.instance, new object[] { Main.spriteBatch, leftNPC, false, Main.screenPosition });
                DrawNPCDirectMethod.Invoke(Main.instance, new object[] { Main.spriteBatch, rightNPC, false, Main.screenPosition });
                Main.spriteBatch.End();
            }
        }

        public override void PostAI(NPC npc) {
        }
    }
}
