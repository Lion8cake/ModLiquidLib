using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;
using MonoMod.Cil;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Liquid;
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

		private static Texture2D replace(Texture2D texture, int x, int y)
		{
			int type = Main.tile[x, y].LiquidType;
			return type < LiquidID.Count ? texture : LiquidLoader.LiquidBlockAssets[type].Value;
		}

		internal static void On_Main_oldDrawWater(On_Main.orig_oldDrawWater orig, Main self, bool bg, int Style, float Alpha)
		{
			float num = 0f;
			float num12 = 99999f;
			float num23 = 99999f;
			int num27 = -1;
			int num28 = -1;
			Vector2 vector = new((float)Main.offScreenRange, (float)Main.offScreenRange);
			if (Main.drawToScreen)
			{
				vector = Vector2.Zero;
			}
			//_ = new Color[4];
			int num29 = (int)(255f * (1f - Main.gfxQuality) + 40f * Main.gfxQuality);
			//_ = Main.gfxQuality;
			//_ = Main.gfxQuality;
			int num30 = (int)((Main.screenPosition.X - vector.X) / 16f - 1f);
			int num31 = (int)((Main.screenPosition.X + (float)Main.screenWidth + vector.X) / 16f) + 2;
			int num32 = (int)((Main.screenPosition.Y - vector.Y) / 16f - 1f);
			int num2 = (int)((Main.screenPosition.Y + (float)Main.screenHeight + vector.Y) / 16f) + 5;
			if (num30 < 5)
			{
				num30 = 5;
			}
			if (num31 > Main.maxTilesX - 5)
			{
				num31 = Main.maxTilesX - 5;
			}
			if (num32 < 5)
			{
				num32 = 5;
			}
			if (num2 > Main.maxTilesY - 5)
			{
				num2 = Main.maxTilesY - 5;
			}
			Vector2 vector2 = default(Vector2);
			Rectangle value = default(Rectangle);
			Color newColor = default(Color);
			for (int i = num32; i < num2 + 4; i++)
			{
				for (int j = num30 - 2; j < num31 + 2; j++)
				{
					if (Main.tile[j, i].LiquidAmount <= 0 || (Main.tile[j, i].HasUnactuatedTile && Main.tileSolid[Main.tile[j, i].TileType] && !Main.tileSolidTop[Main.tile[j, i].TileType]) || !(Lighting.Brightness(j, i) > 0f || bg))
					{
						continue;
					}
					Color color = Lighting.GetColor(j, i);
					if (!LiquidLoader.PreRetroDraw(j, i, Main.tile[j, i].LiquidType, Main.spriteBatch)) ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
					{
						continue;
					}
					float num3 = 256 - Main.tile[j, i].LiquidAmount;
					num3 /= 32f;
					bool flag = false;
					int num4 = 0;
					if (Main.tile[j, i].LiquidType == LiquidID.Lava)
					{
						if (Main.drewLava)
						{
							continue;
						}
						float num5 = Math.Abs((float)(j * 16 + 8) - (Main.screenPosition.X + (float)(Main.screenWidth / 2)));
						float num6 = Math.Abs((float)(i * 16 + 8) - (Main.screenPosition.Y + (float)(Main.screenHeight / 2)));
						if (num5 < (float)(Main.screenWidth * 2) && num6 < (float)(Main.screenHeight * 2))
						{
							float num7 = (float)Math.Sqrt(num5 * num5 + num6 * num6);
							float num8 = 1f - num7 / ((float)Main.screenWidth * 0.75f);
							if (num8 > 0f)
							{
								num += num8;
							}
						}
						if (num5 < num12)
						{
							num12 = num5;
							num27 = j * 16 + 8;
						}
						if (num6 < num23)
						{
							num23 = num5;
							num28 = i * 16 + 8;
						}
						num4 = 1;
					}
					else if (Main.tile[j, i].LiquidType == LiquidID.Honey)
					{
						num4 = 11;
					}
					else if (Main.tile[j, i].LiquidType == LiquidID.Shimmer)
					{
						num4 = 14;
						flag = true;
					}
					if (num4 == 0)
					{
						num4 = Style;
					}
					if ((num4 == 1 || num4 == 11) && Main.drewLava)
					{
						continue;
					}
					float num9 = 0.5f;
					if (bg)
					{
						num9 = 1f;
					}
					if (num4 != 1 && num4 != 11)
					{
						num9 *= Alpha;
					}
					Main.DrawTileInWater(-Main.screenPosition + vector, j, i);
					vector2 = new((float)(j * 16), (float)(i * 16 + (int)num3 * 2));
					value = new(0, 0, 16, 16 - (int)num3 * 2);
					bool flag2 = true;
					if (Main.tile[j, i + 1].LiquidAmount < 245 && (!Main.tile[j, i + 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[j, i + 1].TileType] || Main.tileSolidTop[Main.tile[j, i + 1].TileType]))
					{
						float num10 = 256 - Main.tile[j, i + 1].LiquidAmount;
						num10 /= 32f;
						num9 = 0.5f * (8f - num3) / 4f;
						if ((double)num9 > 0.55)
						{
							num9 = 0.55f;
						}
						if ((double)num9 < 0.35)
						{
							num9 = 0.35f;
						}
						float num11 = num3 / 2f;
						if (Main.tile[j, i + 1].LiquidAmount < 200)
						{
							if (bg)
							{
								continue;
							}
							if (Main.tile[j, i - 1].LiquidAmount > 0 && Main.tile[j, i - 1].LiquidAmount > 0)
							{
								value = new(0, 4, 16, 16);
								num9 = 0.5f;
							}
							else if (Main.tile[j, i - 1].LiquidAmount > 0)
							{
								vector2 = new((float)(j * 16), (float)(i * 16 + 4));
								value = new(0, 4, 16, 12);
								num9 = 0.5f;
							}
							else if (Main.tile[j, i + 1].LiquidAmount > 0)
							{
								vector2 = new((float)(j * 16), (float)(i * 16 + (int)num3 * 2 + (int)num10 * 2));
								value = new(0, 4, 16, 16 - (int)num3 * 2);
							}
							else
							{
								vector2 = new((float)(j * 16 + (int)num11), (float)(i * 16 + (int)num11 * 2 + (int)num10 * 2));
								value = new(0, 4, 16 - (int)num11 * 2, 16 - (int)num11 * 2);
							}
						}
						else
						{
							num9 = 0.5f;
							value = new(0, 4, 16, 16 - (int)num3 * 2 + (int)num10 * 2);
						}
					}
					else if (Main.tile[j, i - 1].LiquidAmount > 32)
					{
						value = new(0, 4, value.Width, value.Height);
					}
					else if (num3 < 1f && Main.tile[j, i - 1].HasUnactuatedTile && Main.tileSolid[Main.tile[j, i - 1].TileType] && !Main.tileSolidTop[Main.tile[j, i - 1].TileType])
					{
						vector2 = new((float)(j * 16), (float)(i * 16));
						value = new(0, 4, 16, 16);
					}
					else
					{
						for (int k = i + 1; k < i + 6 && (!Main.tile[j, k].HasUnactuatedTile || !Main.tileSolid[Main.tile[j, k].TileType] || Main.tileSolidTop[Main.tile[j, k].TileType]); k++)
						{
							if (Main.tile[j, k].LiquidAmount < 200)
							{
								flag2 = false;
								break;
							}
						}
						if (!flag2)
						{
							num9 = 0.5f;
							value = new(0, 4, 16, 16);
						}
						else if (Main.tile[j, i - 1].LiquidAmount > 0)
						{
							value = new(0, 2, value.Width, value.Height);
						}
					}
					if ((color.R > 20 || color.B > 20 || color.G > 20) && value.Y < 4)
					{
						int num13 = color.R;
						if (color.G > num13)
						{
							num13 = color.G;
						}
						if (color.B > num13)
						{
							num13 = color.B;
						}
						num13 /= 30;
						if (Main.rand.Next(20000) < num13)
						{
							newColor = new(255, 255, 255);
							if (Main.tile[j, i].LiquidType == LiquidID.Honey)
							{
								newColor = new(255, 255, 50);
							}
							int num14 = Dust.NewDust(new Vector2((float)(j * 16), vector2.Y - 2f), 16, 8, DustID.TintableDustLighted, 0f, 0f, 254, newColor, 0.75f);
							Dust obj = Main.dust[num14];
							obj.velocity *= 0f;
						}
					}
					if (Main.tile[j, i].LiquidType == LiquidID.Honey)
					{
						num9 *= 1.6f;
						if (num9 > 1f)
						{
							num9 = 1f;
						}
					}
					if (Main.tile[j, i].LiquidType == LiquidID.Lava)
					{
						num9 *= 1.8f;
						if (num9 > 1f)
						{
							num9 = 1f;
						}
						if (self.IsActive && !Main.gamePaused && Dust.lavaBubbles < 200)
						{
							if (Main.tile[j, i].LiquidAmount > 200 && Main.rand.Next(700) == 0)
							{
								Dust.NewDust(new Vector2((float)(j * 16), (float)(i * 16)), 16, 16, 35);
							}
							if (value.Y == 0 && Main.rand.NextBool(350))
							{
								int num15 = Dust.NewDust(new Vector2((float)(j * 16), (float)(i * 16) + num3 * 2f - 8f), 16, 8, 35, 0f, 0f, 50, default(Color), 1.5f);
								Dust obj2 = Main.dust[num15];
								obj2.velocity *= 0.8f;
								Main.dust[num15].velocity.X *= 2f;
								Main.dust[num15].velocity.Y -= (float)Main.rand.Next(1, 7) * 0.1f;
								if (Main.rand.Next(10) == 0)
								{
									Main.dust[num15].velocity.Y *= Main.rand.Next(2, 5);
								}
								Main.dust[num15].noGravity = true;
							}
						}
					}
					//this line is useless minus that it gives the IL a good ledge to inject, its reasigned later
					float num16 = (float)(int)color.R * num9;
					newColor = Color.Transparent;///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
					RetroLiquidDrawInfo info = new RetroLiquidDrawInfo();
					info.tileCache = Main.tile[j, i];
					info.liquidAlphaMultiplier = num9;
					info.liquidColor = newColor;
					info.liquidFraming = value;
					info.liquidScreenPos = vector2;
					info.liquidPositionOffset = vector;
					LiquidLoader.RetroDrawEffects(j, i, Main.tile[j, i].LiquidType, Main.spriteBatch, ref info, num3, num29);
					num9 = info.liquidAlphaMultiplier;
					newColor = info.liquidColor;
					value = info.liquidFraming;
					vector2 = info.liquidScreenPos;
					vector = info.liquidPositionOffset;///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
													   //DrawEffects here
													   //num9 alpha
													   //color Color(?) <- use newColor here to save the color by draweffects
													   //value framing
													   //num3 LiquidAmountModified (not ref)
													   //vector2 liquid positioning
													   //num29 quality amount (not ref)
													   //vector offscreenrange
					if (newColor != Color.Transparent)
					{
						color = newColor;
					}
					num16 = (float)(int)color.R * num9;///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
					float num17 = (float)(int)color.G * num9;
					float num18 = (float)(int)color.B * num9;
					float num19 = (float)(int)color.A * num9;
					color = new((int)(byte)num16, (int)(byte)num17, (int)(byte)num18, (int)(byte)num19);
					if (flag)
					{
						color = new(color.ToVector4() * LiquidRenderer.GetShimmerBaseColor(j, i));
					}
					if (Lighting.NotRetro && !bg)
					{
						Color color2 = color;
						if (num4 != 1 && ((double)(int)color2.R > (double)num29 * 0.6 || (double)(int)color2.G > (double)num29 * 0.65 || (double)(int)color2.B > (double)num29 * 0.7))
						{
							for (int l = 0; l < 4; l++)
							{
								int num20 = 0;
								int num21 = 0;
								int width = 8;
								int height = 8;
								Color color3 = color2;
								Color color4 = Lighting.GetColor(j, i);
								if (newColor != Color.Transparent)///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
								{
									color4 = newColor;
								}
								if (l == 0)
								{
									color4 = Lighting.GetColor(j - 1, i - 1);
									if (newColor != Color.Transparent)///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
									{
										color4 = newColor;
									}
									if (value.Height < 8)
									{
										height = value.Height;
									}
								}
								if (l == 1)
								{
									color4 = Lighting.GetColor(j + 1, i - 1);
									if (newColor != Color.Transparent)///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
									{
										color4 = newColor;
									}
									num20 = 8;
									if (value.Height < 8)
									{
										height = value.Height;
									}
								}
								if (l == 2)
								{
									color4 = Lighting.GetColor(j - 1, i + 1);
									if (newColor != Color.Transparent)///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
									{
										color4 = newColor;
									}
									num21 = 8;
									height = 8 - (16 - value.Height);
								}
								if (l == 3)
								{
									color4 = Lighting.GetColor(j + 1, i + 1);
									if (newColor != Color.Transparent)///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
									{
										color4 = newColor;
									}
									num20 = 8;
									num21 = 8;
									height = 8 - (16 - value.Height);
								}
								num16 = (float)(int)color4.R * num9;
								num17 = (float)(int)color4.G * num9;
								num18 = (float)(int)color4.B * num9;
								num19 = (float)(int)color4.A * num9;
								color4 = new((int)(byte)num16, (int)(byte)num17, (int)(byte)num18, (int)(byte)num19);
								color3.R = (byte)((color2.R * 3 + color4.R * 2) / 5);
								color3.G = (byte)((color2.G * 3 + color4.G * 2) / 5);
								color3.B = (byte)((color2.B * 3 + color4.B * 2) / 5);
								color3.A = (byte)((color2.A * 3 + color4.A * 2) / 5);
								if (flag)
								{
									color3 = new(color3.ToVector4() * LiquidRenderer.GetShimmerBaseColor(j, i));
								}
								Main.spriteBatch.Draw(replace(TextureAssets.Liquid[num4].Value, j, i), vector2 - Main.screenPosition + new Vector2((float)num20, (float)num21) + vector, (Rectangle?)new Rectangle(value.X + num20, value.Y + num21, width, height), color3, 0f, default(Vector2), 1f, (SpriteEffects)0, 0f);
								if (flag)
								{
									Main.spriteBatch.Draw(replace(TextureAssets.Liquid[num4].Value, j, i), vector2 - Main.screenPosition + new Vector2((float)num20, (float)num21) + vector, (Rectangle?)new Rectangle(value.X + num20, value.Y + num21 + 36, width, height), LiquidRenderer.GetShimmerGlitterColor(flag2, j, i), 0f, default(Vector2), 1f, (SpriteEffects)0, 0f);
								}
							}
						}
						else
						{
							Main.spriteBatch.Draw(replace(TextureAssets.Liquid[num4].Value, j, i), vector2 - Main.screenPosition + vector, (Rectangle?)value, color, 0f, default(Vector2), 1f, (SpriteEffects)0, 0f);
							if (flag)
							{
								value.Y += 36;
								Main.spriteBatch.Draw(replace(TextureAssets.Liquid[num4].Value, j, i), vector2 - Main.screenPosition + vector, (Rectangle?)value, LiquidRenderer.GetShimmerGlitterColor(flag2, j, i), 0f, default(Vector2), 1f, (SpriteEffects)0, 0f);
							}
						}
					}
					else
					{
						if (value.Y < 4)
						{
							value.X += (int)(Main.wFrame * 18f);
						}
						Main.spriteBatch.Draw(replace(TextureAssets.Liquid[num4].Value, j, i), vector2 - Main.screenPosition + vector, (Rectangle?)value, color, 0f, default(Vector2), 1f, (SpriteEffects)0, 0f);
						if (flag)
						{
							value.Y += 36;
							Main.spriteBatch.Draw(replace(TextureAssets.Liquid[num4].Value, j, i), vector2 - Main.screenPosition + vector, (Rectangle?)value, LiquidRenderer.GetShimmerGlitterColor(flag2, j, i), 0f, default(Vector2), 1f, (SpriteEffects)0, 0f);
						}
					}
					if (!Main.tile[j, i + 1].IsHalfBlock)
					{
						continue;
					}
					color = Lighting.GetColor(j, i + 1);
					if (newColor != Color.Transparent)///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
					{
						color = newColor;
					}
					num16 = (float)(int)color.R * num9;
					num17 = (float)(int)color.G * num9;
					num18 = (float)(int)color.B * num9;
					num19 = (float)(int)color.A * num9;
					color = new((int)(byte)num16, (int)(byte)num17, (int)(byte)num18, (int)(byte)num19);
					if (flag)
					{
						color = new(color.ToVector4() * LiquidRenderer.GetShimmerBaseColor(j, i));
					}
					vector2 = new((float)(j * 16), (float)(i * 16 + 16));
					Main.spriteBatch.Draw(replace(TextureAssets.Liquid[num4].Value, j, i), vector2 - Main.screenPosition + vector, (Rectangle?)new Rectangle(0, 4, 16, 8), color, 0f, default(Vector2), 1f, (SpriteEffects)0, 0f);
					if (flag)
					{
						Main.spriteBatch.Draw(replace(TextureAssets.Liquid[num4].Value, j, i), vector2 - Main.screenPosition + vector, (Rectangle?)new Rectangle(0, 40, 16, 8), LiquidRenderer.GetShimmerGlitterColor(flag2, j, i), 0f, default(Vector2), 1f, (SpriteEffects)0, 0f);
					}
					float num22 = 6f;
					float num24 = 0.75f;
					if (num4 == 1 || num4 == 11)
					{
						num22 = 4f;
						num24 = 0.5f;
					}
					for (int m = 0; (float)m < num22; m++)
					{
						int num25 = i + 2 + m;
						if (WorldGen.SolidTile(j, num25))
						{
							break;
						}
						float num26 = 1f - (float)m / num22;
						num26 *= num24;
						vector2 = new((float)(j * 16), (float)(num25 * 16 - 2));
						Main.spriteBatch.Draw(replace(TextureAssets.Liquid[num4].Value, j, i), vector2 - Main.screenPosition + vector, (Rectangle?)new Rectangle(0, 18, 16, 16), color * num26, 0f, default(Vector2), 1f, (SpriteEffects)0, 0f);
						if (flag)
						{
							Main.spriteBatch.Draw(replace(TextureAssets.Liquid[num4].Value, j, i), vector2 - Main.screenPosition + vector, (Rectangle?)new Rectangle(0, 54, 16, 16), LiquidRenderer.GetShimmerGlitterColor(flag2, j, i) * num26, 0f, default(Vector2), 1f, (SpriteEffects)0, 0f);
						}
					}
					LiquidLoader.PostRetroDraw(j, i, Main.tile[j, i].LiquidType, Main.spriteBatch);///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
				}
			}
			if (!Main.drewLava)
			{
				Main.ambientLavaX = num27;
				Main.ambientLavaY = num28;
				Main.ambientLavaStrength = num;
			}
			Main.drewLava = true;
		}
	}
}
