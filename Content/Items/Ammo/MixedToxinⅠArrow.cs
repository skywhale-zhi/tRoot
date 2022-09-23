using tRoot.Content.Tiles.Furniture;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace tRoot.Content.Items.Ammo
{
    internal class MixedToxinⅠArrow : ModItem
	{
		//混调毒素1 箭
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
		}
		public override void SetDefaults()
		{
			Item.damage = 19;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 14;
			Item.height = 32;
			Item.maxStack = 9999;
			Item.consumable = true;
			Item.knockBack = 4f;
			Item.value = 25;
			Item.rare = ItemRarityID.Pink;
			Item.shoot = ModContent.ProjectileType<Projectiles.Shooter.MixedToxinⅠArrowProjectile>();
			Item.shootSpeed = 4f;
			//该弹药所属的弹药类别。
			Item.ammo = AmmoID.Arrow;
		}


		public override void AddRecipes()
		{
			CreateRecipe(150)
				.AddIngredient(ItemID.WoodenArrow,150)
				.AddIngredient(ItemID.Ichor, 1)
				.AddIngredient(ItemID.CursedFlame, 1)
				.AddIngredient(ItemID.VialofVenom, 1)
				.AddIngredient(ModContent.ItemType<Things.BufferBalancingAgentⅡ>(), 1)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}
