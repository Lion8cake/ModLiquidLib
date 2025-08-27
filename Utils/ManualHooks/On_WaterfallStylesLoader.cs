using MonoMod.RuntimeDetour;
using System.ComponentModel;
using System.Reflection;
using Terraria.ModLoader;

namespace ModLiquidLib.Utils.ManualHooks
{
	public static class On_WaterfallStylesLoader
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public delegate void orig_ResizeArrays(WaterFallStylesLoader self);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public delegate void hook_ResizeArrays(orig_ResizeArrays orig, WaterFallStylesLoader self);

		[EditorBrowsable(EditorBrowsableState.Never)]
		internal static Hook Hook_ResizeArrays = null;

		public static event hook_ResizeArrays ResizeArrays
		{
			add
			{
				Hook_ResizeArrays = new Hook(typeof(WaterFallStylesLoader).GetMethod("ResizeArrays", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance), value);
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
