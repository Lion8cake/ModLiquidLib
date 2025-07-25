﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModLiquidLib.IO;
using ModLiquidLib.Utils;
using ModLiquidLib.Utils.LiquidContent;
using ModLiquidLib.Utils.Structs;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Liquid;
using Terraria.Graphics;
using Terraria.Graphics.Light;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace ModLiquidLib.ModLoader
{
	public static class LiquidLoader
	{
		private delegate void DelegateModifyLight(int i, int j, int type, ref float r, ref float g, ref float b);

		private delegate bool DelegatePreSlopeDraw(int i, int j, int type, bool behindBlocks, ref Vector2 drawPosition, ref Rectangle liquidSize, ref VertexColors colors);

		private delegate void DelegatePostSlopeDraw(int i, int j, int type, bool behindBlocks, ref Vector2 drawPosition, ref Rectangle liquidSize, ref VertexColors colors);

		private delegate void DelegateRetroDrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref RetroLiquidDrawInfo drawData, float liquidAmountModified, int liquidGFXQuality);

		private delegate void DelegateLiquidMergeTilesSound(int i, int j, int type, int otherLiquid, ref SoundStyle? collisionSound);

		private delegate void DelegateCanPlayerDrown(Player player, int type, ref bool isDrowning);

		private delegate void DelegatePoolSizeMultiplier(int type, ref float multiplier);

		private delegate void DelegateSlopeOpacity(int type, ref float slopeOpacity);

		private delegate void DelegateLiquidMaskMode(int i, int j, int type, ref LightMaskMode liquidMaskMode);

		private static int nextLiquid = LiquidID.Count;

		internal static readonly IList<ModLiquid> liquids = new List<ModLiquid>();

		internal static readonly IList<GlobalLiquid> globalLiquids = new List<GlobalLiquid>();

		private static bool loaded = false;

		private static DelegateModifyLight[] HookModifyLight;

		private static Func<int, int, int, LiquidDrawCache, Vector2, bool, bool>[] HookPreDraw;

		private static Action<int, int, int, LiquidDrawCache, Vector2, bool>[] HookPostDraw;

		private static Func<int, int, int, LiquidCache, bool>[] HookEmitEffects;

		private static Func<int, int, int, SpriteBatch, bool>[] HookPreRetroDraw;

		private static DelegateRetroDrawEffects[] HookRetroDrawEffects;

		private static Action<int, int, int, SpriteBatch>[] HookPostRetroDraw;

		private static DelegatePreSlopeDraw[] HookPreSlopeDraw;

		private static DelegatePostSlopeDraw[] HookPostSlopeDraw;

		private static Func<int, int, bool>[] HookDisableRetroLavaBubbles;

		private static Func<int, int, int, int?>[] HookDrawWaterfall;

		private static Func<int, int, int, Liquid, bool>[] HookUpdate;

		private static Func<int, int, int, bool?>[] HookEvaporation;

		private static Func<int, int, int, bool>[] HookSettleLiquidMovement;

		private static Func<int, int, int, int, int?>[] HookMergeTiles;

		private static DelegateLiquidMergeTilesSound[] HookMergeTilesSounds;

		private static Func<int, int, int, int, int, int, bool>[] HookPreLiquidMerge;

		private static Func<int, int?>[] HookLiquidFallDelay;

		private static Func<int, int[]>[] HookAdjLiquids;

		private static Func<Player, int, int, int, bool?>[] HookBlocksTilePlacement;

		private static Func<Player, int, bool, bool>[] HookOnPlayerSplash;

		private static Func<NPC, int, bool, bool>[] HookOnNPCSplash;

		private static Func<Projectile, int, bool, bool>[] HookOnProjectileSplash;

		private static Func<Projectile, int, bool>[] HookOnFishingBobberSplash;

		private static Func<Item, int, bool, bool>[] HookOnItemSplash;

		private static Func<Player, int, bool, bool, bool>[] HookPlayerCollision;

		private static Func<int, bool?>[] HookChecksForDrowning;

		private static Func<int, bool?>[] HookPlayersEmitBreathBubbles;

		private static DelegateCanPlayerDrown[] HookCanPlayerDrown;

		private static DelegatePoolSizeMultiplier[] HookPoolSizeMultiplier;

		private static Func<bool>[] HookAllowFishingInShimmer;

		private static DelegateSlopeOpacity[] HookLiquidSlopeOpacity;

		private static DelegateLiquidMaskMode[] HookLiquidLightMaskMode;

		public static int LiquidCount => nextLiquid;

		public static Asset<Texture2D>[] LiquidAssets = new Asset<Texture2D>[4];

		public static Asset<Texture2D>[] LiquidBlockAssets = new Asset<Texture2D>[4];

		public static Asset<Texture2D>[] LiquidSlopeAssets = new Asset<Texture2D>[4];

		internal static int ReserveLiquidID()
		{
			int result = nextLiquid;
			nextLiquid++;
			return result;
		}

		/// <summary>
		/// Gets the ModLiquid instance with the given type. If no ModLiquid with the given type exists, returns null.
		/// </summary>
		public static ModLiquid GetLiquid(int type)
		{
			if (type < LiquidID.Count || type >= LiquidCount)
			{
				return null;
			}
			return liquids[type - LiquidID.Count];
		}

		/// <inheritdoc cref="M:ModLiquidLib.ModLoader.LiquidLoader.GetLiquid(System.Int32)" />
		public static ModLiquid GetModLiquid(int type)
		{
			return GetLiquid(type);
		}

		/// <summary>
		/// Get the id (type) of a ModLiquid by class. Assumes one instance per class.
		/// </summary>
		public static int LiquidType<T>() where T : ModLiquid
		{
			return ModContent.GetInstance<T>()?.Type ?? 0;
		}

		public static Condition NearLiquid(int liquidID)
		{
			string liquidMapName = Lang.GetMapObjectName(MapLiquidLoader.liquidLookup[liquidID]);
			LocalizedText text;
			if (liquidID == LiquidID.Water)
				text = Language.GetText("Conditions.NearWater");
			else if (liquidID == LiquidID.Lava)
				text = Language.GetText("Conditions.NearLava");
			else if (liquidID == LiquidID.Honey)
				text = Language.GetText("Conditions.NearHoney");
			else if (liquidID == LiquidID.Shimmer)
				text = Language.GetText("Conditions.NearShimmer");
			else
				text = Language.GetText("Mods.ModLiquidLib.Conditions.NearLiquid").WithFormatArgs(liquidMapName == "" ? LiquidLoader.GetLiquid(liquidID).Name : liquidMapName);

			return new Condition(text, () => Main.LocalPlayer.GetAdjLiquids(liquidID));
		}

		internal static void ResizeArrays(bool unloading = false)
		{
			Array.Resize(ref LiquidAssets, LiquidCount);
			Array.Resize(ref LiquidBlockAssets, LiquidCount);
			Array.Resize(ref LiquidSlopeAssets, LiquidCount);
			Array.Resize(ref Unsafe.AsRef(in LiquidRenderer.WATERFALL_LENGTH), LiquidCount);
			Array.Resize(ref Unsafe.AsRef(in LiquidRenderer.DEFAULT_OPACITY), LiquidCount);
			Array.Resize(ref Unsafe.AsRef(in LiquidRenderer.WAVE_MASK_STRENGTH), LiquidCount);
			Array.Resize(ref Unsafe.AsRef(in LiquidRenderer.VISCOSITY_MASK), LiquidCount);
			LoaderUtils.ResetStaticMembers(typeof(LiquidID));
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, DelegateModifyLight>(ref HookModifyLight, globalLiquids, (GlobalLiquid g) => g.ModifyLight);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<int, int, int, LiquidDrawCache, Vector2, bool, bool>>(ref HookPreDraw, globalLiquids, (GlobalLiquid g) => g.PreDraw);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Action<int, int, int, LiquidDrawCache, Vector2, bool>>(ref HookPostDraw, globalLiquids, (GlobalLiquid g) => g.PostDraw); 
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<int, int, int, LiquidCache, bool>>(ref HookEmitEffects, globalLiquids, (GlobalLiquid g) => g.EmitEffects);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<int, int, int, SpriteBatch, bool>>(ref HookPreRetroDraw, globalLiquids, (GlobalLiquid g) => g.PreRetroDraw);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, DelegateRetroDrawEffects>(ref HookRetroDrawEffects, globalLiquids, (GlobalLiquid g) => g.RetroDrawEffects);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Action<int, int, int, SpriteBatch>>(ref HookPostRetroDraw, globalLiquids, (GlobalLiquid g) => g.PostRetroDraw);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, DelegatePreSlopeDraw>(ref HookPreSlopeDraw, globalLiquids, (GlobalLiquid g) => g.PreSlopeDraw);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, DelegatePostSlopeDraw>(ref HookPostSlopeDraw, globalLiquids, (GlobalLiquid g) => g.PostSlopeDraw);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<int, int, bool>>(ref HookDisableRetroLavaBubbles, globalLiquids, (GlobalLiquid g) => g.DisableRetroLavaBubbles);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<int, int, int, int?>>(ref HookDrawWaterfall, globalLiquids, (GlobalLiquid g) => g.ChooseWaterfallStyle);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<int, int, int, Liquid, bool>>(ref HookUpdate, globalLiquids, (GlobalLiquid g) => g.UpdateLiquid);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<int, int, int, bool?>>(ref HookEvaporation, globalLiquids, (GlobalLiquid g) => g.EvaporatesInHell);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<int, int, int, bool>>(ref HookSettleLiquidMovement, globalLiquids, (GlobalLiquid g) => g.SettleLiquidMovement);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<int, int, int, int, int?>>(ref HookMergeTiles, globalLiquids, (GlobalLiquid g) => g.LiquidMerge);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, DelegateLiquidMergeTilesSound>(ref HookMergeTilesSounds, globalLiquids, (GlobalLiquid g) => g.LiquidMergeSound);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<int, int, int, int, int, int, bool>>(ref HookPreLiquidMerge, globalLiquids, (GlobalLiquid g) => g.PreLiquidMerge);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<int, int?>>(ref HookLiquidFallDelay, globalLiquids, (GlobalLiquid g) => g.LiquidFallDelay);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<int, int[]>>(ref HookAdjLiquids, globalLiquids, (GlobalLiquid g) => g.AdjLiquids);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<Player, int, int, int, bool?>>(ref HookBlocksTilePlacement, globalLiquids, (GlobalLiquid g) => g.BlocksTilePlacement);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<Player, int, bool, bool>>(ref HookOnPlayerSplash, globalLiquids, (GlobalLiquid g) => g.OnPlayerSplash);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<NPC, int, bool, bool>>(ref HookOnNPCSplash, globalLiquids, (GlobalLiquid g) => g.OnNPCSplash);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<Projectile, int, bool, bool>>(ref HookOnProjectileSplash, globalLiquids, (GlobalLiquid g) => g.OnProjectileSplash);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<Projectile, int, bool>>(ref HookOnFishingBobberSplash, globalLiquids, (GlobalLiquid g) => g.OnFishingBobberSplash);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<Item, int, bool, bool>>(ref HookOnItemSplash, globalLiquids, (GlobalLiquid g) => g.OnItemSplash);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<Player, int, bool, bool, bool>>(ref HookPlayerCollision, globalLiquids, (GlobalLiquid g) => g.PlayerCollision);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<int, bool?>>(ref HookChecksForDrowning, globalLiquids, (GlobalLiquid g) => g.ChecksForDrowning);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<int, bool?>>(ref HookPlayersEmitBreathBubbles, globalLiquids, (GlobalLiquid g) => g.PlayersEmitBreathBubbles);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, DelegateCanPlayerDrown>(ref HookCanPlayerDrown, globalLiquids, (GlobalLiquid g) => g.CanPlayerDrown);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, DelegatePoolSizeMultiplier>(ref HookPoolSizeMultiplier, globalLiquids, (GlobalLiquid g) => g.LiquidFishingPoolSizeMulitplier);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<bool>>(ref HookAllowFishingInShimmer, globalLiquids, (GlobalLiquid g) => g.AllowFishingInShimmer);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, DelegateSlopeOpacity>(ref HookLiquidSlopeOpacity, globalLiquids, (GlobalLiquid g) => g.LiquidSlopeOpacity);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, DelegateLiquidMaskMode>(ref HookLiquidLightMaskMode, globalLiquids, (GlobalLiquid g) => g.LiquidLightMaskMode);
			if (!unloading)
			{
				loaded = true;
			}
		}

		internal static void Unload()
		{
			loaded = false;
			liquids.Clear();
			nextLiquid = LiquidID.Count;
			globalLiquids.Clear();
			Array.Resize(ref Unsafe.AsRef(in LiquidRenderer.WATERFALL_LENGTH), 4);
			Array.Resize(ref Unsafe.AsRef(in LiquidRenderer.DEFAULT_OPACITY), 4);
			Array.Resize(ref Unsafe.AsRef(in LiquidRenderer.WAVE_MASK_STRENGTH), 5);
			Array.Resize(ref Unsafe.AsRef(in LiquidRenderer.VISCOSITY_MASK), 5);
		}

		public static void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
		{
			GetLiquid(type)?.ModifyLight(i, j, ref r, ref g, ref b);
			DelegateModifyLight[] hookModifyLight = HookModifyLight;
			for (int k = 0; k < hookModifyLight.Length; k++)
			{
				hookModifyLight[k](i, j, type, ref r, ref g, ref b);
			}
		}

		public static bool PreDraw(int i, int j, int type, LiquidDrawCache liquidDrawCache, Vector2 drawOffset, bool isBackgroundDraw)
		{
			Func<int, int, int, LiquidDrawCache, Vector2, bool, bool>[] hookPreDraw = HookPreDraw;
			for (int k = 0; k < hookPreDraw.Length; k++)
			{
				if (!hookPreDraw[k](i, j, type, liquidDrawCache, drawOffset, isBackgroundDraw))
				{
					return false;
				}
			}
			return GetLiquid(type)?.PreDraw(i, j, liquidDrawCache, drawOffset, isBackgroundDraw) ?? true;
		}

		public static void PostDraw(int i, int j, int type, LiquidDrawCache liquidDrawCache, Vector2 drawOffset, bool isBackgroundDraw)
		{
			GetLiquid(type)?.PostDraw(i, j, liquidDrawCache, drawOffset, isBackgroundDraw);
			Action<int, int, int, LiquidDrawCache, Vector2, bool>[] hookPostDraw = HookPostDraw;
			for (int k = 0; k < hookPostDraw.Length; k++)
			{
				hookPostDraw[k](i, j, type, liquidDrawCache, drawOffset, isBackgroundDraw);
			}
		}

		public static bool PreRetroDraw(int i, int j, int type, SpriteBatch spriteBatch)
		{
			Func<int, int, int, SpriteBatch, bool>[] hookPreRetroDraw = HookPreRetroDraw;
			for (int k = 0; k < hookPreRetroDraw.Length; k++)
			{
				if (!hookPreRetroDraw[k](i, j, type, spriteBatch))
				{
					return false;
				}
			}
			return GetLiquid(type)?.PreRetroDraw(i, j, spriteBatch) ?? true;
		}

		public static void RetroDrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref RetroLiquidDrawInfo drawData, float liquidAmountModified, int liquidGFXQuality)
		{
			GetLiquid(type)?.RetroDrawEffects(i, j, spriteBatch, ref drawData, liquidAmountModified, liquidGFXQuality);
			DelegateRetroDrawEffects[] hookRetroDrawEffects = HookRetroDrawEffects;
			for (int k = 0; k < hookRetroDrawEffects.Length; k++)
			{
				hookRetroDrawEffects[k](i, j, type, spriteBatch, ref drawData, liquidAmountModified, liquidGFXQuality);
			}
		}

		public static bool EmitEffects(int i, int j, byte type, LiquidCache liquidCache)
		{
			GetLiquid(type)?.EmitEffects(i, j, liquidCache);
			Func<int, int, int, LiquidCache, bool>[] hookEmitEffects = HookEmitEffects;
			for (int k = 0; k < hookEmitEffects.Length; k++)
			{
				if (!hookEmitEffects[k](i, j, type, liquidCache))
				{
					return false;
				}
			}
			return true;
		}

		public static void PostRetroDraw(int i, int j, int type, SpriteBatch spriteBatch)
		{
			GetLiquid(type)?.PostRetroDraw(i, j, spriteBatch);
			Action<int, int, int, SpriteBatch>[] hookPostRetroDraw = HookPostRetroDraw;
			for (int k = 0; k < hookPostRetroDraw.Length; k++)
			{
				hookPostRetroDraw[k](i, j, type, spriteBatch);
			}
		}

		public static bool PreSlopeDraw(int i, int j, int type, bool behindBlocks, ref Vector2 drawPosition, ref Rectangle liquidSize, ref VertexColors colors)
		{
			DelegatePreSlopeDraw[] hookPreSlopeDraw = HookPreSlopeDraw;
			for (int k = 0; k < hookPreSlopeDraw.Length; k++)
			{
				if (!hookPreSlopeDraw[k](i, j, type, behindBlocks, ref drawPosition, ref liquidSize, ref colors))
				{
					return false;
				}
			}
			return GetLiquid(type)?.PreSlopeDraw(i, j, behindBlocks, ref drawPosition, ref liquidSize, ref colors) ?? true;
		}

		public static void PostSlopeDraw(int i, int j, int type, bool behindBlocks, ref Vector2 drawPosition, ref Rectangle liquidSize, ref VertexColors colors)
		{
			GetLiquid(type)?.PostSlopeDraw(i, j, behindBlocks, ref drawPosition, ref liquidSize, ref colors);
			DelegatePostSlopeDraw[] hookPostSlopeDraw = HookPostSlopeDraw;
			for (int k = 0; k < hookPostSlopeDraw.Length; k++)
			{
				hookPostSlopeDraw[k](i, j, type, behindBlocks, ref drawPosition, ref liquidSize, ref colors);
			}
		}

		public static bool DisableRetroLavaBubbles(int i, int j)
		{
			Func<int, int, bool>[] hookDisableRetroBubbles = HookDisableRetroLavaBubbles;
			for (int k = 0; k < hookDisableRetroBubbles.Length; k++)
			{
				if (!hookDisableRetroBubbles[k](i, j))
				{
					return false;
				}
			}
			return true;
		}

		public static int? DrawWaterfall(int i, int j, int type)
		{
			Func<int, int, int, int?>[] hookDrawWaterfall = HookDrawWaterfall;
			for (int k = 0; k < hookDrawWaterfall.Length; k++)
			{
				if (hookDrawWaterfall[k](i, j, type) != null)
					return hookDrawWaterfall[k](i, j, type);
			}
			return GetLiquid(type)?.ChooseWaterfallStyle(i, j) ?? null;
		}

		public static bool UpdateLiquid(int i, int j, int type, Liquid liquid)
		{
			Func<int, int, int, Liquid, bool>[] hookUpdate = HookUpdate;
			for (int k = 0; k < hookUpdate.Length; k++)
			{
				if (!hookUpdate[k](i, j, type, liquid))
				{
					return false;
				}
			}
			return GetLiquid(type)?.UpdateLiquid(i, j, liquid) ?? true;
		}

		public static bool? EvaporatesInHell(int i, int j, int type)
		{
			Func<int, int, int, bool?>[] hookEvaporation = HookEvaporation;
			for (int k = 0; k < hookEvaporation.Length; k++)
			{
				if (hookEvaporation[k](i, j, type) != null)
					return hookEvaporation[k](i, j, type);
			}
			return GetLiquid(type)?.EvaporatesInHell(i, j) ?? null;
		}

		public static bool SettleLiquidMovement(int i, int j, int type)
		{
			Func<int, int, int, bool>[] hookSettleLiquidMovement = HookSettleLiquidMovement;
			for (int k = 0; k < hookSettleLiquidMovement.Length; k++)
			{
				if (!hookSettleLiquidMovement[k](i, j, type))
				{
					return false;
				}
			}
			return GetLiquid(type)?.SettleLiquidMovement(i, j) ?? true;
		}

		public static int? LiquidMergeTilesType(int i, int j, int type, int otherLiquid)
		{
			Func<int, int, int, int, int?>[] hookMergeTiles = HookMergeTiles;
			for (int k = 0; k < hookMergeTiles.Length; k++)
			{
				if (hookMergeTiles[k](i, j, type, otherLiquid) != null)
					return hookMergeTiles[k](i, j, type, otherLiquid);
			}
			ModLiquid modLiquid = GetLiquid(type);
			ModLiquid otherModLiquid = GetLiquid(otherLiquid);
			if (modLiquid == null && otherModLiquid != null)
			{
				return otherModLiquid?.LiquidMerge(i, j, type) ?? null;
			}
			return modLiquid?.LiquidMerge(i, j, otherLiquid) ?? null;
		}

		public static void LiquidMergeSounds(int i, int j, int type, int otherLiquid, ref SoundStyle? collisionSound)
		{
			ModLiquid modLiquid = GetLiquid(type);
			ModLiquid otherModLiquid = GetLiquid(otherLiquid);
			if (modLiquid == null && otherModLiquid != null)
				otherModLiquid?.LiquidMergeSound(i, j, type, ref collisionSound);
			else
				modLiquid?.LiquidMergeSound(i, j, otherLiquid, ref collisionSound);

			DelegateLiquidMergeTilesSound[] hookMergeTilesSounds = HookMergeTilesSounds;
			for (int k = 0; k < hookMergeTilesSounds.Length; k++)
			{
				hookMergeTilesSounds[k](i, j, type, otherLiquid, ref collisionSound);
			}
		}

		public static bool PreLiquidMerge(int liquidX, int liquidY, int tileX, int tileY, int type, int otherLiquid)
		{
			Func<int, int, int, int, int, int, bool>[] hookPreLiquidMerge = HookPreLiquidMerge;
			for (int k = 0; k < hookPreLiquidMerge.Length; k++)
			{
				if (!hookPreLiquidMerge[k](liquidX, liquidY, tileX, tileY, type, otherLiquid))
				{
					return false;
				}
			}
			ModLiquid modLiquid = GetLiquid(type);
			ModLiquid otherModLiquid = GetLiquid(otherLiquid);
			if (modLiquid == null && otherModLiquid != null)
			{
				return otherModLiquid?.PreLiquidMerge(liquidX, liquidY, tileX, tileY, type) ?? true;
			}
			return modLiquid?.PreLiquidMerge(liquidX, liquidY, tileX, tileY, otherLiquid) ?? true;
		}

		public static int? LiquidEditingFallDelay(int type)
		{
			Func<int, int?>[] hookLiquidFallDelay = HookLiquidFallDelay;
			for (int k = 0; k < hookLiquidFallDelay.Length; k++)
			{
				return hookLiquidFallDelay[k](type);
			}
			return null;
		}

		public static void AdjLiquids(Player player, int type)
		{
			ModLiquid modLiquid = GetLiquid(type);
			if (modLiquid != null)
			{
				int[] adjLiquids = modLiquid.AdjLiquids;
				foreach (int j in adjLiquids)
				{
					player.GetModPlayer<ModLiquidPlayer>().adjLiquid[j] = true;
				}
			}
			Func<int, int[]>[] hookAdjLiquids = HookAdjLiquids;
			for (int k = 0; k < hookAdjLiquids.Length; k++)
			{
				int[] adjLiquids = hookAdjLiquids[k](type);
				foreach (int i in adjLiquids)
				{
					player.GetModPlayer<ModLiquidPlayer>().adjLiquid[i] = true;
				}
			}
		}

		public static bool BlocksTilePlacement(Player player, int i, int j, int type)
		{
			Func<Player, int, int, int, bool?>[] hookBlockTilePlacement = HookBlocksTilePlacement;
			for (int k = 0; k < hookBlockTilePlacement.Length; k++)
			{
				if (hookBlockTilePlacement[k](player, i, j, type) != null)
				{
					return (bool)hookBlockTilePlacement[k](player, i, j, type);
				}
			}
			return GetLiquid(type)?.BlocksTilePlacement(player, i, j) ?? false;
		}

		public static bool OnPlayerSplash(int type, Player player, bool isEnter)
		{
			Func<Player, int, bool, bool>[] hookOnPlayerSplash = HookOnPlayerSplash;
			for (int k = 0; k < hookOnPlayerSplash.Length; k++)
			{
				if (!hookOnPlayerSplash[k](player, type, isEnter))
				{
					return false;
				}
			}
			return true;
		}

		public static bool OnNPCSplash(int type, NPC npc, bool isEnter)
		{
			Func<NPC, int, bool, bool>[] hookOnPlayerSplash = HookOnNPCSplash;
			for (int k = 0; k < hookOnPlayerSplash.Length; k++)
			{
				if (!hookOnPlayerSplash[k](npc, type, isEnter))
				{
					return false;
				}
			}
			return true;
		}

		public static bool OnProjectileSplash(int type, Projectile projectile, bool isEnter)
		{
			Func<Projectile, int, bool, bool>[] hookOnPlayerSplash = HookOnProjectileSplash;
			for (int k = 0; k < hookOnPlayerSplash.Length; k++)
			{
				if (!hookOnPlayerSplash[k](projectile, type, isEnter))
				{
					return false;
				}
			}
			return true;
		}

		public static bool OnFishingBobberSplash(int type, Projectile projectile)
		{
			ModLiquid modLiquid = GetLiquid(type);
			if (modLiquid != null)
			{
				modLiquid.OnFishingBobberSplash(projectile);
			}
			Func<Projectile, int, bool>[] hookOnPlayerSplash = HookOnFishingBobberSplash;
			for (int k = 0; k < hookOnPlayerSplash.Length; k++)
			{
				if (!hookOnPlayerSplash[k](projectile, type))
				{
					return false;
				}
			}
			return true;
		}

		public static bool OnItemSplash(int type, Item item, bool isEnter)
		{
			Func<Item, int, bool, bool>[] hookOnPlayerSplash = HookOnItemSplash;
			for (int k = 0; k < hookOnPlayerSplash.Length; k++)
			{
				if (!hookOnPlayerSplash[k](item, type, isEnter))
				{
					return false;
				}
			}
			return true;
		}

		public static bool PlayerCollision(int type, Player player, bool fallThrough, bool ignorePlats)
		{
			Func<Player, int, bool, bool, bool>[] hookPlayerCollision = HookPlayerCollision;
			for (int k = 0; k < hookPlayerCollision.Length; k++)
			{
				if (!hookPlayerCollision[k](player, type, fallThrough, ignorePlats))
				{
					return false;
				}
			}
			return GetLiquid(type)?.PlayerCollision(player, fallThrough, ignorePlats) ?? true;
		}

		public static bool? ChecksForDrowning(int type)
		{
			Func<int, bool?>[] hookChecksForDrowning = HookChecksForDrowning;
			for (int k = 0; k < hookChecksForDrowning.Length; k++)
			{
				return hookChecksForDrowning[k](type);
			}
			return null;
		}

		public static bool? PlayersEmitBreathBubbles(int type)
		{
			Func<int, bool?>[] hookPlayersEmitBreathBubbles = HookPlayersEmitBreathBubbles;
			for (int k = 0; k < hookPlayersEmitBreathBubbles.Length; k++)
			{
				return hookPlayersEmitBreathBubbles[k](type);
			}
			return null;
		}

		public static void CanPlayerDrown(int type, Player player, ref bool isDrowning)
		{
			GetLiquid(type)?.CanPlayerDrown(player, ref isDrowning);
			DelegateCanPlayerDrown[] hookCanPlayerDrown = HookCanPlayerDrown;
			for (int k = 0; k < hookCanPlayerDrown.Length; k++)
			{
				hookCanPlayerDrown[k](player, type, ref isDrowning);
			}
		}

		public static void LiquidFishingPoolSizeMulitplier(int type, ref float multiplier)
		{
			ModLiquid modLiquid = GetLiquid(type);
			if (modLiquid != null)
			{
				multiplier = (float)modLiquid.FishingPoolSizeMultiplier;
			}
			DelegatePoolSizeMultiplier[] hookPoolSizeMultiplier = HookPoolSizeMultiplier;
			for (int k = 0; k < hookPoolSizeMultiplier.Length; k++)
			{
				hookPoolSizeMultiplier[k](type, ref multiplier);
			}
		}

		public static bool AllowFishingInShimmer()
		{
			Func<bool>[] hookAllowFishingInShimmer = HookAllowFishingInShimmer;
			for (int k = 0; k < hookAllowFishingInShimmer.Length; k++)
			{
				if (hookAllowFishingInShimmer[k]())
				{
					return true;
				}
			}
			return false;
		}

		public static void LiquidSlopeOpacity(int type, ref float slopeOpacity)
		{
			ModLiquid modLiquid = GetLiquid(type);
			if (modLiquid != null)
			{
				slopeOpacity = (float)modLiquid.SlopeOpacity;
			}
			DelegateSlopeOpacity[] hookLiquidSlopeOpacity = HookLiquidSlopeOpacity;
			for (int k = 0; k < hookLiquidSlopeOpacity.Length; k++)
			{
				hookLiquidSlopeOpacity[k](type, ref slopeOpacity);
			}
		}

		public static void LiquidLightMaskMode(int i, int j, int type, ref LightMaskMode liquidMaskMode)
		{
			ModLiquid modLiquid = GetLiquid(type);
			if (modLiquid != null)
			{
				liquidMaskMode = modLiquid.LiquidLightMaskMode(i, j);
			}
			DelegateLiquidMaskMode[] hookLiquidLightMaskMode = HookLiquidLightMaskMode;
			for (int k = 0; k < hookLiquidLightMaskMode.Length; k++)
			{
				hookLiquidLightMaskMode[k](i, j, type, ref liquidMaskMode);
			}
		}
	}
}
