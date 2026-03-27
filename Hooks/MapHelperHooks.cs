using Microsoft.Xna.Framework;
using ModLiquidLib.IO;
using ModLiquidLib.ModLoader;
using MonoMod.Cil;
using System;
using Terraria.ID;

namespace ModLiquidLib.Hooks
{
	internal class MapHelperHooks
	{
		internal static void SaveTLMap(ILContext il)
		{
			ILCursor c = new(il);
			int text_varNum = -1;
			int cloudSave_varNum = -1;
			c.GotoNext(MoveType.After, i => i.MatchLdloc(out text_varNum), i => i.MatchLdloc(out cloudSave_varNum), i => i.MatchCall("Terraria.ModLoader.IO.MapIO", "WriteModFile"));
			c.EmitLdloc(text_varNum);
			c.EmitLdloc(cloudSave_varNum);
			c.EmitDelegate((string text, bool isCloudSave) =>
			{
				MapLiquidIO.WriteModFile(text, isCloudSave);
			});
		}

		internal static void LiquidMapEntries(MonoMod.Cil.ILContext il)
		{
			ILCursor c = new(il);
			int mapTile_varNum = -1;
			int mapIndex_varNum = -1;
			c.GotoNext(MoveType.After, i => i.MatchLdarg(5), i => i.MatchLdsfld("Terraria.Map.MapHelper", "liquidPosition"), i => i.MatchLdloc(out mapIndex_varNum), i => i.MatchAdd(), i => i.MatchStindI4(), i => i.MatchRet());
			c.EmitLdarga(5);
			c.EmitLdloc(mapIndex_varNum);
			c.EmitDelegate((ref int baseType, int num) =>
			{
				baseType = MapLiquidLoader.liquidLookup[num];
			});
		}

		internal static void AddLiquidMapEntrys(ILContext il)
		{
			ILCursor c = new(il);
			int mapType_varNum = -1;
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
