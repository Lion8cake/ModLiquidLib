﻿using Microsoft.Xna.Framework;
using ModLiquidLib.ModLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace ModLiquidLib.Utils
{
    //internalised to prevent malicious users from accessing tmodloader functions
    internal static class TModLoaderUtils
	{
		public static void Load()
		{
			_addliquidCounts = typeof(SceneMetrics).GetField("_liquidCounts", BindingFlags.NonPublic | BindingFlags.Instance);
			tileEntries = (IDictionary<ushort, IList<Terraria.ModLoader.MapEntry>>)typeof(MapLoader).GetField("tileEntries", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
			wallEntries = (IDictionary<ushort, IList<Terraria.ModLoader.MapEntry>>)typeof(MapLoader).GetField("wallEntries", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
		}

		public static void Unload()
		{
			_addliquidCounts = null;
			wallEntries = null;
		}

		public static FieldInfo _addliquidCounts;
		public static IDictionary<ushort, IList<Terraria.ModLoader.MapEntry>> tileEntries;
		public static IDictionary<ushort, IList<Terraria.ModLoader.MapEntry>> wallEntries;

		public static int[] Get_liquidCounts(this SceneMetrics self)
		{
			if (_addliquidCounts.GetValue(self) is int[] _liquidCounts)
			{
				return _liquidCounts;
			}
			return null;
		}

		public static void Set_liquidCounts(this SceneMetrics self, int[] _liquidCounts)
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
	}
}
