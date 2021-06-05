//using HarmonyLib;
//using UnityEngine;

//namespace XXLMod3.Patches.BoardController_
//{
//    [HarmonyPatch(typeof(BoardController), "SetCatchForwardRotation")]
//    class SetCatchForwardRotationPatch
//    {
//        static bool Prefix(BoardController __instance, ref Transform ____catchForwardRotation)
//        {
//            if (Main.enabled && Main.settings.CatchSmooth)
//            {
//                Vector3 vector = (Vector3.Angle(Vector3.ProjectOnPlane(__instance.boardTransform.up, PlayerController.Instance.skaterController.skaterTransform.forward), PlayerController.Instance.skaterController.skaterTransform.up) > 90f) ? (-__instance.boardTransform.right) : __instance.boardTransform.right;
//                Vector3 vector2 = Vector3.Cross(__instance.boardTransform.forward, Vector3.ProjectOnPlane(vector, PlayerController.Instance.skaterController.skaterTransform.up));
//                vector2 = Vector3.ProjectOnPlane(vector2, PlayerController.Instance.skaterController.skaterTransform.right);
//                if (Vector3.Angle(vector2, PlayerController.Instance.skaterController.skaterTransform.up) > 30f)
//                {
//                    if (Vector3.Angle(vector2, PlayerController.Instance.skaterController.skaterTransform.forward) < 90f)
//                    {
//                        Vector3 planeNormal = Quaternion.AngleAxis(-60f, PlayerController.Instance.skaterController.skaterTransform.right) * PlayerController.Instance.skaterController.skaterTransform.up;
//                        vector2 = Vector3.ProjectOnPlane(vector2, planeNormal);
//                    }
//                    else
//                    {
//                        Vector3 planeNormal2 = Quaternion.AngleAxis(60f, PlayerController.Instance.skaterController.skaterTransform.right) * PlayerController.Instance.skaterController.skaterTransform.up;
//                        vector2 = Vector3.ProjectOnPlane(vector2, planeNormal2);
//                    }
//                }
//                Vector3 forward;
//                if (Vector3.Dot(__instance.boardTransform.forward, PlayerController.Instance.skaterController.skaterTransform.forward) <= 0f)
//                {
//                    forward = -PlayerController.Instance.skaterController.skaterTransform.forward;
//                }
//                else
//                {
//                    forward = PlayerController.Instance.skaterController.skaterTransform.forward;
//                }
//                Vector3 vector3 = Vector3.ProjectOnPlane(__instance.boardTransform.forward, vector2);
//                Vector3 forward2 = Vector3.ProjectOnPlane(Vector3.ProjectOnPlane(vector3, PlayerController.Instance.skaterController.skaterTransform.right), vector2);
//                Quaternion rotation = Quaternion.LookRotation(vector3, vector2);
//                Quaternion rotation2 = Quaternion.LookRotation(forward2, vector2);
//                __instance.catchRotation.rotation = __instance.boardTransform.rotation;
//                ____catchForwardRotation.rotation = rotation2;
//                return false;
//            }
//            return true;
//        }
//    }
//}
