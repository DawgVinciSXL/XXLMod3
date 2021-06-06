using HarmonyLib;
using UnityEngine;
using XXLMod3.Controller;
using XXLMod3.Core;

namespace XXLMod3.Patches.IKController_
{
    [HarmonyPatch(typeof(IKController), "MoveAndRotateSkaterIKTargets")]
    class MoveAndRotateSkaterIKTargetsPatch
    {
        static bool Prefix(Transform ___skaterLeftFootTargetParent, Transform ___skaterLeftFoot, Transform ___skaterRightFootTargetParent, Transform ___skaterRightFoot)
        {
            if (GetCustomLegs().Active && Main.enabled)
            {
                Vector3 lfPos = ___skaterLeftFoot.position;
                Vector3 rfPos = ___skaterRightFoot.position;

                lfPos += ___skaterLeftFoot.forward * GetCustomLegs().LeftLegZ + ___skaterLeftFoot.right * GetCustomLegs().LeftLegY + ___skaterLeftFoot.up * GetCustomLegs().LeftLegX;
                rfPos += ___skaterRightFoot.forward * GetCustomLegs().RightLegZ + ___skaterRightFoot.right * GetCustomLegs().RightLegY + ___skaterRightFoot.up * GetCustomLegs().RightLegX;
                ___skaterLeftFootTargetParent.position = lfPos;
                ___skaterLeftFootTargetParent.rotation = ___skaterLeftFoot.rotation;
                ___skaterRightFootTargetParent.position = rfPos;
                ___skaterRightFootTargetParent.rotation = ___skaterRightFoot.rotation;
                return false;
            }
            return true;
        }

        public static CustomLegSettings GetCustomLegs()
        {
            switch (XXLController.PopType)
            {
                case PopType.Ollie:
                    return Main.settings.OllieFlipLegs;
                case PopType.Nollie:
                    return Main.settings.NollieFlipLegs;
                case PopType.Switch:
                    return Main.settings.SwitchFlipLegs;
                case PopType.Fakie:
                    return Main.settings.FakieFlipLegs;
                default:
                    return Main.settings.DefaultFlipLegs;
            }
        }
    }
}