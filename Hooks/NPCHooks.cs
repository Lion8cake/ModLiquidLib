using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils.LiquidContent;
using MonoMod.Cil;
using Terraria;
using Terraria.ID;

namespace ModLiquidLib.Hooks
{
	internal class NPCHooks
	{
		internal static void UnwetNPCs(ILContext il)
		{
			ILCursor c = new(il);
			c.GotoNext(MoveType.After, i => i.MatchLdarg(0), i => i.MatchLdcI4(0), i => i.MatchStfld<Entity>("honeyWet"), i => i.MatchLdarg(0), i => i.MatchLdcI4(0), i => i.MatchStfld<Entity>("shimmerWet"));
			c.EmitLdarg(0);
			c.EmitDelegate((NPC self) =>
			{
				for (int i = LiquidID.Count; i < LiquidLoader.LiquidCount; i++)
				{
					self.GetGlobalNPC<ModLiquidNPC>().moddedWet[i - LiquidID.Count] = false;
				}
			});
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
									LiquidLoader.GetLiquid(i).OnNPCSplash(self, true);
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
									LiquidLoader.GetLiquid(i).OnNPCSplash(self, false);
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
