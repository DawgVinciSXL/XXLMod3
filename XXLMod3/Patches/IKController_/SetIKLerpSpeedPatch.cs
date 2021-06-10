using HarmonyLib;
using System;

namespace XXLMod3.Patches.IKController_
{
    [HarmonyPatch(typeof(IKController), "SetIKLerpSpeed", new Type[] { typeof(float) })]
    class SetIKLerpSpeedPatch
    {
        static bool Prefix(float p_speed, ref float ____lerpSpeed)
        {
            if (Main.enabled && (PlayerController.Instance.currentStateEnum == PlayerController.CurrentState.Release))
            {
                ____lerpSpeed = Main.settings.CatchFootIkLerpSpeed;
                return false;
            }
            return true;
        }
    }
}