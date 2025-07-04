using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModLiquidLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModLiquidLib.ModLoader
{
	public abstract class GlobalLiquid : ModType
	{
		public sealed override void Register()
		{
			ModTypeLookup<GlobalLiquid>.Register(this);
			LiquidLoader.globalLiquids.Add(this);
		}

		public sealed override void SetupContent()
		{
			SetStaticDefaults();
		}

		/// <summary>
		/// Allows you to draw things behind the liquid at the given coordinates. Return false to stop the game from drawing the liquid normally. Returns true by default.<para />
		/// This method is only called in "Color" and "White" lighting modes. Use <see cref="P:ModLiquidLib.ModLoader.GlobalLiquid.PreRetroDraw" /> for the other Lighting modes pre drawing
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type"></param>
		/// <param name="liquidDrawCache">The LiquidDrawCache of the rendering</param>
		/// <param name="drawOffset">The amount the liquid rendered is offset by</param>
		/// <param name="isBackgroundDraw">Whether or not the liquid is in the background</param>
		public virtual bool PreDraw(int i, int j, int type, LiquidDrawCache liquidDrawCache, Vector2 drawOffset, bool isBackgroundDraw)
		{
			return true;
		}

		/// <summary>
		/// Allows you to draw things in front of the liquid at the given coordinates. This can also be used to do things such as rendering glowmasks.<para />
		/// This hook is not called if <see cref="P:ModLiquidLib.ModLoader.ModLiquid.PreDraw" /> returns false.<para />
		/// This method is only called in "Color" and "White" lighting modes. Use <see cref="P:ModLiquidLib.ModLoader.GlobalLiquid.PostRetroDraw" /> for the other Lighting modes post drawing
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type"></param>
		/// <param name="liquidDrawCache">The LiquidDrawCache of the rendering</param>
		/// <param name="drawOffset">The amount the liquid rendered is offset by</param>
		/// <param name="isBackgroundDraw">Whether or not the liquid is in the background</param>
		public virtual void PostDraw(int i, int j, int type, LiquidDrawCache liquidDrawCache, Vector2 drawOffset, bool isBackgroundDraw)
		{
		}

		/// <summary>
		/// Allows you to make stuff happen whenever the liquid at the given coordinates is drawn. For example, creating bubble dusts or changing the color the liquid is drawn in. <para />
		/// This hook is seperate from Liquid Rendering, Only use this for things such as spawning particles. <para />
		/// Return false to stop Lava from spawning some of its bubbles. Returns true by default.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type"></param>
		/// <param name="liquidCache">Various information about the liquid that is being drawn, such as opacity, visual liquid, etc.</param>
		public virtual bool EmitEffects(int i, int j, int type, LiquidCache liquidCache)
		{
			return true;
		}

		/// <summary>
		/// Allows you to draw things behind the liquid at the given coordinates. Return false to stop the game from drawing the liquid normally. Returns true by default.<para />
		/// This method is only called in "Retro" and "Trippy" lighting modes. Use <see cref="P:ModLiquidLib.ModLoader.GlobalLiquid.PreDraw" /> for the other Lighting modes pre drawing
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type"></param>
		/// <param name="spriteBatch"></param>
		public virtual bool PreRetroDraw(int i, int j, int type, SpriteBatch spriteBatch)
		{
			return true;
		}

		/// <summary>
		/// Allows you to draw things in front of the liquid at the given coordinates. This can also be used to do things such as rendering glowmasks.<para />
		/// This hook is not called if <see cref="P:ModLiquidLib.ModLoader.GlobalLiquid.PreRetroDraw" /> returns false.<para />
		/// This method is only called in "Retro" and "Trippy" lighting modes. Use <see cref="P:ModLiquidLib.ModLoader.GlobalLiquid.PostDraw" /> for the other Lighting modes post drawing
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type"></param>
		/// <param name="spriteBatch"></param>
		public virtual void PostRetroDraw(int i, int j, int type, SpriteBatch spriteBatch)
		{
		}

		/// <summary>
		/// Allows you to make stuff happen whenever the liquid at the given coordinates is drawn. For example, creating bubble dusts or changing the color the liquid is drawn in. <para />
		/// This hook is not called if <see cref="P:ModLiquidLib.ModLoader.GlobalLiquid.PreRetroDraw" /> returns false.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type"></param>
		/// <param name="spriteBatch"></param>
		/// <param name="drawData">Various information about the liquid that is being drawn, such as color, opacity, waterfall length, etc.</param>
		/// <param name="liquidAmountModified">Gets the amount of liquid in a tile for retro rendering</param>
		/// <param name="liquidGFXQuality">Gets the number of quality the game has for retro rendering</param>
		public virtual void RetroDrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref RetroLiquidDrawInfo drawData, float liquidAmountModified, int liquidGFXQuality)
		{
		}

		/// <summary>
		/// Allows you to draw things behind the liquid at the given coordinates. Return false to stop the game from drawing the liquid normally. Returns true by default. <para />
		/// Only called for liquid slopes. If you want to predraw the liquid itself, see <see cref="P:ModLiquidLib.ModLoader.GlobalLiquid.PreDraw" /> or <see cref="P:ModLiquidLib.ModLoader.GlobalLiquid.PreRetroDraw" />
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type"></param>
		/// <param name="behindBlocks">Whether or not the slope is rendered behind tiles</param>
		/// <param name="drawPosition">The rendering position of the slope. Use this rather than x/y as the rendering can be offset sometimes</param>
		/// <param name="liquidSize">The size of the liquid</param>
		/// <param name="colors">The color of the liquid. Usually the light color of the liquid</param>
		public virtual bool PreSlopeDraw(int i, int j, int type, bool behindBlocks, ref Vector2 drawPosition, ref Rectangle liquidSize, ref VertexColors colors)
		{
			return true;
		}

		/// <summary>
		/// Allows you to draw things in front of the liquid at the given coordinates. This can also be used to do things such as rendering glowmasks.<para />
		/// This hook is not called if <see cref="P:ModLiquidLib.ModLoader.GlobalLiquid.PreSlopeDraw" /> returns false.<para />
		/// Only called for liquid slopes. If you want to postdraw the liquid itself, see <see cref="P:ModLiquidLib.ModLoader.GlobalLiquid.POstDraw" /> or <see cref="P:ModLiquidLib.ModLoader.GlobalLiquid.PostRetroDraw" />
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type"></param>
		/// <param name="behindBlocks">Whether or not the slope is rendered behind tiles</param>
		/// <param name="drawPosition">The rendering position of the slope. Use this rather than x/y as the rendering can be offset sometimes</param>
		/// <param name="liquidSize">The size of the liquid</param>
		/// <param name="colors">The color of the liquid. Usually the light color of the liquid</param>
		public virtual void PostSlopeDraw(int i, int j, int type, bool behindBlocks, ref Vector2 drawPosition, ref Rectangle liquidSize, ref VertexColors colors)
		{
		}

		/// <summary>
		/// Allows you to determine how much light this liquid emits.<br />
		/// It can also let you light up the block in front of this liquid.<br />
		/// See <see cref="M:Terraria.Graphics.Light.TileLightScanner.ApplyTileLight(Terraria.Tile,System.Int32,System.Int32,Terraria.Utilities.FastRandom@,Microsoft.Xna.Framework.Vector3@)" /> for vanilla tile light values to use as a reference.<br />
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type"></param>
		/// <param name="r">The red component of light, usually a value between 0 and 1</param>
		/// <param name="g">The green component of light, usually a value between 0 and 1</param>
		/// <param name="b">The blue component of light, usually a value between 0 and 1</param>
		public virtual void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
		{
		}

		/// <summary>
		/// Allows for disabling lava bubbles in "Retro" and "Trippy" lighting modes. Only calls when lava's PreRetroDraw isnt false. Returns false by default.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <returns></returns>
		public virtual bool DisableRetroLavaBubbles(int i, int j)
		{
			return false;
		}

		/// <summary>
		/// The ID of the waterfall/liquidfall style the game should use when the liquid is near a half block.
		/// Returns null by default.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type"></param>
		public virtual int? ChooseWaterfallStyle(int i, int j, int type)
		{
			return null;
		}

		/// <summary>
		/// Allows the changing of liquid movement in-game as well as allowing you to do other stuff with liquids during updates such as custom evaporation. Returns true by default. 
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type"></param>
		/// <param name="liquid"></param>
		/// <returns></returns>
		public virtual bool UpdateLiquid(int i, int j, int type, Liquid liquid)
		{
			return true;
		}

		/// <summary>
		/// Allows you to make liquids evaporate in the underworld. Also allows for disabling of water evaporation. Returns null by default.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type"></param>
		/// <returns></returns>
		public virtual bool? EvaporatesInHell(int i, int j, int type)
		{
			return null;
		}

		/// <summary>
		/// Allows you to change how liquids move when loading or creating a world. Returns true by default.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type"></param>
		/// <returns></returns>
		public virtual bool SettleLiquidMovement(int i, int j, int type)
		{
			return true;
		}

		/// <summary>
		/// Allows you to change how liquids move when loading or creating a world. Returns true by default.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type"></param>
		/// <returns></returns>
		public virtual int? LiquidMerge(int i, int j, int type, int otherLiquid, ref SoundStyle? collisionSound)
		{
			return null;
		}

		/// <summary>
		/// Allows the editing of how fast liquids fall, return null for the regular liquid falling to apply. <br/>
		/// For Modded Liquids, use ModLiquid.FallDelay property to edit how fast your liquid falls. This method can be used to edit other mod liquids falling as well.<br/>
		/// Returns null by default <br/>
		/// NOTE: liquids can only have a maximum fall delay of 10 (which is the same as the fall of Honey), this is due to the delay being reset to 10 when in quickfall mode.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public virtual int? LiquidFallDelay(int type)
		{
			return null;
		}

		/// <summary>
		/// Allows you to determine which liquids the given liquid type can be considered as when looking for avaliable liquids.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public virtual int[] AdjLiquids(int type)
		{
			return new int[0];
		}

		/// <summary>
		/// Allows the user to specify whether the liquid prevents the placement or blockswap of a tile over this liquid. Usually used by lava to stop players from placing ontop of it. <br/>
		/// Return true for the liquid to prevent tile placement, return false if the liquid can have tiles placed over it. Return null for the default logic to run.<br/>
		/// Returns null by default.
		/// </summary>
		/// <param name="player">The player instance thats attempting to place a tile over the liquid.</param>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="type"></param>
		/// <returns></returns>
		public virtual bool? BlocksTilePlacement(Player player, int i, int j, int type)
		{
			return null;
		}

		/// <summary>
		/// Allows you to decide what happens when the player enters and exits this liquid. Vanilla liquids use this to spawn dusts and make a splashing noise when a player enters and leaves. <br/>
		/// Players now also have a moddedWet array to show which modded liquids are being entered in at a time. Please see <see cref="P:ModLiquidLib.Utils.ModLiquidPlayer.moddedWet" /> array on how to use it. <br/>
		/// Return true to allow the liquids to call their normal splash code. Returns true by default.
		/// </summary>
		/// <param name="player">The player instance thats entering or exiting the liquid.</param>
		/// <param name="type"></param>
		/// <param name="isEnter">Whether the currently the liquid is being entered or exited.</param>
		public virtual bool OnPlayerSplash(Player player, int type, bool isEnter)
		{
			return true;
		}

		/// <summary>
		/// Allows the user to specify how liquids interacts with player movement speed. <br/>
		/// Please see <see cref="P:Terraria.Player.WaterCollision" />, <see cref="P:Terraria.Player.HoneyCollision" />, or <see cref="P:Terraria.Player.ShimmerCollision" /> to see how vanilla handles it's liquid collision. <br/>
		/// Return true for the liquid to use it's collision. Returns true by default.
		/// </summary>
		/// <param name="player">The player instance thats being effected by the liquid.</param>
		/// <param name="type"></param>
		/// <param name="fallThrough">Whether or not the player is falling through the liquid.</param>
		/// <param name="ignorePlats">Whether or not the player ignores platforms when falling.</param>
		/// <returns></returns>
		public virtual bool PlayerCollision(Player player, int type, bool fallThrough, bool ignorePlats)
		{
			return true;
		}

		public virtual bool? UpdatePlayerLiquidMovement(Player player, int type)
		{
			return null;
		}

		/// <summary>
		/// Hook for deciding whether a liquid should allow or disallow, when overtop of the Collision.CheckDrowning position, to be considered drowning or not. <br/>
		/// Not to be confused with CanPlayersDrown, which is a hook called every frame to check if the player is drowning or not. Useful for making items such as a Breething Reed. <br/>
		/// Return null to execute the normal Collision.CheckDrowning behaviour. Returns null by default.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public virtual bool? ChecksForDrowning(int type)
		{
			return null;
		}

		/// <summary>
		/// Hook for deciding whether a liquid should emit breath dusts when the player is slowly drowning. <br/>
		/// ChecksForDrowning must be set to true for this to run.
		/// Return null to execute the normal breath behaviour. Returns null by default.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public virtual bool? PlayersEmitBreathBubbles(int type)
		{
			return null;
		}

		/// <summary>
		/// Allows you to give conditions for when the game attempts to check to see if the player is drowning. <br/>
		/// Set isDrowning to either true or false depending on whether the player should or should not be drowning. <br/>
		/// This is used by items such as the Breething Reed to make the player be submerged deeper underwater. <br/>
		/// Please see <see cref="P:Terraria.Player.CheckDrowning" /> for how vanilla uses the isDrowning boolean flag. <br/>
		/// ChecksForDrowning must be set to true for this to run. Not to be confused with ChecksForDrowning hook.
		/// </summary>
		/// <param name="player">The player instance thats being effected by the liquid.</param>
		/// <param name="type"></param>
		/// <param name="isDrowning">The boolean flag for whether or not the player is drowning.</param>
		public virtual void CanPlayerDrown(Player player, int type, ref bool isDrowning)
		{
		}
	}
}
