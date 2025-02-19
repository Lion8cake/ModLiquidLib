using Microsoft.Xna.Framework.Graphics;
using ModLiquidLib.ModLoader;
using MonoMod.Cil;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace ModLiquidLib.Hooks
{
	internal class MainHooks
	{
		internal static void EditOldLiquidRendering(ILContext il)
		{
			ILCursor c = new(il);
			while (c.TryGotoNext(MoveType.After, i => i.MatchLdsfld("Terraria.GameContent.TextureAssets", "Liquid"), i => i.MatchLdloc(16), i => i.MatchLdelemRef(), i => i.MatchCallvirt<Asset<Texture2D>>("get_Value")))
			{
				c.EmitLdloc(12);
				c.EmitLdloc(11);
				c.EmitDelegate((Texture2D texture, int x, int y) =>
				{
					int type = Main.tile[x, y].LiquidType;
					return type < LiquidID.Count ? texture : LiquidLoader.LiquidBlockAssets[type].Value;
				});
			}
		}
	}
}
