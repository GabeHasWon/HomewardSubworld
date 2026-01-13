using ContinentOfJourney;
using ContinentOfJourney.NPCs.Boss_Diver;
using ContinentOfJourney.NPCs.Boss_WallofShadow;
using ContinentOfJourney.Tiles;
using MonoMod.RuntimeDetour;
using SubworldLibrary;
using System;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace HomewardSubworld;

public class HomewardSubworld : Mod
{
    public class PacketID
    {
        public static readonly byte SendBossDown = 0;
    }

    internal static readonly MethodInfo DoDeathEventsInfo = typeof(NPC).GetMethod("DoDeathEvents", BindingFlags.Instance | BindingFlags.NonPublic);

    private static Hook ChooseSpawnHook = null;

    public override void Load()
    {
        ChooseSpawnHook = new(typeof(NPCLoader).GetMethod(nameof(NPCLoader.ChooseSpawn), BindingFlags.Public | BindingFlags.Static), ChooseSpawnDetour, true);
        On_NPC.DoDeathEvents += HijackDeathEffects;
    }

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

    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        byte packetType = reader.ReadByte();

        Logger.Debug($"[Abyssal Subworld]: Got packet {packetType}");

        if (packetType == 0)
        {
            short type = reader.ReadInt16();

            var npc = new NPC();
            npc.SetDefaults(type);
            npc.Center = Main.player[0].Center;
            DoDeathEventsInfo.Invoke(npc, [Main.player[0]]);

            if (type == ModContent.NPCType<WallofShadow>())
            {
                ref bool wallOfShadowGen = ref ModContent.GetInstance<BarrierTrackingSystem>().wallOfShadowGen;

                if (SubworldSystem.Current is null && !wallOfShadowGen)
                    AbyssalPlayer.GenerateWallOfShadow(ref wallOfShadowGen);
            }
        }
    }

    private static void HijackDeathEffects(On_NPC.orig_DoDeathEvents orig, NPC self, Player closestPlayer)
    {
        if (SubworldSystem.Current is AbyssalSubworld && (self.type == ModContent.NPCType<Diver>() || self.type == ModContent.NPCType<WallofShadow>()))
        {
            // Automatically add/send the boss downed cache/packet
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                ModPacket packet = ModContent.GetInstance<HomewardSubworld>().GetPacket(5);
                packet.Write(PacketID.SendBossDown);
                packet.Write(self.type);
                SendPacketToMainServer(packet);

                self.type = NPCID.None;
                self.boss = false;
            }
            else if (self.type == ModContent.NPCType<WallofShadow>())
                Main.LocalPlayer.GetModPlayer<AbyssalPlayer>().holdsWallofShadowDeath = true;
        }

        orig(self, closestPlayer);
    }

    internal static void SendPacketToMainServer(ModPacket packet)
    {
        byte[] data = (packet.BaseStream as MemoryStream).GetBuffer();
        data = data[4..]; // Packets have a bunch of garbage data for some reason?
        SubworldSystem.SendToMainServer(ModContent.GetInstance<HomewardSubworld>(), data);
    }
}

public class AbyssalPlayer : ModPlayer
{
    public bool holdsWallofShadowDeath = false;

    public override void OnEnterWorld()
    {
        if (Main.netMode != NetmodeID.SinglePlayer)
            return;

        ref bool wallOfShadowGen = ref ModContent.GetInstance<BarrierTrackingSystem>().wallOfShadowGen;

        if (SubworldSystem.Current is null && holdsWallofShadowDeath && !wallOfShadowGen)
            GenerateWallOfShadow(ref wallOfShadowGen);
    }

    internal static void GenerateWallOfShadow(ref bool wallOfShadowGen)
    {
        wallOfShadowGen = true;

        for (int i = 0; i < Main.maxTilesX; i++)
        {
            int num = WorldGen.genRand.Next(0, Main.maxTilesX);
            int num2 = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 300);

            if (Main.tile[num, num2].TileType is 53 or 397 or 404 or 396 or 234 or 112 or 116 or 400 or 401 or 403)
            {
                WorldGen.OreRunner(num, num2, WorldGen.genRand.Next(4, 8), WorldGen.genRand.Next(10, 16), (ushort)ModContent.TileType<EternalOre>());
            }

            if (Main.tile[num, num2].TileType is 59 or 123 or 60)
            {
                WorldGen.OreRunner(num, num2, WorldGen.genRand.Next(4, 10), WorldGen.genRand.Next(4, 12), (ushort)ModContent.TileType<LivingOre>());
            }

            if (Main.tile[num, num2].TileType is 161 or 147 or 163 or 200 or 164)
            {
                WorldGen.OreRunner(num, num2, 4.0, WorldGen.genRand.Next(10, 24), (ushort)ModContent.TileType<CubistOre>());
            }
        }
    }

    public override void SaveData(TagCompound tag) => tag.Add("holdsWall", holdsWallofShadowDeath);
    public override void LoadData(TagCompound tag) => holdsWallofShadowDeath = tag.GetBool("holdsWall");
}

public class BarrierTrackingSystem : ModSystem
{
    public bool wallOfShadowGen = false;

    public override void ClearWorld() => wallOfShadowGen = false;

    public override void SaveWorldData(TagCompound tag) => tag.Add("wallOfShadowGen", wallOfShadowGen);
    public override void LoadWorldData(TagCompound tag) => wallOfShadowGen = tag.GetBool("wallOfShadowGen");
    public override void NetSend(BinaryWriter writer) => writer.Write(wallOfShadowGen);
    public override void NetReceive(BinaryReader reader) => wallOfShadowGen = reader.ReadBoolean();
}