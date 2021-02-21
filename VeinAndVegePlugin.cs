using BepInEx;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Reflection;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace VegeAndVeinHelper {
    [BepInPlugin("zorb.dsp.plugins.vegeandveinhelper", "Vege and Vein Helper", "1.0.0")]
    [BepInProcess("DSPGAME.exe")]
    public class VeinAndVegePlugin : BaseUnityPlugin 
    {
        public void Awake() 
        {
            new ILHook(typeof(PlanetModelingManager).GetMethod("PrepareWorks", BindingFlags.NonPublic | BindingFlags.Static), new ILContext.Manipulator(FixVeinsAndVegesIL));
        }

        private static void FixVeinsAndVegesIL(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            int arrayLengthLoc = 0;

            for (int i = 0; i < 2; i++)
            {
                c.GotoNext(MoveType.Before,
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdelemRef(),
                    x => x.MatchLdfld(typeof(Proto).GetField("ID", BindingFlags.Public | BindingFlags.Instance)),
                    x => x.MatchLdcI4(1),
                    x => x.MatchAdd(),
                    x => x.MatchStloc(out arrayLengthLoc)
                );

                c.Index += 4;
                c.RemoveRange(2);

                c.Emit(OpCodes.Ldloc, arrayLengthLoc);
                c.EmitDelegate<Func<int, int, int>>((newId, currentLength) =>
                {
                    if (newId + 1 > currentLength)
                        return newId + 1;
                    return currentLength;
                });
            }
        }
    }
}
