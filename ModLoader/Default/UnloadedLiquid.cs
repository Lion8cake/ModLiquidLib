using ModLiquidLib.IO;
using Terraria;

namespace ModLiquidLib.ModLoader.Default
{
	public class UnloadedLiquid : ModLiquid
	{
		public override string Texture => "ModLiquidLib/ModLoader/Default/UnloadedLiquid";

		public override void SetStaticDefaults()
		{
			LiquidFallLength = 1; //makes unloaded floating liquids look less odd
			LiquidIO.Liquids.unloadedTypes.Add(Type);
		}

		public override bool UpdateLiquid(int i, int j, Liquid liquid) => false; //prevents unloaded liquids from moving

		public override bool SettleLiquidMovement(int i, int j) => false; //prevents unloaded liquids from being moved when loading worlds

		public override bool PreLiquidMerge(int liquidX, int liquidY, int tileX, int tileY, int otherLiquid) => false; //prevents creating tiles

		public override bool OnPump(int inX, int inY, int outX, int outY) => false; //prevents pumping
	}
}