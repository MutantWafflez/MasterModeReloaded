using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace MasterModeReloaded.Content.Rarities {
    public class PsuedoMasterRarity : ModRarity {
        public override string Name => "Master";

        public override Color RarityColor => new Color(255, (byte)(Main.masterColor * 200f), 0, Main.mouseTextColor);
    }
}
