using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.WorldBuilding;
using ReLogic.Graphics;
using HomewardSubworld.Generation;
using ContinentOfJourney;

namespace HomewardSubworld;

internal class AbyssalSubworld : Subworld
{
    public override int Width => 600;
    public override int Height => 1800;
    public override bool ShouldSave =>
#if !DEBUG
        true;
#else
        false;
#endif

    protected static void ResetStep(GenerationProgress progress, GameConfiguration configuration)
    {
        WorldGenerator.CurrentGenerationProgress = progress;
        Main.ActiveWorldFileData.SetSeedToRandom();
        GenVars.structures = new();
        WorldGen._genRandSeed = Main.rand.Next();

        int rand = Main.rand.Next(10, 30);

        for (int i = 0; i < rand; ++i)
        {
            WorldGen.genRand.Next();
        }
    }

    public override List<GenPass> Tasks => [new PassLegacy("Reset", ResetStep, 0.2f), new AbyssalPass("Abyss", 1), new Generation.AbyssChestPass("Chests", 1 / 8f), 
        new CityPass("Abyssal City", 1 / 3f), new PassLegacy("Metadata", SetMetadata)];

    public override void CopyMainWorldData()
    {
        SubworldSystem.CopyWorldData("downedDiver", DownedBossSystem.downedDiverBoss);
        SubworldSystem.CopyWorldData("downedBarrier", DownedBossSystem.downedBarrier);
    }

    public override void ReadCopiedMainWorldData()
    {
        DownedBossSystem.downedDiverBoss = SubworldSystem.ReadCopiedWorldData<bool>("downedDiver");
        DownedBossSystem.downedBarrier = SubworldSystem.ReadCopiedWorldData<bool>("downedBarrier");
    }

    private void SetMetadata(GenerationProgress progress, GameConfiguration configuration)
    {
        Main.spawnTileX = Main.maxTilesX / 2;
        Main.spawnTileY = 60;

        while (!WorldGen.SolidTile(Main.spawnTileX, Main.spawnTileY))
        {
            Main.spawnTileY++;
        }

        Main.spawnTileY -= 3;
    }

    public override void Update() => Liquid.UpdateLiquid();

    public override void DrawMenu(GameTime gameTime)
    {
        string statusText = Main.statusText;
        GenerationProgress progress = WorldGenerator.CurrentGenerationProgress;

        if (WorldGen.gen && progress is not null)
        {
            DrawStringCentered(progress.Message, Color.LightGray, new Vector2(0, 60), 0.6f);
            double percentage = progress.Value / progress.CurrentPassWeight * 100f;
            DrawStringCentered($"{percentage:#0.##}%", Color.LightGray, new Vector2(0, 120), 0.7f);
        }

        DrawStringCentered(statusText, Color.White);
    }

    private static void DrawStringCentered(string statusText, Color color, Vector2 position = default, float scale = 1f)
    {
        Vector2 screenCenter = new Vector2(Main.screenWidth, Main.screenHeight) / 2f + position;
        Vector2 halfSize = FontAssets.DeathText.Value.MeasureString(statusText) / 2f * scale;
        Main.spriteBatch.DrawString(FontAssets.DeathText.Value, statusText, screenCenter - halfSize, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
    }
}
