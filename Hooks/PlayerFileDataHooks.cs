using MonoMod.Cil;
using System;

namespace ModLiquidLib.Hooks
{
	internal class PlayerFileDataHooks
	{
		internal static void AddLiquidMapFile(ILContext il)
		{
			ILCursor c = new(il);
			c.GotoNext(MoveType.After, i => i.MatchLdstr(".tmap"), i => i.MatchLdcI4(1), i => i.MatchCallvirt<String>("EndsWith"));
			c.EmitLdarg(2);
			c.EmitDelegate((bool tmodFileExists, string filePath) =>
			{
				return tmodFileExists && !filePath.EndsWith(".tlmap", StringComparison.CurrentCultureIgnoreCase);
			});
		}
	}
}
