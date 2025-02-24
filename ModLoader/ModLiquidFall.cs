using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace ModLiquidLib.ModLoader
{
	public abstract class ModLiquidFall : ModWaterfallStyle
	{
		/// <summary>
		/// Allows you to draw things behind the waterfall at the given coordinates. Return false to stop the game from drawing the waterfall normally. Returns true by default.
		/// </summary>
		/// <param name="currentWaterfallData">The current waterfall data, this is used inside of the waterfalls WaterfallData array.</param>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The Y position in tile coordinates.</param>
		/// <param name="spriteBatch"></param>
		/// <returns></returns>
		public virtual bool PreDraw(int currentWaterfallData, int i, int j, SpriteBatch spriteBatch)
		{
			return true;
		}

		public virtual void PostDraw(int currentWaterfallData, int i, int j, SpriteBatch spriteBatch)
		{
		}

		/// <summary>
		/// Sets the alpha of the waterfall
		/// </summary>
		/// <returns></returns>
		public virtual float? Alpha(float Alpha, int maxSteps, int y, int s, Tile tileCache)
		{
			return null;
		}

		/// <summary>
		/// Used to incriment fields that result in the waterfall animation, usefull for making stuff such as your own weather cloud tile
		/// </summary>
		public virtual void AnimateWaterfall()
		{

		}
	}
}
