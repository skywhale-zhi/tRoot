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
    [AutoloadHead]
    internal class XZ503 : ModNPC
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
                .SetBiomeAffection<DesertBiome>(AffectionLevel.Hate) // 讨厌沙漠
                .SetBiomeAffection<SnowBiome>(AffectionLevel.Hate)
                .SetNPCAffection(ModContent.NPCType<Scientist>(), AffectionLevel.Love) // Likes living near the guide.
                .SetNPCAffection(NPCID.Steampunker, AffectionLevel.Dislike) // Dislikes living near the merchant.
                .SetNPCAffection(209, AffectionLevel.Hate) // Dislikes living near the merchant.
            ;
        }


        public override void SetDefaults()
        {
            NPC.townNPC = true; // Sets NPC to be a Town NPC
            NPC.friendly = true; // NPC Will not attack player
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = 7;
            NPC.damage = 20;    //伤害
            NPC.defense = 15;
            NPC.lifeMax = 500;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.2f;//击退抗性，0完全抵抗
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
                new FlavorTextBestiaryInfoElement("Mods.tRoot.Bestiary.XZ503")
            });
        }


        //PreDraw在绘制sprite之前绘制东西或在绘制sprite之前运行代码非常有用
        //返回false将允许您手动绘制NPC
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // (simply checking NPC.IsABestiaryIconDummy and incrementing NPC.Rotation won't work here as it gets overridden by drawModifiers.Rotation each tick)
            if (NPCID.Sets.NPCBestiaryDrawOffset.TryGetValue(Type, out NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers))
            {
                //将现有的NPCBestriaryDrawModifiers替换为具有调整旋转的新的****。
                NPCID.Sets.NPCBestiaryDrawOffset.Remove(Type);
                NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
            }

            return true;
        }


        //当npc被击中时发生，如制造血腥粒子（npc被怪物杀死）
        public override void HitEffect(int hitDirection, double damage)
        {
            int num = NPC.life > 0 ? 20 : 40;

            for (int k = 0; k < num; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.UnusedWhiteBluePurple);
            }
        }


        //是否满足了这个城镇NPC能够进城的条件。例如，爆破专家要求有炸药。
        public override bool CanTownNPCSpawn(int numTownNPCs, int money)
        {
            for (int k = 0; k < 255; k++)
            {
                NPC tempnpc = Main.npc[k];
                if (tempnpc.type == ModContent.NPCType<Scientist>())
                {
                    return true;
                }
            }
            return false;
        }

        //设置npc名字
        public override List<string> SetNPCNameList()
        {
            return new List<string>() {
                "Day2022105"
            };
        }


        //对话
        public override string GetChat()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();

            chat.Add(Language.GetTextValue("Mods.tRoot.Dialogue.XZ503.StandardDialogue1"));
            chat.Add(Language.GetTextValue("Mods.tRoot.Dialogue.XZ503.StandardDialogue2"));
            chat.Add(Language.GetTextValue("Mods.tRoot.Dialogue.XZ503.StandardDialogue3"));
            chat.Add(Language.GetTextValue("Mods.tRoot.Dialogue.XZ503.StandardDialogue4"));
            chat.Add(Language.GetTextValue("Mods.tRoot.Dialogue.XZ503.StandardDialogue5"));
            chat.Add(Language.GetTextValue("Mods.tRoot.Dialogue.XZ503.StandardDialogue6"));
            chat.Add(Language.GetTextValue("Mods.tRoot.Dialogue.XZ503.StandardDialogue7"));
            chat.Add(Language.GetTextValue("Mods.tRoot.Dialogue.XZ503.StandardDialogue8"));
            chat.Add(Language.GetTextValue("Mods.tRoot.Dialogue.XZ503.StandardDialogue10"));

            if (tRoot.CalamityMod != null)
            {
                chat.Add(Language.GetTextValue("Mods.tRoot.Dialogue.XZ503.StandardDialogue9"));
            }

            return chat;
        }


        //设置对话按钮
        public override void SetChatButtons(ref string button, ref string button2)
        {
            //第一个按钮，商店
            if (!XZ503Data.CalamityModShop || tRoot.CalamityMod == null)
            {
                button = "材料";
            }
            else
            {
                button = "灾厄材料";
            }

            if (tRoot.CalamityMod != null)
            {
                button2 = Language.GetTextValue("切换搜集其他世界的材料");
            }
        }


        //按钮的作用
        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                shop = true;
            }
            else if (tRoot.CalamityMod != null)
            {
                XZ503Data.CalamityModShop = !XZ503Data.CalamityModShop;
            }
        }


        // 商店功能
        public override void SetupShop(Chest shop, ref int nextSlot)
        {
            NPCKillsTracker NKT = Main.BestiaryTracker.Kills;

            //int count = temp.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[50]);
            if (!XZ503Data.CalamityModShop)
            {
                if (NPC.downedBoss1 && (!NPC.downedBoss2 || !NPC.downedBoss3))
                {
                    shop.item[nextSlot].SetDefaults(2674);//普通鱼饵
                    shop.item[nextSlot++].value = Item.buyPrice(0, 0, 15, 0);
                }
                else if (NPC.downedBoss2 && !NPC.downedBoss3)
                {
                    shop.item[nextSlot].SetDefaults(2675);//中级鱼饵
                    shop.item[nextSlot++].value = Item.buyPrice(0, 0, 45, 0);
                }
                else if (NPC.downedBoss3)
                {
                    shop.item[nextSlot].SetDefaults(2676);//高级鱼饵
                    shop.item[nextSlot++].value = Item.buyPrice(0, 90, 0, 0);
                }

                shop.item[nextSlot].SetDefaults(31);//瓶子
                shop.item[nextSlot++].value = Item.buyPrice(0, 0, 1, 0);

                if (NPC.downedBoss2)
                {
                    shop.item[nextSlot].SetDefaults(29);//生命水晶
                    shop.item[nextSlot++].value = Item.buyPrice(0, 10, 0, 0);
                    shop.item[nextSlot].SetDefaults(75);//坠星
                    shop.item[nextSlot++].value = Item.buyPrice(0, 0, 40, 0);
                }
                if (NPC.downedBoss3)
                {
                    shop.item[nextSlot].SetDefaults(148);//水蜡烛
                    shop.item[nextSlot++].value = Item.buyPrice(0, 1, 0, 0);
                }
                if (NPC.downedPlantBoss)
                {
                    shop.item[nextSlot].SetDefaults(1225);//神圣钉
                    shop.item[nextSlot++].value = Item.buyPrice(0, 5, 0, 0);
                    shop.item[nextSlot].SetDefaults(1253);//冰龟壳
                    shop.item[nextSlot++].value = Item.buyPrice(2, 0, 0, 0);

                }
                if (NPC.downedGolemBoss)
                {
                    shop.item[nextSlot].SetDefaults(1006);//叶绿锭
                    shop.item[nextSlot++].value = Item.buyPrice(0, 7, 0, 0);
                    shop.item[nextSlot].SetDefaults(1291);//生命果
                    shop.item[nextSlot++].value = Item.buyPrice(0, 10, 0, 0);
                    shop.item[nextSlot].SetDefaults(1613);//十字章盾
                    shop.item[nextSlot++].value = Item.buyPrice(5, 0, 0, 0);
                    shop.item[nextSlot].SetDefaults(3124);//手机
                    shop.item[nextSlot++].value = Item.buyPrice(5, 0, 0, 0);
                }

                if (NPC.downedEmpressOfLight)
                {
                    shop.item[nextSlot].SetDefaults(1326);//混沌法杖
                    shop.item[nextSlot++].value = Item.buyPrice(50, 0, 0, 0);
                }

                if (NKT.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[NPCID.HallowBoss]) >= 6)
                {
                    shop.item[nextSlot].SetDefaults(5005);//光棱镜
                    shop.item[nextSlot++].value = Item.buyPrice(10, 0, 0, 0);
                }

                if (NPC.downedMoonlord)
                {
                    shop.item[nextSlot].SetDefaults(3456);//四个碎片
                    shop.item[nextSlot++].value = Item.buyPrice(0, 7, 0, 0);
                    shop.item[nextSlot].SetDefaults(3457);//
                    shop.item[nextSlot++].value = Item.buyPrice(0, 7, 0, 0);
                    shop.item[nextSlot].SetDefaults(3458);//
                    shop.item[nextSlot++].value = Item.buyPrice(0, 7, 0, 0);
                    shop.item[nextSlot].SetDefaults(3459);//
                    shop.item[nextSlot++].value = Item.buyPrice(0, 7, 0, 0);
                    shop.item[nextSlot].SetDefaults(3460);//夜明矿
                    shop.item[nextSlot++].value = Item.buyPrice(0, 7, 0, 0);
                }
            }


            else if (XZ503Data.CalamityModShop && tRoot.CalamityMod != null)
            {
                if (Main.hardMode)
                {
                    if (tRoot.CalamityMod.TryFind<ModItem>("SlimeGodBag", out ModItem SlimeGodBag))//史莱姆神袋子
                    {
                        shop.item[nextSlot].SetDefaults(SlimeGodBag.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(0, 50, 0, 0);
                    }
                }

                if (NPC.downedPlantBoss)
                {
                    if (tRoot.CalamityMod.TryFind<ModItem>("SupremeBaitTackleBoxFishingStation", out ModItem sbtbfs))//超级钓鱼器
                    {
                        shop.item[nextSlot].SetDefaults(sbtbfs.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(5, 0, 0, 0);
                    }
                    if (tRoot.CalamityMod.TryFind<ModItem>("AngelTreads", out ModItem AngelTreads))//天使靴
                    {
                        shop.item[nextSlot].SetDefaults(AngelTreads.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(10, 0, 0, 0);
                    }
                    if (tRoot.CalamityMod.TryFind<ModItem>("BloodOrb", out ModItem BloodOrb))//血珠
                    {
                        shop.item[nextSlot].SetDefaults(BloodOrb.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(0, 1, 0, 0);
                    }
                }

                //如果击败了毁灭魔像
                if (tRoot.CalamityMod.TryFind<ModNPC>("RavagerBody", out ModNPC Ravager) && NKT.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Ravager.Type]) > 0)
                {
                    if (tRoot.CalamityMod.TryFind<ModItem>("DeadshotBrooch", out ModItem DeadShotBrooch))//射手饰品
                    {
                        shop.item[nextSlot].SetDefaults(DeadShotBrooch.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(10, 0, 0, 0);
                    }
                    {//战士饰品
                        shop.item[nextSlot].SetDefaults(1343);
                        shop.item[nextSlot++].value = Item.buyPrice(10, 0, 0, 0);
                    }
                    if (tRoot.CalamityMod.TryFind<ModItem>("StarTaintedGenerator", out ModItem StarTaintedGenerator))//召唤饰品
                    {
                        shop.item[nextSlot].SetDefaults(StarTaintedGenerator.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(10, 0, 0, 0);
                    }
                    if (tRoot.CalamityMod.TryFind<ModItem>("SigilofCalamitas", out ModItem SigilOfCalamitas))//法师饰品
                    {
                        shop.item[nextSlot].SetDefaults(SigilOfCalamitas.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(10, 0, 0, 0);
                    }
                    if (tRoot.CalamityMod.TryFind<ModItem>("AbyssalMirror", out ModItem AbyssalMirror))//盗贼饰品
                    {
                        shop.item[nextSlot].SetDefaults(AbyssalMirror.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(10, 0, 0, 0);
                    }

                    if (tRoot.CalamityMod.TryFind<ModItem>("CryonicBar", out ModItem CryonicBar))//寒元锭
                    {
                        shop.item[nextSlot].SetDefaults(CryonicBar.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(0, 6, 0, 0);
                    }
                    if (tRoot.CalamityMod.TryFind<ModItem>("PerennialBar", out ModItem PerennialBar))//世花锭
                    {
                        shop.item[nextSlot].SetDefaults(PerennialBar.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(0, 7, 0, 0);
                    }
                    if (tRoot.CalamityMod.TryFind<ModItem>("ScoriaBar", out ModItem ScoriaBar))//熔渣锭
                    {
                        shop.item[nextSlot].SetDefaults(ScoriaBar.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(0, 8, 0, 0);
                    }
                }

                if (NPC.downedMoonlord)
                {
                    if (tRoot.CalamityMod.TryFind<ModItem>("AstralOre", out ModItem AstralOre))//炫星矿
                    {
                        shop.item[nextSlot].SetDefaults(AstralOre.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(0, 4, 0, 0);
                    }
                    if (tRoot.CalamityMod.TryFind<ModItem>("TheAbsorber", out ModItem TheAbsorber))//阴阳吸石
                    {
                        shop.item[nextSlot].SetDefaults(TheAbsorber.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(15, 0, 0, 0);
                    }
                }

                //亵渎献祭者小怪,吞噬魔>=50
                if (tRoot.CalamityMod.TryFind<ModNPC>("ScornEater", out ModNPC ScornEater) && tRoot.CalamityMod.TryFind<ModNPC>("ImpiousImmolator", out ModNPC ImpiousImmolator) && NKT.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[ScornEater.Type]) + NKT.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[ImpiousImmolator.Type]) >= 50)
                {
                    if (tRoot.CalamityMod.TryFind<ModItem>("UnholyEssence", out ModItem UnholyEssence))//浊火核心
                    {
                        shop.item[nextSlot].SetDefaults(UnholyEssence.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(0, 8, 0, 0);
                    }
                }

                //巨象乌贼>=8
                if (tRoot.CalamityMod.TryFind<ModNPC>("ColossalSquid", out ModNPC ColossalSquid) && NKT.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[ColossalSquid.Type]) >= 8)
                {
                    if (tRoot.CalamityMod.TryFind<ModItem>("CalamarisLament", out ModItem CalamarisLament))//乌贼哀歌法杖
                    {
                        shop.item[nextSlot].SetDefaults(CalamarisLament.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(60, 0, 0, 0);
                    }
                }

                //播毕特虫>=8
                if (tRoot.CalamityMod.TryFind<ModNPC>("BobbitWormHead", out ModNPC BobbitWormHead) && NKT.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[BobbitWormHead.Type]) >= 8)
                {
                    if (tRoot.CalamityMod.TryFind<ModItem>("BobbitHook", out ModItem CalamarisLament))//博比特虫钩
                    {
                        shop.item[nextSlot].SetDefaults(CalamarisLament.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(50, 0, 0, 0);
                    }
                }

                //猎魂鲨>=8
                if (tRoot.CalamityMod.TryFind<ModNPC>("ReaperShark", out ModNPC ReaperShark) && NKT.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[ReaperShark.Type]) >= 8)
                {
                    if (tRoot.CalamityMod.TryFind<ModItem>("ReaperTooth", out ModItem CalamarisLament))//牙齿
                    {
                        shop.item[nextSlot].SetDefaults(CalamarisLament.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(0, 20, 0, 0);
                    }
                }


                //环海妖龙小>=5
                if (tRoot.CalamityMod.TryFind<ModNPC>("EidolonWyrmHead", out ModNPC EidolonWyrmHead) && NKT.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[EidolonWyrmHead.Type]) >= 5)
                {
                    if (tRoot.CalamityMod.TryFind<ModItem>("EidolicWail", out ModItem EidolicWail))
                    {
                        shop.item[nextSlot].SetDefaults(EidolicWail.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(60, 0, 0, 0);
                    }
                    if (tRoot.CalamityMod.TryFind<ModItem>("EidolonStaff", out ModItem EidolonStaff))
                    {
                        shop.item[nextSlot].SetDefaults(EidolonStaff.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(60, 0, 0, 0);
                    }
                    if (tRoot.CalamityMod.TryFind<ModItem>("SoulEdge", out ModItem SoulEdge))
                    {
                        shop.item[nextSlot].SetDefaults(SoulEdge.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(60, 0, 0, 0);
                    }
                }


                //亵渎天神和幽花
                if (tRoot.CalamityMod.TryFind<ModNPC>("Providence", out ModNPC Providence) && tRoot.CalamityMod.TryFind<ModNPC>("Polterghast", out ModNPC Polterghast) && NKT.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Providence.Type]) >= 2 && NKT.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Polterghast.Type]) >= 2)
                {
                    if (tRoot.CalamityMod.TryFind<ModItem>("DivineGeode", out ModItem DivineGeode))//圣石结晶
                    {
                        shop.item[nextSlot].SetDefaults(DivineGeode.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(0, 25, 0, 0);
                    }
                    if (tRoot.CalamityMod.TryFind<ModItem>("RuinousSoul", out ModItem RuinousSoul))//毁灭碎片
                    {
                        shop.item[nextSlot].SetDefaults(RuinousSoul.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(0, 25, 0, 0);
                    }
                    if (tRoot.CalamityMod.TryFind<ModItem>("UelibloomOre", out ModItem UelibloomOre))//龙蒿矿
                    {
                        shop.item[nextSlot].SetDefaults(UelibloomOre.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(0, 5, 0, 0);
                    }
                    if (tRoot.CalamityMod.TryFind<ModItem>("Bloodstone", out ModItem Bloodstone))//血石
                    {
                        shop.item[nextSlot].SetDefaults(Bloodstone.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(0, 12, 0, 0);
                    }
                }


                //神明吞噬者 >= 4
                if (tRoot.CalamityMod.TryFind<ModNPC>("DevourerofGodsHead", out ModNPC DevourerofGodsHead) && NKT.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[DevourerofGodsHead.Type]) >= 4)
                {
                    if (tRoot.CalamityMod.TryFind<ModItem>("CosmiliteBar", out ModItem CosmiliteBar))
                    {
                        shop.item[nextSlot].SetDefaults(CosmiliteBar.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(0, 50, 0, 0);
                    }

                    if (tRoot.CalamityMod.TryFind<ModItem>("ElementalGauntlet", out ModItem ElementalGauntlet))
                    {
                        shop.item[nextSlot].SetDefaults(ElementalGauntlet.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(150, 0, 0, 0);
                    }
                    if (tRoot.CalamityMod.TryFind<ModItem>("ElementalQuiver", out ModItem ElementalQuiver))
                    {
                        shop.item[nextSlot].SetDefaults(ElementalQuiver.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(150, 0, 0, 0);
                    }
                    if (tRoot.CalamityMod.TryFind<ModItem>("EtherealTalisman", out ModItem EtherealTalisman))
                    {
                        shop.item[nextSlot].SetDefaults(EtherealTalisman.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(150, 0, 0, 0);
                    }
                    if (tRoot.CalamityMod.TryFind<ModItem>("Nanotech", out ModItem Nanotech))
                    {
                        shop.item[nextSlot].SetDefaults(Nanotech.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(150, 0, 0, 0);
                    }
                    if (tRoot.CalamityMod.TryFind<ModItem>("Nucleogenesis", out ModItem Nucleogenesis))//
                    {
                        shop.item[nextSlot].SetDefaults(Nucleogenesis.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(150, 0, 0, 0);
                    }

                    if (tRoot.CalamityMod.TryFind<ModItem>("AsgardianAegis", out ModItem AsgardianAegis))
                    {
                        shop.item[nextSlot].SetDefaults(AsgardianAegis.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(150, 0, 0, 0);
                    }
                    if (tRoot.CalamityMod.TryFind<ModItem>("CoreOfTheBloodGod", out ModItem CoreOfTheBloodGod))
                    {
                        shop.item[nextSlot].SetDefaults(CoreOfTheBloodGod.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(150, 0, 0, 0);
                    }
                    if (tRoot.CalamityMod.TryFind<ModItem>("RampartofDeities", out ModItem RampartofDeities))
                    {
                        shop.item[nextSlot].SetDefaults(RampartofDeities.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(150, 0, 0, 0);
                    }
                    if (tRoot.CalamityMod.TryFind<ModItem>("TheAmalgam", out ModItem TheAmalgam))
                    {
                        shop.item[nextSlot].SetDefaults(TheAmalgam.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(150, 0, 0, 0);
                    }
                    if (tRoot.CalamityMod.TryFind<ModItem>("TheSponge", out ModItem TheSponge))
                    {
                        shop.item[nextSlot].SetDefaults(TheSponge.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(150, 0, 0, 0);
                    }
                    if (tRoot.CalamityMod.TryFind<ModItem>("ElysianTracers", out ModItem ElysianTracers))
                    {
                        shop.item[nextSlot].SetDefaults(ElysianTracers.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(150, 0, 0, 0);
                    }

                }


                //丛林龙 >= 3
                if (tRoot.CalamityMod.TryFind<ModNPC>("Yharon", out ModNPC Yharon) && NKT.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Yharon.Type]) >= 3)
                {

                    if (tRoot.CalamityMod.TryFind<ModItem>("YharonSoulFragment", out ModItem YharonSoulFragment))//龙魂碎片
                    {
                        shop.item[nextSlot].SetDefaults(YharonSoulFragment.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(1, 50, 0, 0);
                    }
                    if (tRoot.CalamityMod.TryFind<ModItem>("AuricOre", out ModItem AuricOre) && NKT.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Yharon.Type]) >= 6)
                    {
                        shop.item[nextSlot].SetDefaults(AuricOre.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(0, 50, 0, 0);
                    }
                }


                //嘉登>=2
                if (tRoot.CalamityMod.TryFind<ModNPC>("Draedon", out ModNPC Draedon) && NKT.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Draedon.Type]) >= 2)
                {
                    if (tRoot.CalamityMod.TryFind<ModItem>("ExoPrism", out ModItem ExoPrism))//星流棱晶
                    {
                        shop.item[nextSlot].SetDefaults(ExoPrism.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(5, 50, 0, 0);
                    }
                }


                //至尊灾厄>=2
                if (tRoot.CalamityMod.TryFind<ModNPC>("SupremeCalamitas", out ModNPC SupremeCalamitas) && NKT.GetKillCount(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[SupremeCalamitas.Type]) >= 2)
                {
                    if (tRoot.CalamityMod.TryFind<ModItem>("AshesofAnnihilation", out ModItem AshesofAnnihilation))//至尊灾厄尘
                    {
                        shop.item[nextSlot].SetDefaults(AshesofAnnihilation.Type);
                        shop.item[nextSlot++].value = Item.buyPrice(6, 0, 0, 0);
                    }
                }
            }
        }

        //全职  npc攻击伤害和击退
        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 20;
            knockback = 4f;
        }


        //全职  npc的攻击冷却，第一个参数是基础冷却时间，第二个是在第一个的基础上，随机加 0~randExtraCooldown时间，计算总共的冷却时间
        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 60;
            randExtraCooldown = 60;
        }


        //npc的射弹，只在npc的攻击方式为投掷，射击，魔法时启用。
        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            //projType = ModContent.ProjectileType<Projectiles.Master.MagicSword>();
            projType = 442;
            //projType = ProjectileID.LastPrism;
            attackDelay = 40;
        }


        //攻击的射弹速度
        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            //弹幕速度
            multiplier = 15f;
            //重力修正，描述作用是抛射物在发射时向上偏移的程度，但我感觉影响不大
            gravityCorrection = 0f;
            //攻击精准度，越低越准，最低为0
            randomOffset = 0f;
        }
    }

    public class XZ503Data
    {
        public static bool CalamityModShop = false;
    }
}
