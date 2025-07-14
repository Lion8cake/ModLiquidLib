using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;
using System;
using Terraria;

namespace ModLiquidLib.Hooks
{
	internal class SceneMetricsHooks
	{
		internal static void ResizeLiquidArray(On_SceneMetrics.orig_Reset orig, SceneMetrics self)
		{
			int[] _liquidCounts = self.Get_liquidCounts();
			Array.Resize(ref _liquidCounts, LiquidLoader.LiquidCount);
			self.Set_liquidCounts(_liquidCounts);
			orig.Invoke(self);
		}
	}
}
