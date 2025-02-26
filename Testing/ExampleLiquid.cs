using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;
using Terraria;
using Terraria.GameContent.Liquid;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModLiquidLib.Testing
{
	internal class ExampleLiquid : ModLiquid
	{
		public override string Texture => "ModLiquidLib/Quicksilver";

		public override void SetStaticDefaults()
		{
			LiquidFallLength = 3;
			DefaultOpacity = 0.95f;
			VisualViscosity = 200;
			AddMapEntry(new Color(255, 0, 255), CreateMapEntryName());
			AddMapEntry(new Color(0, 255, 255), CreateMapEntryName());
			AddMapEntry(new Color(255, 255, 0), CreateMapEntryName());
		}

		public override ushort GetMapOption(int i, int j)
		{
			return (ushort)((i + j) % 3);
		}

		public override bool PreDraw(int i, int j, LiquidDrawCache liquidDrawCache, Vector2 drawOffset, bool isBackgroundDraw)
		{
			
			return true;
		}

		public override bool PreSlopeDraw(int i, int j, bool behindBlocks, ref Vector2 drawPosition, ref Rectangle liquidSize, ref VertexColors colors)
		{
			return true;
		}

		public override bool PreRetroDraw(int i, int j, SpriteBatch spriteBatch)
		{
			return true;
		}

		public override void RetroDrawEffects(int i, int j, SpriteBatch spriteBatch, ref RetroLiquidDrawInfo drawData, float liquidAmountModified, int liquidGFXQuality)
		{
			float num9 = drawData.liquidAlphaMultiplier;
			num9 *= 1.8f;
			if (num9 > 1f)
			{
				num9 = 1f;
			}
			drawData.liquidAlphaMultiplier = num9;
			drawData.liquidColor = Lighting.GetColor(i, j, Main.DiscoColor);
			drawData.waterfallLength = 12;
			drawData.waterfallStrength = 0.25f;
			//drawData.liquidFraming = new(0, 2, drawData.liquidFraming.Width, drawData.liquidFraming.Height);
			//drawData.liquidPositionOffset += new Vector2(16, 16);
		}

		/*public override void EmitEffects(int i, int j, LiquidCache liquidCache)
		{
			return;
			if (liquidCache.HasVisibleLiquid)
			{
				if (Main.rand.Next(700) == 0)
				{
					Dust.NewDust(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16, DustID.Lava, 0f, 0f, 0, Color.White);
				}
				if (Main.rand.Next(350) == 0)
				{
					int num20 = Dust.NewDust(new Vector2((float)(i * 16), (float)(j * 16)), 16, 8, DustID.Lava, 0f, 0f, 50, Color.White, 1.5f);
					Dust obj = Main.dust[num20];
					obj.velocity *= 0.8f;
					Main.dust[num20].velocity.X *= 2f;
					Main.dust[num20].velocity.Y -= (float)Main.rand.Next(1, 7) * 0.1f;
					if (Main.rand.Next(10) == 0)
					{
						Main.dust[num20].velocity.Y *= Main.rand.Next(2, 5);
					}
					Main.dust[num20].noGravity = true;
				}
			}
		}*/

		public override void PostRetroDraw(int i, int j, SpriteBatch spriteBatch)
		{
			return;
			Vector2 vector2 = new Vector2((float)(i * 16), (float)(j * 16 + 16));
			Vector2 vector = new((float)Main.offScreenRange, (float)Main.offScreenRange);
			if (Main.drawToScreen)
			{
				vector = Vector2.Zero;
			}
			spriteBatch.Draw(LiquidLoader.LiquidBlockAssets[Type].Value, vector2 - Main.screenPosition + vector, (Rectangle?)new Rectangle(0, 4, 16, 8), Color.Black, 0f, default(Vector2), 1f, (SpriteEffects)0, 0f);
		}

		/*public override bool PreDraw(int i, int j, LiquidDrawCache liquidDrawCache, Vector2 drawOffset, bool isBackgroundDraw)
		{
			Rectangle sourceRectangle = liquidDrawCache.SourceRectangle;
			if (liquidDrawCache.IsSurfaceLiquid)
			{
				sourceRectangle.Y = 1280;
			}
			else
			{
				sourceRectangle.Y += LiquidRenderer.Instance._animationFrame * 80;
			}
			Vector2 liquidOffset = liquidDrawCache.LiquidOffset;
			float val = liquidDrawCache.Opacity * (isBackgroundDraw ? 1f : 0.75f);
			int num2 = 14;
			val = Math.Min(1f, val);
			int num3 = i;
			int num4 = j;
			Lighting.GetCornerColors(num3, num4, out var vertices);
			LiquidRenderer.SetShimmerVertexColors(ref vertices, val, num3, num4);
			Main.DrawTileInWater(drawOffset, num3, num4);
			Main.tileBatch.Draw(LiquidRenderer.Instance._liquidTextures[num2].Value, new Vector2((float)(num3 << 4), (float)(num4 << 4)) + drawOffset + liquidOffset, sourceRectangle, vertices, Vector2.Zero, 1f, (SpriteEffects)0);
			sourceRectangle = liquidDrawCache.SourceRectangle;
			bool flag = sourceRectangle.X != 16 || sourceRectangle.Y % 80 != 48;
			if (flag || (num3 + num4) % 2 == 0)
			{
				sourceRectangle.X += 48;
				sourceRectangle.Y += 80 * LiquidRenderer.Instance.GetShimmerFrame(flag, num3, num4);
				LiquidRenderer.SetShimmerVertexColors_Sparkle(ref vertices, liquidDrawCache.Opacity, num3, num4, flag);
				Main.tileBatch.Draw(LiquidRenderer.Instance._liquidTextures[num2].Value, new Vector2((float)(num3 << 4), (float)(num4 << 4)) + drawOffset + liquidOffset, sourceRectangle, vertices, Vector2.Zero, 1f, (SpriteEffects)0);
			}

			Rectangle sourceRectangle = liquidDrawCache.SourceRectangle;
			if (liquidDrawCache.IsSurfaceLiquid)
			{
				sourceRectangle.Y = 1280;
			}
			else
			{
				sourceRectangle.Y += LiquidRenderer.Instance._animationFrame * 80;
			}
			Vector2 liquidOffset = liquidDrawCache.LiquidOffset;
			float num = liquidDrawCache.Opacity * (isBackgroundDraw ? 1f : LiquidRenderer.DEFAULT_OPACITY[liquidDrawCache.Type]);
			int num2 = 14;
			num = Math.Min(1f, num);
			Lighting.GetCornerColors(i, j, out var vertices);
			ref Color bottomLeftColor = ref vertices.BottomLeftColor;
			bottomLeftColor *= num;
			ref Color bottomRightColor = ref vertices.BottomRightColor;
			bottomRightColor *= num;
			ref Color topLeftColor = ref vertices.TopLeftColor;
			topLeftColor *= num;
			ref Color topRightColor = ref vertices.TopRightColor;
			topRightColor *= num;
			Main.DrawTileInWater(drawOffset, i, j);
			Main.tileBatch.Draw(LiquidRenderer.Instance._liquidTextures[num2].Value, new Vector2((float)(i << 4), (float)(j << 4)) + drawOffset + liquidOffset, sourceRectangle, vertices, Vector2.Zero, 1f, (SpriteEffects)0);
			return true;
		}*/

		/*public override void PostDraw(int i, int j, LiquidDrawCache liquidDrawCache, Vector2 drawOffset, bool isBackgroundDraw)
		{
			Rectangle sourceRectangle = liquidDrawCache.SourceRectangle;
			if (liquidDrawCache.IsSurfaceLiquid)
			{
				sourceRectangle.Y = 1280;
			}
			else
			{
				sourceRectangle.Y += LiquidRenderer.Instance._animationFrame * 80;
			}
			Vector2 liquidOffset = liquidDrawCache.LiquidOffset;
			float val = liquidDrawCache.Opacity * (isBackgroundDraw ? 1f : 0.75f) * 0.5f;
			int num2 = 14;
			val = Math.Min(1f, val);
			int num3 = i;
			int num4 = j;
			Lighting.GetCornerColors(num3, num4, out var vertices);
			LiquidRenderer.SetShimmerVertexColors(ref vertices, val, num3, num4);
			Main.DrawTileInWater(drawOffset, num3, num4);
			Main.tileBatch.Draw(LiquidRenderer.Instance._liquidTextures[num2].Value, new Vector2((float)(num3 << 4), (float)(num4 << 4)) + drawOffset + liquidOffset, sourceRectangle, vertices, Vector2.Zero, 1f, (SpriteEffects)0);
			sourceRectangle = liquidDrawCache.SourceRectangle;
			bool flag = sourceRectangle.X != 16 || sourceRectangle.Y % 80 != 48;
			if (flag || (num3 + num4) % 2 == 0)
			{
				sourceRectangle.X += 48;
				sourceRectangle.Y += 80 * LiquidRenderer.Instance.GetShimmerFrame(flag, num3, num4);
				LiquidRenderer.SetShimmerVertexColors_Sparkle(ref vertices, liquidDrawCache.Opacity, num3, num4, flag);
				Main.tileBatch.Draw(LiquidRenderer.Instance._liquidTextures[num2].Value, new Vector2((float)(num3 << 4), (float)(num4 << 4)) + drawOffset + liquidOffset, sourceRectangle, vertices, Vector2.Zero, 1f, (SpriteEffects)0);
			}
		}*/

		/*public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 2.55f;
			g = 2.55f;
			b = 2.55f;
		}*/

		public override int ChooseWaterfallStyle(int i, int j)
		{
			return ModContent.GetInstance<ExampleWaterfallStyle>().Slot;
		}
	}
}
