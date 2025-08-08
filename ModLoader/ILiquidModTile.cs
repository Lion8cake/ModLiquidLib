using Microsoft.Xna.Framework.Graphics;
using static Terraria.WaterfallManager;

namespace ModLiquidLib.ModLoader
{
	/// <summary>
	/// Contains extra hooks for ModTile that are useful for liquid implementatiom.
	/// </summary>
	public interface ILiquidModTile
	{
		/// <summary>
		/// Creates a waterfall at the tile position. <br/>
		/// Useful when creating a tile similar to rain or snow cloud blocks.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <returns></returns>
		public WaterfallData? CreateWaterfall(int i, int j);

		/// <summary>
		/// Allows the rendering of this tile in liquids. <br/>
		/// Lily pads use this hook to draw themselves to liquids so water ripples/waves effect those tiles.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="spriteBatch"></param>
		public void DrawTileInWater(int i, int j, SpriteBatch spriteBatch);
	}
}