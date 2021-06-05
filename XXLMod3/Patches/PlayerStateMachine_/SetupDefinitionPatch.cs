using FSMHelper;
using HarmonyLib;
using System;
using System.Collections.Generic;
using XXLMod3.PlayerStates;

namespace XXLMod3.Patches.PlayerStateMachine_
{
    [HarmonyPatch(typeof(PlayerStateMachine), "SetupDefinition")]
    class SetupDefinitionPatch
    {
        public static bool Prefix(ref FSMStateType stateType, ref List<Type> children)
        {
            if (Main.enabled)
            {
                children.Add(typeof(Custom_Riding));
                children.Add(typeof(Custom_Pushing));
                children.Add(typeof(Custom_Setup));
                children.Add(typeof(Custom_BeginPop));
                children.Add(typeof(Custom_Pop));
                children.Add(typeof(Custom_InAir));
                children.Add(typeof(Custom_EnterCoping));
                children.Add(typeof(Custom_ExitCoping));
                children.Add(typeof(Custom_Impact));
                children.Add(typeof(Custom_Braking));
                children.Add(typeof(Custom_Powerslide));
                children.Add(typeof(Custom_Grinding));
                children.Add(typeof(Custom_Manualling));
                children.Add(typeof(Custom_Grabs));
                children.Add(typeof(Custom_Released));
                children.Add(typeof(Custom_Bailed));
                children.Add(typeof(Custom_Primo));
                children.Add(typeof(Custom_PrimoSetup));
                children.Add(typeof(Custom_Lateflip));
                children.Add(typeof(Custom_Darkslide));
                return false;
            }
            return true;
        }
    }
}