using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Items.Weapons.Master
{
    //摇蝶灯
    internal class ButterflyLamp : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.scale = 1f;
            Item.rare = ItemRarityID.Purple;
            Item.useTurn = false;
            Item.useTime = 1;
            Item.useAnimation = 1;
            Item.useStyle = ItemUseStyleID.RaiseLamp;
            Item.holdStyle = ItemHoldStyleID.HoldLamp;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item8;
            Item.DamageType = DamageClass.Magic;
            Item.damage = 82;
            Item.crit = 15;
            Item.knockBack = 5f;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.Master.ButterflyLampProjectile1>();
            Item.shootSpeed = 6;
            Item.value = Item.sellPrice(0, 30, 0, 0);
            Item.mana = 3;
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
            int i;
            if (Main.rand.NextBool(2))
            {
                i = player.direction;
            }
            else
            {
                i = -player.direction;
            }

            int j = Main.rand.Next(100);
            Vector2 Position = position + player.Directions * new Vector2(40, -10);
            for (int f = 0; f < 1; f++)
            {
                if (j < 45)//螺旋飞蝶
                {
                    Projectile.NewProjectile(source, Position, velocity.RotatedByRandom(MathHelper.Pi / 6), ModContent.ProjectileType<Projectiles.Master.ButterflyLampProjectile1>(), damage * 2, 0, player.whoAmI, Main.rand.NextFloat(0.8f, 1.5f) * i);
                }
                else if (j < 90)//追踪飞蝶
                {
                    Projectile.NewProjectile(source, Position, velocity.RotatedByRandom(MathHelper.Pi / 12) * 2, ModContent.ProjectileType<Projectiles.Master.ButterflyLampProjectile2>(), damage, knockback, player.whoAmI, Main.rand.NextFloat(0.8f, 1.5f) * i);
                }
                else//环绕飞蝶
                {
                    Projectile.NewProjectile(source, Position, Vector2.Zero, ModContent.ProjectileType<Projectiles.Master.ButterflyLampProjectile3>(), damage * 3, knockback, player.whoAmI, Main.rand.NextFloat(0.8f, 1.5f) * i, Main.rand.NextFloat(0.8f, 1.5f));
                }
            }
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-50, 0);
        }

        public override void HoldItemFrame(Player player)
        {
            base.HoldItemFrame(player);
        }


        public override Vector2? HoldoutOrigin()
        {
            return base.HoldoutOrigin();
        }


        public override void HoldItem(Player player)
        {
            Vector2 setoff = player.Directions * new Vector2(40, -10);
            Lighting.AddLight(player.Center + setoff, new Vector3(255, 165, 0) * 0.01f);
            //Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center + setoff, Vector2.Zero, ModContent.ProjectileType<Projectiles.Master.ButterflyLampProjectile3>(), 0, 0, player.whoAmI, setoff.X, setoff.Y);

            if (Main.rand.NextBool(10))
            {
                if (player.direction == 1)
                {
                    Dust dust = Dust.NewDustDirect(player.Center + setoff - new Vector2(15, 13), 30, 30, DustID.RedTorch, 0f, 0f, 0, new Color(255, 255, 255), 3f);
                    dust.noGravity = true;
                }
                else
                {
                    Dust dust = Dust.NewDustDirect(player.Center + setoff - new Vector2(19, 13), 30, 30, DustID.RedTorch, 0f, 0f, 0, new Color(255, 255, 255), 3f);
                    dust.noGravity = true;
                }
            }

        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.EmpressButterfly, 3)
                .AddIngredient(ItemID.ChineseLantern, 1)
                .AddIngredient(ItemID.FairyQueenMagicItem, 1)
                .AddIngredient(ItemID.LunarBar, 100)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
