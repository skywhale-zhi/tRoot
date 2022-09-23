using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using tRoot.Common.GlobalProjectiles;

namespace tRoot.Content.Items.Weapons.Shooter
{
    //蔚蓝深海
    internal class BlueDeepSea : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }


        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 30;
            Item.rare = ItemRarityID.Yellow;

            // Use Properties
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item11;

            // Weapon Properties
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 38;
            Item.knockBack = 1f;
            Item.noMelee = true;

            // Gun Properties
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Bullet;

            Item.value = Item.buyPrice(0, 50, 0, 0);
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FlintlockPistol, 1)
                .AddIngredient(ItemID.Handgun, 1)
                .AddIngredient(ItemID.ShroomiteBar, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }


        // 以下方法使该枪有40%的几率不消耗弹药
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextFloat() >= 0.40f;
        }


        //以下方法使枪稍微不准确
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(5));
        }


        //这种方法可以让你调整枪在玩家手中的位置。直到它看起来与你的图形好。
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-6f, 3f);
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            BlueDeepSeaProj bdsp = proj.GetGlobalProjectile<BlueDeepSeaProj>();
            bdsp.enable = true;
            return false;
        }
    }
}
