using MonoMod.RuntimeDetour;
using SubworldLibrary;
using System;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace HomewardSubworld;

public class HomewardSubworld : Mod
{
    private static Hook ChooseSpawnHook = null;

    public override void Load()
        => ChooseSpawnHook = new(typeof(NPCLoader).GetMethod(nameof(NPCLoader.ChooseSpawn), BindingFlags.Public | BindingFlags.Static), ChooseSpawnDetour, true);

    /// <summary>
    /// Forces all spawns in the Abyssal Subworld to spawn.
    /// </summary>
    public static int? ChooseSpawnDetour(Func<NPCSpawnInfo, int?> orig, NPCSpawnInfo spawnInfo)
    {
        bool wasPlanteraDowned = NPC.downedPlantBoss;

        if (SubworldSystem.Current is AbyssalSubworld)
        {
            NPC.downedPlantBoss = true;
        }

        int? value = orig(spawnInfo);

        NPC.downedPlantBoss = wasPlanteraDowned;
        return value;
    }
}
