using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Buffs.Minions
{
    internal class LuminiteBladeBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			//当你离开这个世界时，这个buff是否不会保存？
			Main.buffNoSave[Type] = true;
			//剩余时间是否不会显示在此buff上？
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void ModifyBuffTip(ref string tip, ref int rare)
		{
			rare = ItemRarityID.Purple;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			// 如果仆从存在，重置增益时间，否则从玩家身上移除增益
			if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.Summoner.LuminiteBladeProjectile>()] > 0)
			{
				player.buffTime[buffIndex] = 3600;
			}
			else
			{
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}
}
