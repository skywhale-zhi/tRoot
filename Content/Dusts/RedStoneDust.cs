using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace tRoot.Content.Dusts
{
    //红石粒子
    internal class RedStoneDust : ModDust
	{
        public override void OnSpawn(Dust dust)
		{
			dust.noGravity = false; // Makes the dust have no gravity.
			dust.noLight = true; // Makes the dust emit no light
		}

		public override bool Update(Dust dust)
		{
			dust.rotation += dust.velocity.X * 0.8f;
			dust.position += dust.velocity * 0.8f;
			dust.scale *= 0.9f;

			Lighting.AddLight(dust.position, dust.scale * 0.2f, 0, 0);

			if (dust.scale < 0.2f)
			{
				dust.active = false;
			}
			return false;
		}

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return base.GetAlpha(dust, lightColor);
        }
    }
}
