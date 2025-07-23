using ModLiquidLib.ModLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModLiquidLib.Utils.LiquidContent
{
	public class ModLiquidPlayer : ModPlayer
	{
		private bool[] _adjLiquid = new bool[LiquidLoader.LiquidCount];

		private bool[] _oldAdjLiquid = new bool[LiquidLoader.LiquidCount];

		//Modded liquids start at the index of 0 as vanilla liquids are recorded in their own seperate fields
		//use (id - LiquidID.Count) to get the correct modded liquid ID wet type
		public bool[] moddedWet = new bool[LiquidLoader.LiquidCount - LiquidID.Count];

		public bool[] adjLiquid
		{
			get
			{
				if (_adjLiquid.Length != LiquidLoader.LiquidCount)
				{
					Array.Resize(ref _adjLiquid, LiquidLoader.LiquidCount);
				}
				return _adjLiquid;
			}
			set
			{
				_adjLiquid = value;
			}
		}

		public bool[] oldAdjLiquid
		{
			get
			{
				if (_oldAdjLiquid.Length != LiquidLoader.LiquidCount)
				{
					Array.Resize(ref _oldAdjLiquid, LiquidLoader.LiquidCount);
				}
				return _oldAdjLiquid;
			}
			set
			{
				_oldAdjLiquid = value;
			}
		}

		public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
		{
			for (int n = 0; n < adjLiquid.Length; n++)
			{
				adjLiquid[n] = false;
				oldAdjLiquid[n] = false;
			}
			base.ModifyMaxStats(out health, out mana);
		}
	}
}
