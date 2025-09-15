using ModLiquidLib.Utils.ManualHooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModLiquidLib.Hooks
{
	internal class NPCLoaderHooks
	{
		internal static int? DisableWaterSpawning(On_NPCLoader.orig_ChooseSpawn orig, NPCSpawnInfo spawnInfo)
		{
			//Ideally should be done through editing NPC.SpawnNPC, but due to a garbage collection issue, it has to be done through this instead.
			if (Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY - 1].LiquidAmount > 0 && Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY - 2].LiquidAmount > 0 && Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY - 1].LiquidType != LiquidID.Lava)
			{
				if (Main.tile[spawnInfo.SpawnTileX, spawnInfo.SpawnTileY - 1].LiquidType != LiquidID.Water)
				{
					spawnInfo.Water = false;
				}
			}
			return orig.Invoke(spawnInfo);
		}
	}
}
