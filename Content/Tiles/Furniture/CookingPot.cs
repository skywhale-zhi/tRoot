using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;

namespace tRoot.Content.Tiles.Furniture
{
    internal class CookingPot : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileLavaDeath[Type] = true;
            //设置每个图的帧的高度
            AnimationFrameHeight = 56;

            //体积布局3*3
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.StyleHorizontal = false;
            TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 18 };
            //哪种方块禁止放在上面
            TileObjectData.newTile.AnchorInvalidTiles = new int[] { TileID.MagicalIceBlock };
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);

            AdjTiles = new int[] { TileID.Pots };
            ModTranslation name = CreateMapEntryName();
            name.AddTranslation(1, "Cooking Pot");
            name.AddTranslation(7, "烹饪锅");
            AddMapEntry(new Color(112, 109, 108), name);

            TileObjectData.addTile(Type);
        }


        //给予光环
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.78f;
            g = 0.44f;
            b = 0.22f;
        }


        //允许您为图块设置动画。使用 frameCounter 来跟踪当前帧的活动时间，并使用 frame 更改当前帧。这被称为一次更新。使用 AnimateIndividualTile 直接为特定的图块实例设置动画。
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {

            // Spend 9 ticks on each of 6 frames, looping
            frameCounter++;
            if (frameCounter >= 8)
            {
                frameCounter = 0;
                if (++frame >= 4)
                {
                    frame = 0;
                }
            }
            // Above code works, but since we are just mimicking another tile, we can just use the same value
            //frame = Main.tileFrame[TileID.FireflyinaBottle];
        }


        //public override bool KillSound(int i, int j, bool fail)
        //{
        //    // Play the glass shattering sound instead of the normal digging sound if the tile is destroyed on this hit
        //    if (!fail)
        //    {
        //        SoundEngine.PlaySound(SoundID.Shatter, new Vector2(i, j).ToWorldCoordinates());
        //        return false;
        //    }
        //    return base.KillSound(i, j, fail);
        //}


        //掉落计算
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 48, ModContent.ItemType<Items.Placeable.Furniture.CookingPot>());
        }
    }
}
