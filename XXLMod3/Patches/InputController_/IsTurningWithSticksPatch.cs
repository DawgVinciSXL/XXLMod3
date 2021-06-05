using HarmonyLib;

namespace XXLMod3.Patches.InputController_
{
    [HarmonyPatch(typeof(InputController), "IsTurningWithSticks")]
    class IsTurningWithSticksPatch
    {
        static bool Prefix()
        {
            if (Main.enabled && Main.settings.ReducedStickTurning)
            {
                return false;
            }
            return true;
        }
    }
}