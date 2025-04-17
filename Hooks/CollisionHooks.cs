using ModLiquidLib.ModLoader;
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
	internal class CollisionHooks
	{
		internal static void LiquidDrownCollisionCheck(ILContext il)
		{
			ILCursor c = new(il);
			ILLabel IL_0252 = null;
			int tile_var10 = -1;

			c.GotoNext(MoveType.After, i => i.MatchLdloca(out tile_var10), i => i.MatchCall<Tile>("lava"));
			c.EmitDelegate((bool islava) =>
			{
				bool? flag = LiquidLoader.ChecksForDrowning(LiquidID.Lava);
				if (flag == null)
				{
					return islava;
				}
				else
				{
					return !(bool)flag;
				}
			});

			c.GotoNext(MoveType.After, i => i.MatchLdloca(tile_var10), i => i.MatchCall<Tile>("shimmer"));
			c.EmitDelegate((bool isShimmer) =>
			{
				bool? flag = LiquidLoader.ChecksForDrowning(LiquidID.Shimmer);
				if (flag == null)
				{
					return isShimmer;
				}
				else
				{
					return !(bool)flag;
				}
			});

			c.GotoNext(MoveType.After, i => i.MatchBrtrue(out IL_0252));
			c.EmitLdloc(tile_var10);
			c.EmitDelegate((Tile tile) =>
			{
				if (tile.LiquidType != LiquidID.Lava && tile.LiquidType != LiquidID.Shimmer)
				{
					bool? flag = LiquidLoader.ChecksForDrowning(tile.LiquidType);
					if (flag != null)
					{
						return (bool)flag;
					}
					if (tile.LiquidType >= LiquidID.Count)
					{
						return LiquidLoader.GetLiquid(tile.LiquidType).ChecksForDrowning;
					}
				}
				return true;
			});
			c.EmitBrfalse(IL_0252);
		}
	}
}
