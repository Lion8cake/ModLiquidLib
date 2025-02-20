using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;
using MonoMod.Cil;
using ReLogic.Content;
using Steamworks;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Liquid;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using static log4net.Appender.ColoredConsoleAppender;

namespace ModLiquidLib.Hooks
{
	internal class TileDrawingHooks
	{
		internal static void EditSlopeLiquidRendering(ILContext il)
		{
			ILCursor c = new(il);
			c.GotoNext(MoveType.After, i => i.MatchLdcI4(0), i => i.MatchStloc(4));
			c.EmitLdcI4(0);
			c.EmitStloc(26); //create a local var
			c.GotoNext(MoveType.After, i => i.MatchLdarga(8), i => i.MatchCall<Tile>("get_liquid"), i => i.MatchLdindU1(), i => i.MatchStloc(4));
			c.EmitLdloca(26);
			c.EmitLdarg(8);
			c.EmitDelegate((ref int i, Tile tileCache) =>
			{
				i = tileCache.LiquidType; //set the local var to liquid type at switch points chosing a liquid to render
			});
			c.GotoNext(MoveType.After, i => i.MatchLdcI4(1), i => i.MatchStloc(9));
			c.EmitLdloca(26);
			c.EmitLdarg(8);
			c.EmitDelegate((ref int i, Tile tileCache) =>
			{
				i = tileCache.LiquidType;
			});
			c.GotoNext(MoveType.After, i => i.MatchLdcI4(1), i => i.MatchStloc(5));
			c.EmitLdloca(26);
			c.EmitLdloc(1);
			c.EmitDelegate((ref int i, Tile tile2) =>
			{
				i = tile2.LiquidType;
			});
			c.GotoNext(MoveType.After, i => i.MatchLdcI4(1), i => i.MatchStloc(6));
			c.EmitLdloca(26);
			c.EmitLdloc(0);
			c.EmitDelegate((ref int i, Tile tile) =>
			{
				i = tile.LiquidType;
			});
			c.GotoNext(MoveType.After, i => i.MatchLdcI4(1), i => i.MatchStloc(7));
			c.EmitLdloca(26);
			c.EmitLdloc(2);
			c.EmitDelegate((ref int i, Tile tile3) =>
			{
				i = tile3.LiquidType;
			});
			c.GotoNext(MoveType.After, i => i.MatchLdcI4(1), i => i.MatchStloc(8));
			c.EmitLdloca(26);
			c.EmitLdloc(3);
			c.EmitDelegate((ref int i, Tile tile4) =>
			{
				i = tile4.LiquidType;
			});
			c.GotoNext(MoveType.After, i => i.MatchLdarg(5), i => i.MatchCall<Vector2>("op_Addition"), i => i.MatchStloc(17));
			c.EmitLdloca(4);
			c.EmitLdloc(26);
			c.EmitDelegate((ref int num, int i) =>
			{
				num = i; //put local var into another local var
			});
			c.GotoNext(MoveType.After, i => i.MatchLdloca(14), i => i.MatchCall<TileDrawing>("DrawPartialLiquid"));
			c.EmitLdarg0();
			c.EmitLdarg1();
			c.EmitLdarg(8);
			c.EmitLdloca(17);
			c.EmitLdloca(16);
			c.EmitLdloc(26);
			c.EmitLdloc(4);
			c.EmitLdloca(14);
			c.EmitDelegate((TileDrawing self, bool solidLayer, Tile tileCache, ref Vector2 position, ref Rectangle liquidSize, int i, int num, ref VertexColors vertices) =>
			{
				DrawPartialLiquid(self, !solidLayer, tileCache, ref position, ref liquidSize, i, num, ref vertices);
			});
			c.GotoNext(MoveType.After, i => i.MatchLdloca(20), i => i.MatchCall<TileDrawing>("DrawPartialLiquid"));
			c.EmitLdarg0();
			c.EmitLdarg1();
			c.EmitLdarg(8);
			c.EmitLdloca(17);
			c.EmitLdloca(16);
			c.EmitLdloc(10);
			c.EmitLdloc(4);
			c.EmitLdloca(20);
			c.EmitDelegate((TileDrawing self, bool solidLayer, Tile tileCache, ref Vector2 position, ref Rectangle liquidSize, int num2, int num, ref VertexColors colors) =>
			{
				DrawPartialLiquid(self, !solidLayer, tileCache, ref position, ref liquidSize, num2, num, ref colors);
			});
		}

		internal static void BlockOldParticalLiquidRendering(On_TileDrawing.orig_DrawPartialLiquid orig, TileDrawing self, bool behindBlocks, Tile tileCache, ref Vector2 position, ref Rectangle liquidSize, int liquidType, ref Terraria.Graphics.VertexColors colors)
		{
			return;
			orig.Invoke(self, behindBlocks, tileCache, ref position, ref liquidSize, liquidType, ref colors);
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
