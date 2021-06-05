//using HarmonyLib;
//using UnityEngine;

//namespace XXLMod3.Patches.BoardController_
//{
//    [HarmonyPatch(typeof(BoardController), "SetBoardControllerUpVector")]
//    class SetBoardControllerUpVectorPatch
//    {
//        static bool Prefix(Vector3 _up, BoardController __instance)
//        {
//            if(Main.enabled && Main.settings.CatchCorrection)
//            {
//                Quaternion quaternion = Quaternion.FromToRotation(__instance.boardTransform.up, _up) * __instance.boardTransform.rotation;
//                __instance.boardControlTransform.rotation = __instance.boardTransform.rotation;
//                return false;
//            }
//            return true;
//        }
//    }
//}
