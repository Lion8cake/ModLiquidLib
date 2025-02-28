using ModLiquidLib.ModLoader.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModLiquidLib.Testing
{
	internal class TestingItem : ModItem
	{
		public override string Texture => "Terraria/Images/Item_" + ItemID.EmptyBucket;

		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 32;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.stack = 9999;
			Item.useStyle = 1;
		}

		public override bool? UseItem(Player player)
		{
			Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
			//tile.LiquidType = 6;
			tile.LiquidType = ModLiquidLib.LiquidType<ExampleLiquid>();
			//tile.LiquidType = LiquidID.Honey;
			tile.LiquidAmount = 255;
			return null;
		}
	}
}
