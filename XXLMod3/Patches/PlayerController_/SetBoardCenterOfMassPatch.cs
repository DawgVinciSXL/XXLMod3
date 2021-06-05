using HarmonyLib;
using System;
using UnityEngine;
using XXLMod3.Controller;

namespace XXLMod3.Patches.PlayerController_
{
    [HarmonyPatch(typeof(PlayerController), "SetBoardCenterOfMass", new Type[] { typeof(Vector3) })]
    class SetBoardCenterOfMassPatch
    {
        static bool Prefix(Vector3 p_position)
        {
            if (PlayerController.Instance.currentStateEnum == PlayerController.CurrentState.Grinding && Main.enabled && !XXLController.Instance.GrindStabilizer)
            {
                return false;
            }
            return true;
        }
    }
}