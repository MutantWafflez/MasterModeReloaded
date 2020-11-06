using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MasterModeReloaded.Content.Commands {
    public class SummonPlantera : ModCommand {
        public override CommandType Type
            => CommandType.World;

        public override string Command
            => "sumplant";

        public override string Usage
            => "/sumplant";

        public override string Description
            => "Summons Plantera";

        public override void Action(CommandCaller caller, string input, string[] args) {
            NPC.SpawnOnPlayer(caller.Player.whoAmI, NPCID.Plantera);
        }
    }
}