﻿using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace tRoot.Content.Items.Placeable.Furniture
{
    public class MoireWoodBed : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.AddTranslation(7, "云纹木床");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 20;
			Item.maxStack = 99;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.value = 2000;
			Item.createTile = ModContent.TileType<Tiles.Furniture.MoireWoodBed>();
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<Block.MoireWood>(15)
				.AddIngredient(ItemID.Silk,5)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}
