using HarmonyLib;
using UnityEngine;
using XXLMod3.Controller;

namespace XXLMod3.Patches.IKController_
{
    [HarmonyPatch(typeof(IKController), "SetIK")]
    class SetIKPatch
    {
        static bool Prefix(IKController __instance, ref Vector3 ____finalLeftPos, ref Vector3 ____finalRightPos, ref Transform ___ikAnimLeftFootTarget, ref Quaternion ____skaterLeftFootRot, ref float ____ikLeftRotLerp, ref Transform ___ikAnimRightFootTarget, ref Quaternion ____skaterRightFootRot, ref float ____ikRightRotLerp, ref float ____leftPositionWeight, ref float ____rightPositionWeight, ref float ____leftRotationWeight, ref float ____rightRotationWeight)
        {
            if (Main.enabled)
            {
                __instance._finalIk.solver.leftFootEffector.position = ____finalLeftPos;
                __instance._finalIk.solver.rightFootEffector.position = ____finalRightPos;
                __instance._finalIk.solver.leftFootEffector.rotation = Quaternion.Slerp(StanceController.LeftFootRotIndicator == null ? ___ikAnimLeftFootTarget.rotation : StanceController.LeftFootRotIndicator.transform.rotation, ____skaterLeftFootRot, ____ikLeftRotLerp);
                __instance._finalIk.solver.rightFootEffector.rotation = Quaternion.Slerp(StanceController.RightFootRotIndicator == null ? ___ikAnimRightFootTarget.rotation : StanceController.RightFootRotIndicator.transform.rotation, ____skaterRightFootRot, ____ikRightRotLerp);
                __instance._finalIk.solver.leftFootEffector.positionWeight = Mathf.MoveTowards(__instance._finalIk.solver.leftFootEffector.positionWeight, ____leftPositionWeight, Time.deltaTime * 10f);
                __instance._finalIk.solver.rightFootEffector.positionWeight = Mathf.MoveTowards(__instance._finalIk.solver.rightFootEffector.positionWeight, ____rightPositionWeight, Time.deltaTime * 10f);
                __instance._finalIk.solver.rightFootEffector.rotationWeight = Mathf.MoveTowards(__instance._finalIk.solver.rightFootEffector.rotationWeight, ____rightRotationWeight, Time.deltaTime * 5f);
                __instance._finalIk.solver.leftFootEffector.rotationWeight = Mathf.MoveTowards(__instance._finalIk.solver.leftFootEffector.rotationWeight, ____leftRotationWeight, Time.deltaTime * 5f);
                return false;
            }
            return true;
        }
    }
}