using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace tRoot.Content.Items.Things
{
	//缓冲调和剂1
	internal class BufferBalancingAgentⅠ : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
		}

		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 30;
			Item.maxStack = 9999;
			Item.value = Item.buyPrice(0, 0, 1, 0);
			Item.rare = ItemRarityID.Green;
		}
	}
}
