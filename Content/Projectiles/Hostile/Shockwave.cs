using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace MasterModeReloaded.Content.Projectiles.Hostile {
    public class Shockwave : ModProjectile {

        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Shockwave");
        }

        public override void SetDefaults() {
            projectile.hostile = true;
            projectile.width = 288;
            projectile.height = 48;
            projectile.ignoreWater = true;
            projectile.knockBack = 3f;
            projectile.tileCollide = false;
            projectile.timeLeft = 60;
            projectile.maxPenetrate = 999;
            projectile.penetrate = 999;
            projectile.velocity = new Vector2(0, 0);
            projectile.aiStyle = -1;
            projectile.alpha = 255;
        }
    }

}
