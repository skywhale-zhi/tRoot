using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Items.Weapons.Master
{
	//圣剑符文石
	internal class HolySwordRune : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 46;
			Item.height = 46;
			Item.scale = 1f;
			Item.rare = ItemRarityID.LightRed;
			Item.useTurn = false;
			Item.useTime = 40;
			Item.useAnimation = 40;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.autoReuse = true;
			Item.UseSound = SoundID.Item8;
			Item.DamageType = DamageClass.Magic;
			Item.damage = 45;
			Item.crit = 10;
			Item.knockBack = 5f;
			Item.noMelee = true;
			Item.shoot = ProjectileID.BlackBolt;
			Item.shootSpeed = 18;
			Item.mana = 26;
			Item.value = Item.buyPrice(0, 30, 0, 0);
			Item.staff[Item.type] = true;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Projectiles.Master.MagicSword>(), damage, knockback, player.whoAmI, 0, 0);
			return false;
		}

		public override Vector2? HoldoutOrigin()
		{
			Player owner = Main.player[Item.whoAmI];
			Vector2 v = new Vector2(Main.mouseX + Main.screenPosition.X - owner.position.X, Main.mouseY + Main.screenPosition.Y - owner.position.Y);
			v = v.SafeNormalize(Vector2.UnitX);
			return v;
		}
	}
}
