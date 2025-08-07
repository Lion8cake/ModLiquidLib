using static Terraria.WaterfallManager;

namespace ModLiquidLib.ModLoader
{
	public interface ILiquidModTile
	{
		/// <summary>
		/// Creates a waterfall at the tile position
		/// </summary>
		/// <returns></returns>
		public WaterfallData? CreateWaterfall(int i, int j);
	}
}
