using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace tRoot.Content.Items.Tools
{
    internal class BanDianHook : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.AmethystHook);
            Item.shootSpeed = 18f;
            Item.shoot = ModContent.ProjectileType<BanDianHookProjectile>();
        }
    }

    internal class BanDianHookProjectile : ModProjectile
    {
        private static Asset<Texture2D> chainTexture;

        public override void Load()
        { // 当加载此内容时，在mod（re）load上调用一次。
            chainTexture = ModContent.Request<Texture2D>("tRoot/Content/Items/Tools/BanDianHookChain");
        }

        public override void Unload()
        { //当卸载此内容时，这在mod reload时调用一次。
          //目前，像这样卸载静态字段非常重要，以避免在卸载时将部分mod保留在内存中。
            chainTexture = null;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(486);//复制血肉钩投射物的属性。
            Projectile.width = 18;
            Projectile.height = 20;
        }

        
        //使用这个钩子可以在飞行中甩出多个钩子，如 双钩、抛网钩、鱼钩、静态钩、月球钩。
        //每当玩家使用发射此类弹丸的抓钩时，都会调用此代码。用它来改变发射什么样的钩子（例如，双钩会这样做），杀死旧的钩子射弹等。
        public override bool? CanUseGrapple(Player player)
        {
            int hooksOut = 0;
            for (int l = 0; l < 1000; l++)
            {
                if (Main.projectile[l].active && Main.projectile[l].owner == Main.myPlayer && Main.projectile[l].type == Projectile.type && Main.projectile[l].velocity.Length() != 0)
                {
                    hooksOut++;
                }
            }

            return hooksOut <= 0;   //在空中最多发出 (hooksOut <= k)  => k+1 个狗子
        }

        
        //紫水晶挂钩是300，静态挂钩是600。范围
        public override float GrappleRange()
        {
            return 400f;
        }


        //在钩子开始消失之前，给定的玩家可以抓住多少个这种类型的抓钩。更改 numHooks 参数以确定这一点；默认情况下它将是 3。
        public override void NumGrappleHooks(Player player, ref int numHooks)
        {
            numHooks = 2; // 最多可以挂住的挂钩数量
        }


        // default is 11, Lunar is 24
        public override void GrappleRetreatSpeed(Player player, ref float speed)
        {
            speed = 18f; //未抓住物块，返回的速度
        }


        public override void GrapplePullSpeed(Player player, ref float speed)
        {
            speed = 10; //抓到物块，拖动玩家的速度
        }


        //绘制链条
        public override bool PreDrawExtras()
        {
            Vector2 playerCenter = Main.player[Projectile.owner].MountedCenter;
            Vector2 center = Projectile.Center;
            Vector2 directionToPlayer = playerCenter - Projectile.Center;

            float chainRotation = directionToPlayer.ToRotation() - MathHelper.PiOver2;
            float distanceToPlayer = directionToPlayer.Length();

            while (distanceToPlayer > Projectile.height && !float.IsNaN(distanceToPlayer))
            {
                directionToPlayer /= distanceToPlayer; // 获得单位向量
                directionToPlayer *= chainTexture.Height(); // 乘链条纹理的高
                center += directionToPlayer; // 更新绘画位置
                directionToPlayer = playerCenter - center; // 更新绘画距离
                distanceToPlayer = directionToPlayer.Length();

                //颜色亮度随环境而变
                Color drawColor = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16));

                Main.EntitySpriteDraw(chainTexture.Value, center - Main.screenPosition, chainTexture.Value.Bounds, drawColor, chainRotation, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            }
            //阻止原版绘画
            return false;
        }
    }
}
