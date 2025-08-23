using Microsoft.Xna.Framework;
using ModLiquidLib.ModLoader;
using MonoMod.Cil;
using Terraria;
using Terraria.ID;

namespace ModLiquidLib.Hooks
{
	public class CollisionHooks
	{
		internal static Vector2 PreventWaterCollisionOverloadFromExecuting(On_Collision.orig_WaterCollision orig, Vector2 Position, Vector2 Velocity, int Width, int Height, bool fallThrough, bool fall2, bool lavaWalk)
		{
			return Velocity;
		}

		public static Vector2 WaterCollision(Vector2 Position, Vector2 Velocity, int Width, int Height, bool[] walkableLiquids, bool fallThrough = false, bool fall2 = false)
		{
			Vector2 result = Velocity;
			Vector2 vector = Position + Velocity;
			int value5 = (int)(Position.X / 16f) - 1;
			int value2 = (int)((Position.X + (float)Width) / 16f) + 2;
			int value3 = (int)(Position.Y / 16f) - 1;
			int value4 = (int)((Position.Y + (float)Height) / 16f) + 2;
			int num3 = Terraria.Utils.Clamp(value5, 0, Main.maxTilesX - 1);
			value2 = Terraria.Utils.Clamp(value2, 0, Main.maxTilesX - 1);
			value3 = Terraria.Utils.Clamp(value3, 0, Main.maxTilesY - 1);
			value4 = Terraria.Utils.Clamp(value4, 0, Main.maxTilesY - 1);
			Vector2 vector2 = default(Vector2);
			for (int i = num3; i < value2; i++)
			{
				for (int j = value3; j < value4; j++)
				{
					if (Main.tile[i, j].LiquidAmount > 0 && Main.tile[i, j - 1].LiquidAmount == 0 && walkableLiquids[Main.tile[i, j].LiquidType])
					{
						int num2 = Main.tile[i, j].LiquidAmount / 32 * 2 + 2;
						vector2.X = i * 16;
						vector2.Y = j * 16 + 16 - num2;
						if (vector.X + (float)Width > vector2.X && vector.X < vector2.X + 16f && vector.Y + (float)Height > vector2.Y && vector.Y < vector2.Y + (float)num2 && Position.Y + (float)Height <= vector2.Y && !fallThrough)
						{
							result.Y = vector2.Y - (Position.Y + (float)Height);
						}
					}
				}
			}
			return result;
		}

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
