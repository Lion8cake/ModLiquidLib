﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System;
using Terraria.GameContent.Liquid;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.Graphics.Light;
using Terraria.GameContent;
using ModLiquidLib.Utils.Structs;

namespace ModLiquidLib.ModLoader
{

	/// <summary>
	/// This class represents a type of liquid that can be added by a mod. Only one instance of this class will ever exist for each type of liquid that is added. Any hooks that are called will be called by the instance corresponding to the liquid type. This is to prevent the game from using a massive amount of memory storing liquid instances.<br />
	/// </summary>
	// The <see href="https://github.com/tModLoader/tModLoader/wiki/Basic-Tile">Basic Liquid Guide</see> teaches the basics of making a modded tile.
	public abstract class ModLiquid : ModTexturedType, ILocalizedModType, IModType
	{
		public string LocalizationCategory => "Liquids";

		public ushort Type { get; internal set; }

		public virtual string BlockTexture => Texture + "_Block";

		public virtual string SlopeTexture => Texture + "_Slope";

		public int LiquidFallLength 
		{
			get
			{
				return LiquidRenderer.WATERFALL_LENGTH[Type];
			}
			set
			{
				LiquidRenderer.WATERFALL_LENGTH[Type] = value;
			}
		}

		public float DefaultOpacity 
		{
			get
			{
				return LiquidRenderer.DEFAULT_OPACITY[Type];
			}
			set
			{
				LiquidRenderer.DEFAULT_OPACITY[Type] = value;
			}
		}

		public byte VisualViscosity 
		{
			get
			{
				return LiquidRenderer.VISCOSITY_MASK[Type];
			}
			set
			{
				LiquidRenderer.VISCOSITY_MASK[Type] = value;
			}
		}

		/// <summary> Liquids can only have a maximum fall delay of 10 (which is the same as the fall of Honey), this is due to the delay being reset to 10 when in quickfall mode. </summary>
		public int FallDelay { get; set; } = 0;

		/// <summary> The vanilla ID of what should replace the instance when a user unloads and subsequently deletes data from your mod in their save file. Defaults to 0. </summary>
		public ushort VanillaFallbackOnModDeletion { get; set; }

		/// <summary> An array of the IDs of liquids that this tile can be considered as when looking for available liquids. </summary>
		public int[] AdjLiquids { get; set; } = new int[0];

		/// <summary> The check in Collision that checks if a coord will be drowning in water.</summary>
		public bool ChecksForDrowning { get; set; } = true;

		/// <summary> Whether or not, when drowning, the player will emit dusts from their mouth. </summary>
		public bool PlayersEmitBreathBubbles { get; set; } = true;

		/// <summary> The multiplier used to change how many tiles are needed to fish in a pool of liquid. Honey uses a 1.5x multiplier for its pool size. </summary>
		public float FishingPoolSizeMultiplier { get; set; } = 1f;

		/// <summary> The opacity that this liquid slope renders at. Lava and Honey use this to look thicker. Defaults to 0.5f. </summary>
		public float SlopeOpacity { get; set; } = 0.5f;

		/// <summary>
		/// Adds an entry to the minimap for this liquid with the given color and display name. This should be called in SetDefaults.
		/// </summary>
		public void AddMapEntry(Color color, LocalizedText name = null)
		{
			if (!MapLiquidLoader.initialized)
			{
				MapEntry entry = new MapEntry(color, name);
				if (!MapLiquidLoader.liquidEntries.Keys.Contains(Type))
				{
					MapLiquidLoader.liquidEntries[Type] = new List<MapEntry>();
				}
				MapLiquidLoader.liquidEntries[Type].Add(entry);
			}
		}

		/// <summary>
		/// <inheritdoc cref="M:ModLiquidLib.ModLoader.ModLiquid.AddMapEntry(Microsoft.Xna.Framework.Color,Terraria.Localization.LocalizedText)" />
		/// <br /><br /> <b>Overload specific:</b> This overload has an additional <paramref name="nameFunc" /> parameter. This function will be used to dynamically adjust the hover text. The parameters for the function are the default display name, x-coordinate, and y-coordinate.
		/// </summary>
		public void AddMapEntry(Color color, LocalizedText name, Func<string, int, int, string> nameFunc)
		{
			if (!MapLiquidLoader.initialized)
			{
				MapEntry entry = new MapEntry(color, name, nameFunc);
				if (!MapLiquidLoader.liquidEntries.Keys.Contains(Type))
				{
					MapLiquidLoader.liquidEntries[Type] = new List<MapEntry>();
				}
				MapLiquidLoader.liquidEntries[Type].Add(entry);
			}
		}

		public sealed override void Register()
		{
			Type = (ushort)LiquidLoader.ReserveLiquidID();
			ModTypeLookup<ModLiquid>.Register(this);
			LiquidLoader.liquids.Add(this);
		}

		public sealed override void SetupContent()
		{
			LiquidLoader.LiquidAssets[Type] = ModContent.Request<Texture2D>(Texture, (AssetRequestMode)2);
			LiquidLoader.LiquidBlockAssets[Type] = ModContent.Request<Texture2D>(BlockTexture, (AssetRequestMode)2);
			LiquidLoader.LiquidSlopeAssets[Type] = ModContent.Request<Texture2D>(SlopeTexture, (AssetRequestMode)2);
			LiquidRenderer.WATERFALL_LENGTH[Type] = 10;
			LiquidRenderer.DEFAULT_OPACITY[Type] = 0.6f;
			LiquidRenderer.VISCOSITY_MASK[Type] = 0;
			SetStaticDefaults();
		}

		/// <summary>
		/// Legacy helper method for creating a localization sub-key MapEntry
		/// </summary>
		/// <returns></returns>
		public LocalizedText CreateMapEntryName()
		{
			return this.GetLocalization("MapEntry", PrettyPrintName);
		}

		/// <summary>
		/// Allows you to modify the properties after initial loading has completed.
		/// <br /> This is where you would set the properties of this liquid. Many properties are stored as arrays throughout Terraria's code.
		/// <br /> For example:
		/// <list type="bullet">
		/// <item> WATERFALL_LENGTH = 10 </item>
		/// <item> DefaultOpacity = 0.6f; </item>
		/// <item> VisualViscosity = 0; </item>
		/// </list>
		/// </summary>
		public override void SetStaticDefaults()
		{
		}

		/// <summary>
		/// Allows you to choose which minimap entry the liquid at the given coordinates will use. 0 is the first entry added by AddMapEntry, 1 is the second entry, etc. Returns 0 by default.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		public virtual ushort GetMapOption(int i, int j)
		{
			return 0;
		}

		/// <summary>
		/// Allows you to draw things behind the liquid at the given coordinates. Return false to stop the game from drawing the liquid normally. Returns true by default.<para />
		/// This method is only called in "Color" and "White" lighting modes. Use <see cref="P:ModLiquidLib.ModLoader.ModLiquid.PreRetroDraw" /> for the other Lighting modes pre drawing
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="liquidDrawCache">The LiquidDrawCache of the rendering</param>
		/// <param name="drawOffset">The amount the liquid rendered is offset by</param>
		/// <param name="isBackgroundDraw">Whether or not the liquid is in the background</param>
		public virtual bool PreDraw(int i, int j, LiquidDrawCache liquidDrawCache, Vector2 drawOffset, bool isBackgroundDraw)
		{
			return true;
		}

		/// <summary>
		/// Allows you to draw things in front of the liquid at the given coordinates. This can also be used to do things such as rendering glowmasks.<para />
		/// This hook is not called if <see cref="P:ModLiquidLib.ModLoader.ModLiquid.PreDraw" /> returns false.<para />
		/// This method is only called in "Color" and "White" lighting modes. Use <see cref="P:ModLiquidLib.ModLoader.ModLiquid.PostRetroDraw" /> for the other Lighting modes post drawing
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="liquidDrawCache">The LiquidDrawCache of the rendering</param>
		/// <param name="drawOffset">The amount the liquid rendered is offset by</param>
		/// <param name="isBackgroundDraw">Whether or not the liquid is in the background</param>
		public virtual void PostDraw(int i, int j, LiquidDrawCache liquidDrawCache, Vector2 drawOffset, bool isBackgroundDraw)
		{
		}

		/// <summary>
		/// Allows you to make stuff happen whenever the liquid at the given coordinates is drawn. For example, creating bubble dusts or changing the color the liquid is drawn in. <para />
		/// This hook is seperate from Liquid Rendering, Only use this for things such as spawning particles.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="liquidCache">Various information about the liquid that is being drawn, such as opacity, visual liquid, etc.</param>
		public virtual void EmitEffects(int i, int j, LiquidCache liquidCache)
		{
		}

		/// <summary>
		/// Allows you to draw things behind the liquid at the given coordinates. Return false to stop the game from drawing the liquid normally. Returns true by default.<para />
		/// This method is only called in "Retro" and "Trippy" lighting modes. Use <see cref="P:ModLiquidLib.ModLoader.ModLiquid.PreDraw" /> for the other Lighting modes pre drawing
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="spriteBatch"></param>
		public virtual bool PreRetroDraw(int i, int j, SpriteBatch spriteBatch)
		{
			return true;
		}

		/// <summary>
		/// Allows you to draw things in front of the liquid at the given coordinates. This can also be used to do things such as rendering glowmasks.<para />
		/// This hook is not called if <see cref="P:ModLiquidLib.ModLoader.ModLiquid.PreRetroDraw" /> returns false.<para />
		/// This method is only called in "Retro" and "Trippy" lighting modes. Use <see cref="P:ModLiquidLib.ModLoader.ModLiquid.PostDraw" /> for the other Lighting modes post drawing
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="spriteBatch"></param>
		public virtual void PostRetroDraw(int i, int j, SpriteBatch spriteBatch)
		{
		}

		/// <summary>
		/// Allows you to make stuff happen whenever the liquid at the given coordinates is drawn. For example, creating bubble dusts or changing the color the liquid is drawn in. <para />
		/// This hook is not called if <see cref="P:ModLiquidLib.ModLoader.ModLiquid.PreRetroDraw" /> returns false.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="spriteBatch"></param>
		/// <param name="drawData">Various information about the liquid that is being drawn, such as color, opacity, waterfall length, etc.</param>
		/// <param name="liquidAmountModified">Gets the amount of liquid in a tile for retro rendering</param>
		/// <param name="liquidGFXQuality">Gets the number of quality the game has for retro rendering</param>
		public virtual void RetroDrawEffects(int i, int j, SpriteBatch spriteBatch, ref RetroLiquidDrawInfo drawData, float liquidAmountModified, int liquidGFXQuality)
		{
		}

		/// <summary>
		/// Allows you to draw things behind the liquid at the given coordinates. Return false to stop the game from drawing the liquid normally. Returns true by default. <para />
		/// Only called for liquid slopes. If you want to predraw the liquid itself, see <see cref="P:ModLiquidLib.ModLoader.ModLiquid.PreDraw" /> or <see cref="P:ModLiquidLib.ModLoader.ModLiquid.PreRetroDraw" />
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="behindBlocks">Whether or not the slope is rendered behind tiles</param>
		/// <param name="drawPosition">The rendering position of the slope. Use this rather than x/y as the rendering can be offset sometimes</param>
		/// <param name="liquidSize">The size of the liquid</param>
		/// <param name="colors">The color of the liquid. Usually the light color of the liquid</param>
		public virtual bool PreSlopeDraw(int i, int j, bool behindBlocks, ref Vector2 drawPosition, ref Rectangle liquidSize, ref VertexColors colors)
		{
			return true;
		}

		/// <summary>
		/// Allows you to draw things in front of the liquid at the given coordinates. This can also be used to do things such as rendering glowmasks.<para />
		/// This hook is not called if <see cref="P:ModLiquidLib.ModLoader.ModLiquid.PreSlopeDraw" /> returns false.<para />
		/// Only called for liquid slopes. If you want to postdraw the liquid itself, see <see cref="P:ModLiquidLib.ModLoader.ModLiquid.PostDraw" /> or <see cref="P:ModLiquidLib.ModLoader.ModLiquid.PostRetroDraw" />
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="behindBlocks">Whether or not the slope is rendered behind tiles</param>
		/// <param name="drawPosition">The rendering position of the slope. Use this rather than x/y as the rendering can be offset sometimes</param>
		/// <param name="liquidSize">The size of the liquid</param>
		/// <param name="colors">The color of the liquid. Usually the light color of the liquid</param>
		public virtual void PostSlopeDraw(int i, int j, bool behindBlocks, ref Vector2 drawPosition, ref Rectangle liquidSize, ref VertexColors colors)
		{
		}

		/// <summary>
		/// Allows you to determine how much light this liquid emits.<br />
		/// It can also let you light up the block in front of this liquid.<br />
		/// See <see cref="M:Terraria.Graphics.Light.TileLightScanner.ApplyTileLight(Terraria.Tile,System.Int32,System.Int32,Terraria.Utilities.FastRandom@,Microsoft.Xna.Framework.Vector3@)" /> for vanilla tile light values to use as a reference.<br />
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="r">The red component of light, usually a value between 0 and 1</param>
		/// <param name="g">The green component of light, usually a value between 0 and 1</param>
		/// <param name="b">The blue component of light, usually a value between 0 and 1</param>
		public virtual void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
		}

		/// <summary>
		/// The ID of the waterfall/liquidfall style the game should use when this liquid is near a half block
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		public virtual int ChooseWaterfallStyle(int i, int j)
		{
			return 0;
		}

		/// <summary>
		/// Allows the changing of liquid movement in-game as well as allowing you to do other stuff with liquids during updates such as custom evaporation. Returns true by default. 
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="liquid"></param>
		/// <returns></returns>
		public virtual bool UpdateLiquid(int i, int j, Liquid liquid)
		{
			return true;
		}

		/// <summary>
		/// Allows you to make your liquid evaporate like water when in the underworld. Returns false by default.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <returns></returns>
		public virtual bool EvaporatesInHell(int i, int j)
		{
			return false;
		}

		/// <summary>
		/// Allows you to change how your liquid moves when loading or creating a world. Returns true by default.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <returns></returns>
		public virtual bool SettleLiquidMovement(int i, int j)
		{
			return true;
		}

		/// <summary>
		/// Used to change what tile the liquid generates upon coming in contact with another liquid. Use the otherLiquid param to check what liquid this liquid is interacting with.<br/>
		/// This method is called even if a tile isn't generated. It is recommended that modders use a non-frame important tile as the result, otherwise the tile may fail to generate.<br/>
		/// It is recomended that developers use PreLiquidMerge for handling custom collisions and effects as that method is only called when a tile is requested to generate.<br/>
		/// This hook is still called, even if PreLiquidMerge is returned false. <br/>
		/// NOTE: it is encouraged to set a default tile that this liquid results in, otherwise it will default to stone. <br/>
		/// Returns 1 by default.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="otherLiquid">The liquid ID of the liquid colliding with this one.</param>
		/// <returns></returns>
		public virtual int LiquidMerge(int i, int j, int otherLiquid)
		{
			return TileID.Stone;
		}

		/// <summary>
		/// Used to modify the sound thats played when two liquids merge with each other. <br/>
		/// Called every time liquids collide and only on clients. <br/>
		/// Is not called if PreLiquidMerge returns false.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="otherLiquid">The liquid ID of the liquid colliding with this one.</param>
		/// <param name="collisionSound">The sound to be played, set this to a sound if you don't want the default merge sound to play.</param>
		public virtual void LiquidMergeSound(int i, int j, int otherLiquid, ref SoundStyle? collisionSound)
		{

		}

		/// <summary>
		/// Used to do do extra effects and special merges when two liquids collide with each other. Use the otherLiquid param to check which liquid is colliding with this liquid. <br/>
		/// NOTE: this method is called ONLY on servers when in multiplayer. Make sure you send a packet if you want to do extra visual effects. To learn more about handling custom packets, please see <see cref="Mod.HandlePacket" />. <br/>
		/// Return false to prevent the normal liquid merging logic. <br/>
		/// Retruns true by default.
		/// </summary>
		/// <param name="liquidX">The x position of the target liquid in tile coordinates<br/>
		/// NOTE: This may not always mean the x position of THIS liquid, as it may mean the x position of the other liquid.</param>
		/// <param name="liquidY">The y position of the target liquid in tile coordinates<br/>
		/// NOTE: This may not always mean the y position of THIS liquid, as it may mean the y position of the other liquid.</param>
		/// <param name="tileX">The x position of where the tile is sechedualed to generate at in tile coordninates.<br/>
		/// NOTE: most of the time this x position is the same as liquid's x position, this means that the tile is being generated over the liquid.<br/>
		/// Useful for when trying to do something else other than generating a tile.</param>
		/// <param name="tileY">The y position of where the tile is sechedualed to generate at in tile coordninates.<br/>
		/// NOTE: most of the time this y position is the same as liquid's y position, this means that the tile is being generated over the liquid.<br/>
		/// Useful for when trying to do something else other than generating a tile.</param>
		/// <param name="otherLiquid">The Liquid ID of the liquid colliding with this liquid</param>
		/// <returns></returns>
		public virtual bool PreLiquidMerge(int liquidX, int liquidY, int tileX, int tileY, int otherLiquid)
		{
			return true;
		}

		/// <summary>
		/// Allows the user to specify whether the liquid prevents the placement or blockswap of a tile over this liquid. Usually used by lava to stop players from placing ontop of it. <br/>
		/// Return true for the liquid to prevent tile placement, return false if the liquid can have tiles placed over it. <br/>
		/// Returns false by default.
		/// </summary>
		/// <param name="player">The player instance thats attempting to place a tile over the liquid.</param>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <returns></returns>
		public virtual bool BlocksTilePlacement(Player player, int i, int j)
		{
			return false;
		}

		/// <summary>
		/// Allows you to decide what happens when the player enters and exits this liquid. Vanilla liquids use this to spawn dusts and make a splashing noise when a player enters and leaves. <br/>
		/// Players now also have a moddedWet array to show which modded liquids are being entered in at a time. Please see <see cref="P:ModLiquidLib.Utils.ModLiquidPlayer.moddedWet" /> array on how to use it.
		/// </summary>
		/// <param name="player">The player instance thats entering or exiting the liquid.</param>
		/// <param name="isEnter">Whether the currently the liquid is being entered or exited.</param>
		public virtual void OnPlayerSplash(Player player, bool isEnter)
		{
		}

		/// <summary>
		/// Allows you to decide what happens when a NPC enters and exits this liquid. Vanilla liquids use this to spawn dusts and make a splashing noise when a NPC enters and leaves. <br/>
		/// NPCs now also have a moddedWet array to show which modded liquids are being entered in at a time. Please see <see cref="P:ModLiquidLib.Utils.ModLiquidPlayer.moddedWet" /> array on how to use it.
		/// </summary>
		/// <param name="npc">The NPC instance thats entering or exiting the liquid.</param>
		/// <param name="isEnter">Whether the currently the liquid is being entered or exited.</param>
		public virtual void OnNPCSplash(NPC npc, bool isEnter)
		{
		}

		/// <summary>
		/// Allows you to decide what happens when a projectile enters and exits this liquid. Vanilla liquids use this to spawn dusts and make a splashing noise when a projectile enters and leaves. <br/>
		/// Projectiles now also have a moddedWet array to show which modded liquids are being entered in at a time. Please see <see cref="P:ModLiquidLib.Utils.ModLiquidPlayer.moddedWet" /> array on how to use it.
		/// </summary>
		/// <param name="proj">The projectile instance thats entering or exiting the liquid.</param>
		/// <param name="isEnter">Whether the currently the liquid is being entered or exited.</param>
		public virtual void OnProjectileSplash(Projectile proj, bool isEnter)
		{
		}

		/// <summary>
		/// Allows you to decide what happens when a fishing bobber catches a fish. Water uses this to create extra water bubbles and to make a splash sound.		/// </summary>
		/// <param name="proj">The projectile instance thats fishing in the liquid.</param>
		/// <returns></returns>
		public virtual void OnFishingBobberSplash(Projectile proj)
		{
		}

		/// <summary>
		/// Allows you to decide what happens when an item enters and exits this liquid. Vanilla liquids use this to spawn dusts and make a splashing noise when an item enters and leaves. <br/>
		/// Items now also have a moddedWet array to show which modded liquids are being entered in at a time. Please see <see cref="P:ModLiquidLib.Utils.ModLiquidPlayer.moddedWet" /> array on how to use it.
		/// </summary>
		/// <param name="item">The item instance thats entering or exiting the liquid.</param>
		/// <param name="isEnter">Whether the currently the liquid is being entered or exited.</param>
		public virtual void OnItemSplash(Item item, bool isEnter)
		{
		}

		/// <summary>
		/// Allows the user to specify how the liquid interacts with player movement speed. <br/>
		/// Please see <see cref="P:Terraria.Player.WaterCollision" />, <see cref="P:Terraria.Player.HoneyCollision" />, or <see cref="P:Terraria.Player.ShimmerCollision" /> to see how vanilla handles it's liquid collision. <br/>
		/// Return true for the liquid to use the water/lava collision. Returns true by default.
		/// </summary>
		/// <param name="player">The player instance thats being effected by the liquid.</param>
		/// <param name="fallThrough">Whether or not the player is falling through the liquid.</param>
		/// <param name="ignorePlats">Whether or not the player ignores platforms when falling.</param>
		/// <returns></returns>
		public virtual bool PlayerCollision(Player player, bool fallThrough, bool ignorePlats)
		{
			return true;
		}

		/// <summary>
		/// Allows you to give conditions for when the game attempts to check to see if the player is drowning. <br/>
		/// Set isDrowning to either true or false depending on whether the player should or should not be drowning. <br/>
		/// This is used by items such as the Breething Reed to make the player be submerged deeper underwater. <br/>
		/// Please see <see cref="P:Terraria.Player.CheckDrowning" /> for how vanilla uses the isDrowning boolean flag. <br/>
		/// ChecksForDrowning must be set to true for this to run. Not to be confused with ChecksForDrowning.
		/// </summary>
		/// <param name="player">The player instance thats being effected by the liquid.</param>
		/// <param name="isDrowning">The boolean flag for whether or not the player is drowning.</param>
		public virtual void CanPlayerDrown(Player player, ref bool isDrowning)
		{
		}

		/// <summary>
		/// Allows you to change the Light Mask Mode for this liquid. The Light Mask Mode is the mask for lighting to determine how light interacts with this liquid. <br/>
		/// Vanilla has options for Water (slight blue fade) and Honey (dark black) while lava uses the None option. <br/>
		/// Defaults to LightMaskMode.None.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		public virtual LightMaskMode LiquidLightMaskMode(int i, int j)
		{
			return LightMaskMode.None;
		}
	}
}
