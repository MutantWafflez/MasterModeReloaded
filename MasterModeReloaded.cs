using MasterModeReloaded.Common;
using MasterModeReloaded.Common.ID;
using MasterModeReloaded.Content.NPCs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MasterModeReloaded {
    public class MasterModeReloaded : Mod {

        public const bool DebugMode = true;

        public static List<Type> ListOfMMRAI;

        #region Loading
        public override void Load() {

            #region Shaders
            if (Main.netMode != NetmodeID.Server) {
                /*Ref<Effect> verticalRef = new Ref<Effect>((Effect)GetEffect("Effects/Filters/VerticalMirror"));
                Filters.Scene["VerticalMirror"] = new Filter(new ScreenShaderData(verticalRef, "VerticallyMirror"), EffectPriority.Medium);
                Filters.Scene["VerticalMirror"].Load();*/
            }
            #endregion

            Patches.ApplyDetourPatches();
            Patches.ApplyILPatches();
        }

        public override void Unload() {
            #region Static Resets
            ListOfMMRAI = null;
            #endregion
        }

        public override void PostSetupContent() {
            ListOfMMRAI = Assembly.GetExecutingAssembly().GetTypes().Where(m => m.IsSubclassOf(typeof(MMRAI))).ToList();
        }
        #endregion

        #region Netcode
        public override void HandlePacket(BinaryReader reader, int whoAmI) {
            PacketID messageType = (PacketID)reader.ReadByte();
            switch (messageType) {
                case PacketID.SyncModdedNPCAI:
                    if (Main.netMode == NetmodeID.MultiplayerClient) {
                        int npcIndex = reader.ReadInt32();
                        for (int i = 0; i < NPC.maxAI; i++) {
                            Main.npc[npcIndex].GetGlobalNPC<MMRGlobalNPC>().moddedAI[i] = reader.ReadSingle();
                        }
                        Main.npc[npcIndex].aiStyle = reader.ReadInt32();
                    }
                    break;
                default:
                    Main.NewText($"Unexpected packet type recieved: {messageType}");
                    break;
            }
        }
        #endregion
    }
}