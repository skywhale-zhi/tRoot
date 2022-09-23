using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace tRoot.Content.NPCs.Person_Pets
{
    //自动加载一个NPC的小地图图标，加入这句就要添加一个npc头部图片，命名为 类名_Head.png
    [AutoloadHead]
    internal class Scientist : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 25; //NPC拥有的帧数
                                           //默认  +12走路帧  +9椅子猜拳等帧  +4攻击帧（根据职业不同会变），总共25帧，
            NPCID.Sets.ExtraFramesCount[Type] = 9; //一般来说，对于城镇NPC来说，这是NPC做额外事情的方式，比如坐在椅子上，与其他NPC交谈。
            NPCID.Sets.AttackFrameCount[Type] = 4;  //攻击动作的帧数，射手填5，战士和投掷填4，法师填2
            NPCID.Sets.DangerDetectRange[Type] = 700; //npc试图攻击敌人的距离
            NPCID.Sets.AttackType[Type] = 0;    //攻击方式，投掷0、射击1、魔法2
            NPCID.Sets.AttackTime[Type] = 60; //NPC的攻击动画启动后结束所需的时间。
            NPCID.Sets.AttackAverageChance[Type] = 30; //攻击优先度，越大越容易逃跑，越小越好斗
            NPCID.Sets.HatOffsetY[Type] = 4; //因为当一个派对处于活动状态时，派对帽在Y偏移处生成。
                                             //如果该NPC属于法师类，你可以加上这个来改变NPC的魔法光环颜色，这里用紫色
                                             //NPCID.Sets.MagicAuraColor[Type] = Color.Pink;

            //修改NPC在生物图鉴中的形象
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Velocity = 1f,      //npc在图鉴里的奔跑速度
                Direction = -1      //npc在图鉴里面朝哪里
                                    //如果您想看到绘制NPC时手动修改这些的示例，请参阅PreDraw
            };

            //设置npc在图鉴的效果
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

            //npc快乐系统
            NPC.Happiness
                .SetBiomeAffection<ForestBiome>(AffectionLevel.Like) // 喜欢森林
                .SetBiomeAffection<DesertBiome>(AffectionLevel.Dislike) // 讨厌沙漠
                                                                        //.SetNPCAffection(NPCID.Guide, AffectionLevel.Love) // Loves living near the 向导
                .SetNPCAffection(NPCID.Guide, AffectionLevel.Like) // Likes living near the guide.
                .SetNPCAffection(NPCID.Merchant, AffectionLevel.Dislike) // Dislikes living near the merchant.
                .SetNPCAffection(NPCID.Angler, AffectionLevel.Hate) // Hates living near the 渔夫
            ;   //别把分号忘了
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true; // Sets NPC to be a Town NPC
            NPC.friendly = true; // NPC Will not attack player
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = 7;
            NPC.damage = 10;    //伤害
            NPC.defense = 15;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;//击退抗性，0完全抵抗
                                       //动画类型
            AnimationType = NPCID.Guide;
        }


        //设置图鉴
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            //为了一次添加多个项目，我们可以使用AddRange而不是多次调用Add
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				//设置该镇NPC的首选生物群落，并将其列在最佳列表中。
				//对于城镇NPC，你通常将其设置为NPC最喜欢的生物群落。
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
				//在图鉴中设置NPC文本。
				new FlavorTextBestiaryInfoElement("Mods.tRoot.Bestiary.Scientist")
            });
        }


        //PreDraw在绘制sprite之前绘制东西或在绘制sprite之前运行代码非常有用
        //返回false将允许您手动绘制NPC
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // (simply checking NPC.IsABestiaryIconDummy and incrementing NPC.Rotation won't work here as it gets overridden by drawModifiers.Rotation each tick)
            if (NPCID.Sets.NPCBestiaryDrawOffset.TryGetValue(Type, out NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers))
            {
                // 这段代码在图鉴中缓慢旋转NPC
                //drawModifiers.Rotation += 0.001f;

                //将现有的NPCBestriaryDrawModifiers替换为具有调整旋转的新的****。
                NPCID.Sets.NPCBestiaryDrawOffset.Remove(Type);
                NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
            }

            return true;
        }


        //当npc被击中时发生，如制造血腥粒子（npc被怪物杀死）
        public override void HitEffect(int hitDirection, double damage)
        {
            int num = NPC.life > 0 ? 1 : 5;

            for (int k = 0; k < num; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 16);
            }
        }

        //是否满足了这个城镇NPC能够进城的条件。例如，爆破专家要求有炸药。
        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {   //遍历255个npc上限
            for (int k = 0; k < 255; k++)
            {
                Player player = Main.player[k];
                if (!player.active)
                {
                    continue;
                }

                //如果玩家打败了克眼
                if (NPC.downedBoss1)
                {
                    return true;
                }
            }
            return false;
        }


        //允许您设置该NPC使用的城镇NPC配置文件。 默认情况下，返回 null，表示NPC不使用一个。
        public override ITownNPCProfile TownNPCProfile()
        {
            return null;
        }


        //设置npc名字
        public override List<string> SetNPCNameList()
        {
            return new List<string>() {
                "Koarn",
                "科恩"
            };
        }


        //对话
        public override string GetChat()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            /*
            int partyGirl = NPC.FindFirstNPC(NPCID.PartyGirl);
            if (partyGirl >= 0 && Main.rand.NextBool(4))
            {
                chat.Add(Language.GetTextValue("Mods.ExampleMod.Dialogue.ExamplePerson.PartyGirlDialogue", Main.npc[partyGirl].GivenName));
            }
            */
            chat.Add(Language.GetTextValue("Mods.tRoot.Dialogue.Scientist.StandardDialogue1"));
            chat.Add(Language.GetTextValue("Mods.tRoot.Dialogue.Scientist.StandardDialogue2"));
            chat.Add(Language.GetTextValue("Mods.tRoot.Dialogue.Scientist.StandardDialogue3"));
            chat.Add(Language.GetTextValue("Mods.tRoot.Dialogue.Scientist.StandardDialogue4"));
            chat.Add(Language.GetTextValue("Mods.tRoot.Dialogue.Scientist.StandardDialogue5"));
            chat.Add(Language.GetTextValue("Mods.tRoot.Dialogue.Scientist.StandardDialogue6"));
            chat.Add(Language.GetTextValue("Mods.tRoot.Dialogue.Scientist.StandardDialogue7"));
            chat.Add(Language.GetTextValue("Mods.tRoot.Dialogue.Scientist.StandardDialogue8"));
            chat.Add(Language.GetTextValue("Mods.tRoot.Dialogue.Scientist.RareDialogue1"), 0.1);
            chat.Add(Language.GetTextValue("Mods.tRoot.Dialogue.Scientist.RareDialogue2"), 0.3);

            return chat;
        }


        //设置对话按钮
        public override void SetChatButtons(ref string button, ref string button2)
        {
            //第一个按钮，商店
            button = Language.GetTextValue("LegacyInterface.28");
        }


        //按钮的作用
        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                shop = true;
            }
        }


        // 商店功能
        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Ammo.IceFireArrow>());
            shop.item[nextSlot++].value = Item.buyPrice(0, 0, 0, 25);
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Ammo.JadeSpiralBullet>());
            shop.item[nextSlot++].value = Item.buyPrice(0, 0, 0, 30);
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Consumables.Potions.TreasureHuntPotion>());
            shop.item[nextSlot++].value = Item.buyPrice(0, 2, 0, 0);
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Placeable.Block.MoireWood>());
            shop.item[nextSlot++].value = Item.buyPrice(0, 0, 0, 50);
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Tools.BanDianHook>());
            shop.item[nextSlot++].value = Item.buyPrice(0, 5, 0, 0);
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Weapons.Warrior.SeaStoneSpear>());
            shop.item[nextSlot++].value = Item.buyPrice(0, 5, 0, 0);
            shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Weapons.Shooter.WillowVineBow>());
            shop.item[nextSlot++].value = Item.buyPrice(0, 5, 0, 0);

            if (NPC.downedBoss2)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Tools.BanDianAxe>());
                shop.item[nextSlot++].value = Item.buyPrice(0, 20, 0, 0);
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Tools.BanDianHammer>());
                shop.item[nextSlot++].value = Item.buyPrice(0, 20, 0, 0);
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Tools.BanDianPickaxe>());
                shop.item[nextSlot++].value = Item.buyPrice(0, 20, 0, 0);
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Weapons.Warrior.BloodySpinningSpear>());
                shop.item[nextSlot++].value = Item.buyPrice(0, 20, 0, 0);
            }
            if (NPC.downedBoss3)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<Pets.MiniFlower.MiniFlowerItem>());
                shop.item[nextSlot++].value = Item.buyPrice(0, 10, 0, 0);
            }
            if (Main.hardMode)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Weapons.Master.HolySwordRune>());
                shop.item[nextSlot++].value = Item.buyPrice(0, 50, 0, 0);
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.Things.BufferBalancingAgentⅡ>());
                shop.item[nextSlot++].value = Item.buyPrice(0, 0, 20, 0);
            }
        }


        //触发后，使该城镇NPC传送到国王和/或王后雕像。
        public override bool CanGoToStatue(bool toKingStatue) => true;


        //全职  npc攻击伤害和击退
        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 10;
            knockback = 4f;
        }


        //全职  npc的攻击冷却，第一个参数是基础冷却时间，第二个是在第一个的基础上，随机加 0~randExtraCooldown时间，计算总共的冷却时间
        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 40;
            randExtraCooldown = 40;
        }


        //npc的射弹，只在npc的攻击方式为投掷，射击，魔法时启用。
        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            //projType = ModContent.ProjectileType<Projectiles.Master.MagicSword>();
            projType = 254;
            //projType = ProjectileID.LastPrism;
            attackDelay = 40;
        }


        //攻击的射弹速度
        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            //弹幕速度
            multiplier = 6f;
            //重力修正，描述作用是抛射物在发射时向上偏移的程度，但我感觉影响不大
            gravityCorrection = 0f;
            //攻击精准度，越低越准，最低为0
            randomOffset = 0f;
        }


        //即使这不是一个城镇NPC，也能让这个ModNPC在全世界范围内保存。默认为false。注意：城镇NPC将始终被保存。
        //注：需要保存的NPC不会自然消亡。
        public override bool NeedSaving()
        {
            return base.NeedSaving();
        }
    }
}