using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Projectiles.Shooter
{
    internal class MixedToxinⅠArrowProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }


        public override void SetDefaults()
        {
            Projectile.width = 6; // 碰撞箱宽
            Projectile.height = 6; // 碰撞箱长
            DrawOffsetX = -4;
            DrawOriginOffsetY = 0;

            //贴图ai的应用，自动适应1号ai风格，吧贴图扭转
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            //弹药大小
            Projectile.scale = 1f;
            //弹药AI改为。。，不会受重力影响等
            AIType = ProjectileID.WoodenArrowFriendly;
            //弹药性质改成箭
            Projectile.arrow = true;

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            Projectile.Kill();
            return false;
        }

        public override void AI()
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.MixedToxinⅠDust>(), Main.rand.Next(-1, 1), Main.rand.Next(-1, 1), 180, default, 1.5f);
            dust.velocity *= 0.4f;
            Lighting.AddLight(Projectile.Center, 1f, 0.58f, 0);

            if (Main.rand.Next(30) % 20 == 0 && Main.rand.NextBool(40 - Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<MixedToxinⅠBulletProjectile2>()], 40))
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitY * 8f, ModContent.ProjectileType<MixedToxinⅠBulletProjectile2>(), (int)(Projectile.damage * 0.7f), 0, Projectile.owner, 1);
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.MixedToxinⅠDust>(), 0, 0, 100, default, 1.5f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Buffs.FriendlyBuffs.MixedToxinⅠ>(), 360);
        }
    }
}
