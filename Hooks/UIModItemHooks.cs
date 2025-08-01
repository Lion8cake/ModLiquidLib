using Microsoft.Xna.Framework.Graphics;
using ModLiquidLib.ModLoader;
using System;
using System.Linq;
using System.Reflection;
using Terraria.GameContent;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;
using ModLiquidLib.Utils.ManualHooks;

namespace ModLiquidLib.Hooks
{
	internal class UIModItemHooks
	{
		//Code by HiKoNe and their Liquid library mod (2021)
		//
		//I cannot express how much of an inspiration LiquidLib was for ModLiquidLib and my modding journey as a whole.
		//The code, even now, holds up really well even 4 years later and I cannot thank LiquidLib/HiKoNe enough. - Lion8cake
		internal static void AddLiquidCount(On_UIModItem.orig_OnInitialize orig, UIModItem self)
		{
			orig.Invoke(self);
			var ass = typeof(Mod).Assembly;

			if (Terraria.ModLoader.ModLoader.TryGetMod(self.ModName, out var loadedMod))
			{
				int liquidCount = loadedMod.GetContent<ModLiquid>().Count();

				if (liquidCount > 0)
				{
					int baseOffset = -40;
					void ChangeOffset(int modCount)
					{
						if (modCount > 0)
						{
							baseOffset -= 18;
						}
					}
					ChangeOffset(loadedMod.GetContent<ModItem>().Count());
					ChangeOffset(loadedMod.GetContent<ModNPC>().Count());
					ChangeOffset(loadedMod.GetContent<ModTile>().Count());
					ChangeOffset(loadedMod.GetContent<ModWall>().Count());
					ChangeOffset(loadedMod.GetContent<ModBuff>().Count());
					ChangeOffset(loadedMod.GetContent<ModMount>().Count());

					var type_UIHoverImage = ass.GetType("Terraria.ModLoader.UI.UIHoverImage");
					var UIHoverImage = Activator.CreateInstance(type_UIHoverImage, Main.Assets.Request<Texture2D>(TextureAssets.InfoIcon[6].Name), Language.GetTextValue("Mods.ModLiquidLib.ModsXLiquids", liquidCount));
					var field_Left = type_UIHoverImage.GetField("Left", BindingFlags.Public | BindingFlags.Instance);
					field_Left.SetValue(UIHoverImage, new StyleDimension { Percent = 1f, Pixels = baseOffset });
					self.Append((UIElement)UIHoverImage);
				}
			}
		}
	}
}
