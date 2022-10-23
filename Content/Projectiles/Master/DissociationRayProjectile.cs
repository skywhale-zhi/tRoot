using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using tRoot.Common.GlobalNPCs;

namespace tRoot.Content.Projectiles.Master
{
    //解离射线
    internal class DissociationRayProjectile : ModProjectile
    {
        public override string Texture => "tRoot/Content/Projectiles/Other/Temp";
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 500;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 90;
        }


        public override void AI()
        {
            if (Projectile.alpha != 255)
                Projectile.alpha = 255;
            if (!Main.dedServ && Projectile.timeLeft < 490)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(5, 5), 1, 1, DustID.PurificationPowder, 0, 0, 0, Color.White, 1.2f);
                dust.noGravity = true;
                dust.velocity *= 0.1f;
            }
        }


        public override void Kill(int timeLeft)
        {
            base.Kill(timeLeft);
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //撞击，击中贴图引发的贴图效果，如方块灰尘等
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            // 如果弹丸击中瓷砖的左侧或右侧，则反转并调整X速度
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
            {
                Projectile.velocity.X = -1 * oldVelocity.X;

            }
            // 如果弹丸击中瓷砖的顶部或底部，则反转并调整Y速度
            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
            {
                Projectile.velocity.Y = -1 * oldVelocity.Y;
            }

            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustDirect(new Vector2(Projectile.Center.X - 4 - 10 / 4, Projectile.Center.Y - 4 - 10 / 4), 10, 10, DustID.PurificationPowder, 0, 0, 0, new Color(255, 255, 255) * 0.8f, 1.5f);
                dust.noGravity = true;
            }
            return false;
        }


        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
        }
    }
}
