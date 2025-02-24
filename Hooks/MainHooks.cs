using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;
using MonoMod.Cil;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Liquid;
using Terraria.ID;
using Terraria.Utilities.Terraria.Utilities;

namespace ModLiquidLib.Hooks
{
	internal class MainHooks
	{
		internal static void EditOldLiquidRendering(ILContext il)
		{
			ILCursor c = new(il);
			ILLabel IL_0000 = c.DefineLabel();
			ILLabel IL_12ec = null;
			ILLabel IL_0b25 = null;
			c.GotoNext(MoveType.After, i => i.MatchBrfalse(out IL_12ec), i => i.MatchLdloc(12), i => i.MatchLdloc(11), i => i.MatchCall<Lighting>("GetColor"), i => i.MatchStloc(13));
			if (IL_12ec == null)
			{
				return;
			}
			c.EmitLdloc(12);
			c.EmitLdloc(11);
			c.EmitDelegate((int j, int i) =>
			{
				return (!LiquidLoader.PreRetroDraw(j, i, Main.tile[j, i].LiquidType, Main.spriteBatch));
			});
			c.EmitBrtrue(IL_12ec);

			c.GotoNext(MoveType.After, i => i.MatchLdsfld<Dust>("lavaBubbles"), i => i.MatchLdcI4(200), i => i.MatchBge(out IL_0b25));
			if (IL_0b25 == null) return;
			c.EmitLdloc(12);
			c.EmitLdloc(11);
			c.EmitDelegate((int j, int i) =>
			{
				return LiquidLoader.DisableRetroLavaBubbles(j, i);
			});
			c.EmitBrtrue(IL_0b25);

			c.GotoNext(MoveType.After, i => i.MatchLdloca(13), i => i.MatchCall<Color>("get_R"), i => i.MatchConvR4(), i => i.MatchLdloc(17), i => i.MatchMul(), i => i.MatchStloc(21));
			c.EmitLdloc(12); //j (x)
			c.EmitLdloc(11); //i (y)
			c.EmitLdloca(36); // newColor (color used to set all colors)
			c.EmitLdloca(17); //num9 (alpha)
			c.EmitLdloca(19); //value (liquid frame)
			c.EmitLdloca(18); //vector2 (screen positioning)
			c.EmitLdloca(25); //num22 (waterfall length (in tiles)
			c.EmitLdloca(26); //num24 (waterfall strength (how much it degrades over tiles))
			c.EmitLdloc(6); //num29 (quality amount)
			c.EmitLdloc(14); //num3 (liquid amount percentage(?)) 
			c.EmitLdloca(13); //color (color)
			c.EmitLdloca(21); //num16 (R value for color)
			c.EmitDelegate((int j, int i, ref Color newColor, ref float num9, ref Rectangle value, ref Vector2 vector2, ref float num22, ref float num24, int num29, float num3, ref Color color, ref float num16) =>
			{
				num22 = 6f;
				num24 = 0.5f;
				newColor = Color.Transparent;
				RetroLiquidDrawInfo info = new RetroLiquidDrawInfo();
				info.tileCache = Main.tile[j, i];
				info.liquidAlphaMultiplier = num9;
				info.liquidColor = newColor;
				info.liquidFraming = value;
				info.liquidPositionOffset = vector2;
				info.waterfallLength = (int)num22;
				info.waterfallStrength = num24;
				LiquidLoader.RetroDrawEffects(j, i, Main.tile[j, i].LiquidType, Main.spriteBatch, ref info, num3, num29);
				num9 = info.liquidAlphaMultiplier;
				newColor = info.liquidColor;
				value = info.liquidFraming;
				vector2 = info.liquidPositionOffset;
				num22 = info.waterfallLength;
				num24 = info.waterfallStrength;
				if (newColor != Color.Transparent)
				{
					color = newColor;
				}
				num16 = (float)(int)color.R * num9;
			});

			while (c.TryGotoNext(MoveType.After, i => i.MatchCall<Lighting>("GetColor"), i => i.MatchStloc(47)))
			{
				c.EmitLdloc(36);
				c.EmitLdloca(47);
				c.EmitDelegate((Color newColor, ref Color color4) =>
				{
					if (newColor != Color.Transparent)
					{
						color4 = newColor;
					}
				});
			}

			c.GotoNext(MoveType.After, i => i.MatchCall<Lighting>("GetColor"), i => i.MatchStloc(13));
			c.EmitLdloc(36);
			c.EmitLdloca(13);
			c.EmitDelegate((Color newColor, ref Color color) =>
			{
				if (newColor != Color.Transparent)
				{
					color = newColor;
				}
			});

			c.GotoNext(MoveType.After, i => i.MatchLdcR4(6), i => i.MatchStloc(25));
			c.GotoPrev(MoveType.After, i => i.MatchLdcR4(6));
			c.EmitLdloc(25);
			c.EmitDelegate((float six, float num22) =>
			{
				return num22;
			});

			c.GotoNext(MoveType.After, i => i.MatchLdcR4(0.75f), i => i.MatchStloc(26));
			c.GotoPrev(MoveType.After, i => i.MatchLdcR4(0.75f));
			c.EmitLdloc(26);
			c.EmitDelegate((float pointSevenFive, float num24) =>
			{
				return num24;
			});

			c.GotoNext(MoveType.After, i => i.MatchLdloc(49), i => i.MatchConvR4(), i => i.MatchLdloc(25), i => i.MatchBlt(out _));
			c.MarkLabel(IL_0000);
			c.EmitLdloc(12);
			c.EmitLdloc(11);
			c.EmitDelegate((int j, int i) =>
			{
				LiquidLoader.PostRetroDraw(j, i, Main.tile[j, i].LiquidType, Main.spriteBatch);
			});
			c.GotoPrev(MoveType.After, i => i.MatchLdloc(12), i => i.MatchLdloc(50), i => i.MatchLdcI4(0), i => i.MatchCall<WorldGen>("SolidTile"));
			if (IL_0000 != null)
				c.Next.Operand = IL_0000;
			c.GotoPrev(MoveType.After, i => i.MatchLdloca(27), i => i.MatchCall<Tile>("halfBrick"));
			if (IL_0000 != null)
				c.Next.Operand = IL_0000;
			c.GotoPrev(MoveType.After, i => i.MatchLdcR4(99999));
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
