using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Items.Weapons.Master
{
    //解离射线法杖
    internal class DissociationRayStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 70;
            Item.height = 70;
            Item.rare = ItemRarityID.Cyan;
            Item.useTurn = false;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item8;
            Item.DamageType = DamageClass.Magic;
            Item.damage = 100;
            Item.crit = 4;
            Item.knockBack = 1f;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.Master.DissociationRayProjectile>();
            Item.shootSpeed = 7f;
            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.staff[Item.type] = true;
            Item.mana = 10;
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 pos = position;
            Projectile.NewProjectile(source, pos, velocity.RotatedBy(0.03), type, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, pos, velocity, type, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, pos, velocity.RotatedBy(-0.03), type, damage, knockback, player.whoAmI);
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
                .AddIngredient(ItemID.FragmentNebula, 10)
                .AddIngredient(ModContent.ItemType<LethalRay>(), 1)
                .AddIngredient(ItemID.ShadowbeamStaff, 1)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
