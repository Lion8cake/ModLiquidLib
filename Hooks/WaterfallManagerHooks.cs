using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;
using MonoMod.Cil;
using System;
using System.Linq.Expressions;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using static Terraria.WaterfallManager;

namespace ModLiquidLib.Hooks
{
	internal class WaterfallManagerHooks
	{
		internal static void SemiFixforWaterfallLighting(On_WaterfallManager.orig_AddLight orig, int waterfallType, int x, int y)
		{
			if (waterfallType >= LoaderManager.Get<WaterFallStylesLoader>().TotalCount)
			{
				return;
			}
			if (waterfallType >= 26)
			{
				LoaderManager.Get<WaterFallStylesLoader>().Get(waterfallType).AddLight(x, y);
			}
			orig.Invoke(waterfallType, x, y);
		}

		internal static void AnimateModWaterfall(On_WaterfallManager.orig_UpdateFrame orig, WaterfallManager self)
		{
			int totalCount = LoaderManager.Get<WaterFallStylesLoader>().TotalCount;
			if (totalCount > MainHooks.wFallFrame.Length)
			{
				Array.Resize(ref MainHooks.wFallFrame, totalCount);
				Array.Resize(ref MainHooks.wFallFrameBack, totalCount);
				Array.Resize(ref MainHooks.wFallFrameCounter, totalCount);
			}
			for (int i = ID.WaterfallID.Count; i < MainHooks.wFallFrame.Length; i++)
			{
				if (LoaderManager.Get<WaterFallStylesLoader>().Get(i) is ModLiquidFall)
				{
					ModLiquidFall fall = (ModLiquidFall)LoaderManager.Get<WaterFallStylesLoader>().Get(i);
					if (LoaderUtils.HasOverride(fall, (Expression<Func<ModLiquidFall, LiquidFallLoader.DelegateAnimateWaterfall>>)((ModLiquidFall t) => t.AnimateWaterfall)))
					{
						LiquidFallLoader.AnimateWaterfall(i);
						continue;
					}
				}
				MainHooks.wFallFrameCounter[i]++;
				if (MainHooks.wFallFrameCounter[i] > 2)
				{
					MainHooks.wFallFrameCounter[i] = 0;
					MainHooks.wFallFrame[i]++;
					if (MainHooks.wFallFrame[i] > 15)
					{
						MainHooks.wFallFrame[i] = 0;
					}
				}
			}
			orig.Invoke(self);
		}

		internal static void EditWaterfallStyle(ILContext il)
		{
			ILCursor c = new(il);
			int x_varNum = -1;
			int y_varNum = -1;
			c.GotoNext(MoveType.After, i => i.MatchLdloc(out x_varNum), i => i.MatchStfld<WaterfallManager.WaterfallData>("x"));
			c.GotoNext(MoveType.After, i => i.MatchLdloc(out y_varNum), i => i.MatchStfld<WaterfallManager.WaterfallData>("y"));
			c.EmitLdloc(x_varNum);
			c.EmitLdloc(y_varNum);
			c.EmitLdarg(0);
			c.EmitDelegate((int i, int j, WaterfallManager self) =>
			{
				for (int k = 0; k < LiquidLoader.LiquidCount; k++)
				{
					if (Main.tile[i, j - 1].LiquidType == k || Main.tile[i + 1, j].LiquidType == k || Main.tile[i - 1, j].LiquidType == k)
					{
						int? waterfallStyle = LiquidLoader.DrawWaterfall(i, j, k);
						if (waterfallStyle != null)
						{
							self.waterfalls[self.currentMax].type = (int)waterfallStyle;
						}
					}
				}
			});
			c.GotoNext(MoveType.Before, i => i.MatchLdcI4(1), i => i.MatchAdd(), i => i.MatchStloc(y_varNum));
			c.EmitLdloc(x_varNum);
			c.EmitLdarg(0);
			c.EmitDelegate((int j, int i, WaterfallManager self) =>
			{
				if (TileLoader.GetTile(Main.tile[i, j].TileType) is ILiquidModTile liquidTile)
				{
					WaterfallData? data = liquidTile.CreateWaterfall(i, j);
					Tile tile6 = Main.tile[i, j + 1];
					if (data != null && self.currentMax < self.qualityMax)
					{
						self.waterfalls[self.currentMax] = (WaterfallData)data;
						self.currentMax++;
					}
				}
			});
			c.EmitLdloc(y_varNum);
		}

		internal static void PreDrawWaterfallModifier(ILContext il)
		{
			ILCursor c = new(il);
			ILLabel IL_149d = null;
			ILLabel IL_0a27 = null;
			int waterfallData_numVar = -1;
			int waterfallType_numVar = -1;
			int x_numVar = -1;
			int y_numVar = -1;
			//Get some variables used, this is so that if extra variables are added by tmodloader, that this IL edit will still work
			//basically, magic number prevention making the edit less likely to break
			c.GotoNext(i => i.MatchLdloc(out waterfallData_numVar), i => i.MatchLdelema<WaterfallManager.WaterfallData>(), i => i.MatchLdfld<WaterfallManager.WaterfallData>("x"), i => i.MatchStloc(out x_numVar));
			c.GotoNext(i => i.MatchLdfld<WaterfallManager.WaterfallData>("y"), i => i.MatchStloc(out y_numVar));
			c.Index = 0;

			c.GotoNext( //Gets the IL_149d instruction lable
				MoveType.After,
				i => i.MatchLdloc(out waterfallType_numVar),//if (Main.drewLava || waterfalls[i].stopAtStep == 0)
				i => i.MatchLdcI4(25),						//{
				i => i.MatchBneUn(out _),					//	   continue;
				i => i.MatchLdsfld<Main>("drewLava"),		//}
				i => i.MatchBrtrue(out IL_149d)); //used to get the ILLable from the continue
			c.GotoPrev( //Goes to after the intialisation of variables 3 through to 15. This is just before the drawing of lava, honey and shimmer waterfalls
				MoveType.Before,
				i => i.MatchLdloc(waterfallType_numVar), //int num11 = 0;
				i => i.MatchLdcI4(1),					 //int num13 = 0;
				i => i.MatchBeq(out _),					 //int num14;
				i => i.MatchLdloc(waterfallType_numVar), //int num15;
				i => i.MatchLdcI4(14),					 // <- inject here
				i => i.MatchBeq(out _));                 //if (num12 == 1 || num12 == 14 || num12 == 25)
			c.EmitLdarg(0); //self, used for getting the current instance
			c.EmitLdloc(waterfallData_numVar); //the current waterfall data type
			c.EmitLdloc(waterfallType_numVar); //Type of waterfall
			c.EmitLdloc(x_numVar); //X position of the waterfall
			c.EmitLdloc(y_numVar); //Y position of the waterfall
			c.EmitDelegate((WaterfallManager self, int i, int num4, int num5, int num6) => 
			{
				//if (!PreDraw(i, num5, num6, num4, Main.spriteBatch))
				return !LiquidFallLoader.PreDraw(self.waterfalls[i], num5, num6, num4, Main.spriteBatch); //{
			});																			 //		continue;
			c.EmitBrtrue(IL_149d);                                                       //}

			c.GotoNext(MoveType.After, i => i.MatchLdfld<WaterfallManager>("regularFrame"));
			c.EmitLdloc(waterfallType_numVar);
			c.EmitDelegate((int regularFrame, int num4) =>
			{
				if (num4 >= ID.WaterfallID.Count && num4 < MainHooks.wFallFrame.Length)
				{
					return MainHooks.wFallFrame[num4];
				}
				return regularFrame;
			});

			c.GotoNext(MoveType.After, i => i.MatchLdloc(waterfallType_numVar), i => i.MatchLdcI4(12), i => i.MatchBeq(out _), i => i.MatchLdloc(waterfallType_numVar), i => i.MatchLdcI4(22), i => i.MatchBeq(out IL_0a27));
			c.EmitLdloc(waterfallType_numVar);
			c.EmitDelegate((int style) =>
			{
				if (ModContent.GetModWaterfallStyle(style) is ModLiquidFall)
				{
					ModLiquidFall modLiquidFall = (ModLiquidFall)(ModContent.GetModWaterfallStyle(style));
					if (modLiquidFall != null)
						return modLiquidFall.PlayWaterfallSounds();
				}
				return true;
			});
			c.EmitBrfalse(IL_0a27);

			c.GotoNext(MoveType.Before, i => i.MatchLdcI4(1), i => i.MatchAdd(), i => i.MatchStloc(waterfallData_numVar));
			c.EmitLdarg(0); //waterfall data instance
			c.EmitLdloc(waterfallType_numVar); //Type of waterfall
			c.EmitLdloc(x_numVar); //X position of the waterfall
			c.EmitLdloc(y_numVar); //Y position of the waterfall
			c.EmitDelegate((int i, WaterfallManager self, int num4, int num5, int num6) =>
			{
				LiquidFallLoader.PostDraw(self.waterfalls[i], num5, num6, num4, Main.spriteBatch);
			});
			c.EmitLdloc(waterfallData_numVar); //re-emit this instruction, as the one before this injection is being used by the previous delegate
		}

		internal static void editWaterfallAlpha(ILContext il)
		{
			ILCursor c = new(il);
			int num_varNum = -1; //used only to make sure then next instruction is of the same type
			c.GotoNext(MoveType.After, i => i.MatchLdcR4(10), i => i.MatchDiv(), i => i.MatchMul(), i => i.MatchStloc(out num_varNum), i => i.MatchLdloc(num_varNum));
			c.EmitLdarg(0);
			c.EmitLdarg(1);
			c.EmitLdarg(2);
			c.EmitLdarg(3);
			c.EmitLdarg(4);
			c.EmitLdarg(5);
			c.EmitDelegate((float num, float Alpha, int maxSteps, int waterfallType, int y, int s, Tile tileCache) =>
			{
				float? alpha = LiquidFallLoader.Alpha(tileCache.X(), y, waterfallType, Alpha, maxSteps, s, tileCache);
				if (alpha != null)
				{
					return (float)alpha;
				}
				return num;
			});
		}
	}
}
