using FSMHelper;
using SkaterXL.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using XXLMod3.Controller;
using XXLMod3.Core;

namespace XXLMod3.PlayerStates
{
    public class Custom_Grabs : PlayerState_OnBoard
    {
        private bool _canExitGrab;
        private bool enableIk;

        private float _grabMaxTime = 0.2f;
        private float _grabTimer;
        private float ikMaxTime = 0.2f;
        private float ikTimer;
        private float _inAirTurnDelta;
        private float leftIk;
        private float rightIk;

        private float DelayTimer;

        private string[] grabNames = new string[] { "Nose Grab", "Indy Grab", "Tail Grab", "Melon Grab", "Mute Grab", "Stalefish"};

        private enum GrabSide
        {
            Left,
            Right,
            Both
        }

        private GrabSide grab;

        private GrabType grabType;

        public override void SetupDefinition(ref FSMStateType stateType, ref List<Type> children)
        {
            stateType = FSMStateType.Type_OR;
        }

        public Custom_Grabs()
        {
        }

        public Custom_Grabs(bool p_leftGrab, float p_inAirTurnDelta)
        {
            _inAirTurnDelta = p_inAirTurnDelta;
            grab = (p_leftGrab ? GrabSide.Left : GrabSide.Right);
        }

        public override void Enter()
        {
            PlayerController.Instance.currentStateEnum = PlayerController.CurrentState.Grabs;
            XXLController.CurrentState = CurrentState.Grabs;
            PlayerController.Instance.ToggleFlipColliders(false);
            grabType = DetermineGrab();
            EventManager.Instance.StartGrab(grabType);
            PlayerController.Instance.DisableArmPhysics();
            PlayerController.Instance.SetHandIKTarget(grabType);
            PlayerController.Instance.CorrectHandIKRotation(PlayerController.Instance.GetBoardBackwards());
            PlayerController.Instance.SetIKOnOff(1f);
            PlayerController.Instance.ResetIKOffsets();
            XXLController.Instance.ActivateSlowMotion(Main.settings.SlowMotionGrabs, Main.settings.SlowMotionGrabSpeed);
            if (!Main.settings.GrabDelay)
            {
                PlayerController.Instance.animationController.ScaleAnimSpeed(4f);
                return;
            }
            PlayerController.Instance.animationController.ScaleAnimSpeed(1f);
        }

        public override void Exit()
        {
            XXLController.Instance.ResetTime(Main.settings.SlowMotionGrabs);
            if (isFootplanting)
            {
                PlayerController.Instance.skaterController.skaterRigidbody.useGravity = true;
                PlayerController.Instance.boardController.boardRigidbody.AddForce(PlayerController.Instance.skaterController.skaterTransform.up * Main.settings.FootplantJumpForce * 1.5f, ForceMode.Impulse);
                PlayerController.Instance.skaterController.skaterRigidbody.AddForce(PlayerController.Instance.skaterController.skaterTransform.up * Main.settings.FootplantJumpForce * 1.5f, ForceMode.Impulse);
                PlayerController.Instance.skaterController.skaterRigidbody.AddForce(PlayerController.Instance.skaterController.skaterTransform.forward * Main.settings.FootplantForwardForce * 1.5f, ForceMode.Impulse);
                isFootplanting = false;
            }
            EventManager.Instance.ExitGrab();
            PlayerController.Instance.animationController.ScaleAnimSpeed(1f);
            PlayerController.Instance.EnableArmPhysics();
            PlayerController.Instance.SetHandIKWeight(0f, 0f);
            PlayerController.Instance.AnimSetGrabToeside(false);
            PlayerController.Instance.AnimSetGrabHeelside(false);
            PlayerController.Instance.AnimSetGrabNose(false);
            PlayerController.Instance.AnimSetGrabTail(false);
            PlayerController.Instance.AnimSetGrabStale(false);
            PlayerController.Instance.AnimSetGrabMute(false);
        }

        public override void Update()
        {
            base.Update();
            if (!Main.settings.GrabDelay)
            {
                DelayTimer += Time.deltaTime;
                if(DelayTimer > 0.3f)
                {
                    PlayerController.Instance.animationController.ScaleAnimSpeed(1f);
                    DelayTimer = 0f;
                }
            }
            if(isLeftFootplant || isRightFootplant)
            {
                isFootplanting = true;
            }
            else
            {
                isFootplanting = false;
            }

            XXLController.Instance.IsFootplant = isFootplanting;
            PlayerController.Instance.DisableArmPhysics();
            HandleFootplant(); /////////////
            SoundManager.Instance.SetRollingVolumeFromRPS(PlayerController.Instance.GetSurfaceTag(PlayerController.Instance.boardController.GetSurfaceTagString()), PlayerController.Instance.boardController._rollSoundSpeed);
            if (!_canExitGrab)
            {
                _grabTimer += Time.deltaTime;
                if (_grabTimer >= _grabMaxTime)
                {
                    _canExitGrab = true;
                }
            }
            if (grab == GrabSide.Left)
            {
                leftIk = Mathf.MoveTowards(leftIk, 1f, Time.deltaTime * 3f);
            }
            else
            {
                rightIk = Mathf.MoveTowards(rightIk, 1f, Time.deltaTime * 3f);
            }
            PlayerController.Instance.SetHandIKWeight((grab == Custom_Grabs.GrabSide.Left) ? leftIk : 0f, (grab == Custom_Grabs.GrabSide.Right) ? rightIk : 0f);
            DoStance();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!isFootplanting)
            {
                PlayerController.Instance.comController.UpdateCOM();
            }
            PlayerController.Instance.ScalePlayerCollider();
            PlayerController.Instance.boardController.SetBoardControllerUpVector(PlayerController.Instance.skaterController.skaterTransform.up);
            Vector3 vector = SkaterXL.Core.Mathd.LocalAngularVelocity(PlayerController.Instance.skaterController.skaterRigidbody);
            _inAirTurnDelta += 57.29578f * vector.y * Time.deltaTime;
            PlayerController.Instance.SetRotationTarget(true);
            PlayerController.Instance.SnapRotation();
        }

        private GrabType DetermineGrab()
        {
            GrabType result = GrabType.Indy;
            GrabSide grabSide = grab;
            if (grabSide != Custom_Grabs.GrabSide.Left)
            {
                if (grabSide == GrabSide.Right)
                {
                    if (CanGrabNoseOrTail())
                    {
                        if (SettingsManager.Instance.stance == Stance.Regular)
                        {
                            PlayerController.Instance.AnimSetGrabTail(true);
                            result = GrabType.TailGrab;
                        }
                        else
                        {
                            PlayerController.Instance.AnimSetGrabNose(true);
                            result = GrabType.NoseGrab;
                        }
                    }
                    else if (CanStaleOrMute())
                    {
                        if (SettingsManager.Instance.stance == Stance.Regular)
                        {
                            PlayerController.Instance.AnimSetGrabStale(true);
                            result = GrabType.Stalefish;
                        }
                        else
                        {
                            PlayerController.Instance.AnimSetGrabMute(true);
                            result = GrabType.Mute;
                        }
                    }
                    else if (SettingsManager.Instance.stance == Stance.Regular)
                    {
                        PlayerController.Instance.AnimSetGrabToeside(true);
                        result = GrabType.Indy;
                    }
                    else
                    {
                        PlayerController.Instance.AnimSetGrabHeelside(true);
                        result = GrabType.Melon;
                    }
                }
            }
            else if (CanGrabNoseOrTail())
            {
                if (SettingsManager.Instance.stance == Stance.Regular)
                {
                    PlayerController.Instance.AnimSetGrabNose(true);
                    result = GrabType.NoseGrab;
                }
                else
                {
                    PlayerController.Instance.AnimSetGrabTail(true);
                    result = GrabType.TailGrab;
                }
            }
            else if (CanStaleOrMute())
            {
                if (SettingsManager.Instance.stance == Stance.Regular)
                {
                    PlayerController.Instance.AnimSetGrabMute(true);
                    result = GrabType.Mute;
                }
                else
                {
                    PlayerController.Instance.AnimSetGrabStale(true);
                    result = GrabType.Stalefish;
                }
            }
            else if (SettingsManager.Instance.stance == Stance.Regular)
            {
                PlayerController.Instance.AnimSetGrabHeelside(true);
                result = GrabType.Melon;
            }
            else
            {
                PlayerController.Instance.AnimSetGrabToeside(true);
                result = GrabType.Indy;
            }
            return result;
        }

        private bool CanGrabNoseOrTail()
        {
            return (PlayerController.Instance.GetLeftForwardAxis() + PlayerController.Instance.GetRightForwardAxis() < -0.3f || PlayerController.Instance.GetLeftForwardAxis() + PlayerController.Instance.GetRightForwardAxis() > 0.3f) && Mathf.Abs(PlayerController.Instance.GetLeftToeAxis()) < 0.4f && Mathf.Abs(PlayerController.Instance.GetRightToeAxis()) < 0.4f;
        }

        private bool CanStaleOrMute()
        {
            bool result = false;
            switch (SettingsManager.Instance.controlType)
            {
                case ControlType.Same:
                    if (SettingsManager.Instance.stance == Stance.Regular)
                    {
                        if (PlayerController.Instance.GetLeftToeAxis() < -0.5f && PlayerController.Instance.GetRightToeAxis() > 0.5f)
                        {
                            result = true;
                        }
                    }
                    else if (PlayerController.Instance.GetLeftToeAxis() > 0.5f && PlayerController.Instance.GetRightToeAxis() < -0.5f)
                    {
                        result = true;
                    }
                    break;
                case ControlType.Swap:
                    if (!PlayerController.Instance.IsSwitch)
                    {
                        if (SettingsManager.Instance.stance == Stance.Regular)
                        {
                            if (PlayerController.Instance.GetLeftToeAxis() < -0.5f && PlayerController.Instance.GetRightToeAxis() > 0.5f)
                            {
                                result = true;
                            }
                        }
                        else if (PlayerController.Instance.GetLeftToeAxis() > 0.5f && PlayerController.Instance.GetRightToeAxis() < -0.5f)
                        {
                            result = true;
                        }
                    }
                    else if (SettingsManager.Instance.stance == Stance.Regular)
                    {
                        if (PlayerController.Instance.GetLeftToeAxis() > 0.5f && PlayerController.Instance.GetRightToeAxis() < -0.5f)
                        {
                            result = true;
                        }
                    }
                    else if (PlayerController.Instance.GetLeftToeAxis() < -0.5f && PlayerController.Instance.GetRightToeAxis() > 0.5f)
                    {
                        result = true;
                    }
                    break;
                case ControlType.Simple:
                    if (!PlayerController.Instance.IsSwitch)
                    {
                        if (SettingsManager.Instance.stance == Stance.Regular)
                        {
                            if (PlayerController.Instance.GetLeftToeAxis() < -0.5f && PlayerController.Instance.GetRightToeAxis() > 0.5f)
                            {
                                result = true;
                            }
                        }
                        else if (PlayerController.Instance.GetLeftToeAxis() > 0.5f && PlayerController.Instance.GetRightToeAxis() < -0.5f)
                        {
                            result = true;
                        }
                    }
                    else if (SettingsManager.Instance.stance == Stance.Regular)
                    {
                        if (PlayerController.Instance.GetLeftToeAxis() > 0.5f && PlayerController.Instance.GetRightToeAxis() < -0.5f)
                        {
                            result = true;
                        }
                    }
                    else if (PlayerController.Instance.GetLeftToeAxis() < -0.5f && PlayerController.Instance.GetRightToeAxis() > 0.5f)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        public override void OnPredictedCollisionEvent()
        {
            object[] args = new object[]
            {
            2,
            _inAirTurnDelta
            };
            base.DoTransition(typeof(Custom_InAir), args);
        }

        public override void OnCollisionEnterEvent(Vector3 _impulse, bool _isBoard, Collision c)
        {
            if (_isBoard)
            {
                SoundManager.Instance.PlayBoardHit(_impulse.magnitude);
            }
            if (!_canExitGrab && !Main.settings.GrabFix)
            {
                return;
            }
            object[] args = new object[]
            {
            2,
            _inAirTurnDelta
            };
            base.DoTransition(typeof(Custom_InAir), args);
        }

        public override void OnCollisionStayEvent(Collision c)
        {
            if (!_canExitGrab && !Main.settings.GrabFix)
            {
                return;
            }
            object[] args = new object[]
            {
            2,
            _inAirTurnDelta
            };
            base.DoTransition(typeof(Custom_InAir), args);
        }

        public override void OnLBUp()
        {
            if (grab == GrabSide.Left)
            {
                object[] args = new object[]
                {
                2,
                _inAirTurnDelta
                };
                base.DoTransition(typeof(Custom_InAir), args);
            }
        }

        public override void OnRBUp()
        {
            if (grab == GrabSide.Right)
            {
                object[] args = new object[]
                {
                2,
                _inAirTurnDelta
                };
                base.DoTransition(typeof(Custom_InAir), args);
            }
        }

        public override bool IsInGrabState()
        {
            return true;
        }

        public override void OnRespawn()
        {
            base.OnRespawn();
        }

        public override void OnPlayerTriggerEnterEvent(Collider c)
        {
            PlayerController.Instance.ForceBail();
        }

        public override void OnPlayerTriggerStayEvent(Collider c)
        {
            PlayerController.Instance.ForceBail();
        }

        bool isFootplanting;
        bool isRightFootplant;
        bool isLeftFootplant;

        LayerMask layerMask = ~(1 << LayerMask.NameToLayer("Skateboard"));

        Quaternion leftRot;
        Quaternion rightRot;

        Vector3 leftPos;
        Vector3 rightPos;

        private void HandleBodyflip()
        {
            switch (Main.settings.BodyflipMode)
            {
                case BodyflipMode.LS:
                    if (PlayerController.Instance.inputController.LeftStick.rawInput.pos.x >= 0.1f || PlayerController.Instance.inputController.LeftStick.rawInput.pos.x <= -0.1f || PlayerController.Instance.inputController.LeftStick.rawInput.pos.y <= -0.1f || PlayerController.Instance.inputController.LeftStick.rawInput.pos.y >= 0.1f)
                    {
                        if (PlayerController.Instance.movementMaster != PlayerController.MovementMaster.Target)
                        {
                            PlayerController.Instance.skaterController.skaterRigidbody.angularVelocity = new Vector3(0, 0, 0);
                            PlayerController.Instance.SetTargetToMaster();
                        }
                        PlayerController.Instance.boardController.currentRotationTarget = PlayerController.Instance.skaterController.skaterTransform.rotation;
                        if (PlayerController.Instance.inputController.LeftStick.rawInput.pos.x != 0)
                        {
                            PlayerController.Instance.skaterController.skaterTransform.Rotate(0, PlayerController.Instance.inputController.LeftStick.rawInput.pos.x * 5 * Time.deltaTime * Main.settings.BodyflipSpeed, 0, Space.Self);
                        }
                        if (PlayerController.Instance.inputController.LeftStick.rawInput.pos.y != 0)
                        {
                            PlayerController.Instance.skaterController.skaterTransform.Rotate(PlayerController.Instance.inputController.LeftStick.rawInput.pos.y * 5 * Time.deltaTime * Main.settings.BodyflipSpeed, 0, 0, Space.Self);
                        }
                        return;
                    }
                    break;
                case BodyflipMode.RS:
                    if (PlayerController.Instance.inputController.RightStick.rawInput.pos.x >= 0.1f || PlayerController.Instance.inputController.RightStick.rawInput.pos.x <= -0.1f || PlayerController.Instance.inputController.RightStick.rawInput.pos.y <= -0.1f || PlayerController.Instance.inputController.RightStick.rawInput.pos.y >= 0.1f)
                    {
                        if (PlayerController.Instance.movementMaster != PlayerController.MovementMaster.Target)
                        {
                            PlayerController.Instance.SetTargetToMaster();
                            PlayerController.Instance.skaterController.skaterRigidbody.angularVelocity = new Vector3(0, 0, 0);
                        }
                        PlayerController.Instance.boardController.currentRotationTarget = PlayerController.Instance.skaterController.skaterRigidbody.rotation;
                        if (PlayerController.Instance.inputController.RightStick.rawInput.pos.x != 0)
                        {
                            PlayerController.Instance.skaterController.skaterTransform.Rotate(0, -PlayerController.Instance.inputController.RightStick.rawInput.pos.x * 5 * Time.deltaTime * Main.settings.BodyflipSpeed, 0, Space.Self);
                        }
                        if (PlayerController.Instance.inputController.RightStick.rawInput.pos.y != 0)
                        {
                            PlayerController.Instance.skaterController.skaterTransform.Rotate(PlayerController.Instance.inputController.RightStick.rawInput.pos.y * 5 * Time.deltaTime * Main.settings.BodyflipSpeed, 0, 0, Space.Self);
                        }
                        return;
                    }
                    break;
                case BodyflipMode.Off:
                    break;
            }
        }

        private void HandleFootplant()
        {
            if (Main.settings.Footplants)
            {
                switch (Main.settings.OneFootGrabMode)
                {
                    case OneFootGrabMode.Buttons:
                        if (PlayerController.Instance.inputController.player.GetButton("X"))
                        {
                            FootplantRaycast(true, false);
                        }
                        else if (PlayerController.Instance.inputController.player.GetButton("A"))
                        {
                            FootplantRaycast(false, true);
                        }
                        break;
                    case OneFootGrabMode.Sticks:
                        if (PlayerController.Instance.inputController.player.GetButton("Left Stick Button"))
                        {
                            FootplantRaycast(true, false);
                        }
                        else if (PlayerController.Instance.inputController.player.GetButton("Right Stick Button"))
                        {
                            FootplantRaycast(false, true);
                        }
                        break;
                    case OneFootGrabMode.Off:
                        break;
                }

                if (isLeftFootplant || isRightFootplant)
                {
                    switch (Main.settings.OneFootGrabMode)
                    {
                        case OneFootGrabMode.Buttons:
                            if (!PlayerController.Instance.inputController.player.GetButton("X") && !PlayerController.Instance.inputController.player.GetButton("A"))
                            {
                                isLeftFootplant = false;
                                isRightFootplant = false;
                                PlayerController.Instance.skaterController.skaterRigidbody.useGravity = true;
                                PlayerController.Instance.boardController.boardRigidbody.AddForce(PlayerController.Instance.skaterController.skaterTransform.up * Main.settings.FootplantJumpForce, ForceMode.Impulse);
                                PlayerController.Instance.skaterController.skaterRigidbody.AddForce(PlayerController.Instance.skaterController.skaterTransform.up * Main.settings.FootplantJumpForce, ForceMode.Impulse);
                                PlayerController.Instance.skaterController.skaterRigidbody.AddForce(PlayerController.Instance.skaterController.skaterTransform.forward * Main.settings.FootplantForwardForce, ForceMode.Impulse);
                            }
                            break;
                        case OneFootGrabMode.Sticks:
                            if (!PlayerController.Instance.inputController.player.GetButton("Right Stick Button") && !PlayerController.Instance.inputController.player.GetButton("Left Stick Button"))
                            {
                                isLeftFootplant = false;
                                isRightFootplant = false;
                                PlayerController.Instance.skaterController.skaterRigidbody.useGravity = true;
                                PlayerController.Instance.boardController.boardRigidbody.AddForce(PlayerController.Instance.skaterController.skaterTransform.up * Main.settings.FootplantJumpForce, ForceMode.Impulse);
                                PlayerController.Instance.skaterController.skaterRigidbody.AddForce(PlayerController.Instance.skaterController.skaterTransform.up * Main.settings.FootplantJumpForce, ForceMode.Impulse);
                                PlayerController.Instance.skaterController.skaterRigidbody.AddForce(PlayerController.Instance.skaterController.skaterTransform.forward * Main.settings.FootplantForwardForce, ForceMode.Impulse);
                            }
                            break;
                        case OneFootGrabMode.Off:
                            break;
                    }
                }
            }
        }

        private void FootplantRaycast(bool left, bool right)
        {
            if (left)
            {
                if (isLeftFootplant == false)
                {
                    PlayerController.Instance.skaterController.leftFootCollider.isTrigger = false;
                    RaycastHit hit;
                    if (Physics.Raycast(XXLController.LeftFoot.position, Vector3.down, out hit, 0.056f + 1f, layerMask))
                    {
                        if (hit.collider.gameObject.name != "Skater_foot_l" && hit.collider.gameObject.name != "Skater_foot_r" && hit.collider.gameObject.layer != LayerMask.NameToLayer("Skateboard") && hit.collider.gameObject.layer != LayerMask.NameToLayer("Character"))
                        {
                            Vector3 position = hit.point;
                            leftPos = position;
                            position.y += 0.025f;
                            var rot = Quaternion.FromToRotation(XXLController.LeftFoot.right, hit.normal) * XXLController.LeftFoot.rotation;
                            leftRot = rot;
                            XXLController.LeftFootPos = leftPos;
                            XXLController.LeftFootRot = leftRot;
                            float distance = Vector3.Distance(XXLController.LeftFoot.position, position);
                            if (distance <= 0.09f)
                            {
                                isLeftFootplant = true;
                                XXLController.LeftFootPos = leftPos;
                                XXLController.LeftFootRot = leftRot;
                                PlayerController.Instance.skaterController.skaterRigidbody.velocity = new Vector3(0, 0, 0);
                                PlayerController.Instance.skaterController.skaterRigidbody.useGravity = false;
                                return;
                            }
                            else
                            {
                                isLeftFootplant = false;
                            }
                        }
                    }
                }
            }

            if (right)
            {
                if (isRightFootplant == false)
                {
                    PlayerController.Instance.skaterController.rightFootCollider.isTrigger = false;
                    RaycastHit hit;
                    if (Physics.Raycast(XXLController.RightFoot.position, Vector3.down, out hit, 0.056f + 1f, layerMask))
                    {
                        if (hit.collider.gameObject.name != "Skater_foot_l" && hit.collider.gameObject.name != "Skater_foot_r" && hit.collider.gameObject.layer != LayerMask.NameToLayer("Skateboard") && hit.collider.gameObject.layer != LayerMask.NameToLayer("Character"))
                        {
                            Vector3 position = hit.point;
                            position.y += 0.025f;
                            rightPos = position;
                            var rot = Quaternion.FromToRotation(XXLController.RightFoot.right, hit.normal) * XXLController.RightFoot.rotation;
                            rightRot = rot;
                            XXLController.RightFootPos = rightPos;
                            XXLController.RightFootRot = rightRot;
                            float distance = Vector3.Distance(XXLController.RightFoot.position, position);
                            if (distance <= 0.12f)
                            {
                                isRightFootplant = true;
                                XXLController.RightFootPos = rightPos;
                                XXLController.RightFootRot = rightRot;
                                PlayerController.Instance.skaterController.skaterRigidbody.velocity = new Vector3(0, 0, 0);
                                PlayerController.Instance.skaterController.skaterRigidbody.useGravity = false;
                                return;
                            }
                            else
                            {
                                isRightFootplant = false;
                            }
                        }
                    }
                }
            }
        }

        private void DoStance()
        {
            switch (grabType)
            {
                case GrabType.Indy:
                    DoStance(StanceController.Instance.IndyFeet, Main.settings.UseSimpleOnButtonGrabs ? StanceController.Instance.GrabsOnButtonSimpleFeet : StanceController.Instance.IndyOffBoardFeet);
                    break;
                case GrabType.Melon:
                    DoStance(StanceController.Instance.MelonFeet, Main.settings.UseSimpleOnButtonGrabs ? StanceController.Instance.GrabsOnButtonSimpleFeet : StanceController.Instance.MelonOffBoardFeet);
                    break;
                case GrabType.Mute:
                    DoStance(StanceController.Instance.MuteFeet, Main.settings.UseSimpleOnButtonGrabs ? StanceController.Instance.GrabsOnButtonSimpleFeet : StanceController.Instance.MuteOffBoardFeet);
                    break;
                case GrabType.NoseGrab:
                    DoStance(StanceController.Instance.NosegrabFeet, Main.settings.UseSimpleOnButtonGrabs ? StanceController.Instance.GrabsOnButtonSimpleFeet : StanceController.Instance.NosegrabOffBoardFeet);
                    break;
                case GrabType.Stalefish:
                    DoStance(StanceController.Instance.StalefishFeet, Main.settings.UseSimpleOnButtonGrabs ? StanceController.Instance.GrabsOnButtonSimpleFeet : StanceController.Instance.StalefishOffBoardFeet);
                    break;
                case GrabType.TailGrab:
                    DoStance(StanceController.Instance.TailgrabFeet, Main.settings.UseSimpleOnButtonGrabs ? StanceController.Instance.GrabsOnButtonSimpleFeet : StanceController.Instance.TailgrabOffBoardFeet);
                    break;
            }
        }

        private void DoStance(CustomFeetObject Stance, CustomFeetObject OffBoardStance)
        {
            switch (Main.settings.OneFootGrabMode)
            {
                case OneFootGrabMode.Sticks:
                    if (PlayerController.Instance.inputController.player.GetButton("Left Stick Button"))
                    {
                        if (!isLeftFootplant)
                        {
                            StanceController.Instance.SetFreeFootMovementLeft(false, true);
                            StanceController.Instance.DoLeftFootTransition(OffBoardStance);
                            return;
                        }
                        StanceController.Instance.DoLeftFootplant();
                    }
                    else
                    {
                        StanceController.Instance.SetFreeFootMovementLeft(true, false);
                        StanceController.Instance.DoLeftFootTransition(Stance);
                    }
                    if (PlayerController.Instance.inputController.player.GetButton("Right Stick Button"))
                    {
                        if (!isRightFootplant)
                        {
                            StanceController.Instance.SetFreeFootMovementRight(false, true);
                            StanceController.Instance.DoRightFootTransition(OffBoardStance);
                            return;
                        }
                        StanceController.Instance.DoRightFootplant();
                    }
                    else
                    {
                        StanceController.Instance.SetFreeFootMovementRight(true, false);
                        StanceController.Instance.DoRightFootTransition(Stance);
                    }
                    break;
                case OneFootGrabMode.Buttons:
                    if (PlayerController.Instance.inputController.player.GetButton("X"))
                    {
                        if (!isLeftFootplant)
                        {
                            StanceController.Instance.SetFreeFootMovementLeft(false, true);
                            StanceController.Instance.DoLeftFootTransition(OffBoardStance);
                            return;
                        }
                        StanceController.Instance.DoLeftFootplant();
                    }
                    else
                    {
                        StanceController.Instance.SetFreeFootMovementLeft(true, false);
                        StanceController.Instance.DoLeftFootTransition(Stance);
                    }
                    if (PlayerController.Instance.inputController.player.GetButton("A"))
                    {
                        if (!isRightFootplant)
                        {
                            StanceController.Instance.SetFreeFootMovementRight(false, true);
                            StanceController.Instance.DoRightFootTransition(OffBoardStance);
                            return;
                        }
                        StanceController.Instance.DoRightFootplant();
                    }
                    else
                    {
                        StanceController.Instance.SetFreeFootMovementRight(true, false);
                        StanceController.Instance.DoRightFootTransition(Stance);
                    }
                    break;
                case OneFootGrabMode.Off:
                    StanceController.Instance.SetFreeFootMovementLeft(true, false);
                    StanceController.Instance.DoLeftFootTransition(Stance);
                    StanceController.Instance.SetFreeFootMovementRight(true, false);
                    StanceController.Instance.DoRightFootTransition(Stance);
                    break;
            }
        }
    }
}