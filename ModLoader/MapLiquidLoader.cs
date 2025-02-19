using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using System.Reflection;
using ModLiquidLib.Utils;

namespace ModLiquidLib.ModLoader
{
	internal static class MapLiquidLoader
	{
		internal static bool initialized = false;

		internal static readonly IDictionary<ushort, IList<MapEntry>> liquidEntries = new Dictionary<ushort, IList<MapEntry>>();

		internal static readonly IDictionary<ushort, ushort> entryToLiquid = new Dictionary<ushort, ushort>();

		public static ushort[] liquidLookup;

		internal static int modTileOptions(ushort type)
		{
			return TModLoaderUtils.tileEntries[type].Count;
		}

		internal static int modWallOptions(ushort type)
		{
			return TModLoaderUtils.wallEntries[type].Count;
		}

		internal static int modLiquidOptions(ushort type)
		{
			return liquidEntries[type].Count;
		}

		internal static void FinishSetup()
		{
			if (Main.dedServ)
			{
				return;
			}
			initialized = true;
		}

		internal static void UnloadModMap()
		{
			liquidEntries.Clear();
			if (!Main.dedServ)
			{
				entryToLiquid.Clear();
				Array.Resize(ref liquidLookup, LiquidID.Count);
				initialized = false;
			}
		}

		internal static void ModMapOption(ref ushort mapType, int i, int j)
		{
			if (entryToLiquid.ContainsKey(mapType))
			{
				ModLiquid liquid = LiquidLoader.GetLiquid(entryToLiquid[mapType]);
				ushort option = liquid.GetMapOption(i, j);
				if (option < 0 || option >= modLiquidOptions(liquid.Type))
				{
					throw new ArgumentOutOfRangeException("Bad map option for liquid " + liquid.Name + " from mod " + liquid.Mod.Name);
				}
				mapType += option;
			}
		}
	}
}
