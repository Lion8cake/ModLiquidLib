using ModLiquidLib.ModLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace ModLiquidLib.Testing
{
	public class ExampleGlobalLiquid : GlobalLiquid
	{
		public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
		{
			/*if (type == LiquidID.Lava)
			{
				r = 0f;
				g = 0f;
				b = 0f;
			}
			if (type == ModLiquidLib.LiquidType<ExampleLiquid>())
			{
				r = 0f;
				g = 0f; 
				b = 0f;
			}*/
		}
	}
}
