using ModLiquidLib.ModLoader;
using ReLogic.Reflection;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModLiquidLib.ID
{
	public class LiquidID_TLmod //just imagine that the _ is a . and this file is to edit the already existing LiquidID (renamed to not be ambigious between this and LiquidID)
	{
		[ReinitializeDuringResizeArrays]
		public static class Sets
		{
			public static SetFactory Factory = new SetFactory(LiquidLoader.LiquidCount, "LiquidID", Search);

			/// <summary> Whether or not this tile counts as a this liquid's source for crafting purposes. </summary>
			public static bool[][] CountsAsLiquidSource = TileID.Sets.Factory.CreateNamedSet("CountsAsLiquidSource")
			.Description("Indicates if the tile nearby counts towards a certain liquid  when crafting.")
			.RegisterCustomSet(new bool[4]);

			/// <summary> Whether or not the tile is non-solid to liquids, allowing them to flow through. </summary>
			public static bool[] IgnoresWater = TileID.Sets.Factory.CreateNamedSet("IgnoresWater")
			.Description("Whether or not the tile is non-solid to liquids, allowing them to flow through.")
			.RegisterBoolSet(138, 484, 546);

			/// <summary> Whether or not the tile is non-solid to liquids during worldgen. </summary>
			public static bool[] IgnoresWaterDuringWorldgen = TileID.Sets.Factory.CreateNamedSet("IgnoresWaterDuringWorldgen")
			.Description("Whether or not the tile is non-solid to liquids during worldgen.")
			.RegisterBoolSet(10, 192, 191, 190);

			/// <summary> The Item created when a bucket is used on a liquid selected </summary>
			public static int[] CreateLiquidBucketItem = Factory.CreateNamedSet("CreateLiquidBucketItem")
			.Description("The Item created when a bucket is used on a liquid selected")
			.RegisterIntSet(-1, Water, ItemID.WaterBucket, Lava, ItemID.LavaBucket, Honey, ItemID.HoneyBucket);

			/// <summary> Whats Items a liquid can be absorbed by. Used for defining sponges. </summary>
			public static List<int>[] CanBeAbsorbedBy = Factory.CreateNamedSet("CanBeAbsorbedBy")
			.Description("Whats Items a liquid can be absorbed by. Used for defining sponges.")
			.RegisterCustomSet<List<int>>(null,
				LiquidID.Water, new List<int>() { ItemID.SuperAbsorbantSponge, ItemID.UltraAbsorbantSponge },
				LiquidID.Shimmer, new List<int>() { ItemID.SuperAbsorbantSponge, ItemID.UltraAbsorbantSponge },
				LiquidID.Lava, new List<int>() { ItemID.LavaAbsorbantSponge, ItemID.UltraAbsorbantSponge },
				LiquidID.Honey, new List<int>() { ItemID.HoneyAbsorbantSponge, ItemID.UltraAbsorbantSponge });

			/// <summary> Whether or not the NPC can spawn in the liquid specified. NOTE: only works for modded liquids and npcs </summary>
			public static bool[][] CanModdedNPCSpawnInModdedLiquid = NPCID.Sets.Factory.CreateNamedSet("CanModdedNPCSpawnInModdedLiquid")
				.Description("Whether or not the NPC can spawn in the liquid specified. NOTE: only works for modded liquids and npcs")
				.RegisterCustomSet(new bool[4]);

			/// <summary> Whether or not the liquid uses the water loot pool when fished it. NOTE: this set does not effect/change Water, lava or honey and is mainly used for modded liquids. </summary>
			public static bool[] UsesWaterFishingLootPool = Factory.CreateNamedSet("UsesWaterFishingLootPool")
				.Description("Whether or not the liquid uses the water loot pool when fished it. NOTE: this set does not effect/change Water, lava or honey and is mainly used for modded liquids.")
				.RegisterBoolSet(false);

			/// <summary> Whether or not the tile is non-solid to liquids, allowing them to flow through. This is a list containing the tiles that CAN ignore water. DO NOT add your tile to this list, please set it in IgnoresWater.</summary>
			public static List<int> IgnoresWaterList;

			/// <summary> Whether or not the tile is non-solid to liquids during worldgen. This is a list containing the tiles that CAN ignore water during worldgen. DO NOT add your tile to this list, please set it in IgnoresWaterDuringWorldgen.</summary>
			public static List<int> IgnoresWaterDuringWorldgenList;

			static Sets()
			{
				for (int i = 0; i < CanBeAbsorbedBy.Length; i++)
				{
					if (CanBeAbsorbedBy[i] == null)
					{
						CanBeAbsorbedBy[i] = new List<int>() { ItemID.UltraAbsorbantSponge };
					}
				}
			}
		}

		public const short Water = 0;

		public const short Lava = 1;

		public const short Honey = 2;

		public const short Shimmer = 3;

		public static readonly short Count = 4;

		public static readonly IdDictionary Search = IdDictionary.Create<LiquidID_TLmod, short>();
	}
}
