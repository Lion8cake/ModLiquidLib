using Microsoft.Xna.Framework.Graphics;
using ModLiquidLib.Utils;
using Terraria;
using Terraria.ModLoader;
using static Terraria.WaterfallManager;

namespace ModLiquidLib.ModLoader
{
	public abstract class ModLiquidFall : ModWaterfallStyle
	{
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
		/// The current frame of the waterfall
		/// </summary>
		public int WaterfallFrame
		{
			get
			{
				return Main.instance.waterfallManager.GetWFallFrame(Slot);
			}
			set
			{
				Main.instance.waterfallManager.SetWFallFrame(Slot, value);
			}
		}

		/// <summary>
		/// The background frame of the waterfall (used for custom rain waterfalls)
		/// </summary>
		public int WaterfallBackgroundFrame
		{
			get
			{
				return Main.instance.waterfallManager.GetWFallFrameBack(Slot);
			}
			set
			{
				Main.instance.waterfallManager.SetWFallFrameBack(Slot, value);
			}
		}

		/// <summary>
		/// The frame counter of the waterfall
		/// </summary>
		public int WaterfallCounterFrame
		{
			get
			{
				return Main.instance.waterfallManager.GetWFallFrameCounter(Slot);
			}
			set
			{
				Main.instance.waterfallManager.SetWFallFrameCounter(Slot, value);
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
		/// Allows you to animate your waterfall. <br/>
		/// Overriding this method will make your waterfall nolonger animate normally.<br/><br/>
		/// Use frame to specify which frame the waterfall is using currently. <br/><br/>
		/// Use frameBackground to specify which background frame the waterfall is using. (This normally goes unused, but is very useful for modders looking into drawing their own waterfalls manually). <br/>
		/// Rain clouds use this to specify the framing of the rain behind the main rain waterfall.<br/><br/>
		/// Use frameCounter to specify the duration between frames.<br/><br/>
		/// Please see ModLiquidExampleMod.Content.Waterfalls.BloodClotLiquidFall.AnimateWaterfall or ModLiquidExampleMod.Content.Waterfalls.HoneyRain.AnimateWaterfall.
		/// </summary>
		/// <param name="frame">Waterfalls use this to know what frame to use when drawing.</param>
		/// <param name="frameBackground">Unused normally, can be used by modders for extra framing.</param>
		/// <param name="frameCounter">Used to specify a certain amount of time between waterfall frames.</param>
		public virtual void AnimateWaterfall(ref int frame, ref int frameBackground, ref int frameCounter)
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
