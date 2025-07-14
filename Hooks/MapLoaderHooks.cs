using Microsoft.Xna.Framework;
using ModLiquidLib.ModLoader;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;

namespace ModLiquidLib.Hooks
{
	internal class MapLoaderHooks
	{
		internal static void AddLiquidToFinishSetup(ILContext il)
		{
			ILCursor c = new(il);
			int colorArr_numVar = -1;
			int nameArr_numVar = -1;
			c.GotoNext(MoveType.After, i => i.MatchStloc(out colorArr_numVar), i => i.MatchNewobj<List<LocalizedText>>(".ctor"), i => i.MatchStloc(out nameArr_numVar));
			c.EmitLdloca(colorArr_numVar);
			c.EmitLdloca(nameArr_numVar);
			c.EmitDelegate((ref IList<Color> colors, ref IList<LocalizedText> names) =>
			{
				Array.Resize(ref MapLiquidLoader.liquidLookup, LiquidLoader.LiquidCount);
				foreach (ushort type2 in MapLiquidLoader.liquidEntries.Keys)
				{
					MapLiquidLoader.liquidLookup[type2] = (ushort)(MapHelper.modPosition + colors.Count);
					foreach (ModLoader.MapEntry entry2 in MapLiquidLoader.liquidEntries[type2])
					{
						ushort mapType2 = (ushort)(MapHelper.modPosition + colors.Count);
						MapLiquidLoader.entryToLiquid[mapType2] = type2;
						MapLoader.nameFuncs[mapType2] = entry2.getName;
						colors.Add(entry2.color);
						if (entry2.name != null)
						{
							names.Add(entry2.name);
							continue;
						}
						throw new Exception("How did this happen?");
					}
				}
			});
		}
	}
}
