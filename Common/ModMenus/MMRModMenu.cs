using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ID;
using Terraria.ModLoader;

namespace MasterModeReloaded.Common.ModMenus {

    public class MMRModMenu : ModMenu {
        public const string PublicDisplayName = "Master Mode Reloaded";

        private const string MenuTexturePath = nameof(MasterModeReloaded) + "/Assets/Textures";

        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>($"{MenuTexturePath}/Misc/MMRLogo");

        public override int Music => MusicID.Plantera;

        public override string DisplayName => PublicDisplayName;
    }
}