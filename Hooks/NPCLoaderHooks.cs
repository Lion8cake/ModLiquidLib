using ModLiquidLib.ID;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModLiquidLib.Hooks
{
	internal class NPCLoaderHooks
	{
		internal static void DoAlternativeVanillaSpawning(ILContext il)
		{
			ILCursor c = new(il);
			c.Index = 0;
			c.EmitLdarga(0);
			c.EmitDelegate((ref NPCSpawnInfo spawnInfo) =>
			{
				if (Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY - 1].LiquidAmount > 0 && Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY - 2].LiquidAmount > 0)
				{
					if (Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY - 1].LiquidType != LiquidID.Water)
					{
						spawnInfo.Water = false;
					}
				}
			});

			c.GotoNext(MoveType.After, i => i.MatchRet(), i => i.MatchLdloc(out _));
			c.EmitLdarg(0);
			c.EmitDelegate((int? type, NPCSpawnInfo spawnInfo) =>
			{
				if (type == 0)
				{
					SpawnNPC.SpawnNPC_SpawnVanillaNPC(spawnInfo);
					return null;
				}
				if (type != null)
				{
					int num = spawnInfo.SpawnTileX;
					int num24 = spawnInfo.SpawnTileY;
					bool flag12 = true;
					if (flag12)
					{
						if (Main.tile[num, num24 - 1].liquid > 0 && Main.tile[num, num24 - 2].liquid > 0)
						{
							if (Main.tile[num, num24 - 1].LiquidType > LiquidID.Shimmer && !LiquidID_TLmod.Sets.CanModdedNPCSpawnInModdedLiquid[(int)type][Main.tile[num, num24 - 1].LiquidType])
							{
								flag12 = false;
							}
						}
					}
					if (!flag12)
					{
						return null;
					}
				}
				return type;
			});
		}
	}
}
