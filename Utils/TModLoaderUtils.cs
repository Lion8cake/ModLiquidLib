using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils.LiquidContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace ModLiquidLib.Utils
{
    //internalised to prevent malicious users from accessing tmodloader functions
    public static class TModLoaderUtils
	{
		internal static void Load()
		{
			_addliquidCounts = typeof(SceneMetrics).GetField("_liquidCounts", BindingFlags.NonPublic | BindingFlags.Instance);
			tileEntries = (IDictionary<ushort, IList<Terraria.ModLoader.MapEntry>>)typeof(MapLoader).GetField("tileEntries", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
			wallEntries = (IDictionary<ushort, IList<Terraria.ModLoader.MapEntry>>)typeof(MapLoader).GetField("wallEntries", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
		}

		internal static void Unload()
		{
			_addliquidCounts = null;
			wallEntries = null;
		}

		internal static FieldInfo _addliquidCounts;
		internal static IDictionary<ushort, IList<Terraria.ModLoader.MapEntry>> tileEntries;
		internal static IDictionary<ushort, IList<Terraria.ModLoader.MapEntry>> wallEntries;

		internal static int[] Get_liquidCounts(this SceneMetrics self)
		{
			if (_addliquidCounts.GetValue(self) is int[] _liquidCounts)
			{
				return _liquidCounts;
			}
			return null;
		}

		internal static void Set_liquidCounts(this SceneMetrics self, int[] _liquidCounts)
		{
			if (_addliquidCounts.GetValue(self) is int[])
			{
				_addliquidCounts.SetValue(self, _liquidCounts);
			}
		}

		internal static void BuildGlobalHook<T, F>(ref F[] list, IList<T> providers, Expression<Func<T, F>> expr) where F : Delegate
		{
			LoaderUtils.MethodOverrideQuery<T> query = expr.ToOverrideQuery();
			list = (from t in providers.Where(query.HasOverride)
					select (F)query.Binder(t)).ToArray();
		}

		/// <summary>
		/// The X position of the Tile
		/// </summary>
		/// <param name="tile"></param>
		/// <returns></returns>
		public static int X(this Tile tile)
		{
			TilePos(tile, out int x, out _);
			return x;
		}

		/// <summary>
		/// The Y position of the Tile
		/// </summary>
		/// <param name="tile"></param>
		/// <returns></returns>
		public static int Y(this Tile tile)
		{
			TilePos(tile, out _, out int y);
			return y;
		}

		/// <summary>
		/// Gets the Position of the tile, the same values that would be inputted in Main.tile to get this Tile
		/// </summary>
		/// <param name="tile"></param>
		/// <param name="x">The outputted X value, if you want the X by itself use Tile.X</param>
		/// <param name="y">The outputted Y value, if you want the Y by itself use Tile.Y</param>
		public static void TilePos(this Tile tile, out int x, out int y)
		{
			uint tileId = Unsafe.BitCast<Tile, uint>(tile);
			x = Math.DivRem((int)tileId, Main.tile.Height, out y); //Thanks to FoxXD_ for the help with this
		}

		public static bool GetAdjLiquids(this Player player, int Liquid)
		{
			return player.GetModPlayer<ModLiquidPlayer>().AdjLiquid[Liquid];
		}

		public static bool[] GetModdedWetArray(this Player player)
		{
			return player.GetModPlayer<ModLiquidPlayer>().moddedWet;
		}

		public static bool[] GetModdedWetArray(this NPC npc)
		{
			return npc.GetGlobalNPC<ModLiquidNPC>().moddedWet;
		}

		public static bool[] GetModdedWetArray(this Projectile proj)
		{
			return proj.GetGlobalProjectile<ModLiquidProjectile>().moddedWet;
		}

		public static bool[] GetModdedWetArray(this Item item)
		{
			return item.GetGlobalItem<ModLiquidItem>().moddedWet;
		}

		public static bool[] GetModdedWetArray(this Entity entity)
		{
			if (entity is Player player)
			{
				return player.GetModPlayer<ModLiquidPlayer>().moddedWet;
			}
			if (entity is NPC npc)
			{
				return npc.GetGlobalNPC<ModLiquidNPC>().moddedWet;
			}
			if (entity is Projectile proj)
			{
				return proj.GetGlobalProjectile<ModLiquidProjectile>().moddedWet;
			}
			if (entity is Item item)
			{
				return item.GetGlobalItem<ModLiquidItem>().moddedWet;
			}
			return new bool[LiquidLoader.LiquidCount - LiquidID.Count];
		}

		public static void TryFloatingInFluid(this Player player)
		{
			player.TryFloatingInFluid();
		} 
	}
}
