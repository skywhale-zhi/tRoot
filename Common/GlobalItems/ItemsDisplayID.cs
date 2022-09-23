using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace tRoot.Common.GlobalItems
{
    internal class ItemsDisplayID : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return false;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            // Here we add a tooltip to the gel to let the player know what will happen
            tooltips.Add(new TooltipLine(Mod, "ItemIDs: ", $"【{item.type}】"));
        }
    }
}
