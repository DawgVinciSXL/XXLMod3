using GameManagement;
using HarmonyLib;
using UnityEngine;

namespace XXLMod3.Patches.PlayState_
{
    [HarmonyPatch(typeof(PlayState), "OnUpdate")]
    class OnUpdatePatch
    {
        static bool Prefix()
        {
            if (Main.enabled && Main.settings.UseEscToQuit)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Application.Quit();
                    return false;
                }
                return true;
            }
            return true;
        }
    }
}