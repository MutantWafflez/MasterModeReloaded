using Terraria;

namespace MasterModeReloaded.NPCs {
    public abstract class MMRAI {

        public NPC currentNPC;

        public abstract void PreVanillaAI(NPC npc);

        public abstract void AI(NPC npc);

        public abstract void PostAI(NPC npc);

        public MMRGlobalNPC GetMMRGlobalNPC() {
            return currentNPC.GetGlobalNPC<MMRGlobalNPC>();
        }
    }
}
