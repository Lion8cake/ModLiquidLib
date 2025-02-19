using Microsoft.Xna.Framework;
using ModLiquidLib.ModLoader;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;

namespace ModLiquidLib.Hooks
{
	internal class MapHelperHooks
	{
		internal static void AddLiquidMapEntrys(ILContext il)
		{
			ILCursor c = new(il);
			c.GotoNext(MoveType.After, i => i.MatchLdsfld("Terraria.Map.MapHelper", "liquidPosition"), i => i.MatchLdloc(10), i => i.MatchAdd(), i => i.MatchStloc(3));
			c.EmitLdloca(3);
			c.EmitLdloc(10);
			c.EmitDelegate((ref int num5, int num7) =>
			{
				num5 = MapLiquidLoader.liquidLookup[num7];
			});
			c.GotoNext(MoveType.After, i => i.MatchCall("Terraria.ModLoader.MapLoader", "ModMapOption"));
			c.EmitLdloca(5);
			c.EmitLdarg(0);
			c.EmitLdarg(1);
			c.EmitDelegate((ref ushort mapType, int i, int j) =>
			{
				MapLiquidLoader.ModMapOption(ref mapType, i, j);
			});
		}

		internal static void IncrimentLiquidMapEntries(ILContext il)
		{
			ILCursor c = new(il);
			c.GotoNext(MoveType.After, i => i.MatchLdloc(18), i => i.MatchStsfld("Terraria.Map.MapHelper", "liquidPosition"));
			c.EmitDelegate(() =>
			{
				MapLiquidLoader.liquidLookup = new ushort[LiquidID.Count];
			});
			c.GotoNext(MoveType.After, i => i.MatchLdloc(49), i => i.MatchLdelemAny<Color>(), i => i.MatchStelemAny<Color>());
			c.EmitLdloc(49);
			c.EmitLdloc(18);
			c.EmitDelegate((int num18, ushort num13) =>
			{
				MapLiquidLoader.liquidLookup[num18] = num13;
			});
			c.GotoNext(MoveType.After, i => i.MatchLdloc(49), i => i.MatchLdcI4(4));
			c.EmitDelegate((int four) =>
			{
				return LiquidID.Count;
			});
		}
	}
}
