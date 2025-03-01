using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace ModLiquidLib.Hooks
{
	internal class LiquidHooks
	{
		internal static void EditLiquidUpdates(ILContext il)
		{
			ILCursor c = new(il);
			ILLabel IL_00cf = c.DefineLabel();
			c.GotoNext(MoveType.Before, i => i.MatchLdcR4(0.0f), i => i.MatchStloc(6));
			c.MarkLabel(IL_00cf);
			c.GotoPrev(MoveType.After, i => i.MatchLdloca(4), i => i.MatchCall<Tile>("get_liquid"), i => i.MatchLdindU1(), i => i.MatchStloc(5));
			c.EmitLdarg(0);
			c.EmitDelegate((Liquid self) =>
			{
				return LiquidLoader.UpdateLiquid(self.x, self.y, Main.tile[self.x, self.y].LiquidType, self);
			});
			c.EmitBrtrue(IL_00cf);
			c.EmitRet();

			c.GotoNext(MoveType.Before, i => i.MatchBrtrue(out _), i => i.MatchLdloca(4), i => i.MatchCall<Tile>("get_liquid"), i => i.MatchLdindU1(), i => i.MatchLdcI4(0), i => i.MatchBle(out _));
			c.EmitLdloc(4);
			c.EmitDelegate((bool origLiquidID, Tile tile) =>
			{
				bool? flag = LiquidLoader.EvaporatesInHell(tile.X(), tile.Y(), tile.LiquidType);
				if (flag == null)
				{
					return origLiquidID;
				}
				return !(bool)flag;
			});
		}

		internal static void EditLiquidGenMovement(ILContext il)
		{
			ILCursor c = new(il);
			ILLabel IL_0000 = c.DefineLabel();
			c.GotoNext(MoveType.After, i => i.MatchLdarg(0), i => i.MatchStloc(1));
			c.EmitLdarg(0);
			c.EmitLdarg(1);
			c.EmitDelegate((int x, int y) =>
			{
				return !LiquidLoader.SettleLiquidMovement(x, y, Main.tile[x, y].LiquidType);
			});
			c.EmitBrfalse(IL_0000);
			c.EmitRet();
			c.MarkLabel(IL_0000);
		}
	}
}
