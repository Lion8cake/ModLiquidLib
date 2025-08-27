using Microsoft.Xna.Framework.Graphics;
using ModLiquidLib.Utils.ManualHooks;
using System;
using Terraria;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace ModLiquidLib.ModLoader
{
	public static class LiquidFallLoader
	{
		internal delegate void DelegateAnimateWaterfall(ref int frame, ref int frameBack, ref int frameCounter);

		internal static int[] wFallFrame = new int[ID.WaterfallID.Count];

		internal static int[] wFallFrameBack = new int[ID.WaterfallID.Count];

		internal static int[] wFallFrameCounter = new int[ID.WaterfallID.Count];

		internal static void ResizeMoreFallArrays(On_WaterfallStylesLoader.orig_ResizeArrays orig, WaterFallStylesLoader self)
		{
			orig.Invoke(self);
			Array.Resize(ref wFallFrame, self.TotalCount);
			Array.Resize(ref wFallFrameBack, self.TotalCount);
			Array.Resize(ref wFallFrameCounter, self.TotalCount);
		}

		public static bool PreDraw(WaterfallManager.WaterfallData currentWaterfallData, int i, int j, int type, SpriteBatch spriteBatch)
		{
			bool flag = true;
			if (LoaderManager.Get<WaterFallStylesLoader>().Get(type) is ModLiquidFall)
			{
				ModLiquidFall waterStyle = (ModLiquidFall)LoaderManager.Get<WaterFallStylesLoader>().Get(type);
				if (waterStyle != null)
				{
					flag = waterStyle?.PreDraw(currentWaterfallData, i, j, spriteBatch) ?? true;
				}
			}
			return flag;
		}

		public static void PostDraw(WaterfallManager.WaterfallData currentWaterfallData, int i, int j, int type, SpriteBatch spriteBatch)
		{
			if (LoaderManager.Get<WaterFallStylesLoader>().Get(type) is ModLiquidFall)
			{
				ModLiquidFall waterStyle = (ModLiquidFall)LoaderManager.Get<WaterFallStylesLoader>().Get(type);
				if (waterStyle != null)
				{
					waterStyle?.PostDraw(currentWaterfallData, i, j, spriteBatch);
				}
			}
		}

		public static void AnimateWaterfall(int type)
		{
			if (LoaderManager.Get<WaterFallStylesLoader>().Get(type) is ModLiquidFall)
			{
				ModLiquidFall waterStyle = (ModLiquidFall)LoaderManager.Get<WaterFallStylesLoader>().Get(type);
				if (waterStyle != null)
				{
					waterStyle?.AnimateWaterfall(ref wFallFrame[waterStyle.Slot], ref wFallFrameBack[waterStyle.Slot], ref wFallFrameCounter[waterStyle.Slot]);
				}
			}
		}

		public static float? Alpha(int x, int y, int type, float Alpha, int maxSteps, int s, Tile tileCache)
		{
			float? num = null;
			if (LoaderManager.Get<WaterFallStylesLoader>().Get(type) is ModLiquidFall)
			{
				ModLiquidFall waterStyle = (ModLiquidFall)LoaderManager.Get<WaterFallStylesLoader>().Get(type);
				if (waterStyle != null)
				{
					num = waterStyle?.Alpha(x, y, Alpha, maxSteps, s, tileCache);
				}
			}
			return num;
		}
	}
}
