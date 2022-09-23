using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Buffs.FriendlyBuffs
{
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
            npc.GetGlobalNPC<ExampleWhipDebuffNPC>().markedByExampleWhip = true;
        }


        public class ExampleWhipDebuffNPC : GlobalNPC
        {
            // This is required to store information on entities that isn't shared between them.
            // 这是存储非共享实体信息所必需的。
            public override bool InstancePerEntity => true;

            public bool markedByExampleWhip;

            public override void ResetEffects(NPC npc)
            {
                markedByExampleWhip = false;
            }

            // TODO: Inconsistent with vanilla, increasing damage AFTER it is randomised, not before. Change to a different hook in the future.
            //与香草不一致，随机化后增加伤害，而不是之前。将来换一个不同的钩子。
            public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
            {
                // Only player attacks should benefit from this buff, hence the NPC and trap checks.
                // 只有玩家的攻击才会从这个增益中受益，因此NPC和陷阱检查。
                if (markedByExampleWhip && !projectile.npcProj && !projectile.trap && (projectile.minion || ProjectileID.Sets.MinionShot[projectile.type]))
                {
                    Player player = Main.player[projectile.owner];

                    player.statLife += 1;
                    player.HealEffect(1);
                    damage += 5;

                    Vector2 pos = new Vector2(player.Center.X - 5, player.Center.Y - 5);
                    for (int i = 0; i < 2; i++)
                        Dust.NewDustDirect(pos, 10, 10, 107, 0, 0, 0, Color.White, 1);
                }
            }
        }
    }
}
