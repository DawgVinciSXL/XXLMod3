using HarmonyLib;
using XXLMod3.PlayerStates;

namespace XXLMod3.Patches.PlayerState_OffBoard_
{
    [HarmonyPatch(typeof(PlayerState_OffBoard), "OnRespawn")]
    class OnRespawnPatch
    {
        static bool Prefix(PlayerState_OffBoard __instance)
        {
            if (Main.enabled)
            {
                PlayerController.Instance.ResetAllAnimations();
                PlayerController.Instance.AnimGrindTransition(false);
                PlayerController.Instance.AnimOllieTransition(false);
                PlayerController.Instance.AnimSetupTransition(false);
                __instance.DoTransition(typeof(XXLMod3.PlayerStates.Custom_Riding), null);
                return false;
            }
            return true;
        }
    }
}