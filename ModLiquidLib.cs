using Microsoft.Xna.Framework;
using ModLiquidLib.Hooks;
using ModLiquidLib.ModLoader;
using ModLiquidLib.Utils;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Liquid;
using Terraria.GameContent.UI.States;
using Terraria.Graphics.Light;
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
			LiquidLoader.ResizeArrays();

			On_SceneMetrics.Reset += SceneMetricsHooks.ResizeLiquidArray;
			IL_LiquidRenderer.DrawNormalLiquids += LiquidRendererHooks.EditLiquidRendering;
			On_TileLightScanner.ApplyLiquidLight += TileLightScannerHooks.ApplyModliquidLight;
			IL_MapHelper.CreateMapTile += MapHelperHooks.AddLiquidMapEntrys;
			IL_MapLoader.FinishSetup += MapLoaderHooks.AddLiquidToFinishSetup;
			IL_MapHelper.Initialize += MapHelperHooks.IncrimentLiquidMapEntries;
			IL_PlayerFileData.MapBelongsToPath += MapLiquidIOHooks.AddLiquidMapFile;
			IL_WorldMap.Load += MapLiquidIOHooks.InitaliseTLMap;
			IL_MapHelper.InternalSaveMap += MapLiquidIOHooks.SaveTLMap;
			//IL_Main.oldDrawWater += MainHooks.EditOldLiquidRendering;
			IL_TileDrawing.DrawTile_LiquidBehindTile += TileDrawingHooks.EditSlopeLiquidRendering;
			On_TileDrawing.DrawPartialLiquid += TileDrawingHooks.BlockOldParticalLiquidRendering;
			On_Main.oldDrawWater += MainHooks.On_Main_oldDrawWater;

			MapHelper.Initialize();
		}

		public override void PostSetupContent()
		{
			MapLiquidLoader.FinishSetup();
		}

		public override void Unload()
		{
			TModLoaderUtils.Unload();
			LiquidLoader.Unload();
			LiquidLoader.ResizeArrays(true);
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

			MapHelper.Initialize();
		}

		/// <inheritdoc cref="M:Terraria.ModLoader.WallLoader.GetWall(System.Int32)" />
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
	}
}
