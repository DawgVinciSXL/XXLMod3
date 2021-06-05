using HarmonyLib;
using UnityEngine;

namespace XXLMod3.Patches.SkaterController_
{
    [HarmonyPatch(typeof(SkaterController), "PredictLanding")]
    class PredictLandingPatch
    {
        static bool Prefix(Vector3 p_popForce, SkaterController __instance, ref float ____startTime, ref float ____duration, ref Quaternion ____startRotation, ref Vector3 ____startUpVector, ref bool ____landingPrediction, ref Quaternion ____newUp, Vector3 __result)
        {
            if (Main.enabled && !Main.settings.AutoBanklean)
            {
                p_popForce = __instance.skaterRigidbody.velocity + p_popForce;
                ____startTime = Time.time;
                Vector3 zero = Vector3.zero;
                ____duration = PlayerController.Instance.boardController.trajectory.CalculateTrajectory(PlayerController.Instance.boardController.boardTransform.position, p_popForce, 5f, out zero);
                ____startRotation = __instance.skaterRigidbody.rotation;
                ____startUpVector = __instance.skaterTransform.up;
                ____landingPrediction = true;
                ____newUp = Quaternion.FromToRotation(____startUpVector, ____startUpVector);
                ____newUp *= __instance.skaterRigidbody.rotation;
                __result = zero;
                return false;
            }
            return true;
        }
    }
}