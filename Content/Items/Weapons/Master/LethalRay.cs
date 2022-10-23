using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Items.Weapons.Master
{
    internal class LethalRay : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.scale = 1f;
            Item.rare = ItemRarityID.Green;
            Item.useTurn = false;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item12;
            Item.DamageType = DamageClass.Magic;
            Item.damage = 8;
            Item.crit = 0;
            Item.knockBack = 1f;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<Projectiles.Master.LethalRayProjectile>();
            Item.shootSpeed = 0.5f;
            Item.value = Item.sellPrice(0, 1, 0, 0); 
            Item.staff[Item.type] = true;
            Item.mana = 20;
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 pos = position + velocity * 70;
            Projectile.NewProjectile(source,pos,velocity,type,damage,knockback,player.whoAmI);
            return false;
        }


        public override Vector2? HoldoutOrigin()
        {
            Player owner = Main.player[Item.whoAmI];
            Vector2 v = new Vector2(Main.mouseX + Main.screenPosition.X - owner.position.X, Main.mouseY + Main.screenPosition.Y - owner.position.Y);
            v = v.SafeNormalize(Vector2.UnitX);
            return v;
        }
    }
}
