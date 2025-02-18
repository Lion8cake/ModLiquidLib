using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModLiquidLib.Utils
{
	public struct SpecialLiquidDrawCache
	{
		public int X;

		public int Y;

		public Rectangle SourceRectangle;

		public Vector2 LiquidOffset;

		public bool IsVisible;

		public float Opacity;

		public byte Type;

		public bool IsSurfaceLiquid;

		public bool HasWall;
	}
}
