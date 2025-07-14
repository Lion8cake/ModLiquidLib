using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace ModLiquidLib.ModLoader
{
	public static class LiquidFallLoader
	{
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
