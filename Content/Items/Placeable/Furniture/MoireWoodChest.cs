using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria;

namespace tRoot.Content.Items.Placeable.Furniture
{
    public class MoireWoodChest : ModItem
    {
		public override void SetStaticDefaults()
		{
			DisplayName.AddTranslation(7,"云纹木箱");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 22;
			Item.maxStack = 99;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.value = 500;
			Item.createTile = ModContent.TileType<Tiles.Furniture.MoireWoodChest>();
			//Item.placeStyle = 1; // 使用此选项将箱子锁定
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<Block.MoireWood>(8)
				.AddIngredient(ItemID.IronBar, 2)
				.AddTile(TileID.WorkBenches)
				.Register();
			CreateRecipe()
				.AddIngredient<Block.MoireWood>(8)
				.AddIngredient(ItemID.LeadBar, 2)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}

	public class MoireWoodChestKey : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.AddTranslation(7, "云纹木箱钥匙");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3; // Biome keys usually take 1 item to research instead.
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.GoldenKey);
			Item.width = 14;
			Item.height = 20;
			Item.maxStack = 99;
		}
	}
}
