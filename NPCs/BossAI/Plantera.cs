using MasterModeReloaded.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MasterModeReloaded.NPCs.BossAI {
    public class Plantera : MMRAI {

        public float GeneralTimer {
            get => GetMMRGlobalNPC().moddedAI[0];
            set => GetMMRGlobalNPC().moddedAI[0] = value;
        }

        public float CurrentSubPhase {
            get => GetMMRGlobalNPC().moddedAI[1];
            set => GetMMRGlobalNPC().moddedAI[1] = value;
        }

        public const float NormalAIPhase = 0f;
        public const float NettleBurstWarningPhase = 1f;
        public const float NettleBurstPhase = 2f;

        public Plantera() : base(NPCID.Plantera) { }

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
