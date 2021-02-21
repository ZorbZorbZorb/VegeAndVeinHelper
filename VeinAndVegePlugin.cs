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

        private static void FixVeinsAndVegesIL(ILContext il) {
            ILCursor c = new ILCursor(il);

            int arrayLengthLoc = 0;

            for ( int i = 0; i < 2; i++ ) {
                c.GotoNext(MoveType.Before,
                    // Find where the array and index are loaded
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdloc(out _),
                    // Find where the ID of the Proto in the array at the index is grabbed
                    x => x.MatchLdelemRef(),
                    x => x.MatchLdfld(typeof(Proto).GetField("ID", BindingFlags.Public | BindingFlags.Instance)),
                    // Find where it adds 1 to the ID
                    x => x.MatchLdcI4(1),
                    x => x.MatchAdd(),
                    // Find where the length to make the arrays is stored
                    x => x.MatchStloc(out arrayLengthLoc)
                );

                // Move the cursor to inbetween the LdFld and Ldc_I4
                c.Index += 4;
                // Remove the Ldc_I4_1 and Add
                c.RemoveRange(2);

                // Load the current highest number found
                c.Emit(OpCodes.Ldloc, arrayLengthLoc);
                // Takes the next ID in the array and the current highest ID + 1 found and returns
                c.EmitDelegate<Func<int, int, int>>((newId, currentLength) => {
                    if ( newId + 1 > currentLength )
                        return newId + 1;
                    return currentLength;
                });
                // After this comes the Stloc, which stores the returned value back into the variable
            }
        }
    }
}
