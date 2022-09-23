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

namespace tRoot.Content.Tiles.Furniture
{
	public class MoireWoodChest : ModTile
	{

		public override void SetStaticDefaults()
		{
			// Properties
			//洞穴探险者药水作用
			Main.tileSpelunker[Type] = true;
			//容器效果
			Main.tileContainer[Type] = true;
			//闪耀？
			Main.tileShine2[Type] = true;
			//闪耀，类似圣物的那种
			Main.tileShine[Type] = 1200;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			//找矿优先级
			Main.tileOreFinderPriority[Type] = 500;
			TileID.Sets.HasOutlines[Type] = true;
			//???
			TileID.Sets.BasicChest[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;

			//DustType = ModContent.DustType<Sparkle>();
			AdjTiles = new int[] { TileID.Containers };
			ChestDrop = ModContent.ItemType<Items.Placeable.Furniture.MoireWoodChest>();

			// Names
			ContainerName.AddTranslation(7, "云纹木箱");
			ContainerName.AddTranslation(1, "Moire Wood Chest");


			ModTranslation name = CreateMapEntryName();
			name.AddTranslation(1, "Moire Wood Chest");
			name.AddTranslation(7, "云纹木箱");
			AddMapEntry(new Color(163, 127, 27), name);

			name = CreateMapEntryName(Name + "_Locked"); // With multiple map entries, you need unique translation keys.
			name.AddTranslation(1, "Locked Moire Wood Chest");
			name.AddTranslation(7, "云纹木锁箱");
			AddMapEntry(new Color(163, 127, 27), name);

			//体积布局2x2
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
			//???
			TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(Chest.FindEmptyChest, -1, 0, true);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(Chest.AfterPlacement_Hook, -1, 0, false);
			//哪种方块禁止放在上面
			TileObjectData.newTile.AnchorInvalidTiles = new int[] { TileID.MagicalIceBlock };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(Type);
		}

        public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].TileFrameX / 36);

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        //如果此 Tile 对应于已锁定的箱子，则返回 true。防止快速堆叠物品进入箱子
        public override bool IsLockedChest(int i, int j) => Main.tile[i, j].TileFrameX / 36 == 1;

        //允许设置解锁的方式
        public override bool UnlockChest(int i, int j, ref short frameXAdjustment, ref int dustType, ref bool manual)
		{
			if (Main.dayTime)
			{
				Main.NewText("箱子在阳光下固执地拒绝打开，晚上再试一次。", Color.Orange);
				return false;
			}

			DustType = dustType;
			return true;
		}


		//public static string MapChestName(string name, int i, int j)
		//{
		//    int left = i;
		//    int top = j;
		//    Tile tile = Main.tile[i, j];
		//    if (tile.TileFrameX % 36 != 0)
		//    {
		//        left--;
		//    }

		//    if (tile.TileFrameY != 0)
		//    {
		//        top--;
		//    }
		//    int chest = Chest.FindChest(left, top);
		//    if (chest < 0)
		//    {
		//        return Language.GetTextValue("LegacyChestType.0");
		//    }

		//    if (Main.chest[chest].name == "")
		//    {
		//        return name;
		//    }

		//    return name + ": " + Main.chest[chest].name;
		//}

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ChestDrop);
            Chest.DestroyChest(i, j);
        }


        public override bool RightClick(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			Main.mouseRightRelease = false;
			int left = i;
			int top = j;
			if (tile.TileFrameX % 36 != 0)
			{
				left--;
			}

			if (tile.TileFrameY != 0)
			{
				top--;
			}

			//箱子关闭动作
			if (player.sign >= 0)
			{
				SoundEngine.PlaySound(SoundID.MenuClose);
				player.sign = -1;
				Main.editSign = false;
				Main.npcChatText = "";
			}

			//箱子打开动作
			if (Main.editChest)
			{
				SoundEngine.PlaySound(SoundID.MenuTick);
				Main.editChest = false;
				Main.npcChatText = "";
			}

			//编辑箱子名字
			if (player.editedChestName)
			{
				NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f);
				player.editedChestName = false;
			}

			bool isLocked = IsLockedChest(left, top);
			if (Main.netMode == NetmodeID.MultiplayerClient && !isLocked)
			{
				if (left == player.chestX && top == player.chestY && player.chest >= 0)
				{
					player.chest = -1;
					Recipe.FindRecipes();
					SoundEngine.PlaySound(SoundID.MenuClose);
				}
				else
				{
					NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, top);
					Main.stackSplit = 600;
				}
			}
			else
			{
				if (isLocked)
				{
					// 如果手持钥匙并且满足Chest.Unlock条件，那么继续执行解锁
					int key = ModContent.ItemType<Items.Placeable.Furniture.MoireWoodChestKey>();
					if (Chest.Unlock(left, top) && player.ConsumeItem(key))
					{
						if (Main.netMode == NetmodeID.MultiplayerClient)
						{
							NetMessage.SendData(MessageID.Unlock, -1, -1, null, player.whoAmI, 1f, left, top);
						}
					}
				}
				else
				{
					int chest = Chest.FindChest(left, top);
					if (chest >= 0)
					{
						Main.stackSplit = 600;
						if (chest == player.chest)
						{
							player.chest = -1;
							SoundEngine.PlaySound(SoundID.MenuClose);
						}
						else
						{
							SoundEngine.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
							player.chest = chest;
							Main.playerInventory = true;
							Main.recBigList = false;
							player.chestX = left;
							player.chestY = top;
						}

						Recipe.FindRecipes();
					}
				}
			}

			return true;
		}

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			int left = i;
			int top = j;
			if (tile.TileFrameX % 36 != 0)
			{
				left--;
			}

			if (tile.TileFrameY != 0)
			{
				top--;
			}

			int chest = Chest.FindChest(left, top);
			player.cursorItemIconID = -1;

			if (chest < 0)//如果未找到箱子
			{
				player.cursorItemIconText = Language.GetTextValue("LegacyChestType.0");

				Main.NewText("Language.GetTextValue(\"LegacyChestType.0\"):" + Language.GetTextValue("LegacyChestType.0"));
			}
			else
			{
				string defaultName = TileLoader.ContainerName(tile.TileType);
				player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : defaultName;
				if (player.cursorItemIconText == defaultName)
				{
					player.cursorItemIconID = ModContent.ItemType<Items.Placeable.Furniture.MoireWoodChest>();
					if (Main.tile[left, top].TileFrameX / 36 == 1)
					{
						player.cursorItemIconID = ModContent.ItemType<Items.Placeable.Furniture.MoireWoodChestKey>();
					}
					player.cursorItemIconText = "";
				}
			}

			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
		}

		public override void MouseOverFar(int i, int j)
		{
			MouseOver(i, j);
			Player player = Main.LocalPlayer;
			if (player.cursorItemIconText == "")
			{
				player.cursorItemIconEnabled = false;
				player.cursorItemIconID = 0;
			}
		}
	}
}
