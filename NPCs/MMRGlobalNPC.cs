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

        public override GlobalNPC NewInstance(NPC npc) {
            MMRGlobalNPC newNPC = (MMRGlobalNPC)base.NewInstance(npc);
            switch (npc.type) {
                case NPCID.KingSlime:
                    newNPC.currentMMRAI = new KingSlime(npc);
                    break;
                case NPCID.EyeofCthulhu:
                    newNPC.currentMMRAI = new EyeOfCthulhu(npc);
                    break;
                default:
                    newNPC.currentMMRAI = null;
                    break;
            }
            return newNPC;
        }

        #region AI Related

        public override void AI(NPC npc) {
            if (Main.masterMode) {
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
            base.PostAI(npc);
        }
        #endregion

        #region Damage Overrides
        public override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit) {

        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit) {

        }
        #endregion
    }
}
