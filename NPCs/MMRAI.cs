using Terraria;
using System.Reflection;
using System;
using System.Linq;

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
