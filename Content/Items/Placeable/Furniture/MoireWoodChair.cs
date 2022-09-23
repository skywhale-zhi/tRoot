using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Items.Placeable.Furniture
{
    public class MoireWoodChair : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.AddTranslation(7, "云纹木椅");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.MoireWoodChair>());
            Item.value = 150;
            Item.maxStack = 99;
            Item.width = 12;
            Item.height = 30;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Block.MoireWood>(8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
