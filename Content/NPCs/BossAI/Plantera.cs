using MasterModeReloaded.Common;
using MasterModeReloaded.Content.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MasterModeReloaded.Content.NPCs.BossAI {
    public class Plantera : MMRAI {

        public float GeneralTimer {
            get => GetMMRGlobalNPC().moddedAI[0];
            set => GetMMRGlobalNPC().moddedAI[0] = value;
        }

        public float CurrentSubPhase {
            get => GetMMRGlobalNPC().moddedAI[1];
            set => GetMMRGlobalNPC().moddedAI[1] = value;
        }

        public float ChargeCount {
            get => GetMMRGlobalNPC().moddedAI[2];
            set => GetMMRGlobalNPC().moddedAI[2] = value;
        }

        public const float NormalAIPhase = 0f;
        public const float NettleBurstWarningPhase = 1f;
        public const float NettleBurstPhase = 2f;

        public const float NormalAIPhaseTwo = 3f;
        public const float ChargePhase = 4f;

        public override int NpcType => NPCID.Plantera;

        public Plantera(NPC npc) : base(npc) { }

        public override void PreVanillaAI(NPC npc) {
            //Phase 1 only
            if (npc.life > npc.lifeMax / 2) {
                if (CurrentSubPhase == NormalAIPhase) {
                    GeneralTimer++;
                    if (GeneralTimer >= 60 * 15f * npc.GetLifePercent()) {
                        GeneralTimer = 0f;
                        CurrentSubPhase = NettleBurstWarningPhase;
                        SoundEngine.PlaySound(SoundID.ForceRoar, npc.Center, -1);

                        npc.netUpdate = true;
                    }
                }
                else if (CurrentSubPhase == NettleBurstWarningPhase) {
                    int nettleDust = Dust.NewDust(npc.position, npc.width, npc.height, 7, Scale: 1.5f);
                    Main.dust[nettleDust].noGravity = true;

                    GeneralTimer++;
                    if (GeneralTimer >= 60 * 3f) {
                        GeneralTimer = 0f;
                        CurrentSubPhase = NettleBurstPhase;

                        npc.netUpdate = true;
                    }
                }
                else if (CurrentSubPhase == NettleBurstPhase) {
                    if (GeneralTimer == 0f) {
                        if (Main.netMode != NetmodeID.MultiplayerClient) {
                            for (int direction = 0; direction < 4; direction++) {
                                HostileNettleBurst nettleBurst = (HostileNettleBurst)Projectile.NewProjectileDirect(npc.Center, Vector2.Zero, ModContent.ProjectileType<HostileNettleBurst>(), 30, 1f).modProjectile;
                                nettleBurst.psuedoVelocity = new Vector2(0, -nettleBurst.projectile.height).RotatedBy(MathHelper.ToRadians(90) * direction);
                                NetMessage.SendData(MessageID.SyncProjectile, number: nettleBurst.projectile.whoAmI);
                            }

                            SoundEngine.PlaySound(SoundID.ForceRoar, npc.Center, -1);
                        }
                    }
                    GeneralTimer++;
                    if (GeneralTimer >= HostileNettleBurst.MaxTimeLeft) {
                        GeneralTimer = 0f;
                        CurrentSubPhase = NormalAIPhase;

                        npc.netUpdate = true;
                    }
                }
            }
            //Phase 2 only
            else {

                //Total amount of sequential charges Plantera will do in the Charge Phase
                int totalCharges = 2;
                if (npc.GetLifePercent() <= 0.25f) {
                    totalCharges++;
                }
                if (npc.GetLifePercent() <= 0.125f) {
                    totalCharges++;
                }

                //Upon switching to Phase 2, make sure all the AI values are set properly
                if (CurrentSubPhase < NormalAIPhaseTwo) {
                    GeneralTimer = 0f;
                    CurrentSubPhase = NormalAIPhaseTwo;
                    ChargeCount = 0f;

                    npc.netUpdate = true;
                }

                //Normal Phase 2 Plantera AI
                if (CurrentSubPhase == NormalAIPhaseTwo) {
                    GeneralTimer++;

                    if (GeneralTimer >= 60f * MathHelper.Clamp(7.5f * (npc.GetLifePercent() + 0.5f), 3.5f, 7.5f)) {
                        GeneralTimer = 0f;
                        CurrentSubPhase = ChargePhase;
                        npc.aiStyle = -1;

                        npc.netUpdate = true;
                    }
                }
                //Plantera will charge up to 4 times (depending on remaining life) towards the player
                else if (CurrentSubPhase == ChargePhase) {

                    //Time (in seconds) of how long each charge lasts
                    float chargeLength = MathHelper.Clamp(npc.GetLifePercent() + 0.5f, 0.45f, 0.8f);

                    if (ChargeCount < totalCharges || GeneralTimer >= 60 * chargeLength * 0.5f) {
                        if (GeneralTimer <= 0f) {
                            ChargeCount++;

                            npc.TargetClosest();
                            npc.velocity = npc.DirectionTo(Main.player[npc.target].Center) * 18f;
                            RotateTowardsPlayerCorrectly(npc, -90);
                            GeneralTimer = 60 * chargeLength;

                            SoundEngine.PlaySound(SoundID.ForceRoar, npc.Center, -1);
                        }
                        else if (GeneralTimer > 0f) {
                            GeneralTimer--;
                            npc.velocity *= 0.98f;
                        }
                    }
                    else {
                        GeneralTimer = 0f;
                        CurrentSubPhase = NormalAIPhaseTwo;
                        ChargeCount = 0f;
                        npc.aiStyle = npc.GetDefaultAIStyle();

                        npc.netUpdate = true;
                    }
                }
            }
        }

        public override void AI(NPC npc) {
            //Don't want plantera moving during the burst phase, only rotating/shooting
            if (npc.life > npc.lifeMax / 2 && CurrentSubPhase == NettleBurstPhase) {
                npc.velocity *= 0f;
            }
        }

        public override void PostAI(NPC npc) {
        }


    }
}
