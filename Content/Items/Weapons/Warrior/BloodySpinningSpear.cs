using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Items.Weapons.Warrior
{
    //血钻长矛
    internal class BloodySpinningSpear : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;

            Item.useStyle = ItemUseStyleID.Shoot;      //使用风格
            Item.useTime = 20;      //使用时间帧数，60帧一秒
            Item.useAnimation = 20;  //武器使用动画的时间跨度，建议将其设置为与使用时间相同。
            Item.autoReuse = true;     //是否自动挥舞

            Item.DamageType = DamageClass.Melee;    //武器伤害类型
            Item.damage = 40;
            Item.knockBack = 1.2f; //武器击退，最高20
            Item.crit = 5;      //武器暴击，默认玩家4%

            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Orange;      //武器罕见度
            Item.UseSound = SoundID.Item71;  //使用发出的声音
            Item.noUseGraphic = true;   //禁止使用自身的贴图，防止把自己贴图放进去

            Item.shootSpeed = 3.7f;
            Item.shoot = ModContent.ProjectileType<Projectiles.Warrior.BloodySpinningSpearProjectile>();
        }
    }
}
