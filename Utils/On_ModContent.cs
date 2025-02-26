using MonoMod.RuntimeDetour;
using System.ComponentModel;
using System.Reflection;
using Terraria.ModLoader;

namespace ModLiquidLib.Utils
{
	public static class On_ModContent
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public delegate void orig_ResizeArrays(bool unloading = false);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public delegate void hook_ResizeArrays(orig_ResizeArrays orig, bool unloading = false);

		[EditorBrowsable(EditorBrowsableState.Never)]
		internal static Hook Hook_ResizeArrays = null;

		public static event hook_ResizeArrays ResizeArrays
		{
			add
			{
				Hook_ResizeArrays = new Hook(typeof(ModContent).GetMethod("ResizeArrays", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance), value);
				if (Hook_ResizeArrays != null)
					Hook_ResizeArrays.Apply();
			}
			remove
			{
				if (Hook_ResizeArrays != null)
					Hook_ResizeArrays.Dispose();
			}
		}
	}
}
