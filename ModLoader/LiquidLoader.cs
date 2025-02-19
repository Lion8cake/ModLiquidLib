using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using ModLiquidLib.Utils;
using Terraria.GameContent.Liquid;
using System.Runtime.CompilerServices;
using Terraria.DataStructures;
using ReLogic.Content;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace ModLiquidLib.ModLoader
{
	public static class LiquidLoader
	{
		private delegate void DelegateModifyLight(int i, int j, int type, ref float r, ref float g, ref float b);

		private static int nextLiquid = LiquidID.Count;

		internal static readonly IList<ModLiquid> liquids = new List<ModLiquid>();

		internal static readonly IList<GlobalLiquid> globalLiquids = new List<GlobalLiquid>();

		private static bool loaded = false;

		private static DelegateModifyLight[] HookModifyLight;

		private static Func<int, int, int, LiquidDrawCache, Vector2, bool, bool>[] HookPreDraw;

		private static Action<int, int, int, LiquidDrawCache, Vector2, bool>[] HookPostDraw;

		public static int LiquidCount => nextLiquid;

		public static Asset<Texture2D>[] LiquidAssets = new Asset<Texture2D>[4];

		internal static int ReserveLiquidID()
		{
			int result = nextLiquid;
			nextLiquid++;
			return result;
		}

		/// <summary>
		/// Gets the ModLiquid instance with the given type. If no ModLiquid with the given type exists, returns null.
		/// </summary>
		public static ModLiquid GetLiquid(int type)
		{
			if (type < LiquidID.Count || type >= LiquidCount)
			{
				return null;
			}
			return liquids[type - LiquidID.Count];
		}

		internal static void ResizeArrays(bool unloading = false)
		{
			Array.Resize(ref LiquidAssets, LiquidCount);
			Array.Resize(ref Unsafe.AsRef(in LiquidRenderer.WATERFALL_LENGTH), LiquidCount);
			Array.Resize(ref Unsafe.AsRef(in LiquidRenderer.DEFAULT_OPACITY), LiquidCount);
			Array.Resize(ref Unsafe.AsRef(in LiquidRenderer.WAVE_MASK_STRENGTH), LiquidCount);
			Array.Resize(ref Unsafe.AsRef(in LiquidRenderer.VISCOSITY_MASK), LiquidCount);
			LoaderUtils.ResetStaticMembers(typeof(LiquidID));
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, DelegateModifyLight>(ref HookModifyLight, globalLiquids, (GlobalLiquid g) => g.ModifyLight);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<int, int, int, LiquidDrawCache, Vector2, bool, bool>>(ref HookPreDraw, globalLiquids, (GlobalLiquid g) => g.PreDraw);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Action<int, int, int, LiquidDrawCache, Vector2, bool>>(ref HookPostDraw, globalLiquids, (GlobalLiquid g) => g.PostDraw);
			if (!unloading)
			{
				loaded = true;
			}
		}

		internal static void Unload()
		{
			loaded = false;
			liquids.Clear();
			nextLiquid = LiquidID.Count;
			globalLiquids.Clear();
			Array.Resize(ref Unsafe.AsRef(in LiquidRenderer.WATERFALL_LENGTH), 4);
			Array.Resize(ref Unsafe.AsRef(in LiquidRenderer.DEFAULT_OPACITY), 4);
			Array.Resize(ref Unsafe.AsRef(in LiquidRenderer.WAVE_MASK_STRENGTH), 5);
			Array.Resize(ref Unsafe.AsRef(in LiquidRenderer.VISCOSITY_MASK), 5);
		}

		public static void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
		{
			GetLiquid(type)?.ModifyLight(i, j, ref r, ref g, ref b);
			DelegateModifyLight[] hookModifyLight = HookModifyLight;
			for (int k = 0; k < hookModifyLight.Length; k++)
			{
				hookModifyLight[k](i, j, type, ref r, ref g, ref b);
			}
		}

		public static bool PreDraw(int i, int j, int type, LiquidDrawCache liquidDrawCache, Vector2 drawOffset, bool isBackgroundDraw)
		{
			Func<int, int, int, LiquidDrawCache, Vector2, bool, bool>[] hookPreDraw = HookPreDraw;
			for (int k = 0; k < hookPreDraw.Length; k++)
			{
				if (!hookPreDraw[k](i, j, type, liquidDrawCache, drawOffset, isBackgroundDraw))
				{
					return false;
				}
			}
			return GetLiquid(type)?.PreDraw(i, j, liquidDrawCache, drawOffset, isBackgroundDraw) ?? true;
		}

		public static void PostDraw(int i, int j, int type, LiquidDrawCache liquidDrawCache, Vector2 drawOffset, bool isBackgroundDraw)
		{
			GetLiquid(type)?.PostDraw(i, j, liquidDrawCache, drawOffset, isBackgroundDraw);
			Action<int, int, int, LiquidDrawCache, Vector2, bool>[] hookPostDraw = HookPostDraw;
			for (int k = 0; k < hookPostDraw.Length; k++)
			{
				hookPostDraw[k](i, j, type, liquidDrawCache, drawOffset, isBackgroundDraw);
			}
		}
	}
}
