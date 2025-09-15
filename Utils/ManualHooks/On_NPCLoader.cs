using MonoMod.RuntimeDetour;
using System.ComponentModel;
using System.Reflection;
using Terraria.ModLoader;

namespace ModLiquidLib.Utils.ManualHooks
{
	public static class On_NPCLoader
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public delegate int? orig_ChooseSpawn(NPCSpawnInfo spawnInfo);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public delegate int? hook_ChooseSpawn(orig_ChooseSpawn orig, NPCSpawnInfo spawnInfo);

		[EditorBrowsable(EditorBrowsableState.Never)]
		internal static Hook Hook_ChooseSpawn = null;

		public static event hook_ChooseSpawn ChooseSpawn
		{
			add
			{
				Hook_ChooseSpawn = new Hook(typeof(NPCLoader).GetMethod("ChooseSpawn", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance), value);
				if (Hook_ChooseSpawn != null)
					Hook_ChooseSpawn.Apply();
			}
			remove
			{
				if (Hook_ChooseSpawn != null)
					Hook_ChooseSpawn.Dispose();
			}
		}
	}
}
