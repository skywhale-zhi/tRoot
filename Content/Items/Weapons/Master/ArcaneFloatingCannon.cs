using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Items.Weapons.Master
{
    //奥术浮游炮
    internal class ArcaneFloatingCannon : ModItem
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
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item8;
            Item.DamageType = DamageClass.Magic;
            Item.damage = 68;
            Item.crit = 20;
            Item.knockBack = 5f;
            Item.noMelee = true;
            //Item.shoot = ProjectileID.BlackBolt;
            Item.shoot = ModContent.ProjectileType<Projectiles.Master.ArcaneFloatingCannonProjectile>();
            Item.shootSpeed = 18;
            Item.value = Item.buyPrice(0, 30, 0, 0);
            Item.mana = 6;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Projectiles.Master.ArcaneFloatingCannonProjectile>(), damage, knockback, player.whoAmI, 0, 0);
            return false;
        }

        public override Vector2? HoldoutOrigin()
        {
            Player owner = Main.player[Item.whoAmI];
            Vector2 v = new Vector2(Main.mouseX + Main.screenPosition.X - owner.position.X, Main.mouseY + Main.screenPosition.Y - owner.position.Y);
            v = v.SafeNormalize(Vector2.UnitX);
            return v;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.MagicMissile, 1)
                .AddRecipeGroup(nameof(ItemID.CobaltBar), 8)
                .AddIngredient(ItemID.SoulofLight,3)
                .AddIngredient(ItemID.SoulofNight,3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
