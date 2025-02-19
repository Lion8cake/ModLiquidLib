using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;
using MonoMod.Cil;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Liquid;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModLiquidLib.Hooks
{
	internal class TileDrawingHooks
	{
		internal static void EditSlopeLiquidRendering(ILContext il)
		{
			ILCursor c = new(il);
			c.GotoNext();
		}

		internal static void On_TileDrawing_DrawTile_LiquidBehindTile(On_TileDrawing.orig_DrawTile_LiquidBehindTile orig, TileDrawing self, bool solidLayer, bool inFrontOfPlayers, int waterStyleOverride, Vector2 screenPosition, Vector2 screenOffset, int tileX, int tileY, Tile tileCache)
		{
			Tile tile = Main.tile[tileX + 1, tileY];
			Tile tile2 = Main.tile[tileX - 1, tileY];
			Tile tile3 = Main.tile[tileX, tileY - 1];
			Tile tile4 = Main.tile[tileX, tileY + 1];
			if (!tileCache.HasTile || tileCache.IsActuated || Main.tileSolidTop[tileCache.TileType] || (tileCache.IsHalfBlock && (tile2.LiquidAmount > 160 || tile.LiquidAmount > 160) && Main.instance.waterfallManager.CheckForWaterfall(tileX, tileY)) || (TileID.Sets.BlocksWaterDrawingBehindSelf[tileCache.TileType] && tileCache.Slope == 0))
			{
				return;
			}
			int num = 0;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			int num2 = 0;
			int i = 0; ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			bool flag6 = false;
			int num3 = (int)tileCache.Slope;
			int num4 = (int)tileCache.BlockType;
			if (tileCache.TileType == 546 && tileCache.LiquidAmount > 0)
			{
				flag5 = true;
				flag4 = true;
				flag = true;
				flag2 = true;
				switch (tileCache.LiquidType)
				{
					case 0:
						flag6 = true;
						break;
					case 1:
						num2 = 1;
						break;
					case 2:
						num2 = 11;
						break;
					case 3:
						num2 = 14;
						break;
				}
				num = tileCache.LiquidAmount;
				i = tileCache.LiquidType;////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			}
			else
			{
				if (tileCache.LiquidAmount > 0 && num4 != 0 && (num4 != 1 || tileCache.LiquidAmount > 160))
				{
					flag5 = true;
					i = tileCache.LiquidType;////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
					switch (tileCache.LiquidType)
					{
						case 0:
							flag6 = true;
							break;
						case 1:
							num2 = 1;
							break;
						case 2:
							num2 = 11;
							break;
						case 3:
							num2 = 14;
							break;
					}
					if (tileCache.LiquidAmount > num)
					{
						num = tileCache.LiquidType;
					}
				}
				if (tile2.LiquidAmount > 0 && num3 != 1 && num3 != 3)
				{
					flag = true;
					i = tile2.LiquidType;////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
					switch (tile2.LiquidType)
					{
						case 0:
							flag6 = true;
							break;
						case 1:
							num2 = 1;
							break;
						case 2:
							num2 = 11;
							break;
						case 3:
							num2 = 14;
							break;
					}
					if (tile2.LiquidAmount > num)
					{
						num = tile2.LiquidAmount;
					}
				}
				if (tile.LiquidAmount > 0 && num3 != 2 && num3 != 4)
				{
					flag2 = true;
					i = tile.LiquidType;////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
					switch (tile.LiquidType)
					{
						case 0:
							flag6 = true;
							break;
						case 1:
							num2 = 1;
							break;
						case 2:
							num2 = 11;
							break;
						case 3:
							num2 = 14;
							break;
					}
					if (tile.LiquidAmount > num)
					{
						num = tile.LiquidAmount;
					}
				}
				if (tile3.LiquidAmount > 0 && num3 != 3 && num3 != 4)
				{
					flag3 = true;
					i = tile3.LiquidType;////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
					switch (tile3.LiquidType)
					{
						case 0:
							flag6 = true;
							break;
						case 1:
							num2 = 1;
							break;
						case 2:
							num2 = 11;
							break;
						case 3:
							num2 = 14;
							break;
					}
				}
				if (tile4.LiquidAmount > 0 && num3 != 1 && num3 != 2)
				{
					if (tile4.LiquidAmount > 240)
					{
						flag4 = true;
						i = tile4.LiquidType;////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
					}
					switch (tile4.LiquidType)
					{
						case 0:
							flag6 = true;
							break;
						case 1:
							num2 = 1;
							break;
						case 2:
							num2 = 11;
							break;
						case 3:
							num2 = 14;
							break;
					}
				}
			}
			if (!flag3 && !flag4 && !flag && !flag2 && !flag5)
			{
				return;
			}
			if (waterStyleOverride != -1)
			{
				Main.waterStyle = waterStyleOverride;
			}
			if (num2 == 0)
			{
				num2 = Main.waterStyle;
			}
			Lighting.GetCornerColors(tileX, tileY, out var vertices);
			Vector2 vector = new((float)(tileX * 16), (float)(tileY * 16));
			Rectangle liquidSize = new(0, 4, 16, 16);
			if (flag4 && (flag || flag2))
			{
				flag = true;
				flag2 = true;
			}
			if (tileCache.HasTile && (Main.tileSolidTop[tileCache.TileType] || !Main.tileSolid[tileCache.TileType]))
			{
				return;
			}
			if ((!flag3 || !(flag || flag2)) && !(flag4 && flag3))
			{
				if (flag3)
				{
					liquidSize = new(0, 4, 16, 4);
					if (tileCache.IsHalfBlock || tileCache.Slope != 0)
					{
						liquidSize = new(0, 4, 16, 12);
					}
				}
				else if (flag4 && !flag && !flag2)
				{
					vector = new((float)(tileX * 16), (float)(tileY * 16 + 12));
					liquidSize = new(0, 4, 16, 4);
				}
				else
				{
					float num8 = (float)(256 - num) / 32f;
					int y = 4;
					if (tile3.LiquidAmount == 0 && (num4 != 0 || !WorldGen.SolidTile(tileX, tileY - 1)))
					{
						y = 0;
					}
					int num5 = (int)num8 * 2;
					if (tileCache.Slope != 0)
					{
						vector = new((float)(tileX * 16), (float)(tileY * 16 + num5));
						liquidSize = new(0, num5, 16, 16 - num5);
					}
					else if ((flag && flag2) || tileCache.IsHalfBlock)
					{
						vector = new((float)(tileX * 16), (float)(tileY * 16 + num5));
						liquidSize = new(0, y, 16, 16 - num5);
					}
					else if (flag)
					{
						vector = new((float)(tileX * 16), (float)(tileY * 16 + num5));
						liquidSize = new(0, y, 4, 16 - num5);
					}
					else
					{
						vector = new((float)(tileX * 16 + 12), (float)(tileY * 16 + num5));
						liquidSize = new(0, y, 4, 16 - num5);
					}
				}
			}
			Vector2 position = vector - screenPosition + screenOffset;
			num = i;////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			float num6 = 0.5f;
			switch (num2)
			{
				case 1:
					num6 = 1f;
					break;
				case 11:
					num6 = Math.Max(num6 * 1.7f, 1f);
					break;
			}
			if ((double)tileY <= Main.worldSurface || num6 > 1f)
			{
				num6 = 1f;
				if (tileCache.WallType == 21)
				{
					num6 = 0.9f;
				}
				else if (tileCache.WallType > 0)
				{
					num6 = 0.6f;
				}
			}
			if (tileCache.IsHalfBlock && tile3.LiquidAmount > 0 && tileCache.WallType > 0)
			{
				num6 = 0f;
			}
			if (num3 == 4 && tile2.LiquidAmount == 0 && !WorldGen.SolidTile(tileX - 1, tileY))
			{
				num6 = 0f;
			}
			if (num3 == 3 && tile.LiquidAmount == 0 && !WorldGen.SolidTile(tileX + 1, tileY))
			{
				num6 = 0f;
			}
			ref Color bottomLeftColor = ref vertices.BottomLeftColor;
			bottomLeftColor *= num6;
			ref Color bottomRightColor = ref vertices.BottomRightColor;
			bottomRightColor *= num6;
			ref Color topLeftColor = ref vertices.TopLeftColor;
			topLeftColor *= num6;
			ref Color topRightColor = ref vertices.TopRightColor;
			topRightColor *= num6;
			bool flag7 = false;
			if (flag6)
			{
				int totalCount = 15;//LoaderManager.Get<WaterStylesLoader>().TotalCount;
				for (i = 0; i < totalCount; i++)////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
				{
					if (Main.IsLiquidStyleWater(i) && Main.liquidAlpha[i] > 0f && i != num2)
					{
						DrawPartialLiquid(self, !solidLayer, tileCache, ref position, ref liquidSize, i, num, ref vertices);////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
						flag7 = true;
						break;
					}
				}
			}
			VertexColors colors = vertices;
			float num7 = (flag7 ? Main.liquidAlpha[num2] : 1f);
			ref Color bottomLeftColor2 = ref colors.BottomLeftColor;
			bottomLeftColor2 *= num7;
			ref Color bottomRightColor2 = ref colors.BottomRightColor;
			bottomRightColor2 *= num7;
			ref Color topLeftColor2 = ref colors.TopLeftColor;
			topLeftColor2 *= num7;
			ref Color topRightColor2 = ref colors.TopRightColor;
			topRightColor2 *= num7;
			if (num2 == 14)
			{
				LiquidRenderer.SetShimmerVertexColors(ref colors, solidLayer ? 0.75f : 1f, tileX, tileY);
			}
			DrawPartialLiquid(self, !solidLayer, tileCache, ref position, ref liquidSize, num2, num, ref colors);////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		}

		private static void DrawPartialLiquid(TileDrawing self, bool behindBlocks, Tile tileCache, ref Vector2 position, ref Rectangle liquidSize, int watersType, int liquidType, ref VertexColors colors)
		{
			int x = tileCache.X();
			int y = tileCache.Y();
			if (LiquidLoader.PreSlopeDraw(x, y, liquidType, behindBlocks, ref position, ref liquidSize, ref colors))
			{
				SlopeType num = tileCache.Slope;
				bool flag = !TileID.Sets.BlocksWaterDrawingBehindSelf[tileCache.TileType];
				if (!behindBlocks)
				{
					flag = false;
				}
				if (flag || num == 0)
				{
					Main.tileBatch.Draw(liquidType < LiquidID.Count ? TextureAssets.Liquid[watersType].Value : LiquidLoader.LiquidBlockAssets[liquidType].Value, position, liquidSize, colors, default(Vector2), 1f, (SpriteEffects)0);
					return;
				}
				liquidSize.X += 18 * ((int)num - 1);
				switch ((int)num)
				{
					case 1:
						Main.tileBatch.Draw(liquidType < LiquidID.Count ? TextureAssets.LiquidSlope[watersType].Value : LiquidLoader.LiquidSlopeAssets[liquidType].Value, position, liquidSize, colors, Vector2.Zero, 1f, (SpriteEffects)0);
						break;
					case 2:
						Main.tileBatch.Draw(liquidType < LiquidID.Count ? TextureAssets.LiquidSlope[watersType].Value : LiquidLoader.LiquidSlopeAssets[liquidType].Value, position, liquidSize, colors, Vector2.Zero, 1f, (SpriteEffects)0);
						break;
					case 3:
						Main.tileBatch.Draw(liquidType < LiquidID.Count ? TextureAssets.LiquidSlope[watersType].Value : LiquidLoader.LiquidSlopeAssets[liquidType].Value, position, liquidSize, colors, Vector2.Zero, 1f, (SpriteEffects)0);
						break;
					case 4:
						Main.tileBatch.Draw(liquidType < LiquidID.Count ? TextureAssets.LiquidSlope[watersType].Value : LiquidLoader.LiquidSlopeAssets[liquidType].Value, position, liquidSize, colors, Vector2.Zero, 1f, (SpriteEffects)0);
						break;
				}
				LiquidLoader.PostSlopeDraw(x, y, liquidType, behindBlocks, ref position, ref liquidSize, ref colors);
			}
		}
	}
}
