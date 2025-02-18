using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;

namespace ModLiquidLib.ModLoader
{
    internal struct MapEntry
    {
        internal Color color;

        internal LocalizedText name;

        internal Func<string, int, int, string> getName;

        internal MapEntry(Color color, LocalizedText name = null)
        {
            if (name == null)
            {
                name = LocalizedText.Empty;
            }
            this.color = color;
            this.name = name;
            getName = sameName;
        }

        internal MapEntry(Color color, LocalizedText name, Func<string, int, int, string> getName)
        {
            this.color = color;
            this.name = name;
            this.getName = getName;
        }

        private static string sameName(string name, int x, int y)
        {
            return name;
        }
    }
}
