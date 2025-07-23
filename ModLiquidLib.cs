using ModLiquidLib.Hooks;
using ModLiquidLib.ID;
using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;
using System.IO;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Liquid;
using Terraria.Graphics.Light;
using Terraria.IO;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ID;
using ModLiquidLib.Utils.ManualHooks;

namespace ModLiquidLib
{
	public class ModLiquidLib : Mod
	{
		public override void Load()
		{
			TModLoaderUtils.Load();

			On_ModContent.ResizeArrays += ModContentHooks.ResizeArraysLiquid;
			On_SceneMetrics.Reset += SceneMetricsHooks.ResizeLiquidArray; //stuff to be done BEFORE resizing arrays
			On_TileLightScanner.ApplyLiquidLight += TileLightScannerHooks.ApplyModliquidLight;
			IL_MapHelper.CreateMapTile += MapHelperHooks.AddLiquidMapEntrys;
			IL_MapLoader.FinishSetup += MapLoaderHooks.AddLiquidToFinishSetup;
			IL_MapHelper.Initialize += MapHelperHooks.IncrimentLiquidMapEntries;
			IL_PlayerFileData.MapBelongsToPath += PlayerFileDataHooks.AddLiquidMapFile;
			IL_WorldMap.Load += WorldMapHooks.InitaliseTLMap;
			IL_MapHelper.InternalSaveMap += MapHelperHooks.SaveTLMap;
			IL_Main.oldDrawWater += MainHooks.EditOldLiquidRendering;
			IL_TileDrawing.DrawTile_LiquidBehindTile += TileDrawingHooks.EditSlopeLiquidRendering;
			On_TileDrawing.DrawPartialLiquid += TileDrawingHooks.BlockOldParticalLiquidRendering;
			IL_WaterfallManager.FindWaterfalls += WaterfallManagerHooks.EditWaterfallStyle;
			IL_WaterfallManager.DrawWaterfall_int_float += WaterfallManagerHooks.PreDrawWaterfallModifier;
			IL_WaterfallManager.GetAlpha += WaterfallManagerHooks.editWaterfallAlpha;
			IL_Liquid.Update += LiquidHooks.EditLiquidUpdates;
			IL_Liquid.SettleWaterAt += LiquidHooks.EditLiquidGenMovement;
			On_UIModItem.OnInitialize += UIModItemHooks.AddLiquidCount;
			IL_Liquid.LiquidCheck += LiquidHooks.EditLiquidMergeTiles;
			On_Liquid.GetLiquidMergeTypes += LiquidHooks.PreventMergeOverloadFromExecuting;
			On_WorldGen.PlayLiquidChangeSound += WorldGenHooks.PreventSoundOverloadFromExecuting;
			IL_NetMessage.CompressTileBlock_Inner += NetMessageHooks.SendLiquidTypes;
			IL_NetMessage.DecompressTileBlock_Inner += NetMessageHooks.RecieveLiquidTypes;
			IL_Player.AdjTiles += PlayerHooks.AddLiquidCraftingConditions;
			IL_Player.PlaceThing_ValidTileForReplacement += PlayerHooks.PreventLiquidBlockswap;
			IL_Player.PlaceThing_Tiles_CheckLavaBlocking += PlayerHooks.PreventPlacingTilesInLiquids;
			IL_Player.PlaceThing_Tiles_CheckRopeUsability += PlayerHooks.PreventRopePlacingInLiquid;
			IL_Player.Update += PlayerHooks.PlayerLiquidCollision;
			IL_Collision.DrownCollision += CollisionHooks.LiquidDrownCollisionCheck;
			IL_Player.CheckDrowning += PlayerHooks.CanPlayerEmitDrowningBubbles;
			IL_Item.MoveInWorld += ItemHooks.UpdateItemSplash;
			IL_NPC.UpdateCollision += NPCHooks.UnwetNPCs;
			IL_NPC.Collision_WaterCollision += NPCHooks.UpdateNPCSplash;
			IL_Projectile.Update += ProjectileHooks.UpdateProjectileSplash;
			IL_Projectile.GetFishingPondState += ProjectileHooks.FishingPondLiquidEdits;
			IL_Projectile.AI_061_FishingBobber += ProjectileHooks.StopShimmerShimmeringThePlayer;
			On_Projectile.AI_061_FishingBobber_DoASplash += ProjectileHooks.BlockOtherWaterSplashes;
			IL_WorldGen.PlaceLiquid += WorldGenHooks.FixPlaceLiquidMerging;
			On_TileLightScanner.GetTileMask += TileLightScannerHooks.EditLiquidMaskdMode;
			On_Projectile.FishingCheck_RollEnemySpawns += ProjectileHooks.ShimmerFishingFix;
			On_Projectile.FishingCheck_RollItemDrop += ProjectileHooks.ShimmerFishingItemFix;
			IL_Player.ItemCheck_UseBuckets += PlayerHooks.BucketSupport;

			MapHelper.Initialize();
		}

		public override void PostSetupContent()
		{
			IL_LiquidRenderer.DrawNormalLiquids += LiquidRendererHooks.EditLiquidRendering; //stuff to be done AFTER resizing arrays
			IL_LiquidRenderer.InternalPrepareDraw += LiquidRendererHooks.SpawnDustBubbles;

			MapLiquidLoader.FinishSetup();

			for (int i = 0; i < TileLoader.TileCount; i++)
			{
				if (TileID.Sets.CountsAsWaterSource[i])
					LiquidID_TLmod.Sets.CountsAsLiquidSource[i][LiquidID.Water] = TileID.Sets.CountsAsWaterSource[i];
				if (TileID.Sets.CountsAsLavaSource[i])
					LiquidID_TLmod.Sets.CountsAsLiquidSource[i][LiquidID.Lava] = TileID.Sets.CountsAsLavaSource[i];
				if (TileID.Sets.CountsAsHoneySource[i])
					LiquidID_TLmod.Sets.CountsAsLiquidSource[i][LiquidID.Honey] = TileID.Sets.CountsAsHoneySource[i];
				if (TileID.Sets.CountsAsShimmerSource[i])
					LiquidID_TLmod.Sets.CountsAsLiquidSource[i][LiquidID.Shimmer] = TileID.Sets.CountsAsShimmerSource[i];
			}
		}

		public override void Unload()
		{
			On_ModContent.ResizeArrays -= ModContentHooks.ResizeArraysLiquid;
			TModLoaderUtils.Unload();
			LiquidLoader.ResizeArrays(true);
			LiquidLoader.Unload();
			MapLiquidLoader.UnloadModMap();

			On_SceneMetrics.Reset -= SceneMetricsHooks.ResizeLiquidArray;
			IL_LiquidRenderer.DrawNormalLiquids -= LiquidRendererHooks.EditLiquidRendering;
			On_TileLightScanner.ApplyLiquidLight -= TileLightScannerHooks.ApplyModliquidLight;
			IL_MapHelper.CreateMapTile -= MapHelperHooks.AddLiquidMapEntrys;
			IL_MapLoader.FinishSetup -= MapLoaderHooks.AddLiquidToFinishSetup;
			IL_MapHelper.Initialize -= MapHelperHooks.IncrimentLiquidMapEntries;
			IL_PlayerFileData.MapBelongsToPath -= PlayerFileDataHooks.AddLiquidMapFile;
			IL_WorldMap.Load -= WorldMapHooks.InitaliseTLMap;
			IL_MapHelper.InternalSaveMap -= MapHelperHooks.SaveTLMap;
			IL_Main.oldDrawWater -= MainHooks.EditOldLiquidRendering;
			IL_TileDrawing.DrawTile_LiquidBehindTile -= TileDrawingHooks.EditSlopeLiquidRendering;
			On_TileDrawing.DrawPartialLiquid -= TileDrawingHooks.BlockOldParticalLiquidRendering;
			IL_LiquidRenderer.InternalPrepareDraw -= LiquidRendererHooks.SpawnDustBubbles;
			IL_WaterfallManager.FindWaterfalls -= WaterfallManagerHooks.EditWaterfallStyle;
			IL_WaterfallManager.DrawWaterfall_int_float -= WaterfallManagerHooks.PreDrawWaterfallModifier;
			IL_WaterfallManager.GetAlpha -= WaterfallManagerHooks.editWaterfallAlpha;
			IL_Liquid.Update -= LiquidHooks.EditLiquidUpdates;
			IL_Liquid.SettleWaterAt -= LiquidHooks.EditLiquidGenMovement;
			On_UIModItem.OnInitialize -= UIModItemHooks.AddLiquidCount;
			IL_Liquid.LiquidCheck -= LiquidHooks.EditLiquidMergeTiles;
			On_Liquid.GetLiquidMergeTypes -= LiquidHooks.PreventMergeOverloadFromExecuting;
			On_WorldGen.PlayLiquidChangeSound -= WorldGenHooks.PreventSoundOverloadFromExecuting;
			IL_NetMessage.CompressTileBlock_Inner -= NetMessageHooks.SendLiquidTypes;
			IL_NetMessage.DecompressTileBlock_Inner -= NetMessageHooks.RecieveLiquidTypes;
			IL_Player.AdjTiles -= PlayerHooks.AddLiquidCraftingConditions;
			IL_Player.PlaceThing_ValidTileForReplacement -= PlayerHooks.PreventLiquidBlockswap;
			IL_Player.PlaceThing_Tiles_CheckLavaBlocking -= PlayerHooks.PreventPlacingTilesInLiquids;
			IL_Player.PlaceThing_Tiles_CheckRopeUsability -= PlayerHooks.PreventRopePlacingInLiquid;
			IL_Player.Update -= PlayerHooks.PlayerLiquidCollision;
			IL_Collision.DrownCollision -= CollisionHooks.LiquidDrownCollisionCheck;
			IL_Player.CheckDrowning -= PlayerHooks.CanPlayerEmitDrowningBubbles;
			IL_Item.MoveInWorld -= ItemHooks.UpdateItemSplash;
			IL_NPC.UpdateCollision -= NPCHooks.UnwetNPCs;
			IL_NPC.Collision_WaterCollision -= NPCHooks.UpdateNPCSplash;
			IL_Projectile.Update -= ProjectileHooks.UpdateProjectileSplash;
			IL_Projectile.GetFishingPondState -= ProjectileHooks.FishingPondLiquidEdits;
			IL_Projectile.AI_061_FishingBobber -= ProjectileHooks.StopShimmerShimmeringThePlayer;
			On_Projectile.AI_061_FishingBobber_DoASplash -= ProjectileHooks.BlockOtherWaterSplashes;
			IL_WorldGen.PlaceLiquid -= WorldGenHooks.FixPlaceLiquidMerging;
			On_TileLightScanner.GetTileMask -= TileLightScannerHooks.EditLiquidMaskdMode;
			On_Projectile.FishingCheck_RollEnemySpawns -= ProjectileHooks.ShimmerFishingFix;
			On_Projectile.FishingCheck_RollItemDrop -= ProjectileHooks.ShimmerFishingItemFix;
			IL_Player.ItemCheck_UseBuckets -= PlayerHooks.BucketSupport;
		}

		internal enum MessageType : byte
		{
			SyncCollisionSounds
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			MessageType msgType = (MessageType)reader.ReadByte();

			switch (msgType)
			{
				case MessageType.SyncCollisionSounds:
					int x = reader.ReadInt32();
					int y = reader.ReadInt32();
					int thisLiquidType = reader.ReadInt32();
					int liquidMergeType = reader.ReadInt32();
					LiquidHooks.PlayLiquidChangeSound(x, y, thisLiquidType, liquidMergeType);
					break;
			}
		}
	}
}
