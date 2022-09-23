using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Projectiles.Master
{
    public class ColorfulGemProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            DrawOffsetX = -3;
            DrawOriginOffsetY = -3;

            Projectile.friendly = true; // 伤害敌人？
            Projectile.hostile = false; // 伤害玩家？
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 60 * 2;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true; // 弹丸会与瓷砖碰撞吗？
            Projectile.extraUpdates = 0;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            //Projectile.alpha = 255;
            //弹药大小
            //Projectile.scale = 1f;

        }

        public override void AI()
        {
            #region 绘制螺旋线
            Projectile.rotation += 0.2f * Projectile.direction;
            int time = (int)(110 * Math.Pow(Math.Abs(Projectile.ai[0]), 0.5f));
            if (Projectile.timeLeft <= time)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(Math.Pow((float)(time - Projectile.timeLeft) / time, 2.5f) * Math.PI * Projectile.ai[0]);
            }
            #endregion

            if (Projectile.timeLeft > 20)
            {
                Dust dust = Dust.NewDustDirect(new Vector2(Projectile.Center.X - 4 - 10 / 4, Projectile.Center.Y - 4 - 10 / 4), 10, 10, DustID.PortalBolt/*263*/, 0, 0, 0, new Color(255, 255, 255), 1.2f);
                dust.noGravity = true;
                dust.velocity *= 0;
                dust.position = Projectile.Center;
            }
            if (Projectile.timeLeft < 20)
            {
                Projectile.velocity *= 0.9f;
            }
            Visuals();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //如果与瓷砖碰撞，则减少穿透。
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

                // 如果弹丸击中瓷砖的左侧或右侧，则反转并调整X速度
                if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                {
                    Projectile.velocity.X = -1 * oldVelocity.X;

                }
                // 如果弹丸击中瓷砖的顶部或底部，则反转并调整Y速度
                if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                {
                    Projectile.velocity.Y = -1 * oldVelocity.Y;
                }

                for (int i = 0; i < 10; i++)
                {
                    Dust dust = Dust.NewDustDirect(new Vector2(Projectile.Center.X - 4 - 10 / 4, Projectile.Center.Y - 4 - 10 / 4), 10, 10, DustID.PortalBolt, 0, 0, 0, new Color(255, 255, 255) * 0.8f, 1.5f);
                    dust.noGravity = true;
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
            Dust dust;
            for (int i = 0; i < 18; i++)
            {
                if (i / 6 == 0)
                    dust = Dust.NewDustDirect(new Vector2(Projectile.Center.X - 4 - 20 / 4, Projectile.Center.Y - 4 - 20 / 4), 20, 20, DustID.OrangeTorch, 0, 0, 0, new Color(255, 255, 0), 3.5f);
                else if (i / 6 == 1)
                    dust = Dust.NewDustDirect(new Vector2(Projectile.Center.X - 4 - 20 / 4, Projectile.Center.Y - 4 - 20 / 4), 20, 20, DustID.UnusedWhiteBluePurple, 0, 0, 0, default, 3.4f);
                else //if (i / 6 == 2)
                    dust = Dust.NewDustDirect(new Vector2(Projectile.Center.X - 4 - 20 / 4, Projectile.Center.Y - 4 - 20 / 4), 20, 20, DustID.LifeDrain, 0, 0, 0, default, 2.5f);
                dust.noGravity = true;
            }



            int count = 0;
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.friendly && npc.CanBeChasedBy())
                {
                    Vector2 distance = npc.Center - Projectile.Center;
                    if (distance.Length() < 16 * 50)
                    {
                        if (++count >= 3)
                        {
                            break;
                        }
                        Vector2 realToNPC = AnticipatoryShooting1(Projectile.Center, npc, 20, 1);
                        #region
                        int index = 0;
                        switch (Main.rand.Next(7) + 1)
                        {
                            case 1:
                                index = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, realToNPC, ProjectileID.AmethystBolt, (int)(Projectile.damage * 0.75f), 1, Projectile.owner);
                                break;
                            case 2:
                                index = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, realToNPC, ProjectileID.TopazBolt, (int)(Projectile.damage * 0.75), 1, Projectile.owner);
                                break;
                            case 3:
                                index = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, realToNPC, ProjectileID.SapphireBolt, (int)(Projectile.damage * 0.75), 1, Projectile.owner);
                                break;
                            case 4:
                                index = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, realToNPC, ProjectileID.EmeraldBolt, (int)(Projectile.damage * 0.75), 1, Projectile.owner);
                                break;
                            case 5:
                                index = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, realToNPC, ProjectileID.RubyBolt, (int)(Projectile.damage * 0.75), 1, Projectile.owner);
                                break;
                            case 6:
                                index = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, realToNPC, ProjectileID.DiamondBolt, (int)(Projectile.damage * 0.75), 1, Projectile.owner);
                                break;
                            case 7:
                                index = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, realToNPC, ProjectileID.AmberBolt, (int)(Projectile.damage * 0.75), 1, Projectile.owner);
                                break;
                            default: break;
                        }
                        Main.projectile[index].penetrate = 1;
                        #endregion
                    }
                }
            }
        }

        private void Visuals()
        {
            //这是一个简单的“从上到下遍历所有帧”动画
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.8f);
        }

        /// <summary>
        /// 返回对移动目标预判的射弹方向，针对匀速的预判
        /// </summary>
        /// <param name="pos">发射射弹的位置</param>
        /// <param name="targetNPC">目标npc</param>
        /// <param name="speed">射弹的速度</param>
        /// <param name="extraUpdates">射弹的额外更新率</param>
        /// <returns></returns>
        public Vector2 AnticipatoryShooting1(Vector2 pos, NPC targetNPC, float speed, int extraUpdates = 0)
        {
            Vector2 plrToNPC = targetNPC.Center - pos;
            float offset = plrToNPC.ToRotation();
            // 这就是我们推出来的公式
            float G = (plrToNPC.X * targetNPC.velocity.Y - plrToNPC.Y * targetNPC.velocity.X) / (speed * (extraUpdates + 1)) / plrToNPC.Length();
            if (G > 1 || G < -1)
            {
                //Main.NewText("无法预判！");
                return plrToNPC;
            }
            float realr = (float)(offset + Math.Asin(G));
            return realr.ToRotationVector2() * speed;
        }
    }
}
