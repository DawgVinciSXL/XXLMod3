using HarmonyLib;
using System;
using UnityEngine;
using XXLMod3.Controller;
using XXLMod3.Core;

namespace XXLMod3.Patches.PlayerController_
{
    [HarmonyPatch(typeof(PlayerController), "OnPop", new Type[] { typeof(float), typeof(float), typeof(Vector3) })]
    class OnPopFFV3Patch
    {
        static bool Prefix(float p_pop, float p_scoop, Vector3 p_popOutDir, PlayerController __instance)
        {
            if (Main.enabled)
            {
                __instance.VelocityOnPop = __instance.boardController.boardRigidbody.velocity;
                __instance.SetTurningMode(InputController.TurningMode.InAir);
                __instance.SetSkaterToMaster();
                Vector3 vector = (__instance.skaterController.skaterTransform.up + p_popOutDir * XXLController.Instance.GrindPopOutSidewayForce) * p_pop;
                Vector3 vector2 = __instance.skaterController.PredictLanding(vector);
                Vector3 vector3 = (Vector3)Utils.InvokeMethod(__instance, "CheckDistanceAfterPop", new object[] { (vector + vector2) });
                __instance.skaterController.skaterRigidbody.AddForce((vector3 == Vector3.zero) ? vector : (vector + vector3), ForceMode.Impulse);
                __instance.skaterController.skaterRigidbody.AddForce(vector2, ForceMode.Impulse);
                SoundManager.Instance.PlayPopSound(p_scoop);
                __instance.comController.popForce = vector;
                return false;
            }
            return true;
        }
    }
}