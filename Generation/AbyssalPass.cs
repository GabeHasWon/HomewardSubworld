using ContinentOfJourney;
using ContinentOfJourney.Tiles.Abyss;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace HomewardSubworld.Generation;

/// <summary>
/// Generation adapted from Homeward Journey. Used with permission.
/// </summary>
public class AbyssalPass(string name, float loadWeight) : GenPass(name, loadWeight)
{
    private readonly int[,] _pillar1 = 
    {
        {0,0,0,1,1,0,0 },
        {0,0,0,1,1,0,0 },
        {0,0,0,1,1,0,0 },
        {0,0,0,1,1,0,0 },
        {0,0,0,1,1,0,0 },
        {0,0,0,1,1,0,0 },
        {0,0,0,1,1,0,0 },
        {0,0,0,1,1,0,0 },
        {0,0,0,1,1,0,0 },
        {0,0,0,1,1,0,0 },
        {0,0,0,1,1,0,0 },
        {0,0,0,1,1,0,0 },
        {0,0,0,1,1,0,0 },
        {0,0,0,1,1,0,0 },
        {0,0,0,1,1,0,0 },
        {0,0,0,1,1,0,0 },
        {0,0,0,1,1,0,0 },
        {0,0,0,1,1,0,0 },
    };

    private readonly int[,] _pillar2 = 
    {
        {0,0,0,1,0,0,0 },
        {0,0,0,1,0,0,0 },
        {0,0,0,1,0,0,0 },
        {0,0,0,1,0,0,0 },
        {0,0,0,1,0,0,0 },
        {0,0,0,1,0,0,0 },
        {0,0,0,1,0,0,0 },
        {0,0,0,1,0,0,0 },
        {0,0,0,1,0,0,0 },
        {0,0,0,1,0,0,0 },
        {0,0,0,1,0,0,0 },
        {0,0,0,1,0,0,0 },
        {0,0,0,1,0,0,0 },
        {0,0,0,1,0,0,0 },
        {0,0,0,1,0,0,0 },
        {0,0,0,1,0,0,0 },
        {0,0,0,1,0,0,0 },
        {0,0,0,1,0,0,0 },
    };

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        if (DownedBossSystem.isBackdoorWorld) 
            return;

        CoJWorldGeneration.theatrePos = new Vector2(300, (float)Main.worldSurface + 300);

        progress.Message = Language.GetTextValue("Mods.HomewardSubworld.Generation.Carving");

        float height;
        bool isThoriumLoaded = false;
        int HJAbyssStartY = (int)(Main.worldSurface + 0.26f * (Main.maxTilesY - 200 - 100 - Main.worldSurface));
        FastNoiseLite noise = new(WorldGen._genRandSeed);

        for (int i = 20; i < Main.maxTilesX - 20; ++i)
        {
            float offset = noise.GetNoise(i, 20) * 32;

            for (height = (int)(Main.worldSurface - 20 + offset); height < Main.maxTilesY - 200; height++)
            {
                Tile tile = Main.tile[i, (int)height];
                tile.HasTile = true;
                tile.TileType = (ushort)ModContent.TileType<AbyssStone>();
            }
        }

        int center = (int)(Main.worldSurface + (Main.maxTilesY - 200)) / 2;
        int radius2 = 800;
        int rocklayer = (int)(Main.maxTilesY - 200 - Main.rockLayer);

        for (float i = (int)(Main.worldSurface * 1.25f); i < rocklayer; i += 0.01f)
        {
            bool onyx = i % 6 == 1;
            int str = onyx ? WorldGen.genRand.Next(3, 7) : WorldGen.genRand.Next(4, 8);
            int type = onyx ? ModContent.TileType<Onyx>() : ModContent.TileType<DeepOre>();
            WorldGen.TileRunner(Main.rand.Next(50, Main.maxTilesX - 50), center - 100 + radius2 - (int)(Main.rand.NextFloat() * radius2 * 1.98), str, WorldGen.genRand.Next(2, 6), type);
        }

        progress.Message = Language.GetTextValue("Mods.HomewardSubworld.Generation.Decor");

        for (float i = -rocklayer; i < rocklayer; i += 0.3f)
        {
            int placeToDig = center - 100 + radius2 - (int)(Main.rand.NextFloat() * radius2 * 1.98);

            if (WorldGen.genRand.NextBool(8))
            {
                WorldGen.digTunnel(WorldGen.genRand.Next(100, Main.maxTilesX - 100), placeToDig, 0.2f - Main.rand.NextFloat() * 0.4f, 1, 12 + Main.rand.Next(0, 12), 8, false);
            }
            else
            {
                WorldGen.digTunnel(WorldGen.genRand.Next(100, Main.maxTilesX - 100), placeToDig, 1 - Main.rand.NextFloat() * 2, 1 - Main.rand.NextFloat() * 2, 3 + Main.rand.Next(0, 5), 3 + Main.rand.Next(0, 5), false);
            }
        }

        for (int i = 5; i < Main.maxTilesX - 5; ++i)
        {
            for (int j = Main.maxTilesY - 300; j < Main.maxTilesY; ++j)
            {
                Tile tile = Main.tile[i, j];
                tile.HasTile = true;
                tile.TileType = (ushort)ModContent.TileType<AbyssStone>();
            }
        }

        for (float c = 0; c < rocklayer; c += 0.2f)
        {
            int i = Main.rand.Next(50, Main.maxTilesX - 50);
            int j = center - 100 + (int)((radius2 - Main.rand.NextFloat() * radius2 * 1.98));

            if (isThoriumLoaded && j < HJAbyssStartY - 200)
                continue;

            int n = Main.rand.Next(1, 3);
            float percentage = (float)(j - Main.worldSurface) / (float)(Main.maxTilesY - 200 - 100 - Main.worldSurface);

            if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == (ushort)ModContent.TileType<AbyssStone>())
                if (percentage is < 0.15f or > 0.37f)
                    for (int y = 0; y < _pillar1.GetLength(0); y++)
                    {
                        for (int x = 0; x < _pillar1.GetLength(1); x++)
                        {
                            int k = i - 3 + x;
                            int l = j - 6 + y;
                            if (WorldGen.InWorld(k, l, 30))
                            {
                                Tile tile = Framing.GetTileSafely(k, l);
                                if (n == 1)
                                {
                                    switch (_pillar1[y, x])
                                    {
                                        case 1:
                                            if (tile.TileType != ModContent.TileType<DeepOre>())
                                            {
                                                tile.TileType = (ushort)ModContent.TileType<AbyssStone>();
                                            }

                                            tile.HasTile = true;

                                            if (percentage > 0.68f)
                                                tile.ClearTile();

                                            break;
                                    }
                                }
                                else
                                {
                                    switch (_pillar2[y, x])
                                    {
                                        case 1:
                                            if (tile.TileType != ModContent.TileType<DeepOre>())
                                            {
                                                tile.TileType = (ushort)ModContent.TileType<AbyssStone>();
                                            }

                                            tile.HasTile = true;

                                            if (percentage > 0.68f)
                                                tile.ClearTile();
                                            break;
                                    }
                                }
                            }
                        }
                    }
        }

        progress.Message = Language.GetTextValue("Mods.HomewardSubworld.Generation.Growing");

        int start = 10;
        int end = Main.maxTilesX - 10;
        bool thirdFloorShouldBeReadTwice = false;

        GenVars.structures.AddProtectedStructure(new Rectangle(start, (int)Main.worldSurface, end - start, (int)(Main.maxTilesY - 200 - 100 - Main.worldSurface)), 0);

        for (height = (float)Main.worldSurface; height < Main.maxTilesY - 200 - 100; height += 1)
        {
            float percentage = (float)(height - Main.worldSurface) / (float)(Main.maxTilesY - 200 - 100 - Main.worldSurface);
            int chanceOfCreatingFloor = Main.rand.Next(0, 40);
            if (isThoriumLoaded && height < HJAbyssStartY - 200) 
                continue;
            
            for (int x = start; x < end; x++)
            {
                Tile tile = Main.tile[x, (int)height];

                if (percentage <= 0.16f - MathF.Abs(noise.GetNoise(x, 0) * 0.02f))
                {

                }
                else if (percentage <= 0.36f - MathF.Abs(noise.GetNoise(x, 0) * 0.02f))
                {
                    if (tile.HasTile && tile.TileType == ModContent.TileType<AbyssStone>())
                    {
                        int hasTileCount = 0;
                        if (Main.tile[x - 1, (int)height].HasTile)
                            hasTileCount++;
                        if (Main.tile[x + 1, (int)height].HasTile) 
                            hasTileCount++;
                        if (Main.tile[x, (int)height - 1].HasTile) 
                            hasTileCount++;
                        if (Main.tile[x, (int)height + 1].HasTile) 
                            hasTileCount++;
                        if (Main.tile[x - 1, (int)height - 1].HasTile) 
                            hasTileCount++;
                        if (Main.tile[x + 1, (int)height + 1].HasTile) 
                            hasTileCount++;
                        if (Main.tile[x + 1, (int)height - 1].HasTile) 
                            hasTileCount++;
                        if (Main.tile[x - 1, (int)height + 1].HasTile) 
                            hasTileCount++;

                        if (hasTileCount < 8)
                        {
                            SlopeType slopeType = tile.Slope;
                            BlockType blockType = tile.BlockType;
                            tile.TileType = (ushort)ModContent.TileType<FluorescentGrass>();
                            tile.Slope = slopeType; 
                            tile.BlockType = blockType;

                            if (Main.rand.NextBool(4)) 
                                WorldGen.PlaceTile(x, (int)height - 1, ModContent.TileType<ContinentOfJourney.Tiles.Backdoor.Plants.BackdoorPlantNormal_Tall>(), mute: true, false, -1, Main.rand.Next(20));
                            else 
                                WorldGen.PlaceTile(x, (int)height - 1, ModContent.TileType<ContinentOfJourney.Tiles.Backdoor.Plants.BackdoorPlantNormal>(), mute: true, false, -1, Main.rand.Next(11));

                            WorldGen.GrowTree(x, (int)height);
                        }
                    }
                }
                else if (percentage <= 0.68f)
                {
                    if (thirdFloorShouldBeReadTwice)
                    {
                        if (chanceOfCreatingFloor > 0) 
                            chanceOfCreatingFloor -= 1;
                        else
                        {
                            int offY = Main.rand.Next(-5, 6);
                            int placeLength = Main.rand.Next(60, 80);
                            int floorHeight = Main.rand.Next(3, 5);

                            for (int i = -placeLength / 2; i < placeLength / 2; i++)
                            {
                                if (x + i < start || x + i >= end) 
                                    continue;

                                if (i < -placeLength + 2 || i > placeLength / 2 - 3) 
                                    WorldGen.PlaceTile(x, (int)height + offY, ModContent.TileType<AbyssStone>());

                                WorldGen.PlaceTile(x + i, (int)height + 1 + offY, ModContent.TileType<AbyssStone>());

                                if (i > -placeLength / 2 + 2 && i < placeLength / 2 - 3)
                                {
                                    WorldGen.PlaceTile(x + i, (int)height + 2 + offY, ModContent.TileType<AbyssStone>());
                                    if (Main.rand.NextBool(3))
                                    {
                                        WorldGen.PlaceTile(x + i, (int)height + 3 + offY, ModContent.TileType<AbyssStone>());
                                        WorldGen.PlaceTile(x + i, (int)height + 4 + offY, ModContent.TileType<AbyssStone>());
                                        WorldGen.PlaceTile(x + i, (int)height + 5 + offY, ModContent.TileType<AbyssStone>());
                                    }
                                }

                                if (i > -placeLength / 2 + 3 && i < placeLength / 2 - 4)
                                {
                                    if (floorHeight >= 4 || Main.rand.NextBool(2)) 
                                        WorldGen.PlaceTile(x + i, (int)height + 3 + offY, ModContent.TileType<AbyssStone>());
                                    
                                    if (floorHeight >= 4 && Main.rand.NextBool(2)) 
                                        WorldGen.PlaceTile(x + i, (int)height + 4 + offY, ModContent.TileType<AbyssStone>());
                                    
                                    if (Main.rand.NextBool(3))
                                    {
                                        WorldGen.PlaceTile(x + i, (int)height + 4 + offY, ModContent.TileType<AbyssStone>());
                                        WorldGen.PlaceTile(x + i, (int)height + 5 + offY, ModContent.TileType<AbyssStone>());
                                        WorldGen.PlaceTile(x + i, (int)height + 6 + offY, ModContent.TileType<AbyssStone>());
                                    }
                                }
                            }
                            
                            chanceOfCreatingFloor = Main.rand.Next(60, 100);

                        }
                    }
                    else if (x > 90 && x < Main.maxTilesX - 90)
                        tile.ClearTile();
                }
                else
                {
                    if (!thirdFloorShouldBeReadTwice)
                    {
                        height = (float)(Main.worldSurface + (Main.maxTilesY - 200 - 100 - Main.worldSurface) * 0.36f) + 40;
                        thirdFloorShouldBeReadTwice = true;
                        break;
                    }
                }
            }

            progress.Set(Utils.GetLerpValue((float)Main.worldSurface, Main.maxTilesY - 200 - 100, height));

            if (percentage > 0.36f && percentage <= 0.68f && thirdFloorShouldBeReadTwice) 
                height += Main.rand.Next(20, 35);
        }

        Language.GetTextValue("Mods.HomewardSubworld.Generation.Aging");

        for (height = (float)Main.worldSurface; height < Main.maxTilesY - 200 - 100; height += 1)
        {
            for (int x = start; x < end; x += 1)
            {
                if (WorldGen.TileType(x, (int)height) == ModContent.TileType<AbyssStone>() && Main.tile[x, (int)height].BlockType == BlockType.Solid &&
                    !Main.tile[x, (int)height - 1].HasTile && !Main.tile[x, (int)height - 2].HasTile)
                {
                    if (Main.rand.Next(1, 24) == 1)
                    {
                        WorldGen.Place1x2(x, (int)height - 1, (ushort)ModContent.TileType<AbyssRock_1>(), 0);
                    }

                    if (Main.rand.Next(1, 24) == 2)
                    {
                        WorldGen.Place1x2(x, (int)height - 1, (ushort)ModContent.TileType<AbyssRock_2>(), 0);
                    }

                    if (Main.rand.Next(1, 24) == 3)
                    {
                        WorldGen.Place1x1(x, (int)height - 1, (ushort)ModContent.TileType<AbyssRock_6>(), 0);
                    }
                }

                if (WorldGen.TileType(x, (int)height) == ModContent.TileType<AbyssStone>() && Main.tile[x, (int)height].BlockType == BlockType.Solid &&
                    !Main.tile[x, (int)height + 1].HasTile && !Main.tile[x, (int)height + 2].HasTile)
                {
                    if (WorldGen.genRand.NextBool(23))
                    {
                        WorldGen.Place1x2Top(x, (int)height + 1, (ushort)ModContent.TileType<AbyssRock_3>(), 0);
                    }

                    if (WorldGen.genRand.NextBool(23))
                    {
                        WorldGen.Place1x2Top(x, (int)height + 1, (ushort)ModContent.TileType<AbyssRock_4>(), 0);
                    }
                }
            }
        }
    }
}