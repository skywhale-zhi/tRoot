using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Common.GlobalProjectiles
{
    internal class BlueDeepSeaProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool enable = false;

        public override void Kill(Projectile projectile, int timeLeft)
        {
            if (enable)
            {
                int num = Main.rand.Next(3, 7);
                for (int j = 0; j < num; j++)
                {
                    Vector2 v = Vector2.UnitY.RotatedBy(MathHelper.TwoPi / num * j + Main.time % 3) * 20;
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, v, ProjectileID.Mushroom, (int)(projectile.damage * 0.3f), 1, projectile.owner);
                }
            }
            base.Kill(projectile, timeLeft);
        }
    }
}
