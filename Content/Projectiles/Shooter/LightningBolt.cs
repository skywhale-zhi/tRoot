using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Projectiles.Shooter
{
    //电云矢
    internal class LightningBolt : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            DrawOffsetX = 0;
            DrawOriginOffsetY = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 200;
            Projectile.ignoreWater = true;
            Projectile.scale *= 1f;
            Projectile.arrow = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.extraUpdates = 5;
        }

        public override void AI()
        {
            //Dust.NewDustDirect(Projectile.Center, 1, 1, ModContent.DustType<Dusts.ElectrolysisDust>(), 0, 0, 0, Color.White, 0.5f);

            if (Projectile.timeLeft < 170)
            {
                Vector2 speed = Projectile.velocity.SafeNormalize(Vector2.Zero) * 50;
                Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4, 4), 5, 5, DustID.Electric, speed.X, speed.Y, 0, Color.White, 0.5f);
                dust.noGravity = true;
            }
        }


        public override void Kill(int timeLeft)
        {
            Vector2 speed = Projectile.oldVelocity.SafeNormalize(Vector2.Zero) * 50;
            Dust dust = Dust.NewDustDirect(Projectile.Center - new Vector2(4, 4), 5, 5, DustID.Electric, speed.X, speed.Y, 0, Color.White, 2f);
            dust.noGravity = true;
        }

        public override void Load()
        {
            texture = ModContent.Request<Texture2D>("tRoot/Content/Projectiles/Shooter/LightningBolt", AssetRequestMode.ImmediateLoad).Value;
        }
        public override void Unload()
        {
            texture = null;
        }
        static Texture2D texture;
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            Rectangle sourceRectangle = new Rectangle(0, 0, 10, 24);

            int count = 20;
            if(Projectile.timeLeft > 170)
            {
                count = 0;
            }
            for (int k = 0; k < count; k++)
            {
                //Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin;
                Vector2 drawPos = Projectile.Center - Main.screenPosition + drawOrigin - Projectile.velocity.SafeNormalize(Vector2.Zero) * k * 5;

                //非高光需要吧rgb全都设置为0，高光则设置存在值即可
                Color color = new Color(new Vector4((count - k) * 1f / count));

                //淡出
                if (Projectile.timeLeft < 25)
                    color *= Projectile.timeLeft / 25f;

                Main.spriteBatch.Draw(
                    texture,
                    drawPos,
                    sourceRectangle,
                    color,
                    Projectile.velocity.RotatedBy(MathHelper.PiOver2).ToRotation(),
                    drawOrigin,
                    Projectile.scale - k * 1f / count / 2,
                    SpriteEffects.None,
                    0f
                    );
            }
            return false;
        }


        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        }
    }
}
