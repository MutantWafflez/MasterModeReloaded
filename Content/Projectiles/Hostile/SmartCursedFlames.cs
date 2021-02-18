using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MasterModeReloaded.Content.Projectiles.Hostile {

    public class SmartCursedFlames : ModProjectile {
        public Vector2 OriginalPosition;
        public Vector2 TargetPosition;

        private bool hasBurst = false;

        public override void SetDefaults() {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 60 * 30;
            Projectile.hostile = true;
            Projectile.light = 0.8f;
            Projectile.alpha = 100;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.scale = 1.3f;
            Projectile.tileCollide = false;
        }

        public override void AI() {
            if (++Projectile.localAI[0] % 4 == 0) {
                int cursedDust = Dust.NewDust(new Vector2(Projectile.position.X + Projectile.velocity.X, Projectile.position.Y + Projectile.velocity.Y), Projectile.width, Projectile.height, 75, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 3f * Projectile.scale);
                Main.dust[cursedDust].noGravity = true;
                Projectile.localAI[0] = 0f;
            }

            if (Projectile.ai[0] == 1f) {
                if (++Projectile.ai[1] <= 120) {
                    Projectile.velocity = Projectile.DirectionTo(TargetPosition) * 12f;
                }
                else if (Projectile.ai[1] > 120 && !hasBurst) {
                    Projectile.velocity = Projectile.DirectionTo(OriginalPosition) * 12f;
                    hasBurst = true;
                    Projectile.netUpdate = true;
                }
            }

            Projectile.rotation += MathHelper.ToRadians(2.5f);
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