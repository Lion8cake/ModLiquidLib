using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;

namespace ModLiquidLib.Hooks
{
	internal class ModContentHooks
	{
		internal static void ResizeArrayTest(On_ModContent.orig_ResizeArrays orig, bool unloading = false)
		{
			orig.Invoke(unloading);
			LiquidLoader.ResizeArrays(unloading);
		}
	}
}
