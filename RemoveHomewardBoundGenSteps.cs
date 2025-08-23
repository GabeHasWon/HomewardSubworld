using ContinentOfJourney;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace HomewardSubworld;

internal class RemoveHomewardBoundGenSteps : ModSystem
{
    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
    {
        foreach (var task in tasks)
            if (task is AbyssPass or ShadowWallPass or AbyssChestPass)
                task.Disable();
    }
}
