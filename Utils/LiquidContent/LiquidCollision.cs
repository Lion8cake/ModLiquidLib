using Microsoft.Xna.Framework;
using ModLiquidLib.ModLoader;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace ModLiquidLib.Utils.LiquidContent
{
	public class LiquidCollision
	{
		public static bool WetCollision(Vector2 Position, int Width, int Height, out bool[] liquidsIn)
		{
			liquidsIn = new bool[LiquidLoader.LiquidCount];
			Vector2 vector = new(Position.X + Width / 2, Position.Y + Height / 2);
			int num = 10;
			int num2 = Height / 2;
			if (num > Width)
			{
				num = Width;
			}
			if (num2 > Height)
			{
				num2 = Height;
			}
			vector = new(vector.X - num / 2, vector.Y - num2 / 2);
			int value5 = (int)(Position.X / 16f) - 1;
			int value2 = (int)((Position.X + Width) / 16f) + 2;
			int value3 = (int)(Position.Y / 16f) - 1;
			int value4 = (int)((Position.Y + Height) / 16f) + 2;
			int num6 = Terraria.Utils.Clamp(value5, 0, Main.maxTilesX - 1);
			value2 = Terraria.Utils.Clamp(value2, 0, Main.maxTilesX - 1);
			value3 = Terraria.Utils.Clamp(value3, 0, Main.maxTilesY - 1);
			value4 = Terraria.Utils.Clamp(value4, 0, Main.maxTilesY - 1);
			Vector2 vector2 = default;
			for (int i = num6; i < value2; i++)
			{
				for (int j = value3; j < value4; j++)
				{
					if (Main.tile[i, j].LiquidAmount > 0)
					{
						vector2.X = i * 16;
						vector2.Y = j * 16;
						int num3 = 16;
						float num4 = 256 - Main.tile[i, j].LiquidAmount;
						num4 /= 32f;
						vector2.Y += num4 * 2f;
						num3 -= (int)(num4 * 2f);
						if (vector.X + num > vector2.X && vector.X < vector2.X + 16f && vector.Y + num2 > vector2.Y && vector.Y < vector2.Y + num3)
						{
							liquidsIn[Main.tile[i, j].LiquidType] = true;
							return true;
						}
					}
					else
					{
						if (!Main.tile[i, j].HasTile || Main.tile[i, j].Slope == 0 || j <= 0 || Main.tile[i, j - 1].LiquidAmount <= 0)
						{
							continue;
						}
						vector2.X = i * 16;
						vector2.Y = j * 16;
						int num5 = 16;
						if (vector.X + num > vector2.X && vector.X < vector2.X + 16f && vector.Y + num2 > vector2.Y && vector.Y < vector2.Y + num5)
						{
							liquidsIn[Main.tile[i, j].LiquidType] = true;
							return true;
						}
					}
				}
			}
			return false;
		}

		public static bool LavaCollision(Vector2 Position, int Width, int Height, out bool[] liquidsIn)
		{
			liquidsIn = new bool[LiquidLoader.LiquidCount];
			int value5 = (int)(Position.X / 16f) - 1;
			int value2 = (int)((Position.X + (float)Width) / 16f) + 2;
			int value3 = (int)(Position.Y / 16f) - 1;
			int value4 = (int)((Position.Y + (float)Height) / 16f) + 2;
			int num4 = Terraria.Utils.Clamp(value5, 0, Main.maxTilesX - 1);
			value2 = Terraria.Utils.Clamp(value2, 0, Main.maxTilesX - 1);
			value3 = Terraria.Utils.Clamp(value3, 0, Main.maxTilesY - 1);
			value4 = Terraria.Utils.Clamp(value4, 0, Main.maxTilesY - 1);
			for (int i = num4; i < value2; i++)
			{
				for (int j = value3; j < value4; j++)
				{
					if (Main.tile[i, j].LiquidAmount > 0)
					{
						Vector2 vector;
						vector.X = i * 16;
						vector.Y = j * 16;
						int num2 = 16;
						float num3 = 256 - Main.tile[i, j].liquid;
						num3 /= 32f;
						vector.Y += num3 * 2f;
						num2 -= (int)(num3 * 2f);
						if (Position.X + (float)Width > vector.X && Position.X < vector.X + 16f && Position.Y + (float)Height > vector.Y && Position.Y < vector.Y + (float)num2)
						{
							liquidsIn[Main.tile[i, j].LiquidType] = true;
							return true;
						}
					}
				}
			}
			return false;
		}

		public static bool GetAppropriateWets(Vector2 Position, int Width, int Height, out bool[] liquidsIn)
		{
			liquidsIn = new bool[LiquidLoader.LiquidCount];
			WetCollision(Position, Width, Height, out bool[] liquidsInNorm);
			LavaCollision(Position, Width, Height, out bool[] liquidsInAlt);
			Array.Copy(liquidsInNorm, liquidsIn, LiquidLoader.LiquidCount);
			for (int i = LiquidID.Count; i < liquidsInAlt.Length; i++)
			{
				if (LiquidLoader.GetLiquid(i).UsesLavaCollisionForWet)
				{
					liquidsIn[i] = liquidsInAlt[i];
				}
			}
			return liquidsIn.Contains<bool>(true);
		}
	}
}
