using tRoot.Content.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace tRoot.Content.Projectiles.Warrior
{
    public class BanDianStarSwordProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // 几帧
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            //碰撞箱体积
            Projectile.width = 8;
            Projectile.height = 8;
            DrawOffsetX = -16;
            DrawOriginOffsetY = -16;
            //是否伤害怪物
            Projectile.friendly = true;
            //是否伤害npc和玩家
            //Projectile.hostile = true;
            //伤害类型
            Projectile.DamageType = DamageClass.Melee;
            //是否忽略水的减速
            Projectile.ignoreWater = true;
            //是否碰撞实心块
            Projectile.tileCollide = true;
            //最大穿透
            Projectile.penetrate = 2;
            //绘制这个弹丸有多透明。0 到 255。255 是完全透明的。
            Projectile.alpha = 255;
            //击退力
            Projectile.knockBack = 0;
            //AIType = ProjectileID.Starfury;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;

        }

        //允许您确定绘制投射的颜色和透明度
        //返回null以使用默认颜色（通常为浅色和浅黄色）
        //默认情况下返回null。
        public override Color? GetAlpha(Color lightColor)
        {
            //return Color.White;
            return new Color(225, 255, 255, 100) * Projectile.Opacity;
        }

        //撞击物体时收到的效果
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < 15; i++)
                Dust.NewDust(Projectile.position, 40, 40, DustID.MagicMirror, 0, 0, 150, default, 1);
            SoundEngine.PlaySound(SoundID.Item50, Projectile.position);
            Projectile.Kill();
            return false;
        }

        public override void AI()
        {
            //所有射弹都有计时器，有助于延迟某些事件
            //Projectile.ai[0]，ai[1]-在客户端和服务器上自动同步的计时器
            //Projectile.localAI[0]-仅在客户端上
            //在此示例中，计时器用于控制投射物的淡入/淡出和消隐
            Projectile.ai[0] += 1f;

            //淡入淡出
            FadeInAndOut();

            // Slow down
            //Projectile.velocity *= 0.99f;


            //循环浏览4个动画帧，每个帧上花费5个刻度
            //Projectile.frame - 当前帧的索引
            //if (++Projectile.frameCounter >= 30)
            //{
            //	Projectile.frameCounter = 0;
            //	// Or more compactly Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
            //	if (++Projectile.frame >= Main.projFrames[Projectile.type])
            //		Projectile.frame = 0;
            //}


            // 弹幕10秒后消失
            // 或者你也可以使用 Projectile.timeLeft = 60f 在 SetDefaults() 里达到相同的目的
            if (Projectile.ai[0] >= 300f)
            {
                for (int i = 0; i < 15; i++)
                    Dust.NewDust(Projectile.position, 40, 40, DustID.MagicMirror, 0, 0, 150, default, 1);
                Projectile.Kill();
            }


            //将“方向”（direction）和“精灵方向”（SpritedDirection）都设置为1或-1（分别为右侧和左侧）
            //Projectile.direction 能自动正确设置方向在 Projectile.Update，但我们需要在此处设置，否则纹理将在第一帧上绘制错误。
            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
            //Projectile.rotation = Projectile.velocity.ToRotation();
            //由于精灵图只有有一个方向，我们需要调整旋转以补偿绘图翻转
            //if (Projectile.spriteDirection == -1)
            //{
            //    Projectile.rotation += MathHelper.Pi;
            //    //对于垂直的精灵图，请使用 MathHelper.PiOver2
            //}
            Projectile.rotation += MathHelper.Pi * 0.08f;

            //尾焰尘
            Dust.NewDust(Projectile.position + new Vector2(Projectile.width / 2, Projectile.height / 2), 1, 1, 15, 0, 0, 150, default, 0.5f);
        }


        // 许多射弹会逐渐消失，以便在创造时不会与它们出现的位置重叠
        public void FadeInAndOut()
        {
            // 小于560滴答时逐渐降低透明度到能看到的范围
            if (Projectile.ai[0] <= 560f)
            {
                // Fade in
                Projectile.alpha -= 25;
                // 为了保持一定的透明性，这里降低到100就不再降低了
                if (Projectile.alpha < 100)
                    Projectile.alpha = 100;
                return;
            }

            //大于560滴答时消失，增加透明度到完全看不见为止
            Projectile.alpha += 25;
            // 透明度上限为255
            if (Projectile.alpha > 255)
                Projectile.alpha = 255;
        }
    }
}
