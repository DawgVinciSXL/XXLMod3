using HarmonyLib;
using System;
using UnityEngine;
using XXLMod3.Controller;
using XXLMod3.Core;

namespace XXLMod3.Patches.PlayerController_
{
    [HarmonyPatch(typeof(PlayerController), "OnPop", new Type[] { typeof(float), typeof(float) })]
    class OnPopFFPatch
    {
        static bool Prefix(float p_pop, float p_scoop, ref float ____trajectoryToUp)
        {
            if (Main.enabled)
            {
                PlayerController.Instance.VelocityOnPop = PlayerController.Instance.boardController.boardRigidbody.velocity;
                PlayerController.Instance.SetTurningMode(InputController.TurningMode.InAir);
                PlayerController.Instance.SetSkaterToMaster();
                Vector3 vector = PlayerController.Instance.skaterController.skaterTransform.up * p_pop;
                Vector3 vector2 = PlayerController.Instance.skaterController.skaterRigidbody.velocity + vector;
                ____trajectoryToUp = Vector3.Angle(vector2, Vector3.up);
                Vector3.Angle(PlayerController.Instance.VelocityOnPop, vector2);
                Vector3.Angle(PlayerController.Instance.VelocityOnPop, Vector3.up);
                Vector3 vector3 = Vector3.ProjectOnPlane(PlayerController.Instance.skaterController.skaterRigidbody.velocity, Vector3.up);
                Vector3 vector4 = Vector3.ProjectOnPlane(vector3, PlayerController.Instance.boardController.LastGroundedBoardUp);
                Vector3 vector5 = Vector3.ProjectOnPlane(vector3, vector4);
                Vector3 lhs = Vector3.ProjectOnPlane(PlayerController.Instance.boardController.LastGroundedBoardUp, Vector3.up);
                if (Vector3.Dot(lhs, vector5) > 0f)
                {
                    vector5 *= 0.25f;
                }
                float d = Vector3.Dot(PlayerController.Instance.boardController.LastGroundedBoardUp, Vector3.up);
                vector5 = d * vector5;
                if (PlayerController.Instance.skaterController.skaterRigidbody.velocity.y > 0f)
                {
                    PlayerController.Instance.skaterController.skaterRigidbody.velocity = new Vector3(0f, PlayerController.Instance.skaterController.skaterRigidbody.velocity.y, 0f) + vector4 + vector5;
                }
                Vector3 vector6 = Vector3.ProjectOnPlane(vector, Vector3.up);
                Vector3 vector7 = Vector3.ProjectOnPlane(vector6, PlayerController.Instance.boardController.LastGroundedBoardUp);
                Vector3 vector8 = Vector3.ProjectOnPlane(vector6, vector7);
                if (Vector3.Dot(lhs, vector8) > 0f)
                {
                    vector8 *= 0.25f;
                }
                vector8 = d * vector8;
                if (vector.y > 0f)
                {
                    vector = new Vector3(0f, vector.y, 0f) + vector7 + vector8;
                }
                Vector3 force = PlayerController.Instance.skaterController.PredictLanding(vector);
                PlayerController.Instance.skaterController.skaterRigidbody.AddForce(vector, ForceMode.Impulse);
                PlayerController.Instance.skaterController.skaterRigidbody.AddForce(force, ForceMode.Impulse);

                if (!XXLController.Instance.IsLateFlip)
                {
                    SoundManager.Instance.PlayPopSound(p_scoop);
                }
                PlayerController.Instance.comController.popForce = vector;

                if (Main.settings.AdvancedPop == AdvancedPop.Trigger)
                {
                    Vector3 sideway = Vector3.ProjectOnPlane(PlayerController.Instance.skaterController.skaterTransform.right, PlayerController.Instance.skaterController.skaterTransform.up);
                    Vector3 forward = Vector3.ProjectOnPlane(PlayerController.Instance.skaterController.skaterTransform.forward, PlayerController.Instance.skaterController.skaterTransform.up);
                    if (PlayerController.Instance.inputController.player.GetButton("LT") && !PlayerController.Instance.inputController.player.GetButton("RT"))
                    {
                        if (!PlayerController.Instance.IsSwitch)
                        {
                            sideway = -sideway;
                        }
                    }
                    else if (PlayerController.Instance.inputController.player.GetButton("RT") && !PlayerController.Instance.inputController.player.GetButton("LT"))
                    {
                        if (PlayerController.Instance.IsSwitch)
                        {
                            sideway = -sideway;
                        }
                    }
                    else
                    {
                        sideway = Vector3.zero;
                    }
                    if (PlayerController.Instance.inputController.player.GetButton("RT") && PlayerController.Instance.inputController.player.GetButton("LT"))
                    {
                        if (PlayerController.Instance.IsSwitch)
                        {
                            forward = -forward;
                        }
                        PlayerController.Instance.skaterController.skaterRigidbody.AddForce(forward * Main.settings.ForwardPopForce * Time.deltaTime, ForceMode.VelocityChange);
                    }
                    PlayerController.Instance.skaterController.skaterRigidbody.AddForce(sideway * Main.settings.SidewayPopForce * Time.deltaTime, ForceMode.VelocityChange);
                }
                else if(Main.settings.AdvancedPop == AdvancedPop.Bumper || Main.settings.AdvancedPop == AdvancedPop.Sticks)
                {
                    if (XXLController.Instance.LeftPop)
                    {
                        XXLController.Instance.LeftPop = false;
                        Vector3 sideway = Vector3.ProjectOnPlane(PlayerController.Instance.skaterController.skaterTransform.right, PlayerController.Instance.skaterController.skaterTransform.up);
                        if (!PlayerController.Instance.IsSwitch)
                        {
                            sideway = -sideway;
                        }
                        PlayerController.Instance.skaterController.skaterRigidbody.AddForce(sideway * Main.settings.SidewayPopForce * Time.deltaTime, ForceMode.VelocityChange);
                    }
                    else if (XXLController.Instance.RightPop)
                    {
                        XXLController.Instance.RightPop = false;
                        Vector3 sideway = Vector3.ProjectOnPlane(PlayerController.Instance.skaterController.skaterTransform.right, PlayerController.Instance.skaterController.skaterTransform.up);
                        if (PlayerController.Instance.IsSwitch)
                        {
                            sideway = -sideway;
                        }
                        PlayerController.Instance.skaterController.skaterRigidbody.AddForce(sideway * Main.settings.SidewayPopForce * Time.deltaTime, ForceMode.VelocityChange);
                    }

                    if (PlayerController.Instance.inputController.player.GetButton("RT") && PlayerController.Instance.inputController.player.GetButton("LT"))
                    {
                        Vector3 forward = Vector3.ProjectOnPlane(PlayerController.Instance.skaterController.skaterTransform.forward, PlayerController.Instance.skaterController.skaterTransform.up);

                        if (PlayerController.Instance.IsSwitch)
                        {
                            forward = -forward;
                        }
                        PlayerController.Instance.skaterController.skaterRigidbody.AddForce(forward * Main.settings.ForwardPopForce * Time.deltaTime, ForceMode.VelocityChange);
                    }
                }
                return false;
            }
            return true;
        }
    }
}
