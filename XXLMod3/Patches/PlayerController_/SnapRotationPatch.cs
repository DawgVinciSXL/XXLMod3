using HarmonyLib;
using System;
using UnityEngine;

namespace XXLMod3.Patches.PlayerController_
{
    [HarmonyPatch(typeof(PlayerController), "SnapRotation", new Type[] { typeof(Quaternion) })]
    class SnapRotationPatch
    {
        static bool Prefix(Quaternion p_rot)
        {
            if (Main.enabled && !Main.settings.AutoBoardRotationSnap)
            {
                return false;
            }
            return true;
        }
    }
}