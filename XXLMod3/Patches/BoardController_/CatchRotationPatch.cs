using HarmonyLib;
using System;
using UnityEngine;

namespace XXLMod3.Patches.BoardController_
{
    [HarmonyPatch(typeof(BoardController), "CatchRotation", new Type[] { })]
    class CatchRotationPatch
    {
        static bool Prefix(BoardController __instance, ref Transform ____catchForwardRotation, ref float ____catchSignedAngle, ref Transform ___catchRotation)
        {
            if (Main.enabled && !Main.settings.CatchCorrection)
            {
                Vector3 from = Vector3.ProjectOnPlane(PlayerController.Instance.skaterController.transform.up, ____catchForwardRotation.forward);
                ____catchSignedAngle = Vector3.SignedAngle(from, PlayerController.Instance.skaterController.skaterTransform.up, PlayerController.Instance.skaterController.skaterTransform.right);
                PlayerController.Instance.AnimSetCatchAngle(____catchSignedAngle);
                ___catchRotation.rotation = Quaternion.Slerp(__instance.boardTransform.rotation, ____catchForwardRotation.rotation, Time.fixedDeltaTime * Main.settings.CatchCorrectionSpeed);
                Core.Extentions.InvokeMethod(__instance, "PIDRotation", new object[] { ___catchRotation.rotation });
                return false;
            }
            return true;
        }
    }
}