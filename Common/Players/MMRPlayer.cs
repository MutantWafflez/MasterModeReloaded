using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace MasterModeReloaded.Common.Players {
    public class MMRPlayer : ModPlayer {
        #region Player Fields
        //Buffs/Debuffs
        public bool trueConfusion;
        //Accessories

        //Misc.
        #endregion

        public override void ResetEffects() {
            trueConfusion = false;
        }

        #region Buff Related
        public override void PostUpdateBuffs() {
            if (trueConfusion) {
                Filters.Scene.Activate("VerticalMirror");
            }
            else {
                Filters.Scene.Deactivate("VerticalMirror");
            }
        }
        #endregion
    }
}
