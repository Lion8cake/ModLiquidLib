using Microsoft.Xna.Framework;
using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.ID;

namespace ModLiquidLib.Hooks
{
	internal class ProjectileHooks
	{
		internal static void BlockOtherWaterSplashes(On_Projectile.orig_AI_061_FishingBobber_DoASplash orig, Projectile self)
		{
			for (int i = 0; i < LiquidLoader.LiquidCount; i++)
			{
				if (i != LiquidID.Water)
				{
					if (i == LiquidID.Lava)
					{
						if (self.lavaWet)
							LiquidLoader.OnFishingBobberSplash(i, self);
					}
					else if (i == LiquidID.Honey)
					{
						if (self.honeyWet)
							LiquidLoader.OnFishingBobberSplash(i, self);
					}
					else if (i == LiquidID.Shimmer)
					{
						if (self.shimmerWet)
							LiquidLoader.OnFishingBobberSplash(i, self);
					}
					else if (i >= LiquidID.Count)
					{
						if (self.GetGlobalProjectile<ModLiquidProjectile>().moddedWet[i - LiquidID.Count])
						{
							LiquidLoader.OnFishingBobberSplash(i, self);
						}
					}
				}
			}
			if (self.wet && !isModdedWetProj(self, out _) && !self.shimmerWet && !self.honeyWet && !self.lavaWet)
			{
				if (LiquidLoader.OnFishingBobberSplash(LiquidID.Water, self))
				{
					orig.Invoke(self);
				}
			}
		}

		private static bool isModdedWetProj(Projectile self, out int modLiquidID)
		{
			modLiquidID = 0;
			for (int i = LiquidID.Count; i < LiquidLoader.LiquidCount; i++)
			{
				if (self.GetGlobalProjectile<ModLiquidProjectile>().moddedWet[i - LiquidID.Count])
				{
					modLiquidID = i;
					return true;
				}
			}
			return false;
		}

		internal static void FishingPondLiquidEdits(ILContext il)
		{
			ILCursor c = new(il);
			VariableDefinition poolLiquid_varDef = new(il.Import(typeof(bool[])));
			il.Body.Variables.Add(poolLiquid_varDef);
			ILLabel IL_0074 = null;
			ILLabel IL_0000 = c.DefineLabel();
			int tileTemp_varNum = -1;
			c.GotoNext(i => i.MatchBrfalse(out IL_0074), i => i.MatchLdarg(3));
			c.Index = 0;

			c.EmitDelegate(() =>
			{
				return new bool[LiquidLoader.LiquidCount];
			});
			c.EmitStloc(poolLiquid_varDef);

			c.GotoNext(MoveType.Before, i => i.MatchLdloca(out tileTemp_varNum), i => i.MatchCall<Tile>("lava"), i => i.MatchBrfalse(out _));
			c.EmitLdloc(tileTemp_varNum);
			c.EmitDelegate((Tile tile) =>
			{
				return tile.LiquidType == LiquidID.Water;
			});
			c.EmitBrfalse(IL_0000);
			c.EmitLdloca(poolLiquid_varDef);
			c.EmitDelegate((ref bool[] liquidPool) =>
			{
				liquidPool[LiquidID.Water] = true;
			});
			c.EmitBr(IL_0074);
			c.MarkLabel(IL_0000);

			c.GotoNext(MoveType.After, i => i.MatchLdloca(tileTemp_varNum), i => i.MatchCall<Tile>("honey"));
			c.EmitLdloc(tileTemp_varNum);
			c.EmitLdloca(poolLiquid_varDef);
			c.EmitLdarg(3);
			c.EmitLdarg(2);
			c.EmitLdindU1();
			c.EmitDelegate((bool liquidIsHoney, Tile tile, ref bool[] liquidPool, ref bool honey, bool lava) =>
			{
				if (liquidIsHoney)
				{
					honey = liquidPool[LiquidID.Honey] = true;
				}
				else if (tile.LiquidType == LiquidID.Shimmer)
				{
					liquidPool[LiquidID.Shimmer] = true;
				}
				else
				{
					for (int i = LiquidID.Count; i < LiquidLoader.LiquidCount; i++)
					{
						if (tile.LiquidType == i)
						{
							liquidPool[i] = true;
						}
					}
					liquidPool[LiquidID.Lava] = lava;
				}
				return false;
			});

			c.GotoNext(MoveType.After, i => i.MatchLdarg(3), i => i.MatchLdindU1());
			c.EmitLdloc(poolLiquid_varDef);
			c.EmitLdarg(4);
			c.EmitDelegate((bool honey, bool[] liquidPool, ref int numWaters) =>
			{
				float poolMultiplier = 1f;
				if (honey)
				{
					poolMultiplier = 1.5f;
				}
				for (int i = 0; i < liquidPool.Length; i++)
				{
					if (liquidPool[i])
					{
						LiquidLoader.LiquidFishingPoolSizeMulitplier(i, ref poolMultiplier);
					}
				}
				numWaters = (int)((double)numWaters * poolMultiplier);
				return false;
			});
		}

		internal static void StopShimmerShimmeringThePlayer(ILContext il)
		{
			ILCursor c = new ILCursor(il);
			ILLabel IL_0859 = null;
			ILLabel IL_0801 = null;
			for (int j = 0; j < 2; j++) //remove lava and honey limitation
			{
				c.GotoNext(MoveType.Before, i => i.MatchBrtrue(out _), i => i.MatchLdarg(0), i => i.MatchLdfld<Entity>("honeyWet"), i => i.MatchBrtrue(out _),
					i => i.MatchLdarg(0), i => i.MatchCall<Projectile>("AI_061_FishingBobber_DoASplash"));
				c.EmitDelegate((bool lava) =>
				{
					return false;
				});
				c.GotoNext(MoveType.Before, i => i.MatchBrtrue(out _), i => i.MatchLdarg(0), i => i.MatchCall<Projectile>("AI_061_FishingBobber_DoASplash"));
				c.EmitDelegate((bool honey) =>
				{
					return false;
				});
			}

			c.GotoNext(MoveType.After, i => i.MatchLdarg(0), i => i.MatchLdfld<Entity>("shimmerWet"));
			c.EmitDelegate((bool shimmerWet) =>
			{
				return shimmerWet && !LiquidLoader.AllowFishingInShimmer();
			});

			c.GotoNext(MoveType.Before, i => i.MatchLdarg(0), i => i.MatchLdflda<Entity>("velocity"), i => i.MatchLdfld<Vector2>("Y"), i => i.MatchLdcR4(0.0f), i => i.MatchBeq(out IL_0801), 
				i => i.MatchLdarg(0), i => i.MatchLdfld<Entity>("honeyWet"), i => i.MatchBrfalse(out IL_0859));
			c.EmitLdarg(0);
			c.EmitDelegate((Projectile self) =>
			{
				return Math.Abs(self.velocity.Y) <= 0.01f && self.velocity.Y == self.oldVelocity.Y;
			});
			c.EmitBrfalse(IL_0859);
			c.EmitBr(IL_0801);
		}

		internal static void ShimmerFishingItemFix(On_Projectile.orig_FishingCheck_RollItemDrop orig, Projectile self, ref Terraria.DataStructures.FishingAttempt fisher)
		{
			if (Main.tile[fisher.X, fisher.Y].LiquidType == LiquidID.Shimmer)
			{
				return;
			}
			orig.Invoke(self, ref fisher);
		}

		internal static void ShimmerFishingFix(On_Projectile.orig_FishingCheck_RollEnemySpawns orig, Projectile self, ref Terraria.DataStructures.FishingAttempt fisher)
		{
			if (Main.tile[fisher.X, fisher.Y].LiquidType == LiquidID.Shimmer)
			{
				return;
			}
			orig.Invoke(self, ref fisher);
		}

		internal static void UpdateProjectileSplash(ILContext il)
		{
			ILCursor c = new(il);
			ILLabel[] IL_0000 = new ILLabel[8];

			c.GotoNext(MoveType.After, i => i.MatchCall<Collision>("LavaCollision"), i => i.MatchStloc(out _));
			c.EmitLdarg(0);
			c.EmitDelegate((Projectile self) =>
			{
				LiquidCollision.WetCollision(self.position, self.width, self.height, out bool[] liquidIn);
				for (int i = LiquidID.Count; i < LiquidLoader.LiquidCount; i++)
				{
					if (liquidIn[i])
					{
						self.GetGlobalProjectile<ModLiquidProjectile>().moddedWet[i - LiquidID.Count] = true;
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
					c.EmitDelegate((Projectile self) =>
					{
						return LiquidLoader.OnProjectileSplash(LiquidID.Shimmer, self, true);
					});
				}
				else
				{
					c.EmitDelegate((Projectile self) =>
					{
						return LiquidLoader.OnProjectileSplash(LiquidID.Shimmer, self, false);
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
					c.EmitDelegate((Projectile self) =>
					{
						return LiquidLoader.OnProjectileSplash(LiquidID.Honey, self, true);
					});
				}
				else
				{
					c.EmitDelegate((Projectile self) =>
					{
						return LiquidLoader.OnProjectileSplash(LiquidID.Honey, self, false);
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
					c.EmitDelegate((Projectile self) =>
					{
						for (int i = LiquidID.Count; i < LiquidLoader.LiquidCount; i++)
						{
							if (self.GetGlobalProjectile<ModLiquidProjectile>().moddedWet[i - LiquidID.Count])
							{
								if (LiquidLoader.OnProjectileSplash(i, self, true))
								{
									LiquidLoader.GetLiquid(i).OnProjectileSplash(self, true);
								}
								return true;
							}
						}
						return false;
					});
				}
				else
				{
					c.EmitDelegate((Projectile self) =>
					{
						for (int i = LiquidID.Count; i < LiquidLoader.LiquidCount; i++)
						{
							if (self.GetGlobalProjectile<ModLiquidProjectile>().moddedWet[i - LiquidID.Count])
							{
								if (LiquidLoader.OnProjectileSplash(i, self, false))
								{
									LiquidLoader.GetLiquid(i).OnProjectileSplash(self, false);
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
					c.EmitDelegate((Projectile self) =>
					{
						return LiquidLoader.OnProjectileSplash(LiquidID.Water, self, true);
					});
				}
				else
				{
					c.EmitDelegate((Projectile self) =>
					{
						return LiquidLoader.OnProjectileSplash(LiquidID.Water, self, false);
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
					c.EmitDelegate((Projectile self) =>
					{
						return LiquidLoader.OnProjectileSplash(LiquidID.Lava, self, true);
					});
				}
				else
				{
					c.EmitDelegate((Projectile self) =>
					{
						return LiquidLoader.OnProjectileSplash(LiquidID.Lava, self, false);
					});
				}
				c.EmitBrfalse(IL_0000[3 + (j * 4)]);
				#endregion
			}

			c.GotoNext(MoveType.After, i => i.MatchLdarg(0), i => i.MatchLdcI4(0), i => i.MatchStfld<Entity>("honeyWet"), i => i.MatchLdarg(0), i => i.MatchLdcI4(0), i => i.MatchStfld<Entity>("shimmerWet"));
			c.EmitLdarg(0);
			c.EmitDelegate((Projectile self) =>
			{
				for (int i = LiquidID.Count; i < LiquidLoader.LiquidCount; i++)
				{
					self.GetGlobalProjectile<ModLiquidProjectile>().moddedWet[i - LiquidID.Count] = false;
				}
			});
		}
	}
}
