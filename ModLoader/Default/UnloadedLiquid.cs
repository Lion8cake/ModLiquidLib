using ModLiquidLib.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

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
	}
}
