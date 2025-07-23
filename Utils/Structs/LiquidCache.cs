using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModLiquidLib.Utils.Structs
{
	public struct LiquidCache
	{
		public float LiquidLevel;

		public float VisibleLiquidLevel;

		public float Opacity;

		public bool IsSolid;

		public bool IsHalfBrick;

		public bool HasLiquid;

		public bool HasVisibleLiquid;

		public bool HasWall;

		public Point FrameOffset;

		public bool HasLeftEdge;

		public bool HasRightEdge;

		public bool HasTopEdge;

		public bool HasBottomEdge;

		public float LeftWall;

		public float RightWall;

		public float BottomWall;

		public float TopWall;

		public float VisibleLeftWall;

		public float VisibleRightWall;

		public float VisibleBottomWall;

		public float VisibleTopWall;

		public byte Type;

		public byte VisibleType;
	}
}
