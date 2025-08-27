using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils.LiquidContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.GameContent.Liquid;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace ModLiquidLib.Utils
{
    public static class TModLoaderUtils
	{
		internal static void Load()
		{
			_addliquidCounts = typeof(SceneMetrics).GetField("_liquidCounts", BindingFlags.NonPublic | BindingFlags.Instance);
		}

		internal static void Unload()
		{
			_addliquidCounts = null;
		}

		internal static FieldInfo _addliquidCounts;

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

		public static bool[] GetWalkableLiquids(this Player player)
		{
			return player.GetModPlayer<ModLiquidPlayer>().canLiquidBeWalkedOn;
		}

		public static void SetWalkableLiquids(this Player player, bool[] walkableLiquids)
		{
			 Array.Copy(player.GetModPlayer<ModLiquidPlayer>().canLiquidBeWalkedOn, walkableLiquids, LiquidLoader.LiquidCount);
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
			item.TryGetGlobalItem(out ModLiquidItem liquidItem);
			return liquidItem.moddedWet;
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
				item.TryGetGlobalItem(out ModLiquidItem liquidItem);
				return liquidItem.moddedWet;
			}
			return new bool[LiquidLoader.LiquidCount - LiquidID.Count];
		}

		public static void TryFloatingInFluid(this Player player)
		{
			player.TryFloatingInFluid();
		} 

		public static int GetAnimationFrame(this LiquidRenderer liquidDrawing)
		{
			return liquidDrawing._animationFrame;
		}

		public static void SetAnimationFrame(this LiquidRenderer liquidDrawing, int animationFrame)
		{
			liquidDrawing._animationFrame = animationFrame;
		}

		public static void DrawWaterfall(this WaterfallManager self, int waterfallType, int x, int y, float opacity, Vector2 position, Rectangle sourceRect, Color color, SpriteEffects effects)
		{
			self.DrawWaterfall(waterfallType, x, y, opacity, position, sourceRect, color, effects);
		}

		public static Color StylizeColor(this WaterfallManager self, float alpha, int maxSteps, int waterfallType, int y, int s, Tile tileCache, Color aColor)
		{
			return WaterfallManager.StylizeColor(alpha, maxSteps, waterfallType, y, s, tileCache, aColor);
		}

		public static float GetAlpha(this WaterfallManager self, float Alpha, int maxSteps, int waterfallType, int y, int s, Tile tileCache)
		{
			return WaterfallManager.GetAlpha(Alpha, maxSteps, waterfallType, y, s, tileCache);
		}

		public static void TrySparkling(this WaterfallManager self, int x, int y, int direction, Color aColor2)
		{
			WaterfallManager.TrySparkling(x, y, direction, aColor2);
		}

		public static void AddLight(this WaterfallManager self, int waterfallType, int x, int y)
		{
			WaterfallManager.AddLight(waterfallType, x, y);
		}

		public static int GetWFallFrame(this WaterfallManager self, int type)
		{
			if (type >= ID.WaterfallID.Count)
			{
				return LiquidFallLoader.wFallFrame[type];
			}
			else if (type == ID.WaterfallID.Lava || type == ID.WaterfallID.Honey || type == ID.WaterfallID.Shimmer)
			{
				return self.slowFrame;
			}
			else if (type == ID.WaterfallID.Rain)
			{
				return self.rainFrameForeground;
			}
			else if (type == ID.WaterfallID.Snow)
			{
				return self.snowFrameForeground;
			}
			else
			{
				return self.regularFrame;
			}
		}

		public static int GetWFallFrameBack(this WaterfallManager self, int type)
		{
			if (type >= ID.WaterfallID.Count)
			{
				return LiquidFallLoader.wFallFrameBack[type];
			}
			else if (type == ID.WaterfallID.Rain)
			{
				return self.rainFrameBackground;
			}
			else
			{
				ModContent.GetInstance<ModLiquidLib>().Logger.Warn("Attempting to get a waterfall back frame for a vanilla waterfall that does not contain a backframe frame.");
				return 0;
			}
		}

		public static int GetWFallFrameCounter(this WaterfallManager self, int type)
		{
			if (type >= ID.WaterfallID.Count)
			{
				return LiquidFallLoader.wFallFrameCounter[type];
			}
			else if (type == ID.WaterfallID.Lava || type == ID.WaterfallID.Honey || type == ID.WaterfallID.Shimmer)
			{
				return self.wFallFrCounter2;
			}
			else if (type == ID.WaterfallID.Rain)
			{
				return self.rainFrameCounter;
			}
			else if (type == ID.WaterfallID.Snow)
			{
				return self.snowFrameCounter;
			}
			else
			{
				return self.wFallFrCounter;
			}
		}

		public static void SetWFallFrame(this WaterfallManager self, int type, int frame)
		{
			if (type >= ID.WaterfallID.Count)
			{
				LiquidFallLoader.wFallFrame[type] = frame;
			}
			else if (type == ID.WaterfallID.Lava || type == ID.WaterfallID.Honey || type == ID.WaterfallID.Shimmer)
			{
				self.slowFrame = frame;
			}
			else if (type == ID.WaterfallID.Rain)
			{
				self.rainFrameForeground = frame;
			}
			else if (type == ID.WaterfallID.Snow)
			{
				self.snowFrameForeground = frame;
			}
			else
			{
				self.regularFrame = frame;
			}
		}

		public static void SetWFallFrameBack(this WaterfallManager self, int type, int frame)
		{
			if (type >= ID.WaterfallID.Count)
			{
				LiquidFallLoader.wFallFrameBack[type] = frame;
			}
			else if (type == ID.WaterfallID.Rain)
			{
				self.rainFrameBackground = frame;
			}
			else
			{
				ModContent.GetInstance<ModLiquidLib>().Logger.Warn("Attempting to get a waterfall back frame for a vanilla waterfall that does not contain a backframe frame.");
			}
		}

		public static void SetWFallFrameCounter(this WaterfallManager self, int type, int frame)
		{
			if (type >= ID.WaterfallID.Count)
			{
				LiquidFallLoader.wFallFrameCounter[type] = frame;
			}
			else if (type == ID.WaterfallID.Lava || type == ID.WaterfallID.Honey || type == ID.WaterfallID.Shimmer)
			{
				self.wFallFrCounter2 = frame;
			}
			else if (type == ID.WaterfallID.Rain)
			{
				self.rainFrameCounter = frame;
			}
			else if (type == ID.WaterfallID.Snow)
			{
				self.snowFrameCounter = frame;
			}
			else
			{
				self.wFallFrCounter = frame;
			}
		}
	}
}
