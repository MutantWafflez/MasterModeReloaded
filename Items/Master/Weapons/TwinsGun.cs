using MasterModeReloaded.Projectiles.Friendly;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace MasterModeReloaded.Items.Master.Weapons {
    public class TwinsGun : ModItem {

        //TODO: Glowmask
        public override void SetStaticDefaults() {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            Tooltip.SetDefault("'Always watching {name here}, always watching...'");
        }

        public override void SetDefaults() {
            item.width = 72;
            item.height = 28;
            item.useStyle = ItemUseStyleID.Shoot;
            item.shoot = ProjectileID.PurpleLaser;
            item.DamageType = DamageClass.Ranged;
            item.autoReuse = true;
            item.shootSpeed = 7f;
            item.useAnimation = 16;
            item.useTime = 16;
            item.rare = ModContent.RarityType<PsuedoMasterRarity>();
            item.value = Item.sellPrice(gold: 3);
            item.damage = 40;
            item.knockBack = 1.5f;
            item.UseSound = SoundID.Item12;
        }

        public override Vector2? HoldoutOffset() {
            return new Vector2(-9, 0);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            var tooltip0 = tooltips.FirstOrDefault(l => l.Name == "Tooltip0" && l.mod == "Terraria");
            if (tooltip0 != null) {
                tooltip0.text = $"'Always watching {Main.LocalPlayer.name}, always watching...'";
            }
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack) {
            Vector2 shootVelocity = new Vector2(speedX, speedY);

            //Two friendly cursed flame projectiles skewed 10 degrees from the center of the gun
            Projectile.NewProjectile(position, shootVelocity.RotatedBy(MathHelper.ToRadians(-10)), ModContent.ProjectileType<FriendlyEyeFire>(), (int)((float)damage * 1.25f), knockBack);
            Projectile.NewProjectile(position, shootVelocity.RotatedBy(MathHelper.ToRadians(10)), ModContent.ProjectileType<FriendlyEyeFire>(), (int)((float)damage * 1.25f), knockBack);

            return base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
        }
    }
}
