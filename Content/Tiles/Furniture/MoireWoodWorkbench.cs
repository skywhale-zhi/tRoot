using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace tRoot.Content.Tiles.Furniture
{
    public class MoireWoodWorkbench : ModTile
    {
        public override void SetStaticDefaults()
        {
            // 是否设定为实心的 默认为假
            //Main.tileSolid[Type] = true;

            // 是否允许水瓶等小家具放在上面，默认为假
            Main.tileTable[Type] = true;
            // 是否能踩上去，平台效果
            Main.tileSolidTop[Type] = true;
            // 禁止被其他瓷砖贴上
            Main.tileNoAttach[Type] = true;
            // 是否触碰岩浆销毁
            Main.tileLavaDeath[Type] = true;
            // 这个只能改成true，否咋会出现一系列bug.!!!!!!平铺框架重要!!!
            Main.tileFrameImportant[Type] = true;
            // 禁用智能光标，没看出来效果
            TileID.Sets.DisableSmartCursor[Type] = true;
            // 禁止npc自己跳上去
            TileID.Sets.IgnoredByNpcStepUp[Type] = true;

            //灰尘风格
            //DustType = ModContent.DustType<Dusts.Sparkle>();

            //让他具有某个工作站的功能
            AdjTiles = new int[] { TileID.CrystalBall,TileID.WorkBenches };

            //设计体积为2*1,宽和高
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
            //精灵图的高度，每个方格为16像素，但是最下面一个方格得带有缓冲的像素块2个，所以为18
            TileObjectData.newTile.CoordinateHeights = new[] { 18 };
            //确定精灵图对应谁的id
            TileObjectData.addTile(Type);

            //把这个东西算作正常房屋需要的桌子来计数
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);

            //这些应该是先查看地图时显示的名称，颜色等
            ModTranslation name = CreateMapEntryName();
            name.AddTranslation(7, "云纹木工作台");
            name.AddTranslation(1, "Moire Wood Work Bench");
            AddMapEntry(new Color(163, 127, 27), name);


        }

        //灰尘特效，我猜
        //public override void NumDust(int x, int y, bool fail, ref int num)
        //{
        //    num = fail ? 1 : 3;
        //}

        //挖掘后掉落一个物品本身，不能直接用物块掉落的语句，因为那是单个方块对应掉一个，工作台由两个构成
        public override void KillMultiTile(int x, int y, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(x, y), x * 16, y * 16, 32, 16, ModContent.ItemType<Items.Placeable.Furniture.MoireWoodWorkbench>());
        }
    }
}
