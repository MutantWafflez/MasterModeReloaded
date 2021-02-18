using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace MasterModeReloaded.Content.Projectiles.Hostile {

    public class Shockwave : ModProjectile {

        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Shockwave");
        }

        public override void SetDefaults() {
            Projectile.hostile = true;
            Projectile.width = 288;
            Projectile.height = 48;
            Projectile.ignoreWater = true;
            Projectile.knockBack = 3f;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.maxPenetrate = 999;
            Projectile.penetrate = 999;
            Projectile.velocity = new Vector2(0, 0);
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
        }
    }
}