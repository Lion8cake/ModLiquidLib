using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using ModLiquidLib.ModLoader;
using System;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Liquid;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.WaterfallManager;

namespace ModLiquidLib.Testing
{
    public class ExampleWaterfallStyle : ModLiquidFall
    {
		public override string Texture => "ModLiquidLib/Quicksilver_Silverfall";

		public override void PostDraw(int i, int x, int y, SpriteBatch spriteBatch)
		{
			float num = 0f;
			float num12 = 99999f;
			float num23 = 99999f;
			int num34 = -1;
			int num45 = -1;
			float num47 = 0f;
			float num48 = 99999f;
			float num49 = 99999f;
			int num50 = -1;
			int num2 = -1;


			int num3 = 0;
			int num4 = waterfalls[i].type;
			int num5 = waterfalls[i].x;
			int num6 = waterfalls[i].y;
			int num7 = 0;
			int num8 = 0;
			int num9 = 0;
			int num10 = 0;
			int num11 = 0;
			int num13 = 0;
			int num14;
			int num15;
		
			if (Main.drewLava || waterfalls[i].stopAtStep == 0)
			{
				return;
			}

			num14 = 32 * Main.instance.waterfallManager.slowFrame;
			int num22 = 0;
			num15 = waterfallDist;
			Color color4 = Color.White;
			for (int k = 0; k < num15; k++)
			{
				if (num22 >= 2)
				{
					break;
				}
				WaterfallManager.AddLight(num4, num5, num6);
				Tile tile3 = Main.tile[num5, num6];
				if (tile3.HasUnactuatedTile && Main.tileSolid[tile3.TileType] && !Main.tileSolidTop[tile3.TileType] && !TileID.Sets.Platforms[tile3.TileType] && tile3.BlockType == 0)
				{
					break;
				}
				Tile tile4 = Main.tile[num5 - 1, num6];
				Tile tile5 = Main.tile[num5, num6 + 1];
				Tile tile6 = Main.tile[num5 + 1, num6];
				if (WorldGen.SolidTile(tile5) && !tile3.IsHalfBlock)
				{
					num3 = 8;
				}
				else if (num8 != 0)
				{
					num3 = 0;
				}
				int num24 = 0;
				int num25 = num10;
				int num26 = 0;
				int num27 = 0;
				bool flag2 = false;
				if (tile5.TopSlope && !tile3.IsHalfBlock && tile5.TileType != 19)
				{
					flag2 = true;
					if (tile5.Slope == (SlopeType)1)
					{
						num24 = 1;
						num26 = 1;
						num9 = 1;
						num10 = num9;
					}
					else
					{
						num24 = -1;
						num26 = -1;
						num9 = -1;
						num10 = num9;
					}
					num27 = 1;
				}
				else if ((!WorldGen.SolidTile(tile5) && !tile5.BottomSlope && !tile3.IsHalfBlock) || (!tile5.HasTile && !tile3.IsHalfBlock))
				{
					num22 = 0;
					num27 = 1;
					num26 = 0;
				}
				else if ((WorldGen.SolidTile(tile4) || tile4.TopSlope || tile4.LiquidAmount > 0) && !WorldGen.SolidTile(tile6) && tile6.LiquidAmount == 0)
				{
					if (num9 == -1)
					{
						num22++;
					}
					num26 = 1;
					num27 = 0;
					num9 = 1;
				}
				else if ((WorldGen.SolidTile(tile6) || tile6.TopSlope || tile6.LiquidAmount > 0) && !WorldGen.SolidTile(tile4) && tile4.LiquidAmount == 0)
				{
					if (num9 == 1)
					{
						num22++;
					}
					num26 = -1;
					num27 = 0;
					num9 = -1;
				}
				else if (((!WorldGen.SolidTile(tile6) && !tile3.TopSlope) || tile6.LiquidAmount == 0) && !WorldGen.SolidTile(tile4) && !tile3.TopSlope && tile4.LiquidAmount == 0)
				{
					num27 = 0;
					num26 = num9;
				}
				else
				{
					num22++;
					num27 = 0;
					num26 = 0;
				}
				if (num22 >= 2)
				{
					num9 *= -1;
					num26 *= -1;
				}
				int num28 = -1;
				if (num4 != 1 && num4 != 14 && num4 != 25)
				{
					if (tile5.HasTile)
					{
						num28 = tile5.TileType;
					}
					if (tile3.HasTile)
					{
						num28 = tile3.TileType;
					}
				}
				switch (num28)
				{
					case 160:
						num4 = 2;
						break;
					case 262:
					case 263:
					case 264:
					case 265:
					case 266:
					case 267:
					case 268:
						num4 = 15 + num28 - 262;
						break;
				}
				if (num28 != -1)
				{
					TileLoader.ChangeWaterfallStyle(num28, ref num4);
				}
				Color color5 = Lighting.GetColor(num5, num6);
				if (k > 50)
				{
					WaterfallManager.TrySparkling(num5, num6, num9, color5);
				}
				float alpha = WaterfallManager.GetAlpha(1f, num15, num4, num6, k, tile3);
				color5 = WaterfallManager.StylizeColor(alpha, num15, num4, num6, k, tile3, color5);
				if (num4 == 1)
				{
					float num29 = Math.Abs((float)(num5 * 16 + 8) - (Main.screenPosition.X + (float)(Main.screenWidth / 2)));
					float num30 = Math.Abs((float)(num6 * 16 + 8) - (Main.screenPosition.Y + (float)(Main.screenHeight / 2)));
					if (num29 < (float)(Main.screenWidth * 2) && num30 < (float)(Main.screenHeight * 2))
					{
						float num31 = (float)Math.Sqrt(num29 * num29 + num30 * num30);
						float num32 = 1f - num31 / ((float)Main.screenWidth * 0.75f);
						if (num32 > 0f)
						{
							num47 += num32;
						}
					}
					if (num29 < num48)
					{
						num48 = num29;
						num50 = num5 * 16 + 8;
					}
					if (num30 < num49)
					{
						num49 = num29;
						num2 = num6 * 16 + 8;
					}
				}
				else if (num4 != 1 && num4 != 14 && num4 != 25 && num4 != 11 && num4 != 12 && num4 != 22)
				{
					float num33 = Math.Abs((float)(num5 * 16 + 8) - (Main.screenPosition.X + (float)(Main.screenWidth / 2)));
					float num35 = Math.Abs((float)(num6 * 16 + 8) - (Main.screenPosition.Y + (float)(Main.screenHeight / 2)));
					if (num33 < (float)(Main.screenWidth * 2) && num35 < (float)(Main.screenHeight * 2))
					{
						float num36 = (float)Math.Sqrt(num33 * num33 + num35 * num35);
						float num37 = 1f - num36 / ((float)Main.screenWidth * 0.75f);
						if (num37 > 0f)
						{
							num += num37;
						}
					}
					if (num33 < num12)
					{
						num12 = num33;
						num34 = num5 * 16 + 8;
					}
					if (num35 < num23)
					{
						num23 = num33;
						num45 = num6 * 16 + 8;
					}
				}
				int num38 = tile3.LiquidAmount / 16;
				if (flag2 && num9 != num25)
				{
					int num39 = 2;
					if (num25 == 1)
					{
						DrawWaterfall(num4, num5, num6, alpha, new Vector2((float)(num5 * 16 - 16), (float)(num6 * 16 + 16 - num39)) - Main.screenPosition, new Rectangle(num14, 24, 32, 16 - num38 - num39), color5, (SpriteEffects)1);
					}
					else
					{
						DrawWaterfall(num4, num5, num6, alpha, new Vector2((float)(num5 * 16), (float)(num6 * 16 + 16 - num39)) - Main.screenPosition, new Rectangle(num14, 24, 32, 16 - num38 - num39), color5, (SpriteEffects)0);
					}
				}
				if (num7 == 0 && num24 != 0 && num8 == 1 && num9 != num10)
				{
					num24 = 0;
					num9 = num10;
					color5 = Color.White;
					if (num9 == 1)
					{
						DrawWaterfall(num4, num5, num6, alpha, new Vector2((float)(num5 * 16 - 16), (float)(num6 * 16 + 16)) - Main.screenPosition, new Rectangle(num14, 24, 32, 16 - num38), color5, (SpriteEffects)1);
					}
					else
					{
						DrawWaterfall(num4, num5, num6, alpha, new Vector2((float)(num5 * 16 - 16), (float)(num6 * 16 + 16)) - Main.screenPosition, new Rectangle(num14, 24, 32, 16 - num38), color5, (SpriteEffects)1);
					}
				}
				if (num11 != 0 && num26 == 0 && num27 == 1)
				{
					if (num9 == 1)
					{
						if (num13 != num4)
						{
							DrawWaterfall(num4, num5, num6, alpha, new Vector2((float)(num5 * 16), (float)(num6 * 16 + num3 + 8)) - Main.screenPosition, new Rectangle(num14, 0, 16, 16 - num38 - 8), color4, (SpriteEffects)1);
						}
						else
						{
							DrawWaterfall(num4, num5, num6, alpha, new Vector2((float)(num5 * 16), (float)(num6 * 16 + num3 + 8)) - Main.screenPosition, new Rectangle(num14, 0, 16, 16 - num38 - 8), color5, (SpriteEffects)1);
						}
					}
					else
					{
						DrawWaterfall(num4, num5, num6, alpha, new Vector2((float)(num5 * 16), (float)(num6 * 16 + num3 + 8)) - Main.screenPosition, new Rectangle(num14, 0, 16, 16 - num38 - 8), color5, (SpriteEffects)0);
					}
				}
				if (num3 == 8 && num8 == 1 && num11 == 0)
				{
					if (num10 == -1)
					{
						if (num13 != num4)
						{
							DrawWaterfall(num13, num5, num6, alpha, new Vector2((float)(num5 * 16), (float)(num6 * 16)) - Main.screenPosition, new Rectangle(num14, 24, 32, 8), color4, (SpriteEffects)0);
						}
						else
						{
							DrawWaterfall(num4, num5, num6, alpha, new Vector2((float)(num5 * 16), (float)(num6 * 16)) - Main.screenPosition, new Rectangle(num14, 24, 32, 8), color5, (SpriteEffects)0);
						}
					}
					else if (num13 != num4)
					{
						DrawWaterfall(num13, num5, num6, alpha, new Vector2((float)(num5 * 16 - 16), (float)(num6 * 16)) - Main.screenPosition, new Rectangle(num14, 24, 32, 8), color4, (SpriteEffects)1);
					}
					else
					{
						DrawWaterfall(num4, num5, num6, alpha, new Vector2((float)(num5 * 16 - 16), (float)(num6 * 16)) - Main.screenPosition, new Rectangle(num14, 24, 32, 8), color5, (SpriteEffects)1);
					}
				}
				if (num24 != 0 && num7 == 0)
				{
					if (num25 == 1)
					{
						if (num13 != num4)
						{
							DrawWaterfall(num13, num5, num6, alpha, new Vector2((float)(num5 * 16 - 16), (float)(num6 * 16)) - Main.screenPosition, new Rectangle(num14, 24, 32, 16 - num38), color4, (SpriteEffects)1);
						}
						else
						{
							DrawWaterfall(num4, num5, num6, alpha, new Vector2((float)(num5 * 16 - 16), (float)(num6 * 16)) - Main.screenPosition, new Rectangle(num14, 24, 32, 16 - num38), color5, (SpriteEffects)1);
						}
					}
					else if (num13 != num4)
					{
						DrawWaterfall(num13, num5, num6, alpha, new Vector2((float)(num5 * 16), (float)(num6 * 16)) - Main.screenPosition, new Rectangle(num14, 24, 32, 16 - num38), color4, (SpriteEffects)0);
					}
					else
					{
						DrawWaterfall(num4, num5, num6, alpha, new Vector2((float)(num5 * 16), (float)(num6 * 16)) - Main.screenPosition, new Rectangle(num14, 24, 32, 16 - num38), color5, (SpriteEffects)0);
					}
				}
				if (num27 == 1 && num24 == 0 && num11 == 0)
				{
					if (num9 == -1)
					{
						if (num8 == 0)
						{
							DrawWaterfall(num4, num5, num6, alpha, new Vector2((float)(num5 * 16), (float)(num6 * 16 + num3)) - Main.screenPosition, new Rectangle(num14, 0, 16, 16 - num38), color5, (SpriteEffects)0);
						}
						else if (num13 != num4)
						{
							DrawWaterfall(num13, num5, num6, alpha, new Vector2((float)(num5 * 16), (float)(num6 * 16)) - Main.screenPosition, new Rectangle(num14, 24, 32, 16 - num38), color4, (SpriteEffects)0);
						}
						else
						{
							DrawWaterfall(num4, num5, num6, alpha, new Vector2((float)(num5 * 16), (float)(num6 * 16)) - Main.screenPosition, new Rectangle(num14, 24, 32, 16 - num38), color5, (SpriteEffects)0);
						}
					}
					else if (num8 == 0)
					{
						DrawWaterfall(num4, num5, num6, alpha, new Vector2((float)(num5 * 16), (float)(num6 * 16 + num3)) - Main.screenPosition, new Rectangle(num14, 0, 16, 16 - num38), color5, (SpriteEffects)1);
					}
					else if (num13 != num4)
					{
						DrawWaterfall(num13, num5, num6, alpha, new Vector2((float)(num5 * 16 - 16), (float)(num6 * 16)) - Main.screenPosition, new Rectangle(num14, 24, 32, 16 - num38), color4, (SpriteEffects)1);
					}
					else
					{
						DrawWaterfall(num4, num5, num6, alpha, new Vector2((float)(num5 * 16 - 16), (float)(num6 * 16)) - Main.screenPosition, new Rectangle(num14, 24, 32, 16 - num38), color5, (SpriteEffects)1);
					}
				}
				else
				{
					switch (num26)
					{
						case 1:
							if (Main.tile[num5, num6].LiquidAmount > 0 && !Main.tile[num5, num6].IsHalfBlock)
							{
								break;
							}
							if (num24 == 1)
							{
								for (int m = 0; m < 8; m++)
								{
									int num43 = m * 2;
									int num44 = 14 - m * 2;
									int num46 = num43;
									num3 = 8;
									if (num7 == 0 && m < 2)
									{
										num46 = 4;
									}
									DrawWaterfall(num4, num5, num6, alpha, new Vector2((float)(num5 * 16 + num43), (float)(num6 * 16 + num3 + num46)) - Main.screenPosition, new Rectangle(16 + num14 + num44, 0, 2, 16 - num3), color5, (SpriteEffects)1);
								}
							}
							else
							{
								int height2 = 16;
								if (TileID.Sets.BlocksWaterDrawingBehindSelf[Main.tile[num5, num6].TileType])
								{
									height2 = 8;
								}
								else if (TileID.Sets.BlocksWaterDrawingBehindSelf[Main.tile[num5, num6 + 1].TileType])
								{
									height2 = 8;
								}
								DrawWaterfall(num4, num5, num6, alpha, new Vector2((float)(num5 * 16), (float)(num6 * 16 + num3)) - Main.screenPosition, new Rectangle(16 + num14, 0, 16, height2), color5, (SpriteEffects)1);
							}
							break;
						case -1:
							if (Main.tile[num5, num6].LiquidAmount > 0 && !Main.tile[num5, num6].IsHalfBlock)
							{
								break;
							}
							if (num24 == -1)
							{
								for (int l = 0; l < 8; l++)
								{
									int num40 = l * 2;
									int num41 = l * 2;
									int num42 = 14 - l * 2;
									num3 = 8;
									if (num7 == 0 && l > 5)
									{
										num42 = 4;
									}
									DrawWaterfall(num4, num5, num6, alpha, new Vector2((float)(num5 * 16 + num40), (float)(num6 * 16 + num3 + num42)) - Main.screenPosition, new Rectangle(16 + num14 + num41, 0, 2, 16 - num3), color5, (SpriteEffects)1);
								}
							}
							else
							{
								int height = 16;
								if (TileID.Sets.BlocksWaterDrawingBehindSelf[Main.tile[num5, num6].TileType])
								{
									height = 8;
								}
								else if (TileID.Sets.BlocksWaterDrawingBehindSelf[Main.tile[num5, num6 + 1].TileType])
								{
									height = 8;
								}
								DrawWaterfall(num4, num5, num6, alpha, new Vector2((float)(num5 * 16), (float)(num6 * 16 + num3)) - Main.screenPosition, new Rectangle(16 + num14, 0, 16, height), color5, (SpriteEffects)0);
							}
							break;
						case 0:
							if (num27 == 0)
							{
								if (Main.tile[num5, num6].LiquidAmount <= 0 || Main.tile[num5, num6].IsHalfBlock)
								{
									DrawWaterfall(num4, num5, num6, alpha, new Vector2((float)(num5 * 16), (float)(num6 * 16 + num3)) - Main.screenPosition, new Rectangle(16 + num14, 0, 16, 16), color5, (SpriteEffects)0);
								}
								k = 1000;
							}
							break;
					}
				}
				if (tile3.LiquidAmount > 0 && !tile3.IsHalfBlock)
				{
					k = 1000;
				}
				num8 = num27;
				num10 = num9;
				num7 = num26;
				num5 += num26;
				num6 += num27;
				num11 = num24;
				color4 = color5;
				if (num13 != num4)
				{
					num13 = num4;
				}
				if ((tile4.HasTile && (tile4.TileType == 189 || tile4.TileType == 196)) || (tile6.HasTile && (tile6.TileType == 189 || tile6.TileType == 196)) || (tile5.HasTile && (tile5.TileType == 189 || tile5.TileType == 196)))
				{
					num15 = (int)(40f * ((float)Main.maxTilesX / 4200f) * Main.gfxQuality);
				}
			}
		}

		private void DrawWaterfall(int waterfallType, int x, int y, float opacity, Vector2 position, Rectangle sourceRect, Color color, SpriteEffects effects)
		{
			Texture2D value = ModContent.Request<Texture2D>("ModLiquidLib/Testing/FreezingWaterfallStyle").Value;
			if (waterfallType == 25)
			{
				Lighting.GetCornerColors(x, y, out var vertices);
				LiquidRenderer.SetShimmerVertexColors(ref vertices, opacity, x, y);
				Main.tileBatch.Draw(value, position + new Vector2(0f, 0f), sourceRect, vertices, default(Vector2), 1f, effects);
				sourceRect.Y += 42;
				LiquidRenderer.SetShimmerVertexColors_Sparkle(ref vertices, opacity, x, y, top: true);
				Main.tileBatch.Draw(value, position + new Vector2(0f, 0f), sourceRect, vertices, default(Vector2), 1f, effects);
			}
			else
			{
				Main.spriteBatch.Draw(value, position, (Rectangle?)sourceRect, color, 0f, default(Vector2), 1f, effects, 0f);
			}
		}

		public override float? Alpha(int x, int y, float Alpha, int maxSteps, int s, Tile tileCache)
		{
			return 1f;
		}

		public override void ColorMultiplier(ref float r, ref float g, ref float b, float a)
		{
			if (r < 190f * a)
			{
				r = 190f * a;
			}
			if (g < 190f * a)
			{
				g = 190f * a;
			}
			if (b < 190f * a)
			{
				b = 190f * a;
			}
		}
	}
}