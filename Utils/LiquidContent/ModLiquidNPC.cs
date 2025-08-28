using ModLiquidLib.ModLoader;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModLiquidLib.Utils.LiquidContent
{
	public class ModLiquidNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		/// <summary>
		/// Modded liquids start at the index of 0 as vanilla liquids are recorded in their own seperate fields. <br/>
		/// Use (id - LiquidID.Count) to get the correct modded liquid ID wet type
		/// </summary>
		public bool[] moddedWet = new bool[LiquidLoader.LiquidCount - LiquidID.Count];

		/// <summary>
		/// Modded liquids start at the index of 0 as vanilla liquids are recorded in their own seperate fields. <br/>
		/// Use (id - LiquidID.Count) to get the correct modded liquid ID movement speed
		/// </summary>
		public float[] moddedLiquidMovementSpeed = new float[LiquidLoader.LiquidCount - LiquidID.Count];

		public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
		{
			return true;
		}

		public ModLiquidNPC()
		{
			moddedLiquidMovementSpeed = new float[LiquidLoader.LiquidCount - LiquidID.Count];
			for (int i = 0; i < moddedLiquidMovementSpeed.Length; i++)
			{
				moddedLiquidMovementSpeed[i] = 0.5f;
			}
		}

		public override GlobalNPC Clone(NPC from, NPC to)
		{
			ModLiquidNPC liquidNPCClone = (ModLiquidNPC)MemberwiseClone();
			liquidNPCClone.moddedWet = from.GetGlobalNPC<ModLiquidNPC>().moddedWet.ToArray();
			liquidNPCClone.moddedLiquidMovementSpeed = from.GetGlobalNPC<ModLiquidNPC>().moddedLiquidMovementSpeed.ToArray();
			return liquidNPCClone;
		}

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

		public override void SetDefaults(NPC entity)
		{
			for (int i = LiquidID.Count; i < LiquidLoader.LiquidCount; i++)
			{
				moddedLiquidMovementSpeed[i - LiquidID.Count] = LiquidLoader.GetModLiquid(i).NPCMovementMultiplierDefault;
			}
			if (entity.waterMovementSpeed == 1f && entity.lavaMovementSpeed == 1f && entity.honeyMovementSpeed == 1f && entity.shimmerMovementSpeed == 1f)
			{
				for (int i = LiquidID.Count; i < LiquidLoader.LiquidCount; i++)
				{
					moddedLiquidMovementSpeed[i - LiquidID.Count] = 1f;
				}
			}
		}
	}
}
