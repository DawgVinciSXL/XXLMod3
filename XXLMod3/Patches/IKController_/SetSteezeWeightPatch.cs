using HarmonyLib;
using UnityEngine;
using XXLMod3.Controller;
using XXLMod3.Core;

namespace XXLMod3.Patches.IKController_
{
    [HarmonyPatch(typeof(IKController), "SetSteezeWeight")]
    class SetSteezeWeightPatch
    {
        static bool Prefix(ref float ____leftSteezeTarget, ref float ____leftSteezeMax, ref float ____rightSteezeTarget, ref float ____rightSteezeMax, ref float ____leftSteezeWeight, ref float ____steezeLerpSpeed, ref float ____rightSteezeWeight, ref Vector3 ____skaterRightFootPos, ref Quaternion ____skaterRightFootRot, ref Transform ___steezeRightFootTarget, ref Transform ___steezeLeftFootTarget, ref Vector3 ____skaterLeftFootPos, ref Quaternion ____skaterLeftFootRot, ref Transform ___skaterLeftFootTarget, ref Transform ___skaterRightFootTarget)
        {
            if (Main.enabled)
            {
                if (PlayerController.Instance.currentStateEnum == PlayerController.CurrentState.Release)
                {
                    if (GetCustomLegs().Active)
                    {
                        float target = Mathf.Clamp(____leftSteezeTarget, 0f, ____leftSteezeMax);
                        float target2 = Mathf.Clamp(____rightSteezeTarget, 0f, ____rightSteezeMax);
                        if (PlayerController.Instance.playerSM.LeftFootOffSM())
                        {
                            target = 1f;
                        }
                        if (PlayerController.Instance.playerSM.RightFootOffSM())
                        {
                            target2 = 1f;
                        }

                        ____leftSteezeWeight = Mathf.MoveTowards(____leftSteezeWeight, target, Time.deltaTime * ____steezeLerpSpeed);
                        ____rightSteezeWeight = Mathf.MoveTowards(____rightSteezeWeight, target2, Time.deltaTime * ____steezeLerpSpeed);

                        Vector3 lfPos = ___steezeLeftFootTarget.position;
                        Vector3 rfPos = ___steezeRightFootTarget.position;

                        lfPos += ___steezeLeftFootTarget.forward * GetCustomLegs().LeftLegZ + ___steezeLeftFootTarget.right * GetCustomLegs().LeftLegY + ___steezeLeftFootTarget.up * GetCustomLegs().LeftLegX;
                        rfPos += ___steezeRightFootTarget.forward * GetCustomLegs().RightLegZ + ___steezeRightFootTarget.right * GetCustomLegs().RightLegY + ___steezeRightFootTarget.up * GetCustomLegs().RightLegX;

                        ____skaterLeftFootPos = Vector3.Lerp(___skaterLeftFootTarget.position, lfPos, ____leftSteezeWeight);
                        ____skaterLeftFootRot = Quaternion.Slerp(___skaterLeftFootTarget.rotation, ___steezeLeftFootTarget.rotation, ____leftSteezeWeight);

                        ____skaterRightFootPos = Vector3.Lerp(___skaterRightFootTarget.position, rfPos, ____rightSteezeWeight);
                        ____skaterRightFootRot = Quaternion.Slerp(___skaterRightFootTarget.rotation, ___steezeRightFootTarget.rotation, ____rightSteezeWeight);
                        return false;
                    }
                    return true;
                }
                return true;
            }
            return true;
        }

        public static CustomLegSettings GetCustomLegs()
        {
            switch (XXLController.PopType)
            {
                case PopType.Ollie:
                    return Main.settings.OllieSteezeLegs;
                case PopType.Nollie:
                    return Main.settings.NollieSteezeLegs;
                case PopType.Switch:
                    return Main.settings.SwitchSteezeLegs;
                case PopType.Fakie:
                    return Main.settings.FakieSteezeLegs;
                default:
                    return Main.settings.DefaultSteezeLegs;
            }
        }
    }
}