using Microsoft.Xna.Framework;
using System.Collections.Generic;
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
            Item.height = 100; // Hitbox height of the item.
            Item.scale = 1f;
            Item.rare = ItemRarityID.Purple; // The color that the item's name will be in-game.

            // Use Properties
            Item.useTime = 20; // The item's use time in ticks (60 ticks == 1 second.)
            Item.useAnimation = 20; // The length of the item's use animation in ticks (60 ticks == 1 second.)

            Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, etc.)
            Item.autoReuse = true; // Whether or not you can hold click to automatically use it again.

            // The sound that this item plays when used.
            Item.UseSound = SoundID.Item5;
            Item.value = Item.sellPrice(gold: 30);


            // Weapon Properties
            Item.DamageType = DamageClass.Ranged; // Sets the damage type to ranged.
            Item.damage = 100; // Sets the item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
            Item.knockBack = 3f; // Sets the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
            Item.noMelee = true; // 因此物品的动画不会造成损坏。

            // Gun Properties
            Item.shoot = ProjectileID.WoodenArrowFriendly; // For some reason, all the guns in the vanilla source have this.
            Item.shootSpeed = 15f; // The speed of the projectile (measured in pixels per frame.)
            Item.useAmmo = AmmoID.Arrow; // The "ammo Id" of the ammo item that this weapon uses. Ammo IDs are magic numbers that usually correspond to the item id of one item that most commonly represent the ammo type.
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
            float rotation = MathHelper.ToRadians(0.5f);
            position += Vector2.Normalize(velocity) * 5f;
            //设置随机每次射出的箭数目
            int num;
            if (Main.rand.NextBool(2, 5))
            {
                num = 6;
            }
            else
            {
                num = 5;
            }
            for (int i = 0; i < num; i++)
            {
                Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i));
                //把木箭和燃烧箭转化成闪电矢
                if (type == 1 || type == 2)
                {
                    int index = Projectile.NewProjectile(source, position, perturbedSpeed * 0.5f, ModContent.ProjectileType<Projectiles.Shooter.LightningBolt>(), (int)(damage * 0.8f), knockback, player.whoAmI);
                    Main.projectile[index].GetGlobalProjectile<StarDestroyerGProj>().enable = true;
                }
                //否则，射出两只闪电矢，其他的变成弹药箭
                else
                {
                    if (i == 0 || i == 3)
                    {
                        int index = Projectile.NewProjectile(source, position, perturbedSpeed * 0.5f, ModContent.ProjectileType<Projectiles.Shooter.LightningBolt>(), (int)(damage * 0.8f), knockback, player.whoAmI);
                        Main.projectile[index].GetGlobalProjectile<StarDestroyerGProj>().enable = true;
                    }
                    else
                    {
                        int index = Projectile.NewProjectile(source, position, perturbedSpeed, type, (int)(damage * 1.2f), knockback, player.whoAmI);
                        Main.projectile[index].GetGlobalProjectile<StarDestroyerGProj>().enable = true;
                        Main.projectile[index].extraUpdates += 1;
                        Main.projectile[index].netUpdate = true;
                    }
                }
            }
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DaedalusStormbow)
                .AddIngredient(ItemID.PulseBow)
                .AddIngredient(ItemID.LunarBar, 100)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }

    internal class StarDestroyerGProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool enable;
        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            //如果射出的是闪电矢，则撞击敌人后释放两只电浆
            if (enable && projectile.type == ModContent.ProjectileType<Projectiles.Shooter.LightningBolt>())
            {
                for (int i = 0; i < 2; i++)
                {
                    int index = Projectile.NewProjectile(projectile.GetSource_OnHit(target), target.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 16f, ProjectileID.ThunderSpearShot, (int)(damage * 0.5f), knockback, projectile.owner);
                    Main.projectile[index].localNPCHitCooldown = 30;
                    Main.projectile[index].usesLocalNPCImmunity = true;
                    Main.projectile[index].extraUpdates = 0;
                    Main.projectile[index].DamageType = DamageClass.Ranged;
                    Main.projectile[index].penetrate = -1;
                    Main.projectile[index].tileCollide = false;
                    Main.projectile[index].ArmorPenetration = 25;
                    Main.projectile[index].netUpdate = true;
                }
                if (Main.rand.NextBool(3))
                    target.AddBuff(ModContent.BuffType<Buffs.Thunder>(), Main.rand.Next(360, 720));
            }
            //否则撞击敌人后释放1只电浆
            if (enable && projectile.type != ModContent.ProjectileType<Projectiles.Shooter.LightningBolt>())
            {
                int index;
                if (projectile.penetrate > 1)
                {
                    index = Projectile.NewProjectile(projectile.GetSource_OnHit(target), target.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 16f, ProjectileID.ThunderSpearShot, (int)(damage * 0.5f), knockback, projectile.owner);
                }
                else
                {
                    index = Projectile.NewProjectile(projectile.GetSource_OnHit(target), target.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 16f, ProjectileID.ThunderSpearShot, (int)(damage * 0.7f), knockback, projectile.owner);
                }
                Main.projectile[index].localNPCHitCooldown = 30;
                Main.projectile[index].usesLocalNPCImmunity = true;
                Main.projectile[index].extraUpdates = 0;
                Main.projectile[index].DamageType = DamageClass.Ranged;
                Main.projectile[index].penetrate = -1;
                Main.projectile[index].tileCollide = false;
                Main.projectile[index].ArmorPenetration = 25;
                Main.projectile[index].netUpdate = true;
            }
        }

        public override void Kill(Projectile projectile, int timeLeft)
        {
            //如果射出的是闪电矢，弹药死亡后释放两只电浆
            if (enable && projectile.type == ModContent.ProjectileType<Projectiles.Shooter.LightningBolt>())
            {
                for (int i = 0; i < 2; i++)
                {
                    int index = Projectile.NewProjectile(projectile.GetSource_Death(), projectile.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 13f, ProjectileID.ThunderSpearShot, (int)(projectile.damage * 0.5f), 0, projectile.owner);
                    Main.projectile[index].localNPCHitCooldown = 20;
                    Main.projectile[index].usesLocalNPCImmunity = true;
                    Main.projectile[index].extraUpdates = 0;
                    Main.projectile[index].DamageType = DamageClass.Ranged;
                    Main.projectile[index].penetrate = -1;
                    Main.projectile[index].tileCollide = false;
                    Main.projectile[index].ArmorPenetration = 25;
                    Main.projectile[index].netUpdate = true;
                }
            }
            //如果射出的不是闪电矢，弹药死亡后释放1只电浆
            if (enable && projectile.type != ModContent.ProjectileType<Projectiles.Shooter.LightningBolt>())
            {
                int index = Projectile.NewProjectile(projectile.GetSource_Death(), projectile.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 13f, ProjectileID.ThunderSpearShot, (int)(projectile.damage * 0.7f), 0, projectile.owner);
                Main.projectile[index].localNPCHitCooldown = 20;
                Main.projectile[index].usesLocalNPCImmunity = true;
                Main.projectile[index].extraUpdates = 0;
                Main.projectile[index].DamageType = DamageClass.Ranged;
                Main.projectile[index].penetrate = -1;
                Main.projectile[index].tileCollide = false;
                Main.projectile[index].ArmorPenetration = 25;
                Main.projectile[index].netUpdate = true;
            }
        }
    }
}
