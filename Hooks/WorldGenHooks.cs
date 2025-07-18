using ModLiquidLib.ModLoader;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.ID;

namespace ModLiquidLib.Hooks
{
	internal class WorldGenHooks
	{
		internal static void FixPlaceLiquidMerging(ILContext il)
		{
			ILCursor c = new(il);
			int liquidMergeTileType_varNum = -1;
			int liquidMergeType_varNum = -1;

			c.GotoNext(MoveType.After, i => i.MatchLdloca(out liquidMergeTileType_varNum), i => i.MatchLdloca(out liquidMergeType_varNum), 
				i => i.MatchLdloc(out _), i => i.MatchLdloc(out _), i => i.MatchLdloc(out _), i => i.MatchLdloc(out _),
				i => i.MatchCall<Liquid>("GetLiquidMergeTypes"));
			c.EmitLdarg(0);
			c.EmitLdarg(1);
			c.EmitLdarg(2);
			c.EmitLdloca(liquidMergeTileType_varNum);
			c.EmitLdloca(liquidMergeType_varNum);
			c.EmitDelegate((int x, int y, int liquidType, ref int liquidMergeTileType, ref int liquidMergeType) =>
			{
				bool[] nearbyLiquids = new bool[LiquidLoader.LiquidCount];
				Array.Fill(nearbyLiquids, false);
				nearbyLiquids[Main.tile[x, y].LiquidType] = true;
				LiquidHooks.GetLiquidMergeTypes(x, y, liquidType, out liquidMergeTileType, out liquidMergeType, nearbyLiquids);
			});

			c.GotoNext(MoveType.Before, i => i.MatchBrfalse(out _), i => i.MatchLdloca(out _), i => i.MatchCall<Tile>("get_liquid"));
			c.EmitLdarg(0);
			c.EmitLdarg(1);
			c.EmitLdarg(2);
			c.EmitLdloc(liquidMergeType_varNum);
			c.EmitDelegate((bool hasAvaliableTile, int x, int y, int liquidType, int liquidMergeType) =>
			{
				return hasAvaliableTile && LiquidLoader.PreLiquidMerge(x, y, x, y, liquidType, liquidMergeType);
			});
		}

		internal static void PreventSoundOverloadFromExecuting(On_WorldGen.orig_PlayLiquidChangeSound orig, TileChangeType eventType, int x, int y, int count)
		{
			return;
			orig.Invoke(eventType, x, y, count);
		}
	}
}
