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
		/// Adds an entry to the minimap for this tile with the given color and display name. This should be called in SetDefaults.
		/// <br /> For a typical tile that has a map display name, use <see cref="M:Terraria.ModLoader.ModBlockType.CreateMapEntryName" /> as the name parameter for a default key using the pattern "Mods.{ModName}.Tiles.{ContentName}.MapEntry".
		/// <br /> If a tile will be using multiple map entries, it is suggested to use <c>this.GetLocalization("CustomMapEntryName")</c>. Modders can also re-use the display name localization of items, such as <c>ModContent.GetInstance&lt;ItemThatPlacesThisStyle&gt;().DisplayName</c>. 
		/// <br /><br /> Multiple map entries are suitable for tiles that need a different color or hover text for different tile styles. Vanilla code uses this mostly only for chest and dresser tiles. Map entries will be given a corresponding map option value, counting from 0, according to the order in which they are added. Map option values don't necessarily correspond to tile styles.
		/// <br /> <see cref="M:Terraria.ModLoader.ModBlockType.GetMapOption(System.Int32,System.Int32)" /> will be used to choose which map entry is used for a given coordinate.
		/// <br /><br /> Vanilla map entries for most furniture tiles tend to be fairly generic, opting to use a single map entry to show "Table" for all styles of tables instead of the style-specific text such as "Wooden Table", "Honey Table", etc. To use these existing localizations, use the <see cref="M:Terraria.Localization.Language.GetText(System.String)" /> method with the appropriate key, such as "MapObject.Chair", "MapObject.Door", "ItemName.WorkBench", etc. Consult the source code or ExampleMod to find the existing localization keys for common furniture types.
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
		/// <inheritdoc cref="M:Terraria.ModLoader.ModTile.AddMapEntry(Microsoft.Xna.Framework.Color,Terraria.Localization.LocalizedText)" />
		/// <br /><br /> <b>Overload specific:</b> This overload has an additional <paramref name="nameFunc" /> parameter. This function will be used to dynamically adjust the hover text. The parameters for the function are the default display name, x-coordinate, and y-coordinate. This function is most typically used for chests and dressers to show the current chest name, if assigned, instead of the default chest name. <see href="https://github.com/tModLoader/tModLoader/blob/1.4.4/ExampleMod/Content/Tiles/Furniture/ExampleChest.cs">ExampleMod's ExampleChest</see> is one example of this functionality.
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
			LiquidRenderer.VISCOSITY_MASK[Type] = VisualViscosity;
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
		/// <br /> This is where you would set the properties of this tile/wall. Many properties are stored as arrays throughout Terraria's code.
		/// <br /> For example:
		/// <list type="bullet">
		/// <item> Main.tileSolid[Type] = true; </item>
		/// <item> Main.tileSolidTop[Type] = true; </item>
		/// <item> Main.tileBrick[Type] = true; </item>
		/// <item> Main.tileBlockLight[Type] = true; </item>
		/// </list>
		/// </summary>
		public override void SetStaticDefaults()
		{
		}

		/// <summary>
		/// Allows you to choose which minimap entry the tile/wall at the given coordinates will use. 0 is the first entry added by AddMapEntry, 1 is the second entry, etc. Returns 0 by default.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		public virtual ushort GetMapOption(int i, int j)
		{
			return 0;
		}

		/// <summary>
		/// Allows you to draw things behind the tile/wall at the given coordinates. Return false to stop the game from drawing the tile normally. Returns true by default.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		public virtual bool PreDraw(int i, int j, LiquidDrawCache liquidDrawCache, Vector2 drawOffset, bool isBackgroundDraw)
		{
			return true;
		}

		/// <summary>
		/// Allows you to draw things in front of the tile/wall at the given coordinates. This can also be used to do things such as creating dust.<para />
		/// Note that this method will be called for tiles even when the tile is <see cref="P:Terraria.Tile.IsTileInvisible" /> due to Echo Coating. Use the <see cref="M:Terraria.GameContent.Drawing.TileDrawing.IsVisible(Terraria.Tile)" /> method to skip effects that shouldn't show when the tile is invisible. This method won't be called for invisible walls.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		public virtual void PostDraw(int i, int j, LiquidDrawCache liquidDrawCache, Vector2 drawOffset, bool isBackgroundDraw)
		{
		}

		/// <summary>
		/// Allows you to make stuff happen whenever the tile at the given coordinates is drawn. For example, creating dust or changing the color the tile is drawn in.
		/// SpecialDraw will only be called if coordinates are added using Main.instance.TilesRenderer.AddSpecialLegacyPoint here.
		/// Only works on Color and White lighting, see RetroDrawEffects for retro lighting of emitting particles
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="spriteBatch"></param>
		/// <param name="drawData">Various information about the tile that is being drawn, such as color, framing, glow textures, etc.</param>
		public virtual void EmitEffects(int i, int j, LiquidCache liquidCache)
		{
		}

		/// <summary>
		/// Allows you to draw things behind the tile/wall at the given coordinates. Return false to stop the game from drawing the tile normally. Returns true by default.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		public virtual bool PreRetroDraw(int i, int j, SpriteBatch spriteBatch)
		{
			return true;
		}

		/// <summary>
		/// Allows you to draw things in front of the tile/wall at the given coordinates. This can also be used to do things such as creating dust.<para />
		/// Note that this method will be called for tiles even when the tile is <see cref="P:Terraria.Tile.IsTileInvisible" /> due to Echo Coating. Use the <see cref="M:Terraria.GameContent.Drawing.TileDrawing.IsVisible(Terraria.Tile)" /> method to skip effects that shouldn't show when the tile is invisible. This method won't be called for invisible walls.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		public virtual void PostRetroDraw(int i, int j, SpriteBatch spriteBatch)
		{
		}

		/// <summary>
		/// Allows you to make stuff happen whenever the tile at the given coordinates is drawn. For example, creating dust or changing the color the tile is drawn in.
		/// SpecialDraw will only be called if coordinates are added using Main.instance.TilesRenderer.AddSpecialLegacyPoint here.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		/// <param name="spriteBatch"></param>
		/// <param name="drawData">Various information about the tile that is being drawn, such as color, framing, glow textures, etc.</param>
		public virtual void RetroDrawEffects(int i, int j, SpriteBatch spriteBatch, ref RetroLiquidDrawInfo drawData, float liquidAmountModified, int liquidGFXQuality)
		{
		}

		/// <summary>
		/// Allows you to draw things behind the tile/wall at the given coordinates. Return false to stop the game from drawing the tile normally. Returns true by default.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		public virtual bool PreSlopeDraw(int i, int j, bool behindBlocks, ref Vector2 drawPosition, ref Rectangle liquidSize, ref VertexColors colors)
		{
			return true;
		}

		/// <summary>
		/// Allows you to draw things in front of the tile/wall at the given coordinates. This can also be used to do things such as creating dust.<para />
		/// Note that this method will be called for tiles even when the tile is <see cref="P:Terraria.Tile.IsTileInvisible" /> due to Echo Coating. Use the <see cref="M:Terraria.GameContent.Drawing.TileDrawing.IsVisible(Terraria.Tile)" /> method to skip effects that shouldn't show when the tile is invisible. This method won't be called for invisible walls.
		/// </summary>
		/// <param name="i">The x position in tile coordinates.</param>
		/// <param name="j">The y position in tile coordinates.</param>
		public virtual void PostSlopeDraw(int i, int j, bool behindBlocks, ref Vector2 drawPosition, ref Rectangle liquidSize, ref VertexColors colors)
		{
		}

		/// <summary>
		/// Allows you to determine how much light this tile/wall emits.<br />
		/// If it is a tile, make sure you set Main.tileLighted[Type] to true in SetDefaults for this to work.<br />
		/// If it is a wall, it can also let you light up the block in front of this wall.<br />
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
		/// The ID of the waterfall style the game should use when this water style is in use.
		/// </summary>
		public virtual int ChooseWaterfallStyle(int i, int j)
		{
			return 0;
		}
	}
}
