using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Projectiles.Shooter
{
    internal class MixedToxinⅠBulletProjectile : ModProjectile
    {
        private int timer = 0;
        public int Timer
        {
            get { return timer; }
            set { timer = value; }
        }

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 2; // 碰撞箱宽
            Projectile.height = 2; // 碰撞箱长

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;

            //贴图ai的应用，自动适应1号ai风格，吧贴图扭转
            //Projectile.aiStyle = 1; // ai风格
            Projectile.friendly = true; // 伤害敌人？
            Projectile.hostile = false; // 伤害玩家？
            Projectile.DamageType = DamageClass.Ranged; // 炮弹是由远程武器发射的
            Projectile.penetrate = 2; // 射弹能穿透多少怪物。（下面的OnTileCollide也会减少穿透反弹）
            Projectile.timeLeft = 600;
            Projectile.alpha = 255; // 弹丸的透明度，255表示完全透明。（aiStyle 1快速淡入投射物）如果您没有使用淡入的aiStyle，请确保删除此项。你会想知道为什么你的射弹是看不见的。
            Projectile.ignoreWater = false; // 弹丸的速度忽略水的影响吗？
            Projectile.tileCollide = true; // 弹丸会与瓷砖碰撞吗？
            Projectile.extraUpdates = 2;
            //弹药性质改为子弹，不会受重力影响等
            //AIType = ProjectileID.Bullet; // Act exactly like default Bullet
            //Projectile.light = 1f;
        }

        public override void AI()
        {
            Timer++;
            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.rotation += MathHelper.PiOver2;
            FadeInAndOut();
            Lighting.AddLight(Projectile.Center, 1f, 0.58f, 0);
        }


        //淡出淡入效果
        public void FadeInAndOut()
        {
            // 刚开始的20帧内
            if (Timer <= 20f)
            {
                // 淡入
                Projectile.alpha -= 12;
                // 强制透明度为0
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;

                return;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.NextBool(2))
                target.AddBuff(ModContent.BuffType<Buffs.FriendlyBuffs.MixedToxinⅠ>(), 360);
            if (Main.rand.NextBool(40 - Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<MixedToxinⅠBulletProjectile2>()], 40))
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.One.RotateRandom(MathHelper.TwoPi) * 2f, ModContent.ProjectileType<MixedToxinⅠBulletProjectile2>(), (int)(Projectile.damage * 0.3f), 0, Projectile.owner);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.Kill();
            //撞击，击中贴图引发的贴图效果，如方块灰尘等
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.MixedToxinⅠDust>(), 0, 0, 100, default, 1.5f);
            }
            return false;
        }


        public override void Kill(int timeLeft)
        {
            //EntitySource_ItemUse_WithAmmo source;
            //此代码和上面在OnTileCollide中的类似代码会从与之碰撞的瓷砖中产生灰尘。SoundID。第10项是你听到的反弹声。
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            if (!Main.dedServ)
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.MixedToxinⅠDust>(), 0, 0, 100, default, 1.5f);
                }
            if (Main.rand.NextBool(40 - Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<MixedToxinⅠBulletProjectile2>()], 40))
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero) * 5f, ModContent.ProjectileType<MixedToxinⅠBulletProjectile2>(), (int)(Projectile.damage * 0.4f), 0, Projectile.owner);
        }
    }

    internal class MixedToxinⅠBulletProjectile2 : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 40; // 碰撞箱宽
            Projectile.height = 40; // 碰撞箱长

            //贴图ai的应用，自动适应1号ai风格，吧贴图扭转
            Projectile.aiStyle = 0; // ai风格
            Projectile.friendly = true; // 伤害敌人？
            Projectile.hostile = false; // 伤害玩家？
            Projectile.timeLeft = 100; // 弹丸的有效时间（60=1秒，因此600是10秒）
            Projectile.alpha = 255;

            Projectile.light = 0.2f;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
        }

        //ai=0是子弹弹幕，1是弓箭弹幕
        public override void AI()
        {
            if (Projectile.timeLeft > 90 && Projectile.ai[0] == 0)
            {
                Projectile.friendly = false;
            }
            else
            {
                Projectile.friendly = true;
            }
            if (Projectile.ai[0] == 0)
            {
                Projectile.velocity *= 0.98f;
                if (!Main.dedServ)
                    for (int i = 0; i < 9; i++)
                    {
                        Dust dust = Dust.NewDustDirect(Projectile.Center + new Vector2(-4, -4), 1, 1, ModContent.DustType<Dusts.MixedToxinⅠDust>(), Main.rand.Next(-5, 5), Main.rand.Next(-5, 5), 230, Color.White, 1f);
                        dust.velocity *= 0.9f;
                    }
            }
            else
            {
                Projectile.velocity *= 0.965f;
                if (!Main.dedServ)
                    for (int i = 0; i < 9; i++)
                    {
                        Dust dust = Dust.NewDustDirect(Projectile.Center + new Vector2(-4, -4), 1, 1, ModContent.DustType<Dusts.MixedToxinⅠDust>(), Main.rand.Next(-5, 5), Main.rand.Next(-5, 5), 230, Color.White, 1f);
                        dust.velocity *= 0.4f * (1 - Projectile.velocity.Y / 8);
                    }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Buffs.FriendlyBuffs.MixedToxinⅠ>(), 300);
        }
    }
}
