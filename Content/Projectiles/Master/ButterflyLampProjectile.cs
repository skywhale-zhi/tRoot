using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Projectiles.Master
{
    //摇蝶灯射弹
    /// <summary>
    /// 第一个射弹，螺旋射弹
    /// </summary>
    internal class ButterflyLampProjectile1 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // 设置此在其精灵图上的帧数
            Main.projFrames[Projectile.type] = 7;
            //拖尾幻影
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }


        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 28;
            Projectile.tileCollide = true;
            DrawOffsetX = 0;
            DrawOriginOffsetY = 0;
            DrawOriginOffsetX = -10;
            Projectile.scale *= Main.rand.NextFloat(0.2f, 1f);
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.timeLeft = 390;
            Projectile.penetrate = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
        }


        //ai0 引发大小差异 
        public override void AI()
        {
            #region 绘制螺旋线            
            double time = 390 * Math.Pow(Math.Abs(Projectile.ai[0]), 0.5f);
            if (Projectile.timeLeft <= time)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(Math.Pow((time - Projectile.timeLeft) / time, Math.Abs(Projectile.ai[0])) * Math.PI * 0.04 * Projectile.ai[0]);
            }
            #endregion
            Visuals();
        }


        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.NextBool(3))
                target.AddBuff(ModContent.BuffType<Buffs.SamadhiTrueFire>(), Main.rand.Next(360, 720));
        }


        private void Visuals()
        {
            //这是一个简单的“从上到下遍历所有帧”动画
            int frameSpeed = 1;
            Projectile.frameCounter++;

            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
            // 红光
            Lighting.AddLight(Projectile.Center, new Color(255, 0, 0).ToVector3() * 0.5f);
            //精灵图指向
            Projectile.spriteDirection = (int)(Projectile.velocity.X / Math.Abs(Projectile.velocity.X));
            Projectile.rotation = Projectile.velocity.RotatedBy(MathHelper.PiOver2).ToRotation();
        }


        public override void Load()
        {
            texture = ModContent.Request<Texture2D>("tRoot/Content/Projectiles/Master/ButterflyLampProjectile1", AssetRequestMode.ImmediateLoad).Value;
        }
        public override void Unload()
        {
            texture = null;
        }
        //残影和高亮的绘制
        //或者这么写 Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;，为什么这么些因为避免每帧request一下，浪费资源
        static Texture2D texture;
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            Rectangle sourceRectangle = new Rectangle(0, Projectile.frame * 28, 30, 28);
            
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin;
                //非高光需要吧rgb全都设置为0，高光则设置存在值即可
                Color color;
                if(k == 0)
                    color = Color.White;
                else 
                    color = new Color(new Vector4((Projectile.oldPos.Length - k) * 0.3f / Projectile.oldPos.Length));

                //淡出
                if(Projectile.timeLeft < 25)
                    color *= Projectile.timeLeft / 25f;

                Main.spriteBatch.Draw(
                    texture, 
                    drawPos, 
                    sourceRectangle, 
                    color,
                    Projectile.oldRot[k], 
                    drawOrigin, 
                    Projectile.scale - k / (float)Projectile.oldPos.Length / 2,
                    SpriteEffects.None, 
                    0f
                    );
            }
            return false;
        }
    }


    /// <summary>
    /// 第二个射弹，追踪射弹
    /// </summary>
    internal class ButterflyLampProjectile2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // 设置此在其精灵图上的帧数
            Main.projFrames[Projectile.type] = 7;
            //让邪教徒抵抗这种投射物，因为邪教徒对追踪投射物有减伤
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;

            //拖尾幻影
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }


        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 28;
            Projectile.tileCollide = true;
            DrawOffsetX = 0;
            DrawOriginOffsetY = 0;
            DrawOriginOffsetX = -10;
            Projectile.scale *= Main.rand.NextFloat(0.2f, 1f);
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.timeLeft = 360;
            Projectile.tileCollide = false;
            Projectile.ArmorPenetration = 10;
            Projectile.DamageType = DamageClass.Magic;
        }


        // ai 0 引发大小差异
        public override void AI()
        {
            if(Projectile.timeLeft >= 300)
            {
                Projectile.velocity *= 0.99f;
            }
            else
            {
                NPC targeNpc = tRoot.getTargetNPC(Projectile.Center, 1700);
                if (targeNpc != null)
                {
                    float k = (300 - Projectile.timeLeft) > 60 ? 1 : (300f - Projectile.timeLeft) / 60f;

                    Vector2 v;
                    if (Vector2.DistanceSquared(Projectile.Center, targeNpc.Center) >= 40000)
                        v = ((targeNpc.Center - Projectile.Center).SafeNormalize(Vector2.Zero) + Projectile.velocity.SafeNormalize(Vector2.Zero) * 8) / 9;
                    else if(Vector2.DistanceSquared(Projectile.Center, targeNpc.Center) >= 10000)
                        v = ((targeNpc.Center - Projectile.Center).SafeNormalize(Vector2.Zero) + Projectile.velocity.SafeNormalize(Vector2.Zero)) * 2 / 3;
                    else
                        v = ((targeNpc.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 2 + Projectile.velocity.SafeNormalize(Vector2.Zero)) / 3;

                    Projectile.velocity = v * k * 30;
                }
                //如果未找到敌人，速度减慢，加速死亡
                if(targeNpc == null)
                {
                    Projectile.velocity *= 0.97f;
                    Projectile.timeLeft--;
                }
            }
            Visuals();
        }


        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.NextBool(3))
                target.AddBuff(ModContent.BuffType<Buffs.SamadhiTrueFire>(), Main.rand.Next(360, 720));
        }


        private void Visuals()
        {
            //这是一个简单的“从上到下遍历所有帧”动画
            int frameSpeed = 1;
            Projectile.frameCounter++;

            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
            // 红光
            Lighting.AddLight(Projectile.Center, new Color(255, 100, 0).ToVector3() * 0.5f);
            //精灵图指向
            Projectile.spriteDirection = (int)(Projectile.velocity.X / Math.Abs(Projectile.velocity.X));
            Projectile.rotation = Projectile.velocity.RotatedBy(MathHelper.PiOver2).ToRotation();
        }


        public override void Load()
        {
            texture = ModContent.Request<Texture2D>("tRoot/Content/Projectiles/Master/ButterflyLampProjectile2", AssetRequestMode.ImmediateLoad).Value;
        }
        public override void Unload()
        {
            texture = null;
        }
        static Texture2D texture;
        //残影和高亮的绘制
        //或者这么写 Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;，为什么这么些因为避免每帧request一下，浪费资源
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            Rectangle sourceRectangle = new Rectangle(0, Projectile.frame * 28, 30, 28);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin;
                //非高光需要吧rgb全都设置为0，高光则设置存在值即可
                Color color;
                if (k == 0)
                    color = Color.White;
                else
                    color = new Color(new Vector4((Projectile.oldPos.Length - k) * 0.6f / Projectile.oldPos.Length));

                //淡出
                if (Projectile.timeLeft < 60)
                    color *= Projectile.timeLeft / 60f;

                Main.spriteBatch.Draw(
                    texture,
                    drawPos,
                    sourceRectangle,
                    color,
                    Projectile.oldRot[k],
                    drawOrigin,
                    Projectile.scale - k / (float)Projectile.oldPos.Length / 2,
                    SpriteEffects.None,
                    0f
                    );
            }
            return false;
        }
    }



    /// <summary>
    /// 第三个射弹，环绕射弹
    /// </summary>
    internal class ButterflyLampProjectile3 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // 设置此在其精灵图上的帧数
            Main.projFrames[Projectile.type] = 7;

            //拖尾幻影
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }


        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            DrawOffsetX = 0;
            DrawOriginOffsetY = 0;
            DrawOriginOffsetX = -10;
            Projectile.scale *= Main.rand.NextFloat(0.2f, 1f);
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.timeLeft = Main.rand.Next(150, 250);
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
        }


        //ai0 引发旋转速度的差异，ai1 引发旋转半径的差异
        public override void AI()
        {
            if (Projectile.timeLeft < 250)
            {
                Vector2 tar = Vector2.UnitY.RotatedBy(Projectile.timeLeft * Projectile.ai[0] * Projectile.ai[0] * 0.2f / MathHelper.TwoPi + MathHelper.TwoPi * Projectile.ai[0]) * 60 * Projectile.ai[1] + Main.player[Projectile.owner].Center;
                Projectile.velocity = (tar - Projectile.Center) * 0.4f;
            }
            Visuals();
        }


        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.CanBeChasedBy())
            {
                Main.player[Projectile.owner].statLife += 1;
                Main.player[Projectile.owner].HealEffect(1);
            }
            if (Main.rand.NextBool(3))
                target.AddBuff(ModContent.BuffType<Buffs.SamadhiTrueFire>(), Main.rand.Next(360, 720));
        }


        private void Visuals()
        {
            //这是一个简单的“从上到下遍历所有帧”动画
            int frameSpeed = 1;
            Projectile.frameCounter++;

            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
            // 红光
            Lighting.AddLight(Projectile.Center, new Color(255, 0, 100).ToVector3() * 0.5f);
            //精灵图指向
            Projectile.spriteDirection = (int)(Projectile.velocity.X / Math.Abs(Projectile.velocity.X));
            Projectile.rotation = Projectile.velocity.RotatedBy(MathHelper.PiOver2).ToRotation();
        }


        public override void Load()
        {
            texture = ModContent.Request<Texture2D>("tRoot/Content/Projectiles/Master/ButterflyLampProjectile3", AssetRequestMode.ImmediateLoad).Value;
        }
        public override void Unload()
        {
            texture = null;
        }
        static Texture2D texture;
        //残影和高亮的绘制
        //或者这么写 Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;，为什么这么些因为避免每帧request一下，浪费资源
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            Rectangle sourceRectangle = new Rectangle(0, Projectile.frame * 28, 30, 28);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin;
                //非高光需要吧rgb全都设置为0，高光则设置存在值即可
                Color color;
                if (k == 0)
                    color = Color.White;
                else
                    color = new Color(new Vector4((Projectile.oldPos.Length - k) * 0.8f / Projectile.oldPos.Length));

                //淡出
                if (Projectile.timeLeft < 25)
                    color *= Projectile.timeLeft / 25f;

                Main.spriteBatch.Draw(
                    texture,
                    drawPos,
                    sourceRectangle,
                    color,
                    Projectile.oldRot[k],
                    drawOrigin,
                    Projectile.scale - k / (float)Projectile.oldPos.Length / 2,
                    SpriteEffects.None,
                    0f
                    );
            }
            return false;
        }
    }
}
