using Microsoft.Xna.Framework;
using ModLiquidLib.ID;
using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;
using ModLiquidLib.Utils.LiquidContent;
using MonoMod.Cil;
using System;
using System.Diagnostics.Contracts;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace ModLiquidLib.Hooks
{
	internal class PlayerHooks
	{
		internal static void AllowCustomAccessories(ILContext il)
		{
			ILCursor c = new(il);
			int vector4_varNum = -1;

			c.GotoNext(MoveType.After, i => i.MatchCall<Collision>("WaterCollision"), i => i.MatchStloc(out vector4_varNum));
			c.EmitLdloca(vector4_varNum);
			c.EmitLdarg(0);
			c.EmitLdarg(1);
			c.EmitDelegate((ref Vector2 vector4, Player self, bool fallThrough) =>
			{
				vector4 = CollisionHooks.WaterCollision(self.position, vector4, self.width, self.height, self.GetModPlayer<ModLiquidPlayer>().canLiquidBeWalkedOn, fallThrough, fall2: false);
			});

			c.GotoNext(MoveType.After, i => i.MatchCall<Collision>("WaterCollision"), i => i.MatchStfld<Entity>("velocity"));
			c.EmitLdarg(0);
			c.EmitLdarg(0);
			c.EmitLdarg(1);
			c.EmitDelegate((Player self, bool fallThrough) =>
			{
				return CollisionHooks.WaterCollision(self.position, self.velocity, self.width, self.height, self.GetModPlayer<ModLiquidPlayer>().canLiquidBeWalkedOn, fallThrough, fall2: false);
			});
			c.EmitStfld(typeof(Entity).GetField(nameof(Entity.velocity)));

			c.Index = 0;
			for (int j = 0; j < 3; j++)
			{
				ILLabel IL_XXXX = null;
				c.GotoNext(MoveType.After, i => i.MatchLdarg(0), i => i.MatchLdfld<Player>("waterWalk"), i => i.MatchBrtrue(out IL_XXXX));
				c.EmitLdarg(0);
				c.EmitDelegate((Player self) =>
				{
					return self.GetModPlayer<ModLiquidPlayer>().canLiquidBeWalkedOn.Contains(true);
				});
				c.EmitBrtrue(IL_XXXX);
			}
		}

		internal static void BucketSupport(ILContext il)
		{
			ILCursor c = new(il);
			ILLabel IL_0414 = null;
			c.GotoNext(MoveType.After, i => i.MatchCall<Tile>("shimmer"));
			c.EmitDelegate((bool isShimmer) =>
			{
				return LiquidID_TLmod.Sets.CreateLiquidBucketItem[Main.tile[Player.tileTargetX, Player.tileTargetY].LiquidType] == -1;
			});

			c.GotoNext(MoveType.After, i => i.MatchBeq(out IL_0414), 
				i => i.MatchLdsflda<Main>("tile"), i => i.MatchLdsfld<Player>("tileTargetX"), i => i.MatchLdsfld<Player>("tileTargetY"),
				i => i.MatchCall<Tilemap>("get_Item"), i => i.MatchStloc(0));

			c.EmitLdarg(0);
			c.EmitLdarg(1);
			c.EmitDelegate((Player self, Item sItem) =>
			{
				Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];

				if (LiquidID_TLmod.Sets.CreateLiquidBucketItem[tile.LiquidType] != -1)
				{
					sItem.stack--;
					self.PutItemInInventoryFromItemUsage(LiquidID_TLmod.Sets.CreateLiquidBucketItem[tile.LiquidType], self.selectedItem);
					return true;
				}
				return false;
			});
			c.EmitBrtrue(IL_0414);
			c.EmitRet();
		}

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
				for (int i = 0; i < self.GetModPlayer<ModLiquidPlayer>().AdjLiquid.Length; i++)
				{
					self.GetModPlayer<ModLiquidPlayer>().OldAdjLiquid[i] = self.GetModPlayer<ModLiquidPlayer>().AdjLiquid[i];
					self.GetModPlayer<ModLiquidPlayer>().AdjLiquid[i] = false;
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
					self.GetModPlayer<ModLiquidPlayer>().AdjLiquid[Main.tile[j, k].LiquidType] = true;
				}
				else
				{
					for (int i = 0; i < LiquidLoader.LiquidCount; i++)
					{
						if (LiquidID_TLmod.Sets.CountsAsLiquidSource[Main.tile[j, k].TileType][i])
						{
							self.GetModPlayer<ModLiquidPlayer>().AdjLiquid[i] = true;
						}
					}
				}
				LiquidLoader.AdjLiquids(self, Main.tile[j, k].LiquidType);
			});

			c.GotoNext(MoveType.After, i => i.MatchRet(), i => i.MatchLdcI4(0), i => i.MatchStloc(out flag_var4));
			c.EmitLdarg(0);
			c.EmitLdloca(flag_var4);
			c.EmitDelegate((Player self, ref bool flag) =>
			{
				self.adjWater = self.GetModPlayer<ModLiquidPlayer>().AdjLiquid[0];
				self.adjLava = self.GetModPlayer<ModLiquidPlayer>().AdjLiquid[1];
				self.adjHoney = self.GetModPlayer<ModLiquidPlayer>().AdjLiquid[2];
				self.adjShimmer = self.GetModPlayer<ModLiquidPlayer>().AdjLiquid[3];
				for (int l = 0; l < self.GetModPlayer<ModLiquidPlayer>().AdjLiquid.Length; l++)
				{
					if (self.GetModPlayer<ModLiquidPlayer>().OldAdjLiquid[l] != self.GetModPlayer<ModLiquidPlayer>().AdjLiquid[l])
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

		internal static void CanPlayerEmitDrowningBubbles(ILContext il)
		{
			ILCursor c = new(il);
			ILLabel IL_03f2 = null;
			int flag_var0 = -1;

			c.GotoNext(MoveType.After, i => i.MatchStloc(out flag_var0), i => i.MatchLdsfld<Main>("myPlayer"), i => i.MatchLdarg(0), i => i.MatchLdfld<Entity>("whoAmI"));
			c.EmitLdarg(0);
			c.EmitLdloca(flag_var0);
			c.EmitDelegate((bool myPlayerCheck, Player self, ref bool flag) =>
			{
				int? liquid = LiquidIDofLiquidWet(self);
				if (liquid != null)
				{
					LiquidLoader.CanPlayerDrown((int)liquid, self, ref flag);
				}
				return myPlayerCheck;
			});

			c.GotoNext(MoveType.After, i => i.MatchLdarg(0), i => i.MatchLdfld<Entity>("lavaWet"));
			c.EmitDelegate((bool isLavaWet) =>
			{
				bool? flag = LiquidLoader.PlayersEmitBreathBubbles(LiquidID.Lava);
				if (flag == null)
				{
					return isLavaWet;
				}
				else
				{
					return !(bool)flag;
				}
			});

			c.GotoNext(MoveType.After, i => i.MatchLdarg(0), i => i.MatchLdfld<Entity>("honeyWet"));
			c.EmitDelegate((bool isHoneyWet) =>
			{
				bool? flag = LiquidLoader.PlayersEmitBreathBubbles(LiquidID.Honey);
				if (flag == null)
				{
					return isHoneyWet;
				}
				else
				{
					return !(bool)flag;
				}
			});

			c.GotoNext(MoveType.After, i => i.MatchBrtrue(out IL_03f2));
			c.EmitLdarg(0);
			c.EmitDelegate((Player self) =>
			{
				for (int i = LiquidLoader.LiquidCount - 1; i >= 0; i--)
				{
					if (i == LiquidID.Lava || i == LiquidID.Honey)
					{
						continue;
					}
					bool? flag = LiquidLoader.PlayersEmitBreathBubbles(i);
					if (flag != null)
					{
						if (!(bool)flag)
						{
							if (i >= LiquidID.Count)
							{
								if (self.GetModPlayer<ModLiquidPlayer>().moddedWet[i - LiquidID.Count])
								{
									return true;
								}
							}
							else if (i == LiquidID.Shimmer)
							{
								if (self.shimmerWet)
								{
									return true;
								}
							}
							else if (i == LiquidID.Water)
							{
								if (self.wet && !self.lavaWet && !self.honeyWet && !self.shimmerWet && !self.GetModPlayer<ModLiquidPlayer>().moddedWet.Contains(true))
								{
									return true;
								}
							}
						}
					}

					if (i >= LiquidID.Count)
					{
						ModLiquid modLiquid = LiquidLoader.GetLiquid(i);
						if (modLiquid != null)
						{
							if (!modLiquid.PlayersEmitBreathBubbles)
							{
								if (self.GetModPlayer<ModLiquidPlayer>().moddedWet[i - LiquidID.Count])
								{
									return true;
								}
							}
						}
					}
				}
				return false;
			});
			c.EmitBrtrue(IL_03f2);
		}

		private static int? LiquidIDofLiquidWet(Player self)
		{
			for (int i = 0; i < LiquidLoader.LiquidCount; i++)
			{
				if (i == LiquidID.Water)
				{
					if (self.wet && !self.lavaWet && !self.honeyWet && !self.shimmerWet && !self.GetModPlayer<ModLiquidPlayer>().moddedWet.Contains(true))
					{
						return LiquidID.Water;
					}
				}
				else if (i == LiquidID.Lava)
				{
					if (self.lavaWet)
					{
						return LiquidID.Lava;
					}
				}
				else if (i == LiquidID.Honey)
				{
					if (self.honeyWet)
					{
						return LiquidID.Honey;
					}
				}
				else if (i == LiquidID.Shimmer)
				{
					if (self.shimmerWet)
					{
						return LiquidID.Shimmer;
					}
				}
				else if (i >= LiquidID.Count)
				{
					if (self.GetModPlayer<ModLiquidPlayer>().moddedWet[i - LiquidID.Count])
					{
						return i;
					}
				}
			}
			return null;
		} 

		internal static void PlayerLiquidCollision(ILContext il)
		{
			ILCursor c = new(il);
			ILLabel[] IL_0000 = new ILLabel[8];
			ILLabel IL_01fa = null;
			ILLabel IL_82ba = null;
			ILLabel IL_838e = null;
			int shimmer_var9 = -1;
			int fallThrough_var14 = -1;
			int ignorePlats_var13 = -1;

			c.GotoNext(MoveType.After, i => i.MatchBrfalse(out IL_01fa), i => i.MatchLdarg(0), i => i.MatchLdfld<Entity>("honeyWet"));
			c.GotoPrev(MoveType.Before, i => i.MatchLdfld<Entity>("shimmerWet"), i => i.MatchBrtrue(out _), 
				i => i.MatchLdarg(0), i => i.MatchLdfld<Player>("shimmering"), i => i.MatchBrfalse(out _),
				i => i.MatchLdarg(0), i => i.MatchLdfld<Player>("shimmering"), i => i.MatchBrfalse(out _),
				i => i.MatchLdarg(0), i => i.MatchLdarg(0), i => i.MatchLdfld<Player>("gravity"));
			c.EmitDelegate((Player self) => 
			{
				if (self.shimmerWet || self.shimmering)
				{
					if (self.shimmering)
					{
						self.gravity *= 0.9f;
						self.maxFallSpeed *= 0.9f;
					}
					else
					{
						self.gravity = 0.15f;
						Player.jumpHeight = 23;
						Player.jumpSpeed = 5.51f;
					}
				}
				else if (self.wet)
				{
					if (self.honeyWet)
					{
						self.gravity = 0.1f;
						self.maxFallSpeed = 3f;
					}
					else if (self.merman)
					{
						self.gravity = 0.3f;
						self.maxFallSpeed = 7f;
					}
					else if (self.trident && !self.lavaWet)
					{
						self.gravity = 0.25f;
						self.maxFallSpeed = 6f;
						Player.jumpHeight = 25;
						Player.jumpSpeed = 5.51f;
						if (self.controlUp)
						{
							self.gravity = 0.1f;
							self.maxFallSpeed = 2f;
						}
					}
					else
					{
						self.gravity = 0.2f;
						self.maxFallSpeed = 5f;
						Player.jumpHeight = 30;
						Player.jumpSpeed = 6.01f;
					}
				}
				LiquidLoader.PlayerGravityModifier(WetToLiquidID(self), self, ref self.gravity, ref self.maxFallSpeed, ref Player.jumpHeight, ref Player.jumpSpeed);
			});
			c.EmitBr(IL_01fa);
			c.EmitLdarg(0);

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
									ModLiquid modLiquid = LiquidLoader.GetLiquid(i);
									if (modLiquid.OnPlayerSplash(self, true))
									{
										if (modLiquid.SplashDustType >= 0)
										{
											for (int j = 0; j < 20; j++)
											{
												int dust = Dust.NewDust(new Vector2(self.position.X - 6f, self.position.Y + (self.height / 2) - 8f), self.width + 12, 24, modLiquid.SplashDustType);
												Main.dust[dust].velocity.Y -= 2f;
												Main.dust[dust].velocity.X *= 2.5f;
												Main.dust[dust].scale = 1.3f;
												Main.dust[dust].alpha = 100;
												Main.dust[dust].noGravity = true;
											}
										}
										SoundEngine.PlaySound(modLiquid.SplashSound, self.position);
									}
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
									ModLiquid modLiquid = LiquidLoader.GetLiquid(i);
									if (modLiquid.OnPlayerSplash(self, false))
									{
										if (modLiquid.SplashDustType >= 0)
										{
											for (int j = 0; j < 20; j++)
											{
												int dust = Dust.NewDust(new Vector2(self.position.X - 6f, self.position.Y + (self.height / 2) - 8f), self.width + 12, 24, modLiquid.SplashDustType);
												Main.dust[dust].velocity.Y -= 2f;
												Main.dust[dust].velocity.X *= 2.5f;
												Main.dust[dust].scale = 1.3f;
												Main.dust[dust].alpha = 100;
												Main.dust[dust].noGravity = true;
											}
										}
										SoundEngine.PlaySound(modLiquid.SplashSound, self.position);
									}
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

			c.GotoNext(MoveType.After, i => i.MatchLdarg(0), i => i.MatchLdfld<Player>("trident"), i => i.MatchBrtrue(out IL_82ba), i => i.MatchLdarg(0), i => i.MatchLdloc(out fallThrough_var14), i => i.MatchLdloc(out ignorePlats_var13), i => i.MatchCall<Player>("WaterCollision"), i => i.MatchBr(out IL_838e));
			c.GotoPrev(MoveType.Before, i => i.MatchLdfld<Entity>("shimmerWet"), i => i.MatchBrtrue(out _), i => i.MatchLdarg(0), i => i.MatchLdfld<Player>("shimmering"), i => i.MatchBrfalse(out _));
			c.EmitLdloc(ignorePlats_var13);
			c.EmitLdloc(fallThrough_var14);
			c.EmitDelegate((Player self, bool ignorePlats, bool fallThrough) =>
			{
				if (self.shimmering)
				{
					if (LiquidLoader.PlayerLiquidMovement(LiquidID.Shimmer, self, fallThrough, ignorePlats))
					{
						self.ShimmerCollision(fallThrough, ignorePlats, true);
					}
					return true;
				}
				else if (self.wet)
				{
					if (LiquidLoader.PlayerLiquidMovement(WetToLiquidID(self), self, fallThrough, ignorePlats))
					{
						if (hasModdedWet(self))
						{
							ModLiquidCollision(self, WetToLiquidID(self), fallThrough, ignorePlats);
						}
						else if (self.shimmerWet || self.shimmering)
						{
							self.ShimmerCollision(fallThrough, ignorePlats, self.shimmering);
						}
						else if (self.honeyWet && !self.ignoreWater)
						{
							self.HoneyCollision(fallThrough, ignorePlats);
						}
						else if (!self.merman && !self.ignoreWater && !self.trident)
						{
							self.WaterCollision(fallThrough, ignorePlats);
						}
						else
						{
							return false;
						}
					}
					return true;
				}
				return false;
			});
			c.EmitBrtrue(IL_838e);
			c.EmitBr(IL_82ba);
			c.EmitLdarg(0);
		}

		public static void ModLiquidCollision(Player player, int liquidID, bool fallThrough, bool ignorePlats)
		{
			int num = (!player.onTrack) ? player.height : (player.height - 20);
			Vector2 vector = player.velocity;
			player.velocity = Collision.TileCollision(player.position, player.velocity, player.width, num, fallThrough, ignorePlats, (int)player.gravDir);
			Vector2 vector2 = player.velocity * LiquidLoader.GetLiquid(liquidID).PlayerMovementMultiplier;
			if (player.velocity.X != vector.X)
			{
				vector2.X = player.velocity.X;
			}
			if (player.velocity.Y != vector.Y)
			{
				vector2.Y = player.velocity.Y;
			}
			player.position += vector2;
			player.TryFloatingInFluid();
		}

		internal static int WetToLiquidID(Player self)
		{
			int modLiquidID = -1;
			for (int i = LiquidLoader.LiquidCount - 1; i >= LiquidID.Count; i--)
			{
				if (self.GetModPlayer<ModLiquidPlayer>().moddedWet[i - LiquidID.Count])
				{
					modLiquidID = i;
				}
			}
			if (modLiquidID == -1)
			{
				if (self.shimmerWet)
				{
					modLiquidID = LiquidID.Shimmer;
				}
				else if (self.honeyWet)
				{
					modLiquidID = LiquidID.Honey;
				}
				else if (self.lavaWet)
				{
					modLiquidID = LiquidID.Lava;
				}
				else
				{
					modLiquidID = LiquidID.Water;
				}
			}
			return modLiquidID;
		}

		private static bool hasModdedWet(Player self)
		{
			for (int i = LiquidLoader.LiquidCount - 1; i >= LiquidID.Count; i--)
			{
				if (self.GetModPlayer<ModLiquidPlayer>().moddedWet[i - LiquidID.Count])
				{
					return true;
				}
			}
			return false;
		}
	}
}
