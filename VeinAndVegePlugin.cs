using BepInEx;
using HarmonyLib;
using System.Security;
using System.Security.Permissions;
using VegeAndVeinHelper.Patches;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace VegeAndVeinHelper {
    [BepInDependency("me.xiaoye97.plugin.Dyson.LDBTool", "1.8.0")]
    [BepInPlugin("zorb.dsp.plugins.vegeandveinhelper", "Vege and Vein Helper", "1.0.0")]
    [BepInProcess("DSPGAME.exe")]
    public class VeinAndVegePlugin : BaseUnityPlugin {
        internal void Awake() {
            Harmony harmony = new Harmony("zorb.dsp.plugins.vegeandveinhelper");
            Harmony.CreateAndPatchAll(typeof(PatchPlanetModelingManager), harmony.Id);
        }
    }
}
