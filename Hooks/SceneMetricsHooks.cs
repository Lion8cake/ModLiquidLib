using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

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
