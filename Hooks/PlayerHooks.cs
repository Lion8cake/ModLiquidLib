using AsmResolver.DotNet.Cloning;
using Microsoft.Xna.Framework;
using ModLiquidLib.ID;
using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

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

		internal static void PreventLiquidBlockswap(ILContext il)
		{
			ILCursor c = new(il);
			ILLabel IL_005f = c.DefineLabel();

			c.GotoNext(MoveType.After, i => i.MatchLdsfld<Player>("tileTargetX"), i => i.MatchLdsfld<Player>("tileTargetY"), i => i.MatchCall<Tilemap>("get_Item"), i => i.MatchStloc(out _));
			c.EmitLdarg(0);
			c.EmitDelegate((Player self) =>
			{
				for (int i = 0; i < LiquidLoader.LiquidCount; i++)
				{
					if (LiquidLoader.BlocksTilePlacement(self, Player.tileTargetX, Player.tileTargetY, i))
					{
						return !WorldGen.WouldTileReplacementBeBlockedByLiquid(Player.tileTargetX, Player.tileTargetY, i);
					}
				}
				return true;
			});
			c.EmitBrtrue(IL_005f);
			c.EmitLdcI4(0);
			c.EmitRet();
			c.MarkLabel(IL_005f);

			c.GotoNext(MoveType.Before, i => i.MatchCall<WorldGen>("WouldTileReplacementBeBlockedByLiquid"), i => i.MatchBrfalse(out _));
			c.EmitLdarg(0);
			c.EmitDelegate((int lavaID, Player self) =>
			{
				if (LiquidLoader.BlocksTilePlacement(self, Player.tileTargetX, Player.tileTargetY, lavaID))
				{
					return lavaID;
				}
				return -1;
			});
		}

		internal static void PreventPlacingTilesInLiquids(ILContext il)
		{
			ILCursor c = new(il);
			c.GotoNext(MoveType.After, i => i.MatchCall<Tile>("lava"));
			c.EmitLdarg(0);
			c.EmitDelegate((bool isLavaTile, Player self) =>
			{
				return LiquidLoader.BlocksTilePlacement(self, Player.tileTargetX, Player.tileTargetY, Main.tile[Player.tileTargetX, Player.tileTargetY].LiquidType);
			});
		}

		internal static void PreventRopePlacingInLiquid(ILContext il)
		{
			ILCursor c = new(il);
			int num2_var1 = -1;
			int num_var2 = -1;

			c.GotoNext(MoveType.After, i => i.MatchLdloc(out num2_var1), i => i.MatchLdloc(out num_var2), i => i.MatchLdcI4(1), i => i.MatchAdd(), i => i.MatchCall<Tilemap>("get_Item"), i => i.MatchStloc(out _), i => i.MatchLdloca(out _), i => i.MatchCall<Tile>("lava"));
			c.EmitLdarg(0);
			c.EmitLdloc(num2_var1);
			c.EmitLdloc(num_var2);
			c.EmitDelegate((bool isLavaTile, Player self, int num2, int num) =>
			{
				return LiquidLoader.BlocksTilePlacement(self, num2, num + 1, Main.tile[num2, num + 1].LiquidType);
			});
		}

		internal static void PlayerLiquidCollision(ILContext il)
		{
			ILCursor c = new(il);
			ILLabel[] IL_0000 = new ILLabel[8];
			int shimmer_var9 = -1;

			c.GotoNext(MoveType.After, i => i.MatchLdsfld<Collision>("shimmer"), i => i.MatchStloc(out _));
			c.EmitLdarg(0);
			c.EmitDelegate((Player self) =>
			{
				LiquidCollision.WetCollision(self.position, self.width, self.height, out bool[] liquidIn);
				for (int i = LiquidID.Count; i < LiquidLoader.LiquidCount; i++)
				{
					if (liquidIn[i])
					{
						self.GetModPlayer<ModLiquidPlayer>().moddedWet[i - LiquidID.Count] = true;
					}
				}
			});
			for (int j = 0; j < 2; j++)
			{
				#region Shimmer Splash Edit 
				//Shimmer
				c.GotoNext(MoveType.After, i => i.MatchLdfld<Entity>("shimmerWet"), i => i.MatchBrfalse(out IL_0000[0 + (j * 4)]), i => i.MatchLdcI4(0), i => i.MatchStloc(out _));
				c.EmitLdarg(0);
				if (j == 0)
				{
					c.EmitDelegate((Player self) =>
					{
						return LiquidLoader.OnPlayerSplash(LiquidID.Shimmer, self, true);
					});
				}
				else
				{
					c.EmitDelegate((Player self) =>
					{
						return LiquidLoader.OnPlayerSplash(LiquidID.Shimmer, self, false);
					});
				}
				c.EmitBrfalse(IL_0000[0 + (j * 4)]);
				#endregion

				#region Honey Splash Edit 
				//Honey 
				c.GotoNext(MoveType.After, i => i.MatchLdfld<Entity>("honeyWet"), i => i.MatchBrfalse(out IL_0000[1 + (j * 4)]), i => i.MatchLdcI4(0), i => i.MatchStloc(out _));
				c.EmitLdarg(0);
				if (j == 0)
				{
					c.EmitDelegate((Player self) =>
					{
						return LiquidLoader.OnPlayerSplash(LiquidID.Honey, self, true);
					});
				}
				else
				{
					c.EmitDelegate((Player self) =>
					{
						return LiquidLoader.OnPlayerSplash(LiquidID.Honey, self, false);
					});
				}
				c.EmitBrfalse(IL_0000[1 + (j * 4)]);
				#endregion

				#region Water and Modded Liquid Splash Edit 
				//Water
				c.GotoNext(MoveType.After, i => i.MatchPop(), i => i.MatchBr(out IL_0000[2 + (j * 4)]), i => i.MatchLdcI4(0), i => i.MatchStloc(out _));
				c.EmitLdarg(0);
				if (j == 0)
				{
					c.EmitDelegate((Player self) =>
					{
						for (int i = LiquidID.Count; i < LiquidLoader.LiquidCount; i++)
						{
							if (self.GetModPlayer<ModLiquidPlayer>().moddedWet[i - LiquidID.Count])
							{
								if (LiquidLoader.OnPlayerSplash(i, self, true))
								{
									LiquidLoader.GetLiquid(i).OnPlayerSplash(self, true);
								}
								return true;
							}
						}
						return false;
					});
				}
				else
				{
					c.EmitDelegate((Player self) =>
					{
						for (int i = LiquidID.Count; i < LiquidLoader.LiquidCount; i++)
						{
							if (self.GetModPlayer<ModLiquidPlayer>().moddedWet[i - LiquidID.Count])
							{
								if (LiquidLoader.OnPlayerSplash(i, self, false))
								{
									LiquidLoader.GetLiquid(i).OnPlayerSplash(self, false);
								}
								return true;
							}
						}
						return false;
					});
				}
				c.EmitBrtrue(IL_0000[2 + (j * 4)]);
				c.EmitLdarg(0);
				if (j == 0)
				{
					c.EmitDelegate((Player self) =>
					{
						return LiquidLoader.OnPlayerSplash(LiquidID.Water, self, true);
					});
				}
				else
				{
					c.EmitDelegate((Player self) =>
					{
						return LiquidLoader.OnPlayerSplash(LiquidID.Water, self, false);
					});
				}
				c.EmitBrfalse(IL_0000[2 + (j * 4)]);
				#endregion

				#region Lava Splash Edit
				//Lava
				c.GotoNext(MoveType.After, i => i.MatchBr(out IL_0000[3 + (j * 4)]), i => i.MatchLdcI4(0), i => i.MatchStloc(out _));
				c.EmitLdarg(0);
				if (j == 0)
				{
					c.EmitDelegate((Player self) =>
					{
						return LiquidLoader.OnPlayerSplash(LiquidID.Lava, self, true);
					});
				}
				else
				{
					c.EmitDelegate((Player self) =>
					{
						return LiquidLoader.OnPlayerSplash(LiquidID.Lava, self, false);
					});
				}
				c.EmitBrfalse(IL_0000[3 + (j * 4)]);
				#endregion
			}
			c.GotoNext(MoveType.After, i => i.MatchLdarg(0), i => i.MatchLdcI4(0), i => i.MatchStfld<Entity>("honeyWet"), i => i.MatchLdloc(out shimmer_var9));
			c.EmitLdarg(0);
			c.EmitDelegate((int unused, Player self) =>
			{
				LiquidCollision.WetCollision(self.position, self.width, self.height, out bool[] liquidIn);
				for (int i = LiquidID.Count; i < LiquidLoader.LiquidCount; i++)
				{
					if (!liquidIn[i])
					{
						self.GetModPlayer<ModLiquidPlayer>().moddedWet[i - LiquidID.Count] = false;
					}
				}
			});
			c.EmitLdloc(shimmer_var9);

			c.GotoNext(MoveType.After, i => i.MatchLdarg(0), i => i.MatchLdcI4(0), i => i.MatchStfld<Entity>("honeyWet"), i => i.MatchLdarg(0), i => i.MatchLdcI4(0), i => i.MatchStfld<Entity>("shimmerWet"));
			c.EmitLdarg(0);
			c.EmitDelegate((Player self) =>
			{
				for (int i = LiquidID.Count; i < LiquidLoader.LiquidCount; i++)
				{
					self.GetModPlayer<ModLiquidPlayer>().moddedWet[i - LiquidID.Count] = false;
				}
			});
		}
	}
}
