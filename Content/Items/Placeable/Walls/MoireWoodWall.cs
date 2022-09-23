﻿
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria;

namespace tRoot.Content.Items.Placeable.Walls
{
    public class MoireWoodWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.AddTranslation(7, "云纹木墙");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 400;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 7;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createWall = ModContent.WallType<Content.Walls.MoireWoodWall>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
            .AddIngredient<Block.MoireWood>()
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}
