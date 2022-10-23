using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace tRoot.Content.Buffs
{
    internal class MixedToxinⅠ : ModBuff
    {
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<MixedToxinⅠNPC>().enable = true;
        }


        public class MixedToxinⅠNPC : GlobalNPC
        {
            public override bool InstancePerEntity => true;

            //buff是否起效果中
            public bool enable;

            public override void ResetEffects(NPC npc)
            {
                enable = false;
                npc.color = default;
            }

            public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
            {
                if (enable)
                {
                    damage += npc.checkArmorPenetration(20);
                }
                return base.StrikeNPC(npc, ref damage, defense, ref knockback, hitDirection, ref crit);
            }

            //在这里修改buff造成的颜色变化
            public override void DrawEffects(NPC npc, ref Color drawColor)
            {
                if (enable)
                {
                    if (Main.rand.NextBool(1))
                    {
                        Dust.NewDustDirect(npc.Center - new Vector2(npc.width / 2, npc.height / 2), npc.width, npc.height, ModContent.DustType<Dusts.MixedToxinⅠDust>(), 0, 0, 160, Color.White, 1.7f);
                    }
                    drawColor = new Color(242, 129, 94);
                }
            }


            //玩家掉血的话要在UpdateBadLifeRegen里
            public override void UpdateLifeRegen(NPC npc, ref int damage)
            {
                if (enable)
                {
                    //印象扣血速度
                    npc.lifeRegen -= 80;
                    if (damage == -1)
                        damage += 21;//影响扣血量
                    else
                        damage += 20;

                    //一般设置lifeRegen为damage的 4 倍就行
                }
            }
        }
    }
}
