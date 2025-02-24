using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace ModLiquidLib.ModLoader
{
	public static class LiquidFallLoader
	{
		public static bool PreDraw(int currentWaterfallData, int i, int j, int type, SpriteBatch spriteBatch)
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
	}
}
