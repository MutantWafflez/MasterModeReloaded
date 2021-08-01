using MasterModeReloaded.Content.Projectiles.Friendly;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace MasterModeReloaded.Content.Items.Master.Weapons {

    public class TwinsGun : ModItem {

        //TODO: Glowmask
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("The Amalgam");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            Tooltip.SetDefault("Fires Death Lasers and Cursed Flames simultaneously" +
                "\n'Always watching {name here}, always watching...'");
        }

        public override void SetDefaults() {
            Item.width = 72;
            Item.height = 28;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ProjectileID.PurpleLaser;
            Item.DamageType = DamageClass.Ranged;
            Item.autoReuse = true;
            Item.shootSpeed = 7f;
            Item.useAnimation = 16;
            Item.useTime = 16;
            Item.master = true;
            Item.value = Item.sellPrice(gold: 3);
            Item.damage = 55;
            Item.knockBack = 1.5f;
            Item.UseSound = SoundID.Item157;
        }

        public override Vector2? HoldoutOffset() {
            return new Vector2(-9, 0);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            var tooltip1 = tooltips.FirstOrDefault(l => l.Name == "Tooltip1" && l.mod == "Terraria");
            if (tooltip1 != null) {
                tooltip1.text = $"'Always watching {Main.LocalPlayer.name}, always watching...'";
            }
        }

        public override bool Shoot(Player player, ProjectileSource_Item_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            //Two friendly cursed flame projectiles skewed 10 degrees from the center of the gun

            Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(-10)), ModContent.ProjectileType<FriendlyEyeFire>(), (int)(damage * 1.25f), knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(10)), ModContent.ProjectileType<FriendlyEyeFire>(), (int)(damage * 1.25f), knockback, player.whoAmI);

            SoundEngine.PlaySound(SoundID.Item34.WithVolume(0.75f), position);

            return true;
        }
    }
}