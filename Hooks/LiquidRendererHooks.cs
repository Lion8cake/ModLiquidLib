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
using Terraria.Utilities;
using System.Xml.Serialization;
using Terraria.ModLoader;

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

		internal static void SpawnDustBubbles(ILContext il)
		{
			ILCursor c = new(il);
			ILLabel IL_13a0 = null;
			c.GotoNext(MoveType.Before, i => i.MatchLdfld<LiquidCache>("VisibleType"), i => i.MatchLdcI4(1), i => i.MatchBneUn(out IL_13a0));
			if (IL_13a0 == null)
			{
				return;
			}
			c.EmitLdfld(typeof(LiquidCache).GetField("VisibleType"));
			c.EmitLdloc(66);
			c.EmitLdloc(67);
			c.EmitLdloc(4);
			c.EmitLdfld(typeof(LiquidCache).GetField("BottomWall"));
			c.EmitLdloc(4);
			c.EmitLdfld(typeof(LiquidCache).GetField("FrameOffset"));
			c.EmitLdloc(4);
			c.EmitLdfld(typeof(LiquidCache).GetField("HasBottomEdge"));
			c.EmitLdloc(4);
			c.EmitLdfld(typeof(LiquidCache).GetField("HasLeftEdge"));
			c.EmitLdloc(4);
			c.EmitLdfld(typeof(LiquidCache).GetField("HasLiquid"));
			c.EmitLdloc(4);
			c.EmitLdfld(typeof(LiquidCache).GetField("HasRightEdge"));
			c.EmitLdloc(4);
			c.EmitLdfld(typeof(LiquidCache).GetField("HasTopEdge"));
			c.EmitLdloc(4);
			c.EmitLdfld(typeof(LiquidCache).GetField("HasVisibleLiquid"));
			c.EmitLdloc(4);
			c.EmitLdfld(typeof(LiquidCache).GetField("HasWall"));
			c.EmitLdloc(4);
			c.EmitLdfld(typeof(LiquidCache).GetField("IsHalfBrick"));
			c.EmitLdloc(4);
			c.EmitLdfld(typeof(LiquidCache).GetField("IsSolid"));
			c.EmitLdloc(4);
			c.EmitLdfld(typeof(LiquidCache).GetField("LeftWall"));
			c.EmitLdloc(4);
			c.EmitLdfld(typeof(LiquidCache).GetField("LiquidLevel"));
			c.EmitLdloc(4);
			c.EmitLdfld(typeof(LiquidCache).GetField("Opacity"));
			c.EmitLdloc(4);
			c.EmitLdfld(typeof(LiquidCache).GetField("RightWall"));
			c.EmitLdloc(4);
			c.EmitLdfld(typeof(LiquidCache).GetField("TopWall"));
			c.EmitLdloc(4);
			c.EmitLdfld(typeof(LiquidCache).GetField("Type"));
			c.EmitLdloc(4);
			c.EmitLdfld(typeof(LiquidCache).GetField("VisibleBottomWall"));
			c.EmitLdloc(4);
			c.EmitLdfld(typeof(LiquidCache).GetField("VisibleLeftWall"));
			c.EmitLdloc(4);
			c.EmitLdfld(typeof(LiquidCache).GetField("VisibleLiquidLevel"));
			c.EmitLdloc(4);
			c.EmitLdfld(typeof(LiquidCache).GetField("VisibleRightWall"));
			c.EmitLdloc(4);
			c.EmitLdfld(typeof(LiquidCache).GetField("VisibleTopWall"));
			c.EmitDelegate((byte liquidID, int x, int y, 
				float BottomWall, Point FrameOffset, bool HasBottomEdge, bool HasLeftEdge, bool HasLiquid, bool HasRightEdge, bool HasTopEdge, bool HasVisibleLiquid, bool HasWall, bool IsHalfBrick, 
				bool IsSolid, float LeftWall, float LiquidLevel, float Opacity, float RightWall, float TopWall, byte Type, float VisibleBottomWall, float VisibleLeftWall, float VisibleLiquidLevel, float VisibleRightWall, float VisibleTopWall) =>
			{
				return LiquidLoader.EmitEffects(x, y, liquidID, new Utils.LiquidCache
				{
					VisibleType = liquidID,
					BottomWall = BottomWall,
					FrameOffset = FrameOffset,
					HasBottomEdge = HasBottomEdge,
					HasLeftEdge = HasLeftEdge,
					HasLiquid = HasLiquid,
					HasRightEdge = HasRightEdge,
					HasTopEdge = HasTopEdge,
					HasVisibleLiquid = HasVisibleLiquid,
					HasWall = HasWall,
					IsHalfBrick = IsHalfBrick,
					IsSolid = IsSolid,
					LeftWall = LeftWall,
					LiquidLevel = LiquidLevel,
					Opacity = Opacity,
					RightWall = RightWall,
					TopWall = TopWall,
					Type = Type,
					VisibleBottomWall = VisibleBottomWall,
					VisibleLeftWall = VisibleLeftWall,
					VisibleLiquidLevel = VisibleLiquidLevel,
					VisibleRightWall = VisibleRightWall,
					VisibleTopWall = VisibleTopWall
				});
			});
			c.EmitBrfalse(IL_13a0);
			c.EmitLdloc(4);
		}
	}
}
