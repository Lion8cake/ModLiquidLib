using Microsoft.Xna.Framework;
using ModLiquidLib.ModLoader;
using Terraria;

namespace ModLiquidLib.Utils
{
	public class LiquidCollision
	{
		public static bool WetCollision(Vector2 Position, int Width, int Height, out bool[] liquidsIn)
		{
			liquidsIn = new bool[LiquidLoader.LiquidCount];
			Vector2 vector = new(Position.X + (float)(Width / 2), Position.Y + (float)(Height / 2));
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
			vector = new(vector.X - (float)(num / 2), vector.Y - (float)(num2 / 2));
			int value5 = (int)(Position.X / 16f) - 1;
			int value2 = (int)((Position.X + (float)Width) / 16f) + 2;
			int value3 = (int)(Position.Y / 16f) - 1;
			int value4 = (int)((Position.Y + (float)Height) / 16f) + 2;
			int num6 = Terraria.Utils.Clamp(value5, 0, Main.maxTilesX - 1);
			value2 = Terraria.Utils.Clamp(value2, 0, Main.maxTilesX - 1);
			value3 = Terraria.Utils.Clamp(value3, 0, Main.maxTilesY - 1);
			value4 = Terraria.Utils.Clamp(value4, 0, Main.maxTilesY - 1);
			Vector2 vector2 = default(Vector2);
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
						if (vector.X + (float)num > vector2.X && vector.X < vector2.X + 16f && vector.Y + (float)num2 > vector2.Y && vector.Y < vector2.Y + (float)num3)
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
						if (vector.X + (float)num > vector2.X && vector.X < vector2.X + 16f && vector.Y + (float)num2 > vector2.Y && vector.Y < vector2.Y + (float)num5)
						{
							liquidsIn[Main.tile[i, j].LiquidType] = true;
							return true;
						}
					}
				}
			}
			return false;
		}
	}
}
