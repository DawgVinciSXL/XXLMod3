using HarmonyLib;
using System;
using UnityEngine;
using XXLMod3.Controller;

namespace XXLMod3.Patches.PlayerController_
{
    [HarmonyPatch(typeof(PlayerController), "LerpFlipAxis")]
    class LerpFlixAxisPatch
    {
        static bool Prefix(PlayerController __instance, ref float ____flipAxis, ref float ____flipAxisTarget)
        {
            if (Main.enabled)
            {
                float strength = GetStrength();
                ____flipAxis = Mathf.MoveTowards(____flipAxis, ____flipAxisTarget, Time.deltaTime * 15f);
                __instance.animationController.SetValue("FlipAxis", ____flipAxis * strength);
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