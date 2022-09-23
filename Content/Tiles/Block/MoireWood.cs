using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace tRoot.Content.Tiles.Block
{
    public class MoireWood : ModTile
    {
        public override void SetStaticDefaults()
        {
            //是否为实体
            Main.tileSolid[Type] = true;
            //是否自动粘合土壤
            Main.tileMergeDirt[Type] = true;
            //是否阻塞光
            Main.tileBlockLight[Type] = true;

            //灰尘风格
            //DustType = ModContent.DustType<MetalSpikesDust>();
            //挖掉这个贴图块掉落哪种物块
            ItemDrop = ModContent.ItemType<Items.Placeable.Block.MoireWood>();
            //设置在地图显示的颜色
            AddMapEntry(new Color(147, 103, 23));
        }

        //public override void NumDust(int i, int j, bool fail, ref int num)
        //{
        //    //num = fail ? 1 : 3;
        //    num = 2;
        //    fail = false;
        //}

        // todo: implement
        // public override void ChangeWaterfallStyle(ref int style) {
        // 	style = mod.GetWaterfallStyleSlot("ExampleWaterfallStyle");
        // }

    }
}
