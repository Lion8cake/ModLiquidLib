using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModLiquidLib.ModLoader;
using MonoMod.Cil;
using ReLogic.Content;
using System;
using System.Runtime.CompilerServices;
using Terraria.GameContent.Liquid;
using Terraria.Graphics;
using Terraria.ID;
using static Terraria.GameContent.Liquid.LiquidRenderer;

namespace ModLiquidLib.Hooks
{
	internal class LiquidRendererHooks
	{
		internal unsafe static void EditLiquidRendering(ILContext il)
		{
			ILCursor c = new(il);
			int x_varNum = -1;
			int y_varNum = -1;
			int pointer2_varNum = -1;
			int miscWatersType_varNum = -1;

			c.GotoNext(MoveType.After, i => i.MatchStloc(out x_varNum), i => i.MatchBr(out _), i => i.MatchLdloc(out _), i => i.MatchLdfld<Rectangle>("Y"), i => i.MatchStloc(out y_varNum));
			c.GotoNext(MoveType.After, i => i.MatchLdloc(out pointer2_varNum), i => i.MatchLdfld<LiquidDrawCache>("IsVisible"));
			c.EmitLdloc(x_varNum);
			c.EmitLdloc(y_varNum);
			c.EmitLdloc(pointer2_varNum);
			c.EmitLdarg(2);
			c.EmitLdarg(3);
			c.EmitLdarg(4);
			c.EmitLdarg(5);
			c.EmitDelegate((bool isVisibleCondition, int i, int j, IntPtr ptr2, Vector2 drawOffset, int waterStyle, float globalAlpha, bool isBackgroundDraw) =>
			{
				return isVisibleCondition && LiquidLoader.PreDraw(
					i, 
					j, 
					Unsafe.AsRef<LiquidDrawCache>((void*)ptr2).Type, 
					Unsafe.As<LiquidDrawCache, Utils.Structs.LiquidDrawCache>(ref Unsafe.AsRef<LiquidDrawCache>((void*)ptr2)), 
					drawOffset, 
					isBackgroundDraw,
					waterStyle,
					globalAlpha);
			});
			c.GotoNext(MoveType.After,
				i => i.MatchLdfld<LiquidRenderer>("_liquidTextures"),
				i => i.MatchLdloc(out miscWatersType_varNum));
			c.EmitDelegate((int num2) =>
			{
				return 1;
			});
			c.GotoNext(MoveType.After, i => i.MatchLdelemRef(),
				i => i.MatchCallvirt<Asset<Texture2D>>("get_Value"));
			c.EmitLdarg(0);
			c.EmitLdloc(miscWatersType_varNum);
			c.EmitLdloc(pointer2_varNum);
			c.EmitLdfld(typeof(LiquidDrawCache).GetField("Type"));
			c.EmitDelegate((Texture2D old, LiquidRenderer self, int num2, byte Type) =>
			{
				return Type < LiquidID.Count ? self._liquidTextures[num2].Value : LiquidLoader.LiquidAssets[Type].Value;
			});
			c.GotoNext(MoveType.After, i => i.MatchCallvirt<TileBatch>("Draw"));
			c.EmitLdloc(x_varNum);
			c.EmitLdloc(y_varNum);
			c.EmitLdloc(pointer2_varNum);
			c.EmitLdarg(2);
			c.EmitLdarg(3);
			c.EmitLdarg(4);
			c.EmitLdarg(5);
			c.EmitDelegate((int i, int j, IntPtr ptr2, Vector2 drawOffset, int waterStyle, float globalAlpha, bool isBackgroundDraw) =>
			{
				LiquidLoader.PostDraw(
					i, 
					j, 
					Unsafe.AsRef<LiquidDrawCache>((void*)ptr2).Type, 
					Unsafe.As<LiquidDrawCache, Utils.Structs.LiquidDrawCache>(ref Unsafe.AsRef<LiquidDrawCache>((void*)ptr2)), 
					drawOffset, 
					isBackgroundDraw,
					waterStyle,
					globalAlpha);
			});
		}

		internal static void SpawnDustBubbles(ILContext il)
		{
			ILCursor c = new(il);
			ILLabel IL_13a0 = null;
			int x_varNum = -1;
			int y_varNum = -1;
			int pointer2_varNum = -1;

			unsafe
			{
				c.GotoNext(MoveType.After, 
					i => i.MatchStloc(out x_varNum), 
					i => i.MatchBr(out _), 
					i => i.MatchLdloc(out _), 
					i => i.MatchLdfld<Rectangle>("Y"), 
					i => i.MatchStloc(out y_varNum), 
					i => i.MatchBr(out _), 
					i => i.MatchLdloc(out pointer2_varNum), 
					i => i.MatchLdfld<LiquidCache>("VisibleType"), 
					i => i.MatchLdcI4(1));
				c.Index = 0;
				c.GotoNext(MoveType.Before, 
					i => i.MatchLdfld<LiquidCache>("VisibleType"), 
					i => i.MatchLdcI4(1), 
					i => i.MatchBneUn(out IL_13a0));
				if (IL_13a0 == null)
				{
					return;
				}
				c.EmitLdfld(typeof(LiquidCache).GetField("VisibleType"));
				c.EmitLdloc(x_varNum);
				c.EmitLdloc(y_varNum);
				c.EmitLdloc(pointer2_varNum);
				c.EmitDelegate((byte liquidID, int x, int y, IntPtr ptr2) =>
				{
					return LiquidLoader.EmitEffects(x, y, liquidID, Unsafe.As<LiquidCache, Utils.Structs.LiquidCache>(ref Unsafe.AsRef<LiquidCache>((void*)ptr2)));
				});
				c.EmitBrfalse(IL_13a0);
				c.EmitLdloc(pointer2_varNum);
			}
		}
	}
}
