using ModLiquidLib.ModLoader;
using MonoMod.Cil;
using Terraria;

namespace ModLiquidLib.Hooks
{
	internal class WiringHooks
	{
		internal static void LiquidPumpEdits(ILContext il)
		{
			ILCursor c = new(il);

			ILLabel IL_017f = null;

			int inPumpIndex_varNum = -1;
			int outPumpIndex_varNum = -1;
			int liquidTypeB_varNum = -1;

			c.GotoNext(i => i.MatchLdsfld("Terraria.Wiring", "_inPumpX"), i => i.MatchLdloc(out inPumpIndex_varNum));
			c.GotoNext(i => i.MatchLdsfld("Terraria.Wiring", "_outPumpX"), i => i.MatchLdloc(out outPumpIndex_varNum));

			c.GotoNext(MoveType.After, i => i.MatchLdloc(out _), i => i.MatchLdloc(out liquidTypeB_varNum), i => i.MatchBneUn(out IL_017f));
			c.EmitLdloc(liquidTypeB_varNum);
			c.EmitLdloc(inPumpIndex_varNum);
			c.EmitLdloc(outPumpIndex_varNum);
			c.EmitDelegate((int b, int i, int j) =>
			{
				return LiquidLoader.OnPump(b, Wiring._inPumpX[i], Wiring._inPumpY[i], Wiring._outPumpX[j], Wiring._outPumpY[j]);
			});
			c.EmitBrfalse(IL_017f);
		}
	}
}
