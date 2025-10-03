using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System.ComponentModel;
using System.Reflection;
using Terraria.ModLoader;

namespace ModLiquidLib.Utils.ManualHooks
{
	internal static class IL_NPCLoader
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal static ILHook ILHook_ChooseSpawn = null;

		public static event ILContext.Manipulator ChooseSpawn
		{
			add
			{
				ILHook_ChooseSpawn = new ILHook(typeof(NPCLoader).GetMethod("ChooseSpawn", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance), value);
				if (ILHook_ChooseSpawn != null)
					ILHook_ChooseSpawn.Apply();
			}
			remove
			{
				if (ILHook_ChooseSpawn != null)
					ILHook_ChooseSpawn.Dispose();
			}
		}
	}
}
