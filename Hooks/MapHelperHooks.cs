using Microsoft.Xna.Framework;
using ModLiquidLib.ModLoader;
using MonoMod.Cil;
using Terraria.ID;

namespace ModLiquidLib.Hooks
{
	internal class MapHelperHooks
	{
		internal static void AddLiquidMapEntrys(ILContext il)
		{
			ILCursor c = new(il);
			int mapTile_varNum = -1;
			int mapIndex_varNum = -1;
			int mapType_varNum = -1;
			c.GotoNext(MoveType.After, i => i.MatchLdsfld("Terraria.Map.MapHelper", "liquidPosition"), i => i.MatchLdloc(out mapIndex_varNum), i => i.MatchAdd(), i => i.MatchStloc(out mapTile_varNum));
			c.EmitLdloca(mapTile_varNum);
			c.EmitLdloc(mapIndex_varNum);
			c.EmitDelegate((ref int num5, int num7) =>
			{
				num5 = MapLiquidLoader.liquidLookup[num7];
			});
			c.GotoNext(MoveType.After, i => i.MatchLdloca(out mapType_varNum), i => i.MatchLdarg(0), i => i.MatchLdarg(1), i => i.MatchCall("Terraria.ModLoader.MapLoader", "ModMapOption"));
			c.EmitLdloca(mapType_varNum);
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
			int mapChoiceIndex_varNum = -1;
			int mapChoice_varNum = -1;
			c.GotoNext(MoveType.After, i => i.MatchLdloc(out mapChoiceIndex_varNum), i => i.MatchStsfld("Terraria.Map.MapHelper", "liquidPosition"));
			c.EmitDelegate(() =>
			{
				MapLiquidLoader.liquidLookup = new ushort[LiquidID.Count];
			});
			c.GotoNext(MoveType.After, i => i.MatchLdloc(out mapChoice_varNum), i => i.MatchLdelemAny<Color>(), i => i.MatchStelemAny<Color>());
			c.EmitLdloc(mapChoice_varNum);
			c.EmitLdloc(mapChoiceIndex_varNum);
			c.EmitDelegate((int num18, ushort num13) =>
			{
				MapLiquidLoader.liquidLookup[num18] = num13;
			});
			c.GotoNext(MoveType.After, i => i.MatchLdloc(mapChoice_varNum), i => i.MatchLdcI4(4));
			c.EmitDelegate((int four) =>
			{
				return LiquidID.Count;
			});
		}
	}
}
