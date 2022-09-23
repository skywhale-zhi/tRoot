using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using tRoot.Content.Projectiles.Warrior;
using Terraria.GameContent.Creative;

namespace tRoot.Content.Items.Weapons.Warrior
{
    internal class HallowedMace : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.AddTranslation(7, "神圣链球");
			Tooltip.AddTranslation(7, "右键自动掷出，但略微降低伤害");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

			//此行将使工具提示中显示的伤害为实际Item.damage的两倍。该乘数用于调整弹丸的动态伤害情况。
			//当直接向敌人投掷时，连枷投射物造成Item.damage*2 伤害，与工具提示匹配，但在其他模式下造成1/2伤害。
			ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
		}

		public override void SetDefaults()
		{
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useAnimation = 45;
			Item.useTime = 45;
			Item.knockBack = 5.5f;
			Item.width = 28;
			Item.height = 34;
			Item.damage = 70;//50
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<HallowedMaceProjectile>();
			Item.shootSpeed = 12f;
			Item.UseSound = SoundID.Item1;
			Item.rare = ItemRarityID.Pink;
			Item.value = Item.sellPrice(0, 4, 80, 0);
			Item.DamageType = DamageClass.Melee;
			Item.channel = true;
			Item.noMelee = true;
			Item.scale = 1.2f;
			Item.autoReuse = true;
		}

        public override bool AltFunctionUse(Player player)
        {
        	return true;
        }

        public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.HallowedBar, 12)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}
