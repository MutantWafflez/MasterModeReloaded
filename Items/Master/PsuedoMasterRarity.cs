﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace MasterModeReloaded.Items.Master {
    public class PsuedoMasterRarity : ModRarity {
        public override string Name => "Master";

        public override Color RarityColor => new Color(255, (byte)(Main.masterColor * 200f), 0, Main.mouseTextColor);
    }
}
