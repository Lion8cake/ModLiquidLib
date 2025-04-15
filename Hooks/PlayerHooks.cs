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
			ILLabel[] IL_0000 = new ILLabel[16];
			ILLabel[] IL_0000_2 = new ILLabel[2];
			for (int i = 0; i < IL_0000.Length; i++)
			{
				IL_0000[i] = c.DefineLabel();
			}
			for (int j = 0; j < IL_0000_2.Length; j++)
			{
				IL_0000_2[j] = c.DefineLabel();
			}
			int shimmerLoop_var = -1;
			int honeyLoop_var = -1;
			int waterLoop_var = -1;
			int lavaLoop_var = -1;
			int shimmer_var9 = -1;

			c.GotoNext(MoveType.After, i => i.MatchLdsfld<Collision>("shimmer"), i => i.MatchStloc(out _));
			c.EmitLdarg(0);
			c.EmitDelegate((Player self) =>
			{
				LiquidCollision.WetCollision(self.position, self.width, self.height, out bool[] liquidIn);
				self.GetModPlayer<ModLiquidPlayer>().moddedWet = liquidIn;
			});
			for (int i = 0; i < 2; i++)
			{
				#region Shimmer Splash Edit 
				//Shimmer
				c.GotoNext(MoveType.After, i => i.MatchLdfld<Entity>("shimmerWet"), i => i.MatchBrfalse(out _), i => i.MatchLdcI4(0), i => i.MatchStloc(out shimmerLoop_var), i => i.MatchBr(out _), i => i.MatchLdarg(0));
				if (i == 0)
				{
					c.EmitDelegate((Player self) =>
					{
						return BlockDustsIfNegitive(LiquidID.Shimmer, true);
					});
				}
				else
				{
					c.EmitDelegate((Player self) =>
					{
						return BlockDustsIfNegitive(LiquidID.Shimmer, false);
					});
				}
				c.EmitBrfalse(IL_0000[0 + (i * 8)]);
				c.EmitLdarg(0);

				c.GotoNext(MoveType.After, i => i.MatchAdd(), i => i.MatchLdcI4(24), i => i.MatchLdcI4(308));
				if (i == 0)
				{
					c.EmitDelegate((int defaultDustType) =>
					{
						return EditChangeBubbleDust(LiquidID.Honey, defaultDustType, true);
					});
				}
				else
				{
					c.EmitDelegate((int defaultDustType) =>
					{
						return EditChangeBubbleDust(LiquidID.Honey, defaultDustType, false);
					});
				}

				c.GotoNext(MoveType.Before, i => i.MatchLdloc(shimmerLoop_var), i => i.MatchLdcI4(1), i => i.MatchAdd(), i => i.MatchStloc(shimmerLoop_var));
				c.MarkLabel(IL_0000[0 + (i * 8)]);

				//Sound
				c.GotoNext(MoveType.Before, i => i.MatchLdcI4(19), i => i.MatchLdarg(0), i => i.MatchLdflda<Entity>("position"));
				c.EmitBr(IL_0000[4 + (i * 8)]);
				c.GotoNext(MoveType.After, i => i.MatchCall("Terraria.Audio.SoundEngine", "PlaySound"), i => i.MatchPop());
				c.MarkLabel(IL_0000[4 + (i * 8)]);
				c.EmitLdarg(0);
				if (i == 0)
				{
					c.EmitDelegate((Player self) =>
					{
						PlaySplashSoundUpdated(LiquidID.Shimmer, self, SoundID.Shimmer1, true);
					});
				}
				else
				{
					c.EmitDelegate((Player self) =>
					{
						PlaySplashSoundUpdated(LiquidID.Shimmer, self, SoundID.Shimmer2, false);
					});
				}
				#endregion

				#region Honey Splash Edit 
				//Honey 
				c.GotoNext(MoveType.After, i => i.MatchLdfld<Entity>("honeyWet"), i => i.MatchBrfalse(out _), i => i.MatchLdcI4(0), i => i.MatchStloc(out honeyLoop_var), i => i.MatchBr(out _), i => i.MatchLdarg(0));
				if (i == 0)
				{
					c.EmitDelegate((Player self) =>
					{
						return BlockDustsIfNegitive(LiquidID.Honey, true);
					});
				}
				else
				{
					c.EmitDelegate((Player self) =>
					{
						return BlockDustsIfNegitive(LiquidID.Honey, false);
					});
				}
				c.EmitBrfalse(IL_0000[1 + (i * 8)]);
				c.EmitLdarg(0);

				c.GotoNext(MoveType.After, i => i.MatchAdd(), i => i.MatchLdcI4(24), i => i.MatchLdcI4(152));
				if (i == 0)
				{
					c.EmitDelegate((int defaultDustType) =>
					{
						return EditChangeBubbleDust(LiquidID.Honey, defaultDustType, true);
					});
				}
				else
				{
					c.EmitDelegate((int defaultDustType) =>
					{
						return EditChangeBubbleDust(LiquidID.Honey, defaultDustType, false);
					});
				}

				c.GotoNext(MoveType.Before, i => i.MatchLdloc(honeyLoop_var), i => i.MatchLdcI4(1), i => i.MatchAdd(), i => i.MatchStloc(honeyLoop_var));
				c.MarkLabel(IL_0000[1 + (i * 8)]);

				//Sound
				c.GotoNext(MoveType.Before, i => i.MatchLdcI4(19), i => i.MatchLdarg(0), i => i.MatchLdflda<Entity>("position"));
				c.EmitBr(IL_0000[5 + (i * 8)]);
				c.GotoNext(MoveType.After, i => i.MatchCall("Terraria.Audio.SoundEngine", "PlaySound"), i => i.MatchPop());
				c.MarkLabel(IL_0000[5 + (i * 8)]);
				c.EmitLdarg(0);
				if (i == 0)
				{
					c.EmitDelegate((Player self) =>
					{
						PlaySplashSoundUpdated(LiquidID.Honey, self, SoundID.SplashWeak, true);
					});
				}
				else
				{
					c.EmitDelegate((Player self) =>
					{
						PlaySplashSoundUpdated(LiquidID.Honey, self, SoundID.SplashWeak, false);
					});
				}
				#endregion

				#region Water Splash Edit 
				//Water
				c.GotoNext(MoveType.After, i => i.MatchBr(out _), i => i.MatchLdcI4(0), i => i.MatchStloc(out waterLoop_var), i => i.MatchBr(out _), i => i.MatchLdarg(0));
				if (i == 0)
				{
					c.EmitDelegate((Player self) =>
					{
						return BlockDustsIfNegitive(LiquidID.Water, true);
					});
				}
				else
				{
					c.EmitDelegate((Player self) =>
					{
						return BlockDustsIfNegitive(LiquidID.Water, false);
					});
				}
				c.EmitBrfalse(IL_0000[2 + (i * 8)]);
				c.EmitLdarg(0);

				c.GotoNext(MoveType.After, i => i.MatchAdd(), i => i.MatchLdcI4(24), i => i.MatchCall<Dust>("dustWater"));
				if (i == 0)
				{
					c.EmitDelegate((int defaultDustType) =>
					{
						return EditChangeBubbleDust(LiquidID.Water, defaultDustType, true);
					});
				}
				else
				{
					c.EmitDelegate((int defaultDustType) =>
					{
						return EditChangeBubbleDust(LiquidID.Water, defaultDustType, false);
					});
				}

				c.GotoNext(MoveType.Before, i => i.MatchLdloc(waterLoop_var), i => i.MatchLdcI4(1), i => i.MatchAdd(), i => i.MatchStloc(waterLoop_var));
				c.MarkLabel(IL_0000[2 + (i * 8)]);

				//Sound
				c.GotoNext(MoveType.Before, i => i.MatchLdcI4(19), i => i.MatchLdarg(0), i => i.MatchLdflda<Entity>("position"));
				c.EmitBr(IL_0000[6 + (i * 8)]);
				c.GotoNext(MoveType.After, i => i.MatchCall("Terraria.Audio.SoundEngine", "PlaySound"), i => i.MatchPop());
				c.MarkLabel(IL_0000[6 + (i * 8)]);
				c.MarkLabel(IL_0000_2[i]);
				c.EmitLdarg(0);
				if (i == 0)
				{
					c.EmitDelegate((Player self) =>
					{
						PlaySplashSoundUpdated(LiquidID.Water, self, SoundID.Splash, true);
					});
				}
				else
				{
					c.EmitDelegate((Player self) =>
					{
						PlaySplashSoundUpdated(LiquidID.Water, self, SoundID.Splash, false);
					});
				}
				#endregion

				#region Lava Splash Edit
				//Lava
				c.GotoNext(MoveType.After, i => i.MatchBr(out _), i => i.MatchLdcI4(0), i => i.MatchStloc(out lavaLoop_var), i => i.MatchBr(out _), i => i.MatchLdarg(0));
				if (i == 0)
				{
					c.EmitDelegate((Player self) =>
					{
						return BlockDustsIfNegitive(LiquidID.Lava, true);
					});
				}
				else
				{
					c.EmitDelegate((Player self) =>
					{
						return BlockDustsIfNegitive(LiquidID.Lava, false);
					});
				}
				c.EmitBrfalse(IL_0000[3 + (i * 8)]);
				c.EmitLdarg(0);

				c.GotoNext(MoveType.After, i => i.MatchAdd(), i => i.MatchLdcI4(24), i => i.MatchLdcI4(35));
				if (i == 0)
				{
					c.EmitDelegate((int defaultDustType) =>
					{
						return EditChangeBubbleDust(LiquidID.Lava, defaultDustType, true);
					});
				}
				else
				{
					c.EmitDelegate((int defaultDustType) =>
					{
						return EditChangeBubbleDust(LiquidID.Lava, defaultDustType, false);
					});
				}

				c.GotoNext(MoveType.Before, i => i.MatchLdloc(lavaLoop_var), i => i.MatchLdcI4(1), i => i.MatchAdd(), i => i.MatchStloc(lavaLoop_var));
				c.MarkLabel(IL_0000[3 + (i * 8)]);

				//Sound
				c.GotoNext(MoveType.Before, i => i.MatchLdcI4(19), i => i.MatchLdarg(0), i => i.MatchLdflda<Entity>("position"));
				c.EmitBr(IL_0000[7 + (i * 8)]);
				c.GotoNext(MoveType.After, i => i.MatchCall("Terraria.Audio.SoundEngine", "PlaySound"), i => i.MatchPop());
				c.MarkLabel(IL_0000[7 + (i * 8)]);
				c.EmitLdarg(0);
				if (i == 0)
				{
					c.EmitDelegate((Player self) =>
					{
						PlaySplashSoundUpdated(LiquidID.Lava, self, SoundID.SplashWeak, true);
					});
				}
				else
				{
					c.EmitDelegate((Player self) =>
					{
						PlaySplashSoundUpdated(LiquidID.Lava, self, SoundID.SplashWeak, false);
					});
				}
				#endregion

				#region ModLiquid Splash Additions
				c.GotoPrev(MoveType.Before, i => i.MatchStloc(waterLoop_var), i => i.MatchBr(out _));
				c.EmitLdarg(0);
				c.EmitDelegate((int unused, Player self) =>
				{
					for (int i = LiquidID.Count; i < LiquidLoader.LiquidCount; i++)
					{
						if (self.GetModPlayer<ModLiquidPlayer>().moddedWet[i])
						{
							return true;
						}
					}
					return false;
				});
				c.EmitBrtrue(IL_0000_2[i]);
				c.EmitLdcI4(0);
				c.GotoLabel(IL_0000_2[i]);
				c.EmitLdarg(0);
				if (i == 0)
				{
					c.EmitDelegate((Player self) =>
					{
						for (int i = LiquidID.Count; i < LiquidLoader.LiquidCount; i++)
						{
							if (self.GetModPlayer<ModLiquidPlayer>().moddedWet[i])
							{
								ModLiquid modLiquid = LiquidLoader.GetLiquid(i);
								for (int num105 = 0; num105 < 50; num105++)
								{
									int num106 = Dust.NewDust(new Vector2(self.position.X - 6f, self.position.Y + (float)(self.height / 2) - 8f), self.width + 12, 24, modLiquid.SplashDustType);
									Main.dust[num106].velocity.Y -= 3f;
									Main.dust[num106].velocity.X *= 2.5f;
									Main.dust[num106].scale = 0.8f;
									Main.dust[num106].alpha = 100;
									Main.dust[num106].noGravity = true;
								}
								SoundEngine.PlaySound(modLiquid.SplashEnterSound, (int)self.position.X, (int)self.position.Y);
								break;
							}
						}
					});
				}
				else
				{
					c.EmitDelegate((Player self) =>
					{
						for (int i = LiquidID.Count; i < LiquidLoader.LiquidCount; i++)
						{
							if (self.GetModPlayer<ModLiquidPlayer>().moddedWet[i])
							{
								ModLiquid modLiquid = LiquidLoader.GetLiquid(i);
								for (int num13 = 0; num13 < 50; num13++)
								{
									int num14 = Dust.NewDust(new Vector2(self.position.X - 6f, self.position.Y + (float)(self.height / 2)), self.width + 12, 24, modLiquid.SplashDustType);
									Main.dust[num14].velocity.Y -= 4f;
									Main.dust[num14].velocity.X *= 2.5f;
									Main.dust[num14].scale = 0.8f;
									Main.dust[num14].alpha = 100;
									Main.dust[num14].noGravity = true;
								}
								SoundEngine.PlaySound(modLiquid.SplashExitSound, (int)self.position.X, (int)self.position.Y);
								break;
							}
						}
					});
				}
				#endregion
			}
			c.GotoNext(MoveType.After, i => i.MatchLdarg(0), i => i.MatchLdcI4(0), i => i.MatchStfld<Entity>("honeyWet"), i => i.MatchLdloc(out shimmer_var9));
			c.EmitLdarg(0);
			c.EmitDelegate((int unused, Player self) =>
			{
				LiquidCollision.WetCollision(self.position, self.width, self.height, out bool[] liquidIn);
				for (int i = 0; i < LiquidLoader.LiquidCount; i++)
				{
					if (!liquidIn[i])
					{
						self.GetModPlayer<ModLiquidPlayer>().moddedWet[i] = false;
					}
				}
			});
			c.EmitLdloc(shimmer_var9);

			c.GotoNext(MoveType.After, i => i.MatchLdarg(0), i => i.MatchLdcI4(0), i => i.MatchStfld<Entity>("honeyWet"), i => i.MatchLdarg(0), i => i.MatchLdcI4(0), i => i.MatchStfld<Entity>("shimmerWet"));
			c.EmitLdarg(0);
			c.EmitDelegate((Player self) =>
			{
				for (int i = 0; i < LiquidLoader.LiquidCount; i++)
				{
					self.GetModPlayer<ModLiquidPlayer>().moddedWet[i] = false;
				}
			});
		}

		private static bool BlockDustsIfNegitive(int liquidID, bool isEnter)
		{
			SoundStyle? nullSound = null;
			if (LiquidLoader.SplashDustType(liquidID, ref nullSound, isEnter) == -1)
			{
				return false;
			}
			return true;
		}

		private static int EditChangeBubbleDust(int liquidID, int defaultDustType, bool isEnter)
		{
			SoundStyle? nullSound = null;
			int? dustType = LiquidLoader.SplashDustType(liquidID, ref nullSound, isEnter);
			if (dustType != null)
			{
				return (int)dustType;
			}
			return defaultDustType;
		}

		private static void PlaySplashSoundUpdated(int liquidID, Player self, SoundStyle? splashSound, bool isEnter)
		{
			LiquidLoader.SplashDustType(liquidID, ref splashSound, isEnter);
			if (splashSound != null)
				SoundEngine.PlaySound(splashSound, (int)self.position.X, (int)self.position.Y);
		}
	}
}
