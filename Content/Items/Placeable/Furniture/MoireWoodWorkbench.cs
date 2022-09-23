using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using tRoot.Content.Items.Placeable.Block;

namespace tRoot.Content.Items.Placeable.Furniture
{
    public class MoireWoodWorkbench : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.AddTranslation(7,"云纹木工作台");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            //创建可放置贴图，对应贴图类的那个工作台
            Item.createTile = ModContent.TileType<Tiles.Furniture.MoireWoodWorkbench>(); // This sets the id of the tile that this item should place when used.

            Item.width = 32; // The item texture's width
            Item.height = 18; // The item texture's height

            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.useAnimation = 15;

            Item.maxStack = 99;
            Item.consumable = true;
            Item.value = 150;

        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Items.Placeable.Block.MoireWood>(),10)
                .Register();
        }
    }
}
