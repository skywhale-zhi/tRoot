using tRoot.Content.Tiles.Furniture;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace tRoot.Content.Items.Ammo
{
    internal class JadeSpiralBullet : ModItem
	{
		//螺玉弹
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
		}

		public override void SetDefaults()
		{
			Item.damage = 9;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 8;
			Item.height = 8;
			Item.maxStack = 9999;
			//这会将物品标记为消耗品，使其在用作弹药或其他物品时自动消耗（如果可能）
			Item.consumable = true;
			Item.knockBack = 1.5f;
			Item.value = 30;
			Item.rare = ItemRarityID.Green;
			Item.shoot = ModContent.ProjectileType<Projectiles.Shooter.JadeSpiralBullet>();
			Item.shootSpeed = 4f;
			//该弹药所属的弹药类别。
			Item.ammo = AmmoID.Bullet;
		}


		public override void AddRecipes()
		{
		}
	}
}
