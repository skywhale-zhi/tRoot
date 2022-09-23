using tRoot.Content.Tiles.Furniture;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace tRoot.Content.Items.Ammo
{
    internal class IceFireArrow : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.AddTranslation(7, "冰火箭");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
		}

		public override void SetDefaults()
		{
			Item.damage = 10;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 14;
			Item.height = 32;
			Item.maxStack = 999;
			//这会将物品标记为消耗品，使其在用作弹药或其他物品时自动消耗（如果可能）
			Item.consumable = true;
			Item.knockBack = 1.5f;
			Item.value = 25;
			Item.rare = ItemRarityID.Blue;
			Item.shoot = ModContent.ProjectileType<Projectiles.Shooter.IceFireArrow>();
			Item.shootSpeed = 4f;
			//该弹药所属的弹药类别。
			Item.ammo = AmmoID.Arrow;
		}

		public override void AddRecipes()
		{
			CreateRecipe(30)
				.AddIngredient(ItemID.FlamingArrow,20)
				.AddIngredient(ItemID.FrostburnArrow,20)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}
