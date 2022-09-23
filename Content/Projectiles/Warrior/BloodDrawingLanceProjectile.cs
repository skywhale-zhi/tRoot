using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System.IO;
using Terraria.Audio;

namespace tRoot.Content.Projectiles.Warrior
{
    internal class BloodDrawingLanceProjectile : ModProjectile
    {
        //这些数据都需要放在ExtraAI里进行同步，因为ai[]不够用
        public Vector2 offPos;       //位置偏移
        public float offRor;                    //角度偏移
        public float Ror;                       //击中敌怪时记录角度偏移
        public bool enableM;        //是否修改伤害
        private int dam;            //伤害增长值

        public override void SetStaticDefaults()
        {
            //拖尾幻影
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override string Texture => "tRoot/Content/Projectiles/Warrior/BloodySpinningSpearProjectile";
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
                            Dust.NewDustDirect(Projectile.Center, 20, 20, 182, temp.X * Main.rand.Next(4, 15), temp.Y * Main.rand.Next(4, 15), 0, Color.White, 1.4f);
                        }
                    
                    //生成回血射弹,如果距离近的话
                    if ((Main.player[Projectile.owner].Center - (temp * 210 + Projectile.Center)).LengthSquared() <= 2000 * 2000)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), temp + Projectile.Center, (Main.player[Projectile.owner].Center - (temp + Projectile.Center)).SafeNormalize(Vector2.Zero), ModContent.ProjectileType<BloodDrawingLanceProjectile2>(), 0, 0, Projectile.owner, 1);
                }
            }
            Projectile.alpha = (int)((1f - Projectile.timeLeft * 1.0f / 300f) * 255);
            if (Projectile.alpha == 230)
            {
                Vector2 temp = Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver4) * 210;
                if (!Main.dedServ)
                    for (int i = 0; i < 50; i++)
                    {
                        Dust dust = Dust.NewDustDirect(temp + Projectile.Center, 20, 20, 182, 0, 0, 0, Color.White, 1.4f);
                        dust.velocity = Vector2.UnitY.RotatedBy(MathHelper.TwoPi / 50 * i) * 5f;
                        dust.noGravity = true;

                    }
                //生成回血射弹,如果距离近的话
                int heal = 0;
                if (Main.npc[(int)Projectile.ai[1]].CanBeChasedBy())
                {
                    heal = (int)(Projectile.damage * 0.02) > 0 ? (int)(Projectile.damage * 0.02) : 1;
                }
                if ((Main.player[Projectile.owner].Center - (temp + Projectile.Center)).LengthSquared() <= 2000 * 2000)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), temp + Projectile.Center, (Main.player[Projectile.owner].Center - (temp + Projectile.Center)).SafeNormalize(Vector2.Zero), ModContent.ProjectileType<BloodDrawingLanceProjectile2>(), 0, 0, Projectile.owner, heal);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //变成黏附弹
            if (Projectile.ai[0] == 0)
            {
                //如果是本地玩家
                if (Projectile.owner == Main.myPlayer)
                {
                    Ror = Projectile.rotation;
                    offPos = (Projectile.Center - target.Center) * 0.7f;
                    offRor = target.rotation;
                    SoundEngine.PlaySound(SoundID.Item66 with { Pitch = 1f }, Projectile.position);
                }

                Projectile.ai[1] = target.whoAmI;
                Projectile.ai[0] = 1;
                Projectile.velocity = Vector2.Zero;
                Projectile.damage = (int)(damage * 0.5);
                Projectile.netUpdate = true;

            }
            if (Projectile.ai[0] == 1)
            {
                int tdamage = (Projectile.damage * 2 - target.defense / 2) / 2;

                if (tdamage - damage > 20)
                {
                    dam += tdamage - damage;
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (Projectile.ai[0] == 1)
            {
                damage += dam;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Ror);
            writer.Write(offPos.X);
            writer.Write(offPos.Y);
            writer.Write(offRor);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Ror = reader.ReadSingle();
            offPos.X = reader.ReadSingle();
            offPos.Y = reader.ReadSingle();
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


    //ai[0] = 回血量
    internal class BloodDrawingLanceProjectile2 : ModProjectile
    {
        public override string Texture => "tRoot/Content/Projectiles/Warrior/BloodySpinningSpearProjectile";
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3600;
            Projectile.extraUpdates = 9;
            Projectile.alpha = 255;

        }

        public override void AI()
        {
            Projectile.alpha = 255;
            Projectile.Center = Projectile.Center.MoveTowards(Main.player[Projectile.owner].Center, 4);
            if (!Main.dedServ)
                for (int i = 0; i < 3; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.Center, 1, 1, 182, 0, 0, 0, Color.White, 1f);
                    dust.velocity *= 0.2f;
                    dust.noGravity = true;
                }
            if (Vector2.DistanceSquared(Projectile.Center, Main.player[Projectile.owner].Center) < 100)
            {
                int heal = (int)Projectile.ai[0];
                if (Main.rand.NextBool(Projectile.CritChance, 100))
                {
                    heal *= 2;
                }
                Main.player[Projectile.owner].statLife += heal;
                Main.player[Projectile.owner].HealEffect(heal);
                Projectile.Kill();
                if (!Main.dedServ)
                    for (int i = 0; i < 20; i++)
                    {
                        Dust dust = Dust.NewDustDirect(Main.player[Projectile.owner].Center - new Vector2(Main.player[Projectile.owner].width / 2, Main.player[Projectile.owner].height / 2), Main.player[Projectile.owner].width, Main.player[Projectile.owner].height, 235, 0, 0, 0, Color.White, 1.5f);
                        dust.velocity *= 0.5f;
                    }
                return;
            }
        }
    }
}
