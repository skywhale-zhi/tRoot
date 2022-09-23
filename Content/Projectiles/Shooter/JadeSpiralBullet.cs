using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Projectiles.Shooter
{
    internal class JadeSpiralBullet : ModProjectile
    {
        private int timer = 0;
        public int Timer
        {
            get { return timer; }
            set { timer = value; } 
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }

        public override void SetDefaults()
        {
            Projectile.width = 2; // 碰撞箱宽
            Projectile.height = 2; // 碰撞箱长

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 45;

            //贴图ai的应用，自动适应1号ai风格，吧贴图扭转
            //Projectile.aiStyle = 1; // ai风格
            Projectile.friendly = true; // 伤害敌人？
            Projectile.hostile = false; // 伤害玩家？
            Projectile.DamageType = DamageClass.Ranged; // 炮弹是由远程武器发射的
            Projectile.penetrate = 2; // 射弹能穿透多少怪物。（下面的OnTileCollide也会减少穿透反弹）
            Projectile.timeLeft = 600; // 弹丸的有效时间（60=1秒，因此600是10秒）
            Projectile.alpha = 255; // 弹丸的透明度，255表示完全透明。（aiStyle 1快速淡入投射物）如果您没有使用淡入的aiStyle，请确保删除此项。你会想知道为什么你的射弹是看不见的。
            Projectile.ignoreWater = false; // 弹丸的速度忽略水的影响吗？
            Projectile.tileCollide = true; // 弹丸会与瓷砖碰撞吗？
                                           // Set to above 0 if you want the projectile to update multiple time in a frame
                                           //默认为 0，表示没有倍率（对于大部分箭为0，子弹为1）。若为 1 则代表具有 ×2 倍率，2 则为 ×3，以此类推。
            Projectile.extraUpdates = 1;
            //弹药大小
            Projectile.scale = 1.5f;
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
            Lighting.AddLight(Projectile.Center, new Vector3(148, 252, 249) / 255 / 2);
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
            // 淡出
            //Projectile.alpha += 25;
            // 防止过界
            //if (Projectile.alpha > 255)
            //   Projectile.alpha = 255;
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //如果与瓷砖碰撞，则减少穿透。
            //因此，弹丸最多可以反射5次
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                //撞击，击中贴图引发的贴图效果，如方块灰尘等
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

                //Main.NewText($"Projectile.velocity.X: {Projectile.velocity.X}");
                //Main.NewText($"oldVeloccity.X: {oldVelocity.X}");
                //Main.NewText($"Projectile.velocity.Y: {Projectile.velocity.Y}");
                //Main.NewText($"oldVeloccity.Y: {oldVelocity.Y}");


                // 如果弹丸击中瓷砖的左侧或右侧，则反转并调整X速度
                if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                {
                    Projectile.velocity.X = -1 * (oldVelocity.X * 0.5f + Main.rand.NextFloat(oldVelocity.X) * 0.5f);

                }

                // 如果弹丸击中瓷砖的顶部或底部，则反转并调整Y速度
                if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                {
                    Projectile.velocity.Y = -1 * (oldVelocity.Y * 0.5f + Main.rand.NextFloat(oldVelocity.Y) * 0.5f);
                }
            }

            return false;
        }


        public override void Kill(int timeLeft)
        {
            //EntitySource_ItemUse_WithAmmo source;
            //此代码和上面在OnTileCollide中的类似代码会从与之碰撞的瓷砖中产生灰尘。SoundID。第10项是你听到的反弹声。
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            //生成碎金
            float speedX = Main.rand.NextFloat(-2f, 2f);
            float speedY = Main.rand.NextFloat(-2f, 2f);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y, speedX, speedY, ModContent.ProjectileType<JadeSpiralBulletFragment>(), (int)(Projectile.damage * 0.5), 0f, Projectile.owner);
        }
    }



    //螺玉弹的碎屑
    internal class JadeSpiralBulletFragment : ModProjectile
    {
        //螺玉碎屑
        public override void SetDefaults()
        {
            Projectile.width = 6; // 碰撞箱宽
            Projectile.height = 4; // 碰撞箱长
            DrawOffsetX = -2;
            DrawOriginOffsetY = -2;
            DrawOriginOffsetX = 0;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;

            //贴图ai的应用，自动适应1号ai风格，吧贴图扭转
            Projectile.aiStyle = 0; // ai风格
            Projectile.friendly = true; // 伤害敌人？
            Projectile.hostile = false; // 伤害玩家？
                                        //Projectile.DamageType = DamageClass.Ranged; // 炮弹是由远程武器发射的
            Projectile.penetrate = 1; // 射弹能穿透多少怪物。（下面的OnTileCollide也会减少穿透反弹）
            Projectile.timeLeft = 100; // 弹丸的有效时间（60=1秒，因此600是10秒）
                                      //Projectile.alpha = 255; // 弹丸的透明度，255表示完全透明。（aiStyle 1快速淡入投射物）如果您没有使用淡入的aiStyle，请确保删除此项。你会想知道为什么你的射弹是看不见的。
            Projectile.light = 0.2f; // 弹丸周围发射多少光
            Projectile.ignoreWater = false; // 弹丸的速度会受到水的影响吗？
            Projectile.tileCollide = true; // 弹丸会与瓷砖碰撞吗？,false是穿过去了
                                           // Set to above 0 if you want the projectile to update multiple time in a frame
                                           //如果希望投射物在一帧中多次更新，请将其设置为0以上
            Projectile.extraUpdates = 1;
            //弹药大小
            Projectile.scale = Main.rand.NextFloat(0.7f) + 1f;
            //弹药性质改为子弹，不会受重力影响等
            AIType = ProjectileID.Bullet; // Act exactly like default Bullet
        }


        public override void AI()
        {
            Projectile.rotation += 0.4f * Projectile.direction;
            Projectile.velocity *= 0.98f;
            Projectile.scale *= 0.99f;
        }

        //撞墙时反弹
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
            {
                Projectile.velocity.X = -1 * oldVelocity.X;
            }
            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
            {
                Projectile.velocity.Y = -1 * oldVelocity.Y;
            }
            return false;
        }
    }
}
