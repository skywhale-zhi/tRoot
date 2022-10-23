using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.Audio;

namespace tRoot.Content.Items.Weapons.Warrior
{
    internal class BloodDrawingLance : ModItem
    {
		//夜空长明
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

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
			Item.damage = 180;
			Item.knockBack = 5;
			Item.crit = 10;
			Item.noMelee = true; // 因此物品的动画不会造成损坏。

			Item.value = Item.sellPrice(gold: 30);

			Item.rare = ItemRarityID.Purple;
			Item.UseSound = SoundID.Item131;

			Item.shootSpeed = 30f;                  // 弹幕的飞行速度
			Item.shoot = ModContent.ProjectileType<Projectiles.Warrior.BloodDrawingLanceProjectile>();
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			// This code shows using Color.Lerp,  Main.GameUpdateCount, and the modulo operator (%) to do a neat effect cycling between 4 custom colors.
			int numColors = tRoot.ItemNameColor1.Length;

			foreach (TooltipLine line2 in tooltips)
			{
				if (line2.Mod == "Terraria" && line2.Name == "ItemName")
				{
					float fade = (Main.GameUpdateCount % 60) / 60f;
					int index = (int)((Main.GameUpdateCount / 60) % numColors);
					int nextIndex = (index + 1) % numColors;

					line2.OverrideColor = Color.Lerp(tRoot.ItemNameColor1[index], tRoot.ItemNameColor1[nextIndex], fade);
				}
			}
		}


		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, 0);
			return false;
        }

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.BoneJavelin, 20)
				.AddIngredient(ItemID.DayBreak)
				.AddIngredient(ItemID.LunarBar, 100)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}
	}
}
