using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MasterModeReloaded {
    public class MMRModMenu : ModMenu {

        private const string MENU_TEXTURE_PATH = nameof(MasterModeReloaded) + "/Textures";

        public override Asset<Texture2D> Logo => ModContent.GetTexture($"{MENU_TEXTURE_PATH}/Misc/MMRLogo");

        public override int Music => MusicID.Plantera;

        public override Asset<Texture2D> MoonTexture => ModContent.GetTexture("Terraria/Moon_Pumpkin");

        public override string DisplayName => "Master Mode Reloaded";

        public override void Load() {
            //Since the Update Method is called in the middle of the DrawMenu method and removes the ability to change the eclipse boolean,
            //gotta method "swap" it to change the boolean at the end of the method.
            On.Terraria.Main.DrawMenu += Main_DrawMenu;
        }

        private void Main_DrawMenu(On.Terraria.Main.orig_DrawMenu orig, Main self, GameTime gameTime) {
            orig(self, gameTime);
            if (IsSelected) {
                Main.eclipse = true;
            }
        }
    }
}