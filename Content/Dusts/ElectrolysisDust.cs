using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace tRoot.Content.Dusts
{
    internal class ElectrolysisDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.frame = new Rectangle(0, Main.rand.Next(6) * 22, 22, 22);
            dust.color = color[Main.rand.Next(color.Length)];
        }

        private readonly Color[] color =
        {
            new Color(255,255,255),
            new Color(0,255,255),
            new Color(54,160,246),
            new Color(39,51,117),
            new Color(68,184,184)
        };

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.velocity *= 0.9f;
            dust.scale -= 0.02f;
            dust.rotation = dust.velocity.ToRotation();
            if (dust.scale < 0.2f)
                dust.active = false;
            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return Color.White * (dust.velocity * 0.8f).LengthSquared();
        }
    }
}
