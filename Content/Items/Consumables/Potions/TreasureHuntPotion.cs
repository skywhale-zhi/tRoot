using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace tRoot.Content.Items.Consumables.Potions
{
	public class TreasureHuntPotion : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.AddTranslation(7, "寻宝药水");
			Tooltip.AddTranslation(7, "提供勘探宝藏、发光、增加挖掘速度");

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
			Item.maxStack = 99;
			Item.consumable = true;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.buyPrice(gold: 10);
			Item.buffTime = 36000;
			//药水的buff，这个必须有，否则B键不支持
			Item.buffType = BuffID.Shine;
			//Item.buffType = ModContent.BuffType<Buffs.ExampleDefenseBuff>();
			//告诉系统这个属于治疗药水类，显然这里是false，目的区分buff药水和治疗药水的区别
			Item.potion = false;
			//治疗多少血量，搭配上一句会获得抗药性debuff
			//Item.healLife = 150;
		}

        // 当物品使用的时候触发，如果是用这个方法给物品添加使用buff，必须为true，保证为消耗品
        public override bool? UseItem(Player player)
        {
            player.AddBuff(BuffID.Shine, 36000);        //发光
            player.AddBuff(BuffID.Mining, 36000);       //挖矿速度
            player.AddBuff(BuffID.Spelunker, 36000);    //洞穴探险

            return true;
        }

        public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.ShinePotion, 1)
				.AddIngredient(ItemID.MiningPotion, 1)
				.AddIngredient(ItemID.SpelunkerPotion, 1)
				.AddTile(TileID.AlchemyTable)
				.Register();
			CreateRecipe()
				.AddIngredient(ItemID.BottledWater,1)
				.AddIngredient(ItemID.Daybloom, 1)
				.AddIngredient(ItemID.GlowingMushroom, 1)
				.AddIngredient(ItemID.Blinkroot, 2)
				.AddIngredient(ItemID.Moonglow, 1)
				.AddIngredient(ItemID.GoldOre, 1)
				.AddIngredient(ItemID.AntlionMandible, 1)
				.AddTile(TileID.AlchemyTable)
				.Register();
			CreateRecipe()
				.AddIngredient(ItemID.BottledWater,1)
				.AddIngredient(ItemID.Daybloom, 1)
				.AddIngredient(ItemID.GlowingMushroom, 1)
				.AddIngredient(ItemID.Blinkroot, 2)
				.AddIngredient(ItemID.Moonglow, 1)
				.AddIngredient(ItemID.PlatinumOre, 1)
				.AddIngredient(ItemID.AntlionMandible, 1)
				.AddTile(TileID.AlchemyTable)
				.Register();
		}
	}
}