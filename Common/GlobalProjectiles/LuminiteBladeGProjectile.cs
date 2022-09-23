using Terraria;
using Terraria.ModLoader;

namespace tRoot.Common.GlobalProjectiles
{
    //夜明之锋的伤害额外穿透计算
    internal class LuminiteBladeGProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool enable;     //是否起效果中

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (enable)
            {
                //破甲额外计算
                float armorp;
                armorp = Main.player[projectile.owner].GetArmorPenetration(DamageClass.Generic);
                if (target.ichor)
                {
                    armorp += 15;
                }
                if (target.betsysCurse)
                {
                    armorp += 40;
                }
                if (target.HasBuff(ModContent.BuffType<Content.Buffs.FriendlyBuffs.MixedToxinⅠ>()))
                {
                    armorp += 20;
                }
                damage += (int)(armorp * 0.5);

                //伤害减益计算
                if (target.lifeRegen < 0)
                {
                    int r = -(int)(target.lifeRegen * 0.15f);
                    if (r <= 100)
                    {
                        damage += r;
                    }
                    else
                    {
                        damage += 100;
                    }
                }
            }
        }
    }
}
