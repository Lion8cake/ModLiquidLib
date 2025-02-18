using Microsoft.Xna.Framework.Graphics;
using System;
using static Terraria.GameContent.Liquid.LiquidRenderer;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Liquid;
using ModLiquidLib.ModLoader;
using Terraria.ID;
using MonoMod.Cil;
using Terraria.Graphics;
using ReLogic.Content;

namespace ModLiquidLib.Hooks
{
	internal class LiquidRendererHooks
	{
		internal static void EditLiquidRendering(ILContext il)
		{
			ILCursor c = new(il);
			c.GotoNext(MoveType.After, i => i.MatchLdloc(2), i => i.MatchLdfld<LiquidRenderer.LiquidDrawCache>("IsVisible"));
			c.EmitLdloc(3);
			c.EmitLdloc(4);
			c.EmitLdloc(2);
			c.EmitLdfld(typeof(LiquidDrawCache).GetField("Type"));
			c.EmitLdloc(2);
			c.EmitLdfld(typeof(LiquidDrawCache).GetField("SourceRectangle"));
			c.EmitLdloc(2);
			c.EmitLdfld(typeof(LiquidDrawCache).GetField("LiquidOffset"));
			c.EmitLdloc(2);
			c.EmitLdfld(typeof(LiquidDrawCache).GetField("IsVisible"));
			c.EmitLdloc(2);
			c.EmitLdfld(typeof(LiquidDrawCache).GetField("Opacity"));
			c.EmitLdloc(2);
			c.EmitLdfld(typeof(LiquidDrawCache).GetField("IsSurfaceLiquid"));
			c.EmitLdloc(2);
			c.EmitLdfld(typeof(LiquidDrawCache).GetField("HasWall"));
			c.EmitLdarg(2);
			c.EmitLdarg(5);
			c.EmitDelegate((bool isVisibleCondition, int i, int j, byte Type, Rectangle SourceRectangle, Vector2 LiquidOffset, bool IsVisible, float Opacity, bool IsSurfaceLiquid, bool HasWall, Vector2 drawOffset, bool isBackgroundDraw) =>
			{
				return isVisibleCondition && LiquidLoader.PreDraw(i, j, Type, new Utils.LiquidDrawCache
				{
					SourceRectangle = SourceRectangle,
					LiquidOffset = LiquidOffset,
					IsVisible = IsVisible,
					Opacity = Opacity,
					Type = Type,
					IsSurfaceLiquid = IsSurfaceLiquid,
					HasWall = HasWall
				}, drawOffset, isBackgroundDraw);
			});
			c.GotoNext(MoveType.After, 
				i => i.MatchLdfld<LiquidRenderer>("_liquidTextures"), 
				i => i.MatchLdloc(8), i => i.MatchLdelemRef(), 
				i => i.MatchCallvirt<Asset<Texture2D>>("get_Value"));
			c.EmitLdloc(2);
			c.EmitLdfld(typeof(LiquidDrawCache).GetField("Type"));
			c.EmitDelegate((Texture2D texture, byte Type) =>
			{
				return Type < LiquidID.Count ? texture : LiquidLoader.LiquidAssets[Type].Value;
			});
			c.GotoNext(MoveType.After, i => i.MatchCallvirt<TileBatch>("Draw"));
			c.EmitLdloc(3);
			c.EmitLdloc(4);
			c.EmitLdloc(2);
			c.EmitLdfld(typeof(LiquidDrawCache).GetField("Type"));
			c.EmitLdloc(2);
			c.EmitLdfld(typeof(LiquidDrawCache).GetField("SourceRectangle"));
			c.EmitLdloc(2);
			c.EmitLdfld(typeof(LiquidDrawCache).GetField("LiquidOffset"));
			c.EmitLdloc(2);
			c.EmitLdfld(typeof(LiquidDrawCache).GetField("IsVisible"));
			c.EmitLdloc(2);
			c.EmitLdfld(typeof(LiquidDrawCache).GetField("Opacity"));
			c.EmitLdloc(2);
			c.EmitLdfld(typeof(LiquidDrawCache).GetField("IsSurfaceLiquid"));
			c.EmitLdloc(2);
			c.EmitLdfld(typeof(LiquidDrawCache).GetField("HasWall"));
			c.EmitLdarg(2);
			c.EmitLdarg(5);
			c.EmitDelegate((int i, int j, byte Type, Rectangle SourceRectangle, Vector2 LiquidOffset, bool IsVisible, float Opacity, bool IsSurfaceLiquid, bool HasWall, Vector2 drawOffset, bool isBackgroundDraw) =>
			{
				LiquidLoader.PostDraw(i, j, Type, new Utils.LiquidDrawCache
				{
					SourceRectangle = SourceRectangle,
					LiquidOffset = LiquidOffset,
					IsVisible = IsVisible,
					Opacity = Opacity,
					Type = Type,
					IsSurfaceLiquid = IsSurfaceLiquid,
					HasWall = HasWall
				}, drawOffset, isBackgroundDraw);
			});
		}
	}
}
