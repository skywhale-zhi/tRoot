using tRoot.Content.Projectiles;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Items.Weapons.Summoner
{
    internal class ChlorophyteWhip : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{   //这种方法可以快速设置鞭子的属性。或者看下面的写法
            //Item.DefaultToWhip(ModContent.ProjectileType<Projectiles.Summoner.ChlorophyteWhipProjectile>(), 70, 2, 7);

            #region 或者这样一步步设置
            Item.DamageType = DamageClass.SummonMeleeSpeed;
			Item.damage = 137;
			Item.knockBack = 2;
			Item.shoot = ModContent.ProjectileType<Projectiles.Summoner.ChlorophyteWhipProjectile>();
			//shootspeed不影响攻速，影响范围，默认 4
			Item.shootSpeed = 7;
			Item.useStyle = ItemUseStyleID.Swing;
			//下面俩影响攻速和范围，越小攻速越快范围越小，越大反之，默认30，（4，30搭配默认为1倍范围，比荆鞭略大，我认为的）
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.UseSound = SoundID.Item152; 
			Item.noMelee = true;
			Item.noUseGraphic = true;
            #endregion

            //下面两个是额外的
            Item.rare = ItemRarityID.Lime;
			Item.channel = false;
			Item.value = Item.sellPrice(0, 12, 0, 0);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.ChlorophyteBar,12)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}
