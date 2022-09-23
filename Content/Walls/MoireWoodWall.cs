using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace tRoot.Content.Walls
{
    public class MoireWoodWall : ModWall
    {
		public override void SetStaticDefaults()
		{
			//是否算作房子墙
			Main.wallHouse[Type] = true;

			//灰尘
			//DustType = ModContent.DustType<Sparkle>();

			//敲掉掉落墙物品1个
			ItemDrop = ModContent.ItemType<Items.Placeable.Walls.MoireWoodWall>();

			//地图显示颜色
			AddMapEntry(new Color(147, 103, 23));
		}

		//灰尘
		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
	}
}
