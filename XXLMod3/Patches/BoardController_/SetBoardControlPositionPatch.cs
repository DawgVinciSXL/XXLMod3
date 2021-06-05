using HarmonyLib;
using System;
using UnityEngine;
using XXLMod3.Controller;

namespace XXLMod3.Patches.BoardController_
{
    [HarmonyPatch(typeof(BoardController), "SetBoardControlPosition")]
    class SetBoardControlPositionPatch
    {
        static bool Prefix(BoardController __instance)
        {
            if (Main.enabled)
            {
                if (PlayerController.Instance.currentStateEnum == PlayerController.CurrentState.Grabs)
                {
                    if (XXLController.Instance.IsFootplant)
                    {
                        __instance.boardControlTransform.position = PlayerController.Instance.skaterController.skaterTargetTransform.position + new Vector3(0f, -Main.settings.FootplantBoardOffset, 0f);
                        return false;
                    }
                    __instance.boardControlTransform.position = PlayerController.Instance.skaterController.skaterTargetTransform.position + new Vector3(0f, -Main.settings.GrabBoardOffset, 0f);
                    return false;
                }
                if (PlayerController.Instance.currentStateEnum == PlayerController.CurrentState.Pop && XXLController.Instance.FlipDetected)
                {
                    if (XXLController.Instance.IsLateFlip)
                    {
                        __instance.boardControlTransform.position = PlayerController.Instance.skaterController.skaterTargetTransform.position + new Vector3(0f, -Main.settings.LateFlipBoardOffset, 0f);
                        return false;
                    }
                    else if (XXLController.Instance.IsPrimoFlip)
                    {
                        __instance.boardControlTransform.position = PlayerController.Instance.skaterController.skaterTargetTransform.position + new Vector3(0f, -Main.settings.PrimoBoardOffset, 0f);
                        return false;
                    }
                    __instance.boardControlTransform.position = PlayerController.Instance.skaterController.skaterTargetTransform.position + new Vector3(0f, -Main.settings.FlipBoardOffset, 0f);
                    return false;
                }
                if (PlayerController.Instance.currentStateEnum == PlayerController.CurrentState.Release && !XXLController.Instance.FlipDetected)
                {
                    __instance.boardControlTransform.position = PlayerController.Instance.skaterController.skaterTargetTransform.position + new Vector3(0f, Mathf.MoveTowards(-Main.settings.FlipBoardOffset, 0f, 5f), 0f);
                    return false;
                }
                return true;
            }
            return true;
        }
    }
}