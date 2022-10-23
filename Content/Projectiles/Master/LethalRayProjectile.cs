using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.ID;

namespace tRoot.Content.Projectiles.Master
{
    internal class LethalRayProjectile : ModProjectile
    {
        public override string Texture => "tRoot/Content/Projectiles/Other/Temp";
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 50; 
            Projectile.timeLeft = 1200; 
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 100;
        }

        public override void AI()
        {
            if (Projectile.alpha != 255)
                Projectile.alpha = 255;
            Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(5,5), 1, 1, DustID.PurificationPowder);
            dust.noGravity = true;
            dust.velocity *= 0;
            dust.shader = GameShaders.Armor.GetSecondaryShader(99, Main.LocalPlayer);
        }

        public override void Kill(int timeLeft)
        {
            base.Kill(timeLeft);
        }
    }
}
