using Microsoft.Xna.Framework;
using ModLiquidLib.ModLoader;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			c.GotoNext(MoveType.After, i => i.MatchNewobj<List<LocalizedText>>(".ctor"), i => i.MatchStloc(1));
			c.EmitLdloca(0);
			c.EmitLdloca(1);
			c.EmitDelegate((ref IList<Color> colors, ref IList<LocalizedText> names) =>
			{
				Array.Resize(ref MapLiquidLoader.liquidLookup, LiquidLoader.LiquidCount);
				ModContent.GetInstance<ModLiquidLib>().Logger.Debug("liquidLookup size: " + MapLiquidLoader.liquidEntries.Count);
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

		internal static void AddLiquidColorstoMap(ILContext il)
		{
			ILCursor c = new(il);
			ILLabel IL_0000 = c.DefineLabel(); //the else if for liquids
			ILLabel IL_0069 = c.DefineLabel(); //the else if for walls
			c.GotoNext(MoveType.Before, i => i.MatchLdsfld("Terraria.ModLoader.MapLoader", "entryToWall"), i => i.MatchLdarg(0), i => i.MatchLdindU2(), i => i.MatchCallvirt<IDictionary<ushort, ushort>>("ContainsKey"));
			c.MarkLabel(IL_0000);
			c.GotoPrev(MoveType.After, i => i.MatchLdsfld("Terraria.ModLoader.MapLoader", "entryToTile"), i => i.MatchLdarg(0), i => i.MatchLdindU2(), i => i .MatchCallvirt<IDictionary<ushort, ushort>>("ContainsKey"), i => i.MatchBrfalse(out IL_0069));
			if (c.Prev != null)
			{
				c.Prev.Operand = IL_0000;
			}
			c.GotoNext(MoveType.Before, i => i.MatchLdsfld("Terraria.ModLoader.MapLoader", "entryToWall"), i => i.MatchLdarg(0), i => i.MatchLdindU2(), i => i.MatchCallvirt<IDictionary<ushort, ushort>>("ContainsKey"));
			c.EmitLdarg(0);
			c.EmitDelegate((ushort mapType) =>
			{
				return MapLiquidLoader.entryToLiquid.ContainsKey(mapType);
			});
			c.EmitBrfalse(IL_0069);
			c.EmitLdarga(0);
			c.EmitLdarg(1);
			c.EmitLdarg(2);
			c.EmitDelegate((ref ushort mapType, int i, int j) =>
			{
				ModLiquid liquid = LiquidLoader.GetLiquid(MapLiquidLoader.entryToLiquid[mapType]);
				ushort option = liquid.GetMapOption(i, j);
				if (option < 0 || option >= MapLiquidLoader.modLiquidOptions(liquid.Type))
				{
					throw new ArgumentOutOfRangeException("Bad map option for liquid " + liquid.Name + " from mod " + liquid.Mod.Name);
				}
				mapType += option; //turned into IL due to mapType being an arg
			});
		}
	}
}
