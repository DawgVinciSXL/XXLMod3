using System;
using HarmonyLib;
using UnityEngine;
using XXLMod3.Controller;

namespace XXLMod3.Patches.BoardController_
{
    [HarmonyPatch(typeof(BoardController), "Rotate", new Type[] { typeof(bool), typeof(bool) })]
    class RotatePatch
    {
        public static bool Prefix(ref float ____firstDelta, ref float ____secondDelta, ref float ____thirdDelta, ref float ____flipDelta, ref float ____spinDelta, ref Quaternion ____rotDeltaThisFrame, ref Quaternion ____bufferedRotation, ref Transform ____catchForwardRotation, ref float ____bufferedFlip, ref bool doFlip, ref bool doPop, ref float ____lastuplift)
        {
            if (Main.enabled)
            {
                ____firstDelta = PlayerController.Instance.boardController.firstVel * 500f * Time.deltaTime;
                ____secondDelta = PlayerController.Instance.boardController.secondVel * 20f * Time.deltaTime;
                ____thirdDelta = PlayerController.Instance.boardController.thirdVel * 20f * Time.deltaTime;
                bool flag2 = SettingsManager.Instance.stance == SkaterXL.Core.Stance.Regular;
                if (flag2)
                {
                    ____secondDelta = -____secondDelta;
                }
                bool flag3 = SettingsManager.Instance.controlType == SkaterXL.Core.ControlType.Simple && PlayerController.Instance.IsSwitch;
                if (flag3)
                {
                    ____secondDelta = -____secondDelta;
                }
                ____firstDelta = Mathf.Clamp(____firstDelta, -5f, 5f);
                ____secondDelta = Mathf.Clamp(____secondDelta, -6f, 6f);
                ____thirdDelta = Mathf.Clamp(doFlip ? ____thirdDelta : (____thirdDelta * 0f), -9f, 9f);
                Vector3 vector = SkaterXL.Core.Mathd.LocalAngularVelocity(PlayerController.Instance.skaterController.skaterRigidbody);

                float FlipSpeed = GetFlipSpeed();
                float ScoopSpeed = GetScoopSpeed();

                float num = 0f;
                if (Main.settings.DecoupledMode == Core.DecoupledMode.Simple)
                {
                    num = (Mathf.Abs(____secondDelta) < 1) ? (57.29578f * vector.y * Time.deltaTime) : 0f;
                }
                else if ((Main.settings.DecoupledMode == Core.DecoupledMode.Total || Main.settings.DecoupledMode == Core.DecoupledMode.Hardcore))
                {
                    num = 0f;
                }
                else
                {
                    num = 57.29578f * vector.y * Time.deltaTime;
                }

                ____rotDeltaThisFrame = Quaternion.Euler(XXLController.Instance.IsPrimoFlip ? -____firstDelta : ____firstDelta, (XXLController.Instance.IsLateFlip && PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie") == 0 ? ____secondDelta * (ScoopSpeed) : -____secondDelta * (ScoopSpeed)), 0f);
                Quaternion lhs = Quaternion.AngleAxis(num, PlayerController.Instance.skaterController.skaterTransform.up);
                ____bufferedRotation = lhs * ____bufferedRotation * ____rotDeltaThisFrame;
                ____catchForwardRotation.Rotate(PlayerController.Instance.skaterController.transform.up, num);
                PlayerController.Instance.boardController.boardTransform.rotation = ____bufferedRotation;
                ____bufferedFlip += ____thirdDelta;
                PlayerController.Instance.boardController.boardTransform.Rotate(new Vector3(0f, 0f, (____bufferedFlip * (FlipSpeed))));
                bool flag5 = doPop;
                if (flag5)
                {
                    float d = PlayerController.Instance.boardController.popBoardVelAdd * Mathf.Abs(Mathf.Atan(0.0174532924f * ____firstDelta)) - ____lastuplift;
                    PlayerController.Instance.boardController.boardRigidbody.AddForce(PlayerController.Instance.skaterController.skaterTransform.up * d, ForceMode.VelocityChange);
                }
                return false;
            }
            return true;
        }

        public static float GetFlipSpeed()
        {
            if (XXLController.Instance.IsLateFlip)
            {
                return Main.settings.LateFlipSpeed;
            }
            else if (XXLController.Instance.IsPrimoFlip)
            {
                return Main.settings.PrimoFlipSpeed;
            }
            else if (XXLController.Instance.IsFingerFlip)
            {
                return Main.settings.FingerFlipSpeed;
            }
            return Main.settings.FlipSpeed;
        }

        public static float GetScoopSpeed()
        {
            if (XXLController.Instance.IsLateFlip)
            {
                return Main.settings.LateScoopSpeed;
            }
            else if (XXLController.Instance.IsPrimoFlip)
            {
                return Main.settings.PrimoScoopSpeed;
            }
            else if (XXLController.Instance.IsFingerFlip)
            {
                return Main.settings.FingerScoopSpeed;
            }
            return Main.settings.ScoopSpeed;
        }
    }
}