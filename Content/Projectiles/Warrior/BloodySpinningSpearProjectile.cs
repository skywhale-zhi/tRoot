using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Projectiles.Warrior
{
    //血钻长矛的本体射弹
    internal class BloodySpinningSpearProjectile : ModProjectile
    {
        //定义矛射弹的射程。这些是可重写的属性，以应对您希望使一个类继承自这个类。
        protected virtual float HoldoutRangeMin => 60f;

        protected virtual float HoldoutRangeMax => 260f;

        public override void SetDefaults()
        {
            //克隆矛的默认值。矛的特定值设置为宽度、高度、aiStyle、是否友好、穿透数、撞墙情况、比例、隐藏、所有者击中检查和近战。
            //(width, height, aiStyle, friendly, penetrate, tileCollide, scale, hide, ownerHitCheck, and melee.)
            Projectile.CloneDefaults(ProjectileID.Spear);
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.DamageType = DamageClass.Melee;
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

            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0, 0, Alpha: 0, Color.White, Scale: 1f);


            return false; // Don't execute vanilla AI.
        }

        public override void AI()
        {
        }

        //击中敌怪时生成黏附弹幕，ai[0]为黏附敌怪索引
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //如果坠落火星过多，停止生成黏附弹幕
            if (Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<BloodySpinningSpearProjectile3>()] < 30)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BloodySpinningSpearProjectile2>(), (int)(Projectile.damage * 0.8f), 0, Projectile.owner, target.whoAmI);
            }
        }
    }

    //黏附弹幕
    //ai0 == index 是黏附敌怪索引
    internal class BloodySpinningSpearProjectile2 : ModProjectile
    {
        public override string Texture => "tRoot/Content/Projectiles/Warrior/BloodySpinningSpearProjectile";

        public Vector2 offPos;  //位置偏移
        public float offRor;    //偏移角度


        public override void OnSpawn(IEntitySource source)
        {
            offPos = (Projectile.Center - Main.npc[(int)Projectile.ai[0]].Center) * 0.5f;
            offRor = Main.npc[(int)Projectile.ai[0]].rotation;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            DrawOffsetX = -20;
            DrawOriginOffsetY = -20;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 60;
        }


        public override void AI()
        {
            Projectile.Center = Main.npc[(int)Projectile.ai[0]].Center + offPos.RotatedBy(Main.npc[(int)Projectile.ai[0]].rotation - offRor);

            if (!Main.npc[(int)Projectile.ai[0]].active)
            {
                Projectile.Kill();
            }
            if (!Main.dedServ)
                for (int i = 0; i < 4; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.Blood, 0, 0, Alpha: 20, Color.White, Scale: 1f);
                    //dust.velocity = dust.velocity.RotatedByRandom(MathHelper.TwoPi) * 2f;
                }

            if (Main.rand.NextBool(60))
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 5f, ModContent.ProjectileType<BloodySpinningSpearProjectile3>(), (int)(Projectile.damage * 0.5f), 1, Projectile.owner);
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(((Projectile.Center - Main.npc[(int)Projectile.ai[0]].Center) * 0.5f));
            writer.Write(Main.npc[(int)Projectile.ai[0]].rotation);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            offPos = reader.ReadVector2();
            offRor = reader.ReadSingle();
        }


        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 5f, ModContent.ProjectileType<BloodySpinningSpearProjectile3>(), (int)(Projectile.damage * 0.5f), 1, Projectile.owner);
        }
    }



    //坠落火星
    internal class BloodySpinningSpearProjectile3 : ModProjectile
    {
        public override string Texture => "tRoot/Content/Projectiles/Warrior/BloodySpinningSpearProjectile";

        public override void SetDefaults()
        {
            Projectile.width = 3;
            Projectile.height = 3;
            DrawOffsetX = -1;
            DrawOriginOffsetY = -1;
            Projectile.penetrate = 6;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.tileCollide = true;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 120;
        }


        public override void AI()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            if (Projectile.velocity.Y < 10)
            {
                Projectile.velocity += Vector2.UnitY * 0.1f;
                Projectile.velocity.X *= 0.99f;
            }
            if (Projectile.velocity.LengthSquared() > 0.1)
            {
                if (!Main.dedServ)
                    for (int i = 0; i < 2; i++)
                    {
                        Dust dust = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.Blood, 0, 0, Alpha: 200, Color.White, Scale: 1f);
                        dust.velocity = dust.velocity.RotatedByRandom(MathHelper.TwoPi) * 0.4f;
                    }
            }
            else if (!Main.dedServ)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.Blood, 0, 0, Alpha: 20, Color.White, Scale: 1f);
                dust.velocity = dust.velocity.RotatedByRandom(MathHelper.TwoPi) * 0.5f;
                dust.velocity.X *= 0.3f;
                dust.velocity.Y -= 2;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = Vector2.Zero;
            return false;
        }
    }
}
