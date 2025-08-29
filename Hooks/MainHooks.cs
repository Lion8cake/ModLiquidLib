using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils.Structs;
using MonoMod.Cil;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModLiquidLib.Hooks
{
	internal class MainHooks
	{
		internal static int[] wFallFrame = new int[ID.WaterfallID.Count];

		internal static int[] wFallFrameBack = new int[ID.WaterfallID.Count];

		internal static int[] wFallFrameCounter = new int[ID.WaterfallID.Count];

		internal static void ModifyStopWatchLiquidMultipliers(ILContext il)
		{
			ILCursor c = new(il);
			ILLabel IL_0ce3 = null;
			int stopwatchMPH_varNum = -1;
			c.GotoNext(MoveType.After, i => i.MatchLdfld<Entity>("honeyWet"), i => i.MatchBrfalse(out _), i => i.MatchLdloc(out stopwatchMPH_varNum));
			c.GotoPrev(MoveType.After, i => i.MatchLdfld<Player>("ignoreWater"), i => i.MatchBrtrue(out IL_0ce3));
			c.EmitLdloca(stopwatchMPH_varNum);
			c.EmitDelegate((ref float num13) =>
			{
				float multiplier = 1f;
				if (Main.player[Main.myPlayer].shimmerWet)
				{
					multiplier = 0.375f;
				}
				else if (Main.player[Main.myPlayer].honeyWet)
				{
					multiplier = 0.25f;
				}
				else if (Main.player[Main.myPlayer].wet)
				{
					multiplier = 0.5f;
				}
				LiquidLoader.StopWatchMPHMultiplier(PlayerHooks.WetToLiquidID(Main.player[Main.myPlayer]), ref multiplier);
				num13 *= multiplier;
			});
			c.EmitBr(IL_0ce3);
		}

		internal static void RenderWaterTiles(On_Main.orig_DrawTileInWater orig, Vector2 drawOffset, int x, int y)
		{
			orig.Invoke(drawOffset, x, y);
			if (Main.tile[x, y].HasTile && TileLoader.GetTile(Main.tile[x, y].TileType) is ILiquidModTile liquidTile)
			{
				Main.instance.LoadTiles(Main.tile[x, y].TileType);
				liquidTile.DrawTileInWater(x, y, Main.spriteBatch);
			}
		}

		internal static void EditOldLiquidRendering(ILContext il)
		{
			ILCursor c = new(il);
			ILLabel IL_0000 = c.DefineLabel();
			ILLabel IL_12ec = null;
			ILLabel IL_0b25 = null;
			int x_varNum = -1;
			int y_varNum = -1;
			int color_varNum = -1;
			int colorAlpha_varNum = -1;
			int colorRValue_varNum = -1;
			int newColor_varNum = -1;
			int value_varNUm = -1;
			int vector2ScreenPos_varNum = -1;
			int qualityNum_varNum = -1;
			int liquidAmountPer_varNum = -1;
			int liquidfallLength_varNum = -1;
			int liquidfallStrength_varNum = -1;
			int color2_varNum = -1;
			c.GotoNext(i => i.MatchLdcR4(40), i => i.MatchLdsfld<Main>("gfxQuality"), i => i.MatchMul(), i => i.MatchAdd(), i => i.MatchConvI4(), i => i.MatchStloc(out qualityNum_varNum));
			c.GotoNext(i => i.MatchStloc(out liquidAmountPer_varNum), i => i.MatchLdloc(liquidAmountPer_varNum), i => i.MatchLdcR4(32), i => i.MatchDiv(), i => i.MatchStloc(liquidAmountPer_varNum));
			c.GotoNext(i => i.MatchLdloca(out vector2ScreenPos_varNum), i => i.MatchLdloc(out _), i => i.MatchLdcI4(16), i => i.MatchMul(), i => i.MatchConvR4(), i => i.MatchLdloc(out _), i => i.MatchLdcI4(16), i => i.MatchMul());
			c.GotoNext(i => i.MatchLdloca(out value_varNUm), i => i.MatchLdcI4(0), i => i.MatchLdcI4(0), i => i.MatchLdcI4(16), i => i.MatchLdcI4(16));
			c.GotoNext(i => i.MatchLdloca(out newColor_varNum), i => i.MatchLdcI4(255), i => i.MatchLdcI4(255), i => i.MatchLdcI4(255));
			c.GotoNext(i => i.MatchLdloc(out _), i => i.MatchLdcI4(1), i => i.MatchSub(), i => i.MatchLdloc(out _), i => i.MatchLdcI4(1), i => i.MatchSub(), i => i.MatchCall<Lighting>("GetColor"), i => i.MatchStloc(out color2_varNum));
			c.GotoNext(i => i.MatchCallvirt<SpriteBatch>("Draw"), i => i.MatchLdcR4(6), i => i.MatchStloc(out liquidfallLength_varNum), i => i.MatchLdcR4(0.75f), i => i.MatchStloc(out liquidfallStrength_varNum));
			c.Index = 0;

			c.GotoNext(MoveType.After, i => i.MatchBrfalse(out IL_12ec), i => i.MatchLdloc(out x_varNum), i => i.MatchLdloc(out y_varNum), i => i.MatchCall<Lighting>("GetColor"), i => i.MatchStloc(out color_varNum));
			if (IL_12ec == null)
			{
				return;
			}
			c.EmitLdloc(x_varNum);
			c.EmitLdloc(y_varNum);
			c.EmitDelegate((int j, int i) =>
			{
				return (!LiquidLoader.PreRetroDraw(j, i, Main.tile[j, i].LiquidType, Main.spriteBatch));
			});
			c.EmitBrtrue(IL_12ec);

			c.GotoNext(MoveType.After, i => i.MatchLdsfld<Dust>("lavaBubbles"), i => i.MatchLdcI4(200), i => i.MatchBge(out IL_0b25));
			if (IL_0b25 == null) return;
			c.EmitLdloc(x_varNum);
			c.EmitLdloc(y_varNum);
			c.EmitDelegate((int j, int i) =>
			{
				return LiquidLoader.DisableRetroLavaBubbles(j, i);
			});
			c.EmitBrtrue(IL_0b25);

			c.GotoNext(MoveType.After, i => i.MatchLdloca(out _), i => i.MatchCall<Color>("get_R"), i => i.MatchConvR4(), i => i.MatchLdloc(out colorAlpha_varNum), i => i.MatchMul(), i => i.MatchStloc(out colorRValue_varNum));
			c.EmitLdloc(x_varNum); //j (x)
			c.EmitLdloc(y_varNum); //i (y)
			c.EmitLdloca(newColor_varNum); // newColor (color used to set all colors)
			c.EmitLdloca(colorAlpha_varNum); //num9 (alpha)
			c.EmitLdloca(value_varNUm); //value (liquid frame)
			c.EmitLdloca(vector2ScreenPos_varNum); //vector2 (screen positioning)
			c.EmitLdloca(liquidfallLength_varNum); //num22 (waterfall length (in tiles)
			c.EmitLdloca(liquidfallStrength_varNum); //num24 (waterfall strength (how much it degrades over tiles))
			c.EmitLdloc(qualityNum_varNum); //num29 (quality amount)
			c.EmitLdloc(liquidAmountPer_varNum); //num3 (liquid amount percentage(?)) 
			c.EmitLdloca(color_varNum); //color (color)
			c.EmitLdloca(colorRValue_varNum); //num16 (R value for color)
			c.EmitDelegate((int j, int i, ref Color newColor, ref float num9, ref Rectangle value, ref Vector2 vector2, ref float num22, ref float num24, int num29, float num3, ref Color color, ref float num16) =>
			{
				num22 = 6f;
				num24 = 0.75f;
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

			while (c.TryGotoNext(MoveType.After, i => i.MatchCall<Lighting>("GetColor"), i => i.MatchStloc(color2_varNum)))
			{
				c.EmitLdloc(newColor_varNum);
				c.EmitLdloca(color2_varNum);
				c.EmitDelegate((Color newColor, ref Color color4) =>
				{
					if (newColor != Color.Transparent)
					{
						color4 = newColor;
					}
				});
			}

			c.GotoNext(MoveType.After, i => i.MatchCall<Lighting>("GetColor"), i => i.MatchStloc(color_varNum));
			c.EmitLdloc(newColor_varNum);
			c.EmitLdloca(color_varNum);
			c.EmitDelegate((Color newColor, ref Color color) =>
			{
				if (newColor != Color.Transparent)
				{
					color = newColor;
				}
			});

			c.GotoNext(MoveType.After, i => i.MatchLdcR4(6), i => i.MatchStloc(liquidfallLength_varNum));
			c.GotoPrev(MoveType.After, i => i.MatchLdcR4(6));
			c.EmitLdloc(liquidfallLength_varNum);
			c.EmitDelegate((float six, float num22) =>
			{
				return num22;
			});

			c.GotoNext(MoveType.After, i => i.MatchLdcR4(0.75f), i => i.MatchStloc(liquidfallStrength_varNum));
			c.GotoPrev(MoveType.After, i => i.MatchLdcR4(0.75f));
			c.EmitLdloc(liquidfallStrength_varNum);
			c.EmitDelegate((float pointSevenFive, float num24) =>
			{
				return num24;
			});

			c.GotoNext(MoveType.After, i => i.MatchLdloc(out _), i => i.MatchConvR4(), i => i.MatchLdloc(liquidfallLength_varNum), i => i.MatchBlt(out _));
			c.MarkLabel(IL_0000);
			c.EmitLdloc(x_varNum);
			c.EmitLdloc(y_varNum);
			c.EmitDelegate((int j, int i) =>
			{
				LiquidLoader.PostRetroDraw(j, i, Main.tile[j, i].LiquidType, Main.spriteBatch);
			});
			c.GotoPrev(MoveType.After, i => i.MatchLdloc(x_varNum), i => i.MatchLdloc(out _), i => i.MatchLdcI4(0), i => i.MatchCall<WorldGen>("SolidTile"));
			if (IL_0000 != null)
				c.Next.Operand = IL_0000;
			c.GotoPrev(MoveType.After, i => i.MatchLdloca(out _), i => i.MatchCall<Tile>("halfBrick"));
			if (IL_0000 != null)
				c.Next.Operand = IL_0000;
			c.GotoPrev(MoveType.After, i => i.MatchLdcR4(99999));
			while (c.TryGotoNext(MoveType.After, i => i.MatchLdsfld("Terraria.GameContent.TextureAssets", "Liquid"), i => i.MatchLdloc(out _), i => i.MatchLdelemRef(), i => i.MatchCallvirt<Asset<Texture2D>>("get_Value")))
			{
				c.EmitLdloc(x_varNum);
				c.EmitLdloc(y_varNum);
				c.EmitDelegate((Texture2D texture, int x, int y) =>
				{
					int type = Main.tile[x, y].LiquidType;
					return type < LiquidID.Count ? texture : LiquidLoader.LiquidBlockAssets[type].Value;
				});
			}
		}
	}
}
