using HarmonyLib;
using SkaterXL.Core;
using UnityEngine;
using XXLMod3.Controller;

namespace XXLMod3.Patches.IKController_
{
    [HarmonyPatch(typeof(IKController), "PrepIK")]
    class PrepIKPatch
    {
        static bool Prefix(ref Transform ___ikLeftFootPosition, ref Transform ___ikAnimLeftFootTarget, ref Transform ___ikRightFootPosition, ref Transform ___ikAnimRightFootTarget, ref Vector3 ____leftPos, ref Transform ___ikLeftFootPositionOffset, ref Transform ___ikLeftFootPositionOffsetGoofy, ref Vector3 ____skaterLeftFootPos, ref float ____ikLeftPosLerp, ref Vector3 ____rightPos, ref Transform ___ikRightFootPositionOffset, ref Transform ___ikRightFootPositionOffsetGoofy, ref Vector3 ____skaterRightFootPos, ref float ____ikRightPosLerp, ref Vector3 ____finalLeftPos, ref Vector3 ____finalRightPos, ref Transform ___rightHip, ref Transform ___leftHip)
        {
            if (Main.enabled)
            {
                ___ikLeftFootPosition.position = ___ikAnimLeftFootTarget.position;
                ___ikRightFootPosition.position = ___ikAnimRightFootTarget.position;
                ___ikLeftFootPosition.position = ___ikAnimLeftFootTarget.position;
                ___ikRightFootPosition.position = ___ikAnimRightFootTarget.position;
                ____leftPos = Vector3.Lerp((SettingsManager.Instance.stance == Stance.Regular) ? StanceController.LeftFootIndicator == null ? ___ikLeftFootPositionOffset.position : StanceController.LeftFootIndicator.transform.position : StanceController.LeftFootIndicator == null ? ___ikLeftFootPositionOffsetGoofy.position : StanceController.LeftFootIndicator.transform.position, ____skaterLeftFootPos, ____ikLeftPosLerp);
                ____rightPos = Vector3.Lerp((SettingsManager.Instance.stance == Stance.Regular) ? StanceController.LeftFootIndicator == null ? ___ikRightFootPositionOffset.position : StanceController.RightFootIndicator.transform.position : StanceController.RightFootIndicator == null ? ___ikRightFootPositionOffsetGoofy.position : StanceController.RightFootIndicator.transform.position, ____skaterRightFootPos, ____ikRightPosLerp);
                ____finalLeftPos = ____leftPos;
                ____finalRightPos = ____rightPos;
                if ((___rightHip.position - ____rightPos).magnitude > 0.7f)
                {
                    ____finalRightPos = ___rightHip.position + (____rightPos - ___rightHip.position).normalized * 0.7f;
                }
                if ((___leftHip.position - ____leftPos).magnitude > 0.7f)
                {
                    ____finalLeftPos = ___leftHip.position + (____leftPos - ___leftHip.position).normalized * 0.7f;
                }
                return false;
            }
            return true;
        }
    }
}
