using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModLiquidLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace ModLiquidLib.ModLoader
{
	public abstract class GlobalLiquid : ModType
	{
		protected sealed override void Register()
		{
			ModTypeLookup<GlobalLiquid>.Register(this);
			LiquidLoader.globalLiquids.Add(this);
		}

		public sealed override void SetupContent()
		{
			SetStaticDefaults();
		}

		/// <summary>
		/// Allows you to draw things behind the tile/wall at the given coordinates. Return false to stop the game from drawing the tile/wall normally. Returns true by default.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="j"></param>
		/// <param name="type"></param>
		/// <param name="spriteBatch"></param>
		/// <returns></returns>
		public virtual bool PreDraw(int i, int j, int type, LiquidDrawCache liquidDrawCache, Vector2 drawOffset, bool isBackgroundDraw)
		{
			return true;
		}

		/// <summary>
		/// Allows you to draw things in front of the tile/wall at the given coordinates. This can also be used to do things such as creating dust. Called on active tiles. See also ModSystem.PostDrawTiles.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="j"></param>
		/// <param name="type"></param>
		/// <param name="spriteBatch"></param>
		public virtual void PostDraw(int i, int j, int type, LiquidDrawCache liquidDrawCache, Vector2 drawOffset, bool isBackgroundDraw)
		{
		}

		/// <summary>
		/// Allows you to make stuff happen whenever the tile at the given coordinates is drawn. For example, creating dust or changing the color the tile is drawn in.
		/// SpecialDraw will only be called if coordinates are added using Main.instance.TilesRenderer.AddSpecialLegacyPoint here.
		/// Only works on Color and White lighting, see RetroDrawEffects for retro lighting of emitting particles
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type">The Tile type of the tile being drawn</param>
		/// <param name="spriteBatch">The SpriteBatch that should be used for all draw calls</param>
		/// <param name="drawData">Various information about the tile that is being drawn, such as color, framing, glow textures, etc.</param>
		public virtual bool EmitEffects(int i, int j, int type, LiquidCache liquidCache)
		{
			return true;
		}

		/// <summary>
		/// Allows you to draw things behind the tile/wall at the given coordinates. Return false to stop the game from drawing the tile/wall normally. Returns true by default.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="j"></param>
		/// <param name="type"></param>
		/// <param name="spriteBatch"></param>
		/// <returns></returns>
		public virtual bool PreRetroDraw(int i, int j, int type, SpriteBatch spriteBatch)
		{
			return true;
		}

		/// <summary>
		/// Allows you to draw things in front of the tile/wall at the given coordinates. This can also be used to do things such as creating dust. Called on active tiles. See also ModSystem.PostDrawTiles.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="j"></param>
		/// <param name="type"></param>
		/// <param name="spriteBatch"></param>
		public virtual void PostRetroDraw(int i, int j, int type, SpriteBatch spriteBatch)
		{
		}

		/// <summary>
		/// Allows you to make stuff happen whenever the tile at the given coordinates is drawn. For example, creating dust or changing the color the tile is drawn in.
		/// SpecialDraw will only be called if coordinates are added using Main.instance.TilesRenderer.AddSpecialLegacyPoint here.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type">The Tile type of the tile being drawn</param>
		/// <param name="spriteBatch">The SpriteBatch that should be used for all draw calls</param>
		/// <param name="drawData">Various information about the tile that is being drawn, such as color, framing, glow textures, etc.</param>
		public virtual void RetroDrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref RetroLiquidDrawInfo drawData, float liquidAmountModified, int liquidGFXQuality)
		{
		}

		/// <summary>
		/// Allows you to draw things behind the tile/wall at the given coordinates. Return false to stop the game from drawing the tile/wall normally. Returns true by default.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="j"></param>
		/// <param name="type"></param>
		/// <param name="spriteBatch"></param>
		/// <returns></returns>
		public virtual bool PreSlopeDraw(int i, int j, int type, bool behindBlocks, ref Vector2 drawPosition, ref Rectangle liquidSize, ref VertexColors colors)
		{
			return true;
		}

		/// <summary>
		/// Allows you to draw things in front of the tile/wall at the given coordinates. This can also be used to do things such as creating dust. Called on active tiles. See also ModSystem.PostDrawTiles.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="j"></param>
		/// <param name="type"></param>
		/// <param name="spriteBatch"></param>
		public virtual void PostSlopeDraw(int i, int j, int type, bool behindBlocks, ref Vector2 drawPosition, ref Rectangle liquidSize, ref VertexColors colors)
		{
		}

		/// <summary>
		/// Allows you to determine how much light the block emits.
		/// If it is a tile, make sure you set Main.tileLighted[Type] to true in SetDefaults for this to work.
		/// If it is a wall, it can also let you light up the block in front of this wall.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="j"></param>
		/// <param name="type"></param>
		/// <param name="r"></param>
		/// <param name="g"></param>
		/// <param name="b"></param>
		public virtual void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
		{
		}

		public virtual bool DisableRetroLavaBubbles(int i, int j)
		{
			return false;
		}

		/// <summary>
		/// The ID of the waterfall style the game should use when this water style is in use.
		/// return null to call the normal waterfall style
		/// </summary>
		public virtual int? ChooseWaterfallStyle(int i, int j, int type)
		{
			return null;
		}
	}
}
