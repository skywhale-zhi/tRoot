using Terraria;
using Terraria.ModLoader;

namespace tRoot.Content.Dusts
{
	//调和毒素I的粒子
    internal class MixedToxinⅠDust : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.velocity *= 0.72f; // Multiply the dust's start velocity by 0.4, slowing it down
			dust.noGravity = true; // Makes the dust have no gravity.
			dust.noLight = true; // Makes the dust emit no light.
		}

		public override bool Update(Dust dust)
		{
			dust.position += dust.velocity;
			dust.rotation += dust.velocity.X * 0.25f;
			dust.scale *= 0.97f;

			Lighting.AddLight(dust.position, dust.scale * 1.1f, 0.57f * dust.scale * 1.1f, 0);

			if (dust.scale < 0.5f)
			{
				dust.active = false;
			}
			return false;
		}
	}
}
