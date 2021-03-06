﻿using MasterModeReloaded.Common.Players;
using Terraria;
using Terraria.ModLoader;

namespace MasterModeReloaded.Content.Debuffs.Boss {

    public class TrueConfusion : ModBuff {

        public override void SetDefaults() {
            DisplayName.SetDefault("True Confusion");
            Description.SetDefault("Your mind throbs...");
            Main.debuff[Type] = true;
            CanBeCleared = true;
        }

        //Flips the player's screen vertically
        public override void Update(Player player, ref int buffIndex) {
            player.GetModPlayer<MMRPlayer>().trueConfusion = true;
            player.confused = true;
        }
    }
}