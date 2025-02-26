using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
