using HarmonyLib;
using System;

namespace XXLMod3.Patches.PlayerController_
{
    [HarmonyPatch(typeof(PlayerController), "PowerslideRotation", new Type[] { typeof(float) })]
    class PowerslideRotationPatch
    {
        static bool Prefix(PlayerController __instance, float p_value)
        {
            if (Main.enabled)
            {
                __instance.boardController.PowerslideRotation(p_value * Main.settings.RevertSpeed);
                return false;
            }
            return true;
        }
    }
}