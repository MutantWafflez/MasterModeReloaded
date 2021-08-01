using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MasterModeReloaded.Content.Projectiles.Hostile {

    public class HostileNettleBurst : ModProjectile {

        /// <summary>
        /// How long the projectile will last for.
        /// </summary>
        public const int MaxTimeLeft = 60 * 4;

        public Vector2 psuedoVelocity;

        /// <summary>
        /// Maximum length of the vine
        /// </summary>
        private const int MaxBurstLength = 15;

        #region Defaults

        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Nettle Vine");
            ProjectileID.Sets.DontAttachHideToAlpha[Projectile.type] = true;
        }

        public override void SetDefaults() {
            Projectile.CloneDefaults(ProjectileID.NettleBurstRight);
            Projectile.hide = true;
            Projectile.timeLeft = MaxTimeLeft;
            Projectile.height = 64;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
        }

        #endregion

        #region AI Related

        public override void AI() {
            int appearanceRate = 100;

            if (Main.rand.NextFloat(1) < 0.25f && Main.netMode != NetmodeID.Server) {
                int nettleDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 7, Scale: 0.5f);
                Main.dust[nettleDust].noGravity = true;
            }

            if (Projectile.ai[0] <= 255) {
                Projectile.rotation = psuedoVelocity.ToRotation() - MathHelper.ToRadians(90);
                Projectile.alpha -= appearanceRate;
                if (Projectile.alpha <= 0) {
                    Projectile.alpha = 0;
                }
            }

            Projectile.ai[0] += appearanceRate;
            if (Projectile.ai[0] >= 255 && Projectile.ai[1] < MaxBurstLength && psuedoVelocity != Vector2.Zero) {
                HostileNettleBurst nextSegment = (HostileNettleBurst)Projectile.NewProjectileDirect(Projectile.GetProjectileSource_FromThis(), Projectile.Center + psuedoVelocity, Vector2.Zero, ModContent.ProjectileType<HostileNettleBurst>(), 30, 1f).ModProjectile;
                nextSegment.Projectile.ai[1] = Projectile.ai[1] += 1;
                nextSegment.psuedoVelocity = psuedoVelocity;
                nextSegment.Projectile.netUpdate = true;
                psuedoVelocity = Vector2.Zero;

                Projectile.netUpdate = true;

                if (Main.netMode == NetmodeID.Server) {
                    NetMessage.SendData(MessageID.SyncProjectile, number: nextSegment.Projectile.whoAmI);
                }
            }

            if (Projectile.timeLeft <= 128f) {
                Projectile.damage = 0;
                Projectile.alpha += 2;
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

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {
            behindNPCs.Add(index);
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