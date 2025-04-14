using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace ModLiquidLib.Hooks
{
	internal class PlayerHooks
	{
		internal static void AddLiquidCraftingConditions(ILContext il)
		{
			ILCursor c = new(il);
			int j_var6 = -1;
			int k_var7 = -1;
			int flag_var4 = -1;

			c.GotoNext(MoveType.After, i => i.MatchCall<Player>("get_adjTile"), i => i.MatchLdlen(), i => i.MatchConvI4(), i => i.MatchBlt(out _));
			c.EmitLdarg(0);
			c.EmitDelegate((Player self) =>
			{
				for (int i = 0; i < self.GetModPlayer<ModLiquidPlayer>().adjLiquid.Length; i++)
				{
					self.GetModPlayer<ModLiquidPlayer>().oldAdjLiquid[i] = self.GetModPlayer<ModLiquidPlayer>().adjLiquid[i];
					self.GetModPlayer<ModLiquidPlayer>().adjLiquid[i] = false;
				}
			});

			c.GotoNext(i => i.MatchLdsflda<Main>("tile"), i => i.MatchLdloc(out j_var6), i => i.MatchLdloc(out k_var7), i => i.MatchCall<Tilemap>("get_Item"), i => i.MatchStloc(out _));

			c.GotoNext(MoveType.Before, i => i.MatchLdloca(out _), i => i.MatchCall<Tile>("get_liquid"));
			c.EmitLdarg(0);
			c.EmitLdloc(j_var6);
			c.EmitLdloc(k_var7);
			c.EmitDelegate((Player self, int j, int k) =>
			{
				if (Main.tile[j, k].LiquidAmount > 200)
				{
					self.GetModPlayer<ModLiquidPlayer>().adjLiquid[Main.tile[j, k].LiquidType] = true;
				}
				//if (TileLiquidIDSets.Sets.CountsAsLiquidSource[Main.tile[j, k].TileType].Contains(true))
				//{
				//	self.GetModPlayer<ModLiquidPlayer>().adjLiquid = TileLiquidIDSets.Sets.CountsAsLiquidSource[Main.tile[j, k].TileType]; //TODO: Add Tile support for liquid adjustments
				//}
				LiquidLoader.AdjLiquids(self, Main.tile[j, k].LiquidType);
			});

			c.GotoNext(MoveType.After, i => i.MatchRet(), i => i.MatchLdcI4(0), i => i.MatchStloc(out flag_var4));
			c.EmitLdarg(0);
			c.EmitLdloca(flag_var4);
			c.EmitDelegate((Player self, ref bool flag) =>
			{
				self.adjWater = self.GetModPlayer<ModLiquidPlayer>().adjLiquid[0];
				self.adjLava = self.GetModPlayer<ModLiquidPlayer>().adjLiquid[1];
				self.adjHoney = self.GetModPlayer<ModLiquidPlayer>().adjLiquid[2];
				self.adjShimmer = self.GetModPlayer<ModLiquidPlayer>().adjLiquid[3];
				for (int l = 0; l < self.GetModPlayer<ModLiquidPlayer>().adjLiquid.Length; l++)
				{
					if (self.GetModPlayer<ModLiquidPlayer>().oldAdjLiquid[l] != self.GetModPlayer<ModLiquidPlayer>().adjLiquid[l])
					{
						flag = true;
						break;
					}
				}
			});
		}
	}
}
