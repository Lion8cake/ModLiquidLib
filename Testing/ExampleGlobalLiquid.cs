using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace ModLiquidLib.Testing
{
	public class ExampleGlobalLiquid : GlobalLiquid
	{
		public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
		{
			/*if (type == LiquidID.Lava)
			{
				r = 0f;
				g = 0f;
				b = 0f;
			}
			if (type == ModLiquidLib.LiquidType<ExampleLiquid>())
			{
				r = 0f;
				g = 0f; 
				b = 0f;
			}*/
		}

		public override bool EmitEffects(int i, int j, int type, LiquidCache liquidCache)
		{
			//Main.NewText("test global");
			//Main.NewText(LiquidLoader.GetLiquid(type) != null ? LiquidLoader.GetLiquid(type).Name : type);
			if (type == LiquidID.Lava)
			{
				return false;
			}
			return true;
		}

		public override bool PreRetroDraw(int i, int j, int type, SpriteBatch spriteBatch)
		{
			if (type == LiquidID.Honey)
				return true;
			return true;
		}

		public override bool DisableRetroLavaBubbles(int i, int j)
		{
			return false;
		}

		public override int? ChooseWaterfallStyle(int i, int j, int type)
		{
			//if (type == LiquidID.Shimmer)
			//{
			//	return 2;
			//}
			//if (type == ModLiquidLib.LiquidType<ExampleLiquid>())
			//{
			//	return 2;
			//}
			return null;
		}
	}
}
