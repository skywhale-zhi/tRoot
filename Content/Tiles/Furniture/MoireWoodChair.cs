using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace tRoot.Content.Tiles.Furniture
{
    public class MoireWoodChair : ModTile
    {
        public const int NextStyleHeight = 40;

		public override void SetStaticDefaults()
		{
			//重要框架
			Main.tileFrameImportant[Type] = true;
			//能否不被其他瓷砖粘贴
			Main.tileNoAttach[Type] = true;
			//能否被岩浆摧毁
			Main.tileLavaDeath[Type] = true;
			//智能光标移到上面显示高亮边框
			TileID.Sets.HasOutlines[Type] = true;
			//能否被npc坐上去，下为玩家坐上去
			TileID.Sets.CanBeSatOnForNPCs[Type] = true; 
			TileID.Sets.CanBeSatOnForPlayers[Type] = true; 
			//取消智能光标
			TileID.Sets.DisableSmartCursor[Type] = true;

			//算作房屋椅子条件
			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);

			//灰尘风格
			//DustType = ModContent.DustType<Sparkle>();

			//拥有椅子的功能
			AdjTiles = new int[] { TileID.Chairs };

			//在地图显示的属性
			ModTranslation name = CreateMapEntryName();
			name.AddTranslation(7, "云纹木椅");
			name.AddTranslation(1, "Moire Wood Chair");
			AddMapEntry(new Color(163, 127, 27), name);

			//放置体积1*2，宽高
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
			TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, 2);
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;


			//我不明白： 用于为精灵表中的动画帧、生长阶段或平铺状态提供空间。
			TileObjectData.newTile.StyleMultiplier = 2;

			//当涉及到一张雪碧图上有很多其他的形态或物品的图且他们水平依次排列时，启用下面的语句，允许贴图水平排列
			TileObjectData.newTile.StyleHorizontal = true;
			//接着上条，限定水平排列多少列，这里的贴图是2列
			//这里不需要： TileObjectData.newTile.StyleWrapLimit = 2;

			//调用另一种摆放方式，向右摆放
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			// 向右选择第二种纹理方式，我猜类似数组吧从0开始
			TileObjectData.addAlternate(1); 


			TileObjectData.addTile(Type);
		}


		//灰尘风格
		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}

		//掉落计算
		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 32, ModContent.ItemType<Items.Placeable.Furniture.MoireWoodChair>());
		}

		//智能交互功能是否可以选择此磁贴
		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
		{
			return settings.player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance); // Avoid being able to trigger it from long range 避免远程触发
		}

		//玩家坐上去的动作
		public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info)
		{
			// It is very important to know that this is called on both players and NPCs, so do not use Main.LocalPlayer for example, use info.restingEntity
			Tile tile = Framing.GetTileSafely(i, j);
			//info.directionOffset = info.restingEntity is Player ? 6 : 2; // Default to 6 for players, 2 for NPCs
			//info.visualOffset = Vector2.Zero; // Defaults to (0,0)
			info.TargetDirection = -1;
			if (tile.TileFrameX != 0)
			{
				info.TargetDirection = 1; // 如果坐在右侧备用座椅上，则面向右侧（通过前面的SetStaticDefaults中的addAlternate添加）
			}

			//锚代表椅子最底部的瓷砖。这用于对齐实体复选框
			//由于i和j可能来自椅子的任何坐标，因此我们需要根据该坐标调整锚定
			info.AnchorTilePosition.X = i; // 我们的椅子只有1宽，所以没有什么特别的要求
			info.AnchorTilePosition.Y = j;
			if (tile.TileFrameY % NextStyleHeight == 0)
			{
				info.AnchorTilePosition.Y++; // Here, since our chair is only 2 tiles high, we can just check if the tile is the top-most one, then move it 1 down
			}
		}

		//让玩家右键能坐在上面
		public override bool RightClick(int i, int j)
		{
			Player player = Main.LocalPlayer;

			if (player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
			{   //避免远程触发
				player.GamepadEnableGrappleCooldown();
				player.sitting.SitDown(player, i, j);
			}
			return true;
		}


		//鼠标覆盖时显示交互图标
		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;

			//在Snappng范围内，可平铺
			if (!player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
			{ //右键单击中的匹配条件。只有在单击它时才会显示交互
				return;
			}

			player.noThrow = 2;
			//光标项目图标已启用，鼠标覆盖在椅子上面显示小椅子图案
			player.cursorItemIconEnabled = true;
			player.cursorItemIconID = ModContent.ItemType<Items.Placeable.Furniture.MoireWoodChair>();

			if (Main.tile[i, j].TileFrameX / 18 < 1)
			{
				player.cursorItemIconReversed = true;
			}
		}
	}
}
