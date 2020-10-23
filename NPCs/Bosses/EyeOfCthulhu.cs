using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using MasterModeReloaded.Utils;

namespace MasterModeReloaded.NPCs.Bosses {
    public class EyeOfCthulhu : MMRGlobalNPC {

        public bool IsPhaseThree {
            get
            {
                if (moddedAI[0] == 1f) { return true; }
                else { return false; }
            }
            set { moddedAI[0] = value.ToInt(); }
        }

        public override void PreVanillaAI(NPC npc) {
            if (npc.type == NPCID.EyeofCthulhu && Main.masterMode) {

                void TeleDust() {
                    Dust dust;
                    Vector2 position = npc.position;
                    dust = Main.dust[Dust.NewDust(position, npc.width, npc.height, 55, 0f, 0f, 0, new Color(255, 255, 255), 1.5f)];
                    dust.noGravity = true;
                }

                if (!IsPhaseThree) {
                    //Enter Third Phase
                    if (npc.ai[0] == 3f && npc.life <= npc.lifeMax / 5) {
                        npc.aiStyle = -1;
                        npc.ai[0] = 4f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                        npc.netUpdate = true;
                        return;
                    }
                    //Spin and Disappear
                    if (npc.ai[0] == 4f && npc.ai[1] <= 180f) {
                        if (npc.ai[1] == 30f && Main.netMode != NetmodeID.MultiplayerClient) {
                            SoundEngine.PlaySound(SoundID.Roar, npc.Center, 0);
                        }
                        npc.alpha += 2;
                        if (npc.alpha >= 255) {
                            npc.alpha = 255;
                        }
                        npc.velocity *= 0.97f;
                        npc.dontTakeDamage = true;
                        npc.ai[1]++;
                        npc.rotation += MathHelper.ToRadians(npc.ai[1] / 3.5f);
                        return;
                    }
                    //Transition into attack phase after spinning and disappearing
                    else if (npc.ai[0] == 4f && npc.ai[1] > 180) {
                        npc.ai[0] = 3f; //This is reverted to 3 so that the visual charge effect stays (since it is hard-coded in the source)
                        IsPhaseThree = true;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                        npc.dontTakeDamage = false;
                        npc.netUpdate = true;
                        return;
                    }
                }
                //Attack Phase 3 (Teleport to top left or top right of player, charge, then teleport away, repeat)
                if (IsPhaseThree && npc.ai[2] <= 75f) {
                    //Initial teleport, depending on npc.ai[3]
                    npc.TargetClosest(false);
                    if (npc.ai[2] == 0f) {
                        if (npc.ai[3] == 0f) {
                            //Top-left
                            npc.Center = new Vector2(Main.player[npc.target].Center.X - (16 * 12), Main.player[npc.target].Center.Y - (16 * 12));
                            npc.ai[3] = 1f;
                        }
                        else {
                            //Top-right
                            npc.Center = new Vector2(Main.player[npc.target].Center.X + (16 * 12), Main.player[npc.target].Center.Y - (16 * 12));
                            npc.ai[3] = 0f;
                        }
                        npc.rotation = npc.DirectionTo(Main.player[npc.target].Center).ToRotation() - MathHelper.ToRadians(90);
                        npc.alpha = 255;
                        npc.hide = false;
                    }
                    if (npc.ai[2] < 23f) {
                        npc.rotation = npc.DirectionTo(Main.player[npc.target].Center).ToRotation() - MathHelper.ToRadians(90);
                        npc.alpha -= 8;
                        TeleDust();
                    }
                    npc.ai[2]++;
                    //The actual Charge towards the target
                    if (npc.ai[2] == 23f) {
                        npc.ai[1] = 4f;
                        npc.ai[0] = 3f; //This is changed to 3f to allow for the Vanilla charging effect
                        //^ However, the charging effect changes the alpha, so it must be toggled for the disappearing and reappearing effect
                        npc.velocity = npc.DirectionTo(Main.player[npc.target].Center) * 15f;
                        npc.rotation = npc.velocity.ToRotation() - MathHelper.ToRadians(90);
                        if (Main.netMode != NetmodeID.MultiplayerClient) {
                            SoundEngine.PlaySound(SoundID.ForceRoar, npc.Center, -1);
                        }
                    }
                    //Slowdown
                    if (npc.ai[2] > 45f) {
                        npc.velocity *= 0.95f;
                        npc.ai[0] = 4f; //Revert alpha back into our control
                        npc.alpha += 8;
                        TeleDust();
                        if (npc.alpha > 255) {
                            npc.alpha = 255;
                            npc.hide = true;
                        }
                        npc.rotation = npc.DirectionTo(Main.player[npc.target].Center).ToRotation() - MathHelper.ToRadians(90);
                    }
                }
                else if (IsPhaseThree && npc.ai[2] > 75f) {
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;
                }
                //Prevent Vanilla AI from running, if we don't, the Vanilla AI will attempt to keep charging
                //However, we still need the EOC to float away when the target(s) are dead, Vanilla can handle that
                if (IsPhaseThree && !Main.player[npc.target].dead && !Main.dayTime) {
                    npc.aiStyle = -1;
                    npc.TargetClosest(false);
                    npc.netUpdate = true;
                }
                else {
                    npc.aiStyle = npc.GetAIStyle();
                }
            }
        }
    }
}
