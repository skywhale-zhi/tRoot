using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Items.Weapons.Shooter
{
    internal class GalvanicCoupleGun : ModItem
    {
        //电偶枪
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }


        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.rare = ItemRarityID.Yellow;

            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item94;

            Item.DamageType = DamageClass.Ranged;
            Item.damage = 72;
            Item.knockBack = 1f;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<Projectiles.Shooter.GalvanicCoupleProjectile1>();
            Item.shootSpeed = 1f;
            Item.useAmmo = AmmoID.Bullet;

            Item.value = Item.buyPrice(0, 50, 0, 0);
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Wire, 10)
                .AddIngredient(ItemID.FlintlockPistol, 1)
                .AddIngredient(ItemID.MagnetSphere, 1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }


        //这种方法可以让你调整枪在玩家手中的位置。直到它看起来与你的图形好。
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-6f, -5f);
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 pos = position + new Vector2(player.direction * 5, -5);
            int index = Projectile.NewProjectile(source, pos, 
                velocity.RotatedBy(-MathHelper.Pi / 6 * Main.rand.NextFloat(0.4f, 1.4f)) * Main.rand.NextFloat(0.6f,1.4f), 
                ModContent.ProjectileType<Projectiles.Shooter.GalvanicCoupleProjectile1>(), 
                damage * 2, knockback, player.whoAmI, -2);

            Projectile.NewProjectile(source, pos, 
                velocity.RotatedBy(MathHelper.Pi / 6 * Main.rand.NextFloat(0.4f, 1.4f)) * Main.rand.NextFloat(0.6f, 1.4f), 
                ModContent.ProjectileType<Projectiles.Shooter.GalvanicCoupleProjectile1>(), 
                damage * 2, knockback, player.whoAmI, -1, index);
            return false;
        }
    }
}
