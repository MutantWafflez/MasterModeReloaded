using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MasterModeReloaded.Projectiles.Hostile {
    public class SmartCursedFlames : ModProjectile {

        public Vector2 OriginalPosition;
        public Vector2 TargetPosition;

        private bool hasBurst = false;

        public override string Texture => "Terraria/Projectile_" + ProjectileID.CursedFlameHostile;

        public override void SetDefaults() {
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = -1;
            projectile.timeLeft = 60 * 30;
            projectile.hostile = true;
            projectile.light = 0.8f;
            projectile.alpha = 100;
            projectile.DamageType = DamageClass.Magic;
            projectile.penetrate = -1;
            projectile.scale = 1.3f;
            projectile.tileCollide = false;
        }

        public override void AI() {
            if (++projectile.localAI[0] % 4 == 0) {
                int cursedDust = Dust.NewDust(new Vector2(projectile.position.X + projectile.velocity.X, projectile.position.Y + projectile.velocity.Y), projectile.width, projectile.height, 75, projectile.velocity.X, projectile.velocity.Y, 100, default, 3f * projectile.scale);
                Main.dust[cursedDust].noGravity = true;
                projectile.localAI[0] = 0f;
            }

            if (projectile.ai[0] == 1f) {
                if (++projectile.ai[1] <= 120) {
                    projectile.velocity = projectile.DirectionTo(TargetPosition) * 12f;
                }
                else if (projectile.ai[1] > 120 && !hasBurst) {
                    projectile.velocity = projectile.DirectionTo(OriginalPosition) * 12f;
                    hasBurst = true;
                    projectile.netUpdate = true;
                }
            }

            projectile.rotation += MathHelper.ToRadians(2.5f);
        }

        public override void SendExtraAI(BinaryWriter writer) {
            writer.Write(OriginalPosition.X);
            writer.Write(OriginalPosition.Y);
            writer.Write(TargetPosition.X);
            writer.Write(TargetPosition.Y);
            writer.Write(hasBurst);
        }

        public override void ReceiveExtraAI(BinaryReader reader) {
            OriginalPosition = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            TargetPosition = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            hasBurst = reader.ReadBoolean();
        }
    }
}
