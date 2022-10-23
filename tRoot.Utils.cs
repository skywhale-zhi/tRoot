using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot
{
    public partial class tRoot : Mod
    {
        /// <summary>
        /// 返回当前 dis 距离内最近的敌对npc
        /// </summary>
        /// <param name="dis">搜索半径</param>
        /// <returns></returns>
        public static NPC getTargetNPC(Vector2 pos, int dis)
        {
            NPC targetnpc = null;
            float disS = dis * dis;
            foreach (NPC npc in Main.npc)
            {
                float temp = Vector2.DistanceSquared(npc.Center, pos);
                if (temp <= disS && npc.CanBeChasedBy())
                {
                    targetnpc = npc;
                    disS = temp;
                }
            }
            return targetnpc;
        }


        /// <summary>
        /// 返回对移动目标预判的射弹方向，针对匀速的预判
        /// </summary>
        /// <param name="pos">发射射弹的位置</param>
        /// <param name="targetNPC">目标npc</param>
        /// <param name="speed">射弹的速度</param>
        /// <param name="extraUpdates">射弹的额外更新率，如果没有则不填</param>
        /// <returns></returns>
        public static Vector2 AnticipatoryShooting(Vector2 pos, NPC targetNPC, float speed, int extraUpdates = 0)
        {
            Vector2 plrToNPC = targetNPC.Center - pos;
            float offset = plrToNPC.ToRotation();
            // 这就是我们推出来的公式
            float G = (plrToNPC.X * targetNPC.velocity.Y - plrToNPC.Y * targetNPC.velocity.X) / (speed * (extraUpdates + 1)) / plrToNPC.Length();
            if (G > 1 || G < -1)
            {
                //无法预判！
                return plrToNPC;
            }
            float realr = (float)(offset + Math.Asin(G));
            return realr.ToRotationVector2() * speed;
        }


        /// <summary>
        /// 获取从2020.1.1日到现在经过的秒数
        /// </summary>
        /// <returns></returns>
        public static long getNowTimeSecond()
        {
            DateTime centuryBegin = new DateTime(2020, 1, 1);
            long elapsedTicks = DateTime.Now.Ticks - centuryBegin.Ticks;
            return (elapsedTicks / 10000000L);
        }


        /// <summary>
        /// 获取从2020.1.1日到现在经过的嘀嗒数,60 tick == 1s
        /// </summary>
        /// <returns></returns>
        public static long getNowTimeTicks()
        {
            DateTime centuryBegin = new DateTime(2020, 1, 1);
            long elapsedTicks = DateTime.Now.Ticks - centuryBegin.Ticks;
            return (elapsedTicks / 166667L);
        }


        #region 自定义颜色类型

        public static readonly Color[] ItemNameColor1 = {
            new Color(255, 255, 255),
            new Color(0,255,255),
            new Color(0,255,0),
            new Color(255, 0 ,255)
        };

        #endregion


        /// <summary>
        /// 绘制神火粒子团
        /// </summary>
        /// <param name="pos">绘制地点</param>
        /// <param name="TailDir">焰尾摆动方向</param>
        /// <param name="width">粒子团宽</param>
        /// <param name="height">粒子团高</param>
        /// <param name="rate">生成率</param>
        public static void DrawSamadhiTrueFireDust(Vector2 pos, Vector2 TailDir, int width, int height, float rate = 1)
        {
            if (rate > 1) rate = 1;

            //最重色粒子
            if (Main.rand.NextBool(8) && Main.rand.NextBool((int)(rate * 100000), 100000))
            {
                Dust dust = Dust.NewDustDirect(pos - new Vector2(width / 2, height / 2), width, height, DustID.GemRuby, TailDir.X, TailDir.Y, 0, Color.White, 3f);
                dust.noGravity = true;
            }
            //坠落烟尘粒子
            if (Main.rand.NextBool(8) && Main.rand.NextBool((int)(rate * 100000), 100000))
            {
                Dust.NewDustDirect(pos - new Vector2(width / 2, height / 2), width, height, DustID.Flare, Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi).X * 2, Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi).Y * 2, 0, Color.White, 2);
            }
            //升腾烟雾重色
            if (Main.rand.NextBool(4) && Main.rand.NextBool((int)(rate * 100000), 100000))
            {
                Dust dust = Dust.NewDustDirect(pos - new Vector2(width / 2, height / 2), width, height, DustID.LifeDrain, TailDir.X * 2, TailDir.Y * 4, 0, Color.White, 3.5f);
                dust.noGravity = true;
                dust.shader = GameShaders.Armor.GetSecondaryShader(81, Main.LocalPlayer);
            }
            //浅色升腾粒子
            if (Main.rand.NextBool(4) && Main.rand.NextBool((int)(rate * 100000), 100000))
            {
                Dust dust = Dust.NewDustDirect(pos - new Vector2(width / 2, height / 2), width, height, DustID.LifeDrain, TailDir.X * 3, TailDir.Y * 7, 0, Color.White, 3.5f);
                dust.shader = GameShaders.Armor.GetSecondaryShader(83, Main.LocalPlayer);
                dust.noGravity = true;
                dust.velocity.X *= 0.5f;
            }
            //神圣粒子
            if (Main.rand.NextBool(6) && Main.rand.NextBool((int)(rate * 100000), 100000))
            {
                Vector2 temp = Vector2.UnitX.RotateRandom(MathHelper.Pi);
                Dust.NewDustDirect(pos - new Vector2(width / 2, height / 2), width, height, DustID.TreasureSparkle, temp.X, temp.Y, 0, Color.White, 2f);
            }
        }
    }
}
