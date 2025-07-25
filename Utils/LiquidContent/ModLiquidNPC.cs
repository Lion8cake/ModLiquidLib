﻿using ModLiquidLib.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModLiquidLib.Utils.LiquidContent
{
	public class ModLiquidNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		//Modded liquids start at the index of 0 as vanilla liquids are recorded in their own seperate fields
		//use (id - LiquidID.Count) to get the correct modded liquid ID wet type
		public bool[] moddedWet = new bool[LiquidLoader.LiquidCount - LiquidID.Count];

		public override void AI(NPC npc)
		{
			if (npc.aiStyle == 7)
			{
				if (npc.ai[0] == 25f)
				{
					for (int i = LiquidID.Count; i < LiquidLoader.LiquidCount; i++)
					{
						moddedWet[i - LiquidID.Count] = false;
					}
				}
			}
		}
	}
}
