using Microsoft.Xna.Framework;
using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils.LiquidContent;
using MonoMod.Cil;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace ModLiquidLib.Hooks
{
	internal class ItemHooks
	{
		internal static void UpdateItemSplash(ILContext il)
		{
			ILCursor c = new(il);
			ILLabel[] IL_0000 = new ILLabel[8];

			c.GotoNext(MoveType.After, i => i.MatchCall<Collision>("LavaCollision"), i => i.MatchStloc(out _));
			c.EmitLdarg(0);
			c.EmitDelegate((Item self) =>
			{
				LiquidCollision.WetCollision(self.position, self.width, self.height, out bool[] liquidIn);
				for (int i = LiquidID.Count; i < LiquidLoader.LiquidCount; i++)
				{
					if (liquidIn[i])
					{
						if (self.TryGetGlobalItem(out ModLiquidItem liquidItem))
						{
							liquidItem.moddedWet[i - LiquidID.Count] = true;
						}
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
					c.EmitDelegate((Item self) =>
					{
						return LiquidLoader.OnItemSplash(LiquidID.Shimmer, self, true);
					});
				}
				else
				{
					c.EmitDelegate((Item self) =>
					{
						return LiquidLoader.OnItemSplash(LiquidID.Shimmer, self, false);
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
					c.EmitDelegate((Item self) =>
					{
						return LiquidLoader.OnItemSplash(LiquidID.Honey, self, true);
					});
				}
				else
				{
					c.EmitDelegate((Item self) =>
					{
						return LiquidLoader.OnItemSplash(LiquidID.Honey, self, false);
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
					c.EmitDelegate((Item self) =>
					{
						for (int i = LiquidID.Count; i < LiquidLoader.LiquidCount; i++)
						{
							if (self.TryGetGlobalItem(out ModLiquidItem liquidItem))
							{
								if (liquidItem.moddedWet[i - LiquidID.Count])
								{
									if (LiquidLoader.OnItemSplash(i, self, true))
									{
										ModLiquid modLiquid = LiquidLoader.GetLiquid(i);
										if (modLiquid.OnItemSplash(self, true))
										{
											if (modLiquid.SplashDustType >= 0)
											{
												for (int j = 0; j < 5; j++)
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
						}
						return false;
					});
				}
				else
				{
					c.EmitDelegate((Item self) =>
					{
						for (int i = LiquidID.Count; i < LiquidLoader.LiquidCount; i++)
						{
							if (self.TryGetGlobalItem(out ModLiquidItem liquidItem))
							{
								if (liquidItem.moddedWet[i - LiquidID.Count])
								{
									if (LiquidLoader.OnItemSplash(i, self, false))
									{
										ModLiquid modLiquid = LiquidLoader.GetLiquid(i);
										if (modLiquid.OnItemSplash(self, false))
										{
											if (modLiquid.SplashDustType >= 0)
											{
												for (int j = 0; j < 5; j++)
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
						}
						return false;
					});
				}
				c.EmitBrtrue(IL_0000[2 + (j * 4)]);
				c.EmitLdarg(0);
				if (j == 0)
				{
					c.EmitDelegate((Item self) =>
					{
						return LiquidLoader.OnItemSplash(LiquidID.Water, self, true);
					});
				}
				else
				{
					c.EmitDelegate((Item self) =>
					{
						return LiquidLoader.OnItemSplash(LiquidID.Water, self, false);
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
					c.EmitDelegate((Item self) =>
					{
						return LiquidLoader.OnItemSplash(LiquidID.Lava, self, true);
					});
				}
				else
				{
					c.EmitDelegate((Item self) =>
					{
						return LiquidLoader.OnItemSplash(LiquidID.Lava, self, false);
					});
				}
				c.EmitBrfalse(IL_0000[3 + (j * 4)]);
				#endregion
			}

			c.GotoNext(MoveType.After, i => i.MatchLdarg(0), i => i.MatchLdcI4(0), i => i.MatchStfld<Entity>("honeyWet"), i => i.MatchLdarg(0), i => i.MatchLdcI4(0), i => i.MatchStfld<Entity>("shimmerWet"));
			c.EmitLdarg(0);
			c.EmitDelegate((Item self) =>
			{
				for (int i = LiquidID.Count; i < LiquidLoader.LiquidCount; i++)
				{
					if (self.TryGetGlobalItem(out ModLiquidItem liquidItem))
					{
						liquidItem.moddedWet[i - LiquidID.Count] = false;
					}
				}
			});

			//STORY TIME!!!!
			//so aparently, since this is the only way items can become unwet, this can cause items that have been previously marked as 'wet' splash apon being created again such as items coming out of player inventories
		}
	}
}
