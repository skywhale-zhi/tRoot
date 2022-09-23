using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot
{
    public partial class tRoot : Mod
    {
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
            }
            base.Load();
        }
    }
}