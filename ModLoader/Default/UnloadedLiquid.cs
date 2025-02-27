using ModLiquidLib.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace ModLiquidLib.ModLoader.Default
{
	public class UnloadedLiquid : ModLiquid
	{
		public override string Texture => "ModLiquidLib/ModLoader/Default/UnloadedLiquid";

		public override void SetStaticDefaults()
		{
			LiquidIO.Liquids.unloadedTypes.Add(Type);
		}
	}
}
