using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System.IO;
using Terraria.Audio;
using System.Collections.Generic;
using tRoot.Common.GlobalNPCs;

namespace tRoot.Content.Projectiles.Warrior
{
    //汲血长枪射弹
    internal class BloodDrawingLanceProjectile : ModProjectile
    {
        //这些数据都需要放在ExtraAI里进行同步，因为ai[]不够用
        public Vector2 offPos;       //位置偏移
        public float offRor;         //角度偏移
        public float Ror;            //击中敌怪时记录角度偏移

        public override void SetStaticDefaults()
        {
            //拖尾幻影
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.knockBack = 6;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.timeLeft = 300;
            DrawOffsetX = 0;
            DrawOriginOffsetY = 0;
            DrawOriginOffsetX = -65;
            Projectile.penetrate = -1;
        }

        //ai0 == 0 普通平飞
        //ai0 == 1 黏附伤害
        //ai1 == 目标敌怪的索引
        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.rotation = Projectile.velocity.RotatedBy(MathHelper.PiOver4 * 3).ToRotation();
                //重力效果
                if (Projectile.timeLeft < 250 && Projectile.velocity.Y < 40)
                {
                    Projectile.velocity += Vector2.UnitY;
                }
            }
            if (Projectile.ai[0] == 1)
            {
                if (Main.npc[(int)Projectile.ai[1]].active)
                {
                    Projectile.Center = Main.npc[(int)Projectile.ai[1]].Center + offPos.RotatedBy(Main.npc[(int)Projectile.ai[1]].rotation - offRor);
                    Projectile.rotation = Main.npc[(int)Projectile.ai[1]].rotation - offRor + Ror;
                }
                //如果射中敌怪时，敌怪死了
                else
                {
                    Projectile.Kill();
                    Vector2 temp = Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver4);
                    if (!Main.dedServ)
                        for (int i = 0; i < 30; i++)
                        {
                            Dust.NewDustDirect(Projectile.Center, 20, 20, DustID.TheDestroyer, temp.X * Main.rand.Next(4, 15), temp.Y * Main.rand.Next(4, 15), 0, Color.White, 1.4f);
                        }

                    //生成回血射弹,如果距离近的话
                    if ((Main.player[Projectile.owner].Center - (temp * 210 + Projectile.Center)).LengthSquared() <= 2000 * 2000)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), temp + Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BloodDrawingLanceProjectile2>(), 0, 0, Projectile.owner, 1);
                }

                //黏附时按下右键爆炸，造成三倍（1 * 0.5 * 6 == 3）伤害，生成回血射弹
                if (Main.mouseRight && Main.myPlayer == Projectile.owner)
                {
                    int heal = 0;
                    if (Main.npc[(int)Projectile.ai[1]].CanBeChasedBy() || Main.npc[(int)Projectile.ai[1]].boss)
                    {
                        heal = (int)(Projectile.damage * 0.02) > 0 ? (int)(Projectile.damage * 0.02) : 1;
                    }
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BloodDrawingLanceProjectile2>(), 0, 0, Projectile.owner, heal);
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BloodDrawingLanceProjectile3>(), Projectile.damage * 6, Projectile.knockBack * 2, Projectile.owner);
                    Projectile.Kill();
                }
            }
            //淡入
            Projectile.alpha = (int)((1f - Projectile.timeLeft * 1.0f / 300f) * 255);
            //快死的时候生成粒子回血弹等
            if (Projectile.alpha == 230)
            {
                Vector2 temp = Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver4) * 220;
                if (!Main.dedServ)
                    for (int i = 0; i < 50; i++)
                    {
                        Dust dust = Dust.NewDustDirect(temp + Projectile.Center, 20, 20, DustID.TheDestroyer, 0, 0, 0, Color.White, 1.4f);
                        dust.velocity = Vector2.UnitY.RotatedBy(MathHelper.TwoPi / 50 * i) * 5f;
                        dust.noGravity = true;
                    }
                //生成回血射弹,如果距离近的话
                int heal = 0;
                if (Main.npc[(int)Projectile.ai[1]].CanBeChasedBy() || Main.npc[(int)Projectile.ai[1]].boss)
                {
                    heal = (int)(Projectile.damage * 0.02) > 0 ? (int)(Projectile.damage * 0.02) : 1;
                }
                if ((Main.player[Projectile.owner].Center - (temp + Projectile.Center)).LengthSquared() <= 2000 * 2000)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), temp + Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BloodDrawingLanceProjectile2>(), 0, 0, Projectile.owner, heal);
            }
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            /*
            //创建无视伤害减免
            double realdamage = Main.CalculateDamageNPCsTake(Projectile.damage, target.defense);
            target.GetGlobalNPC<ReduceDamageReduction>().enable = true;
            target.GetGlobalNPC<ReduceDamageReduction>().realDamage = realdamage;
            */

            //变成黏附弹
            if (Projectile.ai[0] == 0)
            {
                //如果是本地玩家
                if (Projectile.owner == Main.myPlayer)
                {
                    Ror = Projectile.rotation;
                    offPos = (Projectile.Center - target.Center) * 0.7f;
                    offRor = target.rotation;
                }
                if (!Main.dedServ)
                    SoundEngine.PlaySound(SoundID.Item66 with { Pitch = 1f }, Projectile.position);
                Projectile.ai[1] = target.whoAmI;
                Projectile.ai[0] = 1;
                Projectile.velocity = Vector2.Zero;
                Projectile.damage = (int)(damage * 0.5);
                Projectile.netUpdate = true;
            }
        }


        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Ror);
            writer.WriteVector2(offPos);
            writer.Write(offRor);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Ror = reader.ReadSingle();
            offPos = reader.ReadVector2();
            offRor = reader.ReadSingle();
        }


        public override bool PreDraw(ref Color lightColor)
        {
            //幻影纹理
            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new Vector2(projectileTexture.Width * 0.5f + DrawOriginOffsetX, Projectile.height * 0.5f);
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color;
                if (k == 0)
                {
                    color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                }
                else//次级残影不能过亮，这里 * 0.8f
                {
                    color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) * 0.8f / Projectile.oldPos.Length);
                }
                Main.spriteBatch.Draw(projectileTexture, drawPos, new Rectangle(0, 0, 180, 180), color, Projectile.rotation, drawOrigin, Projectile.scale - k / (float)Projectile.oldPos.Length / 3, spriteEffects, 0);
            }
            return false;
        }
    }


    ///************************************
    /// <summary>
    /// 回血射弹
    /// </summary>
    //ai[0] = 回血量
    internal class BloodDrawingLanceProjectile2 : ModProjectile
    {
        public override string Texture => "tRoot/Content/Projectiles/Other/Temp";
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3600;
            Projectile.extraUpdates = 8;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            if (Main.player[Projectile.owner].dead)
            {
                Projectile.Kill();
            }
            Projectile.alpha = 255;
            Projectile.Center = Projectile.Center.MoveTowards(Main.player[Projectile.owner].Center, 4);
            if (!Main.dedServ)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.TheDestroyer, 0, 0, 0, Color.White, 1f);
                dust.velocity *= 0.1f;
                dust.noGravity = true;
            }
            if (Vector2.DistanceSquared(Projectile.Center, Main.player[Projectile.owner].Center) < 10)
            {
                int heal = (int)Projectile.ai[0];
                if (Main.rand.NextBool(Projectile.CritChance, 100))
                {
                    heal *= 2;
                }
                if (heal >= 8)
                {
                    heal = 8;
                }
                Main.player[Projectile.owner].statLife += heal;
                Main.player[Projectile.owner].HealEffect(heal);
                Projectile.Kill();
                if (!Main.dedServ)
                    for (int i = 0; i < 20; i++)
                    {
                        Dust dust = Dust.NewDustDirect(Main.player[Projectile.owner].Center - new Vector2(Main.player[Projectile.owner].width / 2, Main.player[Projectile.owner].height / 2), Main.player[Projectile.owner].width, Main.player[Projectile.owner].height, DustID.LifeDrain, 0, 0, 0, Color.White, 1.5f);
                        dust.velocity *= 0.5f;
                    }
            }
        }
    }


    /// <summary>
    /// 爆炸效果弹
    /// </summary>
    internal class BloodDrawingLanceProjectile3 : ModProjectile
    {
        public override string Texture => "tRoot/Content/Projectiles/Other/Temp";
        public override void SetDefaults()
        {
            Projectile.width = 170;
            Projectile.height = 170;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            if(Projectile.alpha != 255)
                Projectile.alpha = 255;
        }

        public override void Kill(int timeLeft)
        {
            if (!Main.dedServ)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.Center, 20, 20, DustID.TheDestroyer, 0, 0, 0, Color.White, 2f);
                    dust.velocity = Vector2.UnitY.RotatedBy(MathHelper.TwoPi / 20 * i * Main.rand.NextFloat(-1.2f, 1.2f)) * 15 * Main.rand.NextFloat(0.2f, 1);
                    dust.noGravity = true;
                }
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            /*
            //创建无视伤害减免
            double realdamage = Main.CalculateDamageNPCsTake(Projectile.damage, target.defense);
            if(target.realLife != -1)
            {
                realdamage *= 0.8;
            }
            else
            {
                realdamage *= 1.1;
            }
            target.GetGlobalNPC<ReduceDamageReduction>().enable = true;
            target.GetGlobalNPC<ReduceDamageReduction>().realDamage = realdamage;
            */
        }
    }
}
