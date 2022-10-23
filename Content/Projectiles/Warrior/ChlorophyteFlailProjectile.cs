using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using ReLogic.Content;

namespace tRoot.Content.Projectiles.Warrior
{
    //叶绿链球
    public class ChlorophyteFlailProjectile : ModProjectile
    {
        private const string ChainTexturePath = "tRoot/Content/Projectiles/Warrior/ChlorophyteFlailProjectileChain";
        private const string ChainTextureExtraPath = "tRoot/Content/Projectiles/Warrior/ChlorophyteFlailProjectileChainExtra";

        private enum AIState
        {
            Spinning,           //旋转
            LaunchingForward,   //向前扔出
            Retracting,         //收回
            UnusedState,        //未使用状态
            ForcedRetracting,   //强制牵引
            Ricochet,           //跳弹
            Dropping            //下坠
        }

        private bool RightMode = false;

        // These properties wrap the usual ai and localAI arrays for cleaner and easier to understand code.
        private AIState CurrentAIState
        {
            get => (AIState)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }

        //状态计时器
        public ref float StateTimer => ref Projectile.ai[1];

        //碰撞计数器
        public ref float CollisionCounter => ref Projectile.localAI[0];

        //旋转状态计时器
        public ref float SpinningStateTimer => ref Projectile.localAI[1];

        public override void SetStaticDefaults()
        {
            // These lines facilitate the trail drawing
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true; // 这确保了当其他玩家加入世界时，投射物是同步的。
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true; // 用于ai挂钩中的命中冷却更改
            Projectile.localNPCHitCooldown = 10; // 这有助于自定义命中冷却逻辑

            // 原版链球都使用aiStyle 15，但代码不可自定义，因此 AI method 中使用了该aiStyle的自适应
        }

        // 此AI代码改编自原版代码：Terraria.spollet.AI_015_Flails（）
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            // 如果玩家死亡或被人群控制，则杀死投射物
            if (!player.active || player.dead || player.noItems || player.CCed || Vector2.Distance(Projectile.Center, player.Center) > 900f)
            {
                Projectile.Kill();
                return;
            }
            if (Main.myPlayer == Projectile.owner && Main.mapFullscreen)
            {
                Projectile.Kill();
                return;
            }
            Vector2 mountedCenter = player.MountedCenter;

            bool doFastThrowDust = false;
            bool shouldOwnerHitCheck = false; //假如所有者点击检查
            int launchTimeLimit = 20;  //最大发射时间：弹丸在缩回之前可以移动多少时间（速度和shootTimer将设置连枷的射程）
            float launchSpeed = 20f; // 弹丸的发射速度
            float maxLaunchLength = 1000f; // 最大发射距离：在发射状态下，弹丸链在被迫收回之前可以伸展多远
            float retractAcceleration = 5f; // 收缩加速度：弹丸在收回时加速回到玩家身边的速度有多快
            float maxRetractSpeed = 18f; // 弹丸收回时的最大速度
            float forcedRetractAcceleration = 6f; //强制缩回加速度：弹丸在被迫收回时加速回到玩家身边的速度有多快
            float maxForcedRetractSpeed = 33f; // 弹丸在被迫收回时的最大速度

            float unusedRetractAcceleration = 1f; //未使用的回缩加速度
            float unusedMaxRetractSpeed = 14f; // 未使用的最大收回速度
            //int unusedChainLength = 60; //未使用的链条长度

            int defaultHitCooldown = 10; // 当链家躺在地上或缩回时，连枷撞击的频率有多高
            int spinHitCooldown = 20; // 旋转时连枷撞击的频率
            int movingHitCooldown = 10; // 移动时连枷撞击的频率

            int ricochetTimeLimit = launchTimeLimit - 10; //跳弹时限

            ////如果玩家提高他们的近战速度，则通过玩家的近战速度来调整这些速度和加速度，使武器更加灵敏
            float meleeSpeed = player.GetAttackSpeed(DamageClass.Melee);//获取该玩家该伤害类型的攻击速度修正值。这将返回一个引用，因此，您可以使用运算符自由修改此方法的返回值。

            float meleeSpeedMultiplier = 1f / meleeSpeed;
            launchSpeed *= meleeSpeed;
            unusedRetractAcceleration *= meleeSpeedMultiplier;
            unusedMaxRetractSpeed *= meleeSpeedMultiplier;
            retractAcceleration *= meleeSpeed;
            maxRetractSpeed *= meleeSpeed;
            forcedRetractAcceleration *= meleeSpeed;
            maxForcedRetractSpeed *= meleeSpeed;
            float launchRange = launchSpeed * launchTimeLimit;
            float maxDroppedRange = launchRange + 158f;
            Projectile.localNPCHitCooldown = defaultHitCooldown;

            switch (CurrentAIState)
            {
                case AIState.Spinning://旋转
                    {
                        shouldOwnerHitCheck = true;
                        if (Projectile.owner == Main.myPlayer)
                        {   //朝向鼠标的单位矢量
                            Vector2 unitVectorTowardsMouse = mountedCenter.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.UnitX * player.direction);
                            //更改方向
                            player.ChangeDir((unitVectorTowardsMouse.X > 0f) ? 1 : (-1));

                            //右键模式，不想要把下面3个if全删掉
                            if (!player.controlUseItem && Main.mouseRight)
                                RightMode = true;
                            if (player.controlUseItem && !Main.mouseRight)
                                RightMode = false;
                            if (RightMode && SpinningStateTimer >= 18)
                            {
                                RightMode = true;
                                Projectile.tileCollide = true;
                                SpinningStateTimer = 0;
                                CurrentAIState = AIState.LaunchingForward;
                                StateTimer = 0f;
                                Projectile.velocity = unitVectorTowardsMouse * launchSpeed + player.velocity;
                                Projectile.Center = mountedCenter;
                                Projectile.netUpdate = true;
                                //重置本地NPC命中免疫
                                Projectile.ResetLocalNPCHitImmunity();
                                Projectile.localNPCHitCooldown = movingHitCooldown;
                                break;
                            }


                            if (!player.channel && !Main.mouseRight && !RightMode) // 如果玩家释放，则切换到发射模式
                            {
                                RightMode = false;
                                CurrentAIState = AIState.LaunchingForward;
                                StateTimer = 0f;
                                Projectile.velocity = unitVectorTowardsMouse * launchSpeed + player.velocity;
                                Projectile.Center = mountedCenter;
                                Projectile.netUpdate = true;
                                //重置本地NPC命中免疫
                                Projectile.ResetLocalNPCHitImmunity();
                                Projectile.localNPCHitCooldown = movingHitCooldown;
                                break;
                            }
                        }
                        SpinningStateTimer += 1f;
                        //这条线创建了一个单位向量，该向量围绕玩家不断旋转。10f控制投射物在玩家周围视觉旋转的速度
                        // This line creates a unit vector that is constantly rotated around the player. 10f controls how fast the projectile visually spins around the player
                        Vector2 offsetFromPlayer = new Vector2(player.direction).RotatedBy((float)Math.PI * 10f * (SpinningStateTimer / 60f) * player.direction);
                        //向四周发射规律孢子
                        if (Main.rand.NextBool(20))
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), mountedCenter + offsetFromPlayer * 35f, offsetFromPlayer, ProjectileID.SporeCloud, (int)(Projectile.damage * 0.8f), 0.5f, Projectile.owner,0,0);
                        }

                        offsetFromPlayer.Y *= 0.8f;
                        if (offsetFromPlayer.Y * player.gravDir > 0f)
                        {
                            offsetFromPlayer.Y *= 0.5f;
                        }
                        Projectile.Center = mountedCenter + offsetFromPlayer * 30f;
                        Projectile.velocity = Vector2.Zero;
                        Projectile.localNPCHitCooldown = spinHitCooldown; // set the hit speed to the spinning hit speed
                        break;
                    }
                case AIState.LaunchingForward://发射状态
                    {
                        Projectile.tileCollide = true;
                        doFastThrowDust = true;
                        //应该转向收回状态 = 状态计数器 大于等于了 发射时间限制
                        bool shouldSwitchToRetracting = StateTimer++ >= launchTimeLimit;
                        //应该转向收回状态 = 应该转向收回状态 或运算 射弹离玩家的距离大于等于了最大发射长度
                        shouldSwitchToRetracting |= Projectile.Distance(mountedCenter) >= maxLaunchLength;

                        // 如果玩家在这个状态点击点击，则切换到下降状态
                        if (player.controlUseItem) 
                        {
                            CurrentAIState = AIState.Dropping;
                            StateTimer = 0f;
                            Projectile.netUpdate = true;
                            Projectile.velocity *= 0.2f;
                            // 这也是“滴滴怪跛子”产生其投射物的地方，请参见上面的代码。
                            //提前回收产生弹幕
                            if (Main.myPlayer == Projectile.owner)
                            {
                                for (int i = -2; i < 3; i++)
                                {
                                    int index = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.Pi / 3 * i), ProjectileID.CrystalLeafShot, Projectile.damage / 3, 0, Main.myPlayer);
                                    Projectile p = Main.projectile[index];
                                    p.usesLocalNPCImmunity = true;
                                    p.localNPCHitCooldown = 10;
                                    p.DamageType = DamageClass.Melee;
                                    p.penetrate = -1;
                                    p.timeLeft = 300;
                                    p.velocity *= 0.1f;
                                }
                            }
                            break;
                        }
                        //如果转化成收回状态
                        if (shouldSwitchToRetracting)
                        {
                            CurrentAIState = AIState.Retracting;
                            StateTimer = 0f;
                            Projectile.netUpdate = true;
                            Projectile.velocity *= 0.3f;
                            // 这也是“滴滴怪跛子”产生其投射物的地方，请参见上面的代码。
                            // 最大距离发射产生弹幕
                            if (Main.myPlayer == Projectile.owner)
                            {
                                for (int i = -4; i < 5; i++)
                                {
                                    int index = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.Pi / 4.8 * i), ProjectileID.CrystalLeafShot, (int)(Projectile.damage * 0.5f), 0, Main.myPlayer);
                                    Projectile p = Main.projectile[index];
                                    p.usesLocalNPCImmunity = true;
                                    p.localNPCHitCooldown = 8;
                                    p.DamageType = DamageClass.Melee;
                                    p.penetrate = -1;
                                    p.timeLeft = 300;
                                    p.velocity *= 0.1f;
                                }
                            }
                        }

                        //右键模式，不想要删掉下面这个if
                        if (shouldSwitchToRetracting && RightMode)
                        {
                            CurrentAIState = AIState.ForcedRetracting;
                            StateTimer = 0f;
                            Projectile.netUpdate = true;
                            Projectile.velocity *= 0.3f;
                            // 这也是“滴滴怪跛子”产生其投射物的地方，请参见上面的代码。
                        }

                        player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
                        Projectile.localNPCHitCooldown = movingHitCooldown;
                        break;
                    }
                case AIState.Retracting://回收
                    {
                        //单位回收速度向量
                        Vector2 unitVectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);

                        {
                            //右键模式，不想要删掉，普通回收
                            //if (Projectile.Distance(mountedCenter) <= maxRetractSpeed && Main.mouseRight)
                            //{
                            //    CurrentAIState = AIState.Spinning;
                            //    StateTimer = 0f;
                            //    Projectile.netUpdate = true;
                            //    Projectile.velocity *= 0.2f;
                            //    SoundEngine.PlaySound(SoundID.Item1, player.Center);
                            //    break;
                            //}
                        }

                        if (Projectile.Distance(mountedCenter) <= maxRetractSpeed)
                        {
                            Projectile.Kill(); // 一旦射弹离玩家足够近，就销毁他
                            return;
                        }
                        if (player.controlUseItem) // 如果玩家点击，则切换到下降状态
                        {
                            CurrentAIState = AIState.Dropping;
                            StateTimer = 0f;
                            Projectile.netUpdate = true;
                            Projectile.velocity *= 0.2f;
                        }
                        else
                        {

                            Projectile.velocity *= 0.98f;
                            //   射弹速度.MoveTowards(指向目的地移动的最大速度，加速度）；  返回一个加过加速度的射弹速度
                            Projectile.velocity = Projectile.velocity.MoveTowards(unitVectorTowardsPlayer * maxRetractSpeed, retractAcceleration);
                            player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
                        }
                        break;
                    }

                #region 这个案例实际上没有使用
                /*
                case AIState.UnusedState: // Projectile.ai[0] == 3; 这个案例实际上没有使用，但也许Terraria更新会将其添加回，或者可能它没有用，所以我把它留在这里。
                    {
                        if (!player.controlUseItem)
                        {
                            CurrentAIState = AIState.ForcedRetracting; // 如果玩家点击，移动到缩回模式 Move to super retracting mode if the player taps
                            StateTimer = 0f;
                            Projectile.netUpdate = true;
                            break;
                        }
                        float currentChainLength = Projectile.Distance(mountedCenter);
                        Projectile.tileCollide = StateTimer == 1f;
                        bool flag3 = currentChainLength <= launchRange;
                        if (flag3 != Projectile.tileCollide)
                        {
                            Projectile.tileCollide = flag3;
                            StateTimer = Projectile.tileCollide ? 1 : 0;
                            Projectile.netUpdate = true;
                        }
                        if (currentChainLength > unusedChainLength)
                        {

                            if (currentChainLength >= launchRange)
                            {
                                Projectile.velocity *= 0.5f;
                                Projectile.velocity = Projectile.velocity.MoveTowards(Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero) * unusedMaxRetractSpeed, unusedMaxRetractSpeed);
                            }
                            Projectile.velocity *= 0.98f;
                            Projectile.velocity = Projectile.velocity.MoveTowards(Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero) * unusedMaxRetractSpeed, unusedRetractAcceleration);
                        }
                        else
                        {
                            if (Projectile.velocity.Length() < 6f)
                            {
                                Projectile.velocity.X *= 0.96f;
                                Projectile.velocity.Y += 0.2f;
                            }
                            if (player.velocity.X == 0f)
                            {
                                Projectile.velocity.X *= 0.96f;
                            }
                        }
                        player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
                        break;
                    }
                */
                #endregion

                case AIState.ForcedRetracting: //强制收回
                    {
                        Projectile.tileCollide = false;
                        Vector2 unitVectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);

                        //右键模式，不想要删掉
                        if (Projectile.Distance(mountedCenter) <= maxForcedRetractSpeed && Main.mouseRight && Main.myPlayer == Projectile.owner)
                        {
                            CurrentAIState = AIState.Spinning;
                            StateTimer = 0f;
                            Projectile.velocity *= 0.2f;
                            Projectile.netUpdate = true;
                            SoundEngine.PlaySound(SoundID.Item1, player.Center);
                            break;
                        }

                        if (Projectile.Distance(mountedCenter) <= maxForcedRetractSpeed)
                        {
                            Projectile.Kill();
                            return;
                        }

                        Projectile.velocity *= 0.98f;
                        Projectile.velocity = Projectile.velocity.MoveTowards(unitVectorTowardsPlayer * maxForcedRetractSpeed, forcedRetractAcceleration);
                        Vector2 target = Projectile.Center + Projectile.velocity;
                        Vector2 value = mountedCenter.DirectionFrom(target).SafeNormalize(Vector2.Zero);
                        if (Vector2.Dot(unitVectorTowardsPlayer, value) < 0f)
                        {
                            Projectile.Kill(); // 如果射弹经过玩家，则杀死它
                            return;
                        }
                        player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
                        break;
                    }
                case AIState.Ricochet://跳弹
                    //if(Main.rand.NextBool(2))
                    if (StateTimer++ >= ricochetTimeLimit)
                    {
                        //撞击各种障碍物产生弹幕
                        if (Main.myPlayer == Projectile.owner)
                        {
                            for (int i = -3; i < 3; i++)
                            {
                                int index = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.Pi / 3 * i), ProjectileID.CrystalLeafShot, Projectile.damage / 4, 0, Main.myPlayer);
                                Projectile p = Main.projectile[index];
                                p.usesLocalNPCImmunity = true;
                                p.localNPCHitCooldown = 10;
                                p.DamageType = DamageClass.Melee;
                                p.penetrate = -1;
                                p.timeLeft = 300;
                                p.velocity *= 0.1f;
                            }
                        }
                        CurrentAIState = AIState.Dropping;
                        StateTimer = 0f;
                        Projectile.netUpdate = true;
                    }
                    else
                    {
                        Projectile.localNPCHitCooldown = movingHitCooldown;
                        Projectile.velocity.Y += 0.6f;
                        Projectile.velocity.X *= 0.95f;
                        player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
                    }
                    break;
                case AIState.Dropping://下坠
                    if (!player.controlUseItem || Projectile.Distance(mountedCenter) > maxDroppedRange)
                    {
                        CurrentAIState = AIState.ForcedRetracting;
                        StateTimer = 0f;
                        Projectile.netUpdate = true;
                    }
                    else
                    {
                        Projectile.velocity.Y += 0.8f;
                        Projectile.velocity.X *= 0.95f;
                        player.ChangeDir((player.Center.X < Projectile.Center.X) ? 1 : (-1));
                    }
                    break;
            }

            // 这是写花冠俘发射炮弹的地方。自己去反编译Terraria以查看该代码。
            //向四周发射不规律孢子粉
            if (Main.rand.NextBool(15))
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitX, ProjectileID.SporeCloud, (int)(Projectile.damage * 0.7f), 0.5f, Projectile.owner, 0, 0);
                proj.velocity = proj.velocity.RotatedByRandom(Math.PI);
            }



            Projectile.direction = (Projectile.velocity.X > 0f) ? 1 : -1;
            Projectile.spriteDirection = Projectile.direction;
            //这可以防止玩家在视线之外试图伤害敌人。旋转的自定义碰撞代码使得这一点非常必要。
            Projectile.ownerHitCheck = shouldOwnerHitCheck; // This prevents attempting to damage enemies without line of sight to the player. The custom Colliding code for spinning makes this necessary.
            


            //这个旋转代码对于连枷来说是唯一的，因为精灵图不是旋转对称的，并且有尖端。
            bool freeRotation = CurrentAIState == AIState.Ricochet || CurrentAIState == AIState.Dropping;
            if (freeRotation)
            {
                if (Projectile.velocity.Length() > 1f)
                    Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.velocity.X * 0.1f; // skid
                else
                    Projectile.rotation += Projectile.velocity.X * 0.1f; // roll
            }
            else
            {
                Vector2 vectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
                Projectile.rotation = vectorTowardsPlayer.ToRotation() + MathHelper.PiOver2;
            }


            //如果你有一个球形连枷，你可以用这个简化的旋转代码代替
            /*
			if (Projectile.velocity.Length() > 1f)
				Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.velocity.X * 0.1f; // skid
			else
				Projectile.rotation += Projectile.velocity.X * 0.1f; // roll
			*/


            Projectile.timeLeft = 2; //确保连枷不会死亡（当连枷躺在地上）
            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2); //增加延迟，使玩家无法按下连枷的按钮
            player.itemRotation = Projectile.DirectionFrom(mountedCenter).ToRotation();
            if (Projectile.Center.X < mountedCenter.X)
            {
                player.itemRotation += (float)Math.PI;
            }
            player.itemRotation = MathHelper.WrapAngle(player.itemRotation);


            //生成粒子。当处于LaunchingForward状态时，我们会更频繁地产生灰尘
            int dustRate = 6;
            if (doFastThrowDust)
                dustRate = 1;

            if (Main.rand.NextBool(dustRate))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Chlorophyte, 0f, 0f, 50, default(Color), 1.25f);
                dust.noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            int defaultLocalNPCHitCooldown = 10;
            int impactIntensity = 0;
            Vector2 velocity = Projectile.velocity;
            float bounceFactor = 0.2f;
            if (CurrentAIState == AIState.LaunchingForward || CurrentAIState == AIState.Ricochet)
                bounceFactor = 0.4f;

            if (CurrentAIState == AIState.Dropping)
                bounceFactor = 0f;

            if (oldVelocity.X != Projectile.velocity.X)
            {
                if (Math.Abs(oldVelocity.X) > 4f)
                    impactIntensity = 1;

                Projectile.velocity.X = (0f - oldVelocity.X) * bounceFactor;
                CollisionCounter += 1f;
            }

            if (oldVelocity.Y != Projectile.velocity.Y)
            {
                if (Math.Abs(oldVelocity.Y) > 4f)
                    impactIntensity = 1;

                Projectile.velocity.Y = (0f - oldVelocity.Y) * bounceFactor;
                CollisionCounter += 1f;
            }

            // If in the Launched state, spawn sparks
            // 如果处于启动状态，则生成火花
            if (CurrentAIState == AIState.LaunchingForward)
            {
                CurrentAIState = AIState.Ricochet;
                Projectile.localNPCHitCooldown = defaultLocalNPCHitCooldown;
                Projectile.netUpdate = true;
                Point scanAreaStart = Projectile.TopLeft.ToTileCoordinates();
                Point scanAreaEnd = Projectile.BottomRight.ToTileCoordinates();
                impactIntensity = 2;
                Projectile.CreateImpactExplosion(2, Projectile.Center, ref scanAreaStart, ref scanAreaEnd, Projectile.width, out bool causedShockwaves);
                Projectile.CreateImpactExplosion2_FlailTileCollision(Projectile.Center, causedShockwaves, velocity);
                Projectile.position -= velocity;
            }

            // Here the tiles spawn dust indicating they've been hit
            // 在这里，瓷砖上积满了灰尘，表明它们被击中了
            if (impactIntensity > 0)
            {
                Projectile.netUpdate = true;
                for (int i = 0; i < impactIntensity; i++)
                {
                    Collision.HitTiles(Projectile.position, velocity, Projectile.width, Projectile.height);
                }

                SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            }


            //收回时，如果卡在瓷砖上，则强制收回
            if (CurrentAIState != AIState.UnusedState && CurrentAIState != AIState.Spinning && CurrentAIState != AIState.Ricochet && CurrentAIState != AIState.Dropping && CollisionCounter >= 10f)
            {
                CurrentAIState = AIState.ForcedRetracting;
                Projectile.netUpdate = true;
            }

            //tModLoader目前不提供wetVelocity参数，该代码应使连枷在与水下瓦片碰撞时反弹更快。
            //if (Projectile.wet)
            //	wetVelocity = Projectile.velocity;

            return false;
        }


        //该射弹是否能够杀死tile（例如草）并伤害 NPC/玩家。返回 false 以防止它造成任何损害。
        //如果您希望抛射物造成伤害而不考虑默认黑名单，则返回 true。返回 null 以让弹丸遵循原版 can-damage-anything 规则。
        public override bool? CanDamage()
        {
            // 旋转模式下的连枷在前12帧内不会对敌人造成伤害。从视觉上看，这会延迟第一次命中，直到玩家在破坏任何东西之前摆动连枷一整圈。
            if (CurrentAIState == AIState.Spinning && SpinningStateTimer <= 12f)
                return false;
            return base.CanDamage();
        }


        //允许您在该投射物和该投射物可能损坏的玩家或NPC之间使用自定义碰撞检测
        //适用于对角激光、在其后面留下轨迹的投射物等。
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // Flails do special collision logic that serves to hit anything within an ellipse centered on the player when the flail is spinning around the player. For example, the projectile rotating around the player won't actually hit a bee if it is directly on the player usually, but this code ensures that the bee is hit. This code makes hitting enemies while spinning more consistant and not reliant of the actual position of the flail projectile.
            //连枷具有特殊的碰撞逻辑，当连枷围绕玩家旋转时，连枷可以击中以玩家为中心的椭圆内的任何物体。例如，如果投射物通常直接在玩家身上，围绕玩家旋转的投射物实际上不会击中蜜蜂，但此代码确保蜜蜂被击中。该代码使旋转时打击敌人更加一致，不依赖连枷弹丸的实际位置。
            if (CurrentAIState == AIState.Spinning)
            {
                Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
                Vector2 shortestVectorFromPlayerToTarget = targetHitbox.ClosestPointInRect(mountedCenter) - mountedCenter;
                shortestVectorFromPlayerToTarget.Y /= 0.8f; // Makes the hit area an ellipse. Vertical hit distance is smaller due to this math.
                float hitRadius = 55f; // The length of the semi-major radius of the ellipse (the long end)
                return shortestVectorFromPlayerToTarget.Length() <= hitRadius;
            }
            // Regular collision logic happens otherwise.
            return base.Colliding(projHitbox, targetHitbox);
        }


        //允许您对此投射物实施动态伤害缩放。例如，连枷在飞行中造成的伤害更大，而骑枪在移动快时造成的伤害也越大。
        //这个钩子只在所有者身上运行。
        public override void ModifyDamageScaling(ref float damageScale)
        {
            //右键自动发仅造成190%伤害
            if ((CurrentAIState == AIState.LaunchingForward || CurrentAIState == AIState.ForcedRetracting) && RightMode)
                damageScale *= 1.9f;

            // Flails do 20% more damage while spinning
            //连枷在旋转时造成的伤害增加20% 即 Projectile.damge*1.2f
            if (CurrentAIState == AIState.Spinning && !RightMode)
                damageScale *= 1.2f;

            //连枷在发射或收回时造成100%以上的伤害。这是连枷物品工具提示所要匹配的伤害，因为这是最常见的攻击模式。这就是该项具有ItemID.Set的原因。ToolTipDamageMultiplier[类型]=2f；
            if ((CurrentAIState == AIState.LaunchingForward || CurrentAIState == AIState.Retracting) && !RightMode)
            damageScale *= 2f;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            //连枷做一些定制的事情，你会想保持这些有同样的感觉香草连枷。
            //命中方向总是设置为远离玩家，即使连枷在返回时损坏npc
            //我猜这里是防止回收链球时击中敌怪，导致敌怪被击退向玩家，这不应该发生
            hitDirection = (Main.player[Projectile.owner].Center.X < target.Center.X) ? 1 : (-1);

            //在旋转模式下，击退的威力只有旋转模式的25%
            if (CurrentAIState == AIState.Spinning)
                knockback *= 0.25f;
            //在坠落模式下，击退的威力仅为50%
            if (CurrentAIState == AIState.Dropping)
                knockback *= 0.5f;

            base.ModifyHitNPC(target, ref damage, ref knockback, ref crit, ref hitDirection);
        }


        public override void Load()
        {
            chainTexture = ModContent.Request<Texture2D>(ChainTexturePath);
            chainTextureExtra = ModContent.Request<Texture2D>(ChainTextureExtraPath); //此纹理和相关代码是可选的，用于实现独特的效果
        }
        public override void Unload()
        {
            chainTexture = null;
            chainTextureExtra = null;
        }
        private static Asset<Texture2D> chainTexture;
        private static Asset<Texture2D> chainTextureExtra;
        // PreDraw用于在正常绘制投射物之前绘制链和轨迹。
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 playerArmPosition = Main.GetPlayerArmPosition(Projectile);

            //这修复了vanilla GetPlayerArmPosition错误，该错误导致链在爬坡时绘制错误。由于另一个类似的错误，连枷本身仍然不能正确牵引。一旦vanilla bug被修复，这应该被删除。
            playerArmPosition.Y -= Main.player[Projectile.owner].gfxOffY;

            Rectangle? chainSourceRectangle = null;
            // Drippler Crippler customizes sourceRectangle to cycle through sprite frames: sourceRectangle = asset.Frame(1, 6);
            // 滴滴怪链球自定义sourceRectangle以在精灵帧之间循环：sourceRectangle = asset.Frame(1, 6);
            float chainHeightAdjustment = 0f; //使用此选项调整链重叠。

            Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (chainTexture.Size() / 2f);
            Vector2 chainDrawPosition = Projectile.Center;
            Vector2 vectorFromProjectileToPlayerArms = playerArmPosition.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
            Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
            float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : chainTexture.Height()) + chainHeightAdjustment;
            if (chainSegmentLength == 0)
                chainSegmentLength = 10; //加载链纹理时，高度为0，这将导致无限循环。
            float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
            int chainCount = 0;
            float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;

            // This while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
            // 该while循环将链纹理从投射物绘制到玩家，循环以沿路径绘制链纹理
            while (chainLengthRemainingToDraw > 0f)
            {
                //此代码获取当前瓷砖坐标处的照明
                Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                // Flaming Mace and Drippler Crippler use code here to draw custom sprite frames with custom lighting.
                // Cycling through frames: sourceRectangle = asset.Frame(1, 6, 0, chainCount % 6);
                // This example shows how Flaming Mace works. It checks chainCount and changes chainTexture and draw color at different values
                // 火焰链球和滴滴怪链球在这里使用代码绘制具有自定义照明的自定义精灵框架。
                // 在帧之间循环：sourceRectangle = asset.Frame(1, 6, 0, chainCount % 6);
                // 这个例子展示了火焰链球的工作原理。它会检查链数并更改chainTexture，并以不同的值绘制颜色

                var chainTextureToDraw = chainTexture;
                if (chainCount >= 4)
                {
                    //使用普通链纹理和照明，无更改
                }
                else if (chainCount >= 2)
                {
                    //在球的附近，我们绘制了一个自定义链纹理，如果没有照明，则稍微使其发光。
                    chainTextureToDraw = chainTextureExtra;
                    /*解开这个注释启用微发光
                    byte minValue = 140;
                    if (chainDrawColor.R < minValue)
                        chainDrawColor.R = minValue;

                    if (chainDrawColor.G < minValue)
                        chainDrawColor.G = minValue;

                    if (chainDrawColor.B < minValue)
                        chainDrawColor.B = minValue;
                    */
                }
                else
                {   //在靠近球的地方，我们绘制了一个自定义链纹理，并以全亮度进行绘制。
                    chainTextureToDraw = chainTextureExtra;
                    //chainDrawColor = Color.White;   解开这个注释启用高亮发光
                }


                //在这里，我们在坐标处绘制链纹理
                Main.spriteBatch.Draw(chainTextureToDraw.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                //chainDrawPosition沿着向量前进，通过chainSegmentLength返回给player
                chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
                chainCount++;
                chainLengthRemainingToDraw -= chainSegmentLength;
            }

            // 向前移动时添加运动轨迹，就像大多数连枷一样（如果已经撞到瓷砖，不要添加轨迹）
            if (CurrentAIState == AIState.LaunchingForward)
            {
                Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
                Vector2 drawOrigin = new Vector2(projectileTexture.Width * 0.5f, Projectile.height * 0.5f);
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (Projectile.spriteDirection == -1)
                    spriteEffects = SpriteEffects.FlipHorizontally;
                for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++)
                {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.spriteBatch.Draw(projectileTexture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale - k / (float)Projectile.oldPos.Length / 3, spriteEffects, 0f);
                }
            }
            return true;
        }

    }
}
