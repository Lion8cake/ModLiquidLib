using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;
using System;
using Terraria;
using Terraria.GameContent.Liquid;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.Graphics.Capture.IL_CaptureBiome.Sets;

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
			AddMapEntry(new Color(5, 5, 7));
			//AddMapEntry(new Color(255, 0, 255));
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
	}
}
