using HarmonyLib;
using System;
using UnityEngine;
using XXLMod3.Controller;

namespace XXLMod3.Patches.BoardController_
{
    [HarmonyPatch(typeof(PlayerController), "AddPumpForce", new Type[] { typeof(float) })]
    class AddPumpForcePatch
    {
        static bool Prefix(float p_force)
        {
            if (Main.enabled)
            {
                PlayerController.Instance.boardController.boardRigidbody.AddForce(PlayerController.Instance.boardController.boardRigidbody.velocity.normalized * p_force * Main.settings.PumpForceMult, ForceMode.VelocityChange);
                return false;
            }
            return true;
        }
    }
}