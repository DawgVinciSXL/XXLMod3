using HarmonyLib;

namespace XXLMod3.Patches.TriggerManager_
{
    [HarmonyPatch(typeof(TriggerManager), "GrindTriggerCheck")]
    class GrindTriggerCheckPatch
    {
        private static void Postfix(ref float ____maxStallTime)
        {
            if (Main.enabled && Main.settings.InfiniteStallTime)
            {
                ____maxStallTime = 999f;
            }
        }
    }
}