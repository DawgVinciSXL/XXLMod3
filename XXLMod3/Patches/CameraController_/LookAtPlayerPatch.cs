using HarmonyLib;
using UnityEngine;

namespace XXLMod3.Patches.CameraController_
{
    [HarmonyPatch(typeof(CameraController), "LookAtPlayer")]
    class LookAtPlayerPatch
    {
        static void Postfix(ref Transform ____actualCam)
        {
            if(Main.enabled && Main.settings.BailLookAtPlayer)
            {
                ____actualCam.LookAt(PlayerController.Instance.respawn.behaviourPuppet.puppetMaster.muscles[0].transform);
            }
        }
    }
}