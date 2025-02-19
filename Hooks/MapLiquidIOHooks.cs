using ModLiquidLib.IO;
using MonoMod.Cil;
using System;

namespace ModLiquidLib.Hooks
{
	//Despite all other hooks being based on their own file name, this contains all edits for the map IO stuff
	internal class MapLiquidIOHooks
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

		internal static void InitaliseTLMap(ILContext il)
		{
			ILCursor c = new(il);
			c.GotoNext(MoveType.After, i => i.MatchCall("Terraria.ModLoader.IO.MapIO", "ReadModFile"));
			c.EmitLdloc(1);
			c.EmitLdloc(0);
			c.EmitDelegate((string text, bool isCloudSave) =>
			{
				MapLiquidIO.ReadModFile(text, isCloudSave);
			});
		}

		internal static void SaveTLMap(ILContext il)
		{
			ILCursor c = new(il);
			c.GotoNext(MoveType.After, i => i.MatchCall("Terraria.ModLoader.IO.MapIO", "WriteModFile"));
			c.EmitLdloc(1);
			c.EmitLdloc(0);
			c.EmitDelegate((string text, bool isCloudSave) =>
			{
				MapLiquidIO.WriteModFile(text, isCloudSave);
			});
		}
	}
}
