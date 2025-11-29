using ContinentOfJourney;
using ContinentOfJourney.Tiles.Paintings;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace HomewardSubworld.Generation;

/// <summary>
/// Generation adapted from Homeward Journey. Used with permission.
/// </summary>
public class AbyssChestPass(string name, float loadWeight) : GenPass(name, loadWeight)
{
    private readonly int[,] _shrine1 = {
            {9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9 },
            {9,9,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,9,9 },
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
            {1,2,4,2,2,2,2,2,2,2,2,2,2,2,2,2,2,4,2,1 },
            {0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0 },
            {0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0 },
            {0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0 },
            {1,1,1,1,1,1,1,1,1,1,1,3,1,1,1,1,1,1,1,1 },
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
            {9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9 },

        };
    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        if (DownedBossSystem.isBackdoorWorld) 
            return;

        progress.Message = "Modifying the Abyss";

        int centerOfGeneration = Main.maxTilesX / 2;
        int passwordNum = 2;
        int passwordPlaced = 0;
        int attempt = 0;
        int lBorder = 0;
        int rBorder = Main.maxTilesX;

        if (GenVars.dungeonSide > 0) 
            lBorder = (int)(Main.maxTilesX * 0.66f);
        else 
            rBorder = (int)(Main.maxTilesX * 0.33f);

        while (passwordPlaced < passwordNum && attempt < 10000)
        {
            int placeX = WorldGen.genRand.Next(lBorder, rBorder);
            int placeY = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 200);
            if (!Main.tile[placeX, placeY].HasTile && Main.wallDungeon[Main.tile[placeX, placeY].WallType])
            {
                bool placedAltar = //(Main.tile[placeX, placeY - 1].HasTile && Main.tile[placeX, placeY - 1].TileType == TileType<PickaxeShelf>());
                    WorldGen.PlaceObject(placeX, placeY - 1, ModContent.TileType<PaintingGiant>(), false, 11);
                if (placedAltar) 
                    passwordPlaced++;
            }

            //ContinentOfJourney.Instance.Logger.Warn("Flag3 - " + attempt.ToString());
            attempt++;
        }

        int rocklayer = (int)(Main.maxTilesY - 200 - Main.rockLayer);
        int chestfirstitem = 1;
        attempt = 0;

        for (int c = (int)Main.rockLayer - rocklayer / 4; c < Main.rockLayer + rocklayer / 2 - 100; c += 64)
        {
            int j = c + 24 + (int)(24 - Main.rand.NextFloat() * 48);
            float percentage = (float)(j - Main.worldSurface) / (float)(Main.maxTilesY - 200 - 100 - Main.worldSurface);
            if (percentage > 0.36f) 
                continue;

            pasta4:
            int i = centerOfGeneration + Main.rand.Next(-60, 61) * 2;
            for (int y = 0; y < _shrine1.GetLength(0); y++)
            {
                for (int x = 0; x < _shrine1.GetLength(1); x++)
                {
                    int k = i - 10 + x;
                    int l = j - 5 + y;
                    Tile tile = Framing.GetTileSafely(k, l);
                    if (tile.TileType == TileID.BlueDungeonBrick ||
                        tile.TileType == TileID.PinkDungeonBrick ||
                        tile.TileType == TileID.GreenDungeonBrick)
                    {
                        attempt += 1;
                        if (attempt < 150)
                        {
                            goto pasta4;
                        }
                    }
                }
            }

            attempt = 0;
            for (int y = 0; y < _shrine1.GetLength(0); y++)
            {
                for (int x = 0; x < _shrine1.GetLength(1); x++)
                {
                    int k = i - 10 + x;
                    int l = j - 5 + y;
                    if (WorldGen.InWorld(k, l, 30))
                    {
                        Tile tile = Framing.GetTileSafely(k, l);
                        switch (_shrine1[y, x])
                        {
                            case 0:
                                tile.HasTile = false;
                                break;
                            case 1:
                                tile.TileType = (ushort)ModContent.TileType<ContinentOfJourney.Tiles.Abyss.AbyssBrick>();
                                tile.Slope = 0;
                                tile.IsHalfBlock = false;
                                tile.HasTile = true;
                                break;
                            case 2:
                                tile.HasTile = false;
                                WorldGen.KillWall(k, l);
                                WorldGen.PlaceWall(k, l, ModContent.WallType<ContinentOfJourney.Tiles.Abyss.AbyssBrickWall>());
                                break;
                            case 3:
                                tile.TileType = (ushort)ModContent.TileType<ContinentOfJourney.Tiles.Abyss.AbyssBrick>();
                                tile.Slope = 0;
                                tile.IsHalfBlock = false;
                                tile.HasTile = true;

                                int chestIndex = WorldGen.PlaceChest(k - 2, l - 1, (ushort)ModContent.TileType<ContinentOfJourney.Tiles.Abyss.AbyssChest>(), false, 1);

                                if (chestIndex != -1)
                                {
                                    Chest chest = Main.chest[chestIndex];
                                    var itemsToAdd = new List<(int type, int stack)>();

                                    // Using a switch statement and a random choice to add sets of items.
                                    switch (chestfirstitem)
                                    {
                                        case 1:
                                            itemsToAdd.Add((ModContent.ItemType<ContinentOfJourney.Items.QuickBoomerang>(), 1));
                                            break;
                                        case 2:
                                            itemsToAdd.Add((ModContent.ItemType<ContinentOfJourney.Items.AncientTome>(), 1));
                                            break;
                                        case 3:
                                            itemsToAdd.Add((ModContent.ItemType<ContinentOfJourney.Items.VoidStorm>(), 1));
                                            break;
                                        case 4:
                                            itemsToAdd.Add((ModContent.ItemType<ContinentOfJourney.Items.Threepeater>(), 1));
                                            break;
                                        case 5:
                                            itemsToAdd.Add((ModContent.ItemType<ContinentOfJourney.Items.Accessories.AbyssCore>(), 1));
                                            break;
                                    }

                                    chestfirstitem += 1;

                                    if (chestfirstitem > 5)
                                    {
                                        chestfirstitem = 1;
                                    }

                                    switch (Main.rand.Next(1, 10))
                                    {
                                        case 1:
                                            itemsToAdd.Add((ItemID.SummoningPotion, Main.rand.Next(1, 3)));
                                            break;
                                        case 2:
                                            itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
                                            break;
                                        case 3:
                                            itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                            break;
                                        case 4:
                                            itemsToAdd.Add((ItemID.RagePotion, Main.rand.Next(1, 3)));
                                            break;
                                        case 5:
                                            itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 3)));
                                            break;
                                        case 6:
                                            itemsToAdd.Add((ItemID.AmmoReservationPotion, Main.rand.Next(1, 3)));
                                            break;
                                        case 7:
                                            itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 3)));
                                            break;
                                        case 8:
                                            itemsToAdd.Add((ItemID.MagicPowerPotion, Main.rand.Next(1, 3)));
                                            break;
                                        case 9:
                                            itemsToAdd.Add((ItemID.ManaRegenerationPotion, Main.rand.Next(1, 3)));
                                            break;
                                    }

                                    if (Main.rand.Next(1, 3) == 1)
                                    {
                                        switch (Main.rand.Next(1, 10))
                                        {
                                            case 1:
                                                itemsToAdd.Add((ItemID.SummoningPotion, Main.rand.Next(1, 3)));
                                                break;
                                            case 2:
                                                itemsToAdd.Add((ItemID.EndurancePotion, Main.rand.Next(1, 3)));
                                                break;
                                            case 3:
                                                itemsToAdd.Add((ItemID.SwiftnessPotion, Main.rand.Next(1, 3)));
                                                break;
                                            case 4:
                                                itemsToAdd.Add((ItemID.RagePotion, Main.rand.Next(1, 3)));
                                                break;
                                            case 5:
                                                itemsToAdd.Add((ItemID.ArcheryPotion, Main.rand.Next(1, 3)));
                                                break;
                                            case 6:
                                                itemsToAdd.Add((ItemID.AmmoReservationPotion, Main.rand.Next(1, 3)));
                                                break;
                                            case 7:
                                                itemsToAdd.Add((ItemID.InfernoPotion, Main.rand.Next(1, 3)));
                                                break;
                                            case 8:
                                                itemsToAdd.Add((ItemID.MagicPowerPotion, Main.rand.Next(1, 3)));
                                                break;
                                            case 9:
                                                itemsToAdd.Add((ItemID.ManaRegenerationPotion, Main.rand.Next(1, 3)));
                                                break;
                                        }
                                    }

                                    switch (Main.rand.Next(1, 4))
                                    {
                                        case 1:
                                            itemsToAdd.Add((ItemID.FairyGlowstick, Main.rand.Next(8, 17)));
                                            break;
                                        case 2:
                                            itemsToAdd.Add((ItemID.SpelunkerGlowstick, Main.rand.Next(8, 17)));
                                            break;
                                        case 3:
                                            itemsToAdd.Add((ModContent.ItemType<ContinentOfJourney.Items.Placables.AbyssTorch>(), Main.rand.Next(8, 17)));
                                            break;
                                    }

                                    if (Main.rand.Next(1, 3) == 1)
                                    {
                                        switch (Main.rand.Next(1, 3))
                                        {
                                            case 1:
                                                itemsToAdd.Add((ItemID.AdamantiteBar, Main.rand.Next(10, 19)));
                                                break;
                                            case 2:
                                                itemsToAdd.Add((ItemID.TitaniumBar, Main.rand.Next(10, 19)));
                                                break;
                                        }
                                    }

                                    switch (Main.rand.Next(1, 4))
                                    {
                                        case 1:
                                            itemsToAdd.Add((ItemID.Torch, Main.rand.Next(8, 17)));
                                            break;
                                        case 2:
                                            itemsToAdd.Add((ItemID.Glowstick, Main.rand.Next(8, 17)));
                                            break;
                                    }

                                    if (Main.rand.Next(1, 6) == 1)
                                    {
                                        itemsToAdd.Add((ItemID.GoldCoin, Main.rand.Next(1, 6)));
                                    }
                                    else
                                    {
                                        itemsToAdd.Add((ItemID.SilverCoin, Main.rand.Next(50, 100)));
                                    }

                                    int chestItemIndex = 0;
                                    foreach (var (type, stack) in itemsToAdd)
                                    {
                                        Item item = new Item();
                                        item.SetDefaults(type);
                                        item.stack = stack;
                                        chest.item[chestItemIndex] = item;
                                        chestItemIndex++;
                                        if (chestItemIndex >= 40)
                                            break;
                                    }
                                }

                                break;
                            case 4:
                                tile.ClearTile();
                                WorldGen.Place1x1(k, l, (ushort)ModContent.TileType<ContinentOfJourney.Tiles.Abyss.AbyssTorch>());
                                WorldGen.KillWall(k, l);
                                WorldGen.PlaceWall(k, l, ModContent.WallType<ContinentOfJourney.Tiles.Abyss.AbyssBrickWall>());
                                break;
                        }
                    }
                }
            }
        }
    }
}
