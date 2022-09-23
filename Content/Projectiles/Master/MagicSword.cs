using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Projectiles.Master
{
    public class MagicSword : ModProjectile
    {
        private enum AttackState : int
        {
            Ready,
            Dash,
            Search
        };
        private AttackState State
        {
            get { return (AttackState)(int)Projectile.ai[0]; }
            set { Projectile.ai[0] = (int)value; }
        }
        private int Timer
        {
            get { return (int)Projectile.ai[1]; }
            set { Projectile.ai[1] = value; }
        }

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            DrawOffsetX = -8;
            DrawOriginOffsetY = -15;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 6;
            Projectile.timeLeft = 60 * 10;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 0;
            Projectile.scale = 1.2f;
            Timer = 0;

        }

        public override void AI()
        {
            if (Projectile.timeLeft > 10)
            {
                Dust dust = Dust.NewDustDirect(new Vector2(Projectile.Center.X - 4 - 10 / 4, Projectile.Center.Y - 4 - 10 / 4), 10, 10, DustID.PortalBolt, 0, 0, 100, new Color(255, 255, 0), 1f);
                dust.noGravity = true;
                dust.velocity *= 0.5f;
                dust.position = Projectile.Center;
                float min_distance = 380f;

                NPC targeNpc = null;
                foreach (NPC npc in Main.npc)
                {
                    if (npc.active && !npc.friendly && (npc.Center - Projectile.Center).Length() <= min_distance && npc.CanBeChasedBy())
                    {
                        min_distance = (npc.Center - Projectile.Center).Length();
                        targeNpc = npc;
                    }
                }

                //刚发射弹幕规定攻击状态为搜寻
                if (Timer == 0)
                    State = AttackState.Search;
                switch (State)
                {
                    case AttackState.Search:
                        if (targeNpc != null)
                        {
                            //如果有目标且距离为370~110，则对目标进行跟踪
                            if ((Projectile.Center - targeNpc.Center).Length() < 370 && (Projectile.Center - targeNpc.Center).Length() > 100f)
                            {
                                Projectile.tileCollide = false;
                                Projectile.velocity = ((targeNpc.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 18f + Projectile.velocity * 5) / 6;
                                Visuals();
                            }
                            //如果接近目标（<100），则修改攻击状态为准备
                            if ((Projectile.Center - targeNpc.Center).Length() < 100f)
                            {
                                Visuals();
                                Projectile.tileCollide = false;
                                Projectile.velocity *= 0;
                                State = AttackState.Ready;
                                Projectile.netUpdate = true;
                                Timer = 1;
                                //因为每帧移动20像素（速度20），因为帧不能有小数，无法精确到单位的像素，所以强制设置坐标来规避，半径100
                                //Projectile.Center = targeNpc.Center + (Projectile.Center - targeNpc.Center).SafeNormalize(Vector2.Zero) * 100f;
                            }
                        }
                        //如果没有目标则默认飞行
                        else
                        {
                            //若追踪失败导致速度降低至非0，则缓慢提升速度至14f
                            //这个情况来自dash不了快速移动的怪
                            if (Projectile.velocity.Length() < 14f && Projectile.velocity.Length() > 0f)
                            {
                                Projectile.velocity *= 1.1f;
                            }
                            //当速度为0，快速杀死该射弹
                            //这个情况来自ready旋转方向角时，目标怪被杀死，速度为0且无目标追踪
                            else if (Projectile.velocity.Length() == 0)
                            {
                                Projectile.timeLeft -= 30;
                            }
                            Projectile.tileCollide = true;
                            Visuals();
                        }
                        break;

                    case AttackState.Ready:
                        Projectile.tileCollide = false;
                        if (targeNpc == null)
                        {
                            State = AttackState.Search;
                            Projectile.netUpdate = true;
                            break;
                        }
                        Timer++;
                        float diff = (targeNpc.Center - Projectile.Center).ToRotation();
                        //如果倒计时12到了，则进行冲刺准备
                        if (Timer == 13)
                        {
                            // 确定速度
                            Projectile.velocity = (targeNpc.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 20f;
                            State = AttackState.Dash;
                            Projectile.netUpdate = true;
                            Timer = 1;
                        }
                        //否则，慢慢调整飞剑的旋转角
                        else
                        {
                                Projectile.rotation = diff - (float)Math.PI / 2 + (float)Math.PI * Timer / 12f;
                        }
                        break;

                    case AttackState.Dash:
                        //接近目标冲刺点时停止冲刺，改变攻击状态
                        Projectile.tileCollide = false;
                        Projectile.velocity *= 0.90f;
                        if (Projectile.velocity.Length() < 1f)
                        {
                            State = AttackState.Search;
                            Projectile.netUpdate = true;
                        }
                        break;

                    default:
                        State = AttackState.Ready;
                        Projectile.netUpdate = true;
                        break;
                }
            }
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
                //撞墙产生粒子
                for (int i = 0; i < 10; i++)
                {
                    Dust dust = Dust.NewDustDirect(new Vector2(Projectile.Center.X - 4 - 10 / 4, Projectile.Center.Y - 4 - 10 / 4), 10, 10, DustID.PortalBolt, 0, 0, 100, new Color(255, 255, 0) * 0.8f, 1.5f);
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
            for (int i = 0; i < 20; i++)
            {
                dust = Dust.NewDustDirect(new Vector2(Projectile.Center.X - 4 - 10 / 4, Projectile.Center.Y - 4 - 10 / 4), 10, 10, DustID.PortalBolt, 0, 0, 100, new Color(255, 255, 0) * 0.8f, 1.2f);
                dust.noGravity = true;
            }

        }

        private void Visuals()
        {
            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.rotation += MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, new Color(255, 255, 0).ToVector3() * 1f);
        }

    }
}