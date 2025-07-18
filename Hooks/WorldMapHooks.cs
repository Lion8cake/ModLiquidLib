using ModLiquidLib.IO;
using MonoMod.Cil;

namespace ModLiquidLib.Hooks
{
	internal class WorldMapHooks
	{
		internal static void InitaliseTLMap(ILContext il)
		{
			ILCursor c = new(il);
			int text_varNum = -1;
			int cloudSave_varNum = -1;
			c.GotoNext(MoveType.After, i => i.MatchLdloc(out text_varNum), i => i.MatchLdloc(out cloudSave_varNum), i => i.MatchCall("Terraria.ModLoader.IO.MapIO", "ReadModFile"));
			c.EmitLdloc(text_varNum);
			c.EmitLdloc(cloudSave_varNum);
			c.EmitDelegate((string text, bool isCloudSave) =>
			{
				MapLiquidIO.ReadModFile(text, isCloudSave);
			});
		}
	}
}
