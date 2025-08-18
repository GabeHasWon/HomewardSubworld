using ContinentOfJourney.Items;
using ContinentOfJourney.NPCs.Boss_Diver;
using SubworldLibrary;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace HomewardSubworld;

internal class DivingLeech : ModItem
{
    public override void SetDefaults()
    {
        Item.Size = new(32, 26);
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.useTime = Item.useAnimation = 20;
        Item.consumable = true;
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ItemRarityID.Green;
    }

    public override bool? UseItem(Player player)
    {
        SubworldSystem.Enter<AbyssalSubworld>();
        return true;
    }

    public override bool CanUseItem(Player player) => SubworldSystem.Current is not AbyssalSubworld;
}

internal class TheDiverNPC : GlobalNPC
{
    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        if (npc.type == ModContent.NPCType<Diver>())
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DivingLeech>()));
    }
}

internal class TheDiverGlobalItem : GlobalItem
{
    public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
    {
        if (item.type == ModContent.ItemType<DiverTreasureBag>())
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<DivingLeech>()));
    }
}