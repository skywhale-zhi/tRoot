using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using System.IO;

namespace tRoot.Content.Items.Tools
{
    internal class ChlorophyteFishingRod : ModItem
    {
        public override void SetStaticDefaults()
        {
            //允许杆子在熔岩中捕鱼，这里是false
            ItemID.Sets.CanFishInLava[Item.type] = false;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            //These are copied through the CloneDefaults method:
            Item.width = 44;
            Item.height = 48;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 8;
            Item.useTime = 8;
            Item.UseSound = SoundID.Item1;
            // Item.CloneDefaults(ItemID.WoodFishingPole);

            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(0, 12, 0, 0);
            Item.fishingPole = 40; // 渔利
            Item.shootSpeed = 14f; // Wooden Fishing Pole is 9f and Golden Fishing Rod is 17f.
            Item.shoot = ModContent.ProjectileType<ChlorophyteFishingRodBobber>();
        }


        //如果持有物品，则授予高测试钓鱼线bool。
        //注意：仅通过热键触发，如果您在库存之外用手握住物品，则不会触发。
        public override void HoldItem(Player player)
        {
            player.accFishingLine = true;
        }

        // Overrides the default shooting method to fire multiple bobbers.
        // NOTE: This will allow the fishing rod to summon multiple Duke Fishrons with multiple Truffle Worms in the inventory.
        //覆盖默认的放炮方法以发射多个Bobber。
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int bobberAmount = 2;
            float spreadAmount = 75f; //不同的筒子散开了多少。

            PlayerFishingConditions f = player.GetFishingConditions();
            if (ItemID.Sets.SortingPriorityBossSpawns[f.BaitItemType] >= 0)//如果是召唤物，则掷出一只鱼钩
            {
                bobberAmount = 1;
            }
            for (int index = 0; index < bobberAmount; ++index)
            {
                Vector2 bobberSpeed = velocity + new Vector2(Main.rand.NextFloat(-spreadAmount, spreadAmount) * 0.05f);
                Projectile.NewProjectile(source, position, bobberSpeed, type, 0, 0f, player.whoAmI);
            }
            return false;
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 12)
                .AddIngredient(ItemID.WoodFishingPole, 1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }


    //鱼漂
    public class ChlorophyteFishingRodBobber : ModProjectile
    {
        public static readonly Color[] PossibleLineColors = new Color[] {
            new Color(61, 61, 61),
            new Color(113, 21, 18)
        };

        // 这将在 PossibleLineColors 数组中保存钓鱼线颜色的索引。
        private int fishingLineColorIndex;

        private Color FishingLineColor => PossibleLineColors[fishingLineColorIndex];

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            // These are copied through the CloneDefaults method
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = 61;
            Projectile.bobber = true;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            // Projectile.CloneDefaults(ProjectileID.BobberWooden);

            DrawOriginOffsetY = -8; // Adjusts the draw position
        }


        public override void OnSpawn(IEntitySource source)
        {
            //通过从PossibleLineColors数组中获取随机项的索引来确定极点的颜色。
            fishingLineColorIndex = (byte)Main.rand.Next(PossibleLineColors.Length);
        }


        //如果我们想随机化线条颜色呢
        public override void AI()
        {
            // 始终确保图形相关代码不会通过此检查在专用服务器( dedicated servers )上运行。
            //但是addlight已经排除了dedServ 也就是 netMode==2 所以这里写是没有意义的。粒子也遵循同样道理
            //if (!Main.dedServ)
            Lighting.AddLight(Projectile.Center, FishingLineColor.ToVector3());
        }


        public override void ModifyFishingLine(ref Vector2 lineOriginOffset, ref Color lineColor)
        {
            // Change these two values in order to change the origin of where the line is being drawn.
            // This will make it draw 47 pixels right and 31 pixels up from the player's center, while they are looking right and in normal gravity.
            //更改这两个值以更改绘制线的原点。
            //这将使它向右绘制47个像素，从玩家的中心向上绘制31个像素，同时他们在正常重力下向右看。
            lineOriginOffset = new Vector2(47, -31);
            // Sets the fishing line's color. Note that this will be overridden by the colored string accessories.
            // 设置钓鱼线的颜色。请注意，这将被彩色字符串附件覆盖。
            lineColor = FishingLineColor;
        }


        // 这最后两种方法是必需的，以便在多人游戏中正确同步线条颜色。
        // 如果您将 AI 信息存储在 projectile.ai 数组之外，请使用它在客户端和服务器之间发送该 AI 信息。
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((byte)fishingLineColorIndex);
        }


        // 使用它来接收在 SendExtraAI 中发送的信息。
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            fishingLineColorIndex = reader.ReadByte();
        }
    }

}
