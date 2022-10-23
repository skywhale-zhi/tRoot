using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Buffs
{
    //三昧真火
    internal class SamadhiTrueFire : ModBuff
    {
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<SamadhiTrueFireNPC>().enable = true;
        }

        public class SamadhiTrueFireNPC : GlobalNPC
        {
            public override bool InstancePerEntity => true;

            //buff是否起效果中
            public bool enable;

            public override void ResetEffects(NPC npc)
            {
                enable = false;
            }

            //在这里修改buff造成的颜色变化和粒子
            public override void DrawEffects(NPC npc, ref Color drawColor)
            {
                if (enable)
                {
                    if (npc.realLife != -1)
                        tRoot.DrawSamadhiTrueFireDust(npc.Center, new Vector2(0, -1), npc.width / 4, npc.height / 4, 0.3f);
                    else
                        tRoot.DrawSamadhiTrueFireDust(npc.Center, new Vector2(0, -1), npc.width / 4, npc.height / 4, 1f);
                    Lighting.AddLight(npc.Center, new Vector3(240, 230, 170) * 0.004f);
                }
            }


            //玩家掉血的话要在UpdateBadLifeRegen里
            public override void UpdateLifeRegen(NPC npc, ref int damage)
            {
                if (enable)
                {
                    int num = npc.lifeMax / 2000;
                    if (npc.realLife != -1)
                    {
                        num = (int)(num * 0.1);
                    }
                    if (num < 60)
                    {
                        num = 60;
                    }
                    else if (num > 1000)
                    {
                        num = 1000;
                    }

                    //影响扣血速度
                    npc.lifeRegen -= num * 4;
                    if (damage == -1)
                        damage += num + 1;//影响扣血量
                    else
                        damage += num;
                    //一般设置lifeRegen为damage的 4 倍就行，这样做满足正常扣血的速度显示
                }
            }
        }
    }
}
