using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Light;
using Terraria;
using Microsoft.Xna.Framework;
using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;
using Terraria.ID;

namespace ModLiquidLib.Hooks
{
	internal class TileLightScannerHooks
	{
		internal static void ApplyModliquidLight(On_TileLightScanner.orig_ApplyLiquidLight orig, TileLightScanner self, Tile tile, ref Vector3 lightColor)
		{
			if (tile.LiquidAmount <= 0)
			{
				return;
			}
			orig.Invoke(self, tile, ref lightColor);
			float R = lightColor.X;
			float G = lightColor.Y;
			float B = lightColor.Z;
			lightColor.X = 0f;
			lightColor.Y = 0f;
			lightColor.Z = 0f;
			LiquidLoader.ModifyLight(tile.X(), tile.Y(), tile.LiquidType, ref R, ref G, ref B);
			if (lightColor.X < R)
			{
				lightColor.X = R;
			}
			if (lightColor.Y < G)
			{
				lightColor.Y = G;
			}
			if (lightColor.Z < B)
			{
				lightColor.Z = B;
			}
		}
	}
}
