using MasterModeReloaded.Enums;
using MasterModeReloaded.NPCs.BossAI;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MasterModeReloaded.NPCs {
    public class MMRGlobalNPC : GlobalNPC {

        public float[] moddedAI = new float[NPC.maxAI];

        public MMRAI currentMMRAI;

        public override bool InstancePerEntity => true;

        #region Defaults
        public override void SetDefaults(NPC npc) {
            if (Main.masterMode) {
                if (npc.type == NPCID.Spazmatism || npc.type == NPCID.Retinazer) {
                    //The twins share health. This is the value that will equal their Expert mode combined health
                    //if stat boosts are disabled.
                    npc.lifeMax = 43000;
                }
            }
        }

        public override GlobalNPC NewInstance(NPC npc) {
            MMRGlobalNPC newNPC = (MMRGlobalNPC)base.NewInstance(npc);
            switch (npc.type) {
                case NPCID.KingSlime:
                    newNPC.currentMMRAI = new KingSlime(npc);
                    break;
                case NPCID.EyeofCthulhu:
                    newNPC.currentMMRAI = new EyeOfCthulhu(npc);
                    break;
                case NPCID.Spazmatism:
                    newNPC.currentMMRAI = new Spazmatism(npc);
                    break;
                case NPCID.Retinazer:
                    newNPC.currentMMRAI = new Retinazer(npc);
                    break;
                default:
                    newNPC.currentMMRAI = null;
                    break;
            }
            return newNPC;
        }
        #endregion

        #region AI Related

        public override void AI(NPC npc) {
            if (NPCLoader.PreAI(npc) && Main.masterMode) {
                currentMMRAI?.AI(npc);
            }
            base.AI(npc);
        }

        public override void PostAI(NPC npc) {
            if (Main.masterMode) {
                currentMMRAI?.PostAI(npc);
            }
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
            if (MasterModeReloaded.DebugMode && !npc.friendly) {
                Main.NewText($"{npc.TypeName}: {npc.ai[0]}, {npc.ai[1]}, {npc.ai[2]}, {npc.ai[3]}" +
                    $"\n{moddedAI[0]}, {moddedAI[1]}, {moddedAI[2]}, {moddedAI[3]}");
            }
            base.PostAI(npc);
        }
        #endregion
    }
}
