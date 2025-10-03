using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModLiquidLib.Hooks
{
	internal class SpawnNPC
	{
		//What I like to call the C.L.U.S.T.E.R.F.U.C.K
		//Don't ask what it stands for, you'll get it
		//A replication of a large section from NPC.SpawnNPC, ported to now take into account the modified spawn info and additional liquid prevention check
		internal static void SpawnNPC_SpawnVanillaNPC(NPCSpawnInfo spawnInfo)
		{
			bool tooWindyForButterflies = NPC.TooWindyForButterflies;
			bool flag = (double)Main.windSpeedTarget < -0.4 || (double)Main.windSpeedTarget > 0.4;
			int num46 = 0;
			for (int i = 0; i < 255; i++)
			{
				if (Main.player[i].active)
				{
					num46++;
				}
			}
			float num57 = 0f;
			for (int j = 0; j < 200; j++)
			{
				if (Main.npc[j].active)
				{
					switch (Main.npc[j].type)
					{
						case NPCID.HeadlessHorseman:
						case NPCID.MourningWood:
						case NPCID.Pumpking:
						case NPCID.PumpkingBlade:
						case NPCID.Everscream:
						case NPCID.IceQueen:
						case NPCID.SantaNK1:
							num57 += Main.npc[j].npcSlots;
							break;
					}
				}
			}
			float num67 = (int)((float)NPC.defaultMaxSpawns * (2f + 0.3f * (float)num46));

			int k = spawnInfo.Player.whoAmI;
			int num = spawnInfo.SpawnTileX;
			int num24 = spawnInfo.SpawnTileY;
			int num89 = (int)(Main.player[k].position.X + (float)(Main.player[k].width / 2)) / 16;
			int num100 = (int)(Main.player[k].position.Y + (float)(Main.player[k].height / 2)) / 16;
			int num35 = spawnInfo.SpawnTileType;
			int num56 = Main.tile[num, num24].type;

			int maxValue = 65;
			if (Main.remixWorld && (double)(Main.player[k].position.Y / 16f) < Main.worldSurface && (Main.player[k].ZoneCorrupt || Main.player[k].ZoneCrimson))
			{
				maxValue = 25;
			}

			bool flag38 = spawnInfo.Granite;
			bool flag6 = NPC.downedPlantBoss && Main.hardMode;
			bool isItAHappyWindyDay = Main.IsItAHappyWindyDay;

			bool flag12 = true;
			if (flag12)
			{
				if (Main.tile[num, num24 - 1].liquid > 0 && Main.tile[num, num24 - 2].liquid > 0)
				{
					if (Main.tile[num, num24 - 1].LiquidType > LiquidID.Shimmer)
					{
						flag12 = false;
					}
				}
			}
			if (!flag12)
			{
				return;
			}

			bool flag11 = (float)new Point(num89 - num, num100 - num24).X * Main.windSpeedTarget > 0f;
			bool flag13 = (double)num24 <= Main.worldSurface;
			bool flag14 = (double)num24 >= Main.rockLayer;
			bool flag15 = ((num < WorldGen.oceanDistance || num > Main.maxTilesX - WorldGen.oceanDistance) && Main.tileSand[num56] && (double)num24 < Main.rockLayer) || (num35 == 53 && WorldGen.oceanDepths(num, num24));
			bool flag16 = (double)num24 <= Main.worldSurface && (num < WorldGen.beachDistance || num > Main.maxTilesX - WorldGen.beachDistance);
			bool flag17 = Main.cloudAlpha > 0f;
			int range = 10;
			if (Main.remixWorld)
			{
				flag17 = Main.raining;
				flag14 = (((double)num24 > Main.worldSurface && (double)num24 < Main.rockLayer) ? true : false);
				if ((double)num24 < Main.worldSurface + 5.0)
				{
					Main.raining = false;
					Main.cloudAlpha = 0f;
					Main.dayTime = false;
				}
				range = 5;
				if (Main.player[k].ZoneCorrupt || Main.player[k].ZoneCrimson)
				{
					flag15 = false;
					flag16 = false;
				}
				if ((double)num < (double)Main.maxTilesX * 0.43 || (double)num > (double)Main.maxTilesX * 0.57)
				{
					if ((double)num24 > Main.rockLayer - 200.0 && num24 < Main.maxTilesY - 200 && Main.rand.Next(2) == 0)
					{
						flag15 = true;
					}
					if ((double)num24 > Main.rockLayer - 200.0 && num24 < Main.maxTilesY - 200 && Main.rand.Next(2) == 0)
					{
						flag16 = true;
					}
				}
				if ((double)num24 > Main.rockLayer - 20.0)
				{
					if (num24 <= Main.maxTilesY - 190 && Main.rand.Next(3) != 0)
					{
						flag13 = true;
						Main.dayTime = false;
						if (Main.rand.Next(2) == 0)
						{
							Main.dayTime = true;
						}
					}
					else if ((Main.bloodMoon || (Main.eclipse && Main.dayTime)) && (double)num > (double)Main.maxTilesX * 0.38 + 50.0 && (double)num < (double)Main.maxTilesX * 0.62)
					{
						flag13 = true;
					}
				}
			}
			bool flag9 = (double)num24 <= Main.rockLayer;
			if (Main.remixWorld)
			{
				flag9 = (double)num24 > Main.rockLayer && num24 <= Main.maxTilesY - 190;
			}

			bool flag23 = spawnInfo.Sky;
			bool flag35 = spawnInfo.Invasion;
			bool flag34 = spawnInfo.PlayerSafe;
			int num58 = Main.tile[num, num24 - 1].wall;
			if (Main.tile[num, num24 - 2].wall == 244 || Main.tile[num, num24].wall == 244)
			{
				num58 = 244;
			}
			bool flag36 = spawnInfo.Water;
			bool flag3 = spawnInfo.SpiderCave;
			bool flag5 = spawnInfo.DesertCave;
			bool flag4 = spawnInfo.PlayerInTown;
			bool flag7 = spawnInfo.SafeRangeX;
			bool flag33 = spawnInfo.Lihzahrd;
			bool flag2 = spawnInfo.Marble;

			int newNPC = 200;

			int cattailX;
			int cattailY;
			if (Main.player[k].ZoneTowerNebula)
			{
				bool flag18 = true;
				int num59 = 0;
				while (flag18)
				{
					num59 = Terraria.Utils.SelectRandom<int>(Main.rand, 424, 424, 424, 423, 423, 423, 421, 421, 421, 420, 420);
					flag18 = false;
					if (num59 == 424 && NPC.CountNPCS(num59) >= 3)
					{
						flag18 = true;
					}
					if (num59 == 423 && NPC.CountNPCS(num59) >= 3)
					{
						flag18 = true;
					}
					if (num59 == 420 && NPC.CountNPCS(num59) >= 3)
					{
						flag18 = true;
					}
				}
				if (num59 != 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, num59, 1);
				}
			}
			else if (Main.player[k].ZoneTowerVortex)
			{
				bool flag19 = true;
				int num60 = 0;
				while (flag19)
				{
					num60 = Terraria.Utils.SelectRandom<int>(Main.rand, 429, 429, 429, 429, 427, 427, 425, 425, 426);
					flag19 = false;
					if (num60 == 425 && NPC.CountNPCS(num60) >= 3)
					{
						flag19 = true;
					}
					if (num60 == 426 && NPC.CountNPCS(num60) >= 3)
					{
						flag19 = true;
					}
					if (num60 == 429 && NPC.CountNPCS(num60) >= 4)
					{
						flag19 = true;
					}
				}
				if (num60 != 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, num60, 1);
				}
			}
			else if (Main.player[k].ZoneTowerStardust)
			{
				int num61 = Terraria.Utils.SelectRandom<int>(Main.rand, 411, 411, 411, 409, 409, 407, 402, 405);
				if (num61 != 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, num61, 1);
				}
			}
			else if (Main.player[k].ZoneTowerSolar)
			{
				bool flag20 = true;
				int num62 = 0;
				while (flag20)
				{
					num62 = Terraria.Utils.SelectRandom<int>(Main.rand, 518, 419, 418, 412, 417, 416, 415);
					flag20 = false;
					if (num62 == 418 && Main.rand.Next(2) == 0)
					{
						num62 = Terraria.Utils.SelectRandom<int>(Main.rand, 415, 416, 419, 417);
					}
					if (num62 == 518 && NPC.CountNPCS(num62) >= 2)
					{
						flag20 = true;
					}
					if (num62 == 412 && NPC.CountNPCS(num62) >= 1)
					{
						flag20 = true;
					}
				}
				if (num62 != 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, num62, 1);
				}
			}
			else if (flag23)
			{
				int maxValue2 = 8;
				int maxValue3 = 30;
				bool flag21 = (float)Math.Abs(num - Main.maxTilesX / 2) / (float)(Main.maxTilesX / 2) > 0.33f && (Main.wallLight[Main.tile[num89, num100].wall] || Main.tile[num89, num100].wall == 73);
				if (flag21 && NPC.AnyDanger())
				{
					flag21 = false;
				}
				if (Main.player[k].ZoneWaterCandle)
				{
					maxValue2 = 3;
					maxValue3 = 10;
				}
				if (flag35 && Main.invasionType == 4)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 388);
				}
				else if (flag21 && Main.hardMode && NPC.downedGolemBoss && (!NPC.downedMartians && Main.rand.Next(maxValue2) == 0 || Main.rand.Next(maxValue3) == 0) && !NPC.AnyNPCs(399))
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 399);
				}
				else if (flag21 && Main.hardMode && NPC.downedGolemBoss && (!NPC.downedMartians && Main.rand.Next(maxValue2) == 0 || Main.rand.Next(maxValue3) == 0) && !NPC.AnyNPCs(399) && (Main.player[k].inventory[Main.player[k].selectedItem].type == 148 || Main.player[k].ZoneWaterCandle))
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 399);
				}
				else if (Main.hardMode && !NPC.AnyNPCs(87) && !flag34 && Main.rand.Next(10) == 0)
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 87, 1);
				}
				else if (Main.hardMode && !NPC.AnyNPCs(87) && !flag34 && Main.rand.Next(10) == 0 && (Main.player[k].inventory[Main.player[k].selectedItem].type == 148 || Main.player[k].ZoneWaterCandle))
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 87, 1);
				}
				else if (!NPC.unlockedSlimePurpleSpawn && Main.player[k].RollLuck(25) == 0 && !NPC.AnyNPCs(686))
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 686);
				}
				else
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 48);
				}
			}
			else if (flag35)
			{
				if (Main.invasionType == 1)
				{
					if (Main.hardMode && !NPC.AnyNPCs(471) && Main.rand.Next(30) == 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 471);
					}
					else if (Main.rand.Next(9) == 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 29);
					}
					else if (Main.rand.Next(5) == 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 26);
					}
					else if (Main.rand.Next(3) == 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 111);
					}
					else if (Main.rand.Next(3) == 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 27);
					}
					else
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 28);
					}
				}
				else if (Main.invasionType == 2)
				{
					if (Main.rand.Next(7) == 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 145);
					}
					else if (Main.rand.Next(3) == 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 143);
					}
					else
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 144);
					}
				}
				else if (Main.invasionType == 3)
				{
					if (Main.invasionSize < Main.invasionSizeStart / 2 && Main.rand.Next(20) == 0 && !NPC.AnyNPCs(491) && !Collision.SolidTiles(num - 20, num + 20, num24 - 40, num24 - 10))
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, (num24 - 10) * 16, 491);
					}
					else if (Main.rand.Next(30) == 0 && !NPC.AnyNPCs(216))
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 216);
					}
					else if (Main.rand.Next(11) == 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 215);
					}
					else if (Main.rand.Next(9) == 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 252);
					}
					else if (Main.rand.Next(7) == 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 214);
					}
					else if (Main.rand.Next(3) == 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 213);
					}
					else
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 212);
					}
				}
				else if (Main.invasionType == 4)
				{
					int num63 = 0;
					int num64 = Main.rand.Next(7);
					bool flag22 = (Main.invasionSizeStart - Main.invasionSize) / (float)Main.invasionSizeStart >= 0.3f && !NPC.AnyNPCs(395);
					if (Main.rand.Next(45) == 0 && flag22)
					{
						num63 = 395;
					}
					else if (num64 >= 6)
					{
						if (Main.rand.Next(20) == 0 && flag22)
						{
							num63 = 395;
						}
						else
						{
							int num111 = Main.rand.Next(2);
							if (num111 == 0)
							{
								num63 = 390;
							}
							if (num111 == 1)
							{
								num63 = 386;
							}
						}
					}
					else if (num64 >= 4)
					{
						int num65 = Main.rand.Next(5);
						num63 = num65 < 2 ? 382 : num65 >= 4 ? 388 : 381;
					}
					else
					{
						int num66 = Main.rand.Next(4);
						if (num66 == 3)
						{
							if (!NPC.AnyNPCs(520))
							{
								num63 = 520;
							}
							else
							{
								num66 = Main.rand.Next(3);
							}
						}
						if (num66 == 0)
						{
							num63 = 385;
						}
						if (num66 == 1)
						{
							num63 = 389;
						}
						if (num66 == 2)
						{
							num63 = 383;
						}
					}
					if (num63 != 0)
					{
						newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, num63, 1);
					}
				}
			}
			else if (num58 == 244 && !Main.remixWorld)
			{
				if (flag36)
				{
					if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 592);
					}
					else
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 55);
					}
				}
				else if ((double)num24 > Main.worldSurface)
				{
					if (Main.rand.Next(3) == 0)
					{
						if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 447);
						}
						else
						{
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 300);
						}
					}
					else if (Main.rand.Next(2) == 0)
					{
						newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 359);
					}
					else if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 448);
					}
					else if (Main.rand.Next(3) != 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 357);
					}
				}
				else if (Main.player[k].RollLuck(2) == 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 624);
					Main.npc[newNPC].timeLeft *= 10;
				}
				else if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 443);
				}
				else if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 539);
				}
				else if (Main.halloween && Main.rand.Next(3) != 0)
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 303);
				}
				else if (Main.xMas && Main.rand.Next(3) != 0)
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 337);
				}
				else if (BirthdayParty.PartyIsUp && Main.rand.Next(3) != 0)
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 540);
				}
				else if (Main.rand.Next(3) == 0)
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, Terraria.Utils.SelectRandom(Main.rand, new short[2] { 299, 538 }));
				}
				else
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 46);
				}
			}
			else if (!NPC.savedBartender && DD2Event.ReadyToFindBartender && !NPC.AnyNPCs(579) && Main.rand.Next(80) == 0 && !flag36)
			{
				NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 579);
			}
			else if (Main.tile[num, num24].wall == 62 || flag3)
			{
				bool flag24 = flag14 && num24 < Main.maxTilesY - 210;
				if (Main.dontStarveWorld)
				{
					flag24 = num24 < Main.maxTilesY - 210;
				}
				if (Main.tile[num, num24].wall == 62 && Main.rand.Next(8) == 0 && !flag36 && flag24 && !NPC.savedStylist && !NPC.AnyNPCs(354))
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 354);
				}
				else if (Main.hardMode && Main.rand.Next(10) != 0)
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 163);
				}
				else
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 164);
				}
			}
			else if ((NPC.SpawnTileOrAboveHasAnyWallInSet(num, num24, WallID.Sets.AllowsUndergroundDesertEnemiesToSpawn) || flag5) && WorldGen.checkUnderground(num, num24))
			{
				float num68 = 1.15f;
				if ((double)num24 > (Main.rockLayer * 2.0 + Main.maxTilesY) / 3.0)
				{
					num68 *= 0.5f;
				}
				else if ((double)num24 > Main.rockLayer)
				{
					num68 *= 0.85f;
				}
				if (Main.rand.Next(20) == 0 && !flag36 && !NPC.savedGolfer && !NPC.AnyNPCs(589))
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 589);
				}
				else if (Main.hardMode && Main.rand.Next((int)(45f * num68)) == 0 && !flag34 && (double)num24 > Main.worldSurface + 100.0)
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 510);
				}
				else if (Main.rand.Next((int)(45f * num68)) == 0 && !flag34 && (double)num24 > Main.worldSurface + 100.0 && NPC.CountNPCS(513) == 0)
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 513);
				}
				else if (Main.hardMode && Main.rand.Next(5) != 0)
				{
					List<int> list = new List<int>();
					if (Main.player[k].ZoneCorrupt)
					{
						list.Add(525);
						list.Add(525);
					}
					if (Main.player[k].ZoneCrimson)
					{
						list.Add(526);
						list.Add(526);
					}
					if (Main.player[k].ZoneHallow)
					{
						list.Add(527);
						list.Add(527);
					}
					if (list.Count == 0)
					{
						list.Add(524);
						list.Add(524);
					}
					if (Main.player[k].ZoneCorrupt || Main.player[k].ZoneCrimson)
					{
						list.Add(533);
						list.Add(529);
					}
					else
					{
						list.Add(530);
						list.Add(528);
					}
					list.Add(532);
					int num69 = Terraria.Utils.SelectRandom(Main.rand, list.ToArray());
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, num69);
					list.Clear();
				}
				else
				{
					int num70 = Terraria.Utils.SelectRandom<int>(Main.rand, 69, 580, 580, 580, 581);
					if (Main.rand.Next(15) == 0)
					{
						num70 = 537;
					}
					else if (Main.rand.Next(10) == 0)
					{
						switch (num70)
						{
							case 580:
								num70 = 508;
								break;
							case 581:
								num70 = 509;
								break;
						}
					}
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, num70);
				}
			}
			else if (Main.hardMode && flag36 && Main.player[k].ZoneJungle && Main.rand.Next(3) != 0)
			{
				NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 157);
			}
			else if (Main.hardMode && flag36 && Main.player[k].ZoneCrimson && Main.rand.Next(3) != 0)
			{
				NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 242);
			}
			else if (Main.hardMode && flag36 && Main.player[k].ZoneCrimson && Main.rand.Next(3) != 0)
			{
				NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 241);
			}
			else if ((!flag4 || !NPC.savedAngler && !NPC.AnyNPCs(376)) && flag36 && flag15)
			{
				bool flag25 = false;
				if (!NPC.savedAngler && !NPC.AnyNPCs(376) && ((double)num24 < Main.worldSurface - 10.0 || Main.remixWorld))
				{
					int num71 = -1;
					for (int num72 = num24 - 1; num72 > num24 - 50; num72--)
					{
						if (Main.tile[num, num72].liquid == 0 && !WorldGen.SolidTile(num, num72) && !WorldGen.SolidTile(num, num72 + 1) && !WorldGen.SolidTile(num, num72 + 2))
						{
							num71 = num72 + 2;
							break;
						}
					}
					if (num71 > num24)
					{
						num71 = num24;
					}
					if (num71 > 0 && !flag7)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num71 * 16, 376);
						flag25 = true;
					}
				}
				if (!flag25 && !flag7)
				{
					int num73 = -1;
					int num74 = -1;
					if (((double)num24 < Main.worldSurface || Main.remixWorld) && num24 > 50)
					{
						for (int num75 = num24 - 1; num75 > num24 - 50; num75--)
						{
							if (Main.tile[num, num75].liquid == 0 && !WorldGen.SolidTile(num, num75) && !WorldGen.SolidTile(num, num75 + 1) && !WorldGen.SolidTile(num, num75 + 2))
							{
								num73 = num75 + 2;
								if (!WorldGen.SolidTile(num, num73 + 1) && !WorldGen.SolidTile(num, num73 + 2) && !Main.wallHouse[Main.tile[num, num73 + 2].wall])
								{
									num74 = num73 + 2;
								}
								if (Main.wallHouse[Main.tile[num, num73].wall])
								{
									num73 = -1;
								}
								break;
							}
						}
						if (num73 > num24)
						{
							num73 = num24;
						}
						if (num74 > num24)
						{
							num74 = num24;
						}
					}
					if (num73 > 0 && !flag7 && Main.rand.Next(10) == 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num73 * 16, 602);
					}
					else if (Main.rand.Next(10) == 0)
					{
						int num76 = Main.rand.Next(3);
						if (num76 == 0 && num73 > 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num73 * 16, 625);
						}
						else if (num76 == 1 && num74 > 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num74 * 16, 615);
						}
						else if (num76 == 2)
						{
							int num77 = num24;
							if (num74 > 0)
							{
								num77 = num74;
							}
							if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
							{
								NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num77 * 16, 627);
							}
							else
							{
								NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num77 * 16, 626);
							}
						}
					}
					else if (Main.rand.Next(40) == 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 220);
					}
					else if (Main.rand.Next(18) == 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 221);
					}
					else if (Main.rand.Next(8) == 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 65);
					}
					else if (Main.rand.Next(3) == 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 67);
					}
					else
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 64);
					}
				}
			}
			else if (!flag36 && !NPC.savedAngler && !NPC.AnyNPCs(376) && (num < WorldGen.beachDistance || num > Main.maxTilesX - WorldGen.beachDistance) && Main.tileSand[num56] && ((double)num24 < Main.worldSurface || Main.remixWorld))
			{
				NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 376);
			}
			else if (!flag4 && flag36 && (flag14 && Main.rand.Next(2) == 0 || num56 == 60))
			{
				bool flag26 = false;
				if (num56 == 60 && flag13 && num24 > 50 && Main.rand.Next(3) == 0 && Main.dayTime)
				{
					int num79 = -1;
					for (int num80 = num24 - 1; num80 > num24 - 50; num80--)
					{
						if (Main.tile[num, num80].liquid == 0 && !WorldGen.SolidTile(num, num80) && !WorldGen.SolidTile(num, num80 + 1) && !WorldGen.SolidTile(num, num80 + 2))
						{
							num79 = num80 + 2;
							break;
						}
					}
					if (num79 > num24)
					{
						num79 = num24;
					}
					if (num79 > 0 && !flag7)
					{
						flag26 = true;
						if (Main.rand.Next(4) == 0)
						{
							flag26 = true;
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num79 * 16, 617);
						}
						else if (!flag && Main.cloudAlpha == 0f)
						{
							flag26 = true;
							int num81 = Main.rand.Next(1, 4);
							for (int num82 = 0; num82 < num81; num82++)
							{
								if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
								{
									NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8 + Main.rand.Next(-16, 17), num79 * 16 - 16, 613);
								}
								else
								{
									NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8 + Main.rand.Next(-16, 17), num79 * 16 - 16, 612);
								}
							}
						}
					}
				}
				if (!flag26)
				{
					if (Main.hardMode && Main.rand.Next(3) > 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 102);
					}
					else
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 58);
					}
				}
			}
			else if (!flag4 && flag36 && (double)num24 > Main.worldSurface && Main.rand.Next(3) == 0)
			{
				if (Main.hardMode && Main.rand.Next(3) > 0)
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 103);
				}
				else
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 63);
				}
			}
			else if (flag36 && Main.rand.Next(4) == 0 && (num > WorldGen.oceanDistance && num < Main.maxTilesX - WorldGen.oceanDistance || (double)num24 > Main.worldSurface + 50.0))
			{
				if (Main.player[k].ZoneCorrupt)
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 57);
				}
				else if (Main.player[k].ZoneCrimson)
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 465);
				}
				else if ((double)num24 < Main.worldSurface && num24 > 50 && Main.rand.Next(3) != 0 && Main.dayTime)
				{
					int num83 = -1;
					for (int num84 = num24 - 1; num84 > num24 - 50; num84--)
					{
						if (Main.tile[num, num84].liquid == 0 && !WorldGen.SolidTile(num, num84) && !WorldGen.SolidTile(num, num84 + 1) && !WorldGen.SolidTile(num, num84 + 2))
						{
							num83 = num84 + 2;
							break;
						}
					}
					if (num83 > num24)
					{
						num83 = num24;
					}
					if (num83 > 0 && !flag7)
					{
						if (Main.rand.Next(5) == 0 && (num35 == 2 || num35 == 477))
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num83 * 16, 616);
						}
						else if (num35 == 53)
						{
							if (Main.rand.Next(2) == 0 && !flag && Main.cloudAlpha == 0f)
							{
								int num85 = Main.rand.Next(1, 4);
								for (int num86 = 0; num86 < num85; num86++)
								{
									if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
									{
										NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8 + Main.rand.Next(-16, 17), num83 * 16 - 16, 613);
									}
									else
									{
										NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8 + Main.rand.Next(-16, 17), num83 * 16 - 16, 612);
									}
								}
							}
							else
							{
								NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num83 * 16, 608);
							}
						}
						else if (Main.rand.Next(2) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num83 * 16, 362);
						}
						else
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num83 * 16, 364);
						}
					}
					else if (num35 == 53 && num > WorldGen.beachDistance && num < Main.maxTilesX - WorldGen.beachDistance)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num83 * 16, 607);
					}
					else if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 592);
					}
					else
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 55);
					}
				}
				else if (num35 == 53 && num > WorldGen.beachDistance && num < Main.maxTilesX - WorldGen.beachDistance)
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 607);
				}
				else if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 592);
				}
				else
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 55);
				}
			}
			else if (NPC.downedGoblins && Main.player[k].RollLuck(20) == 0 && !flag36 && flag14 && num24 < Main.maxTilesY - 210 && !NPC.savedGoblin && !NPC.AnyNPCs(105))
			{
				NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 105);
			}
			else if (Main.hardMode && Main.player[k].RollLuck(20) == 0 && !flag36 && flag14 && num24 < Main.maxTilesY - 210 && !NPC.savedWizard && !NPC.AnyNPCs(106))
			{
				NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 106);
			}
			else if (NPC.downedBoss3 && Main.player[k].RollLuck(20) == 0 && !flag36 && flag14 && num24 < Main.maxTilesY - 210 && !NPC.unlockedSlimeOldSpawn && !NPC.AnyNPCs(685))
			{
				NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 685);
			}
			else if (flag4)
			{
				if (Main.player[k].ZoneGraveyard)
				{
					if (!flag36)
					{
						if (Main.rand.Next(2) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 606);
						}
						else
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 610);
						}
					}
				}
				else if (!flag7 && flag16)
				{
					if (flag36)
					{
						int num87 = -1;
						int num88 = -1;
						if (((double)num24 < Main.worldSurface || Main.remixWorld) && num24 > 50)
						{
							for (int num90 = num24 - 1; num90 > num24 - 50; num90--)
							{
								if (Main.tile[num, num90].liquid == 0 && !WorldGen.SolidTile(num, num90) && !WorldGen.SolidTile(num, num90 + 1) && !WorldGen.SolidTile(num, num90 + 2))
								{
									num87 = num90 + 2;
									if (!WorldGen.SolidTile(num, num87 + 1) && !WorldGen.SolidTile(num, num87 + 2))
									{
										num88 = num87 + 2;
									}
									break;
								}
							}
							if (num87 > num24)
							{
								num87 = num24;
							}
							if (num88 > num24)
							{
								num88 = num24;
							}
						}
						if (Main.rand.Next(2) == 0)
						{
							int num91 = Main.rand.Next(3);
							if (num91 == 0 && num87 > 0)
							{
								NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num87 * 16, 625);
							}
							else if (num91 == 1 && num88 > 0)
							{
								NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num88 * 16, 615);
							}
							else if (num91 == 2)
							{
								int num92 = num24;
								if (num88 > 0)
								{
									num92 = num88;
								}
								if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
								{
									NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num92 * 16, 627);
								}
								else
								{
									NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num92 * 16, 626);
								}
							}
						}
						else if (num87 > 0 && !flag7)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num87 * 16, 602);
						}
					}
					else
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 602);
					}
				}
				else if ((num56 == 2 || num56 == 477 || num56 == 53) && !tooWindyForButterflies && Main.raining && Main.dayTime && Main.rand.Next(2) == 0 && ((double)num24 <= Main.worldSurface || Main.remixWorld) && NPC.FindCattailTop(num, num24, out cattailX, out cattailY))
				{
					if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), cattailX * 16 + 8, cattailY * 16, 601);
					}
					else
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), cattailX * 16 + 8, cattailY * 16, NPC.RollDragonflyType(num56));
					}
					if (Main.rand.Next(3) == 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), cattailX * 16 + 8 - 16, cattailY * 16, NPC.RollDragonflyType(num56));
					}
					if (Main.rand.Next(3) == 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), cattailX * 16 + 8 + 16, cattailY * 16, NPC.RollDragonflyType(num56));
					}
				}
				else if (flag36)
				{
					if (flag13 && num24 > 50 && Main.rand.Next(3) != 0 && Main.dayTime)
					{
						int num93 = -1;
						for (int num94 = num24 - 1; num94 > num24 - 50; num94--)
						{
							if (Main.tile[num, num94].liquid == 0 && !WorldGen.SolidTile(num, num94) && !WorldGen.SolidTile(num, num94 + 1) && !WorldGen.SolidTile(num, num94 + 2))
							{
								num93 = num94 + 2;
								break;
							}
						}
						if (num93 > num24)
						{
							num93 = num24;
						}
						if (num93 > 0 && !flag7)
						{
							switch (num35)
							{
								case 60:
									if (Main.rand.Next(3) != 0 && !flag && Main.cloudAlpha == 0f)
									{
										int num97 = Main.rand.Next(1, 4);
										for (int num98 = 0; num98 < num97; num98++)
										{
											if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
											{
												NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8 + Main.rand.Next(-16, 17), num93 * 16 - 16, 613);
											}
											else
											{
												NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8 + Main.rand.Next(-16, 17), num93 * 16 - 16, 612);
											}
										}
									}
									else
									{
										NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num93 * 16, 617);
									}
									break;
								case 53:
									if (Main.rand.Next(3) != 0 && !flag && Main.cloudAlpha == 0f)
									{
										int num95 = Main.rand.Next(1, 4);
										for (int num96 = 0; num96 < num95; num96++)
										{
											if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
											{
												NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8 + Main.rand.Next(-16, 17), num93 * 16 - 16, 613);
											}
											else
											{
												NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8 + Main.rand.Next(-16, 17), num93 * 16 - 16, 612);
											}
										}
									}
									else
									{
										NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num93 * 16, 608);
									}
									break;
								default:
									if (Main.rand.Next(5) == 0 && (num35 == 2 || num35 == 477))
									{
										NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num93 * 16, 616);
									}
									else if (Main.rand.Next(2) == 0)
									{
										NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num93 * 16, 362);
									}
									else
									{
										NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num93 * 16, 364);
									}
									break;
							}
						}
						else if (num35 == 53 && num > WorldGen.beachDistance && num < Main.maxTilesX - WorldGen.beachDistance)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 607);
						}
						else if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 592);
						}
						else
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 55);
						}
					}
					else if (num35 == 53 && num > WorldGen.beachDistance && num < Main.maxTilesX - WorldGen.beachDistance)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 607);
					}
					else if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 592);
					}
					else
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 55);
					}
				}
				else if (num56 == 147 || num56 == 161)
				{
					if (Main.rand.Next(2) == 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 148);
					}
					else
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 149);
					}
				}
				else if (num56 == 60)
				{
					if (Main.dayTime && Main.rand.Next(3) != 0)
					{
						switch (Main.rand.Next(5))
						{
							case 0:
								NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 671);
								break;
							case 1:
								NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 672);
								break;
							case 2:
								NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 673);
								break;
							case 3:
								NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 674);
								break;
							default:
								NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 675);
								break;
						}
					}
					else
					{
						NPC.SpawnNPC_SpawnFrog(num, num24, k);
					}
				}
				else if (num56 == 53)
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, Main.rand.Next(366, 368));
				}
				else
				{
					if (num56 != 2 && num56 != 477 && num56 != 109 && num56 != 492 && !((double)num24 > Main.worldSurface))
					{
						return;
					}
					bool flag27 = flag13;
					if (Main.raining && num24 <= Main.UnderworldLayer)
					{
						if (flag14 && Main.rand.Next(5) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, NPC.SpawnNPC_GetGemSquirrelToSpawn());
						}
						else if (flag14 && Main.rand.Next(5) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, NPC.SpawnNPC_GetGemBunnyToSpawn());
						}
						else if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 448);
						}
						else if (Main.rand.Next(3) != 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 357);
						}
						else if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 593);
						}
						else
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 230);
						}
					}
					else if (!Main.dayTime && Main.numClouds <= 55 && Main.cloudBGActive == 0f && Star.starfallBoost > 3f && flag27 && Main.player[k].RollLuck(2) == 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 484);
					}
					else if (!tooWindyForButterflies && !Main.dayTime && Main.rand.Next(NPC.fireFlyFriendly) == 0 && flag27)
					{
						int num99 = 355;
						if (num56 == 109)
						{
							num99 = 358;
						}
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, num99);
						if (Main.rand.Next(NPC.fireFlyMultiple) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8 - 16, num24 * 16, num99);
						}
						if (Main.rand.Next(NPC.fireFlyMultiple) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8 + 16, num24 * 16, num99);
						}
						if (Main.rand.Next(NPC.fireFlyMultiple) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16 - 16, num99);
						}
						if (Main.rand.Next(NPC.fireFlyMultiple) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16 + 16, num99);
						}
					}
					else if (Main.cloudAlpha == 0f && !Main.dayTime && Main.rand.Next(5) == 0 && flag27)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 611);
					}
					else if (Main.dayTime && Main.time < 18000.0 && Main.rand.Next(3) != 0 && flag27)
					{
						int num101 = Main.rand.Next(4);
						if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 442);
						}
						else
						{
							switch (num101)
							{
								case 0:
									NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 297);
									break;
								case 1:
									NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 298);
									break;
								default:
									NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 74);
									break;
							}
						}
					}
					else if (!tooWindyForButterflies && !Main.raining && Main.dayTime && Main.rand.Next(NPC.stinkBugChance) == 0 && flag27)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 669);
						if (Main.rand.Next(4) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8 - 16, num24 * 16, 669);
						}
						if (Main.rand.Next(4) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8 + 16, num24 * 16, 669);
						}
					}
					else if (!tooWindyForButterflies && !Main.raining && Main.dayTime && Main.rand.Next(NPC.butterflyChance) == 0 && flag27)
					{
						if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 444);
						}
						else
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 356);
						}
						if (Main.rand.Next(4) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8 - 16, num24 * 16, 356);
						}
						if (Main.rand.Next(4) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8 + 16, num24 * 16, 356);
						}
					}
					else if (tooWindyForButterflies && !Main.raining && Main.dayTime && Main.rand.Next(NPC.butterflyChance / 2) == 0 && flag27)
					{
						if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 605);
						}
						else
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 604);
						}
						if (Main.rand.Next(3) != 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 604);
						}
						if (Main.rand.Next(2) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 604);
						}
						if (Main.rand.Next(3) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 604);
						}
						if (Main.rand.Next(4) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 604);
						}
					}
					else if (Main.rand.Next(2) == 0 && flag27)
					{
						int num102 = Main.rand.Next(4);
						if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 442);
						}
						else
						{
							switch (num102)
							{
								case 0:
									NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 297);
									break;
								case 1:
									NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 298);
									break;
								default:
									NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 74);
									break;
							}
						}
					}
					else if (num24 > Main.UnderworldLayer)
					{
						if (Main.remixWorld && (double)(Main.player[k].Center.X / 16f) > Main.maxTilesX * 0.39 + 50.0 && (double)(Main.player[k].Center.X / 16f) < Main.maxTilesX * 0.61 && Main.rand.Next(2) == 0)
						{
							if (Main.rand.Next(2) == 0)
							{
								NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, NPC.SpawnNPC_GetGemSquirrelToSpawn());
							}
							else
							{
								NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, NPC.SpawnNPC_GetGemBunnyToSpawn());
							}
						}
						else
						{
							newNPC = NPC.SpawnNPC_SpawnLavaBaitCritters(num, num24);
						}
					}
					else if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 443);
					}
					else if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0 && flag27)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 539);
					}
					else if (Main.halloween && Main.rand.Next(3) != 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 303);
					}
					else if (Main.xMas && Main.rand.Next(3) != 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 337);
					}
					else if (BirthdayParty.PartyIsUp && Main.rand.Next(3) != 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 540);
					}
					else if (Main.rand.Next(3) == 0)
					{
						if (Main.remixWorld)
						{
							if ((double)num24 < Main.rockLayer && (double)num24 > Main.worldSurface)
							{
								if (Main.rand.Next(5) == 0)
								{
									NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, NPC.SpawnNPC_GetGemSquirrelToSpawn());
								}
							}
							else if (flag27)
							{
								NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, Terraria.Utils.SelectRandom(Main.rand, new short[2] { 299, 538 }));
							}
						}
						else if ((double)num24 >= Main.rockLayer && num24 <= Main.UnderworldLayer)
						{
							if (Main.rand.Next(5) == 0)
							{
								NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, NPC.SpawnNPC_GetGemSquirrelToSpawn());
							}
						}
						else if (flag27)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, Terraria.Utils.SelectRandom(Main.rand, new short[2] { 299, 538 }));
						}
					}
					else if (Main.remixWorld)
					{
						if ((double)num24 < Main.rockLayer && (double)num24 > Main.worldSurface)
						{
							if ((double)num24 >= Main.rockLayer && num24 <= Main.UnderworldLayer)
							{
								if (Main.rand.Next(5) == 0)
								{
									NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, NPC.SpawnNPC_GetGemBunnyToSpawn());
								}
							}
							else
							{
								NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 46);
							}
						}
					}
					else if ((double)num24 >= Main.rockLayer && num24 <= Main.UnderworldLayer)
					{
						if (Main.rand.Next(5) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, NPC.SpawnNPC_GetGemBunnyToSpawn());
						}
					}
					else
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 46);
					}
				}
			}
			else if (Main.player[k].ZoneDungeon)
			{
				int num103 = 0;
				ushort wall = Main.tile[num, num24].wall;
				ushort wall2 = Main.tile[num, num24 - 1].wall;
				if (wall == 94 || wall == 96 || wall == 98 || wall2 == 94 || wall2 == 96 || wall2 == 98)
				{
					num103 = 1;
				}
				if (wall == 95 || wall == 97 || wall == 99 || wall2 == 95 || wall2 == 97 || wall2 == 99)
				{
					num103 = 2;
				}
				if (Main.player[k].RollLuck(7) == 0)
				{
					num103 = Main.rand.Next(3);
				}
				bool flag28 = !NPC.downedBoss3;
				if (Main.drunkWorld && Main.player[k].position.Y / 16f < (float)(Main.dungeonY + 40))
				{
					flag28 = false;
				}
				if (flag28)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 68);
				}
				else if (NPC.downedBoss3 && !NPC.savedMech && Main.rand.Next(5) == 0 && !flag36 && !NPC.AnyNPCs(123) && (double)num24 > (Main.worldSurface * 4.0 + Main.rockLayer) / 5.0)
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 123);
				}
				else if (flag6 && Main.rand.Next(30) == 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 287);
				}
				else if (flag6 && num103 == 0 && Main.rand.Next(15) == 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 293);
				}
				else if (flag6 && num103 == 1 && Main.rand.Next(15) == 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 291);
				}
				else if (flag6 && num103 == 2 && Main.rand.Next(15) == 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 292);
				}
				else if (flag6 && !NPC.AnyNPCs(290) && num103 == 0 && Main.rand.Next(35) == 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 290);
				}
				else if (flag6 && (num103 == 1 || num103 == 2) && Main.rand.Next(30) == 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 289);
				}
				else if (flag6 && Main.rand.Next(20) == 0)
				{
					int num104 = 281;
					if (num103 == 0)
					{
						num104 += 2;
					}
					if (num103 == 2)
					{
						num104 += 4;
					}
					num104 += Main.rand.Next(2);
					if (!NPC.AnyNPCs(num104))
					{
						newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, num104);
					}
				}
				else if (flag6 && Main.rand.Next(3) != 0)
				{
					int num105 = 269;
					if (num103 == 0)
					{
						num105 += 4;
					}
					if (num103 == 2)
					{
						num105 += 8;
					}
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, num105 + Main.rand.Next(4));
				}
				else if (Main.player[k].RollLuck(35) == 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 71);
				}
				else if (num103 == 1 && Main.rand.Next(3) == 0 && !NPC.NearSpikeBall(num, num24))
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 70);
				}
				else if (num103 == 2 && Main.rand.Next(5) == 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 72);
				}
				else if (num103 == 0 && Main.rand.Next(7) == 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 34);
				}
				else if (Main.rand.Next(7) == 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 32);
				}
				else
				{
					switch (Main.rand.Next(5))
					{
						case 0:
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 294);
							break;
						case 1:
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 295);
							break;
						case 2:
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 296);
							break;
						default:
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 31);
							if (Main.rand.Next(4) == 0)
							{
								Main.npc[newNPC].SetDefaults(-14);
							}
							else if (Main.rand.Next(5) == 0)
							{
								Main.npc[newNPC].SetDefaults(-13);
							}
							break;
					}
				}
			}
			else if (Main.player[k].ZoneMeteor)
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 23);
			}
			else if (DD2Event.Ongoing && Main.player[k].ZoneOldOneArmy)
			{
				DD2Event.SpawnNPC(ref newNPC);
			}
			else if ((Main.remixWorld || (double)num24 <= Main.worldSurface) && !Main.dayTime && Main.snowMoon)
			{
				int num106 = NPC.waveNumber;
				if (Main.rand.Next(30) == 0 && NPC.CountNPCS(341) < 4)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 341);
				}
				else if (num106 >= 20)
				{
					int num107 = Main.rand.Next(3);
					if (!(num57 >= (float)num46 * num67))
					{
						newNPC = num107 switch
						{
							0 => NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 345),
							1 => NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 346),
							_ => NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 344),
						};
					}
				}
				else if (num106 >= 19)
				{
					newNPC = Main.rand.Next(10) == 0 && NPC.CountNPCS(345) < 4 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 345) : Main.rand.Next(10) == 0 && NPC.CountNPCS(346) < 5 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 346) : Main.rand.Next(10) != 0 || NPC.CountNPCS(344) >= 7 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 343) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 344);
				}
				else if (num106 >= 18)
				{
					newNPC = Main.rand.Next(10) == 0 && NPC.CountNPCS(345) < 3 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 345) : Main.rand.Next(10) == 0 && NPC.CountNPCS(346) < 4 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 346) : Main.rand.Next(10) == 0 && NPC.CountNPCS(344) < 6 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 344) : Main.rand.Next(3) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 348) : Main.rand.Next(3) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 343) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 351);
				}
				else if (num106 >= 17)
				{
					newNPC = Main.rand.Next(10) == 0 && NPC.CountNPCS(345) < 2 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 345) : Main.rand.Next(10) == 0 && NPC.CountNPCS(346) < 3 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 346) : Main.rand.Next(10) == 0 && NPC.CountNPCS(344) < 5 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 344) : Main.rand.Next(4) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 347) : Main.rand.Next(2) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 343) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 351);
				}
				else if (num106 >= 16)
				{
					newNPC = Main.rand.Next(10) == 0 && NPC.CountNPCS(345) < 2 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 345) : Main.rand.Next(10) == 0 && NPC.CountNPCS(346) < 2 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 346) : Main.rand.Next(10) == 0 && NPC.CountNPCS(344) < 4 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 344) : Main.rand.Next(2) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 343) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 352);
				}
				else if (num106 >= 15)
				{
					newNPC = Main.rand.Next(10) == 0 && !NPC.AnyNPCs(345) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 345) : Main.rand.Next(10) == 0 && NPC.CountNPCS(346) < 2 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 346) : Main.rand.Next(10) == 0 && NPC.CountNPCS(344) < 3 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 344) : Main.rand.Next(3) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 343) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 347);
				}
				else
				{
					switch (num106)
					{
						case 14:
							if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(345))
							{
								newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 345);
							}
							else if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(346))
							{
								newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 346);
							}
							else if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(344))
							{
								newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 344);
							}
							else if (Main.rand.Next(3) == 0)
							{
								newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 343);
							}
							break;
						case 13:
							newNPC = Main.rand.Next(10) == 0 && !NPC.AnyNPCs(345) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 345) : Main.rand.Next(10) == 0 && !NPC.AnyNPCs(346) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 346) : Main.rand.Next(3) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 352) : Main.rand.Next(6) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 343) : Main.rand.Next(3) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 347) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 342);
							break;
						case 12:
							newNPC = Main.rand.Next(10) == 0 && !NPC.AnyNPCs(345) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 345) : Main.rand.Next(10) == 0 && !NPC.AnyNPCs(344) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 344) : Main.rand.Next(8) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 343) : Main.rand.Next(3) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, Main.rand.Next(338, 341)) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 342);
							break;
						case 11:
							newNPC = Main.rand.Next(10) == 0 && !NPC.AnyNPCs(345) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 345) : Main.rand.Next(6) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 352) : Main.rand.Next(2) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, Main.rand.Next(338, 341)) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 342);
							break;
						case 10:
							newNPC = Main.rand.Next(10) == 0 && !NPC.AnyNPCs(346) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 346) : Main.rand.Next(10) == 0 && NPC.CountNPCS(344) < 2 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 344) : Main.rand.Next(6) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 351) : Main.rand.Next(3) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 348) : Main.rand.Next(3) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, Main.rand.Next(338, 341)) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 347);
							break;
						case 9:
							newNPC = Main.rand.Next(10) == 0 && !NPC.AnyNPCs(346) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 346) : Main.rand.Next(10) == 0 && !NPC.AnyNPCs(344) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 344) : Main.rand.Next(2) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 348) : Main.rand.Next(3) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 342) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 347);
							break;
						case 8:
							newNPC = Main.rand.Next(10) == 0 && !NPC.AnyNPCs(346) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 346) : Main.rand.Next(8) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 351) : Main.rand.Next(3) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 348) : Main.rand.Next(3) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 350) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 347);
							break;
						case 7:
							newNPC = Main.rand.Next(10) == 0 && !NPC.AnyNPCs(346) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 346) : Main.rand.Next(3) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 342) : Main.rand.Next(4) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, Main.rand.Next(338, 341)) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 350);
							break;
						case 6:
							newNPC = Main.rand.Next(10) == 0 && NPC.CountNPCS(344) < 2 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 344) : Main.rand.Next(4) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 347) : Main.rand.Next(2) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 350) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 348);
							break;
						case 5:
							newNPC = Main.rand.Next(10) == 0 && !NPC.AnyNPCs(344) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 344) : Main.rand.Next(4) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 350) : Main.rand.Next(8) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, Main.rand.Next(338, 341)) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 348);
							break;
						case 4:
							newNPC = Main.rand.Next(10) == 0 && !NPC.AnyNPCs(344) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 344) : Main.rand.Next(4) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 350) : Main.rand.Next(3) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, Main.rand.Next(338, 341)) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 342);
							break;
						case 3:
							newNPC = Main.rand.Next(8) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 348) : Main.rand.Next(4) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 350) : Main.rand.Next(3) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, Main.rand.Next(338, 341)) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 342);
							break;
						case 2:
							newNPC = Main.rand.Next(3) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, Main.rand.Next(338, 341)) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 350);
							break;
						default:
							newNPC = Main.rand.Next(3) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, Main.rand.Next(338, 341)) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 342);
							break;
					}
				}
			}
			else if ((Main.remixWorld || (double)num24 <= Main.worldSurface) && !Main.dayTime && Main.pumpkinMoon)
			{
				int num108 = NPC.waveNumber;
				if (NPC.waveNumber >= 20)
				{
					if (!(num57 >= (float)num46 * num67))
					{
						if (Main.rand.Next(2) == 0 && NPC.CountNPCS(327) < 2)
						{
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 327);
						}
						else if (Main.rand.Next(3) != 0 && NPC.CountNPCS(325) < 2)
						{
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 325);
						}
						else if (NPC.CountNPCS(315) < 3)
						{
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 315);
						}
					}
				}
				else
				{
					switch (num108)
					{
						case 19:
							if (Main.rand.Next(5) == 0 && NPC.CountNPCS(327) < 2)
							{
								newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 327);
							}
							else if (Main.rand.Next(5) == 0 && NPC.CountNPCS(325) < 2)
							{
								newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 325);
							}
							else if (!(num57 >= (float)num46 * num67) && NPC.CountNPCS(315) < 5)
							{
								newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 315);
							}
							break;
						case 18:
							if (Main.rand.Next(7) == 0 && NPC.CountNPCS(327) < 2)
							{
								newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 327);
							}
							newNPC = Main.rand.Next(7) == 0 && NPC.CountNPCS(325) < 2 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 325) : Main.rand.Next(7) != 0 || NPC.CountNPCS(315) >= 3 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 330) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 315);
							break;
						case 17:
							if (Main.rand.Next(7) == 0 && NPC.CountNPCS(327) < 2)
							{
								newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 327);
							}
							newNPC = Main.rand.Next(7) == 0 && NPC.CountNPCS(325) < 2 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 325) : Main.rand.Next(7) == 0 && NPC.CountNPCS(315) < 2 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 315) : Main.rand.Next(3) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 329) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 330);
							break;
						case 16:
							newNPC = Main.rand.Next(10) == 0 && NPC.CountNPCS(327) < 2 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 327) : Main.rand.Next(10) == 0 && NPC.CountNPCS(315) < 2 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 315) : Main.rand.Next(6) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 330) : Main.rand.Next(3) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 326) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 329);
							break;
						case 15:
							if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(327))
							{
								newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 327);
							}
							newNPC = Main.rand.Next(7) == 0 && NPC.CountNPCS(325) < 2 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 325) : Main.rand.Next(5) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 330) : Main.rand.Next(3) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, Main.rand.Next(305, 315)) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 326);
							break;
						case 14:
							if (Main.rand.Next(10) == 0 && !NPC.AnyNPCs(327))
							{
								newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 327);
							}
							newNPC = Main.rand.Next(7) == 0 && NPC.CountNPCS(325) < 2 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 325) : Main.rand.Next(10) == 0 && !NPC.AnyNPCs(315) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 315) : Main.rand.Next(10) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 330) : Main.rand.Next(7) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 329) : Main.rand.Next(3) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, Main.rand.Next(305, 315)) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 326);
							break;
						case 13:
							newNPC = Main.rand.Next(7) == 0 && NPC.CountNPCS(325) < 2 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 325) : Main.rand.Next(10) == 0 && NPC.CountNPCS(315) < 2 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 315) : Main.rand.Next(6) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 330) : Main.rand.Next(3) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 326) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 329);
							break;
						case 12:
							newNPC = Main.rand.Next(5) != 0 || NPC.AnyNPCs(327) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 330) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 327);
							break;
						case 11:
							newNPC = Main.rand.Next(7) == 0 && NPC.CountNPCS(325) < 2 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 325) : Main.rand.Next(3) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 326) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 330);
							break;
						case 10:
							newNPC = Main.rand.Next(10) == 0 && !NPC.AnyNPCs(327) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 327) : Main.rand.Next(3) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, Main.rand.Next(305, 315)) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 329);
							break;
						case 9:
							newNPC = Main.rand.Next(10) == 0 && NPC.CountNPCS(325) < 2 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 325) : Main.rand.Next(8) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 330) : Main.rand.Next(5) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 329) : Main.rand.Next(2) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, Main.rand.Next(305, 315)) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 326);
							break;
						case 8:
							newNPC = Main.rand.Next(8) == 0 && NPC.CountNPCS(315) < 2 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 315) : Main.rand.Next(4) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 329) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 330);
							break;
						case 7:
							newNPC = Main.rand.Next(7) == 0 && NPC.CountNPCS(325) < 2 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 325) : Main.rand.Next(4) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 329) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 330);
							break;
						case 6:
							newNPC = Main.rand.Next(7) == 0 && NPC.CountNPCS(325) < 2 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 325) : Main.rand.Next(2) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, Main.rand.Next(305, 315)) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 326);
							break;
						case 5:
							newNPC = Main.rand.Next(10) != 0 || NPC.AnyNPCs(315) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 329) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 315);
							break;
						case 4:
							newNPC = Main.rand.Next(8) == 0 && !NPC.AnyNPCs(325) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 330) : Main.rand.Next(2) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, Main.rand.Next(305, 315)) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 326);
							break;
						case 3:
							newNPC = Main.rand.Next(3) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 326) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 329);
							break;
						case 2:
							newNPC = Main.rand.Next(3) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, Main.rand.Next(305, 315)) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 326);
							break;
						default:
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, Main.rand.Next(305, 315));
							break;
					}
				}
			}
			else if (((double)num24 <= Main.worldSurface || Main.remixWorld && (double)num24 > Main.rockLayer) && Main.dayTime && Main.eclipse)
			{
				bool flag29 = false;
				if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
				{
					flag29 = true;
				}
				newNPC = NPC.downedPlantBoss && Main.rand.Next(80) == 0 && !NPC.AnyNPCs(477) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 477) : Main.rand.Next(50) == 0 && !NPC.AnyNPCs(251) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 251) : NPC.downedPlantBoss && Main.rand.Next(5) == 0 && !NPC.AnyNPCs(466) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 466) : NPC.downedPlantBoss && Main.rand.Next(20) == 0 && !NPC.AnyNPCs(463) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 463) : NPC.downedPlantBoss && Main.rand.Next(20) == 0 && NPC.CountNPCS(467) < 2 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 467) : Main.rand.Next(15) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 159) : flag29 && Main.rand.Next(13) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 253) : Main.rand.Next(8) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 469) : NPC.downedPlantBoss && Main.rand.Next(7) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 468) : NPC.downedPlantBoss && Main.rand.Next(5) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 460) : Main.rand.Next(4) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 162) : Main.rand.Next(3) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 461) : Main.rand.Next(2) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 166) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 462);
			}
			else if (NPC.SpawnNPC_CheckToSpawnUndergroundFairy(num, num24, k))
			{
				int num109 = Main.rand.Next(583, 586);
				if (Main.tenthAnniversaryWorld && !Main.getGoodWorld && Main.rand.Next(4) != 0)
				{
					num109 = 583;
				}
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, num109);
				Main.npc[newNPC].ai[2] = 2f;
				Main.npc[newNPC].TargetClosest();
				Main.npc[newNPC].ai[3] = 0f;
			}
			else if (!Main.remixWorld && !flag36 && (!Main.dayTime || Main.tile[num, num24].wall > 0) && Main.tile[num89, num100].wall == 244 && !Main.eclipse && !Main.bloodMoon && Main.player[k].RollLuck(30) == 0 && NPC.CountNPCS(624) <= Main.rand.Next(3))
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 624);
			}
			else if (!Main.player[k].ZoneCorrupt && !Main.player[k].ZoneCrimson && !flag36 && !Main.eclipse && !Main.bloodMoon && Main.player[k].RollLuck(range) == 0 && (!Main.remixWorld && (double)num24 >= Main.worldSurface * 0.800000011920929 && (double)num24 < Main.worldSurface * 1.100000023841858 || Main.remixWorld && (double)num24 > Main.rockLayer && num24 < Main.maxTilesY - 350) && NPC.CountNPCS(624) <= Main.rand.Next(3) && (!Main.dayTime || Main.tile[num, num24].wall > 0) && (Main.tile[num, num24].wall == 63 || Main.tile[num, num24].wall == 2 || Main.tile[num, num24].wall == 196 || Main.tile[num, num24].wall == 197 || Main.tile[num, num24].wall == 198 || Main.tile[num, num24].wall == 199))
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 624);
			}
			else if (Main.hardMode && num35 == 70 && flag36)
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 256);
			}
			else if (num35 == 70 && (double)num24 <= Main.worldSurface && Main.rand.Next(3) != 0)
			{
				if (!Main.hardMode && Main.rand.Next(6) == 0 || Main.rand.Next(12) == 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 360);
				}
				else if (Main.rand.Next(3) != 0)
				{
					newNPC = Main.rand.Next(2) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 255) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 254);
				}
				else if (Main.rand.Next(4) != 0)
				{
					newNPC = Main.rand.Next(2) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 258) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 257);
				}
				else if (Main.hardMode && Main.rand.Next(3) != 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 260);
					Main.npc[newNPC].ai[0] = num;
					Main.npc[newNPC].ai[1] = num24;
					Main.npc[newNPC].netUpdate = true;
				}
				else
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 259);
					Main.npc[newNPC].ai[0] = num;
					Main.npc[newNPC].ai[1] = num24;
					Main.npc[newNPC].netUpdate = true;
				}
			}
			else if (num35 == 70 && Main.hardMode && (double)num24 >= Main.worldSurface && Main.rand.Next(3) != 0 && (!Main.remixWorld || Main.getGoodWorld || num24 < Main.maxTilesY - 360))
			{
				if (Main.hardMode && Main.player[k].RollLuck(5) == 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 374);
				}
				else if (!Main.hardMode && Main.rand.Next(4) == 0 || Main.rand.Next(8) == 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 360);
				}
				else if (Main.rand.Next(4) != 0)
				{
					newNPC = Main.rand.Next(2) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 258) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 257);
				}
				else if (Main.hardMode && Main.rand.Next(3) != 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 260);
					Main.npc[newNPC].ai[0] = num;
					Main.npc[newNPC].ai[1] = num24;
					Main.npc[newNPC].netUpdate = true;
				}
				else
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 259);
					Main.npc[newNPC].ai[0] = num;
					Main.npc[newNPC].ai[1] = num24;
					Main.npc[newNPC].netUpdate = true;
				}
			}
			else if (Main.player[k].ZoneCorrupt && Main.rand.Next(maxValue) == 0 && !flag34)
			{
				newNPC = !Main.hardMode || Main.rand.Next(4) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 7, 1) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 98, 1);
			}
			else if (Main.remixWorld && !Main.hardMode && (double)num24 > Main.worldSurface && Main.player[k].RollLuck(100) == 0)
			{
				newNPC = !Main.player[k].ZoneSnow ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 85) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 629);
			}
			else if (Main.hardMode && (double)num24 > Main.worldSurface && Main.player[k].RollLuck(Main.tenthAnniversaryWorld ? 25 : 75) == 0)
			{
				newNPC = Main.rand.Next(2) == 0 && Main.player[k].ZoneCorrupt && !NPC.AnyNPCs(473) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 473) : Main.rand.Next(2) == 0 && Main.player[k].ZoneCrimson && !NPC.AnyNPCs(474) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 474) : Main.rand.Next(2) == 0 && Main.player[k].ZoneHallow && !NPC.AnyNPCs(475) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 475) : Main.tenthAnniversaryWorld && Main.rand.Next(2) == 0 && Main.player[k].ZoneJungle && !NPC.AnyNPCs(476) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 476) : !Main.player[k].ZoneSnow ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 85) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 629);
			}
			else if (Main.hardMode && Main.tile[num, num24].wall == 2 && Main.rand.Next(20) == 0)
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 85);
			}
			else if (Main.hardMode && (double)num24 <= Main.worldSurface && !Main.dayTime && (Main.rand.Next(20) == 0 || Main.rand.Next(5) == 0 && Main.moonPhase == 4))
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 82);
			}
			else if (Main.hardMode && Main.halloween && (double)num24 <= Main.worldSurface && !Main.dayTime && Main.rand.Next(10) == 0)
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 304);
			}
			else if (num56 == 60 && Main.player[k].RollLuck(500) == 0 && !Main.dayTime)
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 52);
			}
			else if (num56 == 60 && (double)num24 > Main.worldSurface && Main.rand.Next(60) == 0)
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 219);
			}
			else if ((double)num24 > Main.worldSurface && num24 < Main.maxTilesY - 210 && !Main.player[k].ZoneSnow && !Main.player[k].ZoneCrimson && !Main.player[k].ZoneCorrupt && !Main.player[k].ZoneJungle && !Main.player[k].ZoneHallow && Main.rand.Next(8) == 0)
			{
				if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 448);
				}
				else
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 357);
				}
			}
			else if ((double)num24 > Main.worldSurface && num24 < Main.maxTilesY - 210 && !Main.player[k].ZoneSnow && !Main.player[k].ZoneCrimson && !Main.player[k].ZoneCorrupt && !Main.player[k].ZoneJungle && !Main.player[k].ZoneHallow && Main.rand.Next(13) == 0)
			{
				if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 447);
				}
				else
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 300);
				}
			}
			else if ((double)num24 > Main.worldSurface && (double)num24 < (Main.rockLayer + Main.maxTilesY) / 2.0 && !Main.player[k].ZoneSnow && !Main.player[k].ZoneCrimson && !Main.player[k].ZoneCorrupt && !Main.player[k].ZoneHallow && Main.rand.Next(13) == 0)
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 359);
			}
			else if (flag13 && Main.player[k].ZoneJungle && !Main.player[k].ZoneCrimson && !Main.player[k].ZoneCorrupt && Main.rand.Next(7) == 0)
			{
				if (Main.dayTime && Main.time < 43200.00064373016 && Main.rand.Next(3) != 0)
				{
					switch (Main.rand.Next(5))
					{
						case 0:
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 671);
							break;
						case 1:
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 672);
							break;
						case 2:
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 673);
							break;
						case 3:
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 674);
							break;
						default:
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 675);
							break;
					}
				}
				else
				{
					NPC.SpawnNPC_SpawnFrog(num, num24, k);
				}
			}
			else if (num56 == 225 && Main.rand.Next(2) == 0)
			{
				if (Main.hardMode && Main.rand.Next(4) != 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 176);
					if (Main.rand.Next(10) == 0)
					{
						Main.npc[newNPC].SetDefaults(-18);
					}
					if (Main.rand.Next(10) == 0)
					{
						Main.npc[newNPC].SetDefaults(-19);
					}
					if (Main.rand.Next(10) == 0)
					{
						Main.npc[newNPC].SetDefaults(-20);
					}
					if (Main.rand.Next(10) == 0)
					{
						Main.npc[newNPC].SetDefaults(-21);
					}
				}
				else
				{
					switch (Main.rand.Next(8))
					{
						case 0:
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 231);
							if (Main.rand.Next(4) == 0)
							{
								Main.npc[newNPC].SetDefaults(-56);
							}
							else if (Main.rand.Next(4) == 0)
							{
								Main.npc[newNPC].SetDefaults(-57);
							}
							break;
						case 1:
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 232);
							if (Main.rand.Next(4) == 0)
							{
								Main.npc[newNPC].SetDefaults(-58);
							}
							else if (Main.rand.Next(4) == 0)
							{
								Main.npc[newNPC].SetDefaults(-59);
							}
							break;
						case 2:
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 233);
							if (Main.rand.Next(4) == 0)
							{
								Main.npc[newNPC].SetDefaults(-60);
							}
							else if (Main.rand.Next(4) == 0)
							{
								Main.npc[newNPC].SetDefaults(-61);
							}
							break;
						case 3:
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 234);
							if (Main.rand.Next(4) == 0)
							{
								Main.npc[newNPC].SetDefaults(-62);
							}
							else if (Main.rand.Next(4) == 0)
							{
								Main.npc[newNPC].SetDefaults(-63);
							}
							break;
						case 4:
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 235);
							if (Main.rand.Next(4) == 0)
							{
								Main.npc[newNPC].SetDefaults(-64);
							}
							else if (Main.rand.Next(4) == 0)
							{
								Main.npc[newNPC].SetDefaults(-65);
							}
							break;
						default:
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 42);
							if (Main.rand.Next(4) == 0)
							{
								Main.npc[newNPC].SetDefaults(-16);
							}
							else if (Main.rand.Next(4) == 0)
							{
								Main.npc[newNPC].SetDefaults(-17);
							}
							break;
					}
				}
			}
			else if (num56 == 60 && Main.hardMode && Main.rand.Next(3) != 0)
			{
				if (flag13 && !Main.dayTime && Main.rand.Next(3) == 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 152);
				}
				else if (flag13 && Main.dayTime && Main.rand.Next(4) != 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 177);
				}
				else if ((double)num24 > Main.worldSurface && Main.rand.Next(100) == 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 205);
				}
				else if ((double)num24 > Main.worldSurface && Main.rand.Next(5) == 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 236);
				}
				else if ((double)num24 > Main.worldSurface && Main.rand.Next(4) != 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 176);
					if (Main.rand.Next(10) == 0)
					{
						Main.npc[newNPC].SetDefaults(-18);
					}
					if (Main.rand.Next(10) == 0)
					{
						Main.npc[newNPC].SetDefaults(-19);
					}
					if (Main.rand.Next(10) == 0)
					{
						Main.npc[newNPC].SetDefaults(-20);
					}
					if (Main.rand.Next(10) == 0)
					{
						Main.npc[newNPC].SetDefaults(-21);
					}
				}
				else if (Main.rand.Next(3) == 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 175);
					Main.npc[newNPC].ai[0] = num;
					Main.npc[newNPC].ai[1] = num24;
					Main.npc[newNPC].netUpdate = true;
				}
				else
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 153);
				}
			}
			else if ((num56 == 226 || num56 == 232) && flag33 || Main.remixWorld && flag33)
			{
				newNPC = Main.rand.Next(3) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 198) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 226);
			}
			else if (num58 == 86 && Main.rand.Next(8) != 0)
			{
				switch (Main.rand.Next(8))
				{
					case 0:
						newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 231);
						if (Main.rand.Next(4) == 0)
						{
							Main.npc[newNPC].SetDefaults(-56);
						}
						else if (Main.rand.Next(4) == 0)
						{
							Main.npc[newNPC].SetDefaults(-57);
						}
						break;
					case 1:
						newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 232);
						if (Main.rand.Next(4) == 0)
						{
							Main.npc[newNPC].SetDefaults(-58);
						}
						else if (Main.rand.Next(4) == 0)
						{
							Main.npc[newNPC].SetDefaults(-59);
						}
						break;
					case 2:
						newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 233);
						if (Main.rand.Next(4) == 0)
						{
							Main.npc[newNPC].SetDefaults(-60);
						}
						else if (Main.rand.Next(4) == 0)
						{
							Main.npc[newNPC].SetDefaults(-61);
						}
						break;
					case 3:
						newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 234);
						if (Main.rand.Next(4) == 0)
						{
							Main.npc[newNPC].SetDefaults(-62);
						}
						else if (Main.rand.Next(4) == 0)
						{
							Main.npc[newNPC].SetDefaults(-63);
						}
						break;
					case 4:
						newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 235);
						if (Main.rand.Next(4) == 0)
						{
							Main.npc[newNPC].SetDefaults(-64);
						}
						else if (Main.rand.Next(4) == 0)
						{
							Main.npc[newNPC].SetDefaults(-65);
						}
						break;
					default:
						newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 42);
						if (Main.rand.Next(4) == 0)
						{
							Main.npc[newNPC].SetDefaults(-16);
						}
						else if (Main.rand.Next(4) == 0)
						{
							Main.npc[newNPC].SetDefaults(-17);
						}
						break;
				}
			}
			else if (num56 == 60 && (!Main.remixWorld && (double)num24 > (Main.worldSurface + Main.rockLayer) / 2.0 || Main.remixWorld && ((double)num24 < Main.rockLayer || Main.rand.Next(2) == 0)))
			{
				if (Main.rand.Next(4) == 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 204);
				}
				else if (Main.rand.Next(4) == 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 43);
					Main.npc[newNPC].ai[0] = num;
					Main.npc[newNPC].ai[1] = num24;
					Main.npc[newNPC].netUpdate = true;
				}
				else
				{
					switch (Main.rand.Next(8))
					{
						case 0:
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 231);
							if (Main.rand.Next(4) == 0)
							{
								Main.npc[newNPC].SetDefaults(-56);
							}
							else if (Main.rand.Next(4) == 0)
							{
								Main.npc[newNPC].SetDefaults(-57);
							}
							break;
						case 1:
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 232);
							if (Main.rand.Next(4) == 0)
							{
								Main.npc[newNPC].SetDefaults(-58);
							}
							else if (Main.rand.Next(4) == 0)
							{
								Main.npc[newNPC].SetDefaults(-59);
							}
							break;
						case 2:
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 233);
							if (Main.rand.Next(4) == 0)
							{
								Main.npc[newNPC].SetDefaults(-60);
							}
							else if (Main.rand.Next(4) == 0)
							{
								Main.npc[newNPC].SetDefaults(-61);
							}
							break;
						case 3:
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 234);
							if (Main.rand.Next(4) == 0)
							{
								Main.npc[newNPC].SetDefaults(-62);
							}
							else if (Main.rand.Next(4) == 0)
							{
								Main.npc[newNPC].SetDefaults(-63);
							}
							break;
						case 4:
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 235);
							if (Main.rand.Next(4) == 0)
							{
								Main.npc[newNPC].SetDefaults(-64);
							}
							else if (Main.rand.Next(4) == 0)
							{
								Main.npc[newNPC].SetDefaults(-65);
							}
							break;
						default:
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 42);
							if (Main.rand.Next(4) == 0)
							{
								Main.npc[newNPC].SetDefaults(-16);
							}
							else if (Main.rand.Next(4) == 0)
							{
								Main.npc[newNPC].SetDefaults(-17);
							}
							break;
					}
				}
			}
			else if (num56 == 60 && Main.rand.Next(4) == 0)
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 51);
			}
			else if (num56 == 60 && Main.rand.Next(8) == 0)
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 56);
				Main.npc[newNPC].ai[0] = num;
				Main.npc[newNPC].ai[1] = num24;
				Main.npc[newNPC].netUpdate = true;
			}
			else if (Sandstorm.Happening && Main.player[k].ZoneSandstorm && TileID.Sets.Conversion.Sand[num56] && NPC.Spawning_SandstoneCheck(num, num24))
			{
				if (!NPC.downedBoss1 && !Main.hardMode)
				{
					newNPC = Main.rand.Next(2) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 546) : Main.rand.Next(2) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 69) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 61);
				}
				else if (Main.hardMode && Main.rand.Next(20) == 0 && !NPC.AnyNPCs(541))
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 541);
				}
				else if (Main.hardMode && !flag34 && Main.rand.Next(3) == 0 && NPC.CountNPCS(510) < 4)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, (num24 + 10) * 16, 510);
				}
				else if (!Main.hardMode || flag34 || Main.rand.Next(2) != 0)
				{
					newNPC = Main.hardMode && num56 == 53 && Main.rand.Next(3) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 78) : Main.hardMode && num56 == 112 && Main.rand.Next(3) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 79) : Main.hardMode && num56 == 234 && Main.rand.Next(3) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 630) : Main.hardMode && num56 == 116 && Main.rand.Next(3) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 80) : Main.rand.Next(2) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 546) : Main.rand.Next(2) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 581) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 580);
				}
				else
				{
					int num110 = 542;
					if (TileID.Sets.Corrupt[num56])
					{
						num110 = 543;
					}
					if (TileID.Sets.Crimson[num56])
					{
						num110 = 544;
					}
					if (TileID.Sets.Hallow[num56])
					{
						num110 = 545;
					}
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, num110);
				}
			}
			else if (Main.hardMode && num56 == 53 && Main.rand.Next(3) == 0)
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 78);
			}
			else if (Main.hardMode && num56 == 112 && Main.rand.Next(2) == 0)
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 79);
			}
			else if (Main.hardMode && num56 == 234 && Main.rand.Next(2) == 0)
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 630);
			}
			else if (Main.hardMode && num56 == 116 && Main.rand.Next(2) == 0)
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 80);
			}
			else if (Main.hardMode && !flag36 && flag9 && (num56 == 116 || num56 == 117 || num56 == 109 || num56 == 164))
			{
				if (NPC.downedPlantBoss && (Main.remixWorld || !Main.dayTime && Main.time < 16200.0) && flag13 && Main.player[k].RollLuck(10) == 0 && !NPC.AnyNPCs(661))
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 661);
				}
				else if (!flag17 || NPC.AnyNPCs(244) || Main.rand.Next(12) != 0)
				{
					newNPC = !Main.dayTime && Main.rand.Next(2) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 122) : Main.rand.Next(10) != 0 && (!Main.player[k].ZoneWaterCandle || Main.rand.Next(10) != 0) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 75) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 86);
				}
				else
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 244);
				}
			}
			else if (!flag34 && Main.hardMode && Main.rand.Next(50) == 0 && !flag36 && flag14 && (num56 == 116 || num56 == 117 || num56 == 109 || num56 == 164))
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 84);
			}
			else if (num56 == 204 && Main.player[k].ZoneCrimson || num56 == 199 || num56 == 200 || num56 == 203 || num56 == 234 || num56 == 662)
			{
				bool flag30 = (double)num24 >= Main.rockLayer;
				if (Main.remixWorld)
				{
					flag30 = (double)num24 <= Main.rockLayer;
				}
				if (Main.hardMode && flag30 && Main.rand.Next(40) == 0 && !flag34)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 179);
				}
				else if (Main.hardMode && flag30 && Main.rand.Next(5) == 0 && !flag34)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 182);
				}
				else if (Main.hardMode && flag30 && Main.rand.Next(2) == 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 268);
				}
				else if (Main.hardMode && Main.rand.Next(3) == 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 183);
					if (Main.rand.Next(3) == 0)
					{
						Main.npc[newNPC].SetDefaults(-24);
					}
					else if (Main.rand.Next(3) == 0)
					{
						Main.npc[newNPC].SetDefaults(-25);
					}
				}
				else if (Main.hardMode && (Main.rand.Next(2) == 0 || (double)num24 > Main.worldSurface && !Main.remixWorld))
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 174);
				}
				else if (Main.tile[num, num24].wall > 0 && Main.rand.Next(4) != 0 || Main.rand.Next(8) == 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 239);
				}
				else if (Main.rand.Next(2) == 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 181);
				}
				else
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 173);
					if (Main.rand.Next(3) == 0)
					{
						Main.npc[newNPC].SetDefaults(-22);
					}
					else if (Main.rand.Next(3) == 0)
					{
						Main.npc[newNPC].SetDefaults(-23);
					}
				}
			}
			else if (num56 == 22 && Main.player[k].ZoneCorrupt || num56 == 23 || num56 == 25 || num56 == 112 || num56 == 163 || num56 == 661)
			{
				bool flag31 = (double)num24 >= Main.rockLayer;
				if (Main.remixWorld)
				{
					flag31 = (double)num24 <= Main.rockLayer;
				}
				if (Main.hardMode && flag31 && Main.rand.Next(40) == 0 && !flag34)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 83);
				}
				else if (Main.hardMode && flag31 && Main.rand.Next(3) == 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 101);
					Main.npc[newNPC].ai[0] = num;
					Main.npc[newNPC].ai[1] = num24;
					Main.npc[newNPC].netUpdate = true;
				}
				else if (Main.hardMode && Main.rand.Next(3) == 0)
				{
					newNPC = Main.rand.Next(3) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 81) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 121);
				}
				else if (Main.hardMode && (Main.rand.Next(2) == 0 || flag31))
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 94);
				}
				else
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 6);
					if (Main.rand.Next(3) == 0)
					{
						Main.npc[newNPC].SetDefaults(-11);
					}
					else if (Main.rand.Next(3) == 0)
					{
						Main.npc[newNPC].SetDefaults(-12);
					}
				}
			}
			else if (flag13)
			{
				bool flag32 = (float)Math.Abs(num - Main.maxTilesX / 2) / (float)(Main.maxTilesX / 2) > 0.33f;
				if (flag32 && NPC.AnyDanger())
				{
					flag32 = false;
				}
				if (Main.player[k].ZoneGraveyard && !flag36 && (num35 == 2 || num35 == 477) && Main.rand.Next(10) == 0)
				{
					if (Main.rand.Next(2) == 0)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 606);
					}
					else
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 610);
					}
				}
				else if (Main.player[k].ZoneSnow && Main.hardMode && flag17 && !NPC.AnyNPCs(243) && Main.player[k].RollLuck(20) == 0)
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 243);
				}
				else if (!Main.player[k].ZoneSnow && Main.hardMode && flag17 && NPC.CountNPCS(250) < 2 && Main.rand.Next(10) == 0)
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 250);
				}
				else if (flag32 && Main.hardMode && NPC.downedGolemBoss && (!NPC.downedMartians && Main.rand.Next(100) == 0 || Main.rand.Next(400) == 0) && !NPC.AnyNPCs(399))
				{
					NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 399);
				}
				else if (!Main.player[k].ZoneGraveyard && Main.dayTime)
				{
					int num3 = Math.Abs(num - Main.spawnTileX);
					if (!flag36 && num3 < Main.maxTilesX / 2 && Main.rand.Next(15) == 0 && (num56 == 2 || num56 == 477 || num56 == 109 || num56 == 492 || num56 == 147 || num56 == 161))
					{
						if (num56 == 147 || num56 == 161)
						{
							if (Main.rand.Next(2) == 0)
							{
								NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 148);
							}
							else
							{
								NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 149);
							}
						}
						else if (!tooWindyForButterflies && !Main.raining && Main.dayTime && Main.rand.Next(NPC.stinkBugChance) == 0 && flag13)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 669);
							if (Main.rand.Next(4) == 0)
							{
								NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8 - 16, num24 * 16, 669);
							}
							if (Main.rand.Next(4) == 0)
							{
								NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8 + 16, num24 * 16, 669);
							}
						}
						else if (!tooWindyForButterflies && !Main.raining && Main.dayTime && Main.rand.Next(NPC.butterflyChance) == 0 && flag13)
						{
							if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
							{
								NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 444);
							}
							else
							{
								NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 356);
							}
							if (Main.rand.Next(4) == 0)
							{
								NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8 - 16, num24 * 16, 356);
							}
							if (Main.rand.Next(4) == 0)
							{
								NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8 + 16, num24 * 16, 356);
							}
						}
						else if (tooWindyForButterflies && !Main.raining && Main.dayTime && Main.rand.Next(NPC.butterflyChance / 2) == 0 && flag13)
						{
							if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
							{
								NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 605);
							}
							else
							{
								NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 604);
							}
							if (Main.rand.Next(3) != 0)
							{
								NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 604);
							}
							if (Main.rand.Next(2) == 0)
							{
								NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 604);
							}
							if (Main.rand.Next(3) == 0)
							{
								NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 604);
							}
							if (Main.rand.Next(4) == 0)
							{
								NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 604);
							}
						}
						else if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 443);
						}
						else if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0 && (double)num24 <= Main.worldSurface)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 539);
						}
						else if (Main.halloween && Main.rand.Next(3) != 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 303);
						}
						else if (Main.xMas && Main.rand.Next(3) != 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 337);
						}
						else if (BirthdayParty.PartyIsUp && Main.rand.Next(3) != 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 540);
						}
						else if (Main.rand.Next(3) == 0 && (double)num24 <= Main.worldSurface)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, Terraria.Utils.SelectRandom(Main.rand, new short[2] { 299, 538 }));
						}
						else
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 46);
						}
					}
					else if (!flag36 && num > WorldGen.beachDistance && num < Main.maxTilesX - WorldGen.beachDistance && Main.rand.Next(12) == 0 && num56 == 53)
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, Main.rand.Next(366, 368));
					}
					else if ((num56 == 2 || num56 == 477 || num56 == 53) && !tooWindyForButterflies && !Main.raining && Main.dayTime && Main.rand.Next(3) != 0 && ((double)num24 <= Main.worldSurface || Main.remixWorld) && NPC.FindCattailTop(num, num24, out cattailX, out cattailY))
					{
						if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), cattailX * 16 + 8, cattailY * 16, 601);
						}
						else
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), cattailX * 16 + 8, cattailY * 16, NPC.RollDragonflyType(num56));
						}
						if (Main.rand.Next(3) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), cattailX * 16 + 8 - 16, cattailY * 16, NPC.RollDragonflyType(num56));
						}
						if (Main.rand.Next(3) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), cattailX * 16 + 8 + 16, cattailY * 16, NPC.RollDragonflyType(num56));
						}
					}
					else if (!flag36 && num3 < Main.maxTilesX / 3 && Main.dayTime && Main.time < 18000.0 && (num56 == 2 || num56 == 477 || num56 == 109 || num56 == 492) && Main.rand.Next(4) == 0 && (double)num24 <= Main.worldSurface && NPC.CountNPCS(74) + NPC.CountNPCS(297) + NPC.CountNPCS(298) < 6)
					{
						int num4 = Main.rand.Next(4);
						if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 442);
						}
						else
						{
							switch (num4)
							{
								case 0:
									NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 297);
									break;
								case 1:
									NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 298);
									break;
								default:
									NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 74);
									break;
							}
						}
					}
					else if (!flag36 && num3 < Main.maxTilesX / 3 && Main.rand.Next(15) == 0 && (num56 == 2 || num56 == 477 || num56 == 109 || num56 == 492 || num56 == 147))
					{
						int num5 = Main.rand.Next(4);
						if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 442);
						}
						else
						{
							switch (num5)
							{
								case 0:
									NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 297);
									break;
								case 1:
									NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 298);
									break;
								default:
									NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 74);
									break;
							}
						}
					}
					else if (!flag36 && num3 > Main.maxTilesX / 3 && num56 == 2 && Main.rand.Next(300) == 0 && !NPC.AnyNPCs(50))
					{
						NPC.SpawnOnPlayer(k, 50);
					}
					else if (!flag7 && num56 == 53 && (num < WorldGen.beachDistance || num > Main.maxTilesX - WorldGen.beachDistance))
					{
						if (!flag36 && Main.rand.Next(10) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 602);
						}
						else if (flag36)
						{
							int num6 = -1;
							int num7 = -1;
							if ((double)num24 < Main.worldSurface && num24 > 50)
							{
								for (int num8 = num24 - 1; num8 > num24 - 50; num8--)
								{
									if (Main.tile[num, num8].liquid == 0 && !WorldGen.SolidTile(num, num8) && !WorldGen.SolidTile(num, num8 + 1) && !WorldGen.SolidTile(num, num8 + 2))
									{
										num6 = num8 + 2;
										if (!WorldGen.SolidTile(num, num6 + 1) && !WorldGen.SolidTile(num, num6 + 2))
										{
											num7 = num6 + 2;
										}
										break;
									}
								}
								if (num6 > num24)
								{
									num6 = num24;
								}
								if (num7 > num24)
								{
									num7 = num24;
								}
							}
							if (Main.rand.Next(10) == 0)
							{
								int num9 = Main.rand.Next(3);
								if (num9 == 0 && num6 > 0)
								{
									NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num6 * 16, 625);
								}
								else if (num9 == 1 && num7 > 0)
								{
									NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num7 * 16, 615);
								}
								else if (num9 == 2)
								{
									int num10 = num24;
									if (num7 > 0)
									{
										num10 = num7;
									}
									if (Main.player[k].RollLuck(NPC.goldCritterChance) == 0)
									{
										NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num10 * 16, 627);
									}
									else
									{
										NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num10 * 16, 626);
									}
								}
							}
						}
					}
					else if (!flag36 && num56 == 53 && Main.rand.Next(5) == 0 && NPC.Spawning_SandstoneCheck(num, num24) && !flag36)
					{
						newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 69);
					}
					else if (num56 == 53 && !flag36)
					{
						newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 61);
					}
					else if (!flag36 && (num3 > Main.maxTilesX / 3 || Main.remixWorld) && (Main.rand.Next(15) == 0 || !NPC.downedGoblins && WorldGen.shadowOrbSmashed && Main.rand.Next(7) == 0))
					{
						newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 73);
					}
					else if (Main.raining && Main.rand.Next(4) == 0)
					{
						newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 224);
					}
					else if (!flag36 && Main.raining && Main.rand.Next(2) == 0)
					{
						newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 225);
					}
					else if (!flag36 && num58 == 0 && isItAHappyWindyDay && flag11 && Main.rand.Next(3) != 0)
					{
						newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 594);
					}
					else if (!flag36 && num58 == 0 && (num35 == 2 || num35 == 477) && isItAHappyWindyDay && flag11 && Main.rand.Next(10) != 0)
					{
						newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 628);
					}
					else if (!flag36)
					{
						newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 1);
						switch (num56)
						{
							case 60:
								Main.npc[newNPC].SetDefaults(-10);
								break;
							case 147:
							case 161:
								Main.npc[newNPC].SetDefaults(147);
								break;
							default:
								if (Main.halloween && Main.rand.Next(3) != 0)
								{
									Main.npc[newNPC].SetDefaults(302);
								}
								else if (Main.xMas && Main.rand.Next(3) != 0)
								{
									Main.npc[newNPC].SetDefaults(Main.rand.Next(333, 337));
								}
								else if (Main.rand.Next(3) == 0 || num3 < 200 && !Main.expertMode)
								{
									Main.npc[newNPC].SetDefaults(-3);
								}
								else if (Main.rand.Next(10) == 0 && (num3 > 400 || Main.expertMode))
								{
									Main.npc[newNPC].SetDefaults(-7);
								}
								break;
						}
					}
				}
				else
				{
					if (!Main.player[k].ZoneGraveyard && !tooWindyForButterflies && (num56 == 2 || num56 == 477 || num56 == 109 || num56 == 492) && !Main.raining && Main.rand.Next(NPC.fireFlyChance) == 0 && (double)num24 <= Main.worldSurface)
					{
						int num11 = 355;
						if (num56 == 109)
						{
							num11 = 358;
						}
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, num11);
						if (Main.rand.Next(NPC.fireFlyMultiple) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8 - 16, num24 * 16, num11);
						}
						if (Main.rand.Next(NPC.fireFlyMultiple) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8 + 16, num24 * 16, num11);
						}
						if (Main.rand.Next(NPC.fireFlyMultiple) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16 - 16, num11);
						}
						if (Main.rand.Next(NPC.fireFlyMultiple) == 0)
						{
							NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16 + 16, num11);
						}
					}
					else if ((Main.halloween || Main.player[k].ZoneGraveyard) && Main.rand.Next(12) == 0)
					{
						newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 301);
					}
					else if (Main.player[k].ZoneGraveyard && Main.rand.Next(30) == 0)
					{
						newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 316);
					}
					else if (Main.player[k].ZoneGraveyard && Main.hardMode && (double)num24 <= Main.worldSurface && Main.rand.Next(10) == 0)
					{
						newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 304);
					}
					else if (Main.rand.Next(6) == 0 || Main.moonPhase == 4 && Main.rand.Next(2) == 0)
					{
						if (Main.hardMode && Main.rand.Next(3) == 0)
						{
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 133);
						}
						else if (Main.halloween && Main.rand.Next(2) == 0)
						{
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, Main.rand.Next(317, 319));
						}
						else if (Main.rand.Next(2) == 0)
						{
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 2);
							if (Main.rand.Next(4) == 0)
							{
								Main.npc[newNPC].SetDefaults(-43);
							}
						}
						else
						{
							switch (Main.rand.Next(5))
							{
								case 0:
									newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 190);
									if (Main.rand.Next(3) == 0)
									{
										Main.npc[newNPC].SetDefaults(-38);
									}
									break;
								case 1:
									newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 191);
									if (Main.rand.Next(3) == 0)
									{
										Main.npc[newNPC].SetDefaults(-39);
									}
									break;
								case 2:
									newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 192);
									if (Main.rand.Next(3) == 0)
									{
										Main.npc[newNPC].SetDefaults(-40);
									}
									break;
								case 3:
									newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 193);
									if (Main.rand.Next(3) == 0)
									{
										Main.npc[newNPC].SetDefaults(-41);
									}
									break;
								case 4:
									newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 194);
									if (Main.rand.Next(3) == 0)
									{
										Main.npc[newNPC].SetDefaults(-42);
									}
									break;
							}
						}
					}
					else if (Main.hardMode && Main.rand.Next(50) == 0 && Main.bloodMoon && !NPC.AnyNPCs(109))
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 109);
					}
					else if (Main.rand.Next(250) == 0 && (Main.bloodMoon || Main.player[k].ZoneGraveyard))
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 53);
					}
					else if (Main.rand.Next(250) == 0 && (Main.bloodMoon || Main.player[k].ZoneGraveyard))
					{
						NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 536);
					}
					else if (!Main.dayTime && Main.moonPhase == 0 && Main.hardMode && Main.rand.Next(3) != 0)
					{
						newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 104);
					}
					else if (!Main.dayTime && Main.hardMode && Main.rand.Next(3) == 0)
					{
						newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 140);
					}
					else if (Main.bloodMoon && Main.rand.Next(5) < 2)
					{
						newNPC = Main.rand.Next(2) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 490) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 489);
					}
					else if (num35 == 147 || num35 == 161 || num35 == 163 || num35 == 164 || num35 == 162)
					{
						newNPC = !Main.player[k].ZoneGraveyard && Main.hardMode && Main.rand.Next(4) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 169) : !Main.player[k].ZoneGraveyard && Main.hardMode && Main.rand.Next(3) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 155) : !Main.expertMode || Main.rand.Next(2) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 161) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 431);
					}
					else if (Main.raining && Main.rand.Next(2) == 0)
					{
						newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 223);
						if (Main.rand.Next(3) == 0)
						{
							if (Main.rand.Next(2) == 0)
							{
								Main.npc[newNPC].SetDefaults(-54);
							}
							else
							{
								Main.npc[newNPC].SetDefaults(-55);
							}
						}
					}
					else
					{
						int num12 = Main.rand.Next(7);
						int num14 = 12;
						int maxValue4 = 20;
						if (Main.player[k].ConsumedLifeCrystals == 0)
						{
							num14 = 5;
							num14 -= Main.CurrentFrameFlags.ActivePlayersCount / 2;
							if (num14 < 2)
							{
								num14 = 2;
							}
						}
						if (Main.player[k].ZoneGraveyard && Main.rand.Next(maxValue4) == 0)
						{
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 632);
						}
						else if (Main.rand.Next(num14) == 0)
						{
							newNPC = !Main.expertMode || Main.rand.Next(2) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 590) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 591);
						}
						else if (Main.halloween && Main.rand.Next(2) == 0)
						{
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, Main.rand.Next(319, 322));
						}
						else if (Main.xMas && Main.rand.Next(2) == 0)
						{
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, Main.rand.Next(331, 333));
						}
						else if (num12 == 0 && Main.expertMode && Main.rand.Next(3) == 0)
						{
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 430);
						}
						else if (num12 == 2 && Main.expertMode && Main.rand.Next(3) == 0)
						{
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 432);
						}
						else if (num12 == 3 && Main.expertMode && Main.rand.Next(3) == 0)
						{
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 433);
						}
						else if (num12 == 4 && Main.expertMode && Main.rand.Next(3) == 0)
						{
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 434);
						}
						else if (num12 == 5 && Main.expertMode && Main.rand.Next(3) == 0)
						{
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 435);
						}
						else if (num12 == 6 && Main.expertMode && Main.rand.Next(3) == 0)
						{
							newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 436);
						}
						else
						{
							switch (num12)
							{
								case 0:
									newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 3);
									if (Main.rand.Next(3) == 0)
									{
										if (Main.rand.Next(2) == 0)
										{
											Main.npc[newNPC].SetDefaults(-26);
										}
										else
										{
											Main.npc[newNPC].SetDefaults(-27);
										}
									}
									break;
								case 1:
									newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 132);
									if (Main.rand.Next(3) == 0)
									{
										if (Main.rand.Next(2) == 0)
										{
											Main.npc[newNPC].SetDefaults(-28);
										}
										else
										{
											Main.npc[newNPC].SetDefaults(-29);
										}
									}
									break;
								case 2:
									newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 186);
									if (Main.rand.Next(3) == 0)
									{
										if (Main.rand.Next(2) == 0)
										{
											Main.npc[newNPC].SetDefaults(-30);
										}
										else
										{
											Main.npc[newNPC].SetDefaults(-31);
										}
									}
									break;
								case 3:
									newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 187);
									if (Main.rand.Next(3) == 0)
									{
										if (Main.rand.Next(2) == 0)
										{
											Main.npc[newNPC].SetDefaults(-32);
										}
										else
										{
											Main.npc[newNPC].SetDefaults(-33);
										}
									}
									break;
								case 4:
									newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 188);
									if (Main.rand.Next(3) == 0)
									{
										if (Main.rand.Next(2) == 0)
										{
											Main.npc[newNPC].SetDefaults(-34);
										}
										else
										{
											Main.npc[newNPC].SetDefaults(-35);
										}
									}
									break;
								case 5:
									newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 189);
									if (Main.rand.Next(3) == 0)
									{
										if (Main.rand.Next(2) == 0)
										{
											Main.npc[newNPC].SetDefaults(-36);
										}
										else
										{
											Main.npc[newNPC].SetDefaults(-37);
										}
									}
									break;
								case 6:
									newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 200);
									if (Main.rand.Next(3) == 0)
									{
										if (Main.rand.Next(2) == 0)
										{
											Main.npc[newNPC].SetDefaults(-44);
										}
										else
										{
											Main.npc[newNPC].SetDefaults(-45);
										}
									}
									break;
							}
						}
					}
					if (Main.player[k].ZoneGraveyard)
					{
						Main.npc[newNPC].target = k;
					}
				}
			}
			else if (flag9)
			{
				if (!flag34 && Main.rand.Next(50) == 0 && !Main.player[k].ZoneSnow)
				{
					newNPC = !Main.hardMode ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 10, 1) : Main.rand.Next(3) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 10, 1) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 95, 1);
				}
				else if (Main.hardMode && Main.rand.Next(3) == 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 140);
				}
				else if (Main.hardMode && Main.rand.Next(4) != 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 141);
				}
				else if (Main.remixWorld)
				{
					if (num35 == 147 || num35 == 161 || num35 == 163 || num35 == 164 || num35 == 162 || Main.player[k].ZoneSnow)
					{
						newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 147);
					}
					else
					{
						newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 1);
						if (Main.rand.Next(3) == 0)
						{
							Main.npc[newNPC].SetDefaults(-9);
						}
						else
						{
							Main.npc[newNPC].SetDefaults(-8);
						}
					}
				}
				else if (num56 == 147 || num56 == 161 || Main.player[k].ZoneSnow)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 147);
				}
				else
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 1);
					if (Main.rand.Next(5) == 0)
					{
						Main.npc[newNPC].SetDefaults(-9);
					}
					else if (Main.rand.Next(2) == 0)
					{
						Main.npc[newNPC].SetDefaults(1);
					}
					else
					{
						Main.npc[newNPC].SetDefaults(-8);
					}
				}
			}
			else if (num24 > Main.maxTilesY - 190)
			{
				newNPC = Main.remixWorld && (double)num > Main.maxTilesX * 0.38 + 50.0 && (double)num < Main.maxTilesX * 0.62 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 59) : Main.hardMode && !NPC.savedTaxCollector && Main.rand.Next(20) == 0 && !NPC.AnyNPCs(534) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 534) : Main.rand.Next(8) == 0 ? NPC.SpawnNPC_SpawnLavaBaitCritters(num, num24) : Main.rand.Next(40) == 0 && !NPC.AnyNPCs(39) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 39, 1) : Main.rand.Next(14) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 24) : Main.rand.Next(7) != 0 ? Main.rand.Next(3) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 59) : !Main.hardMode || !NPC.downedMechBossAny || Main.rand.Next(5) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 60) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 151) : Main.rand.Next(10) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 66) : !Main.hardMode || !NPC.downedMechBossAny || Main.rand.Next(5) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 62) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 156);
			}
			else if (NPC.SpawnNPC_CheckToSpawnRockGolem(num, num24, k, num56))
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 631);
			}
			else if (Main.rand.Next(60) == 0)
			{
				newNPC = !Main.player[k].ZoneSnow ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 217) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 218);
			}
			else if ((num56 == 116 || num56 == 117 || num56 == 164) && Main.hardMode && !flag34 && Main.rand.Next(8) == 0)
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 120);
			}
			else if ((num35 == 147 || num35 == 161 || num35 == 162 || num35 == 163 || num35 == 164 || num35 == 200) && !flag34 && Main.hardMode && Main.player[k].ZoneCorrupt && Main.rand.Next(30) == 0)
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 170);
			}
			else if ((num35 == 147 || num35 == 161 || num35 == 162 || num35 == 163 || num35 == 164 || num35 == 200) && !flag34 && Main.hardMode && Main.player[k].ZoneHallow && Main.rand.Next(30) == 0)
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 171);
			}
			else if ((num35 == 147 || num35 == 161 || num35 == 162 || num35 == 163 || num35 == 164 || num35 == 200) && !flag34 && Main.hardMode && Main.player[k].ZoneCrimson && Main.rand.Next(30) == 0)
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 180);
			}
			else if (Main.hardMode && Main.player[k].ZoneSnow && Main.rand.Next(10) == 0)
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 154);
			}
			else if (!flag34 && Main.rand.Next(100) == 0 && !Main.player[k].ZoneHallow)
			{
				newNPC = Main.hardMode ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 95, 1) : !Main.player[k].ZoneSnow ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 10, 1) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 185);
			}
			else if (Main.player[k].ZoneSnow && Main.rand.Next(20) == 0)
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 185);
			}
			else if (!Main.hardMode && Main.rand.Next(10) == 0 || Main.hardMode && Main.rand.Next(20) == 0)
			{
				if (Main.player[k].ZoneSnow)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 184);
				}
				else if (Main.rand.Next(3) == 0)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 1);
					Main.npc[newNPC].SetDefaults(-6);
				}
				else
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 16);
				}
			}
			else if (!Main.hardMode && Main.rand.Next(4) == 0)
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 1);
				if (Main.player[k].ZoneJungle)
				{
					Main.npc[newNPC].SetDefaults(-10);
				}
				else if (Main.player[k].ZoneSnow)
				{
					Main.npc[newNPC].SetDefaults(184);
				}
				else
				{
					Main.npc[newNPC].SetDefaults(-6);
				}
			}
			else if (Main.rand.Next(2) != 0)
			{
				newNPC = Main.hardMode && Main.player[k].ZoneHallow & Main.rand.Next(2) == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 138) : Main.player[k].ZoneJungle ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 51) : Main.player[k].ZoneGlowshroom && (num35 == 70 || num35 == 190) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 634) : Main.hardMode && Main.player[k].ZoneHallow ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 137) : Main.hardMode && Main.rand.Next(6) > 0 ? Main.rand.Next(3) != 0 || num35 != 147 && num35 != 161 && num35 != 162 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 93) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 150) : num35 != 147 && num35 != 161 && num35 != 162 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 49) : !Main.hardMode ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 150) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 169);
			}
			else if (Main.rand.Next(35) == 0 && NPC.CountNPCS(453) == 0)
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 453);
			}
			else if (Main.rand.Next(80) == 0)
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 195);
			}
			else if (Main.hardMode && (Main.remixWorld || (double)num24 > (Main.rockLayer + Main.maxTilesY) / 2.0) && Main.rand.Next(200) == 0)
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 172);
			}
			else if ((Main.remixWorld || (double)num24 > (Main.rockLayer + Main.maxTilesY) / 2.0) && (Main.rand.Next(200) == 0 || Main.rand.Next(50) == 0 && Main.player[k].hasGemRobe && Main.player[k].armor[0].type != 238))
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 45);
			}
			else if (flag2 && Main.rand.Next(4) != 0)
			{
				newNPC = Main.rand.Next(6) == 0 || NPC.AnyNPCs(480) || !Main.hardMode ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 481) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 480);
			}
			else if (flag38 && Main.rand.Next(5) != 0)
			{
				newNPC = Main.rand.Next(6) == 0 || NPC.AnyNPCs(483) ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 482) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 483);
			}
			else if (Main.hardMode && Main.rand.Next(10) != 0)
			{
				if (Main.rand.Next(2) != 0)
				{
					newNPC = !Main.player[k].ZoneSnow ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 110) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 206);
				}
				else if (Main.player[k].ZoneSnow)
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 197);
				}
				else
				{
					newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 77);
					if ((Main.remixWorld || (double)num24 > (Main.rockLayer + Main.maxTilesY) / 2.0) && Main.rand.Next(5) == 0)
					{
						Main.npc[newNPC].SetDefaults(-15);
					}
				}
			}
			else if (!flag34 && (Main.halloween || Main.player[k].ZoneGraveyard) && Main.rand.Next(30) == 0)
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 316);
			}
			else if (Main.rand.Next(20) == 0)
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 44);
			}
			else if (num35 == 147 || num35 == 161 || num35 == 162)
			{
				newNPC = Main.rand.Next(15) != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 167) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 185);
			}
			else if (Main.player[k].ZoneSnow)
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 185);
			}
			else if (Main.rand.Next(3) == 0)
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, NPC.cavernMonsterType[Main.rand.Next(2), Main.rand.Next(3)]);
			}
			else if (Main.player[k].ZoneGlowshroom && (num35 == 70 || num35 == 190))
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 635);
			}
			else if (Main.halloween && Main.rand.Next(2) == 0)
			{
				newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, Main.rand.Next(322, 325));
			}
			else if (Main.expertMode && Main.rand.Next(3) == 0)
			{
				int num15 = Main.rand.Next(4);
				newNPC = num15 == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 449) : num15 == 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 450) : num15 != 0 ? NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 452) : NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 451);
			}
			else
			{
				switch (Main.rand.Next(4))
				{
					case 0:
						newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 21);
						if (Main.rand.Next(3) == 0)
						{
							if (Main.rand.Next(2) == 0)
							{
								Main.npc[newNPC].SetDefaults(-47);
							}
							else
							{
								Main.npc[newNPC].SetDefaults(-46);
							}
						}
						break;
					case 1:
						newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 201);
						if (Main.rand.Next(3) == 0)
						{
							if (Main.rand.Next(2) == 0)
							{
								Main.npc[newNPC].SetDefaults(-49);
							}
							else
							{
								Main.npc[newNPC].SetDefaults(-48);
							}
						}
						break;
					case 2:
						newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 202);
						if (Main.rand.Next(3) == 0)
						{
							if (Main.rand.Next(2) == 0)
							{
								Main.npc[newNPC].SetDefaults(-51);
							}
							else
							{
								Main.npc[newNPC].SetDefaults(-50);
							}
						}
						break;
					case 3:
						newNPC = NPC.NewNPC(NPC.GetSpawnSourceForNaturalSpawn(), num * 16 + 8, num24 * 16, 203);
						if (Main.rand.Next(3) == 0)
						{
							if (Main.rand.Next(2) == 0)
							{
								Main.npc[newNPC].SetDefaults(-53);
							}
							else
							{
								Main.npc[newNPC].SetDefaults(-52);
							}
						}
						break;
				}
			}
		}
	}
}