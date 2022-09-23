using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Items.Weapons.Master
{
    //宝石节枝法杖
    internal class ColorfulGemStaff : ModItem
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
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item8;
            Item.DamageType = DamageClass.Magic;
            Item.damage = 80;
            Item.knockBack = 5f;
            Item.noMelee = true;
            Item.shoot = ProjectileID.BlackBolt;
            Item.shootSpeed = 14;
            Item.mana = 40;
            Item.staff[Item.type] = true;
            Item.value = Item.buyPrice(0, 30, 0, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float f = velocity.ToRotation();
            for (int i = -1; i <= 1; i++)
            {
                f += i * (float)MathHelper.ToRadians(1);
                velocity = f.ToRotationVector2() * Item.shootSpeed * (Main.rand.NextFloat(0.6f, 1.2f));
                int n = Main.rand.NextBool(2) ? 1 : -1;
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<Projectiles.Master.ColorfulGemProjectile>(), damage, knockback, player.whoAmI, Main.rand.NextFloat(0.7f, 1f) * n, 0);
            }
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
                .AddIngredient(ItemID.AmethystStaff, 1)
                .AddIngredient(ItemID.SapphireStaff, 1)
                .AddIngredient(ItemID.RubyStaff, 1)
                .AddIngredient(ItemID.AmberStaff, 1)
                .AddIngredient(ItemID.SoulofLight, 3)
                .AddIngredient(ItemID.SoulofNight, 3)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.TopazStaff, 1)
                .AddIngredient(ItemID.EmeraldStaff, 1)
                .AddIngredient(ItemID.DiamondStaff, 1)
                .AddIngredient(ItemID.AmberStaff, 1)
                .AddIngredient(ItemID.SoulofLight, 3)
                .AddIngredient(ItemID.SoulofNight, 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
