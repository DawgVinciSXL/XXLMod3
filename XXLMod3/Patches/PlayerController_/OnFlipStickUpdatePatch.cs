using HarmonyLib;
using UnityEngine;
using XXLMod3.Controller;
using XXLMod3.Core;

namespace XXLMod3.Patches.PlayerController_
{
    [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.OnFlipStickUpdate))]
    class OnFlipStickUpdatePatch
    {
        static void Prefix(ref bool p_p_flipDetected, ref bool p_potentialFlip, ref Vector2 p_initialFlipDir, ref int p_p_flipFrameCount, ref int p_p_flipFrameMax, ref float p_toeAxis, ref float p_p_flipVel, ref float p_popVel, ref float p_popDir, ref float p_flip, ref StickInput p_flipStick, ref bool p_releaseBoard, ref bool p_isSettingUp, ref float p_invertVel, ref float p_augmentedAngle, bool popRotationDone, ref bool p_forwardLoad, ref float p_flipWindowTimer)
        {
            if (Main.enabled)
            {
                if (!p_p_flipDetected)
                {
                    float num = PlayerController.Instance.flipMult * p_flipStick.PopToeSpeed;
                    if (num > PlayerController.Instance.flipThreshold)
                    {
                        float num2 = Vector3.Angle(p_flipStick.PopToeVector, Vector2.up);
                        if (num2 < 150f && num2 > 15f && p_flipStick.PopToeVector.magnitude > PlayerController.Instance.flipStickDeadZone && Vector2.Angle(p_flipStick.PopToeVel, p_flipStick.PopToeVector - Vector2.zero) < 90f)
                        {
                            p_initialFlipDir = p_flipStick.PopToeVector;
                            p_toeAxis = p_flipStick.FlipDir;
                            p_flip = p_flipStick.ToeAxis;
                            float num3 = p_flipStick.ForwardDir;
                            if (num3 <= 0.2f)
                            {
                                num3 += 0.2f;
                            }
                            p_popDir = Mathf.Clamp(num3, 0f, 1f);
                            p_p_flipVel = num;
                            p_flipWindowTimer = 0f;
                            p_p_flipDetected = true;
                            PlayerController.Instance.playerSM.PoppedSM();
                            return;
                        }
                    }
                }
                else
                {
                    float p_value = (p_flip == 0f) ? 0f : ((p_flip > 0f) ? 1f : -1f);
                    PlayerController.Instance.AnimSetFlip(p_value);
                    //GetFlipDirection(p_value);
                    PlayerController.Instance.animationController.ScaleAnimSpeed(GetAnimationSpeed());
                    PlayerController.Instance.AnimRelease(true);
                    if (PlayerController.Instance.playerSM.PoppedSM())
                    {
                        PlayerController.Instance.SetLeftIKLerpTarget(1f);
                        PlayerController.Instance.SetRightIKLerpTarget(1f);
                    }
                    float num4 = (p_toeAxis == 0f) ? 0f : ((p_toeAxis > 0f) ? 1f : -1f);
                    float num5 = PlayerController.Instance.boneMult * p_flipStick.PopToeVel.y;
                    float num6 = (float)(p_forwardLoad ? -1 : 1);
                    bool flag = false;
                    float num7 = PlayerController.Instance.flipMult * p_flipStick.PopToeSpeed;
                    if ((Mathf.Sign(p_flipStick.ToeAxisVel) == Mathf.Sign(p_flip) || !PlayerController.Instance.playerSM.PoppedSM()) && Mathf.Abs(num7) > Mathf.Abs(p_p_flipVel))
                    {
                        p_p_flipVel = num7;
                        p_flipWindowTimer = 0f;
                    }
                    if (Mathf.Abs(p_invertVel) == 0f || (Mathf.Sign(num5) == Mathf.Sign(1f) && Mathf.Abs(num5) > Mathf.Abs(p_invertVel)))
                    {
                        p_invertVel = num6 * num5;
                    }
                    if (!flag && !PlayerController.Instance.playerSM.PoppedSM())
                    {
                        p_flipWindowTimer += Time.deltaTime;
                        if (p_flipWindowTimer >= 0.3f)
                        {
                            p_p_flipVel = 0f;
                            p_invertVel = 0f;
                            p_p_flipDetected = false;
                            PlayerController.Instance.AnimRelease(false);
                            PlayerController.Instance.AnimSetFlip(0f);
                            PlayerController.Instance.AnimForceFlipValue(0f);
                            p_flipWindowTimer = 0f;
                        }
                    }
                    PlayerController.Instance.SetFlipSpeed(Mathf.Clamp(p_p_flipVel, -4000f, 4000f) * num4);
                }
            }
            return;
        }

        private static float GetAnimationSpeed()
        {
            if (XXLController.Instance.IsLateFlip)
            {
                return Main.settings.LateFlipAnimationSpeed;
            }
            else if (XXLController.Instance.IsPrimoFlip)
            {
                return Main.settings.PrimoFlipAnimationSpeed;
            }
            return Main.settings.FlipAnimationSpeed;
        }

        private static void GetFlipDirection(float flipDir)
        {
            if(flipDir > 0.1f)
            {
                if(SettingsManager.Instance.stance == SkaterXL.Core.Stance.Regular)
                {
                    UnityModManagerNet.UnityModManager.Logger.Log("Kickflip");
                    return;
                }
                UnityModManagerNet.UnityModManager.Logger.Log("Heelflip");
                return;
            }
            if (SettingsManager.Instance.stance == SkaterXL.Core.Stance.Regular)
            {
                UnityModManagerNet.UnityModManager.Logger.Log("Heelflip");
                return;
            }
            UnityModManagerNet.UnityModManager.Logger.Log("Kickflip");
        }
    }
}
