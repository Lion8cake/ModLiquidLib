using ModLiquidLib.ModLoader;
using ReLogic.Reflection;
using System;
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

			public static int[] CreateLiquidBucketItem = Factory.CreateNamedSet("CreateLiquidBucketItem")
			.Description("The Item created when a bucket is used on a liquid selected")
			.RegisterIntSet(-1, Water, ItemID.WaterBucket, Lava, ItemID.LavaBucket, Honey, ItemID.HoneyBucket);
		}

		public const short Water = 0;

		public const short Lava = 1;

		public const short Honey = 2;

		public const short Shimmer = 3;

		public static readonly short Count = 4;

		public static readonly IdDictionary Search = IdDictionary.Create<LiquidID_TLmod, short>();
	}
}
