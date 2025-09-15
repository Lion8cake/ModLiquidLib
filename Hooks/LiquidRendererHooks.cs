using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModLiquidLib.ModLoader;
using MonoMod.Cil;
using ReLogic.Content;
using System;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.GameContent.Liquid;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.GameContent.Liquid.LiquidRenderer;

namespace ModLiquidLib.Hooks
{
	internal class LiquidRendererHooks
	{
		internal static int[] liquidAnimationFrame = new int[LiquidID.Count];

		internal static float[] liquidFrameState = new float[LiquidID.Count];

		internal static void UpdateLiquidArrayFrames(On_LiquidRenderer.orig_Update orig, LiquidRenderer self, GameTime gameTime)
		{
			orig.Invoke(self, gameTime);
			if (!Main.gamePaused && Main.hasFocus)
			{
				for (int i = 0; i < liquidFrameState.Length; i++)
				{
					if (LiquidLoader.AnimateLiquid(i, gameTime, ref liquidAnimationFrame[i], ref liquidFrameState[i]))
					{
						liquidAnimationFrame[i] = self._animationFrame;
						liquidFrameState[i] = self._frameState;
					}
				}
			}
		}

		internal static void EditAnimationField(ILContext il)
		{
			if (Terraria.ModLoader.ModLoader.HasMod("LiquidSlopesPatch"))
			{
				LiquidSlopesPatchCompat.EditAnimationField_LiquidSlopesPatch(il);
			}
			else
			{
				EditAnimationField_Vanilla(il);
			}
		}

		private unsafe static void EditAnimationField_Vanilla(ILContext il)
		{
			ILCursor c = new(il);
			int pointer2_varNum = -1;
			c.GotoNext(MoveType.After, i => i.MatchLdloc(out pointer2_varNum), i => i.MatchLdfld<SpecialLiquidDrawCache>("IsVisible"));
			c.GotoNext(MoveType.After, i => i.MatchLdfld<LiquidRenderer>("_animationFrame"));
			c.EmitLdloc(pointer2_varNum);
			c.EmitDelegate((int unused, IntPtr ptr2) =>
			{
				return liquidAnimationFrame[Unsafe.AsRef<LiquidDrawCache>((void*)ptr2).Type];
			});
		}
		
		internal static void EditLiquidRendering(ILContext il)
		{
			if (Terraria.ModLoader.ModLoader.HasMod("LiquidSlopesPatch"))
			{
				LiquidSlopesPatchCompat.EditLiquidRendering_LiquidSlopesPatch(il);
			}
			else
			{
				EditLiquidRendering_Vanilla(il);
			}
		}

		private unsafe static void EditLiquidRendering_Vanilla(ILContext il)
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

			c.GotoNext(MoveType.After, i => i.MatchLdfld<LiquidRenderer>("_animationFrame"));
			c.EmitLdloc(pointer2_varNum);
			c.EmitDelegate((int unused, IntPtr ptr2) =>
			{
				if (Unsafe.AsRef<LiquidDrawCache>((void*)ptr2).Type < liquidAnimationFrame.Length)
				{
					return liquidAnimationFrame[Unsafe.AsRef<LiquidDrawCache>((void*)ptr2).Type];
				}
				return unused;
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
				if (Type < LiquidID.Count)
				{
					return self._liquidTextures[num2].Value;
				}
				else if (Type < LiquidLoader.LiquidAssets.Length)
				{
					return LiquidLoader.LiquidAssets[Type].Value;
				}
				return old;
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
			if (Terraria.ModLoader.ModLoader.HasMod("LiquidSlopesPatch"))
			{
				LiquidSlopesPatchCompat.SpawnDustBubbles_LiquidSlopesPatch(il);
			}
			else
			{
				SpawnDustBubbles_Vanilla(il);
			}
		}

		private unsafe static void SpawnDustBubbles_Vanilla(ILContext il)
		{
			ILCursor c = new(il);
			ILLabel IL_13a0 = null;
			int x_varNum = -1;
			int y_varNum = -1;
			int pointer2_varNum = -1;
			int ptr3_varNum = -1;
			int liquidfallLoop_varNum = -1;

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

			c.GotoNext(MoveType.Before, i => i.MatchSub(), i => i.MatchStloc(out _), i => i.MatchLdloc(out ptr3_varNum), i => i.MatchLdloc(out liquidfallLoop_varNum), i => i.MatchConvI(), i => i.MatchSizeof(typeof(LiquidCache)));
			c.EmitLdloc(ptr3_varNum);
			c.EmitLdfld(typeof(LiquidCache).GetField(nameof(LiquidCache.Type)));
			c.EmitDelegate((float num3, int type) =>
			{
				if (type >= LiquidID.Count)
				{
					return (float)(num3 * LiquidLoader.GetLiquid(type).LiquidfallOpacityMultiplier);
				}
				return num3;
			});

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
			c.EmitDelegate((byte liquidID, int x, int y, IntPtr ptr3) =>
			{
				Utils.Structs.LiquidCache liquidCache = Unsafe.As<LiquidCache, Utils.Structs.LiquidCache>(ref Unsafe.AsRef<LiquidCache>((void*)ptr3));
				if (liquidCache.HasVisibleLiquid)
				{
					return LiquidLoader.EmitEffects(x, y, liquidID, liquidCache);
				}
				return false;
			});
			c.EmitBrfalse(IL_13a0);
			c.EmitLdloc(pointer2_varNum);
		}
		
		[JITWhenModsEnabled("LiquidSlopesPatch")]
		private static class LiquidSlopesPatchCompat
		{
			public unsafe static void SpawnDustBubbles_LiquidSlopesPatch(ILContext il)
			{
				ILCursor c = new(il);
				ILLabel IL_13a0 = null;
				int x_varNum = -1;
				int y_varNum = -1;
				int pointer2_varNum = -1;
				int ptr3_varNum = -1;
				int liquidfallLoop_varNum = -1;

				c.GotoNext(MoveType.After,
					i => i.MatchStloc(out x_varNum),
					i => i.MatchBr(out _),
					i => i.MatchLdloc(out _),
					i => i.MatchLdfld<Rectangle>("Y"),
					i => i.MatchStloc(out y_varNum),
					i => i.MatchBr(out _),
					i => i.MatchLdloc(out pointer2_varNum),
					i => i.MatchLdfld<LiquidSlopesPatch.Common.RewrittenLiquidRenderer.LiquidCache>("VisibleType"),
					i => i.MatchLdcI4(1));
				c.Index = 0;

				c.GotoNext(MoveType.Before, i => i.MatchSub(), i => i.MatchStloc(out _), i => i.MatchLdloc(out ptr3_varNum), i => i.MatchLdloc(out liquidfallLoop_varNum), i => i.MatchConvI(), i => i.MatchSizeof(typeof(LiquidSlopesPatch.Common.RewrittenLiquidRenderer.LiquidCache)));
				c.EmitLdloc(ptr3_varNum);
				c.EmitLdfld(typeof(LiquidSlopesPatch.Common.RewrittenLiquidRenderer.LiquidCache).GetField(nameof(LiquidSlopesPatch.Common.RewrittenLiquidRenderer.LiquidCache.Type)));
				c.EmitDelegate((float num3, int type) =>
				{
					if (type >= LiquidID.Count)
					{
						return (float)(num3 * LiquidLoader.GetLiquid(type).LiquidfallOpacityMultiplier);
					}
					return num3;
				});

				c.GotoNext(MoveType.Before, 
					i => i.MatchLdfld<LiquidSlopesPatch.Common.RewrittenLiquidRenderer.LiquidCache>("VisibleType"), 
					i => i.MatchLdcI4(1), 
					i => i.MatchBneUn(out IL_13a0));
				if (IL_13a0 == null)
				{
					return;
				}
				c.EmitLdfld(typeof(LiquidSlopesPatch.Common.RewrittenLiquidRenderer.LiquidCache).GetField("VisibleType"));
				c.EmitLdloc(x_varNum);
				c.EmitLdloc(y_varNum);
				c.EmitLdloc(pointer2_varNum);
				c.EmitDelegate((byte liquidID, int x, int y, IntPtr ptr3) =>
				{
					Utils.Structs.LiquidCache liquidCache = Unsafe.As<LiquidSlopesPatch.Common.RewrittenLiquidRenderer.LiquidDrawCache, Utils.Structs.LiquidCache>(ref Unsafe.AsRef<LiquidSlopesPatch.Common.RewrittenLiquidRenderer.LiquidDrawCache>((void*)ptr3));
					if (liquidCache.HasVisibleLiquid)
					{
						return LiquidLoader.EmitEffects(x, y, liquidID, liquidCache);
					}
					return false;
				});
				c.EmitBrfalse(IL_13a0);
				c.EmitLdloc(pointer2_varNum);
			}
			
    		public unsafe static void EditLiquidRendering_LiquidSlopesPatch(ILContext il)
    		{
    		    ILCursor c = new(il);
    		    int x_varNum = -1;
    		    int y_varNum = -1;
    		    int pointer2_varNum = -1;
    		    int miscWatersType_varNum = -1;

    		    /*c.GotoNext(MoveType.After, i => i.MatchStloc(out x_varNum), i => i.MatchBr(out _), i => i.MatchLdloc(out _), i => i.MatchLdfld<Rectangle>("Y"), i => i.MatchStloc(out y_varNum));
    		    c.GotoNext(MoveType.After, i => i.MatchLdloc(out pointer2_varNum), i => i.MatchLdfld<LiquidDrawCache>("IsVisible"));*/

		        ILLabel loopLabel = null;
		        c.GotoNext(x => x.MatchLdfld<LiquidSlopesPatch.Common.RewrittenLiquidRenderer.LiquidDrawCache>("IsVisible"), x => x.MatchBrfalse(out loopLabel));
		        
    		    c.GotoNext(x => x.MatchLdfld<LiquidSlopesPatch.Common.RewrittenLiquidRenderer.LiquidDrawCache>("X"));
    		    c.GotoPrev(x => x.MatchLdloc(out pointer2_varNum));
    		    c.GotoNext(x => x.MatchStloc(out x_varNum));
    		    c.GotoNext(MoveType.After, x => x.MatchStloc(out y_varNum));
    		    
    		    // c.EmitLdloc(pointer2_varNum);
		        // c.EmitLdfld(typeof(LiquidSlopesPatch.Common.RewrittenLiquidRenderer.LiquidDrawCache).GetField("IsVisible"));
    		    c.EmitLdloc(x_varNum);
    		    c.EmitLdloc(y_varNum);
    		    c.EmitLdloc(pointer2_varNum);
    		    c.EmitLdarg(2);
    		    c.EmitLdarg(3);
    		    c.EmitLdarg(4);
    		    c.EmitLdarg(5);
    		    c.EmitDelegate((/*bool isVisibleCondition,*/ int i, int j, IntPtr ptr2, Vector2 drawOffset, int waterStyle, float globalAlpha, bool isBackgroundDraw) =>
    		    {
    		        return /*isVisibleCondition &&*/ LiquidLoader.PreDraw(
    		            i, 
    		            j, 
    		            Unsafe.AsRef<LiquidSlopesPatch.Common.RewrittenLiquidRenderer.LiquidDrawCache>((void*)ptr2).Type, 
    		            Unsafe.As<LiquidSlopesPatch.Common.RewrittenLiquidRenderer.LiquidDrawCache, Utils.Structs.LiquidDrawCache>(ref Unsafe.AsRef<LiquidSlopesPatch.Common.RewrittenLiquidRenderer.LiquidDrawCache>((void*)ptr2)), 
    		            drawOffset, 
    		            isBackgroundDraw,
    		            waterStyle,
    		            globalAlpha);
    		    });
		        c.EmitBrfalse(loopLabel);

    		    c.GotoPrev(MoveType.After, i => i.MatchLdsfld(typeof(LiquidSlopesPatch.Common.RewrittenLiquidRenderer), "_animationFrame"));
    		    c.EmitLdloc(pointer2_varNum);
    		    c.EmitDelegate((int unused, IntPtr ptr2) =>
    		    {
    		        if (Unsafe.AsRef<LiquidSlopesPatch.Common.RewrittenLiquidRenderer.LiquidDrawCache>((void*)ptr2).Type < liquidAnimationFrame.Length)
    		        {
    		            return liquidAnimationFrame[Unsafe.AsRef<LiquidSlopesPatch.Common.RewrittenLiquidRenderer.LiquidDrawCache>((void*)ptr2).Type];
    		        }
    		        return unused;
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
    		    c.EmitLdfld(typeof(LiquidSlopesPatch.Common.RewrittenLiquidRenderer.LiquidDrawCache).GetField("Type"));
    		    c.EmitDelegate((Texture2D old, LiquidRenderer self, int num2, byte Type) =>
    		    {
    		        if (Type < LiquidID.Count)
    		        {
    		            return self._liquidTextures[num2].Value;
    		        }
    		        else if (Type < LiquidLoader.LiquidAssets.Length)
    		        {
    		            return LiquidLoader.LiquidAssets[Type].Value;
    		        }
    		        return old;
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
    		            Unsafe.AsRef<LiquidSlopesPatch.Common.RewrittenLiquidRenderer.LiquidDrawCache>((void*)ptr2).Type, 
    		            Unsafe.As<LiquidSlopesPatch.Common.RewrittenLiquidRenderer.LiquidDrawCache, Utils.Structs.LiquidDrawCache>(ref Unsafe.AsRef<LiquidSlopesPatch.Common.RewrittenLiquidRenderer.LiquidDrawCache>((void*)ptr2)), 
    		            drawOffset, 
    		            isBackgroundDraw,
    		            waterStyle,
    		            globalAlpha);
    		    });
    		}
		    
		    public unsafe static void EditAnimationField_LiquidSlopesPatch(ILContext il)
		    {
			    ILCursor c = new(il);
			    int pointer2_varNum = -1;
			    c.GotoNext(MoveType.After, i => i.MatchLdloc(out pointer2_varNum), i => i.MatchLdfld<LiquidSlopesPatch.Common.RewrittenLiquidRenderer.LiquidDrawCache>("IsVisible"));
			    c.GotoNext(MoveType.After, i => i.MatchLdsfld(typeof(LiquidSlopesPatch.Common.RewrittenLiquidRenderer), "_animationFrame"));
			    c.EmitLdloc(pointer2_varNum);
			    c.EmitDelegate((int unused, IntPtr ptr2) =>
			    {
				    return liquidAnimationFrame[Unsafe.AsRef<LiquidSlopesPatch.Common.RewrittenLiquidRenderer.LiquidDrawCache>((void*)ptr2).Type];
			    });
		    }
		}
	}
}
