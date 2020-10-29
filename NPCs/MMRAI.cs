using Microsoft.Xna.Framework;
using Terraria;

namespace MasterModeReloaded.NPCs {
    public abstract class MMRAI {

        /// <summary>
        /// The instanced NPC this AI is attached to.
        /// </summary>
        public NPC currentNPC;

        /// <summary>
        /// What NPC type will have the designated AI changes. For example, if this value is NPCID.EyeOfCthulhu, the EOC will have this
        /// AI.
        /// </summary>
        public readonly int npcAIType;

        public MMRAI(int npcType) {
            npcAIType = npcType;
        }

        public abstract void PreVanillaAI(NPC npc);

        public abstract void AI(NPC npc);

        public abstract void PostAI(NPC npc);

        public MMRGlobalNPC GetMMRGlobalNPC() {
            return currentNPC.GetGlobalNPC<MMRGlobalNPC>();
        }

        /// <summary>
        /// Rotates the given NPC towards their target player.
        /// If the NPCs sprite is facing any other direction than 90 degrees to the right by default,
        /// rotation skew is the value (in degrees) needed to make the sprite face right.
        /// For example, the Eye of Cthulhu sprite faces upwards by default, so to make it face right,
        /// you must rotate it 90 degrees. 
        /// </summary>
        /// <param name="npc">NPC to have their direction changed.</param>
        /// <param name="rotationSkew">Degree npc is rotated by by default, in case sprite faces direction other than to the right.</param>
        public static void RotateTowardsPlayerCorrectly(NPC npc, float rotationSkew = 0) {
            npc.rotation = npc.DirectionTo(Main.player[npc.target].Center).ToRotation() - MathHelper.ToRadians(rotationSkew);
        }
    }
}
