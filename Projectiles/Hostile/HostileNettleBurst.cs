using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MasterModeReloaded.Projectiles.Hostile {
    public class HostileNettleBurst : ModProjectile {

        /// <summary>
        /// What visual will be used for projectile projectile. 0 == Right, 1 == Left, 2 == End
        /// </summary>
        public int typeOfBurst = 0;

        public Vector2 psuedoVelocity;

        public override string Texture => "Terraria/Projectile_" + ProjectileID.NettleBurstRight;

        #region Defaults
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Nettle Vine");
        }

        public override void SetDefaults() {
            projectile.CloneDefaults(ProjectileID.NettleBurstRight);
            projectile.timeLeft = 60 * 4;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.aiStyle = -1;
        }
        #endregion

        #region AI Related
        public override void AI() {
            int appearanceRate = 135;
            int maxLength = 20;

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
            if  (projectile.ai[0] >= 255 && projectile.ai[1] < maxLength && psuedoVelocity != Vector2.Zero) {
                HostileNettleBurst nextSegment = (HostileNettleBurst)Projectile.NewProjectileDirect(projectile.Center + psuedoVelocity, Vector2.Zero, ModContent.ProjectileType<HostileNettleBurst>(), 30, 1f).modProjectile;
                nextSegment.projectile.ai[1] = projectile.ai[1] += 1;
                nextSegment.psuedoVelocity = psuedoVelocity;
                if (typeOfBurst == 0) {
                    nextSegment.typeOfBurst = 1;
                }
                else if (typeOfBurst == 1) {
                    nextSegment.typeOfBurst = 0;
                }
                if (nextSegment.projectile.ai[1] >= maxLength) {
                    nextSegment.typeOfBurst = 2;
                }
                nextSegment.projectile.netUpdate = true;
                psuedoVelocity = Vector2.Zero;

                projectile.netUpdate = true;

                if (Main.netMode == NetmodeID.Server) {
                    NetMessage.SendData(MessageID.SyncProjectile, number: nextSegment.projectile.whoAmI);
                }
            }

            if (projectile.timeLeft <= 128f) {
                projectile.alpha += 2;
            }
		}

        public override void SendExtraAI(BinaryWriter writer) {
            writer.Write(typeOfBurst);
            writer.Write(psuedoVelocity.X);
            writer.Write(psuedoVelocity.Y);
        }

        public override void ReceiveExtraAI(BinaryReader reader) {
            typeOfBurst = reader.ReadInt32();
            psuedoVelocity = new Vector2(reader.ReadSingle(), reader.ReadSingle());
        }
        #endregion

        #region Drawing
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) {
            Texture2D texture = ModContent.GetTexture("Terraria/Projectile_" + (ProjectileID.NettleBurstRight + typeOfBurst)).Value;
            spriteBatch.Draw(texture,
                new Rectangle((int)(projectile.position.X - Main.screenPosition.X), (int)(projectile.position.Y - Main.screenPosition.Y), (int)(texture.Width * projectile.scale), (int)(texture.Height * projectile.scale)),
                null,
                lightColor,
                projectile.rotation,
                Vector2.Zero,
                SpriteEffects.None,
                0f
                );
            return false;
		}

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
