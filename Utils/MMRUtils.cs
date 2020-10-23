using MasterModeReloaded.NPCs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
