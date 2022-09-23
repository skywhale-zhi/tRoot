using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Pets.MiniFlower
{
    internal class MiniFlowerProjectile : ModProjectile
    {
        //public ref float AlphaForVisuals => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            Main.projPet[Projectile.type] = true;
            //发光宠物加上这个，其他则不加
            ProjectileID.Sets.LightPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 0;
            Projectile.height = 0;
            Projectile.penetrate = -1;
            //将射弹与玩家同步，宠物必备
            Projectile.netImportant = true;
            Projectile.timeLeft *= 5;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.manualDirectionChange = true;

            DrawOffsetX = -16;
            DrawOriginOffsetY = -16;
            Projectile.aiStyle = -1; //使用自定义AI
        }


        //允许您确定绘制此弹丸的颜色和透明度。返回 null 以使用默认颜色（通常为浅色和浅黄色）。默认情况下返回 null。
        //public override Color? GetAlpha(Color lightColor)
        //{
        //	return Color.White * AlphaForVisuals;
        //}

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            //检查玩家是否还活着
            CheckActive(player);
            //移动处理
            Movement(player);

            if (!Main.dedServ)
            {
                float k = 0.0035f;
                Lighting.AddLight(Projectile.Center, 255 * k, 78 * k, 255 * k);
            }
        }

        private void CheckActive(Player player)
        {   //只要玩家没有死亡并且拥有宠物增益，就不要让射弹消失
            if (!player.dead && player.HasBuff(ModContent.BuffType<MiniFlowerBuff>()))
            {
                Projectile.timeLeft = 5;
            }
        }

        private void Movement(Player player)
        {
            //离目标位置的最小距离，接近这个值时进行处理，防止移动鬼畜等
            float velDistanceChange = 2f;

            // Calculates the desired resting position, aswell as some vectors used in velocity/rotation calculations
            //计算所需的静止位置以及速度/旋转计算中使用的一些矢量
            int dir = player.direction;
            Projectile.direction = Projectile.spriteDirection = dir;

            //目标位置，相对于玩家而言
            Vector2 desiredCenterRelative = new Vector2(dir * 1, -50f);

            //上限缓慢浮动
            desiredCenterRelative.Y += (float)Math.Sin(Main.GameUpdateCount / 120f * MathHelper.TwoPi) * 5;

            //目标位置，相对于世界坐标
            Vector2 desiredCenter = player.MountedCenter + desiredCenterRelative;
            Vector2 betweenDirection = desiredCenter - Projectile.Center;
            float betweenSQ = betweenDirection.LengthSquared(); //据说平方比直接开方求间距速度更快


            //如果距离玩家太远，或接近所需位置，则直接传送到所需位置
            if (betweenSQ > 1000f * 1000f || betweenSQ < velDistanceChange * velDistanceChange)
            {
                Projectile.Center = desiredCenter;
                Projectile.velocity = Vector2.Zero;
            }


            //离玩家的距离越大，速度越大，体现出追随效果，后面数字乘数越大，追随力度越强，越小越弱
            if (betweenDirection != Vector2.Zero)
            {
                //乘数不能过大否则会陷入movefast情况
                Projectile.velocity = betweenDirection * 0.1f * 2f;
            }

            bool movesFast = Projectile.velocity.LengthSquared() > 8f * 8f;

            //如果玩家移动速度非常快
            if (movesFast)
            {
                Projectile.rotation += 0.2f * Projectile.direction;
                Dust dust = Dust.NewDustDirect(Projectile.position + new Vector2(-16, -16), 30, 30, DustID.PinkFairy, 0, 0, 0, Color.White, 0.5f);
                dust.velocity *= 0.8f;
                float light = 0.0015f * dust.scale;
                Lighting.AddLight(dust.position, 255 * light, 78 * light, 255 * light);
            }
            else
            {
                Projectile.rotation += 0.05f * Projectile.direction;
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position + new Vector2(-16, -16), 30, 30, DustID.PinkFairy, 0, 0, 0, Color.White, 1f);
                dust.noGravity = true;
            }
        }
    }
}
