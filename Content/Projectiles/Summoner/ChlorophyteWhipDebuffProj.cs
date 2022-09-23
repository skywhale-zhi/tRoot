using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Projectiles.Summoner
{
    //吸血弹幕
    internal class ChlorophyteWhipDebuffProj : ModProjectile
    {

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            DrawOffsetX = -7;
            DrawOriginOffsetY = -7;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;

            Projectile.friendly = false; // 伤害敌人？
            Projectile.hostile = false; // 伤害玩家？
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 1200;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 7;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Projectile.Center = Projectile.Center.MoveTowards(Main.player[Projectile.owner].Center, 2);
            if (!Main.dedServ)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center, 1, 1, DustID.TerraBlade, 0, 0, 0, Color.White, 0.7f);
                dust.velocity *= 0.15f;
                dust.noGravity = true;
            }
            if ((Projectile.Center - Main.player[Projectile.owner].Center).LengthSquared() < 260)
            {
                Main.player[Projectile.owner].statLife += (int)Projectile.ai[0];
                Main.player[Projectile.owner].HealEffect((int)Projectile.ai[0]);
                int count = 0;
                foreach (Projectile p in Main.projectile)
                {
                    if (p.active && p.type == Type && p.owner == Projectile.owner)
                    {
                        count++;
                    }
                }
                Projectile.Kill();
                if (count != 0 && Main.rand.NextBool(count / 4 > 0 ? count / 4 : 1) && !Main.dedServ)
                {
                    for (int i = 0; i < 25; i++)
                    {
                        Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                        Dust dust1 = Dust.NewDustPerfect(Main.player[Projectile.owner].Center, DustID.TerraBlade, speed * 5);
                        dust1.noGravity = true;
                    }
                }
            }
        }
    }
}
