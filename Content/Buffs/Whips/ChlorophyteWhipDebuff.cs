using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using tRoot.Content.Projectiles.Summoner;

namespace tRoot.Content.Buffs.Whips
{
    //翠鞭叶绿根殖debuff
    internal class ChlorophyteWhipDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // 这允许对免疫所有减益的NPC施加减益。(如，邪教徒，毁灭者）
            // 其他MOD可能出于不同目的对其进行检查。
            BuffID.Sets.IsAnNPCWhipDebuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<ChlorophyteWhipDebuffNPC>().markedByChlorophyteWhip = true;
            npc.GetGlobalNPC<ChlorophyteWhipDebuffNPC>().bufftime = npc.buffTime[buffIndex];
        }


        public class ChlorophyteWhipDebuffNPC : GlobalNPC
        {
            // This is required to store information on entities that isn't shared between them.
            // 这是存储非共享实体信息所必需的。
            public override bool InstancePerEntity => true;

            public bool markedByChlorophyteWhip;
            public int bufftime;
            public override void ResetEffects(NPC npc)
            {
                markedByChlorophyteWhip = false;
            }

            // TODO: Inconsistent with vanilla, increasing damage AFTER it is randomised, not before. Change to a different hook in the future.
            //与香草不一致，随机化后增加伤害，而不是之前。将来换一个不同的钩子。
            public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
            {
                // Only player attacks should benefit from this buff, hence the NPC and trap checks.
                // 只有玩家的攻击才会从这个增益中受益，因此NPC和陷阱检查。
                if (markedByChlorophyteWhip && !projectile.npcProj && !projectile.trap && (projectile.minion || ProjectileID.Sets.MinionShot[projectile.type]))
                {
                    damage += 10;//300帧内，伤害 +10
                    int time = 240;
                    Player player = Main.player[projectile.owner];
                    //对回血射弹数目进行约束，免得回血太快
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<ChlorophyteWhipDebuffProj>()] < 5 && npc.CanBeChasedBy())
                    {
                        if (bufftime > time)//time帧内，吸血弹幕生成，10%额外暴击
                        {
                            if (Main.rand.Next(100) <= 10)
                            {
                                crit = true;
                                int heal = damage / 10 > 2 ? damage / 10 : 2;
                                heal = damage / 10 <= 5 ? damage / 10 : 5;
                                Projectile.NewProjectile(npc.GetSource_OnHit(player), npc.Center, Vector2.Zero, ModContent.ProjectileType<ChlorophyteWhipDebuffProj>(), 0, 0, player.whoAmI, heal);
                            }
                            else
                            {
                                Projectile.NewProjectile(npc.GetSource_OnHit(player), npc.Center, Vector2.Zero, ModContent.ProjectileType<ChlorophyteWhipDebuffProj>(), 0, 0, player.whoAmI, 1);
                            }
                        }
                    }
                }
            }

            public override void AI(NPC npc)
            {
                if (markedByChlorophyteWhip)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Dust dust = Dust.NewDustDirect(npc.Center - new Vector2(npc.width / 2, npc.height / 2), npc.width, npc.height, DustID.Chlorophyte, 0, 0, 185, Color.White, 1.7f);
                        dust.noGravity = true;
                    }
                }
                base.AI(npc);
            }
        }
    }
}
