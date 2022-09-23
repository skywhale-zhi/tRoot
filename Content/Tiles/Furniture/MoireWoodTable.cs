using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace tRoot.Content.Tiles.Furniture
{
    public class MoireWoodTable : ModTile
    {
        public override void SetStaticDefaults()
        {
            //能否在上面放水瓶，小家具等
            Main.tileTable[Type] = true;
            //能否站在上面
            Main.tileSolidTop[Type] = true;
            //禁止被其他瓷砖贴上
            Main.tileNoAttach[Type] = true;
            //能否被岩浆摧毁
            Main.tileLavaDeath[Type] = true;
            //重要框架
            Main.tileFrameImportant[Type] = true;
            //禁止智能光标
            TileID.Sets.DisableSmartCursor[Type] = true;
            //npc不会跳上去
            TileID.Sets.IgnoredByNpcStepUp[Type] = true;

            //灰尘样式
            //DustType = ModContent.DustType<Dusts.Sparkle>();
            //添加额外桌子属性
            AdjTiles = new int[] { TileID.Tables };

            //设置贴图宽高风格
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);

            //当涉及到一张雪碧图上有很多其他的形态或物品的图且他们水平依次排列时，启用下面的语句，允许贴图水平排列
            //这里不需要：TileObjectData.newTile.StyleHorizontal = true;

            //贴图高像素设置
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };

            //贴图仅需的安放方块，三个参数分别是 依靠方块的类型，第二个是宽度，第三个变量是需要锚的图块的开始，从0开始，类似数组
            //这里不需要：TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width-2, 1);
            
            
            //选定type
            TileObjectData.addTile(Type);


            //加上房屋属性要求
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);


            //地图上的具体显示
            ModTranslation name = CreateMapEntryName();
            name.AddTranslation(7, "云纹木桌");
            name.AddTranslation(1, "Moire Wood Table");
            AddMapEntry(new Color(163, 127, 27), name);
        }

        //灰尘
        public override void NumDust(int x, int y, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        //挖掘掉落一个
        public override void KillMultiTile(int x, int y, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(x, y), x * 16, y * 16, 48, 32, ModContent.ItemType<Items.Placeable.Furniture.MoireWoodTable>());
        }

    }
}
