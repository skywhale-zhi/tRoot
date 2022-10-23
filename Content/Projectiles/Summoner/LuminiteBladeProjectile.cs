using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Projectiles.Summoner
{
    internal class LuminiteBladeProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // 设置此仆从在其精灵图上的帧数
            Main.projFrames[Projectile.type] = 9;
            // 这对于右击定位是必要的
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            //表示该投射物是宠物或仆从
            Main.projPet[Projectile.type] = true;
            //这是必要的，这样你的仆从可以在召唤时正确繁殖，并在其他仆从被召唤时被替换
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            //让邪教徒抵抗这种投射物，因为邪教徒对追踪投射物有减伤
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;

            //拖尾幻影
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 14;
            //使仆从能穿墙穿过瓷砖
            Projectile.tileCollide = false;
            DrawOffsetX = -2;
            DrawOriginOffsetY = -5;
            DrawOriginOffsetX = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.scale *= 1.5f;
            Projectile.ArmorPenetration = 100;
            Projectile.ignoreWater = true;
            //以下是仆从武器所需的
            Projectile.friendly = true; // 仅当它在接触时对敌人造成伤害时才进行控制（稍后将详细介绍）
            Projectile.minion = true; // 将其声明为仆从（具有多种效果）
            Projectile.DamageType = DamageClass.Summon; // 声明伤害类型（造成伤害所需）
            Projectile.minionSlots = 1; // 该仆从在玩家可用仆从槽位总数中所占的槽位数量（稍后将详细介绍）
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Summon;
        }

        //在这里，你可以决定你的仆从是否打碎了草或罐子之类的东西
        public override bool? CanCutTiles()
        {
            return true;
        }

        // 如果你的仆从造成接触伤害（移动区域的AI（）中的其他相关内容），这是强制性的
        public override bool MinionContactDamage()
        {
            return true;
        }

        // The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
        //该仆从的人工智能分为多种方法，以避免膨胀。该方法只在调用AI的实际部分之间传递值。
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!CheckActive(owner))
            {
                return;
            }
            //以下方法的距离均用的平方，因为平方避免的开方计算，速度更快
            GeneralBehavior(owner, out Vector2 idlePosition, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
            SearchForTargets(out NPC targetnpc, owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            Movement(targetnpc, foundTarget, distanceFromTarget, targetCenter, distanceToIdlePosition, vectorToIdlePosition, idlePosition);
            Visuals();
        }


        //这是“主动检查”，确保仆从在玩家活着的时候还活着，如果没有，就取消仆从
        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<Buffs.Minions.LuminiteBladeBuff>());

                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<Buffs.Minions.LuminiteBladeBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }

        /// <summary>
        /// 一般行为
        /// </summary>
        /// <param name="owner">玩家自己</param>
        /// <param name="idlePosition">仆从的世界坐标</param>
        /// <param name="vectorToIdlePosition">从玩家指向仆从的向量</param>
        /// <param name="distanceToIdlePosition">从玩家指向仆从的向量的距离平方</param>
        private void GeneralBehavior(Player owner, out Vector2 idlePosition, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition)
        {
            //获得当前这个仆从召唤了几个
            int count = owner.ownedProjectileCounts[ModContent.ProjectileType<LuminiteBladeProjectile>()];
            //角度偏移
            List<Projectile> temp = new List<Projectile>();
            foreach (Projectile p in Main.projectile)
            {
                if (p.type == ModContent.ProjectileType<LuminiteBladeProjectile>() && p.owner == owner.whoAmI && p.active)
                {
                    temp.Add(p);
                    if (temp.Count == count)
                    {
                        break;
                    }
                }
            }
            //需要标记的偏移数目
            int offsetcount = 0;
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].whoAmI == Projectile.whoAmI)
                {
                    offsetcount = i;
                    break;
                }
            }

            Vector2 offR = Vector2.UnitY.RotatedBy(offsetcount * MathHelper.TwoPi / count + Main.timeForVisualEffects / MathHelper.TwoPi * 0.02f * (count - 1));
            idlePosition = owner.Center - 100 * offR;

            Projectile.rotation = offR.RotatedBy(MathHelper.PiOver2).ToRotation();

            //如果距离太远，则传送给玩家
            vectorToIdlePosition = idlePosition - Projectile.Center;
            distanceToIdlePosition = vectorToIdlePosition.LengthSquared();
            // 无论何时，当您处理的非常规事件会彻底改变行为或位置时，请确保仅在射弹的所有者（owner）上运行代码
            if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 4000f * 4000f)
            {
                // 然后将 netUpdate 设置为 true
                Projectile.Center = idlePosition;
                Projectile.velocity *= 0;
                Projectile.netUpdate = true;
                if (!Main.dedServ)
                    for (int i = 0; i < 30; i++)
                    {
                        Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                        Dust.NewDustPerfect(Projectile.Center, 160, speed * 1, 0, default, 2);
                    }
            }
        }

        //搜寻目标
        /// <summary>
        /// 搜寻目标
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="foundTarget">是否找到npc</param>
        /// <param name="distanceFromTarget">目标npc离射弹的距离平方</param>
        /// <param name="targetCenter">目标npc的中心</param>
        private void SearchForTargets(out NPC targetnpc, Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter)
        {
            // Starting search distance
            distanceFromTarget = 6000 * 6000;
            targetCenter = Projectile.position;
            foundTarget = false;
            targetnpc = null;

            // 如果您的仆从武器具有瞄准功能，则需要此代码
            // 右键锁定功能
            if (owner.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[owner.MinionAttackTargetNPC];
                float between = Vector2.DistanceSquared(npc.Center, Projectile.Center);

                // 合理的距离，使其不会跨越多个屏幕
                if (between <= 6000 * 6000)
                {
                    distanceFromTarget = between;
                    targetCenter = npc.Center;
                    targetnpc = npc;
                    foundTarget = true;
                }
            }

            if (!foundTarget)
            {
                foreach (NPC npc in Main.npc)
                {
                    if ((distanceFromTarget > Vector2.DistanceSquared(Projectile.Center, npc.Center)) && npc.CanBeChasedBy())
                    {
                        distanceFromTarget = Vector2.DistanceSquared(Projectile.Center, npc.Center);
                        targetCenter = npc.Center;
                        foundTarget = true;
                        targetnpc = npc;
                    }
                }
            }

            //这个可以确定当没有目标时，仆从不造成伤害
            //Projectile.friendly = foundTarget;
        }

        private void Movement(NPC targetnpc, bool foundTarget, float distanceFromTarget, Vector2 targetCenter, float distanceToIdlePosition, Vector2 vectorToIdlePosition, Vector2 idlePosition)
        {
            // 默认移动参数（此处用于攻击）
            if (foundTarget)
            {
                float speed;
                if (targetnpc.velocity.Length() < 30)
                {
                    speed = 40;
                }
                else
                {
                    speed = targetnpc.velocity.Length() + 10f;
                }

                //仆从有一个目标：攻击（这里，向敌人飞去）
                Vector2 direction = targetCenter - Projectile.Center;
                if (distanceFromTarget > 170 * 170)
                {
                    //目标周围的直接距离（因此在接近时不会锁定）
                    direction = direction.SafeNormalize(Vector2.UnitX);
                    direction *= speed;
                    Projectile.velocity = direction;
                    Projectile.rotation = direction.RotatedBy(-MathHelper.PiOver2).ToRotation();
                }
                else
                {
                    //如果速度为0，也就是特殊情况，生成怪物在<170内，导致射弹速度不动，但是却找到了怪物，这里强制设置速度避免这种情况
                    if (Projectile.velocity == Vector2.Zero)
                    {
                        direction = direction.SafeNormalize(Vector2.UnitX);
                        direction *= speed;
                        Projectile.velocity = direction;
                        Projectile.rotation = direction.RotatedBy(-MathHelper.PiOver2).ToRotation();
                    }

                    //冲刺打击敌怪旋转
                    Projectile.rotation = direction.RotatedBy(-MathHelper.PiOver2).ToRotation();
                }
            }
            else
            {
                //仆从没有目标：返回玩家并空闲
                //把仆从速度重置
                Projectile.velocity = Vector2.Zero;
                Projectile.Center = Projectile.Center.MoveTowards(idlePosition, 25);
            }
        }

        private void Visuals()
        {
            //这是一个简单的“从上到下遍历所有帧”动画
            int frameSpeed = 8;
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
            // 红绿闪光
            if (Main.rand.NextBool(2))
            {
                Lighting.AddLight(Projectile.Center, new Color(154, 248, 224).ToVector3() * 0.78f);
            }
            else
            {
                Lighting.AddLight(Projectile.Center, new Color(213, 155, 148).ToVector3() * 0.78f);
            }
            //精灵图指向
            Projectile.spriteDirection = (int)(Projectile.velocity.X / Math.Abs(Projectile.velocity.X));
        }


        //残影的绘制
        public override bool PreDraw(ref Color lightColor)
        {
            //幻影纹理
            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new Vector2(projectileTexture.Width * 0.5f, Projectile.height * 0.5f);
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Rectangle sourceRectangle = new Rectangle(0, Projectile.frame * 28, 18, 28);
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color;
                if (k == 0)
                {
                    color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                }
                else//次级残影不能过亮，这里 * 0.5f
                {
                    color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) * 0.5f / Projectile.oldPos.Length);
                }
                Main.spriteBatch.Draw(projectileTexture, drawPos, sourceRectangle, color, Projectile.rotation, drawOrigin, Projectile.scale - k / (float)Projectile.oldPos.Length / 3, spriteEffects, 0f);
            }
            return false;
        }


        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (!Main.dedServ)
                SoundEngine.PlaySound(SoundID.Item50 with { Pitch = -0.3f }, Projectile.position);
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            //玩家获得的破甲额外奖励伤害
            float armorp;
            armorp = Main.player[Projectile.owner].GetArmorPenetration(DamageClass.Generic);
            if (target.ichor)
            {
                armorp += 15;
            }
            if (target.betsysCurse)
            {
                armorp += 40;
            }
            if (target.HasBuff(ModContent.BuffType<Buffs.MixedToxinⅠ>()))
            {
                armorp += 20;
            }
            damage += (int)(armorp * 0.5);

            //敌怪获得的减益额外奖励伤害
            if (target.lifeRegen < 0)
            {
                int r = -(int)(target.lifeRegen * 0.15f);
                if (r <= 300)//设置一个上限
                {
                    damage += r;
                }
                else
                {
                    damage += 300;
                }
            }

            //射弹自身的破甲额外奖励伤害
            if(target.defense < 100)
            {
                damage += (int)(target.defense * 0.001);
            }
        }
    }
}
