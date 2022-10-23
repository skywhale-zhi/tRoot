using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace tRoot.Content.Items.Weapons.Shooter
{
    //毁灭冲击
    internal class DestructiveImpact : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }


        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 32;
            Item.scale = 1f;
            Item.rare = ItemRarityID.Pink; // The color that the item's name will be in-game.
            Item.useTurn = false;

            Item.value = Item.buyPrice(0, 30, 0, 0);

            // 发条步枪效果
            Item.useTime = 4;
            Item.useAnimation = 12;
            //间隔延迟
            Item.reuseDelay = 45;
            //最后一发消耗弹药
            Item.consumeAmmoOnLastShotOnly = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;

            // The sound that this item plays when used.
            Item.UseSound = new SoundStyle($"{nameof(tRoot)}/Assets/Sounds/Items/Guns/GunSound_1")
            {
                Volume = 0.9f,
                PitchVariance = 0.0f,
                MaxInstances = 1,
            };

            // Weapon Properties
            Item.DamageType = DamageClass.Ranged; // Sets the damage type to ranged.
            Item.damage = 40; // Sets the item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
            Item.knockBack = 5f; // Sets the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
            Item.noMelee = true; // 因此物品的动画不会造成损坏。

            // Gun Properties
            Item.shoot = ProjectileID.PurificationPowder; // For some reason, all the guns in the vanilla source have this.
            Item.shootSpeed = 10f; // The speed of the projectile (measured in pixels per frame.)
            Item.useAmmo = AmmoID.Bullet; // The "ammo Id" of the ammo item that this weapon uses. Ammo IDs are magic numbers that usually correspond to the item id of one item that most commonly represent the ammo type.
        }

        // 这种方法可以让你调整枪在玩家手中的位置
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10f, -2f);
        }

        // What if I wanted multiple projectiles in a even spread? (Vampire Knives)
        // Even Arc style: Multiple Projectile, Even Spread
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float numberProjectiles = 2 + Main.rand.Next(3); // 2, 3, or 4 shots
            float rotation = MathHelper.ToRadians(1);

            position += Vector2.Normalize(velocity) * 1f;

            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f; // Watch out for dividing by 0 if there is only 1 projectile.
                Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
            }

            return false;
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ClockworkAssaultRifle, 1)
                .AddIngredient(ItemID.HallowedBar, 10)
                .AddIngredient(ItemID.SoulofFright, 20)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
