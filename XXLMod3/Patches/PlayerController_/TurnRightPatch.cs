//using System;
//using HarmonyLib;
//using SkaterXL.Core;

//namespace XXLMod3.Patches.PlayerController_
//{
//    [HarmonyPatch(typeof(PlayerController), "TurnLeft", new Type[] { typeof(float), typeof(InputController.TurningMode) })]
//    class TurnRightPatch
//    {
//        static bool Prefix(float p_value, InputController.TurningMode p_turningMode)
//        {
//            if (!Main.enabled)
//            {
//                return true;
//            }
//            switch (p_turningMode)
//            {
//                case InputController.TurningMode.Grounded:
//                    PlayerController.Instance.boardController.AddTurnTorque(p_value);
//                    PlayerController.Instance.skaterController.AddTurnTorque(p_value * PlayerController.Instance.torsoTorqueMult);
//                    return false;
//                case InputController.TurningMode.PreWind:
//                    PlayerController.Instance.boardController.AddTurnTorque(p_value / 5f);
//                    return false;
//                case InputController.TurningMode.InAir:
//                    PlayerController.Instance.skaterController.AddTurnTorque(p_value * Main.settings.InAirTurnSpeed);
//                    return false;
//                case InputController.TurningMode.FastLeft:
//                    if (SettingsManager.Instance.stance == Stance.Regular)
//                    {
//                        PlayerController.Instance.skaterController.AddTurnTorque(p_value);
//                        return false;
//                    }
//                    PlayerController.Instance.skaterController.AddTurnTorque(p_value, true);
//                    return false;
//                case InputController.TurningMode.FastRight:
//                    if (SettingsManager.Instance.stance == Stance.Regular)
//                    {
//                        PlayerController.Instance.skaterController.AddTurnTorque(p_value, true);
//                        return false;
//                    }
//                    PlayerController.Instance.skaterController.AddTurnTorque(p_value);
//                    return false;
//                case InputController.TurningMode.Manual:
//                    PlayerController.Instance.boardController.AddTurnTorqueManuals(p_value);
//                    return false;
//                case InputController.TurningMode.Powerslide:
//                    PlayerController.Instance.skaterController.AddTurnTorque(p_value);
//                    return false;
//                default:
//                    return false;
//            }
//        }
//    }
//}