using ContinentOfJourney;
using ContinentOfJourney.Items;
using ContinentOfJourney.NPCs.Boss_Diver;
using SubworldLibrary;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
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

    public override void ModifyGlobalLoot(GlobalLoot globalLoot)
    {
        globalLoot.Add(ItemDropRule.ByCondition(new DownedPlanteraNotDownedDiver(), ModContent.ItemType<DivingLeech>(), 100));
        globalLoot.Add(ItemDropRule.ByCondition(new DownedDiver(), ModContent.ItemType<DivingLeech>(), 1500));
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

public class DownedPlanteraNotDownedDiver : IItemDropRuleCondition, IProvideItemConditionDescription
{
    public bool CanDrop(DropAttemptInfo info) => NPC.downedPlantBoss && !DownedBossSystem.downedDiverBoss;
    public bool CanShowItemDropInUI() => false;
    public string GetConditionDescription() => null;
}

public class DownedDiver : IItemDropRuleCondition, IProvideItemConditionDescription
{
    public bool CanDrop(DropAttemptInfo info) => DownedBossSystem.downedDiverBoss;
    public bool CanShowItemDropInUI() => true;
    public string GetConditionDescription() => Language.GetTextValue("HomewardSubworld.PostDiver");
}