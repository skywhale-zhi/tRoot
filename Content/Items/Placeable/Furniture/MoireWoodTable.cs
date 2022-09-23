using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace tRoot.Content.Items.Placeable.Furniture
{
    public class MoireWoodTable : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.AddTranslation(7, "云纹木桌");  //中文
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.MoireWoodTable>());
            Item.value = 150;
            Item.maxStack = 99;
            Item.width = 38;
            Item.height = 24;
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
