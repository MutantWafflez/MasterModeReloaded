using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace MasterModeReloaded.NPCs.Bosses {

    public class KingSlime : MMRGlobalNPC {

        private float SlamTimer {
            get => moddedAI[0];
            set => moddedAI[0] = value;
        }

        private bool IsSlamming {
            get
            {
                if (moddedAI[1] == 1f) { return true; }
                else { return false; }
            }
            set => moddedAI[1] = value.ToInt();
        }

        public override void PreVanillaAI(NPC npc) {
            if (npc.type == NPCID.KingSlime && Main.masterMode) {

                void DoTeleport() {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 5f;
                    SlamTimer = 0f;
                    npc.netUpdate = true;
                }

                if (npc.life < npc.lifeMax / 2 && !Main.player[npc.target].dead) {
                    //Set the nearest player to be the "target"
                    Player target = Main.player[Player.FindClosest(npc.position, npc.width, npc.height)];
                    Vector2 Center = target.Center;
                    npc.localAI[1] = Center.X;
                    npc.localAI[2] = Center.Y - (16 * 25); //Teleportation destination is 25 blocks above the target
                    //So the timer doesn't increase when King Slime is in the process of teleporting
                    if (npc.ai[1] != 5f && npc.ai[1] != 6f) {
                        SlamTimer++;
                    }
                    //Teleport cooldown based on health
                    if (SlamTimer >= 480f) {
                        DoTeleport();
                    }
                    else if (npc.life < npc.lifeMax / 3 && SlamTimer >= 420f) {
                        DoTeleport();
                    }
                    else if (npc.life < npc.lifeMax / 4 && SlamTimer >= 300f) {
                        DoTeleport();
                    }
                    //Takes place right after King Slime re-appears
                    if (npc.ai[1] == 6f) {
                        npc.velocity.Y = 0f;
                        npc.velocity.X = 0f;
                        IsSlamming = true;
                        //To prevent cheese of hiding under something
                        npc.noTileCollide = true;

                    }
                    //So King Slime doesn't unintentionally horizontally move
                    if (IsSlamming) {
                        npc.velocity.X = 0f;
                        //As soon as gravity is applied (in the Vanilla AI), this triggers the velocity increase
                        if (npc.velocity.Y > 0 && npc.velocity.Y != 20f) {
                            npc.velocity.Y = 20f;
                            //Gravity is turned off so that terminal velocity doesn't occur
                            npc.noGravity = true;
                        }
                    }
                    //So King Slime doesn't fall through the map
                    if (npc.noTileCollide && npc.Bottom.Y >= target.Bottom.Y) {
                        npc.noTileCollide = false;
                    }
                    //Hitting the ground code (Shockwave, stuck in the ground, etc.)
                    if (IsSlamming && npc.collideY) {
                        IsSlamming = false;
                        if (Main.netMode != NetmodeID.MultiplayerClient) {
                            //Projectile.NewProjectile(new Vector2(npc.Center.X, npc.Bottom.Y), new Vector2(0, 0), ModContent.ProjectileType<Shockwave>(), 20, 3f);
                            SoundEngine.PlaySound(SoundID.Item62, npc.Center);
                        }
                        //So the dust actually shows, just using the statement once barely shows any dust
                        for (int i = 0; i < 40; i++) {
                            Dust dust;
                            Vector2 position = new Vector2(npc.Center.X - 144, npc.Bottom.Y);
                            dust = Main.dust[Dust.NewDust(position, 288, 48, 0, 0f, -8f, 0, new Color(255, 255, 255), 2f)];
                            dust.noGravity = true;
                            dust.noLight = true;
                        }
                        npc.ai[0] = -300f;
                        npc.ai[1] = 3f;
                        //So it looks like King Slime is stuck in the ground (give it that OOMPH)
                        npc.position.Y += 16f;
                        npc.behindTiles = true;
                        //Gravity is reverted back so King Slime doesn't fly around
                        npc.noGravity = false;
                        npc.netUpdate = true;
                    }
                    //To revert King Slime to be shown in front of tiles once he hops out of the ground
                    if (!Collision.SolidCollision(npc.position, npc.width, npc.height) && !IsSlamming) {
                        npc.behindTiles = false;
                    }
                    //Since the slamming process makes it so King Slime can go through blocks, the vanilla teleportation process is not needed
                    if (npc.ai[2] != 1) {
                        npc.ai[2] = 0f;
                    }
                }
            }
        }
    }
}