using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Items.Weapons.Summoner
{
    //夜明之锋
    internal class LuminiteBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            //这可以让玩家在使用控制器时瞄准整个屏幕上的任何位置
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.knockBack = 0.5f;
            Item.mana = 10;
            Item.width = 36;
            Item.height = 36;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.scale = 1f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(gold: 30);
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = true;

            Item.noMelee = true;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = ModContent.BuffType<Buffs.Minions.LuminiteBladeBuff>();
            Item.shoot = ModContent.ProjectileType<Projectiles.Summoner.LuminiteBladeProjectile>();
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


        //在这里你可以改变仆从的出生位置。大多数原版仆从在光标位置生成
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
            for (int i = 0; i < 40; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                Dust.NewDustPerfect(position, 160, speed * 6, 0, default, 2);
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // 这是必需的，因此可以使你的仆从存活并允许你将其摧毁的buff可以正确应用
            player.AddBuff(Item.buffType, 3600);

            // 必须手动生成仆从，然后将原始伤害分配给召唤物品的伤害
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;

            // 由于我们已经手动生成了投射物，我们不再需要游戏为自己生成它，因此返回false
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Smolstar)
                .AddIngredient(ItemID.FlyingKnife)
                .AddIngredient(ItemID.LunarBar, 100)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
