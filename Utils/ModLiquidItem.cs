using ModLiquidLib.ModLoader;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModLiquidLib.Utils
{
	public class ModLiquidItem : GlobalItem
	{
		public override bool InstancePerEntity => true;

		//Modded liquids start at the index of 0 as vanilla liquids are recorded in their own seperate fields
		//use (id - LiquidID.Count) to get the correct modded liquid ID wet type
		public bool[] moddedWet = new bool[LiquidLoader.LiquidCount - LiquidID.Count];
	}
}
