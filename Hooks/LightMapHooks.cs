using Microsoft.Xna.Framework;
using ModLiquidLib.ModLoader;
using MonoMod.Cil;
using Terraria.Graphics.Light;
using Terraria;
using rail;
using Terraria.ID;

namespace ModLiquidLib.Hooks
{
	internal class LightMapHooks
	{
		internal static void ModifyLiquidMaskMode(ILContext il)
		{
			ILCursor c = new(il);
			
			ILLabel IL_02bb = null; //continue

			int modX_varNum = -1;
			int modY_varNum = -1;
			int modZ_varNum = -1;
			int index_varNum = -1;
			int color_varNum = -1;

			c.GotoNext(MoveType.After, i => i.MatchLdloc(out index_varNum), i => i.MatchLdelema<Vector3>(), i => i.MatchLdloc(out color_varNum), i => i.MatchLdfld<Vector3>("Z"), i => i.MatchStfld<Vector3>("Z"),
				i => i.MatchLdloc(out modX_varNum), i => i.MatchLdloc(out modZ_varNum), i => i.MatchAnd(), i => i.MatchLdloc(out modY_varNum), i => i.MatchAnd(), i => i.MatchBrtrue(out IL_02bb));
			c.EmitLdarg(0);
			c.EmitLdloc(index_varNum);
			c.EmitLdloc(modX_varNum);
			c.EmitLdloc(modY_varNum);
			c.EmitLdloc(modZ_varNum);
			c.EmitLdloca(color_varNum);
			c.EmitDelegate((LightMap self, int index, bool flag, bool flag2, bool flag3, ref Vector3 zero) =>
			{
				byte potentialLiquidID = (byte)self._mask[index];
				if (potentialLiquidID >= LiquidID.Count)
				{
					Vector3 color = new Vector3(self.LightDecayThroughAir, self.LightDecayThroughAir, self.LightDecayThroughAir);
					LiquidLoader.GetLiquid(potentialLiquidID)?.ModifyLightMaskMode(index, ref color.X, ref color.Y, ref color.Z);
					if (!flag)
					{
						zero.X *= color.X;
					}
					if (!flag2)
					{
						zero.Y *= color.Y;
					}
					if (!flag3)
					{
						zero.Z *= color.Z;
					}
					return true;
				}
				return false;
			});
			c.EmitBrtrue(IL_02bb);
		}
	}
}