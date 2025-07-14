using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.WebRequestMethods;

namespace ModLiquidLib.Hooks
{
	internal class NetMessageHooks
	{
		internal static void RecieveLiquidTypes(ILContext il)
		{
			ILCursor c = new(il);
			int tile_var0 = -1; //tile we want to set
			c.GotoNext(i => i.MatchLdloca(out tile_var0), i => i.MatchInitobj("Terraria.Tile"), i => i.MatchLdcI4(0), i => i.MatchStloc(out _)); //get the tile local var

			c.GotoNext(MoveType.After, i => i.MatchCall<Tile>("get_liquid"), i => i.MatchLdarg(0), i => i.MatchCallvirt<BinaryReader>("ReadByte"), i => i.MatchStindI1());
			c.EmitLdloca(tile_var0);
			c.EmitLdarg(0);
			c.EmitDelegate((ref Tile tile, BinaryReader reader) =>
			{
				tile.LiquidType = reader.ReadByte(); //read the liquidtype byte and set the tile accordingly
			});
		}

		internal static void SendLiquidTypes(ILContext il)
		{
			ILCursor c = new(il);
			int num16_var4 = -1; //index for the array
			int array_var7 = -1; //array
			int tile2_var11 = -1; //tile of the tile being sent to the client

			c.GotoNext(MoveType.After, i => i.MatchLdcI4(0), i => i.MatchStloc(out _), i => i.MatchLdcI4(16));
			c.EmitLdcI4(1);
			c.EmitAdd(); //incriment the array size by 1 to make room for the LiquidType

			c.GotoNext(i => i.MatchLdsflda<Main>("tile"), i => i.MatchLdloc(out _), i => i.MatchLdloc(out _), i => i.MatchCall<Tilemap>("get_Item"), i => i.MatchStloc(out tile2_var11));
			c.GotoNext(i => i.MatchLdloc(out array_var7), i => i.MatchLdloc(out num16_var4), i => i.MatchLdloc(out _), i => i.MatchLdcI4(255)); //get the indecies of the correct local variables 

			c.GotoNext(MoveType.After, i => i.MatchCall<Tile>("get_liquid"), i => i.MatchLdindU1(), i => i.MatchStelemI1(), i => i.MatchLdloc(num16_var4), i => i.MatchLdcI4(1), i => i.MatchAdd(), i => i.MatchStloc(num16_var4)); //add LiquidType to the array
			c.EmitLdloca(array_var7);
			c.EmitLdloca(num16_var4);
			c.EmitLdloc(tile2_var11);
			c.EmitDelegate((ref byte[] array, ref int num16, Tile tile2) =>
			{
				array[num16] = (byte)tile2.LiquidType;
				num16++;
			});
		}
	}
}
