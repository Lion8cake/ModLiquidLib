using Microsoft.Xna.Framework;
using ModLiquidLib.Hooks;
using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Liquid;
using Terraria.GameContent.UI.States;
using Terraria.Graphics.Light;
using Terraria.ID;
using Terraria.IO;
using Terraria.Map;
using Terraria.ModLoader;

namespace ModLiquidLib
{
	public class ModLiquidLib : Mod
	{
		public override void Load()
		{
			TModLoaderUtils.Load();

			On_ModContent.ResizeArrays += ModContentHooks.ResizeArrayTest;
			On_SceneMetrics.Reset += SceneMetricsHooks.ResizeLiquidArray; //stuff to be done BEFORE resizing arrays
			On_TileLightScanner.ApplyLiquidLight += TileLightScannerHooks.ApplyModliquidLight;
			IL_MapHelper.CreateMapTile += MapHelperHooks.AddLiquidMapEntrys;
			IL_MapLoader.FinishSetup += MapLoaderHooks.AddLiquidToFinishSetup;
			IL_MapHelper.Initialize += MapHelperHooks.IncrimentLiquidMapEntries;
			IL_PlayerFileData.MapBelongsToPath += MapLiquidIOHooks.AddLiquidMapFile;
			IL_WorldMap.Load += MapLiquidIOHooks.InitaliseTLMap;
			IL_MapHelper.InternalSaveMap += MapLiquidIOHooks.SaveTLMap;
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
			On_WorldGen.PlayLiquidChangeSound += LiquidHooks.PreventSoundOverloadFromExecuting;

			MapHelper.Initialize();
		}

		public override void PostSetupContent()
		{
			IL_LiquidRenderer.DrawNormalLiquids += LiquidRendererHooks.EditLiquidRendering; //stuff to be done AFTER resizing arrays
			IL_LiquidRenderer.InternalPrepareDraw += LiquidRendererHooks.SpawnDustBubbles;

			MapLiquidLoader.FinishSetup();
		}

		public override void Unload()
		{
			On_ModContent.ResizeArrays -= ModContentHooks.ResizeArrayTest;
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
			IL_PlayerFileData.MapBelongsToPath -= MapLiquidIOHooks.AddLiquidMapFile;
			IL_WorldMap.Load -= MapLiquidIOHooks.InitaliseTLMap;
			IL_MapHelper.InternalSaveMap -= MapLiquidIOHooks.SaveTLMap;
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
			On_WorldGen.PlayLiquidChangeSound -= LiquidHooks.PreventSoundOverloadFromExecuting;
		}

		/// <inheritdoc cref="M:ModLiquidLib.ModLoader.LiquidLoader.GetLiquid(System.Int32)" />
		public static ModLiquid GetModLiquid(int type)
		{
			return LiquidLoader.GetLiquid(type);
		}

		/// <summary>
		/// Get the id (type) of a ModLiquid by class. Assumes one instance per class.
		/// </summary>
		public static int LiquidType<T>() where T : ModLiquid
		{
			return ModContent.GetInstance<T>()?.Type ?? 0;
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
