using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria.Map;
using Terraria.ModLoader;

namespace ModLiquidLib.Utils.ManualHooks
{
	public static class IL_MapLoader
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal static ILHook ILHook_FinishSetup = null;

		public static event ILContext.Manipulator FinishSetup
		{
			add
			{
				ILHook_FinishSetup = new ILHook(typeof(MapLoader).GetMethod("FinishSetup", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance), value);
				if (ILHook_FinishSetup != null)
					ILHook_FinishSetup.Apply();
			}
			remove
			{
				if (ILHook_FinishSetup != null)
					ILHook_FinishSetup.Dispose();
			}
		}
	}
}
