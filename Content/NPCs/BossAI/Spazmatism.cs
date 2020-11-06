using MasterModeReloaded.Common;
using MasterModeReloaded.Content.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MasterModeReloaded.Content.NPCs.BossAI {
    public class Spazmatism : MMRAI {

        public float GeneralTimer {
            get => GetMMRGlobalNPC().moddedAI[0];
            set => GetMMRGlobalNPC().moddedAI[0] = value;
        }

        public float CirclePhase {
            get => GetMMRGlobalNPC().moddedAI[1];
            set => GetMMRGlobalNPC().moddedAI[1] = value;
        }

        public float CenterOfCircleX {
            get => GetMMRGlobalNPC().moddedAI[2];
            set => GetMMRGlobalNPC().moddedAI[2] = value;
        }

        public float CenterOfCircleY {
            get => GetMMRGlobalNPC().moddedAI[3];
            set => GetMMRGlobalNPC().moddedAI[3] = value;
        }

        public const float NormalAIPhase = 0f;
        public const float EncirclementWarningPhase = 1f;
        public const float EncirclementPreparationPhase = 2f;
        public const float EncirclementPhase = 3f;
        public const float CircleAndFirePhase = 4f;

        public Spazmatism() : base(NPCID.Spazmatism) { }

        private void RestartTwins(NPC npc, bool deleteProjectiles = true) {
            GeneralTimer = -360f;
            CirclePhase = NormalAIPhase;
            CenterOfCircleX = 0f;
            CenterOfCircleY = 0f;

            SoundEngine.PlaySound(SoundID.ForceRoar, npc.Center, -1);
            npc.ai[1] = 0f;
            npc.ai[2] = 0f;
            npc.ai[3] = 0f;
            if (Main.netMode != NetmodeID.MultiplayerClient && deleteProjectiles) {
                for (int i = 0; i < Main.maxProjectiles; i++) {
                    if (Main.projectile[i].type == ModContent.ProjectileType<SmartCursedFlames>()) {
                        Main.projectile[i].Kill();
                        NetMessage.SendData(MessageID.KillProjectile, number: i);
                    }
                }
            }

            if (Main.netMode != NetmodeID.MultiplayerClient) {
                int retIndex = NPC.FindFirstNPC(NPCID.Retinazer);
                if (retIndex != -1) {
                    Retinazer retAI = (Retinazer)Main.npc[retIndex].GetGlobalNPC<MMRGlobalNPC>().currentMMRAI;
                    retAI.GeneralTimer = 0;
                    retAI.BarragePhase = Retinazer.NormalAIPhase;
                    Main.npc[retIndex].ai[1] = 0f;
                    NetMessage.SendData(MessageID.SyncNPC, number: retIndex);
                    Main.npc[retIndex].netUpdate = true;
                }
            }

            npc.netUpdate = true;
        }

        public override void PreVanillaAI(NPC npc) {

            Player target = Main.player[npc.target];

            //Will enter second phase at 60% health instead of 40%
            if (npc.ai[0] == 0f && npc.life < npc.lifeMax * 0.6f) {
                npc.ai[0] = 1f;
                npc.ai[1] = 0f;
                npc.ai[2] = 0f;
                npc.ai[3] = 0f;
                npc.netUpdate = true;
            }

            //Will fire three projectiles with a 15 degree skew in MMR instead of one inaccurate projectile
            if (npc.ai[0] == 0 && npc.ai[1] == 0f && npc.ai[3] >= 59f) {
                npc.ai[3] = 0f;
                for (float projRotation = -15; projRotation <= 15; projRotation += 15) {
                    Vector2 projSpeed = (npc.DirectionTo(Main.player[npc.target].Center) * 10f).RotatedBy(MathHelper.ToRadians(projRotation));
                    Projectile.NewProjectile(npc.Center, projSpeed, ProjectileID.CursedFlameHostile, npc.GetAttackDamage_ForProjectiles(25f, 22f), 1f);
                }
            }

            //Second Phase Only
            if (npc.ai[0] == 3f) {

                //Whether or not spaz is in his "third phase" (just essentially do stuff at a faster rate such as encircle)
                bool isPseudoThirdPhase = npc.life < npc.lifeMax * 0.25f;

                //Rotation value in radians to have the sprite face upwards
                float upwardRotationValue = MathHelper.ToRadians(180);
                //Rotation value in degrees used to determine how big the circle is in the EncirclementPhase
                float defaultEncirclementRotation = 3f;

                //Speed at which Spazmatism will encircle the target in the EncirclementPhase
                float encirclementRotationSpeed = -20f;
                //Speed at which Spazmatism will rotate around the flame circle and fire at the target in the CircleAndFirePhase
                float encirclementFireRotationSpeed = isPseudoThirdPhase ? -6f : -3f;

                //Radius of the Cursed Flame circle.
                float radiusOfCircle = 16f * 25f;

                //Amount of Full Rotations during the CircleAndFire Phase
                int fullRotationCount = isPseudoThirdPhase ? 2 : 1;

                //Don't want Vanilla AI running past the Warning phase
                if ((CirclePhase == NormalAIPhase || CirclePhase == EncirclementWarningPhase) && npc.aiStyle == -1) {
                    npc.aiStyle = npc.GetDefaultAIStyle();
                }

                //Cancel Encirclement phase if target player is dead
                if (CirclePhase >= EncirclementPhase && Main.player[target.whoAmI].dead) {
                    RestartTwins(npc);
                }

                //If the target player leaves the Circle, trigger Cursed Flame Implosion and restart Spazmatism
                if (CirclePhase == CircleAndFirePhase && Main.netMode != NetmodeID.MultiplayerClient && Main.player[npc.target].Distance(new Vector2(CenterOfCircleX, CenterOfCircleY)) > radiusOfCircle) {
                    for (int i = 0; i < Main.maxProjectiles; i++) {
                        if (Main.projectile[i].type == ModContent.ProjectileType<SmartCursedFlames>()) {
                            Main.projectile[i].ai[0] = 1f;
                            NetMessage.SendData(MessageID.SyncProjectile, number: i);
                        }
                    }
                    RestartTwins(npc, false);
                }

                //Normal Vanilla AI runs here
                if (CirclePhase == NormalAIPhase) {
                    GeneralTimer++;
                    if (isPseudoThirdPhase) {
                        GeneralTimer += 0.25f;
                    }
                    if (GeneralTimer >= 60f * 15f) {
                        GeneralTimer = 0;
                        CirclePhase = EncirclementWarningPhase;
                        SoundEngine.PlaySound(SoundID.ForceRoar, npc.Center, -1);
                        npc.netUpdate = true;
                    }
                }
                //Gives visual and audio cue for the encirclement about to occur while Spazmatism disappears
                else if (CirclePhase == EncirclementWarningPhase) {
                    //Dust as a visual indicator
                    int cursedDust = Dust.NewDust(new Vector2(npc.position.X + npc.velocity.X, npc.position.Y + npc.velocity.Y), npc.width, npc.height, DustID.CursedTorch, npc.velocity.X, npc.velocity.Y, 100, default(Color), 3f * npc.scale);
                    Main.dust[cursedDust].noGravity = true;

                    npc.alpha += 3;
                    if (npc.alpha >= 255) {
                        npc.alpha = 255;
                    }

                    GeneralTimer += 3f;
                    if (GeneralTimer >= 255) {
                        npc.TargetClosest();

                        GeneralTimer = 0f;
                        CenterOfCircleX = target.Center.X;
                        CenterOfCircleY = target.Center.Y;
                        CirclePhase = EncirclementPreparationPhase;

                        SoundEngine.PlaySound(SoundID.ForceRoar, npc.Center, -1);
                        npc.aiStyle = -1;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;

                        //Trigger Ret's Barrage AI
                        if (Main.netMode != NetmodeID.MultiplayerClient) {
                            int retIndex = NPC.FindFirstNPC(NPCID.Retinazer);
                            if (retIndex != -1) {
                                Retinazer retAI = (Retinazer)Main.npc[retIndex].GetGlobalNPC<MMRGlobalNPC>().currentMMRAI;
                                retAI.BarragePhase = Retinazer.AlignToTargetPhase;
                                retAI.CurrentTarget = npc.target;
                                NetMessage.SendData(MessageID.SyncNPC, number: retIndex);
                                Main.npc[retIndex].netUpdate = true;
                            }
                        }

                        npc.netUpdate = true;
                    }
                }
                //Spazmatism teleports 25 blocks away from the cetner of the Circle and re-appears
                else if (CirclePhase == EncirclementPreparationPhase) {
                    npc.velocity *= 0f;
                    if (GeneralTimer == 0f) {
                        npc.Teleport(new Vector2(CenterOfCircleX - (radiusOfCircle) - (npc.width / 2), CenterOfCircleY - (npc.height / 2)), -1);
                        npc.rotation = upwardRotationValue;
                    }

                    npc.alpha -= 6;
                    if (npc.alpha <= 0) {
                        npc.alpha = 0;
                    }

                    GeneralTimer += 6;
                    if (GeneralTimer >= 255) {
                        GeneralTimer = 0f;
                        CirclePhase = EncirclementPhase;

                        npc.netUpdate = true;
                    }

                }
                //Spazmatism rapidly begins encircling the target, creating a ring of cursed flames
                else if (CirclePhase == EncirclementPhase) {
                    float rotationInRadians = MathHelper.ToRadians(defaultEncirclementRotation);

                    npc.velocity = new Vector2(0, encirclementRotationSpeed).RotatedBy(GeneralTimer);
                    npc.rotation = upwardRotationValue + GeneralTimer;
                    GeneralTimer += rotationInRadians;

                    if (Main.netMode != NetmodeID.MultiplayerClient && ++npc.ai[2] % 3 == 0f) {
                        int cursedFlames = Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<SmartCursedFlames>(), npc.GetAttackDamage_ForProjectiles(30f, 27f), 0);
                        (Main.projectile[cursedFlames].modProjectile as SmartCursedFlames).TargetPosition = new Vector2(CenterOfCircleX, CenterOfCircleY);
                        (Main.projectile[cursedFlames].modProjectile as SmartCursedFlames).OriginalPosition = npc.Center;
                    }

                    if (GeneralTimer >= MathHelper.TwoPi) {
                        GeneralTimer = 0f;
                        CirclePhase = CircleAndFirePhase;

                        npc.velocity *= 0f;
                        npc.ai[2] = 0f;

                        npc.netUpdate = true;
                    }
                }
                //Spazmatism more slowly circles around the center of the circle spewing eye fire towards the center
                else if (CirclePhase == CircleAndFirePhase) {
                    float rotationInRadians = MathHelper.ToRadians(defaultEncirclementRotation * (encirclementFireRotationSpeed / encirclementRotationSpeed));

                    npc.velocity = new Vector2(0, encirclementFireRotationSpeed).RotatedBy(GeneralTimer);
                    npc.rotation = npc.DirectionTo(new Vector2(CenterOfCircleX, CenterOfCircleY)).ToRotation() + MathHelper.ToRadians(-90f);
                    GeneralTimer += rotationInRadians;

                    if (Main.netMode != NetmodeID.MultiplayerClient && ++npc.ai[2] % 6 == 0f) {
                        int eyeFire = Projectile.NewProjectile(npc.Center, npc.DirectionTo(new Vector2(CenterOfCircleX, CenterOfCircleY)) * 11f, ProjectileID.EyeFire, npc.GetAttackDamage_ForProjectiles(30f, 27f), 0);
                        Main.projectile[eyeFire].tileCollide = false;
                    }

                    //Play EyeFire sound
                    if (Main.netMode != NetmodeID.Server && npc.ai[2] % 22 == 0f) {
                        SoundEngine.PlaySound(SoundID.Item34, npc.position);
                    }

                    if (GeneralTimer >= MathHelper.TwoPi * fullRotationCount) {
                        RestartTwins(npc);
                    }
                }

            }
        }

        public override void AI(NPC npc) {
            //Twins share health
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                int retIndex = NPC.FindFirstNPC(NPCID.Retinazer);
                if (retIndex != -1) {
                    NPC retinazer = Main.npc[retIndex];
                    if (npc.life > retinazer.life) {
                        npc.life = retinazer.life;
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
