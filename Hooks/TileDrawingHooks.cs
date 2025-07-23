using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.CodeDom;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.Graphics;
using Terraria.ID;

namespace ModLiquidLib.Hooks
{
	internal class TileDrawingHooks
	{
		internal static void EditSlopeLiquidRendering(ILContext il)
		{
			ILCursor c = new(il);
			VariableDefinition liquidType_varDef = new(il.Import(typeof(int)));
			il.Body.Variables.Add(liquidType_varDef);

			//commended numbers are the local variable IDs as of 14/7/2025
			int tile_varNum = -1;//0
			int tile2_varNum = -1;//1
			int tile3_varNum = -1;//2
			int tile4_varNum = -1;//3
			int num_varNum = -1; //4
			int flag_varNum = -1;//5
			int flag2_varNum = -1;//6
			int flag3_varNum = -1;//7
			int flag4_varNum = -1;//8
			int flag5_varNum = -1;//9
			int num2_varNum = -1;//10
			int vertecies_varNum = -1;//14
			int liquidSize_varNum = -1;//16
			int position_varNum = -1;//17
			int liquidAlpha_varNum = -1; //18
			int colors_varNum = -1;//20
			c.GotoNext(i => i.MatchAdd(), i => i.MatchLdarg(7), i => i.MatchCall<Tilemap>("get_Item"), i => i.MatchStloc(out tile_varNum));
			c.GotoNext(i => i.MatchSub(), i => i.MatchLdarg(7), i => i.MatchCall<Tilemap>("get_Item"), i => i.MatchStloc(out tile2_varNum));
			c.GotoNext(i => i.MatchSub(), i => i.MatchCall<Tilemap>("get_Item"), i => i.MatchStloc(out tile3_varNum));
			c.GotoNext(i => i.MatchAdd(), i => i.MatchCall<Tilemap>("get_Item"), i => i.MatchStloc(out tile4_varNum));
			c.GotoNext(i => i.MatchRet(), i => i.MatchLdcI4(0), i => i.MatchStloc(out num_varNum),
										  i => i.MatchLdcI4(0), i => i.MatchStloc(out flag_varNum),
										  i => i.MatchLdcI4(0), i => i.MatchStloc(out flag2_varNum),
										  i => i.MatchLdcI4(0), i => i.MatchStloc(out flag3_varNum),
										  i => i.MatchLdcI4(0), i => i.MatchStloc(out flag4_varNum),
										  i => i.MatchLdcI4(0), i => i.MatchStloc(out flag5_varNum),
										  i => i.MatchLdcI4(0), i => i.MatchStloc(out num2_varNum));
			c.GotoNext(i => i.MatchLdloca(out vertecies_varNum), i => i.MatchLdcR4(1), i => i.MatchCall<Lighting>("GetCornerColors"));
			c.GotoNext(i => i.MatchLdloca(out liquidSize_varNum), i => i.MatchLdcI4(0), i => i.MatchLdcI4(4), i => i.MatchLdcI4(16), i => i.MatchLdcI4(16), i => i.MatchCall<Rectangle>(".ctor"));
			c.GotoNext(i => i.MatchLdloc(out _), i => i.MatchLdarg(4), i => i.MatchCall<Vector2>("op_Subtraction"), i => i.MatchLdarg(5), i => i.MatchCall<Vector2>("op_Addition"), i => i.MatchStloc(out position_varNum));
			c.GotoNext(i => i.MatchLdloc(vertecies_varNum), i => i.MatchStloc(out colors_varNum));
			c.Index = 0;

			c.GotoNext(MoveType.After, i => i.MatchLdcI4(0), i => i.MatchStloc(num_varNum));
			c.EmitLdcI4(0);
			c.Emit(OpCodes.Stloc, liquidType_varDef); //create a local var
			c.GotoNext(MoveType.After, i => i.MatchLdarga(8), i => i.MatchCall<Tile>("get_liquid"), i => i.MatchLdindU1(), i => i.MatchStloc(num_varNum));
			c.Emit(OpCodes.Ldloca, liquidType_varDef);
			c.EmitLdarg(8);
			c.EmitDelegate((ref int i, Tile tileCache) =>
			{
				i = tileCache.LiquidType; //set the local var to liquid type at switch points chosing a liquid to render
			});
			c.GotoNext(MoveType.After, i => i.MatchLdcI4(1), i => i.MatchStloc(flag5_varNum));
			c.Emit(OpCodes.Ldloca, liquidType_varDef);
			c.EmitLdarg(8);
			c.EmitDelegate((ref int i, Tile tileCache) =>
			{
				i = tileCache.LiquidType;
			});
			c.GotoNext(MoveType.After, i => i.MatchLdcI4(1), i => i.MatchStloc(flag_varNum));
			c.Emit(OpCodes.Ldloca, liquidType_varDef);
			c.EmitLdloc(tile2_varNum);
			c.EmitDelegate((ref int i, Tile tile2) =>
			{
				i = tile2.LiquidType;
			});
			c.GotoNext(MoveType.After, i => i.MatchLdcI4(1), i => i.MatchStloc(flag2_varNum));
			c.Emit(OpCodes.Ldloca, liquidType_varDef);
			c.EmitLdloc(tile_varNum);
			c.EmitDelegate((ref int i, Tile tile) =>
			{
				i = tile.LiquidType;
			});
			c.GotoNext(MoveType.After, i => i.MatchLdcI4(1), i => i.MatchStloc(flag3_varNum));
			c.Emit(OpCodes.Ldloca, liquidType_varDef);
			c.EmitLdloc(tile3_varNum);
			c.EmitDelegate((ref int i, Tile tile3) =>
			{
				i = tile3.LiquidType;
			});
			c.GotoNext(MoveType.After, i => i.MatchLdcI4(1), i => i.MatchStloc(flag4_varNum));
			c.Emit(OpCodes.Ldloca, liquidType_varDef);
			c.EmitLdloc(tile4_varNum);
			c.EmitDelegate((ref int i, Tile tile4) =>
			{
				i = tile4.LiquidType;
			});
			c.GotoNext(MoveType.After, i => i.MatchLdarg(5), i => i.MatchCall<Vector2>("op_Addition"), i => i.MatchStloc(position_varNum));
			c.EmitLdloca(num_varNum);
			c.Emit(OpCodes.Ldloc, liquidType_varDef);
			c.EmitDelegate((ref int num, int i) =>
			{
				num = i; //put local var into another local var
			});

			//Alpha editing
			//float num7 = 0.5f;
			//switch (num2)
			//{
			//	case 1:
			//		num7 = 1f;
			//		break;
			//	case 11:
			//		num7 = Math.Max(num7 * 1.7f, 1f);
			//		break;
			//}
			//    <- Inject right here
			//if ((double)tileY <= Main.worldSurface || num7 > 1f)
			c.GotoNext(MoveType.After, i => i.MatchLdloc(out liquidAlpha_varNum), i => i.MatchLdcR4(1.7f), i => i.MatchMul(), i => i.MatchLdcR4(1), i => i.MatchCall("System.Math", "Max"), 
				i => i.MatchStloc(liquidAlpha_varNum), i => i.MatchLdarg(7));
			c.EmitLdloca(liquidAlpha_varNum);
			c.Emit(OpCodes.Ldloc, liquidType_varDef);
			c.EmitDelegate((int tileY, ref float num7, int i) =>
			{
				LiquidLoader.LiquidSlopeOpacity(i, ref num7);
			});
			c.EmitLdarg(7);

			//Ok, back to slope rendering stuff
			c.GotoNext(MoveType.After, i => i.MatchLdloca(vertecies_varNum), i => i.MatchCall<TileDrawing>("DrawPartialLiquid"));
			c.EmitLdarg(0);
			c.EmitLdarg(1);
			c.EmitLdarg(8);
			c.EmitLdloca(position_varNum);
			c.EmitLdloca(liquidSize_varNum);
			c.Emit(OpCodes.Ldloc, liquidType_varDef);
			c.EmitLdloc(num_varNum);
			c.EmitLdloca(vertecies_varNum);
			c.EmitDelegate((TileDrawing self, bool solidLayer, Tile tileCache, ref Vector2 position, ref Rectangle liquidSize, int i, int num, ref VertexColors vertices) =>
			{
				DrawPartialLiquid(self, !solidLayer, tileCache, ref position, ref liquidSize, i, num, ref vertices);
			});
			c.GotoNext(MoveType.After, i => i.MatchLdloca(20), i => i.MatchCall<TileDrawing>("DrawPartialLiquid"));
			c.EmitLdarg(0);
			c.EmitLdarg(1);
			c.EmitLdarg(8);
			c.EmitLdloca(position_varNum);
			c.EmitLdloca(liquidSize_varNum);
			c.EmitLdloc(num2_varNum);
			c.EmitLdloc(num_varNum);
			c.EmitLdloca(colors_varNum);
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

		public static void DrawPartialLiquid(TileDrawing self, bool behindBlocks, Tile tileCache, ref Vector2 position, ref Rectangle liquidSize, int watersType, int liquidType, ref VertexColors colors)
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
