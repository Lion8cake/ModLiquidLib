using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.IO;
using Terraria.ModLoader;
using ModLiquidLib.ModLoader;
using ModLiquidLib.ModLoader.Default;

namespace ModLiquidLib.IO
{
	internal class ModLiquidEntry : TagSerializable
	{
		public ushort type;

		public string modName;

		public string name;

		public ushort vanillaReplacementType;

		public string unloadedType;

		public ushort loadedLiquid;

		public bool IsUnloaded => loadedLiquid != type;

		public ModLiquid DefaultUnloadedPlaceholder => ModContent.GetInstance<UnloadedLiquid>();

		public static Func<TagCompound, ModLiquidEntry> DESERIALIZER = (TagCompound tag) => new ModLiquidEntry(tag);

		internal ModLiquidEntry(ModLiquid liquid)
		{
			type = (loadedLiquid = liquid.Type);
			modName = liquid.Mod.Name;
			name = liquid.Name;
			vanillaReplacementType = liquid.VanillaFallbackOnModDeletion;
			unloadedType = GetUnloadedPlaceholder(liquid.Type).FullName;
		}

		protected virtual ModLiquid GetUnloadedPlaceholder(ushort type)
		{
			return DefaultUnloadedPlaceholder;
		}

		protected ModLiquidEntry(TagCompound tag)
		{
			type = tag.Get<ushort>("value");
			modName = tag.Get<string>("mod");
			name = tag.Get<string>("name");
			vanillaReplacementType = tag.Get<ushort>("fallbackID");
			unloadedType = tag.Get<string>("uType");
		}

		public virtual TagCompound SerializeData()
		{
			return new TagCompound
			{
				["value"] = type,
				["mod"] = modName,
				["name"] = name,
				["fallbackID"] = vanillaReplacementType,
				["uType"] = unloadedType
			};
		}
	}
}
