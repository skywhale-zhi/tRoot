using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Projectiles.Shooter
{
    public class IceFireArrow : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 6; // 碰撞箱宽
            Projectile.height = 6; // 碰撞箱长
            DrawOffsetX = -4;
            DrawOriginOffsetY = 0;

            //贴图ai的应用，自动适应1号ai风格，吧贴图扭转
            Projectile.aiStyle = 1; // ai风格
            Projectile.friendly = true; // 伤害敌人？
            Projectile.hostile = false; // 伤害玩家？
            Projectile.DamageType = DamageClass.Ranged; // 炮弹是由远程武器发射的
            Projectile.penetrate = 1; // 射弹能穿透多少怪物。（下面的OnTileCollide也会减少穿透反弹）
            Projectile.timeLeft = 600; // 弹丸的有效时间（60=1秒，因此600是10秒）
                                       //Projectile.alpha = 255; // 弹丸的透明度，255表示完全透明。（aiStyle 1快速淡入投射物）如果您没有使用淡入的aiStyle，请确保删除此项。你会想知道为什么你的射弹是看不见的。
            Projectile.light = 0f; // 弹丸周围发射多少光
            Projectile.ignoreWater = false; // 弹丸的速度会受到水的影响吗？
            Projectile.tileCollide = true; // 弹丸会与瓷砖碰撞吗？
                                           //默认为 0，表示没有倍率（对于大部分箭为0，子弹为1）。若为 1 则代表具有 ×2 倍率，2 则为 ×3，以此类推。
            Projectile.extraUpdates = 0;
            //弹药大小
            Projectile.scale = 1f;
            //弹药性质改为子弹，不会受重力影响等
            AIType = ProjectileID.WoodenArrowFriendly; // Act exactly like default Bullet
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
            if (Main.rand.NextBool(2))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0, 0, 0, default, 0.7f);
            }
            if (Main.rand.NextBool(2))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, 0, 0, 0, default, 0.7f);
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0, 0, 0, default, 1);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, 0, 0, 0, default, 1);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.NextBool(3))
                target.AddBuff(BuffID.OnFire, 180);
            if (Main.rand.NextBool(3))
                target.AddBuff(BuffID.Frostburn, 180);
        }
    }
}
