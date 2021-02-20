using HarmonyLib;
using UnityEngine;

namespace VegeAndVeinHelper.Patches {
    [HarmonyPatch(typeof(PlanetModelingManager))]
	public static class PatchPlanetModelingManager {
		[HarmonyPrefix]
		[HarmonyPatch("PrepareWorks")]
		private static bool PrepareWorks() {
			//// This is the original code, commented out, that was changed. Patch below
			//int num = 0;
			//VegeProto[] dataArray = LDB.veges.dataArray;
			//for ( int i = 0; i < dataArray.Length; i++ ) {
			//	num = dataArray[i].ID + 1;
			//	Debug.Log($"i:{i}, num:{num} | from dataArray[{i}].ID = {dataArray[i].ID}");
			//}
			//PlanetModelingManager.vegeHps = new short[num];
			//PlanetModelingManager.vegeScaleRanges = new Vector4[num];
			//PlanetModelingManager.vegeProtos = new VegeProto[num];

			// This is the new code for the above commented code.
			// Normally the devs code looks through the array to find the LAST elements ID and creates arrays for the
			//		planet modeling manager of that ID + 1.
			//		In the vanilla game, the last vege has a sentinal ID value of 9999
			//		This patch works by changes the logic to select the highest ID instead of the last ID.
			//		This patch will still only allow for 10000 veges, but you shouldnt be adding more than that.
			VegeProto[] dataArray = LDB.veges.dataArray;
			int num = 0;
			for ( int i = 0; i < dataArray.Length; i++ ) {
				if ( dataArray[i].ID + 1 > num ) {
					num = dataArray[i].ID + 1;
				}
			}
			PlanetModelingManager.vegeHps = new short[num];
			PlanetModelingManager.vegeScaleRanges = new Vector4[num];
			PlanetModelingManager.vegeProtos = new VegeProto[num];
			// End of modified code

			for ( int j = 0; j < dataArray.Length; j++ ) {
				PlanetModelingManager.vegeHps[dataArray[j].ID] = (short)dataArray[j].HpMax;
				PlanetModelingManager.vegeScaleRanges[dataArray[j].ID] = dataArray[j].ScaleRange;
				PlanetModelingManager.vegeProtos[dataArray[j].ID] = dataArray[j];
			}


			// Veins has the exact same bug in it. The same patch is applied.
			//VeinProto[] dataArray2 = LDB.veins.dataArray;
			//for ( int k = 0; k < dataArray2.Length; k++ ) {
			//	num = dataArray2[k].ID + 1;
			//}
			//PlanetModelingManager.veinProducts = new int[num];
			//PlanetModelingManager.veinModelIndexs = new int[num];
			//PlanetModelingManager.veinModelCounts = new int[num];

			// This is the new code for the above commented code.
			VeinProto[] dataArray2 = LDB.veins.dataArray;
			for ( int i = 0; i < dataArray2.Length; i++ ) {
				if ( dataArray2[i].ID + 1 > num ) {
					num = dataArray2[i].ID + 1;
				}
			}
			PlanetModelingManager.veinProducts = new int[num];
			PlanetModelingManager.veinModelIndexs = new int[num];
			PlanetModelingManager.veinModelCounts = new int[num];
			PlanetModelingManager.veinProtos = new VeinProto[num];
			// End of modified code

			for ( int l = 0; l < dataArray2.Length; l++ ) {
				PlanetModelingManager.veinProducts[dataArray2[l].ID] = dataArray2[l].MiningItem;
				PlanetModelingManager.veinModelIndexs[dataArray2[l].ID] = dataArray2[l].ModelIndex;
				PlanetModelingManager.veinModelCounts[dataArray2[l].ID] = dataArray2[l].ModelCount;
				PlanetModelingManager.veinProtos[dataArray2[l].ID] = dataArray2[l];
			}

			// Prevent original call.
			return false;
		}
	}
}

