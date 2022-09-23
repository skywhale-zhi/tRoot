using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace tRoot.Content.Projectiles.Warrior
{
    internal class ChlorophyteDaggerProjectile : ModProjectile
    {
        public const int FadeInDuration = 8;
        public const int FadeOutDuration = 3;

        public const int TotalDuration = 12;

        //刀刃的“宽度”
        public float CollisionWidth => 10f * Projectile.scale;

        public int Timer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("绿茎匕首");
        }

        public override void SetDefaults()
        {
            //这将宽度和高度设置为相同的值（当投射物可以旋转时很重要）
            Projectile.Size = new Vector2(40);
            //使用我们自己的 AI 来定制它的行为，如果你不想这样，请将其保存在 ProjAIStyleID.shortswice。不过，您仍然需要使用 SetVisualOffsets（）中的代码
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.DamageType = DamageClass.Melee;
            //防止打穿瓷砖。大多数使用射弹的近战武器都有这个
            Projectile.ownerHitCheck = true;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 360;
            //与 player.heldProj 一起使用时很重要。"Hidden" projectiles have special draw conditions
            Projectile.hide = true;

            Projectile.usesLocalNPCImmunity = true; // 用于ai挂钩中的命中冷却更改
            Projectile.localNPCHitCooldown = 12; // 这有助于自定义命中冷却逻辑

        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Timer += 1;
            if (Timer >= TotalDuration)
            {
                Projectile.Kill();
                return;
            }
            else
            {
                // 重要的是，使精灵图 “在” 玩家的手中，而不是完全在玩家的前面或后面
                player.heldProj = Projectile.whoAmI;

            }

            // Fade in and out
            // GetLerpValue returns a value between 0f and 1f - if clamped is true - representing how far Timer got along the "distance" defined by the first two parameters
            // The first call handles the fade in, the second one the fade out.
            // Notice the second call's parameters are swapped, this means the result will be reverted
            //GetLerpValue返回一个介于0f和1f之间的值-如果clamped为true-表示计时器Timer沿着前两个参数定义的“距离”走了多远
            //第一个调用处理淡入，第二个调用处理渐出。
            //注意，第二次调用的参数被交换，这意味着结果将被还原
            //Main.NewText("Utils.GetLerpValue(0f, FadeInDuration, Timer, clamped: true): " + Utils.GetLerpValue(0f, FadeInDuration, Timer, clamped: true));
            //Main.NewText("Utils.GetLerpValue(TotalDuration, TotalDuration - FadeOutDuration, Timer, clamped: true): " + Utils.GetLerpValue(TotalDuration, TotalDuration - FadeOutDuration, Timer, clamped: true));
            Projectile.Opacity = Utils.GetLerpValue(0f, FadeInDuration, Timer, clamped: true) * Utils.GetLerpValue(TotalDuration, TotalDuration - FadeOutDuration, Timer, clamped: true);

            // Keep locked onto the player, but extend further based on the given velocity (Requires ShouldUpdatePosition returning false to work)
            // 保持锁定玩家，但根据给定速度进一步扩展（需要 ShouldUpdatePosition 返回 false 才能工作）
            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, reverseRotation: false, addGfxOffY: false);
            //Vector2 playerCenter = player.Center;
            Projectile.Center = playerCenter + Projectile.velocity * (Timer - 1f);



            // 基于向左或向右移动设置spriteDirection。左-1，右1
            Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
            //Projectile.spriteDirection = (Vector2.Dot(Projectile.velocity, Vector2.UnitX) >= 0f).ToDirectionInt();

            // Point towards where it is moving, applied offset for top right of the sprite respecting spriteDirection
            // 指向其移动的位置，根据spriteDirection为sprite的右上角应用偏移
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 - MathHelper.PiOver4 * Projectile.spriteDirection;
            // The code in this method is important to align the sprite with the hitbox how we want it to
            // 此方法中的代码对于按我们希望的方式将sprite与hitbox对齐很重要
            SetVisualOffsets();
        }

        private void SetVisualOffsets()
        {
            //40是精灵图大小（此处宽度和高度相等）
            const int HalfSpriteWidth = (40 / 2);
            const int HalfSpriteHeight = (40 / 2);

            //int HalfProjWidth = Projectile.width / 2;
            //int HalfProjHeight = Projectile.height / 2;
            int HalfProjWidth = (Projectile.width / 2);
            int HalfProjHeight = Projectile.height / 2;

            // Vanilla configuration for "hitbox in middle of sprite"
            ////“精灵图中间的hitbox”的原版配置
            DrawOriginOffsetX = 0;
            DrawOffsetX = -(HalfSpriteWidth - HalfProjWidth);
            DrawOriginOffsetY = -(HalfSpriteHeight - HalfProjHeight);

            //if (Projectile.spriteDirection == 1)
            //{
            //    DrawOriginOffsetX = 18;//10
            //    DrawOffsetX = 38;
            //    DrawOriginOffsetY = 0;
            //}
            //else
            //{
            //    DrawOriginOffsetX = -18;
            //    DrawOffsetX = 0;
            //    DrawOriginOffsetY = 0;
            //}

        }


        // 此弹丸是否应根据其速度、是否在液体中等因素更新其位置。返回 false 使其速度对其位置没有影响。默认返回真。
        public override bool ShouldUpdatePosition()
        {
            // Update Projectile.Center manually
            return false;
        }


        //当射弹cutting tiles时代码运行。仅在CanCutTiles()返回 true 时运行。在对激光等进行编程时很有用。
        public override void CutTiles()
        {
            //“切砖”是指打碎花盆、草、蜂王幼虫等。
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity.SafeNormalize(-Vector2.UnitY) * 10f;
            Utils.PlotTileLine(start, end, CollisionWidth, DelegateMethods.CutTiles);
        }


        //允许您在此弹丸与该弹丸可能损坏的玩家或NPC之间使用自定义碰撞检测。适用于对角线激光、在其身后留下痕迹的射弹等。
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            // "Hit anything between the player and the tip of the sword"
            // shootSpeed is 2.1f for reference, so this is basically plotting 12 pixels ahead from the center
            //“击中玩家和剑尖之间的任何物体”
            // shootSpeed is 2.1f作为参考，因此这基本上是从中心向前绘制12个像素
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity * 6f;
            float collisionPoint = 0f; //不需要该变量，但需要作为参数
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, CollisionWidth, ref collisionPoint);
        }
    }
}
