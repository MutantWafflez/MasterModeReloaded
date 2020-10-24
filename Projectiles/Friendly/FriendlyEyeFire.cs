using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MasterModeReloaded.Projectiles.Friendly {
    public class FriendlyEyeFire : ModProjectile {

        public override string Texture => "Terraria/Projectile_" + ProjectileID.EyeFire;

        public override void SetDefaults() {
            projectile.CloneDefaults(ProjectileID.EyeFire);
            projectile.width = 8;
            projectile.height = 8;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.DamageType = DamageClass.Ranged;
            aiType = ProjectileID.EyeFire;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) {
            if (Main.rand.Next(3) == 0) {
                target.AddBuff(BuffID.CursedInferno, 60 * 5);
            }
        }
    }
}
