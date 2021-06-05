using HarmonyLib;
using UnityEngine;

namespace XXLMod3.Patches.IKController_
{
    [HarmonyPatch(typeof(IKController), "MoveAndRotateSkaterIKTargets")]
    class MoveAndRotateSkaterIKTargetsPatch
    {
        static bool Prefix(Transform ___skaterLeftFootTargetParent, Transform ___skaterLeftFoot, Transform ___skaterRightFootTargetParent, Transform ___skaterRightFoot)
        {
            if (Main.settings.FlipLegs.Active && Main.enabled)
            {
                Vector3 lfPos = ___skaterLeftFoot.position;
                Vector3 rfPos = ___skaterRightFoot.position;

                lfPos += ___skaterLeftFoot.forward * Main.settings.FlipLegs.LeftLegZ + ___skaterLeftFoot.right * Main.settings.FlipLegs.LeftLegY + ___skaterLeftFoot.up * Main.settings.FlipLegs.LeftLegX;
                rfPos += ___skaterRightFoot.forward * Main.settings.FlipLegs.RightLegZ + ___skaterRightFoot.right * Main.settings.FlipLegs.RightLegY + ___skaterRightFoot.up * Main.settings.FlipLegs.RightLegX;
                ___skaterLeftFootTargetParent.position = lfPos;
                ___skaterLeftFootTargetParent.rotation = ___skaterLeftFoot.rotation;
                ___skaterRightFootTargetParent.position = rfPos;
                ___skaterRightFootTargetParent.rotation = ___skaterRightFoot.rotation;
                return false;
            }
            return true;
        }
    }
}