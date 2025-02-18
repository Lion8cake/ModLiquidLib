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
using Terraria.GameContent.Liquid;
using Terraria.Graphics.Light;
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
			MapLiquidLoader.FinishSetup();

			On_SceneMetrics.Reset += SceneMetricsHooks.ResizeLiquidArray;
			IL_LiquidRenderer.DrawNormalLiquids += LiquidRendererHooks.EditLiquidRendering;
			On_TileLightScanner.ApplyLiquidLight += TileLightScannerHooks.ApplyModliquidLight;
			IL_MapHelper.CreateMapTile += MapHelperHooks.AddLiquidMapEntrys;
			IL_MapLoader.FinishSetup += MapLoaderHooks.AddLiquidToFinishSetup;
			IL_MapLoader.ModMapOption += MapLoaderHooks.AddLiquidColorstoMap;
			IL_MapHelper.Initialize += MapHelperHooks.IncrimentLiquidMapEntries;

			MapHelper.Initialize();
		}

		public override void Unload()
		{
			TModLoaderUtils.Unload();
			LiquidLoader.Unload();
			LiquidLoader.ResizeArrays(true);
			MapLiquidLoader.UnloadModMap();

			On_SceneMetrics.Reset -= SceneMetricsHooks.ResizeLiquidArray;
			IL_LiquidRenderer.DrawNormalLiquids -= LiquidRendererHooks.EditLiquidRendering;
			IL_MapHelper.Initialize -= MapHelperHooks.IncrimentLiquidMapEntries;

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
