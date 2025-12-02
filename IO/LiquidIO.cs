using ModLiquidLib.ModLoader;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace ModLiquidLib.IO
{
	internal class LiquidIO : ModSystem
	{
		public class LiquidIOImpl
		{
			public readonly string entriesKey;

			public readonly string dataKey;

			public ModLiquidEntry[] entries;

			public PosData<ushort>[] unloadedEntryLookup;

			public List<ushort> unloadedTypes = new List<ushort>();

			protected int LoadedLiquidCount => LiquidLoader.LiquidCount;

			protected IEnumerable<ModLiquid> LoadedLiquids => LiquidLoader.liquids;

			public LiquidIOImpl()
			{
				this.entriesKey = "liquidMap";
				this.dataKey = "liquidData";
			}

			protected ModLiquidEntry ConvertLiquidToEntry(ModLiquid liquid) => new ModLiquidEntry(liquid);

			private List<ModLiquidEntry> CreateEntries()
			{
				List<ModLiquidEntry> entries = Enumerable.Repeat<ModLiquidEntry>(null, LoadedLiquidCount).ToList();
				foreach (ModLiquid Liquid in LoadedLiquids)
				{
					if (!unloadedTypes.Contains(Liquid.Type))
					{
						entries[Liquid.Type] = ConvertLiquidToEntry(Liquid);
					}
				}
				return entries;
			}

			public void LoadEntries(TagCompound tag, out ModLiquidEntry[] savedEntryLookup)
			{
				IList<ModLiquidEntry> savedEntryList = tag.GetList<ModLiquidEntry>(entriesKey);
				List<ModLiquidEntry> Entries = CreateEntries();
				if (savedEntryList.Count == 0)
				{
					savedEntryLookup = null;
				}
				else
				{
					savedEntryLookup = new ModLiquidEntry[savedEntryList.Max((ModLiquidEntry e) => e.type) + 1];
					foreach (ModLiquidEntry entry in savedEntryList)
					{
						savedEntryLookup[entry.type] = entry;
						if (ModContent.TryFind<ModLiquid>(entry.modName, entry.name, out var Liquid))
						{
							entry.type = (entry.loadedLiquid = Liquid.Type);
							continue;
						}
						entry.type = (ushort)Entries.Count;
						entry.loadedLiquid = (ModContent.TryFind<ModLiquid>(entry.unloadedType, out var unloadedLiquid) ? unloadedLiquid : entry.DefaultUnloadedPlaceholder).Type;
						Entries.Add(entry);
					}
				}
				entries = Entries.ToArray();
			}

			protected void ReadData(Tile tile, ModLiquidEntry entry, BinaryReader reader)
			{
				tile.LiquidType = entry.loadedLiquid;
			}

			public void LoadData(TagCompound tag, ModLiquidEntry[] savedEntryLookup)
			{
				if (!tag.ContainsKey(dataKey))
				{
					return;
				}
				using BinaryReader reader = new BinaryReader(tag.GetByteArray(dataKey).ToMemoryStream());
				PosData<ushort>.OrderedSparseLookupBuilder builder = new PosData<ushort>.OrderedSparseLookupBuilder();
				for (int x = 0; x < Main.maxTilesX; x++)
				{
					for (int y = 0; y < Main.maxTilesY; y++)
					{
						ushort saveType = reader.ReadUInt16();
						if (saveType != 0)
						{
							ModLiquidEntry entry = savedEntryLookup[saveType];
							ReadData(Main.tile[x, y], entry, reader);
							if (entry.IsUnloaded)
							{
								builder.Add(x, y, entry.type);
							}
						}
					}
				}
				unloadedEntryLookup = builder.Build();
			}

			public void Save(TagCompound tag)
			{
				if (entries == null)
				{
					entries = CreateEntries().ToArray();
				}
				tag[dataKey] = SaveData(out var hasLiquid);
				tag[entriesKey] = SelectEntries(hasLiquid, entries).ToList();
			}

			private IEnumerable<ModLiquidEntry> SelectEntries(bool[] select, ModLiquidEntry[] entries)
			{
				for (int i = 0; i < select.Length; i++)
				{
					if (select[i])
					{
						yield return entries[i];
					}
				}
			}

			protected int GetModLiquidType(Tile tile)
			{
				if (tile.LiquidAmount == 0 || tile.LiquidType < LiquidID.Count)
				{
					return 0;
				}
				return tile.LiquidType;
			}

			protected void WriteData(BinaryWriter writer, Tile tile, ModLiquidEntry entry)
			{
				writer.Write(entry.type);
			}

			public byte[] SaveData(out bool[] hasObj)
			{
				using MemoryStream ms = new MemoryStream();
				BinaryWriter writer = new BinaryWriter(ms);
				PosData<ushort>.OrderedSparseLookupReader unloadedReader = new PosData<ushort>.OrderedSparseLookupReader(unloadedEntryLookup);
				hasObj = new bool[entries.Length];
				for (int x = 0; x < Main.maxTilesX; x++)
				{
					for (int y = 0; y < Main.maxTilesY; y++)
					{
						Tile tile = Main.tile[x, y];
						int liquidType = GetModLiquidType(tile);
						if (liquidType == 0)
						{
							writer.Write((ushort)0);
							continue;
						}
						if (entries[liquidType] == null)
						{
							liquidType = unloadedReader.Get(x, y);
						}
						hasObj[liquidType] = true;
						WriteData(writer, tile, entries[liquidType]);
					}
				}
				return ms.ToArray();
			}

			public void Clear()
			{
				entries = null;
				unloadedEntryLookup = null;
			}
		}

		internal static LiquidIOImpl Liquids = new LiquidIOImpl();

		public override void SaveWorldData(TagCompound tag)
		{
			Liquids.Save(tag);
		}

		public override void LoadWorldData(TagCompound tag)
		{
			Liquids.LoadEntries(tag, out var liquidEntriesLookup);
			Liquids.LoadData(tag, liquidEntriesLookup);
		}

		public override void Unload()
		{
			Liquids.unloadedTypes.Clear();
		}
	}
}
