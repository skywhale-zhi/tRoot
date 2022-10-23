using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace tRoot.Content.Buffs
{
    internal class WeaponImbueMixedToxinⅠ : ModBuff
    {
        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsAFlaskBuff[ModContent.BuffType<WeaponImbueMixedToxinⅠ>()] = true;
        }

        public override void ModifyBuffTip(ref string tip, ref int rare)
        {
            rare = ItemRarityID.Lime;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<WeaponImbueMixedToxinⅠPlayer>().enable = true;
        }

        public class WeaponImbueMixedToxinⅠPlayer : ModPlayer
        {

            public bool enable;
            public override void ResetEffects()
            {
                enable = false;
            }

            //召唤鞭子给予效果
            public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
            {
                if (enable && (ProjectileID.Sets.IsAWhip[proj.type] || proj.DamageType == DamageClass.Melee))
                {
                    target.AddBuff(ModContent.BuffType<MixedToxinⅠ>(), Main.rand.Next(7, 15) * 60);
                }
            }

            //近战给予效果
            public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
            {
                if (enable)
                    target.AddBuff(ModContent.BuffType<MixedToxinⅠ>(), Main.rand.Next(7, 15) * 60);
            }
        }
    }
}
