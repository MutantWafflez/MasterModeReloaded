using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MasterModeReloaded.Content.Projectiles.Hostile {
    public class HostileNettleBurst : ModProjectile {

        public Vector2 psuedoVelocity;

        /// <summary>
        /// Maximum length of the vine
        /// </summary>
        private const int MaxBurstLength = 15;

        /// <summary>
        /// How long the projectile will last for.
        /// </summary>
        public const int MaxTimeLeft = 60 * 4;

        #region Defaults
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Nettle Vine");
            ProjectileID.Sets.DontAttachHideToAlpha[projectile.type] = true;
        }

        public override void SetDefaults() {
            projectile.CloneDefaults(ProjectileID.NettleBurstRight);
            projectile.hide = true;
            projectile.timeLeft = MaxTimeLeft;
            projectile.height = 64;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.aiStyle = -1;
        }
        #endregion

        #region AI Related
        public override void AI() {
            int appearanceRate = 100;

            if (Main.rand.NextFloat(1) < 0.25f && Main.netMode != NetmodeID.Server) {
                int nettleDust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 7, Scale: 0.5f);
                Main.dust[nettleDust].noGravity = true;
            }

            if (projectile.ai[0] <= 255) {
                projectile.rotation = psuedoVelocity.ToRotation() - MathHelper.ToRadians(90);
                projectile.alpha -= appearanceRate;
                if (projectile.alpha <= 0) {
                    projectile.alpha = 0;
                }
            }

            projectile.ai[0] += appearanceRate;
            if (projectile.ai[0] >= 255 && projectile.ai[1] < MaxBurstLength && psuedoVelocity != Vector2.Zero) {
                HostileNettleBurst nextSegment = (HostileNettleBurst)Projectile.NewProjectileDirect(projectile.Center + psuedoVelocity, Vector2.Zero, ModContent.ProjectileType<HostileNettleBurst>(), 30, 1f).modProjectile;
                nextSegment.projectile.ai[1] = projectile.ai[1] += 1;
                nextSegment.psuedoVelocity = psuedoVelocity;
                nextSegment.projectile.netUpdate = true;
                psuedoVelocity = Vector2.Zero;

                projectile.netUpdate = true;

                if (Main.netMode == NetmodeID.Server) {
                    NetMessage.SendData(MessageID.SyncProjectile, number: nextSegment.projectile.whoAmI);
                }
            }

            if (projectile.timeLeft <= 128f) {
                projectile.damage = 0;
                projectile.alpha += 2;
            }
        }

        public override void SendExtraAI(BinaryWriter writer) {
            writer.Write(psuedoVelocity.X);
            writer.Write(psuedoVelocity.Y);
        }

        public override void ReceiveExtraAI(BinaryReader reader) {
            psuedoVelocity = new Vector2(reader.ReadSingle(), reader.ReadSingle());
        }

        #endregion

        #region Drawing
        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI) {
            drawCacheProjsBehindNPCs.Add(index);
        }
        #endregion

        #region Misc Methods
        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit) {
            //Ignores 10% of defense
            damage += Math.Abs(target.statDefense / 10);
        }
        #endregion

    }
}
