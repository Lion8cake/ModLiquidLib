using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModLiquidLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModLiquidLib.ModLoader
{
	public abstract class GlobalLiquid : ModType
	{
		public sealed override void Register()
		{
			ModTypeLookup<GlobalLiquid>.Register(this);
			LiquidLoader.globalLiquids.Add(this);
		}

		public sealed override void SetupContent()
		{
			SetStaticDefaults();
		}

		/// <summary>
		/// Allows you to draw things behind the liquid at the given coordinates. Return false to stop the game from drawing the liquid normally. Returns true by default.<para />
		/// This method is only called in "Color" and "White" lighting modes. Use <see cref="P:ModLiquidLib.ModLoader.GlobalLiquid.PreRetroDraw" /> for the other Lighting modes pre drawing
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type"></param>
		/// <param name="liquidDrawCache">The LiquidDrawCache of the rendering</param>
		/// <param name="drawOffset">The amount the liquid rendered is offset by</param>
		/// <param name="isBackgroundDraw">Whether or not the liquid is in the background</param>
		public virtual bool PreDraw(int i, int j, int type, LiquidDrawCache liquidDrawCache, Vector2 drawOffset, bool isBackgroundDraw)
		{
			return true;
		}

		/// <summary>
		/// Allows you to draw things in front of the liquid at the given coordinates. This can also be used to do things such as rendering glowmasks.<para />
		/// This hook is not called if <see cref="P:ModLiquidLib.ModLoader.ModLiquid.PreDraw" /> returns false.<para />
		/// This method is only called in "Color" and "White" lighting modes. Use <see cref="P:ModLiquidLib.ModLoader.GlobalLiquid.PostRetroDraw" /> for the other Lighting modes post drawing
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type"></param>
		/// <param name="liquidDrawCache">The LiquidDrawCache of the rendering</param>
		/// <param name="drawOffset">The amount the liquid rendered is offset by</param>
		/// <param name="isBackgroundDraw">Whether or not the liquid is in the background</param>
		public virtual void PostDraw(int i, int j, int type, LiquidDrawCache liquidDrawCache, Vector2 drawOffset, bool isBackgroundDraw)
		{
		}

		/// <summary>
		/// Allows you to make stuff happen whenever the liquid at the given coordinates is drawn. For example, creating bubble dusts or changing the color the liquid is drawn in. <para />
		/// This hook is seperate from Liquid Rendering, Only use this for things such as spawning particles. <para />
		/// Return false to stop Lava from spawning some of its bubbles. Returns true by default.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type"></param>
		/// <param name="liquidCache">Various information about the liquid that is being drawn, such as opacity, visual liquid, etc.</param>
		public virtual bool EmitEffects(int i, int j, int type, LiquidCache liquidCache)
		{
			return true;
		}

		/// <summary>
		/// Allows you to draw things behind the liquid at the given coordinates. Return false to stop the game from drawing the liquid normally. Returns true by default.<para />
		/// This method is only called in "Retro" and "Trippy" lighting modes. Use <see cref="P:ModLiquidLib.ModLoader.GlobalLiquid.PreDraw" /> for the other Lighting modes pre drawing
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type"></param>
		/// <param name="spriteBatch"></param>
		public virtual bool PreRetroDraw(int i, int j, int type, SpriteBatch spriteBatch)
		{
			return true;
		}

		/// <summary>
		/// Allows you to draw things in front of the liquid at the given coordinates. This can also be used to do things such as rendering glowmasks.<para />
		/// This hook is not called if <see cref="P:ModLiquidLib.ModLoader.GlobalLiquid.PreRetroDraw" /> returns false.<para />
		/// This method is only called in "Retro" and "Trippy" lighting modes. Use <see cref="P:ModLiquidLib.ModLoader.GlobalLiquid.PostDraw" /> for the other Lighting modes post drawing
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type"></param>
		/// <param name="spriteBatch"></param>
		public virtual void PostRetroDraw(int i, int j, int type, SpriteBatch spriteBatch)
		{
		}

		/// <summary>
		/// Allows you to make stuff happen whenever the liquid at the given coordinates is drawn. For example, creating bubble dusts or changing the color the liquid is drawn in. <para />
		/// This hook is not called if <see cref="P:ModLiquidLib.ModLoader.GlobalLiquid.PreRetroDraw" /> returns false.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type"></param>
		/// <param name="spriteBatch"></param>
		/// <param name="drawData">Various information about the liquid that is being drawn, such as color, opacity, waterfall length, etc.</param>
		/// <param name="liquidAmountModified">Gets the amount of liquid in a tile for retro rendering</param>
		/// <param name="liquidGFXQuality">Gets the number of quality the game has for retro rendering</param>
		public virtual void RetroDrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref RetroLiquidDrawInfo drawData, float liquidAmountModified, int liquidGFXQuality)
		{
		}

		/// <summary>
		/// Allows you to draw things behind the liquid at the given coordinates. Return false to stop the game from drawing the liquid normally. Returns true by default. <para />
		/// Only called for liquid slopes. If you want to predraw the liquid itself, see <see cref="P:ModLiquidLib.ModLoader.GlobalLiquid.PreDraw" /> or <see cref="P:ModLiquidLib.ModLoader.GlobalLiquid.PreRetroDraw" />
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type"></param>
		/// <param name="behindBlocks">Whether or not the slope is rendered behind tiles</param>
		/// <param name="drawPosition">The rendering position of the slope. Use this rather than x/y as the rendering can be offset sometimes</param>
		/// <param name="liquidSize">The size of the liquid</param>
		/// <param name="colors">The color of the liquid. Usually the light color of the liquid</param>
		public virtual bool PreSlopeDraw(int i, int j, int type, bool behindBlocks, ref Vector2 drawPosition, ref Rectangle liquidSize, ref VertexColors colors)
		{
			return true;
		}

		/// <summary>
		/// Allows you to draw things in front of the liquid at the given coordinates. This can also be used to do things such as rendering glowmasks.<para />
		/// This hook is not called if <see cref="P:ModLiquidLib.ModLoader.GlobalLiquid.PreSlopeDraw" /> returns false.<para />
		/// Only called for liquid slopes. If you want to postdraw the liquid itself, see <see cref="P:ModLiquidLib.ModLoader.GlobalLiquid.POstDraw" /> or <see cref="P:ModLiquidLib.ModLoader.GlobalLiquid.PostRetroDraw" />
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type"></param>
		/// <param name="behindBlocks">Whether or not the slope is rendered behind tiles</param>
		/// <param name="drawPosition">The rendering position of the slope. Use this rather than x/y as the rendering can be offset sometimes</param>
		/// <param name="liquidSize">The size of the liquid</param>
		/// <param name="colors">The color of the liquid. Usually the light color of the liquid</param>
		public virtual void PostSlopeDraw(int i, int j, int type, bool behindBlocks, ref Vector2 drawPosition, ref Rectangle liquidSize, ref VertexColors colors)
		{
		}

		/// <summary>
		/// Allows you to determine how much light this liquid emits.<br />
		/// It can also let you light up the block in front of this liquid.<br />
		/// See <see cref="M:Terraria.Graphics.Light.TileLightScanner.ApplyTileLight(Terraria.Tile,System.Int32,System.Int32,Terraria.Utilities.FastRandom@,Microsoft.Xna.Framework.Vector3@)" /> for vanilla tile light values to use as a reference.<br />
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type"></param>
		/// <param name="r">The red component of light, usually a value between 0 and 1</param>
		/// <param name="g">The green component of light, usually a value between 0 and 1</param>
		/// <param name="b">The blue component of light, usually a value between 0 and 1</param>
		public virtual void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
		{
		}

		/// <summary>
		/// Allows for disabling lava bubbles in "Retro" and "Trippy" lighting modes. Only calls when lava's PreRetroDraw isnt false. Returns false by default.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <returns></returns>
		public virtual bool DisableRetroLavaBubbles(int i, int j)
		{
			return false;
		}

		/// <summary>
		/// The ID of the waterfall/liquidfall style the game should use when the liquid is near a half block.
		/// Returns null by default.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type"></param>
		public virtual int? ChooseWaterfallStyle(int i, int j, int type)
		{
			return null;
		}

		/// <summary>
		/// Allows the changing of liquid movement in-game as well as allowing you to do other stuff with liquids during updates such as custom evaporation. Returns true by default. 
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type"></param>
		/// <param name="liquid"></param>
		/// <returns></returns>
		public virtual bool UpdateLiquid(int i, int j, int type, Liquid liquid)
		{
			return true;
		}

		/// <summary>
		/// Allows you to make liquids evaporate in the underworld. Also allows for disabling of water evaporation. Returns null by default.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type"></param>
		/// <returns></returns>
		public virtual bool? EvaporatesInHell(int i, int j, int type)
		{
			return null;
		}

		/// <summary>
		/// Allows you to change how liquids move when loading or creating a world. Returns true by default.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type"></param>
		/// <returns></returns>
		public virtual bool SettleLiquidMovement(int i, int j, int type)
		{
			return true;
		}
	}
}
