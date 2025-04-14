
using ModLiquidLib.ModLoader;
using Terraria.ID;

namespace ModLiquidLib.ID {
	public class TileLiquidIDSets
    {
		public class Sets 
		{
			/// <summary> Whether or not this tile counts as a this liquid's source for crafting purposes. </summary>
			public static bool[][] CountsAsLiquidSource = TileID.Sets.Factory.CreateCustomSet(new bool[LiquidLoader.LiquidCount]);
		}
    }
}
