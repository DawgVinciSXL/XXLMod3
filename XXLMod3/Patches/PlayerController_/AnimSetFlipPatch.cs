using HarmonyLib;
using System;
using UnityEngine;
using XXLMod3.Controller;

namespace XXLMod3.Patches.PlayerController_
{
    [HarmonyPatch(typeof(PlayerController), "AnimSetFlip", new Type[] { typeof(float) })]
    class AnimSetFlipPatch
    {
        static bool Prefix(float p_value, ref float ____flipAxis, ref float ____flipAxisTarget)
        {
            if (Main.enabled)
            {
                float strength = GetStrength();
                ____flipAxisTarget = p_value;
                ____flipAxis = p_value * strength;
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