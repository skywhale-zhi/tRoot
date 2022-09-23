using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace tRoot.Content.Pets.MiniFlower
{
    internal class MiniFlowerItem : ModItem
	{
		//花芽
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToVanitypet(ModContent.ProjectileType<MiniFlowerProjectile>(), ModContent.BuffType<MiniFlowerBuff>()); // Vanilla has many useful methods like these, use them! It sets rarity and value aswell, so we have to overwrite those after

			Item.width = 28;
			Item.height = 20;
			Item.rare = ItemRarityID.Yellow;
			Item.master = false; //这确保了“主”显示在工具提示中，因为稀有只会更改项目名称颜色
			Item.value = Item.buyPrice(0,5,0,0);
		}

        //public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        //{
        //    player.AddBuff(Item.buffType, 20); //该物品应用增益，增益产生射弹
        //    return false;
        //}

		//这种方法和上面那种都行，任选一种
        public override void UseStyle(Player player, Rectangle heldItemFrame)
		{
			if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
			{
				player.AddBuff(Item.buffType, 5);
			}
		}
	}
}
