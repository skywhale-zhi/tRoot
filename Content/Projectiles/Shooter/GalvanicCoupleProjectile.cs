using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace tRoot.Content.Projectiles.Shooter
{
    //电离球弹1
    internal class GalvanicCoupleProjectile1 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            DrawOffsetX = 10;
            DrawOriginOffsetY = 12;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.scale *= 1.5f;
        }

        //ai0 标记生成子射弹的索引，ai1 配对的射弹的index
        public override void AI()
        {
            if (Projectile.ai[0] == -1)
            {
                Projectile.ai[0] = Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                    Projectile.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<GalvanicCoupleProjectile2>(),
                    Projectile.damage / 2,
                    Projectile.knockBack,
                    Projectile.owner,
                    Projectile.whoAmI, Projectile.ai[1]);
                Projectile.netUpdate = true;
            }
            Visuals();
        }

        public override void Kill(int timeLeft)
        {
            if (Projectile.ai[0] >= 0)
            {
                Main.projectile[(int)Projectile.ai[0]].Kill();
            }
        }


        public override void Load()
        {
            texture = ModContent.Request<Texture2D>("tRoot/Content/Projectiles/Shooter/GalvanicCoupleProjectile1", AssetRequestMode.ImmediateLoad).Value;
        }
        public override void Unload()
        {
            texture = null;
        }
        private static Texture2D texture;
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            Rectangle sourceRectangle = new Rectangle(0, Projectile.frame * 44, 38, 44);

            Vector2 drawPos = Projectile.position - Main.screenPosition + drawOrigin + new Vector2(DrawOffsetX, DrawOriginOffsetY);
            //非高光需要吧rgb全都设置为0，高光则设置存在值即可
            Color color = Color.White;

            //淡入和淡出
            if (Projectile.timeLeft < 30)
                color *= Projectile.timeLeft / 30f;
            if (Projectile.timeLeft > 200)
                color *= (240f - Projectile.timeLeft) / 40f;

            Main.spriteBatch.Draw(
                texture,
                drawPos,
                sourceRectangle,
                color,
                Projectile.rotation,
                drawOrigin,
                Projectile.scale,
                SpriteEffects.None,
                0f
                );
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if(Projectile.damage > 0)
                Projectile.damage -= 10;
        }

        private void Visuals()
        {
            //这是一个简单的“从上到下遍历所有帧”动画
            int frameSpeed = 4;
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
        }
    }


    //电离球射弹2
    internal class GalvanicCoupleProjectile2 : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 50;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
        }


        //ai0 生成者， ai1 配对者
        public override void AI()
        {
            if (Projectile.alpha != 255)
                Projectile.alpha = 255;
            Projectile.timeLeft = 180;
            if (Vector2.DistanceSquared(Projectile.Center, Main.projectile[(int)Projectile.ai[1]].Center) > 100)
            {
                Projectile.Center = Projectile.Center.MoveTowards(Main.projectile[(int)Projectile.ai[1]].Center, 30);
            }
            else
            {
                Projectile.Center = Main.projectile[(int)Projectile.ai[0]].Center;
            }

            //如果伤害衰减到没有，加快射弹死亡
            if(Projectile.damage <= 1 && Main.projectile[(int)Projectile.ai[0]].timeLeft > 30 && Main.projectile[(int)Projectile.ai[1]].timeLeft > 30)
            {
                Main.projectile[(int)Projectile.ai[0]].timeLeft = 30;
                Main.projectile[(int)Projectile.ai[1]].timeLeft = 30;
            }
        }


        public override void Load()
        {
            chainTexture = ModContent.Request<Texture2D>("tRoot/Content/Projectiles/Shooter/GalvanicCoupleProjectile2");
        }
        public override void Unload()
        {
            chainTexture = null;
        }
        private static Asset<Texture2D> chainTexture;
        public override bool PreDraw(ref Color lightColor)
        {
            //使用此选项调整每个链条绘制时的间隔，如果是负值会更密
            float chainHeightAdjustment = 0f;
            Vector2 chainOrigin = chainTexture.Size() / 2f;
            //链条绘制的初始位置和结束位置
            Vector2 DrawFirstPos = Main.projectile[(int)Projectile.ai[0]].Center;
            Vector2 DrawEndPos = Main.projectile[(int)Projectile.ai[1]].Center;
            Vector2 DrawMiddlePos = (DrawFirstPos + DrawEndPos) / 2;
            //获得从两个端点之间指向结束位置的向量，MovetoWards得到的是点坐标而不是向量，这里要注意
            Vector2 vectorFromMidToLast = DrawEndPos.MoveTowards(DrawMiddlePos, 4f) - DrawMiddlePos;
            Vector2 vectorFromMidToFirst = DrawFirstPos.MoveTowards(DrawMiddlePos, 4f) - DrawMiddlePos;
            //将上个向量单位化
            Vector2 unitFromMidToLast = vectorFromMidToLast.SafeNormalize(Vector2.Zero);
            Vector2 unitFromMidToFirst = vectorFromMidToFirst.SafeNormalize(Vector2.Zero);
            //链条长度
            float chainSegmentLength = chainTexture.Height() + chainHeightAdjustment;
            if (chainSegmentLength == 0)
                chainSegmentLength = 10; //加载链纹理时，如果高度为0，这将导致下面的while无限循环，有点类似分母为0的情况，必须排除
            //链条偏转度
            float chainRotation = unitFromMidToLast.ToRotation() + MathHelper.PiOver2;

            //淡入和淡出
            Color color = Color.White;
            if (Main.projectile[(int)Projectile.ai[0]].timeLeft < 30)
                color *= Main.projectile[(int)Projectile.ai[0]].timeLeft / 30f;
            if (Main.projectile[(int)Projectile.ai[0]].timeLeft > 200)
                color *= (240f - Main.projectile[(int)Projectile.ai[0]].timeLeft) / 40f;


            //绘制从中点指向下部的链条部分
            //需要去画的链条长度
            float chainLengthRemainingToDraw = vectorFromMidToLast.Length() - chainSegmentLength;
            // 该while循环将链纹理初始位置遍历至结束位置，循环以沿路径绘制链纹理
            while (chainLengthRemainingToDraw > 0f)
            {
                //在这里，我们在坐标处绘制链纹理
                Main.spriteBatch.Draw(chainTexture.Value, DrawMiddlePos - Main.screenPosition, null, color, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                //chainDrawPosition沿着向量前进，通过chainSegmentLength返回给player
                DrawMiddlePos += unitFromMidToLast * chainSegmentLength;
                chainLengthRemainingToDraw -= chainSegmentLength;
            }

            //绘制从中点指向上部的链条部分
            DrawMiddlePos = (DrawFirstPos + DrawEndPos) / 2;
            //需要去画的链条长度
            chainLengthRemainingToDraw = vectorFromMidToFirst.Length() - chainSegmentLength / 2;
            // 该while循环将链纹理初始位置遍历至结束位置，循环以沿路径绘制链纹理
            while (chainLengthRemainingToDraw > 0f)
            {
                //在这里，我们在坐标处绘制链纹理
                Main.spriteBatch.Draw(chainTexture.Value, DrawMiddlePos - Main.screenPosition, null, color, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                //chainDrawPosition沿着向量前进，通过chainSegmentLength返回给player
                DrawMiddlePos += unitFromMidToFirst * chainSegmentLength;
                chainLengthRemainingToDraw -= chainSegmentLength;
            }
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //增对长虫类敌人如毁灭者进行伤害削弱
            if (target.realLife != -1)
            {
                Projectile.extraUpdates = 2;
                Projectile.damage = (int)(Projectile.damage * 0.99f);
            }
            else
            {
                Projectile.extraUpdates = 7;
            }
        }
    }
}