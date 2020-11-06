using MasterModeReloaded.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace MasterModeReloaded.Content.NPCs.BossAI {
    public class Retinazer : MMRAI {

        public float GeneralTimer {
            get => GetMMRGlobalNPC().moddedAI[0];
            set => GetMMRGlobalNPC().moddedAI[0] = value;
        }

        public float BarragePhase {
            get => GetMMRGlobalNPC().moddedAI[1];
            set => GetMMRGlobalNPC().moddedAI[1] = value;
        }

        public int CurrentTarget {
            get => (int)GetMMRGlobalNPC().moddedAI[2];
            set => GetMMRGlobalNPC().moddedAI[2] = value;
        }

        public Retinazer() : base(NPCID.Retinazer) { }

        public const float NormalAIPhase = 0f;
        public const float AlignToTargetPhase = 1f;
        public const float ChargeBarragePhase = 2f;
        public const float FireBarragePhase = 3f;

        public override void PreVanillaAI(NPC npc) {
            //Will enter second phase at 60% health instead of 40%
            if (npc.ai[0] == 0f && npc.life < npc.lifeMax * 0.6f) {
                npc.ai[0] = 1f;
                npc.ai[1] = 0f;
                npc.ai[2] = 0f;
                npc.ai[3] = 0f;
                npc.netUpdate = true;
            }

            //Second Phase only
            if (npc.ai[0] == 3f) {

                //Whether or not Ret is in his "third phase" (just essentially do stuff at a faster rate)
                bool isPseudoThirdPhase = npc.life < npc.lifeMax * 0.25f;

                //Wait time, in seconds, after entering the Alignment phase to trigger ChargeBarrage phase
                float alignToTargetTime = isPseudoThirdPhase ? 1.5f : 5f;

                //How long Ret will sit still and "charge" the barrage for
                float barrageChargeTime = isPseudoThirdPhase ? 1f : 2.5f;

                //How long the Barrage lasts for in seconds
                float barrageLength = isPseudoThirdPhase ? 2f : 5f;

                //Only want Vanilla AI running before the Charge Phase
                if ((BarragePhase == NormalAIPhase || BarragePhase == AlignToTargetPhase) && npc.aiStyle == -1) {
                    npc.aiStyle = npc.GetDefaultAIStyle();
                }

                //Ret will horizontally line up with the encircled target
                if (BarragePhase == AlignToTargetPhase) {
                    if (npc.ai[1] != 1f) {
                        npc.ai[1] = 1f;
                    }
                    npc.localAI[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.target = CurrentTarget;

                    GeneralTimer++;
                    if (GeneralTimer >= 60f * alignToTargetTime) {
                        GeneralTimer = 0f;
                        BarragePhase = ChargeBarragePhase;

                        SoundEngine.PlaySound(SoundID.ForceRoar, npc.Center, -1);
                        npc.aiStyle = -1;

                        npc.netUpdate = true;
                    }
                }
                //Ret will freeze aiming at the last location of the target
                else if (BarragePhase == ChargeBarragePhase) {
                    //Dust as a visual indicator
                    int redDust = Dust.NewDust(new Vector2(npc.position.X + npc.velocity.X, npc.position.Y + npc.velocity.Y), npc.width, npc.height, DustID.RedTorch, npc.velocity.X, npc.velocity.Y, 100, default(Color), 3f * npc.scale);
                    Main.dust[redDust].noGravity = true;

                    npc.velocity *= 0.9f;

                    GeneralTimer++;
                    if (GeneralTimer >= 60f * barrageChargeTime) {
                        GeneralTimer = 0;
                        BarragePhase = FireBarragePhase;

                        npc.netUpdate = true;
                    }
                }
                //Fire Barrage (TODO: Make death laser) of lasers aiming at the last location of the target
                else if (BarragePhase == FireBarragePhase) {
                    GeneralTimer++;
                    if (Main.netMode != NetmodeID.MultiplayerClient && GeneralTimer % 5 == 0) {
                        int laserBarrage = Projectile.NewProjectile(npc.Center, (npc.rotation + MathHelper.ToRadians(90f)).ToRotationVector2() * 15f, ProjectileID.DeathLaser, npc.GetAttackDamage_ForProjectiles(22f, 20f), 0.5f);
                        Main.projectile[laserBarrage].tileCollide = false;
                        Main.projectile[laserBarrage].alpha = 310; //This is done so that the laser looks like it properly comes out of the eye barrel
                        Main.projectile[laserBarrage].timeLeft = 60 * 3;
                        NetMessage.SendData(MessageID.SyncProjectile, number: laserBarrage);
                    }

                    if (GeneralTimer >= 60 * barrageLength) {
                        GeneralTimer = 0;
                        BarragePhase = AlignToTargetPhase;

                        npc.ai[1] = 0f;

                        npc.netUpdate = true;
                    }
                }
            }
        }

        public override void AI(NPC npc) {
            //Twins share health
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
