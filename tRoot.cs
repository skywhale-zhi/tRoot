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
        public static Mod CalamityMod;
        public override void Load()
        {
            ModLoader.TryGetMod("CalamityMod", out CalamityMod);

            if (Main.netMode != NetmodeID.Server)
            {
            }

            base.Load();
        }

        public override void Unload()
        {
            CalamityMod = null;
            base.Unload();
        }
    }
}