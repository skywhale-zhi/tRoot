using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace tRoot.Content.Pets.MiniFlower
{
    internal class MiniFlowerBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true;
			//Main.vanityPet[Type] = true; 普通宠物
			Main.lightPet[Type] = true; //照明宠物
		}

		public override void Update(Player player, ref int buffIndex)
		{   //此方法在玩家的buff激活的每一帧都会被调用。
			player.buffTime[buffIndex] = 5;

			int projType = ModContent.ProjectileType<MiniFlowerProjectile>();

			//如果玩家是本地玩家，并且还没有宠物投射物生成，则生成一个。
			if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[projType] <= 0)
			{
				var entitySource = player.GetSource_Buff(buffIndex);

				Projectile.NewProjectile(entitySource, player.Center, Vector2.Zero, projType, 0, 0f, player.whoAmI);
			}
		}
	}
}
