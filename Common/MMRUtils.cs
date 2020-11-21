using MasterModeReloaded.Content.NPCs;
using System;
using Terraria;

namespace MasterModeReloaded.Common {
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

        /// <summary>
        /// Returns a given NPC's Global MMR NPC for ai and such.
        /// </summary>
        public static MMRGlobalNPC GetMMRGlobalNPC(this NPC npc) {
            if (npc.TryGetGlobalNPC<MMRGlobalNPC>(out MMRGlobalNPC GlobalNPC)) {
                return GlobalNPC;
            }
            throw new NullReferenceException("MMR Global NPC not found.");
        }

    }
}
