using ModLiquidLib.ID;
using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils.ManualHooks;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModLiquidLib.Hooks
{
	internal class ModContentHooks
	{
		internal static void ResizeArraysLiquid(On_ModContent.orig_ResizeArrays orig, bool unloading = false)
		{
			orig.Invoke(unloading);
			LiquidLoader.ResizeArrays(unloading);
			for (int i = 0; i < TileLoader.TileCount; i++)
			{
				System.Array.Resize(ref LiquidID_TLmod.Sets.CountsAsLiquidSource[i], LiquidLoader.LiquidCount);
			}
			for (int i = 0; i < NPCLoader.NPCCount; i++)
			{
				System.Array.Resize(ref LiquidID_TLmod.Sets.CanModdedNPCSpawnInModdedLiquid[i], LiquidLoader.LiquidCount);
			}
		}
	}
}
