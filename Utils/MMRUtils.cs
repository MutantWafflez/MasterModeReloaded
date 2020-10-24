using Terraria;

namespace MasterModeReloaded.Utils {
    public static class MMRUtils {

        /// <summary>
        /// Returns the default AI Style of the given NPC.
        /// </summary>
        /// <param name="npc">NPC to get the AI Style of.</param>
        public static int GetDefaultAIStyle(this NPC npc) {
            NPC fakeNPC = new NPC();
            fakeNPC.SetDefaults(npc.type);
            return fakeNPC.aiStyle;
        }

    }
}
