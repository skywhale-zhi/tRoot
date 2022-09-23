using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace tRoot.Content.Items.Weapons.Warrior
{
    internal class BloodDrawingLance : ModItem
    {
		//夜空长明
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

        public override string Texture => "tRoot/Content/Items/Weapons/Warrior/BloodySpinningSpear";

        public override void SetDefaults()
		{
			Item.width = 42;
			Item.height = 60;

			Item.useStyle = ItemUseStyleID.Swing; 
			Item.noUseGraphic = true;   //禁止使用自身的贴图，防止把自己贴图放进去

			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.autoReuse = true;

			Item.DamageType = DamageClass.Melee;
			Item.damage = 145;
			Item.knockBack = 5;
			Item.crit = 10;
			Item.noMelee = true; // 因此物品的动画不会造成损坏。

			Item.value = Item.sellPrice(gold: 30);

			Item.rare = ItemRarityID.Purple;
			Item.UseSound = SoundID.Item1;

			Item.shootSpeed = 30f;                  // 弹幕的飞行速度
			Item.shoot = ModContent.ProjectileType<Projectiles.Warrior.BloodDrawingLanceProjectile>();

			// If you want melee speed to only affect the swing speed of the weapon and not the shoot speed (not recommended)
			// Item.attackSpeedOnlyAffectsWeaponAnimation = true;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0);
			return false;
        }

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.DayBreak)
				.AddIngredient(ItemID.LunarBar, 50)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}
	}
}
