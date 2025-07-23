using MonoMod.RuntimeDetour;
using System.ComponentModel;
using System.Reflection;
using Terraria.ModLoader.UI;

namespace ModLiquidLib.Utils.ManualHooks
{
	public static class On_UIModItem
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public delegate void orig_OnInitialize(UIModItem self);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public delegate void hook_OnInitialize(orig_OnInitialize orig, UIModItem self);

		[EditorBrowsable(EditorBrowsableState.Never)]
		internal static Hook Hook_OnInitialize = null;

		public static event hook_OnInitialize OnInitialize
		{
			add
			{
				Hook_OnInitialize = new Hook(typeof(UIModItem).GetMethod("OnInitialize", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance), value);
				if (Hook_OnInitialize != null)
					Hook_OnInitialize.Apply();
			}
			remove
			{
				if (Hook_OnInitialize != null)
					Hook_OnInitialize.Dispose();
			}
		}
	}
}
