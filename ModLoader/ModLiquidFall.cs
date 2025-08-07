using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using static Terraria.WaterfallManager;

namespace ModLiquidLib.ModLoader
{
	public abstract class ModLiquidFall : ModWaterfallStyle
	{
		public int WFallFrCounter 
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

		public int RegularFrame 
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

		public int WFallFrCounter2 
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

		public int SlowFrame 
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

		public int RainFrameCounter 
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

		public int RainFrameForeground 
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

		public int RainFrameBackground 
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

		public int SnowFrameCounter 
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

		public int SnowFrameForeground 
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

		public int FindWaterfallCount 
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

		public int WaterfallDist 
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

		public int QualityMax 
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

		public int CurrentMax 
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

		public WaterfallData[] Waterfalls
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
		/// <param name="currentWaterfallData">The current waterfall data.</param>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The Y position in tile coordinates.</param>
		/// <param name="spriteBatch"></param>
		/// <returns></returns>
		public virtual bool PreDraw(WaterfallData currentWaterfallData, int i, int j, SpriteBatch spriteBatch)
		{
			return true;
		}

		/// <summary>
		/// Allows you to draw things in front of the waterfall at the given coordinates. This can also be used to do things such as rendering glowmasks.<para />
		/// </summary>
		/// <param name="currentWaterfallData">The current waterfall data, this is used inside of the waterfalls WaterfallData array.</param>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The Y position in tile coordinates.</param>
		/// <param name="spriteBatch"></param>
		/// <returns></returns>
		public virtual void PostDraw(WaterfallData currentWaterfallData, int i, int j, SpriteBatch spriteBatch)
		{
		}

		/// <summary>
		/// Edits the opacity of the waterfall. For example: Waterfalls have an opacity of 60% (0.6f) which allows you to see some stuff behind them, while Lavafalls have an opacity of 100% (1f) which prevents you from seeing anything behind. <br />
		/// Returns null be default.
		/// </summary>
		/// <param name="x">The x position in tile coordinates.</param>
		/// <param name="y">The Y position in tile coordinates.</param>
		/// <param name="Alpha">The current waterfall water style alpha</param>
		/// <param name="maxSteps">The maximum length of the waterfall</param>
		/// <param name="s"></param>
		/// <param name="tileCache">Tile at the waterfall position</param>
		/// <returns></returns>
		public virtual float? Alpha(int x, int y, float Alpha, int maxSteps, int s, Tile tileCache)
		{
			return null;
		}

		/// <summary>
		/// Allows you to prevent the waterfall/liquidfall from making any water sounds when on screen. This is useful for waterfalls/liquidfalls that arent made of water. Returns true by default. 
		/// </summary>
		/// <returns></returns>
		public virtual bool PlayWaterfallSounds()
		{
			return true;
		}
	}
}
