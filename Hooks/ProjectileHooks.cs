using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.ID;

namespace ModLiquidLib.Hooks
{
	public class ProjectileHooks
	{
		public static void UpdateProjectileSplash(ILContext il)
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
