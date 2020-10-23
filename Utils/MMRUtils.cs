using Terraria;

namespace MasterModeReloaded.Utils {
    public static class MMRUtils {
        public static int GetAIStyle(this NPC npc) {
            NPC fakeNPC = new NPC();
            fakeNPC.SetDefaults(npc.type);
            return fakeNPC.aiStyle;
        }

    }
}
