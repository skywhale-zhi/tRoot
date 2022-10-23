using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using tRoot.Content.Buffs.Whips;

namespace tRoot.Content.Projectiles.Summoner
{
    internal class ChlorophyteWhipProjectile : ModProjectile
	{
		private Vector2 oldDustPos = Vector2.Zero;

		public override void SetStaticDefaults()
		{
			// This makes the projectile use whip collision detection and allows flasks to be applied to it.
			//这使得弹丸使用鞭状碰撞检测，并允许对其应用烧瓶。(奇怪的翻译)
			ProjectileID.Sets.IsAWhip[Type] = true;
		}

		public override void SetDefaults()
		{
            //这种方法可以快速设置鞭子的属性，包括AI
            //也就是说，如果你不打算用这个快捷方法，那么就需要重写AI函数
            //Projectile.DefaultToWhip();

            #region 或者这样一步步设置
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true; // 这可以防止弹丸穿墙伤害怪物。
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
			Projectile.DamageType = DamageClass.SummonMeleeSpeed;
			#endregion

			//使用这些更改常规默认值（必需的）
			Projectile.WhipSettings.Segments = 17;		//鞭子节数量
			Projectile.WhipSettings.RangeMultiplier = 1f;	//放大乘数
		}

		private float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}


		//如果你用了 Projectile.DefaultToWhip();那么可以把这个重写删掉
		public override void AI()
		{
			//鞭子伤害衰减，这里是联用全局类的写法，这里不需要，因为可以在onhitnpc方法中轻松实现
			//WhipDamageAttenuation globalProj = Projectile.GetGlobalProjectile<WhipDamageAttenuation>();
			//globalProj.enable = true;

			//召唤物鞭子暴击修正，这个不能放在SetDefaults()，因为无效
			Projectile.CritChance = 0;
			Player owner = Main.player[Projectile.owner];
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Projectile.Center = Main.GetPlayerArmPosition(Projectile) + Projectile.velocity * Timer;
            Projectile.spriteDirection = Projectile.velocity.X >= 0f ? 1 : -1;

            Timer++;
            float swingTime = owner.itemAnimationMax * Projectile.MaxUpdates;
            if (Timer >= swingTime || owner.itemAnimation <= 0)
            {
                Projectile.Kill();
                return;
            }

            owner.heldProj = Projectile.whoAmI;


            if (Timer == swingTime / 2)
            {
                // 在鞭子尖发出鞭痕声。
                List<Vector2> points = Projectile.WhipPointsForCollision;
                Projectile.FillWhipControlPoints(Projectile, points);
                SoundEngine.PlaySound(SoundID.Item153, points[points.Count - 1]);
            }
        }


        #region 蓄力的函数，两种都行，选择一个解注释
        // 如果你不想要这个功能，需要移除 Item.channel = true 从这个物品的类文件里
        // 如果蓄力满了返回true
		/*
        private bool Charge(Player owner)
		{
			//与其他鞭子一样，该鞭子每帧更新两次（extraUpdates=1），因此120等于1秒。
			if (!owner.channel || ChargeTime >= 60)
			{
				return true; // finished charging
			}

			ChargeTime++;

			if (ChargeTime % 12 == 0) // 1 segment per 12 ticks of charge.
				Projectile.WhipSettings.Segments++;

			// 蓄力满时提高范围
			Projectile.WhipSettings.RangeMultiplier += 1 / 120f;

			// 蓄力时重置动画和项目计时器。
			owner.itemAnimation = owner.itemAnimationMax;
			owner.itemTime = owner.itemTimeMax;

			return false; // 仍在蓄力
		}
		*/
		/*
        // 本例使用PreAI实现蓄力攻击。
        // 如果你不想要这个功能，需要移除 Item.channel = true 从这个物品的类文件里
        public override bool PreAI()
        {
            Player owner = Main.player[Projectile.owner];

            //与其他鞭子一样，该鞭子每帧更新两次(Projectile.extraUpdates = 1)，因此120帧等于1秒。
            if (!owner.channel || ChargeTime >= 120)
            {
                return true; // 启用原版AI
            }

            if (++ChargeTime % 12 == 0) // 1 segment per 12 ticks of charge.
                Projectile.WhipSettings.Segments++;

            //将射程增加到2倍以实现蓄力完全。
            Projectile.WhipSettings.RangeMultiplier += 1 / 120f;

            // Reset the animation and item timer while charging.
            // 蓄力时重置动画和项目计时器。
            owner.itemAnimation = owner.itemAnimationMax;
            owner.itemTime = owner.itemTimeMax;

            return false; // 阻止原版AI运行
        }
		*/
        #endregion


        //击中敌人给与buff和伤害衰减
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(ModContent.BuffType<ChlorophyteWhipDebuff>(), 300);
			Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
			//显然可见，在这个方法执行的时候，伤害已经打出，所以*0.8不会在第一次攻击显示出来
			Projectile.damage = (int)(damage * 0.80f);
		}


		//这种方法在鞭子的所有点之间画一条线，以防精灵图之间有空白。
		private void DrawLine(List<Vector2> list)
		{
			//竟然是用钓鱼线做的。。。
			Texture2D texture = TextureAssets.FishingLine.Value;
			Rectangle frame = texture.Frame();
			Vector2 origin = new Vector2(frame.Width / 2, 2);

			Vector2 pos = list[0];
			for (int i = 0; i < list.Count - 2; i++)//我微调了下-2，原版是-1
			{
				Vector2 element = list[i];
				Vector2 diff = list[i + 1] - element;

				float rotation = diff.ToRotation() - MathHelper.PiOver2;
				Color color = Lighting.GetColor(element.ToTileCoordinates(), Color.White);
				Vector2 scale = new Vector2(1, (diff.Length() + 2) / frame.Height);

				Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, new Color(152, 4, 0), rotation, origin, scale, SpriteEffects.None, 0);

				pos += diff;
			}
		}
		

		//绘制鞭子节
		public override bool PreDraw(ref Color lightColor)
		{
			List<Vector2> list = new List<Vector2>();
			//给list放入Projectile.WhipSettings.Segments = 26;的数目，但是list.count为 26+1
			Projectile.FillWhipControlPoints(Projectile, list);

			//画串连起来的线
			DrawLine(list);

			//Main.DrawWhip_WhipBland(Projectile, list);
			// 以下代码用于自定义绘图。
			// 如果你不想这样，你可以把它全部删除，改为调用原版的DrawWhip方法之一，如上所述。
			// 然而，如果你这样做，你必须坚持他们的画法。

			SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			Main.instance.LoadProjectile(Type);
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			//下表0是鞭手端，count-2 是鞭尖，因为count = 26+1
			Vector2 pos = list[0];

			for (int i = 0; i < list.Count - 1; i++)
			{
				// 截取矩形（0，0），长26，宽10。在Main.EntitySpriteDraw里代表从纹理上截取该尺寸的方块，这里应该是鞭手端所以比较长，为26
				Rectangle frame = new Rectangle(0, 0, texture.Width, 26);
				// 绘制中心点
				Vector2 origin = new Vector2(texture.Width/2, 8);
				float scale = 1f;

				// 这里是绘制鞭尖部分，长18
				if (i == list.Count - 2)
				{
					frame.Y = 74;
					frame.Height = 24;
					// 为了获得更具冲击力的外观，这会在完全伸展时向上缩放鞭尖，在卷曲时向下缩放鞭尖。
					Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
					float t = Timer / timeToFlyOut;
					scale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));

					//粒子
					//for (int n = 0; n < 1; n++)
					{
						if(oldDustPos == Vector2.Zero)
                        {
							oldDustPos = list[i];
							break;
                        }
						Vector2 dustPos = (list[i] + oldDustPos) / 2f;
						dustPos += (dustPos - Main.player[Projectile.owner].Center).SafeNormalize(Vector2.Zero) * 45f;
						Dust dust = Dust.NewDustDirect(dustPos, 20, 20, DustID.Chlorophyte, 0f, 0f, 50, default(Color), 1.25f);
						dust.noGravity = true;
						oldDustPos = list[i];
					}
				}
				else if (i > 10)//绘制鞭节长含第12节往后
				{
					frame.Y = 58;
					frame.Height = 16;
				}
				else if (i > 5)//绘制鞭节长含第7节往后
				{
					frame.Y = 42;
					frame.Height = 16;
				}
				else if (i > 2)//绘制鞭节长含第4节往后
				{
					frame.Y = 26;
					frame.Height = 16;
				}
				else if (i > 0)//绘制鞭节长含第2节往后
				{
					frame.Y = 0;
					frame.Height = 26;
				}

				Vector2 element = list[i];
				Vector2 diff = list[i + 1] - element;

				float rotation = diff.ToRotation() - MathHelper.PiOver2; //该弹丸的精灵面朝下，因此PiOver2用于纠正旋转。This projectile's sprite faces down, so PiOver2 is used to correct rotation.
				Color color = Lighting.GetColor(element.ToTileCoordinates());

				Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);

				pos += diff;
			}
			return false;
		}
	}
}
