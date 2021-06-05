using HarmonyLib;

namespace XXLMod3.Patches.PlayerController_
{
    [HarmonyPatch(typeof(PlayerController), "AutoPump")]
    class AutoPumpPatch
    {
        static bool Prefix()
        {
            if (!Main.settings.AutoPump && Main.enabled)
            {
                return false;
            }
            return true;
        }
    }
}