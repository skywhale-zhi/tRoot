using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Items.Weapons.Shooter
{
    internal class StarDestroyer : ModItem
    {
        //歼星者
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 38; // Hitbox width of the item.
            Item.height = 80; // Hitbox height of the item.
            Item.scale = 1f;
            Item.rare = ItemRarityID.Red; // The color that the item's name will be in-game.

            // Use Properties
            Item.useTime = 25; // The item's use time in ticks (60 ticks == 1 second.)
            Item.useAnimation = 25; // The length of the item's use animation in ticks (60 ticks == 1 second.)

            Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, etc.)
            Item.autoReuse = true; // Whether or not you can hold click to automatically use it again.

            // The sound that this item plays when used.
            Item.UseSound = SoundID.Item5;
            Item.value = Item.buyPrice(1, 0, 0, 0);

            // Weapon Properties
            Item.DamageType = DamageClass.Ranged; // Sets the damage type to ranged.
            Item.damage = 80; // Sets the item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
            Item.knockBack = 3f; // Sets the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
            Item.noMelee = true; // 因此物品的动画不会造成损坏。

            // Gun Properties
            Item.shoot = ProjectileID.PurificationPowder; // For some reason, all the guns in the vanilla source have this.
            Item.shootSpeed = 12f; // The speed of the projectile (measured in pixels per frame.)
            Item.useAmmo = AmmoID.Arrow; // The "ammo Id" of the ammo item that this weapon uses. Ammo IDs are magic numbers that usually correspond to the item id of one item that most commonly represent the ammo type.
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float rotation = MathHelper.ToRadians(0.5f);
            position += Vector2.Normalize(velocity) * 45f;
            int num;
            if (Main.rand.NextBool(3))
            {
                num = 5;
            }
            else if(Main.rand.NextBool(2))
            {
                num = 4;
            }
            else
            {
                num = 3;
            }
            for (int i = 0; i < num; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i)); // Watch out for dividing by 0 if there is only 1 projectile.
                Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
            }
            return false; // return false to stop vanilla from calling Projectile.NewProjectile.
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5,0);
        }
    }
}
