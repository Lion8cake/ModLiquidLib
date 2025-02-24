using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.Social.Base;
using static Terraria.WaterfallManager;

namespace ModLiquidLib.ModLoader
{
	public abstract class ModLiquidFall : ModWaterfallStyle
	{
		public int wFallFrCounter 
		{
			get
			{
				return Main.instance.waterfallManager.wFallFrCounter;
			}
			set
			{
				Main.instance.waterfallManager.wFallFrCounter = value;
			}
		}

		public int regularFrame 
		{
			get
			{
				return Main.instance.waterfallManager.regularFrame;
			}
			set
			{
				Main.instance.waterfallManager.regularFrame = value;
			}
		}

		public int wFallFrCounter2 
		{
			get
			{
				return Main.instance.waterfallManager.wFallFrCounter2;
			}
			set
			{
				Main.instance.waterfallManager.wFallFrCounter2 = value;
			}
		}

		public int slowFrame 
		{
			get
			{
				return Main.instance.waterfallManager.slowFrame;
			}
			set
			{
				Main.instance.waterfallManager.slowFrame = value;
			}
		}

		public int rainFrameCounter 
		{
			get
			{
				return Main.instance.waterfallManager.rainFrameCounter;
			}
			set
			{
				Main.instance.waterfallManager.rainFrameCounter = value;
			}
		}

		public int rainFrameForeground 
		{
			get
			{
				return Main.instance.waterfallManager.rainFrameForeground;
			}
			set
			{
				Main.instance.waterfallManager.rainFrameForeground = value;
			}
		}

		public int rainFrameBackground 
		{
			get
			{
				return Main.instance.waterfallManager.rainFrameBackground;
			}
			set
			{
				Main.instance.waterfallManager.rainFrameBackground = value;
			}
		}

		public int snowFrameCounter 
		{
			get
			{
				return Main.instance.waterfallManager.snowFrameCounter;
			}
			set
			{
				Main.instance.waterfallManager.snowFrameCounter = value;
			}
		}

		public int snowFrameForeground 
		{
			get
			{
				return Main.instance.waterfallManager.snowFrameForeground;
			}
			set
			{
				Main.instance.waterfallManager.snowFrameForeground = value;
			}
		}

		public int findWaterfallCount 
		{
			get
			{
				return Main.instance.waterfallManager.findWaterfallCount;
			}
			set
			{
				Main.instance.waterfallManager.findWaterfallCount = value;
			}
		}

		public int waterfallDist 
		{
			get
			{
				return Main.instance.waterfallManager.waterfallDist;
			}
			set
			{
				Main.instance.waterfallManager.waterfallDist = value;
			}
		}

		public int qualityMax 
		{
			get
			{
				return Main.instance.waterfallManager.qualityMax;
			}
			set
			{
				Main.instance.waterfallManager.qualityMax = value;
			}
		}

		public int currentMax 
		{
			get
			{
				return Main.instance.waterfallManager.currentMax;
			}
			set
			{
				Main.instance.waterfallManager.currentMax = value;
			}
		}

		public WaterfallData[] waterfalls
		{
			get
			{
				return Main.instance.waterfallManager.waterfalls;
			}
			set
			{
				Main.instance.waterfallManager.waterfalls = value;
			}
		}

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
		public virtual float? Alpha(int x, int y, float Alpha, int maxSteps, int s, Tile tileCache)
		{
			return null;
		}
	}
}
