using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModLiquidLib.Utils;
using ReLogic.Content;
using System.Collections.Generic;
using System;
using Terraria.GameContent.Liquid;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Graphics;
using Terraria.DataStructures;
using Terraria;

namespace ModLiquidLib.ModLoader
{
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
		
		/// <summary> The vanilla ID of what should replace the instance when a user unloads and subsequently deletes data from your mod in their save file. Defaults to 0. </summary>
		public ushort VanillaFallbackOnModDeletion { get; set; }

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
		/// Only called for liquid slopes. If you want to postdraw the liquid itself, see <see cref="P:ModLiquidLib.ModLoader.ModLiquid.POstDraw" /> or <see cref="P:ModLiquidLib.ModLoader.ModLiquid.PostRetroDraw" />
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
	}
}
