using Microsoft.Xna.Framework;
using tRoot.Content.Projectiles.Warrior;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Items.Weapons.Warrior
{
    internal class ChlorophyteDagger : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.AddTranslation(7, "绿茎匕首");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 126;
			Item.knockBack = 4f;
			Item.useStyle = ItemUseStyleID.Rapier;
			Item.useAnimation = 6;
			Item.useTime = 6;
			Item.width = 32;
			Item.height = 32;
			Item.UseSound = SoundID.Item1;
			Item.DamageType = DamageClass.MeleeNoSpeed;
			Item.autoReuse = false;
			Item.noUseGraphic = true;
			//射弹会造成伤害，而不是物品
			Item.noMelee = true;
			Item.autoReuse = true;

			Item.rare = ItemRarityID.Lime;
			Item.value = Item.sellPrice(0, 8, 0, 0);

			Item.shoot = ModContent.ProjectileType<ChlorophyteDaggerProjectile>(); // The projectile is what makes a shortsword work
			Item.shootSpeed = 3f; // This value bleeds into the behavior of the projectile as velocity, keep that in mind when tweaking values
		}



		//以下方法使枪稍微不准确
		//public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		//{
		//	velocity = velocity.RotatedByRandom(MathHelper.ToRadians(17));
		//}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.ChlorophyteBar, 8)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}
