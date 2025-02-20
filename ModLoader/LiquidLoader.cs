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
using Terraria.Graphics;

namespace ModLiquidLib.ModLoader
{
	public static class LiquidLoader
	{
		private delegate void DelegateModifyLight(int i, int j, int type, ref float r, ref float g, ref float b);

		private delegate bool DelegatePreSlopeDraw(int i, int j, int type, bool behindBlocks, ref Vector2 drawPosition, ref Rectangle liquidSize, ref VertexColors colors);

		private delegate void DelegatePostSlopeDraw(int i, int j, int type, bool behindBlocks, ref Vector2 drawPosition, ref Rectangle liquidSize, ref VertexColors colors);

		private delegate void DelegateRetroDrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref RetroLiquidDrawInfo drawData, float liquidAmountModified, int liquidGFXQuality);

		private static int nextLiquid = LiquidID.Count;

		internal static readonly IList<ModLiquid> liquids = new List<ModLiquid>();

		internal static readonly IList<GlobalLiquid> globalLiquids = new List<GlobalLiquid>();

		private static bool loaded = false;

		private static DelegateModifyLight[] HookModifyLight;

		private static Func<int, int, int, LiquidDrawCache, Vector2, bool, bool>[] HookPreDraw;

		private static Action<int, int, int, LiquidDrawCache, Vector2, bool>[] HookPostDraw;

		private static Func<int, int, int, SpriteBatch, bool>[] HookPreRetroDraw;

		private static DelegateRetroDrawEffects[] HookRetroDrawEffects;

		private static Action<int, int, int, SpriteBatch>[] HookPostRetroDraw;

		private static DelegatePreSlopeDraw[] HookPreSlopeDraw;

		private static DelegatePostSlopeDraw[] HookPostSlopeDraw;

		public static int LiquidCount => nextLiquid;

		public static Asset<Texture2D>[] LiquidAssets = new Asset<Texture2D>[4];

		public static Asset<Texture2D>[] LiquidBlockAssets = new Asset<Texture2D>[4];

		public static Asset<Texture2D>[] LiquidSlopeAssets = new Asset<Texture2D>[4];

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
			Array.Resize(ref LiquidBlockAssets, LiquidCount);
			Array.Resize(ref LiquidSlopeAssets, LiquidCount);
			Array.Resize(ref Unsafe.AsRef(in LiquidRenderer.WATERFALL_LENGTH), LiquidCount);
			Array.Resize(ref Unsafe.AsRef(in LiquidRenderer.DEFAULT_OPACITY), LiquidCount);
			Array.Resize(ref Unsafe.AsRef(in LiquidRenderer.WAVE_MASK_STRENGTH), LiquidCount);
			Array.Resize(ref Unsafe.AsRef(in LiquidRenderer.VISCOSITY_MASK), LiquidCount);
			LoaderUtils.ResetStaticMembers(typeof(LiquidID));
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, DelegateModifyLight>(ref HookModifyLight, globalLiquids, (GlobalLiquid g) => g.ModifyLight);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<int, int, int, LiquidDrawCache, Vector2, bool, bool>>(ref HookPreDraw, globalLiquids, (GlobalLiquid g) => g.PreDraw);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Action<int, int, int, LiquidDrawCache, Vector2, bool>>(ref HookPostDraw, globalLiquids, (GlobalLiquid g) => g.PostDraw);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<int, int, int, SpriteBatch, bool>>(ref HookPreRetroDraw, globalLiquids, (GlobalLiquid g) => g.PreRetroDraw);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, DelegateRetroDrawEffects>(ref HookRetroDrawEffects, globalLiquids, (GlobalLiquid g) => g.RetroDrawEffects);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Action<int, int, int, SpriteBatch>>(ref HookPostRetroDraw, globalLiquids, (GlobalLiquid g) => g.PostRetroDraw);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, DelegatePreSlopeDraw>(ref HookPreSlopeDraw, globalLiquids, (GlobalLiquid g) => g.PreSlopeDraw);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, DelegatePostSlopeDraw>(ref HookPostSlopeDraw, globalLiquids, (GlobalLiquid g) => g.PostSlopeDraw);
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

		public static bool PreRetroDraw(int i, int j, int type, SpriteBatch spriteBatch)
		{
			Func<int, int, int, SpriteBatch, bool>[] hookPreRetroDraw = HookPreRetroDraw;
			for (int k = 0; k < hookPreRetroDraw.Length; k++)
			{
				if (!hookPreRetroDraw[k](i, j, type, spriteBatch))
				{
					return false;
				}
			}
			return GetLiquid(type)?.PreRetroDraw(i, j, spriteBatch) ?? true;
		}

		public static void RetroDrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref RetroLiquidDrawInfo drawData, float liquidAmountModified, int liquidGFXQuality)
		{
			GetLiquid(type)?.RetroDrawEffects(i, j, spriteBatch, ref drawData, liquidAmountModified, liquidGFXQuality);
			DelegateRetroDrawEffects[] hookRetroDrawEffects = HookRetroDrawEffects;
			for (int k = 0; k < hookRetroDrawEffects.Length; k++)
			{
				hookRetroDrawEffects[k](i, j, type, spriteBatch, ref drawData, liquidAmountModified, liquidGFXQuality);
			}
		}

		public static void PostRetroDraw(int i, int j, int type, SpriteBatch spriteBatch)
		{
			GetLiquid(type)?.PostRetroDraw(i, j, spriteBatch);
			Action<int, int, int, SpriteBatch>[] hookPostRetroDraw = HookPostRetroDraw;
			for (int k = 0; k < hookPostRetroDraw.Length; k++)
			{
				hookPostRetroDraw[k](i, j, type, spriteBatch);
			}
		}

		public static bool PreSlopeDraw(int i, int j, int type, bool behindBlocks, ref Vector2 drawPosition, ref Rectangle liquidSize, ref VertexColors colors)
		{
			DelegatePreSlopeDraw[] hookPreSlopeDraw = HookPreSlopeDraw;
			for (int k = 0; k < hookPreSlopeDraw.Length; k++)
			{
				if (!hookPreSlopeDraw[k](i, j, type, behindBlocks, ref drawPosition, ref liquidSize, ref colors))
				{
					return false;
				}
			}
			return GetLiquid(type)?.PreSlopeDraw(i, j, behindBlocks, ref drawPosition, ref liquidSize, ref colors) ?? true;
		}

		public static void PostSlopeDraw(int i, int j, int type, bool behindBlocks, ref Vector2 drawPosition, ref Rectangle liquidSize, ref VertexColors colors)
		{
			GetLiquid(type)?.PostSlopeDraw(i, j, behindBlocks, ref drawPosition, ref liquidSize, ref colors);
			DelegatePostSlopeDraw[] hookPostSlopeDraw = HookPostSlopeDraw;
			for (int k = 0; k < hookPostSlopeDraw.Length; k++)
			{
				hookPostSlopeDraw[k](i, j, type, behindBlocks, ref drawPosition, ref liquidSize, ref colors);
			}
		}
	}
}
