using HarmonyLib;
using System;
using UnityEngine;
using XXLMod3.Controller;

namespace XXLMod3.Patches.PlayerController_
{
    [HarmonyPatch(typeof(PlayerController), "AnimForceFlipValue", new Type[] { typeof(float) })]
    class AnimForceFlipValuePatch
    {
        static bool Prefix(float p_value, PlayerController __instance, ref float ____flipAxis, ref float ____flipAxisTarget)
        {
            if (Main.enabled)
            {
                float strength = GetStrength();
                ____flipAxisTarget = p_value;
                ____flipAxis = p_value * strength;
                __instance.animationController.SetValue("FlipAxis", p_value);
                return false;
            }
            return true;
        }

        private static float GetStrength()
        {
            if (XXLController.Instance.IsLateFlip)
            {
                return Main.settings.LateFlipStrength;
            }
            else if (XXLController.Instance.IsPrimoFlip)
            {
                return Main.settings.PrimoFlipStrength;
            }
            return Main.settings.FlipStrength;
        }
    }
}