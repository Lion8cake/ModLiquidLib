using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModLiquidLib.Hooks;
using ModLiquidLib.IO;
using ModLiquidLib.Utils;
using ModLiquidLib.Utils.LiquidContent;
using ModLiquidLib.Utils.Structs;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
using static Terraria.GameContent.Liquid.LiquidRenderer;

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

		private delegate void DelegateWaterRippleMultipler(int type, ref float multiplier);

		private delegate void DelegateStopWatchMPHMultiplier(int type, ref float multiplier);

		private delegate void DelegateSlopeOpacity(int type, ref float slopeOpacity);

		private delegate void DelegateLiquidMaskMode(int i, int j, int type, ref LightMaskMode liquidMaskMode);

		private delegate void DelegatePlayerGravityModifier(Player player, int type, ref float grav, ref float gravMax, ref int jumpMax, ref float jumpSpeed);

		private delegate void DelegateItemLiquidMovement(Item item, int type, ref Vector2 wetVelocity, ref float grav, ref float gravMax);

		private delegate void DelegateNPCGravityModifier(NPC npc, int type, ref float grav, ref float gravMax);

		private delegate bool DelegateProjectileLiquidMovement(Projectile proj, int type, ref Vector2 wetVelocity, Vector2 collisionPosition, int Width, int height, bool fallThrough);

		private delegate bool DelegateAnimateLiquid(int type, GameTime gameTime, ref int frame, ref float frameState);

		private delegate void DelegateModLiquidAnimateLiquid(GameTime gameTime, ref int frame, ref float frameState);

		private delegate void DelegateNPCRippleModifier(NPC npc, int type, ref float rippleStrength, ref float rippleOffset);

		private delegate void DelegatePlayerRippleModifier(Player player, int type, ref float rippleStrength, ref float rippleOffset);

		private static int nextLiquid = LiquidID.Count;

		internal static readonly IList<ModLiquid> liquids = new List<ModLiquid>();

		internal static readonly IList<GlobalLiquid> globalLiquids = new List<GlobalLiquid>();

		private static bool loaded = false;

		private static DelegateModifyLight[] HookModifyLight;

		private static Func<int, int, int, LiquidDrawCache, Vector2, bool, int, float, bool>[] HookPreDraw;

		private static Action<int, int, int, LiquidDrawCache, Vector2, bool, int, float>[] HookPostDraw;

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

		private static DelegatePlayerGravityModifier[] HookPlayerGravityModifier;

		private static DelegateItemLiquidMovement[] HookItemLiquidMovement;

		private static Func<NPC, int, Vector2, bool>[] HookNPCLiquidCollision;

		private static DelegateNPCGravityModifier[] HookNPCGravityModifier;

		private static DelegateProjectileLiquidMovement[] HookProjectileLiquidMovement;

		private static Func<int, bool?>[] HookChecksForDrowning;

		private static Func<int, bool?>[] HookPlayersEmitBreathBubbles;

		private static DelegateCanPlayerDrown[] HookCanPlayerDrown;

		private static DelegatePoolSizeMultiplier[] HookPoolSizeMultiplier;

		private static Func<bool>[] HookAllowFishingInShimmer;

		private static DelegateSlopeOpacity[] HookLiquidSlopeOpacity;

		private static DelegateLiquidMaskMode[] HookLiquidLightMaskMode;

		private static DelegateWaterRippleMultipler[] HookWaterRippleMultipler;

		private static Action<int, int, int, int, int>[] HookModifyTilesNearby;

		private static Func<int, int, int, int, int, bool>[] HookOnPump;

		private static DelegateStopWatchMPHMultiplier[] HookStopWatchMPHMultiplier;

		private static Action<Projectile, int>[] HookOnProjectileCollision;

		private static Action<NPC, int>[] HookOnNPCCollision;

		private static Action<Player, int>[] HookOnPlayerCollision;

		private static DelegateAnimateLiquid[] HookAnimateLiquid;

		private static DelegateNPCRippleModifier[] HookNPCRippleModifier;

		private static DelegatePlayerRippleModifier[] HookPlayerRippleModifier;

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
            string liquidMapName = "";
            if (liquidID < MapLiquidLoader.liquidLookup.Length)
            {
                liquidMapName = Lang.GetMapObjectName(MapLiquidLoader.liquidLookup[liquidID]);
            }
            if (string.IsNullOrEmpty(liquidMapName))
            {
                ModLiquid liquid = LiquidLoader.GetLiquid(liquidID);
                string key = $"Mods.{liquid.Mod.Name}.Liquids.{liquid.Name}.MapEntry"; // this is it's own separate property for readability
                liquidMapName = Language.GetOrRegister(key, liquid.PrettyPrintName).Value;
            }
            LocalizedText text;
            switch (liquidID)
            {
                case LiquidID.Water:
                    text = Language.GetText("Conditions.NearWater");
                    break;
                case LiquidID.Lava:
                    text = Language.GetText("Conditions.NearLava");
                    break;
                case LiquidID.Honey:
                    text = Language.GetText("Conditions.NearHoney");
                    break;
                case LiquidID.Shimmer:
                    text = Language.GetText("Conditions.NearShimmer");
                    break;
                default:
                    text = Language.GetText("Mods.ModLiquidLib.Conditions.NearLiquid").WithFormatArgs(liquidMapName);
                    break;
            }
            return new Condition(text, () => Main.LocalPlayer.GetAdjLiquids(liquidID));
        }

        internal static void ResizeArrays(bool unloading = false)
		{
			if (!Main.dedServ)
			{
				LiquidRenderer.Instance._cache = new LiquidRenderer.LiquidCache[1];
				LiquidRenderer.Instance._drawCache = new LiquidRenderer.LiquidDrawCache[1];
			}
			Array.Resize(ref LiquidAssets, LiquidCount);
			Array.Resize(ref LiquidBlockAssets, LiquidCount);
			Array.Resize(ref LiquidSlopeAssets, LiquidCount);
			Array.Resize(ref Unsafe.AsRef(in LiquidRenderer.WATERFALL_LENGTH), LiquidCount);
			Array.Resize(ref Unsafe.AsRef(in LiquidRenderer.DEFAULT_OPACITY), LiquidCount);
			Array.Resize(ref Unsafe.AsRef(in LiquidRenderer.WAVE_MASK_STRENGTH), LiquidCount);
			Array.Resize(ref Unsafe.AsRef(in LiquidRenderer.VISCOSITY_MASK), LiquidCount);
			Array.Resize(ref LiquidRendererHooks.liquidAnimationFrame, LiquidCount);
			Array.Resize(ref LiquidRendererHooks.liquidFrameState, LiquidCount);
			LoaderUtils.ResetStaticMembers(typeof(LiquidID));
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, DelegateModifyLight>(ref HookModifyLight, globalLiquids, (GlobalLiquid g) => g.ModifyLight);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<int, int, int, LiquidDrawCache, Vector2, bool, int, float, bool>>(ref HookPreDraw, globalLiquids, (GlobalLiquid g) => g.PreDraw);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Action<int, int, int, LiquidDrawCache, Vector2, bool, int, float>>(ref HookPostDraw, globalLiquids, (GlobalLiquid g) => g.PostDraw); 
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
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<Player, int, bool, bool, bool>>(ref HookPlayerCollision, globalLiquids, (GlobalLiquid g) => g.PlayerLiquidMovement);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, DelegatePlayerGravityModifier>(ref HookPlayerGravityModifier, globalLiquids, (GlobalLiquid g) => g.PlayerGravityModifier);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, DelegateItemLiquidMovement>(ref HookItemLiquidMovement, globalLiquids, (GlobalLiquid g) => g.ItemLiquidCollision);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<NPC, int, Vector2, bool>>(ref HookNPCLiquidCollision, globalLiquids, (GlobalLiquid g) => g.NPCLiquidMovement);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, DelegateNPCGravityModifier>(ref HookNPCGravityModifier, globalLiquids, (GlobalLiquid g) => g.NPCGravityModifier);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, DelegateProjectileLiquidMovement>(ref HookProjectileLiquidMovement, globalLiquids, (GlobalLiquid g) => g.ProjectileLiquidMovement);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<int, bool?>>(ref HookChecksForDrowning, globalLiquids, (GlobalLiquid g) => g.ChecksForDrowning);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<int, bool?>>(ref HookPlayersEmitBreathBubbles, globalLiquids, (GlobalLiquid g) => g.AllowEmitBreathBubbles);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, DelegateCanPlayerDrown>(ref HookCanPlayerDrown, globalLiquids, (GlobalLiquid g) => g.CanPlayerDrown);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, DelegatePoolSizeMultiplier>(ref HookPoolSizeMultiplier, globalLiquids, (GlobalLiquid g) => g.LiquidFishingPoolSizeMulitplier);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<bool>>(ref HookAllowFishingInShimmer, globalLiquids, (GlobalLiquid g) => g.AllowFishingInShimmer);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, DelegateSlopeOpacity>(ref HookLiquidSlopeOpacity, globalLiquids, (GlobalLiquid g) => g.LiquidSlopeOpacity);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, DelegateLiquidMaskMode>(ref HookLiquidLightMaskMode, globalLiquids, (GlobalLiquid g) => g.LiquidLightMaskMode);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, DelegateWaterRippleMultipler>(ref HookWaterRippleMultipler, globalLiquids, (GlobalLiquid g) => g.WaterRippleMultiplier);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Action<int, int, int, int, int>>(ref HookModifyTilesNearby, globalLiquids, (GlobalLiquid g) => g.ModifyNearbyTiles);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Func<int, int, int, int, int, bool>>(ref HookOnPump, globalLiquids, (GlobalLiquid g) => g.OnPump);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, DelegateStopWatchMPHMultiplier>(ref HookStopWatchMPHMultiplier, globalLiquids, (GlobalLiquid g) => g.StopWatchMPHMultiplier);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Action<Player, int>>(ref HookOnPlayerCollision, globalLiquids, (GlobalLiquid g) => g.OnPlayerCollision);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Action<NPC, int>>(ref HookOnNPCCollision, globalLiquids, (GlobalLiquid g) => g.OnNPCCollision);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, Action<Projectile, int>>(ref HookOnProjectileCollision, globalLiquids, (GlobalLiquid g) => g.OnProjectileCollision);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, DelegateAnimateLiquid>(ref HookAnimateLiquid, globalLiquids, (GlobalLiquid g) => g.AnimateLiquid);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, DelegateNPCRippleModifier>(ref HookNPCRippleModifier, globalLiquids, (GlobalLiquid g) => g.NPCRippleModifier);
			TModLoaderUtils.BuildGlobalHook<GlobalLiquid, DelegatePlayerRippleModifier>(ref HookPlayerRippleModifier, globalLiquids, (GlobalLiquid g) => g.PlayerRippleModifier);
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
			if (type >= LiquidID.Count) //sets the mask mode to the liquid ID for modifying
			{
				if (liquidMaskMode == 0)
				{
					liquidMaskMode = (LightMaskMode)(byte)type;
				}
			}
		}

		public static bool PreDraw(int i, int j, int type, LiquidDrawCache liquidDrawCache, Vector2 drawOffset, bool isBackgroundDraw, int waterStyle, float globalAlpha)
		{
			Func<int, int, int, LiquidDrawCache, Vector2, bool, int, float, bool>[] hookPreDraw = HookPreDraw;
			for (int k = 0; k < hookPreDraw.Length; k++)
			{
				if (!hookPreDraw[k](i, j, type, liquidDrawCache, drawOffset, isBackgroundDraw, waterStyle, globalAlpha))
				{
					return false;
				}
			}
			return GetLiquid(type)?.PreDraw(i, j, liquidDrawCache, drawOffset, isBackgroundDraw) ?? true;
		}

		public static void PostDraw(int i, int j, int type, LiquidDrawCache liquidDrawCache, Vector2 drawOffset, bool isBackgroundDraw, int waterStyle, float globalAlpha)
		{
			GetLiquid(type)?.PostDraw(i, j, liquidDrawCache, drawOffset, isBackgroundDraw);
			Action<int, int, int, LiquidDrawCache, Vector2, bool, int, float>[] hookPostDraw = HookPostDraw;
			for (int k = 0; k < hookPostDraw.Length; k++)
			{
				hookPostDraw[k](i, j, type, liquidDrawCache, drawOffset, isBackgroundDraw, waterStyle, globalAlpha);
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
			if (LoaderUtils.HasOverride(otherModLiquid, (Expression<Func<ModLiquid, Func<int, int, int, int, int, bool>>>)((ModLiquid t) => t.PreLiquidMerge)))
			{
				return otherModLiquid?.PreLiquidMerge(liquidX, liquidY, tileX, tileY, type) ?? true;
			}
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
					player.GetModPlayer<ModLiquidPlayer>().AdjLiquid[j] = true;
				}
			}
			Func<int, int[]>[] hookAdjLiquids = HookAdjLiquids;
			for (int k = 0; k < hookAdjLiquids.Length; k++)
			{
				int[] adjLiquids = hookAdjLiquids[k](type);
				foreach (int i in adjLiquids)
				{
					player.GetModPlayer<ModLiquidPlayer>().AdjLiquid[i] = true;
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
			return GetLiquid(type)?.BlocksTilePlacement(player, i, j) ?? type == LiquidID.Lava ? true : false;
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

		public static bool PlayerLiquidMovement(int type, Player player, bool fallThrough, bool ignorePlats)
		{
			Func<Player, int, bool, bool, bool>[] hookPlayerCollision = HookPlayerCollision;
			for (int k = 0; k < hookPlayerCollision.Length; k++)
			{
				if (!hookPlayerCollision[k](player, type, fallThrough, ignorePlats))
				{
					return false;
				}
			}
			return GetLiquid(type)?.PlayerLiquidMovement(player, fallThrough, ignorePlats) ?? true;
		}

		public static void PlayerGravityModifier(int type, Player player, ref float gravity, ref float maxFallSpeed, ref int jumpHeight, ref float jumpSpeed)
		{
			GetLiquid(type)?.PlayerGravityModifier(player, ref gravity, ref maxFallSpeed, ref jumpHeight, ref jumpSpeed);
			DelegatePlayerGravityModifier[] hookPlayerGravityModifier = HookPlayerGravityModifier;
			for (int k = 0; k < hookPlayerGravityModifier.Length; k++)
			{
				hookPlayerGravityModifier[k](player, type, ref gravity, ref maxFallSpeed, ref jumpHeight, ref jumpSpeed);
			}
		}

		public static void ItemLiquidMovement(int type, Item item, ref Vector2 wetVelocity, ref float gravity, ref float maxFallSpeed)
		{
			GetLiquid(type)?.ItemLiquidCollision(item, ref wetVelocity, ref gravity, ref maxFallSpeed);
			DelegateItemLiquidMovement[] hookItemLiquidMovement = HookItemLiquidMovement;
			for (int k = 0; k < hookItemLiquidMovement.Length; k++)
			{
				hookItemLiquidMovement[k](item, type, ref wetVelocity, ref gravity, ref maxFallSpeed);
			}
		}

		public static bool NPCLiquidMovement(int type, NPC npc, Vector2 dryVelocity)
		{
			Func<NPC, int, Vector2, bool>[] hookNPCLiquidCollision = HookNPCLiquidCollision;
			for (int k = 0; k < hookNPCLiquidCollision.Length; k++)
			{
				if (!hookNPCLiquidCollision[k](npc, type, dryVelocity))
				{
					return false;
				}
			}
			return GetLiquid(type)?.NPCLiquidMovement(npc, dryVelocity) ?? true;
		}

		public static void NPCGravityModifier(int type, NPC npc, ref float gravity, ref float maxFallSpeed)
		{
			GetLiquid(type)?.NPCGravityModifier(npc, ref gravity, ref maxFallSpeed);
			DelegateNPCGravityModifier[] hookNPCGravityModifier = HookNPCGravityModifier;
			for (int k = 0; k < hookNPCGravityModifier.Length; k++)
			{
				hookNPCGravityModifier[k](npc, type, ref gravity, ref maxFallSpeed);
			}
		}

		public static bool ProjectileLiquidMovement(int type, Projectile proj, ref Vector2 wetVelocity, Vector2 collisionPosition, int Width, int Height, bool fallThrough)
		{
			DelegateProjectileLiquidMovement[] hookProjectileLiquidMovement = HookProjectileLiquidMovement;
			for (int k = 0; k < hookProjectileLiquidMovement.Length; k++)
			{
				if (!hookProjectileLiquidMovement[k](proj, type, ref wetVelocity, collisionPosition, Width, Height, fallThrough))
				{
					return false;
				}
			}
			return GetLiquid(type)?.ProjectileLiquidMovement(proj, ref wetVelocity, collisionPosition, Width, Height, fallThrough) ?? true;
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

		public static void WaterRippleMultiplier(int type, ref float multiplier)
		{
			DelegateWaterRippleMultipler[] hookWaterRippleMultipler = HookWaterRippleMultipler;
			for (int k = 0; k < hookWaterRippleMultipler.Length; k++)
			{
				hookWaterRippleMultipler[k](type, ref multiplier);
			}
		}

		public static void ModifyNearbyTiles(int x, int y, int type, int liquidX, int liquidY)
		{
			GetLiquid(type)?.ModifyNearbyTiles(x, y, liquidX, liquidY);
			Action<int, int, int, int, int>[] hookModifyTilesNearby = HookModifyTilesNearby;
			for (int k = 0; k < hookModifyTilesNearby.Length; k++)
			{
				hookModifyTilesNearby[k](x, y, type, liquidX, liquidY);
			}
		}

		public static bool OnPump(int type, int x, int y, int x2, int y2)
		{
			Func<int, int, int, int, int, bool>[] hookOnPump = HookOnPump;
			for (int k = 0; k < hookOnPump.Length; k++)
			{
				if (!hookOnPump[k](type, x, y, x2, y2))
				{
					return false;
				}
			}
			return GetLiquid(type)?.OnPump(x, y, x2, y2) ?? true;
		}

		public static void StopWatchMPHMultiplier(int type, ref float multiplier)
		{
			ModLiquid modLiquid = GetLiquid(type);
			if (modLiquid != null)
			{
				multiplier = modLiquid.StopWatchMPHMultiplier;
			}
			DelegateStopWatchMPHMultiplier[] hookStopWatchMPHMultiplier = HookStopWatchMPHMultiplier;
			for (int k = 0; k < hookStopWatchMPHMultiplier.Length; k++)
			{
				hookStopWatchMPHMultiplier[k](type, ref multiplier);
			}
		}

		public static void OnPlayerCollision(int type, Player player)
		{
			GetLiquid(type)?.OnPlayerCollision(player);
			Action<Player, int>[] hookOnPlayerCollision = HookOnPlayerCollision;
			for (int k = 0; k < hookOnPlayerCollision.Length; k++)
			{
				hookOnPlayerCollision[k](player, type);
			}
		}

		public static void OnNPCCollision(int type, NPC npc)
		{
			GetLiquid(type)?.OnNPCCollision(npc);
			Action<NPC, int>[] hookOnNPCCollision = HookOnNPCCollision;
			for (int k = 0; k < hookOnNPCCollision.Length; k++)
			{
				hookOnNPCCollision[k](npc, type);
			}
		}

		public static void OnProjectileCollision(int type, Projectile proj)
		{
			GetLiquid(type)?.OnProjectileCollision(proj);
			Action<Projectile, int>[] hookOnProjectileCollision = HookOnProjectileCollision;
			for (int k = 0; k < hookOnProjectileCollision.Length; k++)
			{
				hookOnProjectileCollision[k](proj, type);
			}
		}

		public static bool AnimateLiquid(int type, GameTime gameTime, ref int frame, ref float frameState)
		{
			bool hasOverride = false;
			ModLiquid modLiquid = GetLiquid(type);
			if (modLiquid != null)
			{
				if (LoaderUtils.HasOverride(modLiquid, (Expression<Func<ModLiquid, DelegateModLiquidAnimateLiquid>>)((ModLiquid t) => t.AnimateLiquid)))
				{
					hasOverride = true;
				}
				modLiquid.AnimateLiquid(gameTime, ref frame, ref frameState);
			}
			DelegateAnimateLiquid[] hookAnimateLiquid = HookAnimateLiquid;
			if (hookAnimateLiquid != null)
			{
				for (int k = 0; k < hookAnimateLiquid.Length; k++)
				{
					if (!hookAnimateLiquid[k](type, gameTime, ref frame, ref frameState))
					{
						return false;
					}
				}
			}
			return !hasOverride;
		}

		public static void NPCRippleModifier(int type, NPC npc, ref float rippleStrength, ref float rippleOffset)
		{
			GetLiquid(type)?.NPCRippleModifier(npc, ref rippleStrength, ref rippleOffset);
			DelegateNPCRippleModifier[] hookNPCRippleModifier = HookNPCRippleModifier;
			for (int k = 0; k < hookNPCRippleModifier.Length; k++)
			{
				hookNPCRippleModifier[k](npc, type, ref rippleStrength, ref rippleOffset);
			}
		}

		public static void PlayerRippleModifier(int type, Player player, ref float rippleStrength, ref float rippleOffset)
		{
			GetLiquid(type)?.PlayerRippleModifier(player, ref rippleStrength, ref rippleOffset);
			DelegatePlayerRippleModifier[] hookPlayerRippleModifier = HookPlayerRippleModifier;
			for (int k = 0; k < hookPlayerRippleModifier.Length; k++)
			{
				hookPlayerRippleModifier[k](player, type, ref rippleStrength, ref rippleOffset);
			}
		}
	}
}
