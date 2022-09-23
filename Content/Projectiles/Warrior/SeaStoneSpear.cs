using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Projectiles.Warrior
{
    public class SeaStoneSpear : ModProjectile
    {
        //定义矛射弹的射程。这些是可重写的属性，以应对您希望使一个类继承自这个类。
        protected virtual float HoldoutRangeMin => 40f;

        protected virtual float HoldoutRangeMax => 130f;

        public override void SetStaticDefaults()
        {
            DisplayName.AddTranslation(7, "海石长矛");
        }

        public override void SetDefaults()
        {
            //克隆矛的默认值。矛的特定值设置为宽度、高度、aiStyle、是否友好、穿透数、撞墙情况、比例、隐藏、所有者击中检查和近战。
            //(width, height, aiStyle, friendly, penetrate, tileCollide, scale, hide, ownerHitCheck, and melee.)
            Projectile.CloneDefaults(ProjectileID.Spear);
        }

        public override bool PreAI()
        {
            //由于我们经常访问owner player实例，因此为此创建一个helper局部变量很有用
            Player player = Main.player[Projectile.owner];
            //定义投射物在帧中的持续时间
            int duration = player.itemAnimationMax;
            //更新玩家持有的投射物id
            player.heldProj = Projectile.whoAmI;

            // 必要时重置弹丸剩余时间
            if (Projectile.timeLeft > duration)
            {
                Projectile.timeLeft = duration;
            }

            ////在这个矛的实现中没有使用速度，但我们使用字段来存储矛的攻击方向。
            Projectile.velocity = Vector2.Normalize(Projectile.velocity);

            float halfDuration = duration * 0.5f;
            float progress;

            //此处“progress”设置为0.0到1.0之间的值，并在项目使用动画期间返回。
            if (Projectile.timeLeft < halfDuration)
            {
                progress = Projectile.timeLeft / halfDuration;
            }
            else
            {
                progress = (duration - Projectile.timeLeft) / halfDuration;
            }

            // Move the projectile from the HoldoutRangeMin to the HoldoutRangeMax and back, using SmoothStep for easing the movement
            //使用SmoothStep将射弹从HoldoutRangeMin移动到HoldoutRangeMax并向后移动
            Projectile.Center = player.MountedCenter + Vector2.SmoothStep(Projectile.velocity * HoldoutRangeMin, Projectile.velocity * HoldoutRangeMax, progress);

            // 对精灵图应用适当的旋转。
            if (Projectile.spriteDirection == -1)
            {
                // If sprite is facing left, rotate 45 degrees
                Projectile.rotation += MathHelper.ToRadians(45f);
            }
            else
            {
                // If sprite is facing right, rotate 135 degrees
                Projectile.rotation += MathHelper.ToRadians(135f);
            }

            //避免在专用服务器上产生灰尘
            // Avoid spawning dusts on dedicated servers
            if (!Main.dedServ)
            {
                // These dusts are added later, for the 'ExampleMod' effect
                //if (Main.rand.NextBool(3))
                //{
                //	Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MetalSpikesDust>(), Projectile.velocity.X * 2f, Projectile.velocity.Y * 2f, Alpha: 128, Scale: 1.2f);
                //}

                if (Main.rand.NextBool(5))
                {
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 16, 0, 0, Alpha: 128, new Color(8, 72, 121), Scale: 0.8f);
                }
            }

            return false; // Don't execute vanilla AI.
        }
    }
}
