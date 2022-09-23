using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace tRoot.Content.Items.Weapons.Warrior
{
    internal class BanDianStarSword : ModItem
    {
        //斑淀星剑
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

		public override void SetDefaults()
		{
			Item.width = 42;
			Item.height = 60;

			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.autoReuse = true;

			Item.DamageType = DamageClass.Melee;
			Item.damage = 82;
            Item.knockBack = 2;
			Item.crit = 6;

			Item.value = Item.buyPrice(gold: 5);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item1;

			Item.shootSpeed = 20f;                  // 弹幕的飞行速度
            Item.shoot = ModContent.ProjectileType<Projectiles.Warrior.BanDianStarSwordProjectile>();
            // If you want melee speed to only affect the swing speed of the weapon and not the shoot speed (not recommended)
            // Item.attackSpeedOnlyAffectsWeaponAnimation = true;
        }

        //真近战挥舞影响
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            //添加灰尘
            if(Main.rand.NextBool(3))
                Dust.NewDust(new Vector2(hitbox.X,hitbox.Y), hitbox.Width, hitbox.Height, DustID.MagicMirror, 0, 0, 150, default, 0.7f);
        }

        //允许你修改这个项目的射击机制，默认为真，请手动改为假
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 target = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
            int num = Main.rand.Next(3) + 1;
            for (int i = 0; i < num; i++)
            {
                position = player.Center - new Vector2(Main.rand.NextFloat(601) * player.direction, 600f);
                if(player.direction == 1)
                {
                    position.X += (Main.rand.Next(100) + 500) ;
                }
                else
                {
                    position.X -= (Main.rand.Next(100) + 500) ;
                }
                position.Y += (Main.rand.Next(200) - 100) * i;
                Vector2 heading = target - position;

                if (heading.Y < 0f)
                {
                    heading.Y *= -1f;
                }

                if (heading.Y < 20f)
                {
                    heading.Y = 20f;
                }

                heading.Normalize();
                heading *= velocity.Length();
                //设置武器命中打击的精准性
                heading.Y += Main.rand.Next(-40, 41) * 0.02f;
                Projectile.NewProjectile(source, position, heading, type, damage, knockback, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Starfury)
                .AddRecipeGroup(nameof(ItemID.MythrilBar),10)
				.AddIngredient(ItemID.SoulofLight,3)
				.AddIngredient(ItemID.SoulofNight,3)
                .AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}
