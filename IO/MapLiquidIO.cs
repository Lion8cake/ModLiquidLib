using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.Social;
using Terraria.Utilities;
using Terraria;
using Terraria.ModLoader.IO;
using ModLiquidLib.ModLoader;

namespace ModLiquidLib.IO
{
	internal class MapLiquidIO
	{
		internal static void WriteModFile(string path, bool isCloudSave)
		{
			path = Path.ChangeExtension(path, ".tlmap");
			bool hasModData;
			byte[] data;
			using (MemoryStream stream = new MemoryStream())
			{
				using BinaryWriter writer = new BinaryWriter(stream);
				hasModData = WriteModMap(writer);
				writer.Flush();
				data = stream.ToArray();
			}
			if (hasModData)
			{
				FileUtilities.WriteAllBytes(path, data, isCloudSave);
			}
			else if (isCloudSave && SocialAPI.Cloud != null)
			{
				SocialAPI.Cloud.Delete(path);
			}
			else
			{
				File.Delete(path);
			}
		}

		internal static void ReadModFile(string path, bool isCloudSave)
		{
			path = Path.ChangeExtension(path, ".tlmap");
			if (FileUtilities.Exists(path, isCloudSave))
			{
				ReadModMap(new BinaryReader(FileUtilities.ReadAllBytes(path, isCloudSave).ToMemoryStream()));
			}
		}

		internal static bool WriteModMap(BinaryWriter writer)
		{
			ISet<ushort> types = new HashSet<ushort>();
			for (int i = 0; i < Main.maxTilesX; i++)
			{
				for (int j = 0; j < Main.maxTilesY; j++)
				{
					ushort type2 = Main.Map[i, j].Type;
					if (type2 >= MapHelper.modPosition)
					{
						types.Add(type2);
					}
				}
			}
			if (types.Count == 0)
			{
				return false;
			}
			writer.Write((ushort)types.Count);
			foreach (ushort type in types)
			{
				writer.Write(type);
				if (MapLoader.entryToTile.ContainsKey(type))
				{
					ModTile tile = TileLoader.GetTile(MapLoader.entryToTile[type]);
					writer.Write(value: true);
					writer.Write(tile.Mod.Name);
					writer.Write(tile.Name);
					writer.Write((ushort)(type - MapHelper.tileLookup[tile.Type]));
				}
				else if (MapLiquidLoader.entryToLiquid.ContainsKey(type))
				{
					ModLiquid liquid = LiquidLoader.GetLiquid(MapLiquidLoader.entryToLiquid[type]);
					writer.Write(value: false);
					writer.Write(liquid.Mod.Name);
					writer.Write(liquid.Name);
					writer.Write((ushort)(type - MapLiquidLoader.liquidLookup[liquid.Type]));
				}
				else if (MapLoader.entryToWall.ContainsKey(type))
				{
					ModWall wall = WallLoader.GetWall(MapLoader.entryToWall[type]);
					writer.Write(value: false);
					writer.Write(wall.Mod.Name);
					writer.Write(wall.Name);
					writer.Write((ushort)(type - MapHelper.wallLookup[wall.Type]));
				}
				else
				{
					writer.Write(value: true);
					writer.Write("");
					writer.Write("");
					writer.Write((ushort)0);
				}
			}
			WriteMapData(writer);
			return true;
		}

		internal static void ReadModMap(BinaryReader reader)
		{
			IDictionary<ushort, ushort> table = new Dictionary<ushort, ushort>();
			ushort count = reader.ReadUInt16();
			for (ushort i = 0; i < count; i++)
			{
				ushort type = reader.ReadUInt16();
				bool num = reader.ReadBoolean();
				string modName = reader.ReadString();
				string name = reader.ReadString();
				ushort option = reader.ReadUInt16();
				ushort newType = 0;
				ModWall wall;
				if (num)
				{
					if (ModContent.TryFind<ModTile>(modName, name, out var tile))
					{
						if (option >= MapLiquidLoader.modTileOptions(tile.Type))
						{
							option = 0;
						}
						newType = (ushort)MapHelper.TileToLookup(tile.Type, option);
					}
				}
				else if (ModContent.TryFind<ModLiquid>(modName, name, out var liquid))
				{
					if (option >= MapLiquidLoader.modLiquidOptions(liquid.Type))
					{
						option = 0;
					}
					newType = (ushort)(MapLiquidLoader.liquidLookup[liquid.Type] + option);
				}
				else if (ModContent.TryFind<ModWall>(modName, name, out wall))
				{
					if (option >= MapLiquidLoader.modWallOptions(wall.Type))
					{
						option = 0;
					}
					newType = (ushort)(MapHelper.wallLookup[wall.Type] + option);
				}
				table[type] = newType;
			}
			ReadMapData(reader, table);
		}

		internal static void WriteMapData(BinaryWriter writer)
		{
			byte skip = 0;
			bool nextModTile = false;
			int i = 0;
			int j = 0;
			do
			{
				MapTile tile = Main.Map[i, j];
				if (tile.Type >= MapHelper.modPosition && tile.Light > 18)
				{
					if (!nextModTile)
					{
						writer.Write(skip);
						skip = 0;
					}
					else
					{
						nextModTile = false;
					}
					WriteMapTile(ref i, ref j, writer, ref nextModTile);
				}
				else
				{
					skip++;
					if (skip == byte.MaxValue)
					{
						writer.Write(skip);
						skip = 0;
					}
				}
			}
			while (NextTile(ref i, ref j));
			if (skip > 0)
			{
				writer.Write(skip);
			}
		}

		internal static void ReadMapData(BinaryReader reader, IDictionary<ushort, ushort> table)
		{
			int i = 0;
			int j = 0;
			bool nextModTile = false;
			do
			{
				if (!nextModTile)
				{
					byte skip;
					for (skip = reader.ReadByte(); skip == byte.MaxValue; skip = reader.ReadByte())
					{
						for (byte l = 0; l < byte.MaxValue; l++)
						{
							if (!NextTile(ref i, ref j))
							{
								return;
							}
						}
					}
					for (byte k = 0; k < skip; k++)
					{
						if (!NextTile(ref i, ref j))
						{
							return;
						}
					}
				}
				else
				{
					nextModTile = false;
				}
				ReadMapTile(ref i, ref j, table, reader, ref nextModTile);
			}
			while (NextTile(ref i, ref j));
		}

		internal static void WriteMapTile(ref int i, ref int j, BinaryWriter writer, ref bool nextModTile)
		{
			MapTile tile = Main.Map[i, j];
			byte flags = 0;
			byte[] data = new byte[9];
			int index = 1;
			data[index] = (byte)tile.Type;
			index++;
			data[index] = (byte)(tile.Type >> 8);
			index++;
			if (tile.Light < byte.MaxValue)
			{
				flags = (byte)(flags | 1u);
				data[index] = tile.Light;
				index++;
			}
			if (tile.Color > 0)
			{
				flags = (byte)(flags | 2u);
				data[index] = tile.Color;
				index++;
			}
			int nextI = i;
			int nextJ = j;
			uint sameCount = 0u;
			while (NextTile(ref nextI, ref nextJ))
			{
				MapTile nextTile = Main.Map[nextI, nextJ];
				if (tile.Equals(ref nextTile) && sameCount < uint.MaxValue)
				{
					sameCount++;
					i = nextI;
					j = nextJ;
					continue;
				}
				if (nextTile.Type >= MapHelper.modPosition && nextTile.Light > 18)
				{
					flags = (byte)(flags | 0x20u);
					nextModTile = true;
				}
				break;
			}
			if (sameCount != 0)
			{
				flags = (byte)(flags | 4u);
				data[index] = (byte)sameCount;
				index++;
				if (sameCount > 255)
				{
					flags = (byte)(flags | 8u);
					data[index] = (byte)(sameCount >> 8);
					index++;
					if (sameCount > 65535)
					{
						flags = (byte)(flags | 0x10u);
						data[index] = (byte)(sameCount >> 16);
						index++;
						data[index] = (byte)(sameCount >> 24);
						index++;
					}
				}
			}
			data[0] = flags;
			writer.Write(data, 0, index);
		}

		internal static void ReadMapTile(ref int i, ref int j, IDictionary<ushort, ushort> table, BinaryReader reader, ref bool nextModTile)
		{
			byte flags = reader.ReadByte();
			ushort type = table[reader.ReadUInt16()];
			byte light = (((flags & 1) == 1) ? reader.ReadByte() : byte.MaxValue);
			byte color = (byte)(((flags & 2) == 2) ? reader.ReadByte() : 0);
			MapTile tile = MapTile.Create(type, light, color);
			Main.Map.SetTile(i, j, ref tile);
			if ((flags & 4) == 4)
			{
				uint sameCount = (((flags & 0x10) == 16) ? reader.ReadUInt32() : (((flags & 8) != 8) ? reader.ReadByte() : reader.ReadUInt16()));
				for (uint k = 0u; k < sameCount; k++)
				{
					NextTile(ref i, ref j);
					tile = MapTile.Create(type, light, color);
					Main.Map.SetTile(i, j, ref tile);
				}
			}
			if ((flags & 0x20) == 32)
			{
				nextModTile = true;
			}
		}

		private static bool NextTile(ref int i, ref int j)
		{
			j++;
			if (j >= Main.maxTilesY)
			{
				j = 0;
				i++;
				if (i >= Main.maxTilesX)
				{
					return false;
				}
			}
			return true;
		}
	}
}
