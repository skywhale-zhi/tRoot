using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Projectiles.Master
{
    public class ArcaneFloatingCannonProjectile : ModProjectile
    {
        #region 私有数据
        private Vector2 target = Vector2.Zero;      //目标位置
        private Vector2 targetV = Vector2.Zero;     //目标速度
        private int surroundCount = 0;  //环绕射弹标号
        private const int Count = 8;         //限制的环绕数目
        private AttackState State           //射弹状态
        {
            set
            {
                Projectile.ai[1] = (int)value;
            }
            get
            {
                return (AttackState)Projectile.ai[1];
            }
        }
        private enum AttackState : int  // 射弹状态枚举
        {
            Null,         //无状态
            Surround,    //环绕
            Launch,     //正常发射
            Dash       //冲刺
        };
        #endregion

        public override void SetDefaults()
        {
            Projectile.width = 6; // 碰撞箱宽
            Projectile.height = 6; // 碰撞箱长
            DrawOffsetX = -7;
            DrawOriginOffsetY = -7;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;

            Projectile.friendly = true; // 伤害敌人？
            Projectile.hostile = false; // 伤害玩家？
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1; // 射弹能穿透多少怪物。（下面的OnTileCollide也会减少穿透反弹）
            Projectile.timeLeft = 60 * 10; // 弹丸的有效时间（60=1秒，因此600是10秒）
            Projectile.ignoreWater = true; // 弹丸的速度会受到水的影响吗？
            Projectile.tileCollide = true; // 弹丸会与瓷砖碰撞吗？
            Projectile.extraUpdates = 1;
            //弹药大小
            Projectile.scale = 0.7f;
        }

        public override void AI()
        {
            Projectile.rotation += 0.05f * (float)Projectile.direction;
            for (int i = 0; i < 2; i++)
            {
                Dust dust = Dust.NewDustDirect(new Vector2(Projectile.Center.X - 4 - 10 / 4, Projectile.Center.Y - 4 - 10 / 4), 10, 10, DustID.PortalBolt, 0, 0, 0, new Color(0, 209, 255), 1f);
                dust.noGravity = true;
                dust.velocity *= 0;
                //dust.position = Projectile.Center;
            }
            Player player = Main.player[Projectile.owner];
            Lighting.AddLight(Projectile.Center, new Color(0, 209, 255).ToVector3() * 0.8f);

            //统计环绕射弹数目
            SetSurroundCount();
            //当环绕射弹<count且不为冲刺和发射状态时，设置为环绕状态
            if (surroundCount <= Count && State == AttackState.Null)
            {
                State = AttackState.Surround;
                Projectile.netUpdate = true;
            }
            //当射弹为环绕且按下右键，则设置为冲刺状态
            if (Main.mouseRight && !Main.mouseLeft && State == AttackState.Surround && Main.myPlayer == Projectile.owner)
            {
                State = AttackState.Dash;
                Projectile.netUpdate = true;
            }

            //当环绕数目为count且为新生射弹的时候，设置为发射状态
            if (surroundCount == Count + 1 && Projectile.timeLeft == 600 && State == AttackState.Null)
            {
                State = AttackState.Launch;
                Projectile.netUpdate = true;
            }

            //状态机
            switch (State)
            {
                case AttackState.Surround:
                    // 射弹速率可以自行调整
                    float t = Projectile.ai[0] * 0.02f;
                    Projectile.ai[0]++;
                    // 要把弹幕速度归零，否则圆会有一个位移
                    Projectile.velocity = Vector2.Zero;
                    //四分之一的概率形成外圆，四分之三形成内部五芒星
                    //(cost-sint,sint-cost)是方程主体
                    //0.7*cos3.07t - 1.35*sin2t ,0.7和1.35修改芒星胖瘦程度，即芒星的边外翻或内陷。cos3.07 和 sin2 ，3+2=5五芒星，后者必须比前者小，0.07让芒星缓慢旋转。50是可能的半径
                    Projectile.Center = player.Center + new Vector2(0.7f * (float)Math.Cos(3.07 * t) - 1.35f * (float)Math.Sin(2 * t), 0.7f * (float)Math.Sin(3.07 * t) - 1.35f * (float)Math.Cos(2 * t)) * 50f;
                    //环绕时射弹永不消失
                    Projectile.timeLeft = 60;
                    Projectile.penetrate = 2;
                    //外部圆圈的粒子
                    if (Main.rand.NextBool(5))
                        for (int i = 0; i < surroundCount; i++)
                        {
                            Dust dust = Dust.NewDustDirect(player.Center + new Vector2((float)Math.Cos(Projectile.ai[0]), (float)Math.Sin(Projectile.ai[0])) * 110f + new Vector2(-5, -5)/*纠正下小小的偏移*/, 8, 8, DustID.PortalBolt, 0, 0, 0, new Color(30, 209, 255), 1f);
                            dust.noGravity = true;
                            dust.velocity *= 0;
                        }
                    break;

                case AttackState.Launch:
                    if (Main.myPlayer == Projectile.owner)
                    {
                        if (target == Vector2.Zero)
                        {
                            target = new Vector2(Main.mouseX + Main.screenPosition.X, Main.mouseY + Main.screenPosition.Y);
                            targetV = (target - player.Center).SafeNormalize(Vector2.Zero) * 7f;
                            Projectile.timeLeft = 9 * 60;
                        }
                        Projectile.velocity = targetV;
                        Projectile.penetrate = 1;
                        Projectile.netUpdate = true;
                    }
                    break;

                case AttackState.Dash:
                    if (Main.myPlayer == Projectile.owner)
                    {
                        if (target == Vector2.Zero)
                        {
                            target = new Vector2(Main.mouseX + Main.screenPosition.X, Main.mouseY + Main.screenPosition.Y);
                            targetV = (target - Projectile.Center).SafeNormalize(Vector2.Zero) * 10f;
                            Projectile.timeLeft = 5 * 60;
                        }
                        Projectile.velocity = targetV;
                        State = AttackState.Dash;
                        Projectile.extraUpdates = 2;
                        Projectile.penetrate = 1;
                        Projectile.netUpdate = true;
                    }
                    break;

                default:
                    break;
            }

        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //如果与瓷砖碰撞，则减少穿透。
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                //撞击，击中贴图引发的贴图效果，如方块灰尘等
                Projectile.Kill();
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            //EntitySource_ItemUse_WithAmmo source;
            //此代码和上面在OnTileCollide中的类似代码会从与之碰撞的瓷砖中产生灰尘。SoundID。第10项是你听到的反弹声。
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int i = 0; i < 7; i++)
            {
                Dust dust = Dust.NewDustDirect(new Vector2(Projectile.Center.X - 4 - 10 / 4, Projectile.Center.Y - 4 - 10 / 4), 10, 10, DustID.PortalBolt, 0, 0, 0, new Color(0, 209, 255) * 0.8f, 1.5f);
                dust.noGravity = true;
            }
        }

        //查找所有该射弹中环绕的数目，并给这些射弹设置surroundcount标号
        private int SetSurroundCount()
        {
            surroundCount = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].type == Projectile.type && Main.projectile[i].owner == Projectile.owner && Main.projectile[i].active && (Main.projectile[i].ai[1] == (float)AttackState.Null || Main.projectile[i].ai[1] == (float)AttackState.Surround))
                {
                    surroundCount++;
                    //如果环绕数大于10，循环结束，数目记录为10
                    if (surroundCount > Count + 1)
                    {
                        surroundCount = Count + 1;
                        break;
                    }
                }
            }
            return surroundCount;
        }
    }
}