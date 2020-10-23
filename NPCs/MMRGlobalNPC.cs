using System;
using System.Collections.Generic;
using System.Linq;
using MasterModeReloaded.Enums;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace MasterModeReloaded.NPCs {
    public abstract class MMRGlobalNPC : GlobalNPC {

        public float[] moddedAI = new float[NPC.maxAI];

        public override bool InstancePerEntity => true;

        public int formerAIStyle = -1;

        #region AI Related
        /// <summary>
        /// Custom AI method that calls before any given NPC's Vanilla AI, but after tML's PreAI hook.
        /// Cannot prevent any AI from running. Will not run if tML PreAI returns false.
        /// The call pattern is tML PreAI -> SpecialPreAI -> VanillaAI -> tML AI -> tml PostAI
        /// </summary>
        /// <param name="npc">NPC to have this method called for.</param>
        public virtual void PreVanillaAI(NPC npc) { }

        public override bool PreAI(NPC npc) {
            if (base.PreAI(npc)) {
                PreVanillaAI(npc);
            }
            return base.PreAI(npc);
        }

        public override void PostAI(NPC npc) {
            if ((npc.netUpdate || npc.netAlways) && Main.netMode == NetmodeID.Server) {
                var packet = Mod.GetPacket();
                packet.Write((byte)PacketType.SyncModdedAI);
                packet.Write(npc.whoAmI);
                for (int i = 0; i < NPC.maxAI; i++) {
                    packet.Write(moddedAI[i]);
                }
                packet.Write(npc.aiStyle);
                packet.Send();
            }
            base.PostAI(npc);
        }
        #endregion

        #region Miscellaneous Methods

        #endregion
    }
}
