﻿using Microsoft.Xna.Framework;
using Terraria;

namespace MasterModeReloaded.Content.NPCs {

    /// <summary>
    /// Class that handles all AI changes for MMR.
    /// </summary>
    public abstract class MMRAI {

        /// <summary>
        /// The instanced NPC this AI is attached to.
        /// </summary>
        public NPC currentNPC;

        /// <summary>
        /// GlobalNPC instance attached to this instance.
        /// </summary>
        public MMRGlobalNPC globalNPC => currentNPC.GetGlobalNPC<MMRGlobalNPC>();

        /// <summary>
        /// What NPC type will have the designated AI changes. For example, if this value is
        /// NPCID.EyeOfCthulhu, any instance of an Eye of Cthulhu will have the designated AI bools
        /// run. If -1, does not run on any NPC.
        /// </summary>
        public virtual int NpcType => -1;

        /// <summary>
        /// Whether or not this NPC will have their AI data printed to the chat for debug purposes.
        /// </summary>
        public virtual bool DebugMode => false;

        public MMRAI(NPC npc) {
            currentNPC = npc;
        }

        /// <summary>
        /// Rotates the given NPC towards their target player. If the NPCs sprite is facing any
        /// other direction than 90 degrees to the right by default, rotation skew is the value (in
        /// degrees) needed to make the sprite face right. For example, the Eye of Cthulhu sprite
        /// faces upwards by default, so to make it face right, you must rotate it 90 degrees.
        /// </summary>
        /// <param name="npc"> NPC to have their direction changed. </param>
        /// <param name="rotationSkew">
        /// Degree npc is rotated by by default, in case sprite faces direction other than to the right.
        /// </param>
        public static void RotateTowardsPlayerCorrectly(NPC npc, float rotationSkew = 0) {
            npc.rotation = npc.DirectionTo(Main.player[npc.target].Center).ToRotation() - MathHelper.ToRadians(rotationSkew);
        }

        /// <summary>
        /// AI Hook that takes place just after PreAI Hook but before Vanilla AI. Exists purely so
        /// there isn't crossmod shenanigans with the PreAI returning false in other mods. Will not
        /// run if PreAI returns false.
        /// </summary>
        public abstract void PreVanillaAI(NPC npc);

        /// <summary>
        /// Same AI hook as the GlobalNPC class. Runs after Vanilla AI, but won't run if PreAI
        /// returns false.
        /// </summary>
        public abstract void AI(NPC npc);

        /// <summary>
        /// Same PostAI hook as the GlobalNPC class. Runs after ALL the other AI hooks, regardless
        /// of what PreAI returns.
        /// </summary>
        public abstract void PostAI(NPC npc);
    }
}