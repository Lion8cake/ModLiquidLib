using Microsoft.Xna.Framework;
using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils.LiquidContent;
using MonoMod.Cil;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace ModLiquidLib.Hooks
{
	internal class NPCHooks
	{
		internal static void EditNPCLiquidMovement(ILContext il)
		{
			ILCursor c = new(il);
			c.GotoNext(MoveType.Before, i => i.MatchLdfld<Entity>("shimmerWet"));
			c.EmitDelegate((NPC self) =>
			{
				float gravity = self.gravity;
				float maxFallSpeed = self.maxFallSpeed;
				if (self.shimmerWet)
				{
					gravity = 0.15f;
					maxFallSpeed = 5.5f;
				}
				else if (self.honeyWet)
				{
					gravity = 0.1f;
					maxFallSpeed = 4f;
				}
				else
				{
					gravity = 0.2f;
					maxFallSpeed = 7f;
				}
				LiquidLoader.NPCGravityModifier(WetToLiquidID(self), self, ref gravity, ref maxFallSpeed);
				self.gravity = gravity;
				self.maxFallSpeed = maxFallSpeed;
			});
			c.EmitRet();
			c.EmitLdarg(0);
		}

		internal static int WetToLiquidID(NPC self)
		{
			int modLiquidID = -1;
			if (self.TryGetGlobalNPC(out ModLiquidNPC liquidNPC))
			{
				for (int i = LiquidLoader.LiquidCount - 1; i >= LiquidID.Count; i--)
				{
					if (liquidNPC.moddedWet[i - LiquidID.Count])
					{
						modLiquidID = i;
					}
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

		private static bool hasModdedWet(NPC self)
		{
			for (int i = LiquidLoader.LiquidCount - 1; i >= LiquidID.Count; i--)
			{
				if (self.GetGlobalNPC<ModLiquidNPC>().moddedWet[i - LiquidID.Count])
				{
					return true;
				}
			}
			return false;
		}

		internal static void ResetLiquidMovementMultipliersForDD2(On_NPC.orig_LazySetLiquidMovementDD2 orig, NPC self)
		{
			orig.Invoke(self);
			self.shimmerMovementSpeed = 1f;
		}

		internal static void UnwetNPCsAndUpdateWetVel(ILContext il)
		{
			ILCursor c = new(il);
			ILLabel IL_00f6 = null;
			int oldDryVel_varNum = -1;

			c.GotoNext(MoveType.After, i => i.MatchLdarg(0), i => i.MatchLdcI4(0), i => i.MatchStfld<Entity>("honeyWet"), i => i.MatchLdarg(0), i => i.MatchLdcI4(0), i => i.MatchStfld<Entity>("shimmerWet"));
			c.EmitLdarg(0);
			c.EmitDelegate((NPC self) =>
			{
				for (int i = LiquidID.Count; i < LiquidLoader.LiquidCount; i++)
				{
					self.GetGlobalNPC<ModLiquidNPC>().moddedWet[i - LiquidID.Count] = false;
				}
			});

			c.GotoNext(MoveType.Before, i => i.MatchLdarg(0), i => i.MatchLdfld<Entity>("shimmerWet"), i => i.MatchBrfalse(out _),
				i => i.MatchLdarg(0), i => i.MatchLdloc(out oldDryVel_varNum), i => i.MatchLdarg(0), i => i.MatchLdfld<NPC>("shimmerMovementSpeed"), i => i.MatchCall<NPC>("Collision_MoveWhileWet"), i => i.MatchBr(out IL_00f6));
			c.EmitLdarg(0);
			c.EmitLdloc(oldDryVel_varNum);
			c.EmitDelegate((NPC self, Vector2 oldDryVelocity) =>
			{
				if (LiquidLoader.NPCLiquidMovement(WetToLiquidID(self), self, oldDryVelocity))
				{
					if (hasModdedWet(self))
					{
						for (int i = LiquidLoader.LiquidCount - 1; i >= LiquidID.Count; i--)
						{
							ModLiquidNPC liquidNPC = self.GetGlobalNPC<ModLiquidNPC>();
							if (liquidNPC.moddedWet[i - LiquidID.Count])
							{
								self.Collision_MoveWhileWet(oldDryVelocity, liquidNPC.moddedLiquidMovementSpeed[i - LiquidID.Count]);
								break;
							}
						}
					}
					else if (self.shimmerWet)
					{
						self.Collision_MoveWhileWet(oldDryVelocity, self.shimmerMovementSpeed);
					}
					else if (self.honeyWet)
					{
						self.Collision_MoveWhileWet(oldDryVelocity, self.honeyMovementSpeed);
					}
					else if (self.lavaWet)
					{
						self.Collision_MoveWhileWet(oldDryVelocity, self.lavaMovementSpeed);
					}
					else
					{
						self.Collision_MoveWhileWet(oldDryVelocity, self.waterMovementSpeed);
					}
				}
			});
			c.EmitBr(IL_00f6);
		}

		internal static void UpdateNPCSplash(ILContext il)
		{
			ILCursor c = new(il);
			ILLabel[] IL_0000 = new ILLabel[8];

			c.GotoNext(MoveType.After, i => i.MatchCall<Collision>("WetCollision"), i => i.MatchStloc(out _));
			c.EmitLdarg(0);
			c.EmitDelegate((NPC self) =>
			{
				LiquidCollision.WetCollision(self.position, self.width, self.height, out bool[] liquidIn);
				for (int i = LiquidID.Count; i < LiquidLoader.LiquidCount; i++)
				{
					if (liquidIn[i])
					{
						self.GetGlobalNPC<ModLiquidNPC>().moddedWet[i - LiquidID.Count] = true;
					}
				}
			});
			for (int j = 0; j < 2; j++)
			{
				
				#region Shimmer Splash Edit 
				//Shimmer
				c.GotoNext(MoveType.After, i => i.MatchLdfld<Entity>("shimmerWet"), i => i.MatchBrfalse(out IL_0000[0 + (j * 4)]));
				c.EmitLdarg(0);
				if (j == 0)
				{
					c.EmitDelegate((NPC self) =>
					{
						return LiquidLoader.OnNPCSplash(LiquidID.Shimmer, self, true);
					});
				}
				else
				{
					c.EmitDelegate((NPC self) =>
					{
						return LiquidLoader.OnNPCSplash(LiquidID.Shimmer, self, false);
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
					c.EmitDelegate((NPC self) =>
					{
						return LiquidLoader.OnNPCSplash(LiquidID.Honey, self, true);
					});
				}
				else
				{
					c.EmitDelegate((NPC self) =>
					{
						return LiquidLoader.OnNPCSplash(LiquidID.Honey, self, false);
					});
				}
				c.EmitBrfalse(IL_0000[1 + (j * 4)]);
				#endregion
				
				#region Water and Modded Liquid Splash Edit 
				//Water
				c.GotoNext(MoveType.After, I => I.MatchLdarg(0), i => i.MatchLdfld<NPC>("type"), i => i.MatchLdcI4(625), i => i.MatchBeq(out IL_0000[2 + (j * 4)]));
				c.EmitLdarg(0);
				if (j == 0)
				{
					c.EmitDelegate((NPC self) =>
					{
						for (int i = LiquidID.Count; i < LiquidLoader.LiquidCount; i++)
						{
							if (self.GetGlobalNPC<ModLiquidNPC>().moddedWet[i - LiquidID.Count])
							{
								if (LiquidLoader.OnNPCSplash(i, self, true))
								{
									ModLiquid modLiquid = LiquidLoader.GetLiquid(i);
									if (modLiquid.OnNPCSplash(self, true))
									{
										if (modLiquid.SplashDustType >= 0)
										{
											for (int j = 0; j < 10; j++)
											{
												int dust = Dust.NewDust(new Vector2(self.position.X - 6f, self.position.Y + (self.height / 2) - 8f), self.width + 12, 24, modLiquid.SplashDustType);
												Main.dust[dust].velocity.Y -= 2f;
												Main.dust[dust].velocity.X *= 2.5f;
												Main.dust[dust].scale = 1.3f;
												Main.dust[dust].alpha = 100;
												Main.dust[dust].noGravity = true;
											}
										}
										if (self.aiStyle != NPCAIStyleID.Slime &&
												self.type != NPCID.BlueSlime && self.type != NPCID.MotherSlime && self.type != NPCID.IceSlime && self.type != NPCID.LavaSlime &&
												self.type != NPCID.Mouse &&
												self.aiStyle != NPCAIStyleID.GiantTortoise &&
												!self.noGravity)
										{
											SoundEngine.PlaySound(modLiquid.SplashSound, self.position);
										}
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
					c.EmitDelegate((NPC self) =>
					{
						for (int i = LiquidID.Count; i < LiquidLoader.LiquidCount; i++)
						{
							if (self.GetGlobalNPC<ModLiquidNPC>().moddedWet[i - LiquidID.Count])
							{
								if (LiquidLoader.OnNPCSplash(i, self, false))
								{
									ModLiquid modLiquid = LiquidLoader.GetLiquid(i);
									if (modLiquid.OnNPCSplash(self, false))
									{
										if (modLiquid.SplashDustType >= 0)
										{
											for (int j = 0; j < 10; j++)
											{
												int dust = Dust.NewDust(new Vector2(self.position.X - 6f, self.position.Y + (self.height / 2) - 8f), self.width + 12, 24, modLiquid.SplashDustType);
												Main.dust[dust].velocity.Y -= 2f;
												Main.dust[dust].velocity.X *= 2.5f;
												Main.dust[dust].scale = 1.3f;
												Main.dust[dust].alpha = 100;
												Main.dust[dust].noGravity = true;
											}
										}
										if (self.aiStyle != NPCAIStyleID.Slime &&
												self.type != NPCID.BlueSlime && self.type != NPCID.MotherSlime && self.type != NPCID.IceSlime && self.type != NPCID.LavaSlime &&
												self.type != NPCID.Mouse &&
												self.aiStyle != NPCAIStyleID.GiantTortoise &&
												!self.noGravity)
										{
											SoundEngine.PlaySound(modLiquid.SplashSound, self.position);
										}
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
					c.EmitDelegate((NPC self) =>
					{
						return LiquidLoader.OnNPCSplash(LiquidID.Water, self, true);
					});
				}
				else
				{
					c.EmitDelegate((NPC self) =>
					{
						return LiquidLoader.OnNPCSplash(LiquidID.Water, self, false);
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
					c.EmitDelegate((NPC self) =>
					{
						return LiquidLoader.OnNPCSplash(LiquidID.Lava, self, true);
					});
				}
				else
				{
					c.EmitDelegate((NPC self) =>
					{
						return LiquidLoader.OnNPCSplash(LiquidID.Lava, self, false);
					});
				}
				c.EmitBrfalse(IL_0000[3 + (j * 4)]);
				#endregion
			}
		}
	}
}
