using Microsoft.CodeAnalysis.CSharp.Syntax;
using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;
using MonoMod.Cil;
using MonoMod.Core.Platforms;
using rail;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModLiquidLib.Hooks
{
	internal class LiquidHooks
	{
		internal static void EditLiquidUpdates(ILContext il)
		{
			ILCursor c = new(il);
			ILLabel IL_00cf = c.DefineLabel();
			ILLabel IL_0331 = c.DefineLabel();
			ILLabel IL_0000 = c.DefineLabel();
			int Tile_var0 = -1; //each local var index is named with both the variable theyre supposed to get + the number of which the index was lasted used with when making the IL edit (13/4/25 tile4 has the local var index of 3)
			int Tile2_var1 = -1;
			int Tile3_var2 = -1;
			int Tile4_var3 = -1; 
			int Tile5_var4 = -1;
			int num_var6 = -1;//get the variable index of some local values
			c.GotoNext(i => i.MatchLdfld<Liquid>("x"), i => i.MatchLdcI4(1), i => i.MatchSub(),i => i.MatchLdarg0(), i => i.MatchLdfld<Liquid>("y"), i => i.MatchCall<Tilemap>("get_Item"), i => i.MatchStloc(out Tile_var0));
			c.GotoNext(i => i.MatchLdfld<Liquid>("x"), i => i.MatchLdcI4(1), i => i.MatchAdd(), i => i.MatchLdarg0(), i => i.MatchLdfld<Liquid>("y"), i => i.MatchCall<Tilemap>("get_Item"), i => i.MatchStloc(out Tile2_var1));
			c.GotoNext(i => i.MatchLdfld<Liquid>("x"), i => i.MatchLdarg0(), i => i.MatchLdfld<Liquid>("y"), i => i.MatchLdcI4(1), i => i.MatchSub(), i => i.MatchCall<Tilemap>("get_Item"), i => i.MatchStloc(out Tile3_var2));
			c.GotoNext(i => i.MatchLdfld<Liquid>("x"), i => i.MatchLdarg0(), i => i.MatchLdfld<Liquid>("y"), i => i.MatchLdcI4(1), i => i.MatchAdd(), i => i.MatchCall<Tilemap>("get_Item"), i => i.MatchStloc(out Tile4_var3)); 
			c.GotoNext(i => i.MatchLdfld<Liquid>("x"), i => i.MatchLdarg0(), i => i.MatchLdfld<Liquid>("y"), i => i.MatchCall<Tilemap>("get_Item"), i => i.MatchStloc(out Tile5_var4));

			c.GotoNext(MoveType.Before, i => i.MatchLdcR4(0.0f), i => i.MatchStloc(out num_var6));
			c.MarkLabel(IL_00cf);
			c.GotoPrev(MoveType.After, i => i.MatchLdloca(Tile5_var4), i => i.MatchCall<Tile>("get_liquid"), i => i.MatchLdindU1(), i => i.MatchStloc(out _));
			c.EmitLdarg(0);
			c.EmitDelegate((Liquid self) =>
			{
				return LiquidLoader.UpdateLiquid(self.x, self.y, Main.tile[self.x, self.y].LiquidType, self);
			});
			c.EmitBrtrue(IL_00cf);
			c.EmitRet();

			c.GotoNext(MoveType.Before, i => i.MatchBrtrue(out _), i => i.MatchLdloca(Tile5_var4), i => i.MatchCall<Tile>("get_liquid"), i => i.MatchLdindU1(), i => i.MatchLdcI4(0), i => i.MatchBle(out _));
			c.EmitLdloc(Tile5_var4);
			c.EmitDelegate((bool origLiquidID, Tile tile5) =>
			{
				bool? flag = LiquidLoader.EvaporatesInHell(tile5.X(), tile5.Y(), tile5.LiquidType);
				if (flag == null)
				{
					return origLiquidID;
				}
				return !(bool)flag;
			});

			c.GotoNext(MoveType.Before, i => i.MatchCall<Tile>("lava"), i => i.MatchBrfalse(out _)); //Jump around the liquid to liquid collision for reimplementation later
			c.EmitDelegate((ref int tile5) => { }); //the consumer (eats the tile 5)
			c.EmitBr(IL_0331);
			c.EmitLdloca(Tile5_var4);
			c.GotoNext(MoveType.Before, i => i.MatchLdloca(Tile4_var3), i => i.MatchCall<Tile>("nactive"), i => i.MatchBrfalse(out _));
			c.MarkLabel(IL_0331);

			c.GotoPrev(MoveType.Before, i => i.MatchCall<Tile>("lava"), i => i.MatchBrfalse(out _));
			c.GotoNext(MoveType.After, i => i.MatchLdloca(Tile4_var3), i => i.MatchCall<Tile>("nactive"));
			c.EmitLdarg(0);
			c.EmitLdloc(Tile5_var4);
			c.EmitLdloc(Tile4_var3);
			c.EmitLdloc(Tile3_var2);
			c.EmitLdloc(Tile2_var1);
			c.EmitLdloc(Tile_var0);
			c.EmitDelegate((bool betweenCondition, Liquid self, Tile tile5, Tile tile4, Tile tile3, Tile tile2, Tile tile) =>
			{
				for (int i = 0; i < LiquidLoader.LiquidCount; i++)
				{
					if (tile5.LiquidType == i)
					{
						if (tile5.LiquidType == LiquidID.Lava)
						{
							Liquid.LavaCheck(self.x, self.y);
						}
						else
						{
							Liquid.LiquidCheck(self.x, self.y, i);
						}
						if (!Liquid.quickFall)
						{
							int fallDelay = 0;
							if (i == LiquidID.Lava)
							{
								fallDelay = 5;
							}
							if (i == LiquidID.Honey)
							{
								fallDelay = 10;
							}
							if (i >= LiquidID.Count)
							{
								fallDelay = LiquidLoader.GetLiquid(i).FallDelay;
							}
							if (LiquidLoader.LiquidEditingFallDelay(i) != null)
							{
								fallDelay = (int)LiquidLoader.LiquidEditingFallDelay(i);
							}
							if (self.delay < fallDelay)
							{
								self.delay++;
								return false;
							}
							self.delay = 0;
						}
						break;
					}
					else
					{
						if (tile.LiquidType == i)
							Liquid.AddWater(self.x - 1, self.y);
						if (tile2.LiquidType == i)
							Liquid.AddWater(self.x + 1, self.y);
						if (tile3.LiquidType == i)
							Liquid.AddWater(self.x, self.y - 1);
						if (tile4.LiquidType == i)
							Liquid.AddWater(self.x, self.y + 1);
					}
				}
				return true;
			});
			c.EmitBrtrue(IL_0000);
			c.EmitRet();
			c.MarkLabel(IL_0000);
			c.EmitLdloc(Tile4_var3);
			c.EmitDelegate((Tile tile4) =>
			{
				return tile4.HasUnactuatedTile;
			});
		}

		internal static void EditLiquidGenMovement(ILContext il)
		{
			ILCursor c = new(il);
			ILLabel IL_0000 = c.DefineLabel();
			c.GotoNext(MoveType.After, i => i.MatchLdarg(0), i => i.MatchStloc(1));
			c.EmitLdarg(0);
			c.EmitLdarg(1);
			c.EmitDelegate((int x, int y) =>
			{
				return !LiquidLoader.SettleLiquidMovement(x, y, Main.tile[x, y].LiquidType);
			});
			c.EmitBrfalse(IL_0000);
			c.EmitRet();
			c.MarkLabel(IL_0000);
		}

		internal static void EditLiquidMergeTiles(ILContext il)
		{
			ILCursor c = new ILCursor(il);
			ILLabel IL_0000 = c.DefineLabel();
			int tile_var0 = -1;
			int tile2_var1 = -1;
			int tile3_var2 = -1;
			int tile4_var3 = -1;
			int tile5_var4 = -1;
			int liquidMergeTileType_var6 = -1;
			int liquidMergeType_var7 = -1;
			int liquidMergeTileType2_var14 = -1;
			int liquidMergeType2_var15 = -1;

			c.GotoNext(i => i.MatchLdcI4(1), i => i.MatchSub(), i => i.MatchLdarg(1), i => i.MatchCall<Tilemap>("get_Item"), i => i.MatchStloc(out tile_var0));
			c.GotoNext(i => i.MatchLdcI4(1), i => i.MatchAdd(), i => i.MatchLdarg(1), i => i.MatchCall<Tilemap>("get_Item"), i => i.MatchStloc(out tile2_var1));
			c.GotoNext(i => i.MatchLdarg(1), i => i.MatchLdcI4(1), i => i.MatchSub(), i => i.MatchCall<Tilemap>("get_Item"), i => i.MatchStloc(out tile3_var2));
			c.GotoNext(i => i.MatchLdarg(1), i => i.MatchLdcI4(1), i => i.MatchAdd(), i => i.MatchCall<Tilemap>("get_Item"), i => i.MatchStloc(out tile4_var3));
			c.GotoNext(i => i.MatchLdarg(0), i => i.MatchLdarg(1), i => i.MatchCall<Tilemap>("get_Item"), i => i.MatchStloc(out tile5_var4));
			c.GotoNext(i => i.MatchLdcI4(56), i => i.MatchStloc(out liquidMergeTileType_var6));
			c.GotoNext(i => i.MatchLdcI4(0), i => i.MatchStloc(out liquidMergeType_var7));
			c.GotoNext(i => i.MatchLdcI4(56), i => i.MatchStloc(out liquidMergeTileType2_var14));
			c.GotoNext(i => i.MatchLdcI4(0), i => i.MatchStloc(out liquidMergeType2_var15));

			c.Index = 0;

			int num_var5 = -1;

			c.GotoNext(MoveType.After, i => i.MatchBeq(out _), i => i.MatchLdcI4(0), i => i.MatchStloc(out num_var5));
			c.EmitLdcI4(56);
			c.EmitStloc(liquidMergeTileType_var6);
			c.EmitLdcI4(0);
			c.EmitStloc(liquidMergeType_var7);

			c.EmitLdarg(0);
			c.EmitLdarg(1);
			c.EmitLdarg(2);
			c.EmitLdloca(liquidMergeTileType_var6);
			c.EmitLdloca(liquidMergeType_var7);
			c.EmitLdloc(tile_var0);
			c.EmitLdloc(tile2_var1);
			c.EmitLdloc(tile3_var2);
			c.EmitDelegate((int x, int y, int thisLiquidType, ref int liquidMergeTileType, ref int liquidMergeType, Tile tile, Tile tile2, Tile tile3) =>
			{
				bool[] liquidNearby = new bool[LiquidLoader.LiquidCount]; //collect data on which liquids are nearby the main liquid

				if (tile.LiquidAmount > 0)
				{
					liquidNearby[tile.LiquidType] = true;
				}

				if (tile2.LiquidAmount > 0)
				{
					liquidNearby[tile2.LiquidType] = true;
				}

				if (tile3.LiquidAmount > 0)
				{
					liquidNearby[tile3.LiquidType] = true;
				}

				GetLiquidMergeTypes(x, y, thisLiquidType, out liquidMergeTileType, out liquidMergeType, liquidNearby); //call updated GetLiquidMergeTypes
			});

			c.GotoNext(MoveType.Before, i => i.MatchStloc(liquidMergeTileType_var6), i => i.MatchLdcI4(0), i => i.MatchStloc(liquidMergeType_var7));
			c.EmitDelegate((int fiftySix) => { });
			c.EmitBr(IL_0000);
			c.EmitLdcI4(56);
			c.GotoNext(MoveType.Before, i => i.MatchLdloc(num_var5), i => i.MatchLdcI4(24), i => i.MatchBlt(out _));
			c.MarkLabel(IL_0000);

			c.GotoNext(MoveType.Before, i => i.MatchStloc(out _), i => i.MatchVolatile());
			c.EmitLdloc(tile5_var4);
			c.EmitLdarg(2);
			c.EmitLdloc(liquidMergeType_var7);
			c.EmitDelegate((TileChangeType betweenChangetype, Tile tile5, int thisLiquidType, int liquidMergeType) =>
			{
				tile5.LiquidType = 0; //update liquid type 
				if (!WorldGen.gen)
				{
					PlayLiquidChangeSound(tile5.X(), tile5.Y(), thisLiquidType, liquidMergeType);
				}
				if (Main.netMode == NetmodeID.Server)
				{
					ModPacket packet = ModContent.GetInstance<ModLiquidLib>().GetPacket();
					packet.Write((byte)ModLiquidLib.MessageType.SyncCollisionSounds);
					packet.Write(tile5.X());
					packet.Write(tile5.Y());
					packet.Write(thisLiquidType);
					packet.Write(liquidMergeType);
					packet.Send();
				}
				return betweenChangetype;
			});

			c.GotoNext(MoveType.Before, i => i.MatchLdcI4(0), i => i.MatchStindI1(), i => i.MatchLdarg(2), i => i.MatchLdcI4(1), i => i.MatchSub());
			c.GotoPrev(MoveType.Before, i => i.MatchLdloca(tile5_var4));
			c.EmitLdarg(0);
			c.EmitLdarg(1);
			c.EmitLdarg(2);
			c.EmitLdloca(liquidMergeTileType2_var14);
			c.EmitLdloca(liquidMergeType2_var15);
			c.EmitLdloc(tile4_var3);
			c.EmitDelegate((int x, int y, int thisLiquidType, ref int liquidMergeTileType2, ref int liquidMergeType2, Tile tile4) =>
			{
				bool[] liquidNearby = new bool[LiquidLoader.LiquidCount]; //call updated GetLiquidMergeTypes
				
				if (tile4.LiquidAmount > 0)
				{
					liquidNearby[tile4.LiquidType] = true;
				}

				GetLiquidMergeTypes(x, y, thisLiquidType, out liquidMergeTileType2, out liquidMergeType2, liquidNearby);
			});

			c.GotoNext(MoveType.Before, i => i.MatchStloc(out _), i => i.MatchLdsfld<Main>("gameMenu"));
			c.EmitLdloc(tile5_var4);
			c.EmitLdarg(2);
			c.EmitLdloc(liquidMergeType2_var15);
			c.EmitDelegate((TileChangeType betweenChangetype, Tile tile5, int thisLiquidType, int liquidMergeType2) =>
			{
				tile5.LiquidType = 0; //reset the liquidtype
				if (!WorldGen.gen)
				{
					PlayLiquidChangeSound(tile5.X(), tile5.Y(), thisLiquidType, liquidMergeType2);
				}
				if (Main.netMode == NetmodeID.Server)
				{
					ModPacket packet = ModContent.GetInstance<ModLiquidLib>().GetPacket();
					packet.Write((byte)ModLiquidLib.MessageType.SyncCollisionSounds);
					packet.Write(tile5.X());
					packet.Write(tile5.Y());
					packet.Write(thisLiquidType);
					packet.Write(liquidMergeType2);
					packet.Send();
				}
				return betweenChangetype;
			});
		}

		public static void PlayLiquidChangeSound(int x, int y, int type, int otherLiquid)
		{
			SoundStyle? collisionSound = null;

			if ((type == LiquidID.Water && otherLiquid == LiquidID.Lava) || (type == LiquidID.Lava && otherLiquid == LiquidID.Water))
				collisionSound = SoundID.LiquidsWaterLava;
			else if ((type == LiquidID.Water && otherLiquid == LiquidID.Honey) || (type == LiquidID.Honey && otherLiquid == LiquidID.Water))
				collisionSound = SoundID.LiquidsHoneyWater;
			else if ((type == LiquidID.Lava && otherLiquid == LiquidID.Honey) || (type == LiquidID.Honey && otherLiquid == LiquidID.Lava))
				collisionSound = SoundID.LiquidsHoneyLava;
			else if ((type == LiquidID.Shimmer || otherLiquid == LiquidID.Shimmer) && type < LiquidID.Count && otherLiquid < LiquidID.Count)
				collisionSound = SoundID.ShimmerWeak1;

			LiquidLoader.LiquidMergeTilesType(x, y, otherLiquid, type, ref collisionSound); //called twice to make sure that other way around operations still populate the sound, this way modders aren't forced to change the sound twice
			LiquidLoader.LiquidMergeTilesType(x, y, type, otherLiquid, ref collisionSound);

			if (collisionSound == null)
			{
				collisionSound = SoundID.LiquidsHoneyWater;
			}

			SoundEngine.PlaySound(collisionSound, x * 16, y * 16);
		}

		internal static void PreventSoundOverloadFromExecuting(On_WorldGen.orig_PlayLiquidChangeSound orig, TileChangeType eventType, int x, int y, int count)
		{
			return;
			orig.Invoke(eventType, x, y, count);
		}

		internal static void PreventMergeOverloadFromExecuting(On_Liquid.orig_GetLiquidMergeTypes orig, int thisLiquidType, out int liquidMergeTileType, out int liquidMergeType, bool waterNearby, bool lavaNearby, bool honeyNearby, bool shimmerNearby)
		{
			liquidMergeType = 0;
			liquidMergeTileType = TileID.Obsidian;
			return;
			orig.Invoke(thisLiquidType, out liquidMergeTileType, out liquidMergeType, waterNearby, lavaNearby, honeyNearby, shimmerNearby);
		}

		public static void GetLiquidMergeTypes(int x, int y, int thisLiquidType, out int liquidMergeTileType, out int liquidMergeType, bool[] liquidsNearby)
		{
			liquidMergeTileType = TileID.Obsidian;
			liquidMergeType = thisLiquidType;
			SoundStyle? nullSound = null;
			if (thisLiquidType != LiquidID.Water && liquidsNearby[LiquidID.Water])
			{
				switch (thisLiquidType)
				{
					case 1:
						liquidMergeTileType = TileID.Obsidian;
						break;
					case 2:
						liquidMergeTileType = TileID.HoneyBlock;
						break;
					case 3:
						liquidMergeTileType = TileID.ShimmerBlock;
						break;
				}
				if (LiquidLoader.LiquidMergeTilesType(x, y, thisLiquidType, 0, ref nullSound) != null)
					liquidMergeTileType = (int)LiquidLoader.LiquidMergeTilesType(x, y, thisLiquidType, 0, ref nullSound);
				if (LiquidLoader.LiquidMergeTilesType(x, y, 0, thisLiquidType, ref nullSound) != null)
					liquidMergeTileType = (int)LiquidLoader.LiquidMergeTilesType(x, y, 0, thisLiquidType, ref nullSound);
				liquidMergeType = 0;
			}
			if (thisLiquidType != LiquidID.Lava && liquidsNearby[LiquidID.Lava])
			{
				switch (thisLiquidType)
				{
					case 0:
						liquidMergeTileType = TileID.Obsidian;
						break;
					case 2:
						liquidMergeTileType = TileID.CrispyHoneyBlock;
						break;
					case 3:
						liquidMergeTileType = TileID.ShimmerBlock;
						break;
				}
				if (LiquidLoader.LiquidMergeTilesType(x, y, thisLiquidType, 1, ref nullSound) != null)
					liquidMergeTileType = (int)LiquidLoader.LiquidMergeTilesType(x, y, thisLiquidType, 1, ref nullSound);
				if (LiquidLoader.LiquidMergeTilesType(x, y, 1, thisLiquidType, ref nullSound) != null)
					liquidMergeTileType = (int)LiquidLoader.LiquidMergeTilesType(x, y, 1, thisLiquidType, ref nullSound);
				liquidMergeType = 1;
			}
			if (thisLiquidType != LiquidID.Honey && liquidsNearby[LiquidID.Honey])
			{
				switch (thisLiquidType)
				{
					case 0:
						liquidMergeTileType = TileID.HoneyBlock;
						break;
					case 1:
						liquidMergeTileType = TileID.CrispyHoneyBlock;
						break;
					case 3:
						liquidMergeTileType = TileID.ShimmerBlock;
						break;
				}
				if (LiquidLoader.LiquidMergeTilesType(x, y, thisLiquidType, 2, ref nullSound) != null)
					liquidMergeTileType = (int)LiquidLoader.LiquidMergeTilesType(x, y, thisLiquidType, 2, ref nullSound);
				if (LiquidLoader.LiquidMergeTilesType(x, y, 2, thisLiquidType, ref nullSound) != null)
					liquidMergeTileType = (int)LiquidLoader.LiquidMergeTilesType(x, y, 2, thisLiquidType, ref nullSound);
				liquidMergeType = 2;
			}
			if (thisLiquidType != LiquidID.Shimmer && liquidsNearby[LiquidID.Shimmer])
			{
				switch (thisLiquidType)
				{
					case 0:
						liquidMergeTileType = TileID.ShimmerBlock;
						break;
					case 1:
						liquidMergeTileType = TileID.ShimmerBlock;
						break;
					case 2:
						liquidMergeTileType = TileID.ShimmerBlock;
						break;
				}
				if (LiquidLoader.LiquidMergeTilesType(x, y, thisLiquidType, 3, ref nullSound) != null)
					liquidMergeTileType = (int)LiquidLoader.LiquidMergeTilesType(x, y, thisLiquidType, 3, ref nullSound);
				if (LiquidLoader.LiquidMergeTilesType(x, y, 3, thisLiquidType, ref nullSound) != null)
					liquidMergeTileType = (int)LiquidLoader.LiquidMergeTilesType(x, y, 3, thisLiquidType, ref nullSound);
				liquidMergeType = 3;
			}

			for (int i = 0; i < LiquidLoader.LiquidCount; i++)
			{
				int? moddedLiquidMerge = LiquidLoader.LiquidMergeTilesType(x, y, thisLiquidType, i, ref nullSound);
				int? moddedLiquidMerge2 = LiquidLoader.LiquidMergeTilesType(x, y, i, thisLiquidType, ref nullSound);
				if (thisLiquidType != i && liquidsNearby[i])
				{
					if (moddedLiquidMerge != null)
					{
						liquidMergeTileType = (int)moddedLiquidMerge;
					}
					else if (LiquidLoader.GetLiquid(i) != null)
					{
						if (moddedLiquidMerge2 != null)
						{
							liquidMergeTileType = (int)moddedLiquidMerge2;
						}
						else
						{
							liquidMergeTileType = TileID.Stone;
						}
					}
					liquidMergeType = i;
					break;
				}
			}
		}
	}
}