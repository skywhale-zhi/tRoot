using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria;
using Terraria.Localization;

namespace tRoot.Content.Items.Placeable.Furniture
{
    internal class CookingPot : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.AddTranslation(7, "烹饪锅");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 34;
			Item.height = 40;
			Item.maxStack = 99;
			Item.value = Item.buyPrice(0,3,0,0);
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.CookingPot>(), 0);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddRecipeGroup(RecipeGroupID.Wood,20)
				.AddRecipeGroup(RecipeGroupID.IronBar,10)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}
