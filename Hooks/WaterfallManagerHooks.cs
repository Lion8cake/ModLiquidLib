using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace ModLiquidLib.Hooks
{
	internal class WaterfallManagerHooks
	{
		internal static void EditWaterfallStyle(ILContext il)
		{
			ILCursor c = new(il);
			c.GotoNext(MoveType.After, i => i.MatchLdloc(5), i => i.MatchStfld<WaterfallManager.WaterfallData>("y"));
			c.EmitLdloc(4);
			c.EmitLdloc(5);
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
		}

		internal static void PreDrawWaterfallModifier(ILContext il)
		{
			ILCursor c = new(il);
			ILLabel IL_149d = null;
			c.GotoNext( //Gets the IL_149d instruction lable
				MoveType.After,
				i => i.MatchLdloc(12),               //if (Main.drewLava || waterfalls[i].stopAtStep == 0)
				i => i.MatchLdcI4(25),               //{
				i => i.MatchBneUn(out _),            //	   continue;
				i => i.MatchLdsfld<Main>("drewLava"),//}
				i => i.MatchBrtrue(out IL_149d)); //used to get the ILLable from the continue
			c.GotoPrev( //Goes to after the intialisation of variables 3 through to 15. This is just before the drawing of lava, honey and shimmer waterfalls
				MoveType.After,
				i => i.MatchLdcI4(0),  //int num11 = 0;
				i => i.MatchStloc(18), //int num13 = 0;
				i => i.MatchLdcI4(0),  //int num14;
				i => i.MatchStloc(19), //int num15;
				i => i.MatchLdcI4(0),
				i => i.MatchStloc(20));
			c.EmitLdloc(10); //the current waterfall data type
			c.EmitLdloc(12); //Type of waterfall
			c.EmitLdloc(13); //X position of the waterfall
			c.EmitLdloc(14); //Y position of the waterfall
			c.EmitDelegate((int i, int num4, int num5, int num6) => {               //if (!PreDraw(i, num5, num6, num4, Main.spriteBatch))
				return !LiquidFallLoader.PreDraw(i, num5, num6, num4, Main.spriteBatch); //{
			});                                                                     //	continue;
			c.EmitBrtrue(IL_149d);                                                  //}
			c.GotoNext(MoveType.Before, i => i.MatchLdcI4(1), i => i.MatchAdd(), i => i.MatchStloc(10));
			c.EmitLdloc(12); //Type of waterfall
			c.EmitLdloc(13); //X position of the waterfall
			c.EmitLdloc(14); //Y position of the waterfall
			c.EmitDelegate((int i, int num4, int num5, int num6) =>
			{
				LiquidFallLoader.PostDraw(i, num5, num6, num4, Main.spriteBatch);
			});
			c.EmitLdloc(10);
		}

		internal static void editWaterfallAlpha(ILContext il)
		{
			ILCursor c = new(il);
			c.GotoNext(MoveType.After, i => i.MatchDiv(), i => i.MatchMul(), i => i.MatchStloc(0), i => i.MatchLdloc(0));
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
