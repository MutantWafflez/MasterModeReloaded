using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MasterModeReloaded.Content.Projectiles.Friendly {

    public class FriendlyEyeFire : ModProjectile {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.EyeFire;

        public override void SetDefaults() {
            Projectile.CloneDefaults(ProjectileID.EyeFire);
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            AIType = ProjectileID.EyeFire;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) {
            if (Main.rand.Next(3) == 0) {
                target.AddBuff(BuffID.CursedInferno, 60 * 5);
            }
        }
    }
}