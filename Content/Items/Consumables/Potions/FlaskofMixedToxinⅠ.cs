using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace tRoot.Content.Items.Consumables.Potions
{
    public class FlaskofMixedToxinⅠ : ModItem
	{
		//混调毒素瓶
		public override void SetStaticDefaults()
		{
			//喝药水时漏下来的口水粒子效果（真的有人注意到这个吗）
			ItemID.Sets.DrinkParticleColors[Type] = new Color[3] {
				new Color(11, 125, 0),
				new Color(0, 179, 49),
				new Color(0, 102, 28)
			};

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 20;
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 26;
			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useAnimation = 15;
			Item.useTime = 15;
			Item.useTurn = true;
			Item.UseSound = SoundID.Item3;
			Item.maxStack = 9999;
			Item.consumable = true;
			Item.rare = ItemRarityID.Pink;
			Item.value = Item.buyPrice(gold: 10);
			Item.buffTime = 72000;
			//药水的buff，这个必须有，否则B键不支持
			Item.buffType = ModContent.BuffType<Buffs.WeaponImbueMixedToxinⅠ>();
			//告诉系统这个属于治疗药水类，显然这里是false，目的区分buff药水和治疗药水的区别
			Item.potion = false;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.BottledWater,1)
				.AddIngredient(ItemID.Ichor, 2)
				.AddIngredient(ItemID.CursedFlame, 2)
				.AddIngredient(ItemID.VialofVenom, 5)
				.AddIngredient(ModContent.ItemType<Things.BufferBalancingAgentⅡ>(),1)
				.AddTile(TileID.ImbuingStation)
				.Register();
		}
	}
}