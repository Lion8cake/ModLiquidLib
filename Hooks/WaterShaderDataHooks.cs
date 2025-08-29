using Microsoft.VisualBasic;
using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;
using ModLiquidLib.Utils.LiquidContent;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.GameContent.Shaders;
using Terraria.ID;

namespace ModLiquidLib.Hooks
{
	internal class WaterShaderDataHooks
	{
		internal static void EditWaveSize(ILContext il)
		{
			ILCursor c = new(il);
			VariableDefinition shimmerFlag_varDef = new(il.Import(typeof(bool)));
			il.Body.Variables.Add(shimmerFlag_varDef);

			ILLabel IL_0234 = null;
			ILLabel IL_04f7 = null;
			ILLabel IL_0772 = null;
			ILLabel IL_087e = null;
			ILLabel IL_07cf = null;

			int npc_varNum = -1;
			int num_varNum = -1;
			int player_varNum = -1;
			int num3_varNum = -1;
			int projFlag_varNum = -1;
			int projFlag2_varNum = -1;
			int projectile_varNum = -1;
			int num7_varNum = -1;

			c.GotoNext(MoveType.After, i => i.MatchLdloc(out npc_varNum), i => i.MatchLdfld<Entity>("lavaWet"), i => i.MatchBrfalse(out IL_0234), i => i.MatchLdloc(out num_varNum), i => i.MatchLdcR4(0.3f), i => i.MatchMul());
			c.GotoPrev(MoveType.Before, i => i.MatchLdarg(0), i => i.MatchLdfld<WaterShaderData>("_useViscosityFilter"));
			c.EmitLdarg(0);
			c.EmitLdloc(npc_varNum);
			c.EmitLdloca(num_varNum);
			c.EmitDelegate((WaterShaderData self, NPC npc, ref float num) =>
			{
				EntityMultiplerModifier(self, npc, ref num, npc.lavaWet, npc.honeyWet, npc.shimmerWet);
			});
			c.EmitBr(IL_0234);

			c.GotoNext(MoveType.After, i => i.MatchLdfld<WaterShaderData>("_usePlayerWaves"));
			c.GotoNext(MoveType.Before, 
				i => i.MatchLdarg(0), i => i.MatchLdfld<WaterShaderData>("_useViscosityFilter"), i => i.MatchBrtrue(out IL_04f7), 
				i => i.MatchLdloc(out player_varNum), i => i.MatchLdfld<Entity>("honeyWet"), i => i.MatchBrtrue(out _), 
				i => i.MatchLdloc(player_varNum), i => i.MatchLdfld<Entity>("lavaWet"), i => i.MatchBrfalse(out _), 
				i => i.MatchLdloc(out num3_varNum), i => i.MatchLdcR4(0.3f), i => i.MatchMul());
			c.EmitLdarg(0);
			c.EmitLdloc(player_varNum);
			c.EmitLdloca(num3_varNum);
			c.EmitDelegate((WaterShaderData self, Player player, ref float num3) =>
			{
				EntityMultiplerModifier(self, player, ref num3, player.lavaWet, player.honeyWet, player.shimmerWet);
			});
			c.EmitBr(IL_04f7);

			c.GotoNext(MoveType.After, i => i.MatchLdfld<WaterShaderData>("_useProjectileWaves"));
			c.GotoNext(MoveType.After, 
				i => i.MatchLdloc(out projectile_varNum), i => i.MatchLdfld<Entity>("lavaWet"), i => i.MatchStloc(out projFlag_varNum),
				i => i.MatchLdloc(projectile_varNum), i => i.MatchLdfld<Entity>("honeyWet"), i => i.MatchStloc(out projFlag2_varNum),
				i => i.MatchLdloc(projectile_varNum), i => i.MatchLdfld<Entity>("wet"), i => i.MatchStloc(out _));
			c.EmitLdloc(projectile_varNum);
			c.EmitDelegate((Projectile projectile) =>
			{
				return projectile.shimmerWet;
			});
			c.EmitStloc(shimmerFlag_varDef);

			c.GotoNext(MoveType.Before, i => i.MatchCall<Collision>("CheckAABBvAABBCollision"), i => i.MatchBrfalse(out IL_087e));
			c.GotoNext(MoveType.Before, i => i.MatchLdloc(projectile_varNum), i => i.MatchLdfld<Projectile>("ignoreWater"), i => i.MatchBrfalse(out IL_0772), i => i.MatchLdloc(projectile_varNum));
			c.EmitLdloc(projectile_varNum);
			c.EmitLdloca(projFlag_varNum);
			c.EmitLdloca(projFlag2_varNum);
			c.EmitLdloca(shimmerFlag_varDef);
			c.EmitDelegate((Projectile projectile, ref bool flag, ref bool flag2, ref bool hasShimmer) =>
			{
				if (projectile.ignoreWater)
				{
					bool num9 = Collision.LavaCollision(projectile.position, projectile.width, projectile.height);
					flag = LiquidCollision.GetAppropriateWets(projectile.position, projectile.width, projectile.height, out bool[] wets);
					flag2 = Collision.honey;
					hasShimmer = Collision.shimmer;
					bool hasModWet = false;
					for (int i = LiquidID.Count; i < wets.Length; i++)
					{
						if (wets[i])
							hasModWet = true;
					}
					if (!(num9 || flag || flag2 || hasShimmer || hasModWet))
					{
						return true;
					}
				}
				return false;
			});
			c.EmitBrtrue(IL_087e);
			c.EmitBr(IL_0772);

			c.GotoNext(MoveType.Before, 
				i => i.MatchLdarg(0), i => i.MatchLdfld<WaterShaderData>("_useViscosityFilter"), i => i.MatchBrtrue(out IL_07cf), 
				i => i.MatchLdloc(projFlag2_varNum), i => i.MatchLdloc(projFlag_varNum), i => i.MatchOr(), i => i.MatchBrfalse(out _), 
				i => i.MatchLdloc(out num7_varNum), i => i.MatchLdcR4(0.3f), i => i.MatchMul());
			c.EmitLdarg(0);
			c.EmitLdloc(projectile_varNum);
			c.EmitLdloca(num7_varNum);
			c.EmitLdloc(projFlag_varNum);
			c.EmitLdloc(projFlag2_varNum);
			c.EmitLdloc(shimmerFlag_varDef);
			c.EmitDelegate((WaterShaderData self, Projectile projectile, ref float num7, bool flag, bool flag2, bool hasShimmer) =>
			{
				EntityMultiplerModifier(self, projectile, ref num7, flag, flag2, hasShimmer);
			});
			c.EmitBr(IL_07cf);
		}

		internal static void EntityMultiplerModifier(WaterShaderData self, Entity entity, ref float num, bool lava, bool honey, bool shimmer)
		{
			if (!self._useViscosityFilter)
			{
				float amplifier = 1f;
				int liquidID = LiquidID.Water;
				if (lava)
				{
					amplifier = 0.3f;
					liquidID = LiquidID.Lava;
				}
				else if (honey)
				{
					amplifier = 0.3f;
					liquidID = LiquidID.Honey;
				}
				else if (shimmer)
					liquidID = LiquidID.Shimmer;
				else
				{
					bool hasModWet = false;
					for (int i = LiquidID.Count; i < LiquidLoader.LiquidCount; i++)
					{
						if (entity.GetModdedWetArray()[i - LiquidID.Count])
						{
							amplifier = LiquidLoader.GetLiquid(i).WaterRippleMultiplier;
							liquidID = i;
							hasModWet = true;
						}
					}
					if (!hasModWet)
					{
						liquidID = LiquidID.Water;
					}
				}
				LiquidLoader.WaterRippleMultiplier(liquidID, ref amplifier);
				num *= amplifier;
			}
		}
	}
}
