using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MasterModeReloaded.Content.Items.Dev {

    [AutoloadEquip(EquipType.Head)]
    public class MutantDevHat : ModItem {

        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Mutant's Cap");
            Tooltip.SetDefault("'Are you ok?'"
                + "\n" + Language.GetTextValue("CommonItemTooltip.DevItem"));
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults() {
            Item.vanity = true;
            Item.value = Item.sellPrice(gold: 1);
            Item.wornArmor = true;
            Item.width = 20;
            Item.height = 10;
            Item.rare = ItemRarityID.Cyan;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<MutantDevChestplate>() && legs.type == ModContent.ItemType<MutantDevPants>();

        public override void DrawHair(ref bool drawHair, ref bool drawAltHair) {
            drawHair = true;
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class MutantDevChestplate : ModItem {

        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Mutant's Vest & Gloves");
            Tooltip.SetDefault("'Buster wolf!'"
                + "\n" + Language.GetTextValue("CommonItemTooltip.DevItem"));
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults() {
            Item.vanity = true;
            Item.value = Item.sellPrice(gold: 1);
            Item.wornArmor = true;
            Item.width = 30;
            Item.height = 22;
            Item.rare = ItemRarityID.Cyan;
        }

        public override void DrawHands(ref bool drawHands, ref bool drawArms) {
            drawHands = drawArms = true;
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class MutantDevPants : ModItem {

        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Mutant's Jeans");
            Tooltip.SetDefault("'Hey, come on, come on!'"
                + "\n" + Language.GetTextValue("CommonItemTooltip.DevItem"));
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults() {
            Item.vanity = true;
            Item.value = Item.sellPrice(gold: 1);
            Item.wornArmor = true;
            Item.width = 22;
            Item.height = 18;
            Item.rare = ItemRarityID.Cyan;
        }
    }
}