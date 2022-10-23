using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using tRoot.Content.Projectiles.Warrior;

namespace tRoot.Common.GlobalNPCs
{
    //修改npc的伤害减免特性，该类最好在ModifyHitNPC里实现而不是在onhit里，因为后者的次序发生在伤害判定后
    public class ReduceDamageReduction : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        //是否启用？
        public bool enable;
        //应当的真实伤害
        public double realDamage = 0;
        //缩放比例
        public double scale = 1f;

        public int realLife;

        public override void ResetEffects(NPC npc)
        {
            enable = false;
            realDamage = 0;
            scale = 1f;
        }

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return false;
        }


        public override void OnSpawn(NPC npc, IEntitySource source)
        {
        }


        public override void SpawnNPC(int npc, int tileX, int tileY)
        {
        }

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            /*
            realLife = (int)(realLife - Main.CalculateDamageNPCsTake((int)damage, defense));
            npc.life = realLife;
            npc.CheckActive();
            npc.checkDead();
            npc.netUpdate = true;
            */
            return true;
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            /*
            if (projectile.type == ModContent.ProjectileType<BloodDrawingLanceProjectile>() || projectile.type == ModContent.ProjectileType<BloodDrawingLanceProjectile3>())
            {
                if(realLife == 0)
                {
                    realLife = npc.life;
                }
                double Damage;
                Damage = realDamage * scale * Main.rand.NextFloat(0.85f, 1.15f);
                if (crit)
                {
                    Damage *= 2;
                }
                //强制扣血
                damage = (int)Damage;
                realLife = (int)(realLife - Damage);
                npc.life = realLife;
                npc.checkDead();
                if(realLife <= 0)
                {
                    npc.active = false;
                    npc.netUpdate = true;
                }
            }
            */
        }
    }
}
