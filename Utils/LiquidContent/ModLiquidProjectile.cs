using ModLiquidLib.ModLoader;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModLiquidLib.Utils.LiquidContent
{
	public class ModLiquidProjectile : GlobalProjectile
	{
		public override bool InstancePerEntity => true;

		//Modded liquids start at the index of 0 as vanilla liquids are recorded in their own seperate fields
		//use (id - LiquidID.Count) to get the correct modded liquid ID wet type
		public bool[] moddedWet = new bool[LiquidLoader.LiquidCount - LiquidID.Count];

		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
		{
			return true;
		}

		public override GlobalProjectile Clone(Projectile from, Projectile to)
		{
			ModLiquidProjectile liquidProjClone = (ModLiquidProjectile)MemberwiseClone();
			liquidProjClone.moddedWet = from.GetGlobalProjectile<ModLiquidProjectile>().moddedWet.ToArray();
			return liquidProjClone;
		}
	}
}
