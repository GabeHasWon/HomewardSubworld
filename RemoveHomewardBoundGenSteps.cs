using ContinentOfJourney;
using HomewardSubworld.Generation;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace HomewardSubworld;

internal class RemoveHomewardBoundGenSteps : ModSystem
{
    public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
    {
        foreach (var task in tasks)
            if (task is AbyssPass or ShadowWallPass or ContinentOfJourney.AbyssChestPass)
                task.Disable();

        int index = tasks.FindIndex(x => x is ShadowWallPass);

        if (index != -1)
        {
            tasks.Add(new PassLegacy("Maze", RunCityPass));
        }
    }

    internal static void RunCityPass(GenerationProgress progress, GameConfiguration configuration)
    {
        int snowCenterX = (GenVars.snowOriginLeft + GenVars.snowOriginRight) / 2;
        int snowBottomY = GenVars.snowBottom;
        int darkHeight = (int)((Main.maxTilesY - 200 - snowBottomY) * 0.75f);

        int theatreY = WorldGen.genRand.Next(snowBottomY, snowBottomY + darkHeight);
        int theatreX = snowCenterX;

        int radius = 240;
        if (Main.maxTilesX > 12800 + 100)
            radius = 480;
        else if (Main.maxTilesX > 8400 + 100)
            radius = 360;
        radius = (int)(radius * 0.76f);

        if (theatreY + radius > Main.maxTilesY - 200)
            theatreY = Main.maxTilesY - 200 - radius;

        CoJWorldGeneration.theatrePos = new Vector2(theatreX, theatreY);

        int x = (int)CoJWorldGeneration.theatrePos.X;
        int y = (int)CoJWorldGeneration.theatrePos.Y;

        int num = 180;

        if (Main.maxTilesX > 12700)
        {
            num = 300;
        }
        else if (Main.maxTilesX > 8300)
        {
            num = 240;
        }

        bool flowControl = MazeGeneration.SetUpMaze(num);

        if (!flowControl)
            return;

        MazeGeneration.GenerateTheatreInMaze(x, y);
    }
}
