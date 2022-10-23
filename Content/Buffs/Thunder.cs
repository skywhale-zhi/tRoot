using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace tRoot.Content.Buffs
{
    internal class Thunder : ModBuff
    {
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<ThunderNPC>().enable = true;
        }
        public class ThunderNPC : GlobalNPC
        {
            public override bool InstancePerEntity => true;

            //buff是否起效果中
            public bool enable;

            public override void ResetEffects(NPC npc)
            {
                enable = false;
            }

            
            public override void DrawEffects(NPC npc, ref Color drawColor)
            {
                if (enable)
                {
                    int count = (npc.frame.Width + npc.frame.Height) / 100;
                    float scale = (npc.frame.Width + npc.frame.Height * 1f) / (60 * 6f);
                    count = count > 0 ? count : 1;
                    scale = scale > 1 ? scale : 1;
                    scale = scale > 2 ? 2 : scale;
                    for(int i = 0;i < count; i++)
                    {
                        Dust.NewDustDirect(npc.Center - new Vector2(npc.frame.Width / 2, npc.frame.Height / 2), npc.frame.Width, npc.frame.Height, ModContent.DustType<Dusts.ElectrolysisDust>(), 0, 0, 0, Color.White, scale);
                    }
                    Lighting.AddLight(npc.Center, new Vector3(170, 240, 230) * 0.004f);
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
