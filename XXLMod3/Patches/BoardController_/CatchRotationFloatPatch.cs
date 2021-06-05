//using HarmonyLib;
//using System;
//using UnityEngine;

//namespace XXLMod3.Patches.BoardController_
//{
//    [HarmonyPatch(typeof(BoardController), "CatchRotation", new Type[] { typeof(float) })]
//    class CatchRotationFloatPatch
//    {
//        static bool Prefix(ref float p_mag, BoardController __instance, ref float ____mag, ref float ____tempUpAngle, ref Transform ____catchForwardRotation, ref float ____catchSignedAngle, ref Transform ___catchRotation, ref float ____catchUpRotateSpeed, ref float ____catchRotateSpeed)
//        {
//            if (Main.enabled)
//            {
//                ____mag = Mathf.MoveTowards(____mag, p_mag, Time.deltaTime * 10f);
//                Vector3 vector = Vector3.ProjectOnPlane(PlayerController.Instance.skaterController.transform.up, __instance.boardRigidbody.transform.forward);
//                ____catchSignedAngle = Vector3.SignedAngle(vector, PlayerController.Instance.skaterController.skaterTransform.up, PlayerController.Instance.skaterController.skaterTransform.right);
//                PlayerController.Instance.AnimSetCatchAngle(____catchSignedAngle);
//                ____tempUpAngle = Vector3.Angle(__instance.boardTransform.up, PlayerController.Instance.skaterController.skaterTransform.up);
//                Quaternion b = Quaternion.LookRotation(__instance.catchRotation.forward, vector);
//                Quaternion a = Quaternion.Slerp(Quaternion.Slerp(__instance.boardRigidbody.rotation, b, Time.fixedDeltaTime * ____catchUpRotateSpeed), __instance.currentRotationTarget, Time.fixedDeltaTime * ((____tempUpAngle > 35f && ____tempUpAngle < 90f) ? ____catchUpRotateSpeed : ____catchRotateSpeed));
//                __instance.currentCatchRotationTarget = Quaternion.Slerp(a, __instance.currentRotationTarget, ____mag);
//                Core.Extentions.InvokeMethod(__instance, "PIDRotation", new object[] { __instance.currentCatchRotationTarget });
//                Vector3 vector2 = SkaterXL.Core.Mathd.LocalAngularVelocity(PlayerController.Instance.skaterController.skaterRigidbody);
//                Quaternion lhs = Quaternion.AngleAxis(Main.settings.CatchBoardRotateSpeed * vector2.y * Time.deltaTime, PlayerController.Instance.skaterController.transform.up);
//                __instance.boardTransform.rotation = lhs * __instance.boardTransform.rotation;
//                return false;
//            }
//            return true;
//        }
//    }
//}