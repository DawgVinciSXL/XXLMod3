using SkaterXL.Core;
using UnityEngine;
using XXLMod3.Controller;
using XXLMod3.Core;

namespace XXLMod3.PlayerStates
{
    public class Custom_Pop : PlayerState_OnBoard
    {
        private bool _canManual;
        private bool _catchRegistered;
        private bool _checkForCollisions;
        private bool _flipDetected;
        private bool _forwardLoad;
        private bool _isRespawning;
        private bool _leftCaught;
        private bool _leftCaughtFirst;
        private bool _leftOn = true;
        private bool _popRotationDone;
        private bool _potentialFlip;
        private bool _scooppFlipInputWindowDone;
        private bool _rightCaught;
        private bool _rightCaughtFirst;
        private bool _rightOn = true;
        private bool _transitionToInAir;
        private bool _wasGrinding;

        private float _augmentedLeftAngle;
        private float _augmentedRightAngle;
        private float _collisionTimer;
        private float _flip;
        private float _flipVel;
        private float _highTime = 0.416f;
        private float _inAirTransitionDuration = 0.19f;
        private float _inAirTurnDelta;
        private float _invertVel;
        private float _kickAddSoFar;
        private float _midTime = 0.35f;
        private float _popDir;
        private float _popRotationRemovalTimer;
        private float _popVel;
        private float _scoopFlipInputWindowTimer;
        private float _steezeTimer;
        private float _timeInState;
        private float _timer;
        private float _toeAxis;

        private int _flipFrameCount;
        private int _flipFrameMax = 25;

        private PlayerController.SetupDir _setupDir;

        private StickInput _flipStick;
        private StickInput _popStick;

        private Vector2 _initialFlipDir = Vector2.zero;

        public Custom_Pop(StickInput p_popStick, StickInput p_flipStick, bool p_forwardLoad, float p_invertVel, PlayerController.SetupDir p_setupDir, float p_augmentedLeftAngle, float p_augmentedRightAngle, float p_kickAddSoFar, float p_inAirTurnDelta)
        {
            _popStick = p_popStick;
            _flipStick = p_flipStick;
            _forwardLoad = p_forwardLoad;
            _setupDir = p_setupDir;
            _augmentedLeftAngle = p_augmentedLeftAngle;
            _augmentedRightAngle = p_augmentedRightAngle;
            _kickAddSoFar = p_kickAddSoFar;
            _inAirTurnDelta = p_inAirTurnDelta;
        }

        public Custom_Pop(StickInput p_popStick, StickInput p_flipStick, bool p_wasGrinding, bool p_forwardLoad, float p_invertVel, PlayerController.SetupDir p_setupDir, float p_augmentedLeftAngle, float p_augmentedRightAngle, float p_popVel, float p_toeAxis, float p_popDir, float p_kickAddSoFar, float p_inAirTurnDelta)
        {
            _popStick = p_popStick;
            _flipStick = p_flipStick;
            _wasGrinding = p_wasGrinding;
            _forwardLoad = p_forwardLoad;
            _invertVel = p_invertVel;
            _setupDir = p_setupDir;
            _augmentedLeftAngle = p_augmentedLeftAngle;
            _augmentedRightAngle = p_augmentedRightAngle;
            _popVel = p_popVel;
            _toeAxis = p_toeAxis;
            _popDir = p_popDir;
            _kickAddSoFar = p_kickAddSoFar;
            _inAirTurnDelta = p_inAirTurnDelta;
        }

        public Custom_Pop(StickInput p_popStick, StickInput p_flipStick, Vector2 p_initialFlipDir, float p_flipVel, float p_popVel, float p_toeAxis, float p_popDir, bool p_flipDetected, float p_flip, bool p_forwardLoad, float p_invertVel, PlayerController.SetupDir p_setupDir, float p_augmentedLeftAngle, float p_augmentedRightAngle, float p_kickAddSoFar, float p_inAirTurnDelta)
        {
            _popStick = p_popStick;
            _flipStick = p_flipStick;
            _potentialFlip = false;
            _flipDetected = p_flipDetected;
            _initialFlipDir = p_initialFlipDir;
            _toeAxis = p_toeAxis;
            _popDir = p_popDir;
            _flipVel = p_flipVel;
            _popVel = p_popVel;
            _flip = p_flip;
            _forwardLoad = p_forwardLoad;
            _invertVel = p_invertVel;
            _setupDir = p_setupDir;
            _augmentedLeftAngle = p_augmentedLeftAngle;
            _augmentedRightAngle = p_augmentedRightAngle;
            _kickAddSoFar = p_kickAddSoFar;
            _inAirTurnDelta = p_inAirTurnDelta;
        }

        public Custom_Pop(StickInput p_popStick, StickInput p_flipStick, Vector2 p_initialFlipDir, float p_flipVel, float p_popVel, float p_toeAxis, float p_popDir, bool p_flipDetected, float p_flip, bool p_wasGrinding, bool p_forwardLoad, float p_invertVel, PlayerController.SetupDir p_setupDir, float p_augmentedLeftAngle, float p_augmentedRightAngle, float p_kickAddSoFar, float p_inAirTurnDelta)
        {
            _popStick = p_popStick;
            _flipStick = p_flipStick;
            _potentialFlip = false;
            _flipDetected = p_flipDetected;
            _initialFlipDir = p_initialFlipDir;
            _toeAxis = p_toeAxis;
            _popDir = p_popDir;
            _flipVel = p_flipVel;
            _popVel = p_popVel;
            _flip = p_flip;
            _wasGrinding = p_wasGrinding;
            _forwardLoad = p_forwardLoad;
            _invertVel = p_invertVel;
            _setupDir = p_setupDir;
            _augmentedLeftAngle = p_augmentedLeftAngle;
            _augmentedRightAngle = p_augmentedRightAngle;
            _kickAddSoFar = p_kickAddSoFar;
            _inAirTurnDelta = p_inAirTurnDelta;
        }

        public override void Enter()
        {
            PlayerController.Instance.currentStateEnum = PlayerController.CurrentState.Pop;
            XXLController.CurrentState = CurrentState.Pop;
            PlayerController.Instance.cameraController.NeedToSlowLerpCamera = true;
            PlayerController.Instance.ToggleFlipTrigger(true);
            PlayerController.Instance.BoardFreezedAfterRespawn = false;
            float num = PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie");
            if (num > 0f)
            {
                num = 2f;
            }
            else
            {
                num = -2f;
            }
            //PlayerController.Instance.ForcePivotForwardRotation(num);
            PlayerController.Instance.AnimSetGrinding(false);
            if (PlayerController.Instance.inputController.turningMode != InputController.TurningMode.FastLeft && PlayerController.Instance.inputController.turningMode != InputController.TurningMode.FastRight)
            {
                PlayerController.Instance.SetTurningMode(InputController.TurningMode.InAir);
            }
        }

        public override void Exit()
        {
            PlayerController.Instance.cameraController.NeedToSlowLerpCamera = false;
            PlayerController.Instance.ToggleFlipTrigger(false);
        }

        float canGrabTimer;
        bool canGrab;

        public override void Update()
        {
            if (_isRespawning)
            {
                return;
            }
            if (PlayerController.Instance.GetAnimReleased())
            {
                XXLController.Instance.FlipDetected = true;
            }

            if (Main.settings.GrabFix)
            {
                if (!canGrab)
                {
                    canGrabTimer += Time.deltaTime;
                    if (canGrabTimer >= 0.15f)
                    {
                        canGrab = true;
                        canGrabTimer = 0f;
                    }
                }
            }
            else
            {
                canGrab = true;
            }

            HandleBoardControllerUpVector(); //////////
            HandleHippieOllie(); ///////////
            XXLController.Instance.VerticalFlip();
            _timeInState += Time.deltaTime;
            SoundManager.Instance.SetRollingVolumeFromRPS(PlayerController.Instance.GetSurfaceTag(PlayerController.Instance.boardController.GetSurfaceTagString()), PlayerController.Instance.boardController._rollSoundSpeed);
            bool popRotationDone = _popRotationDone;
            if (_rightOn && _leftOn)
            {
                PlayerController.Instance.LerpKneeIkWeight();
            }
            else
            {
                PlayerController.Instance.LerpLeftKneeIkWeight();
                PlayerController.Instance.LerpRightKneeIkWeight();
            }
            _steezeTimer += Time.deltaTime;
            if (_steezeTimer > 0.08f && PlayerController.Instance.GetAnimReleased())
            {
                SetPopSteezeWeight();
            }
            if (Mathf.Abs(PlayerController.Instance.boardController.secondVel) > 5f)
            {
                PlayerController.Instance.SetIKLerpSpeed(8f);
            }
            HandleLaidback(); //////////////
            if (!_transitionToInAir && PlayerController.Instance.IsCurrentAnimationPlaying("Extend"))
            {
                PlayerController.Instance.OnExtendAnimEnter();
            }
        }

        private void SetPopSteezeWeight()
        {
            if (_popStick.IsRightStick)
            {
                PlayerController.Instance.SetMaxSteezeRight(1f);
                return;
            }
            PlayerController.Instance.SetMaxSteezeLeft(1f);
        }

        private void KickAdd()
        {
            float num = 15f;
            float num2 = Mathf.Clamp(Mathf.Abs(_popVel) / num, -0.7f, 0.7f);
            float num3 = 1.1f;
            if (_wasGrinding)
            {
                num3 *= 0.5f;
            }
            float num4 = num3 - num3 * num2 - _kickAddSoFar;
            _kickAddSoFar += num4;
            PlayerController.Instance.DoKick(_forwardLoad, num4);
        }

        private void KillPopRotation()
        {
            float num = 15f;
            float num2 = Mathf.Clamp(Mathf.Abs(Mathf.Abs(_popVel)) / num, -1f, 1f);
            float num3 = 1.25f;
            if (_wasGrinding)
            {
                num3 *= 0.5f;
            }
            float strength = num3 - num3 * num2;
            PlayerController.Instance.DoKick(!_forwardLoad, strength);
            _popRotationDone = true;
        }

        private void AddForwardVel()
        {
            float num = 15f;
            float num2 = Mathf.Clamp(Mathf.Abs(_popVel) / num, -1f, 1f);
            float p_value = (1f - num2) * _invertVel;
            PlayerController.Instance.AddForwardSpeed(p_value);
        }

        public override void FixedUpdate()
        {
            if (_isRespawning)
            {
                return;
            }
            base.FixedUpdate();
            PlayerController.Instance.ScalePlayerCollider();
            Vector3 vector = SkaterXL.Core.Mathd.LocalAngularVelocity(PlayerController.Instance.skaterController.skaterRigidbody);
            _inAirTurnDelta += 57.29578f * vector.y * Time.deltaTime;
            PlayerController.Instance.comController.UpdateCOM();
            PlayerController.Instance.SetRotationTarget();
            _scoopFlipInputWindowTimer += Time.deltaTime;
            _popRotationRemovalTimer += Time.deltaTime;
            if (!_popRotationDone)
            {
                KickAdd();
                if (_popRotationRemovalTimer >= 0.04f)
                {
                    KillPopRotation();
                }
            }
            if (Main.settings.DecoupledMode == DecoupledMode.Hardcore)
            {
                if (!_flipDetected)
                {
                    if (!InputController.Instance.player.GetButton("Left Stick Button") && !InputController.Instance.player.GetButton("Right Stick Button"))
                    {
                        XXLController.Instance.IsInHardcorePop = true;
                        PlayerController.Instance.SetRightIKLerpTarget(1f, 1f);
                        PlayerController.Instance.SetRightSteezeWeight(0f);
                        PlayerController.Instance.SetMaxSteezeRight(0f);
                        PlayerController.Instance.SetRightKneeIKTargetWeight(1f);
                        PlayerController.Instance.SetRightIKWeight(1f);
                        PlayerController.Instance.SetRightKneeBendWeight(1f);
                        PlayerController.Instance.SetRightKneeBendWeightManually(1f);
                        //LeftLegIK
                        PlayerController.Instance.SetLeftIKLerpTarget(1f, 1f);
                        PlayerController.Instance.SetLeftSteezeWeight(0f);
                        PlayerController.Instance.SetMaxSteezeLeft(0f);
                        PlayerController.Instance.SetLeftKneeIKTargetWeight(1f);
                        PlayerController.Instance.SetLeftIKWeight(1f);
                        PlayerController.Instance.SetLeftKneeBendWeight(1f);
                        PlayerController.Instance.SetLeftKneeBendWeightManually(1f);
                        PlayerController.Instance.animationController.skaterAnim.SetBool("Released", true);
                        PlayerController.Instance.CrossFadeAnimation("Extend", 0.4f);
                        PlayerController.Instance.scoopFlipWindowNoFlipDetected = 0.075f;
                    }
                    else
                    {
                        PlayerController.Instance.animationController.skaterAnim.SetBool("Released", false);
                    }
                }
            }
            if (!_scooppFlipInputWindowDone && _scoopFlipInputWindowTimer >= PlayerController.Instance.scoopFlipWindowNoFlipDetected + (_flipDetected ? 0.075f : 0f))
            {
                _scooppFlipInputWindowDone = true;
                AddForwardVel();
                if (_flipDetected || PlayerController.Instance.animationController.skaterAnim.GetBool("Released"))
                {
                    if (!_popRotationDone)
                    {
                        KillPopRotation();
                    }
                    PlayerController.Instance.playerSM.OnNextStateSM();
                    PlayerController.Instance.SetLeftIKLerpTarget(1f);
                    PlayerController.Instance.SetRightIKLerpTarget(1f);
                }
            }
            if ((PlayerController.Instance.boardController.boardRigidbody.position - PlayerController.Instance.boardController.boardTargetPosition.position).magnitude > 0.5f)
            {
                PlayerController.Instance.ForceBail();
            }
            if (!_checkForCollisions)
            {
                _collisionTimer += Time.deltaTime;
                if (_collisionTimer > 0.2f)
                {
                    _checkForCollisions = true;
                }
            }
            _timer += Time.deltaTime;
            if (PlayerController.Instance.GetAnimReleased())
            {
                if (!_popRotationDone)
                {
                    PlayerController.Instance.boardController.Rotate(true, false);
                }
                else
                {
                    PlayerController.Instance.FlipTrickRotation();
                }
            }
            if (!PlayerController.Instance.GetAnimReleased())
            {
                if (!_wasGrinding)
                {
                    if (!_popRotationDone)
                    {
                        PlayerController.Instance.boardController.Rotate(true, false);
                    }
                    else
                    {
                        PlayerController.Instance.PhysicsRotation(50f, 10f);
                        PlayerController.Instance.boardController.UpdateReferenceBoardTargetRotation();
                    }
                }
                else if (!_popRotationDone)
                {
                    PlayerController.Instance.boardController.Rotate(true, false);
                }
                else
                {
                    PlayerController.Instance.PhysicsRotation(50f, 10f);
                    PlayerController.Instance.boardController.UpdateReferenceBoardTargetRotation();
                }
            }
            if (!_flipDetected)
            {
                PlayerController.Instance.GetAnimReleased();
            }
        }

        public override float GetAugmentedAngle(StickInput p_stick)
        {
            if (p_stick.IsRightStick)
            {
                return _augmentedRightAngle;
            }
            return _augmentedLeftAngle;
        }

        public override void OnAnimatorUpdate()
        {
        }

        public override void OnStickUpdate(StickInput p_leftStick, StickInput p_rightStick)
        {
            if (_isRespawning)
            {
                return;
            }
            PlayerController.Instance.SetLeftIKOffset(p_leftStick.ToeAxis, p_leftStick.ForwardDir, p_leftStick.PopDir, p_leftStick.IsPopStick, false, PlayerController.Instance.GetAnimReleased());
            PlayerController.Instance.SetRightIKOffset(p_rightStick.ToeAxis, p_rightStick.ForwardDir, p_rightStick.PopDir, p_rightStick.IsPopStick, false, PlayerController.Instance.GetAnimReleased());
        }

        public override void OnStickFixedUpdate(StickInput p_leftStick, StickInput p_rightStick)
        {
            if (_isRespawning)
            {
                return;
            }

            if (_scooppFlipInputWindowDone)
            {
                switch (SettingsManager.Instance.controlType)
                {
                    case ControlType.Same:
                        if (SettingsManager.Instance.stance == Stance.Regular)
                        {
                            PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
                            PlayerController.Instance.SetFrontPivotRotation(p_rightStick.ToeAxis);
                            PlayerController.Instance.SetBackPivotRotation(p_leftStick.ToeAxis);
                            PlayerController.Instance.SetPivotForwardRotation(p_leftStick.ForwardDir + p_rightStick.ForwardDir, Mathf.Lerp(5f, 10f, Time.deltaTime * 50f));
                            PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                            return;
                        }
                        PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_rightStick.rawInput.pos.x) - Mathf.Abs(p_leftStick.rawInput.pos.x));
                        PlayerController.Instance.SetFrontPivotRotation(-p_leftStick.ToeAxis);
                        PlayerController.Instance.SetBackPivotRotation(-p_rightStick.ToeAxis);
                        PlayerController.Instance.SetPivotForwardRotation(p_leftStick.ForwardDir + p_rightStick.ForwardDir, Mathf.Lerp(5f, 10f, Time.deltaTime * 50f));
                        PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                        return;
                    case ControlType.Swap:
                        if (!PlayerController.Instance.IsSwitch)
                        {
                            if (SettingsManager.Instance.stance == Stance.Regular)
                            {
                                PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
                                PlayerController.Instance.SetFrontPivotRotation(p_rightStick.ToeAxis);
                                PlayerController.Instance.SetBackPivotRotation(p_leftStick.ToeAxis);
                                PlayerController.Instance.SetPivotForwardRotation(p_leftStick.ForwardDir + p_rightStick.ForwardDir, Mathf.Lerp(5f, 10f, Time.deltaTime * 50f));
                                PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                                return;
                            }
                            PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_rightStick.rawInput.pos.x) - Mathf.Abs(p_leftStick.rawInput.pos.x));
                            PlayerController.Instance.SetFrontPivotRotation(-p_leftStick.ToeAxis);
                            PlayerController.Instance.SetBackPivotRotation(-p_rightStick.ToeAxis);
                            PlayerController.Instance.SetPivotForwardRotation(p_leftStick.ForwardDir + p_rightStick.ForwardDir, Mathf.Lerp(5f, 10f, Time.deltaTime * 50f));
                            PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                            return;
                        }
                        else
                        {
                            if (SettingsManager.Instance.stance == Stance.Regular)
                            {
                                PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_rightStick.rawInput.pos.x) - Mathf.Abs(p_leftStick.rawInput.pos.x));
                                PlayerController.Instance.SetFrontPivotRotation(p_leftStick.ToeAxis);
                                PlayerController.Instance.SetBackPivotRotation(p_rightStick.ToeAxis);
                                PlayerController.Instance.SetPivotForwardRotation(p_leftStick.ForwardDir + p_rightStick.ForwardDir, Mathf.Lerp(5f, 10f, Time.deltaTime * 50f));
                                PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                                return;
                            }
                            PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
                            PlayerController.Instance.SetFrontPivotRotation(-p_rightStick.ToeAxis);
                            PlayerController.Instance.SetBackPivotRotation(-p_leftStick.ToeAxis);
                            PlayerController.Instance.SetPivotForwardRotation(p_leftStick.ForwardDir + p_rightStick.ForwardDir, Mathf.Lerp(5f, 10f, Time.deltaTime * 50f));
                            PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                            return;
                        }
                        break;
                    case ControlType.Simple:
                        if (!PlayerController.Instance.IsSwitch)
                        {
                            if (SettingsManager.Instance.stance == Stance.Regular)
                            {
                                PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
                                PlayerController.Instance.SetFrontPivotRotation(p_rightStick.ToeAxis);
                                PlayerController.Instance.SetBackPivotRotation(p_leftStick.ToeAxis);
                                PlayerController.Instance.SetPivotForwardRotation(p_leftStick.ForwardDir + p_rightStick.ForwardDir, Mathf.Lerp(5f, 10f, Time.deltaTime * 50f));
                                PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                                return;
                            }
                            PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_rightStick.rawInput.pos.x) - Mathf.Abs(p_leftStick.rawInput.pos.x));
                            PlayerController.Instance.SetFrontPivotRotation(-p_leftStick.ToeAxis);
                            PlayerController.Instance.SetBackPivotRotation(-p_rightStick.ToeAxis);
                            PlayerController.Instance.SetPivotForwardRotation(p_leftStick.ForwardDir + p_rightStick.ForwardDir, Mathf.Lerp(5f, 10f, Time.deltaTime * 50f));
                            PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                            return;
                        }
                        else
                        {
                            if (SettingsManager.Instance.stance == Stance.Regular)
                            {
                                PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
                                PlayerController.Instance.SetFrontPivotRotation(p_rightStick.ToeAxis);
                                PlayerController.Instance.SetBackPivotRotation(p_leftStick.ToeAxis);
                                PlayerController.Instance.SetPivotForwardRotation(p_leftStick.ForwardDir + p_rightStick.ForwardDir, Mathf.Lerp(5f, 10f, Time.deltaTime * 50f));
                                PlayerController.Instance.SetPivotSideRotation(p_rightStick.ToeAxis - p_leftStick.ToeAxis);
                                return;
                            }
                            PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
                            PlayerController.Instance.SetFrontPivotRotation(-p_rightStick.ToeAxis);
                            PlayerController.Instance.SetBackPivotRotation(-p_leftStick.ToeAxis);
                            PlayerController.Instance.SetPivotForwardRotation(p_leftStick.ForwardDir + p_rightStick.ForwardDir, Mathf.Lerp(5f, 10f, Time.deltaTime * 50f));
                            PlayerController.Instance.SetPivotSideRotation(p_rightStick.ToeAxis - p_leftStick.ToeAxis);
                            return;
                        }
                        break;
                    default:
                        return;
                }
            }
            else
            {
                switch (SettingsManager.Instance.controlType)
                {
                    case ControlType.Same:
                        {
                            if (SettingsManager.Instance.stance == Stance.Regular)
                            {
                                PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
                                PlayerController.Instance.SetFrontPivotRotation(p_rightStick.ToeAxis);
                                PlayerController.Instance.SetBackPivotRotation(p_leftStick.ToeAxis);
                                float num = PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie");
                                if (num > 0f)
                                {
                                    num = 2f;
                                }
                                else
                                {
                                    num = -2f;
                                }
                                PlayerController.Instance.SetPivotForwardRotation(num, 20f);
                                PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                                return;
                            }
                            PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_rightStick.rawInput.pos.x) - Mathf.Abs(p_leftStick.rawInput.pos.x));
                            PlayerController.Instance.SetFrontPivotRotation(-p_leftStick.ToeAxis);
                            PlayerController.Instance.SetBackPivotRotation(-p_rightStick.ToeAxis);
                            float num2 = PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie");
                            if (num2 > 0f)
                            {
                                num2 = 2f;
                            }
                            else
                            {
                                num2 = -2f;
                            }
                            PlayerController.Instance.SetPivotForwardRotation(num2, 20f);
                            PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                            return;
                        }
                    case ControlType.Swap:
                        if (PlayerController.Instance.IsSwitch)
                        {
                            if (SettingsManager.Instance.stance == Stance.Regular)
                            {
                                PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
                                PlayerController.Instance.SetFrontPivotRotation(p_rightStick.ToeAxis);
                                PlayerController.Instance.SetBackPivotRotation(p_leftStick.ToeAxis);
                                float num3 = PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie");
                                if (num3 > 0f)
                                {
                                    num3 = 2f;
                                }
                                else
                                {
                                    num3 = -2f;
                                }
                                PlayerController.Instance.SetPivotForwardRotation(num3, 20f);
                                PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                                return;
                            }
                            PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_rightStick.rawInput.pos.x) - Mathf.Abs(p_leftStick.rawInput.pos.x));
                            PlayerController.Instance.SetFrontPivotRotation(-p_leftStick.ToeAxis);
                            PlayerController.Instance.SetBackPivotRotation(-p_rightStick.ToeAxis);
                            float num4 = PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie");
                            if (num4 > 0f)
                            {
                                num4 = 2f;
                            }
                            else
                            {
                                num4 = -2f;
                            }
                            PlayerController.Instance.SetPivotForwardRotation(num4, 20f);
                            PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                            return;
                        }
                        else
                        {
                            if (SettingsManager.Instance.stance == Stance.Regular)
                            {
                                PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_rightStick.rawInput.pos.x) - Mathf.Abs(p_leftStick.rawInput.pos.x));
                                PlayerController.Instance.SetFrontPivotRotation(p_leftStick.ToeAxis);
                                PlayerController.Instance.SetBackPivotRotation(p_rightStick.ToeAxis);
                                float num5 = PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie");
                                if (num5 > 0f)
                                {
                                    num5 = 2f;
                                }
                                else
                                {
                                    num5 = -2f;
                                }
                                PlayerController.Instance.SetPivotForwardRotation(num5, 20f);
                                PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                                return;
                            }
                            PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
                            PlayerController.Instance.SetFrontPivotRotation(-p_rightStick.ToeAxis);
                            PlayerController.Instance.SetBackPivotRotation(-p_leftStick.ToeAxis);
                            float num6 = PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie");
                            if (num6 > 0f)
                            {
                                num6 = 2f;
                            }
                            else
                            {
                                num6 = -2f;
                            }
                            PlayerController.Instance.SetPivotForwardRotation(num6, 20f);
                            PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                            return;
                        }
                        break;
                    case ControlType.Simple:
                        if (!PlayerController.Instance.IsSwitch)
                        {
                            if (SettingsManager.Instance.stance == Stance.Regular)
                            {
                                PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
                                PlayerController.Instance.SetFrontPivotRotation(p_rightStick.ToeAxis);
                                PlayerController.Instance.SetBackPivotRotation(p_leftStick.ToeAxis);
                                float num7 = PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie");
                                if (num7 > 0f)
                                {
                                    num7 = 2f;
                                }
                                else
                                {
                                    num7 = -2f;
                                }
                                PlayerController.Instance.SetPivotForwardRotation(num7, 20f);
                                PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                                return;
                            }
                            PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_rightStick.rawInput.pos.x) - Mathf.Abs(p_leftStick.rawInput.pos.x));
                            PlayerController.Instance.SetFrontPivotRotation(-p_leftStick.ToeAxis);
                            PlayerController.Instance.SetBackPivotRotation(-p_rightStick.ToeAxis);
                            float num8 = PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie");
                            if (num8 > 0f)
                            {
                                num8 = 2f;
                            }
                            else
                            {
                                num8 = -2f;
                            }
                            PlayerController.Instance.SetPivotForwardRotation(num8, 20f);
                            PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                            return;
                        }
                        else
                        {
                            if (SettingsManager.Instance.stance == Stance.Regular)
                            {
                                PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
                                PlayerController.Instance.SetFrontPivotRotation(p_rightStick.ToeAxis);
                                PlayerController.Instance.SetBackPivotRotation(p_leftStick.ToeAxis);
                                float num9 = PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie");
                                if (num9 > 0f)
                                {
                                    num9 = 2f;
                                }
                                else
                                {
                                    num9 = -2f;
                                }
                                PlayerController.Instance.SetPivotForwardRotation(num9, 20f);
                                PlayerController.Instance.SetPivotSideRotation(p_rightStick.ToeAxis - p_leftStick.ToeAxis);
                                return;
                            }
                            PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
                            PlayerController.Instance.SetFrontPivotRotation(-p_rightStick.ToeAxis);
                            PlayerController.Instance.SetBackPivotRotation(-p_leftStick.ToeAxis);
                            float num10 = PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie");
                            if (num10 > 0f)
                            {
                                num10 = 2f;
                            }
                            else
                            {
                                num10 = -2f;
                            }
                            PlayerController.Instance.SetPivotForwardRotation(num10, 20f);
                            PlayerController.Instance.SetPivotSideRotation(p_rightStick.ToeAxis - p_leftStick.ToeAxis);
                            return;
                        }
                        break;
                    default:
                        return;
                }
            }
        }

        public override void OnFlipStickUpdate()
        {
            if (!_scooppFlipInputWindowDone)
            {
                if (!_flipDetected && !Main.settings.FlipsAfterPop)
                {
                    return;
                }
                float num = 0f;
                PlayerController.Instance.OnFlipStickUpdate(ref _flipDetected, ref _potentialFlip, ref _initialFlipDir, ref _flipFrameCount, ref _flipFrameMax, ref _toeAxis, ref _flipVel, ref _popVel, ref _popDir, ref _flip, _flipStick, true, false, ref _invertVel, _popStick.IsRightStick ? _augmentedLeftAngle : _augmentedRightAngle, _scooppFlipInputWindowDone, _forwardLoad, ref num);
            }
        }

        public override void OnPopStickUpdate()
        {
            if (!_scooppFlipInputWindowDone)
            {
                if (_flipDetected && !Main.settings.ShuvMidFlip)
                {
                    return;
                }
                PlayerController.Instance.OnPopStickUpdate(0.1f, PlayerController.Instance.IsGrounded(), _popStick, ref _popVel, 10f, _forwardLoad, ref _setupDir, ref _invertVel, _popStick.IsRightStick ? _augmentedRightAngle : _augmentedLeftAngle);
            }
        }

        public override void OnLeftStickCenteredUpdate()
        {
            if (_scooppFlipInputWindowDone && !_leftCaught)
            {
                _leftCaught = true;
                if (_rightCaught)
                {
                    _rightCaughtFirst = true;
                    _catchRegistered = true;
                }
            }
        }

        public override void OnRightStickCenteredUpdate()
        {
            if (_scooppFlipInputWindowDone && !_rightCaught)
            {
                _rightCaught = true;
                if (_leftCaught)
                {
                    _leftCaughtFirst = true;
                    _catchRegistered = true;
                }
            }
        }

        public override void OnStickPressed(bool p_right)
        {
            if (!PlayerController.Instance.GetAnimReleased())
            {
                if (p_right)
                {
                    if (_rightOn && _leftOn)
                    {
                        PlayerController.Instance.SetRightIKLerpTarget(1f, 1f);
                        PlayerController.Instance.SetRightSteezeWeight(1f);
                        PlayerController.Instance.SetMaxSteezeRight(1f);
                        PlayerController.Instance.SetRightKneeIKTargetWeight(0.2f);
                        _rightOn = false;
                        return;
                    }
                    if (!_rightOn && _leftOn)
                    {
                        SoundManager.Instance.PlayCatchSound();
                        PlayerController.Instance.SetRightIKLerpTarget(0f, 0f);
                        PlayerController.Instance.SetRightSteezeWeight(0f);
                        PlayerController.Instance.SetMaxSteezeRight(0f);
                        PlayerController.Instance.SetRightKneeIKTargetWeight(1f);
                        _rightOn = true;
                        return;
                    }
                }
                else
                {
                    if (_leftOn && _rightOn)
                    {
                        PlayerController.Instance.SetLeftIKLerpTarget(1f, 1f);
                        PlayerController.Instance.SetLeftSteezeWeight(1f);
                        PlayerController.Instance.SetMaxSteezeLeft(1f);
                        PlayerController.Instance.SetLeftKneeIKTargetWeight(0.2f);
                        _leftOn = false;
                        return;
                    }
                    if (!_leftOn && _rightOn)
                    {
                        SoundManager.Instance.PlayCatchSound();
                        PlayerController.Instance.SetLeftIKLerpTarget(0f, 0f);
                        PlayerController.Instance.SetLeftSteezeWeight(0f);
                        PlayerController.Instance.SetMaxSteezeLeft(0f);
                        PlayerController.Instance.SetLeftKneeIKTargetWeight(1f);
                        _leftOn = true;
                    }
                }
            }
        }

        public override StickInput GetPopStick()
        {
            return _popStick;
        }

        public override bool IsInPopState()
        {
            return true;
        }

        public override void OnNextState()
        {
            if (!_wasGrinding)
            {
                object[] args = new object[]
                {
                _popStick,
                _flipStick,
                _initialFlipDir,
                _flipVel,
                _popVel,
                _toeAxis,
                _popDir,
                _flipDetected,
                _flip,
                _forwardLoad,
                _invertVel,
                _setupDir,
                _catchRegistered,
                _leftCaughtFirst,
                _rightCaughtFirst,
                _inAirTurnDelta
                };
                base.DoTransition(typeof(Custom_Released), args);
                return;
            }
            object[] args2 = new object[]
            {
            _popStick,
            _flipStick,
            _initialFlipDir,
            _flipVel,
            _popVel,
            _toeAxis,
            _popDir,
            _flipDetected,
            _flip,
            _forwardLoad,
            _invertVel,
            _setupDir,
            _wasGrinding,
            _catchRegistered,
            _leftCaughtFirst,
            _rightCaughtFirst,
            _inAirTurnDelta
            };
            base.DoTransition(typeof(Custom_Released), args2);
        }

        public override void SendEventEndFlipPeriod()
        {
        }

        public override void SendEventExtend(float value)
        {
            if (!_wasGrinding)
            {
                if (_leftOn && _rightOn)
                {
                    object[] args = new object[]
                    {
                    1,
                    _inAirTurnDelta
                    };
                    base.DoTransition(typeof(Custom_InAir), args);
                    return;
                }
                object[] args2 = new object[]
                {
                _wasGrinding,
                _leftOn,
                _rightOn,
                true,
                _inAirTurnDelta
                };
                base.DoTransition(typeof(Custom_InAir), args2);
                return;
            }
            else
            {
                if (_leftOn && _rightOn)
                {
                    object[] args3 = new object[]
                    {
                    _wasGrinding,
                    1,
                    _inAirTurnDelta
                    };
                    base.DoTransition(typeof(Custom_InAir), args3);
                    return;
                }
                object[] args4 = new object[]
                {
                _wasGrinding,
                _leftOn,
                _rightOn,
                true,
                _inAirTurnDelta
                };
                base.DoTransition(typeof(Custom_InAir), args4);
                return;
            }
        }

        public override void OnLBDown()
        {
            if (!Main.settings.Grabs)
            {
                return;
            }
            if (!PlayerController.Instance.GetAnimReleased() && canGrab)
            {
                PlayerController.Instance.SetRightIKLerpTarget(0f, 0f);
                PlayerController.Instance.SetRightSteezeWeight(0f);
                PlayerController.Instance.SetMaxSteezeRight(0f);
                PlayerController.Instance.SetLeftIKLerpTarget(0f, 0f);
                PlayerController.Instance.SetLeftSteezeWeight(0f);
                PlayerController.Instance.SetMaxSteezeLeft(0f);
                object[] args = new object[]
                {
                true,
                _inAirTurnDelta
                };
                base.DoTransition(typeof(Custom_Grabs), args);
            }
        }

        public override void OnLBHeld()
        {
            if (!Main.settings.Grabs)
            {
                return;
            }
            if (!PlayerController.Instance.GetAnimReleased() && canGrab)
            {
                PlayerController.Instance.SetRightIKLerpTarget(0f, 0f);
                PlayerController.Instance.SetRightSteezeWeight(0f);
                PlayerController.Instance.SetMaxSteezeRight(0f);
                PlayerController.Instance.SetLeftIKLerpTarget(0f, 0f);
                PlayerController.Instance.SetLeftSteezeWeight(0f);
                PlayerController.Instance.SetMaxSteezeLeft(0f);
                object[] args = new object[]
                {
                true,
                _inAirTurnDelta
                };
                base.DoTransition(typeof(Custom_Grabs), args);
            }
        }

        public override void OnRBDown()
        {
            if (!Main.settings.Grabs)
            {
                return;
            }
            if (!PlayerController.Instance.GetAnimReleased() && canGrab)
            {
                PlayerController.Instance.SetRightIKLerpTarget(0f, 0f);
                PlayerController.Instance.SetRightSteezeWeight(0f);
                PlayerController.Instance.SetMaxSteezeRight(0f);
                PlayerController.Instance.SetLeftIKLerpTarget(0f, 0f);
                PlayerController.Instance.SetLeftSteezeWeight(0f);
                PlayerController.Instance.SetMaxSteezeLeft(0f);
                object[] args = new object[]
                {
                false,
                _inAirTurnDelta
                };
                base.DoTransition(typeof(Custom_Grabs), args);
            }
        }

        public override void OnRBHeld()
        {
            if (!Main.settings.Grabs)
            {
                return;
            }
            if (!PlayerController.Instance.GetAnimReleased() && canGrab)
            {
                PlayerController.Instance.SetRightIKLerpTarget(0f, 0f);
                PlayerController.Instance.SetRightSteezeWeight(0f);
                PlayerController.Instance.SetMaxSteezeRight(0f);
                PlayerController.Instance.SetLeftIKLerpTarget(0f, 0f);
                PlayerController.Instance.SetLeftSteezeWeight(0f);
                PlayerController.Instance.SetMaxSteezeLeft(0f);
                object[] args = new object[]
                {
                false,
                _inAirTurnDelta
                };
                base.DoTransition(typeof(Custom_Grabs), args);
            }
        }

        public override bool LeftFootOff()
        {
            return !_leftOn;
        }

        public override bool RightFootOff()
        {
            return !_rightOn;
        }

        public override bool Popped()
        {
            return true;
        }

        public override bool CanGrind()
        {
            if (!Main.settings.Grinds)
            {
                return false;
            }
            return _checkForCollisions && !_flipDetected;
        }

        public override void OnGrindDetected()
        {
            if (!Main.settings.Grinds)
            {
                return;
            }

            if (!_wasGrinding && _checkForCollisions && !_flipDetected)
            {
                base.DoTransition(typeof(Custom_Grinding), null);
            }
        }

        public override void OnCanManual()
        {
            _canManual = true;
        }

        public override void OnPredictedCollisionEvent()
        {
            if (Main.settings.ManualFix)
            {
                if (!_flipDetected && _checkForCollisions)
                {
                    PredictedNextState();
                }
            }
            else
            {
                if (!_flipDetected && _checkForCollisions)
                {
                    PlayerController.Instance.SetBoardToMaster();
                    PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
                    PlayerController.Instance.AnimOllieTransition(false);
                    PlayerController.Instance.AnimSetupTransition(false);
                    Vector3 position = PlayerController.Instance.skaterController.skaterRigidbody.position;
                    Vector3.SignedAngle(PlayerController.Instance.skaterController.skaterTransform.forward, Vector3.ProjectOnPlane(PlayerController.Instance.boardController.GetClosestBoardForward(), PlayerController.Instance.skaterController.skaterTransform.up), PlayerController.Instance.skaterController.skaterTransform.up);
                    base.DoTransition(typeof(PlayerState_Impact), null);
                    PlayerController.Instance.skaterController.AddCollisionOffset();
                }
            }
        }

        public override void OnCollisionEnterEvent(Vector3 _impulse, bool _isBoard, Collision c)
        {
            if (_isBoard)
            {
                SoundManager.Instance.PlayBoardHit(_impulse.magnitude);
            }
            if (Main.settings.ManualFix)
            {
                if (!_flipDetected && _checkForCollisions)
                {
                    TransitionToNextState();
                }
            }
        }

        public override void OnCollisionStayEvent(Collision c)
        {
        }

        public override void OnFlipTriggerEnter(Collider c)
        {
            PlayerController.Instance.ToggleFlipColliders(false);
        }

        public override void OnFlipTriggerStay(Collider c)
        {
            if (PlayerController.Instance.IsFlipCollisionTriggerActive())
            {
                PlayerController.Instance.ToggleFlipColliders(false);
            }
        }

        public override void OnFlipTriggerEmpty()
        {
            PlayerController.Instance.ToggleFlipColliders(true);
        }

        public override void OnRespawn()
        {
            _isRespawning = true;
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


        private void HandleBoardControllerUpVector()
        {
            if (!Main.settings.Primos)
            {
                PlayerController.Instance.boardController.SetBoardControllerUpVector(PlayerController.Instance.skaterController.skaterTransform.up);
                return;
            }

            if (PlayerController.Instance.inputController.player.GetButton("X"))
            {
                PlayerController.Instance.boardController.SetBoardControllerUpVector(-PlayerController.Instance.skaterController.skaterTransform.right);
                XXLController.Instance.CanPrimoCatch = true;
            }
            else if (PlayerController.Instance.inputController.player.GetButton("A"))
            {
                PlayerController.Instance.boardController.SetBoardControllerUpVector(PlayerController.Instance.skaterController.skaterTransform.right);
                XXLController.Instance.CanPrimoCatch = true;
            }
            else
            {
                PlayerController.Instance.boardController.SetBoardControllerUpVector(PlayerController.Instance.skaterController.skaterTransform.up);
                XXLController.Instance.CanPrimoCatch = false;
            }
        }

        private void HandleHippieOllie()
        {
            if (Main.settings.HippieOllie)
            {
                bool isHippieJumping = false;
                if (PlayerController.Instance.inputController.player.GetButton("B"))
                {
                    if (isHippieJumping == false)
                    {
                        //RightLegIK
                        PlayerController.Instance.SetRightIKLerpTarget(1f, 1f);
                        PlayerController.Instance.SetRightSteezeWeight(0f);
                        PlayerController.Instance.SetMaxSteezeRight(0f);
                        PlayerController.Instance.SetRightKneeIKTargetWeight(1f);
                        PlayerController.Instance.SetRightIKWeight(1f);
                        PlayerController.Instance.SetRightKneeBendWeight(1f);
                        PlayerController.Instance.SetRightKneeBendWeightManually(1f);
                        //LeftLegIK
                        PlayerController.Instance.SetLeftIKLerpTarget(1f, 1f);
                        PlayerController.Instance.SetLeftSteezeWeight(0f);
                        PlayerController.Instance.SetMaxSteezeLeft(0f);
                        PlayerController.Instance.SetLeftKneeIKTargetWeight(1f);
                        PlayerController.Instance.SetLeftIKWeight(1f);
                        PlayerController.Instance.SetLeftKneeBendWeight(1f);
                        PlayerController.Instance.SetLeftKneeBendWeightManually(1f);
                        PlayerController.Instance.animationController.skaterAnim.SetBool("Released", true);
                        PlayerController.Instance.CrossFadeAnimation("Extend", 0.3f);
                        isHippieJumping = true;
                    }
                }
                else
                {
                    if (isHippieJumping == true)
                    {
                        PlayerController.Instance.AnimRelease(false);
                        isHippieJumping = false;
                    }
                }
            }
        }

        private void HandleLaidback()
        {
            if (XXLController.Instance.IsLateFlip)
            {
                if (Main.settings.LateFlipLaidbackFlips)
                {
                    if (_flipDetected && !PlayerController.Instance.IsCurrentAnimationPlaying("Extend"))
                    {
                        if (PlayerController.Instance.inputController.LeftStick.IsCentered && PlayerController.Instance.inputController.RightStick.IsCentered)
                        {
                            PlayerController.Instance.CrossFadeAnimation("Extend", 0.3f);
                            return;
                        }
                    }
                }
                return;
            }
            else if (XXLController.Instance.IsPrimoFlip)
            {
                if (Main.settings.PrimoLaidbackFlips)
                {
                    if (_flipDetected && !PlayerController.Instance.IsCurrentAnimationPlaying("Extend"))
                    {
                        if (PlayerController.Instance.inputController.LeftStick.IsCentered && PlayerController.Instance.inputController.RightStick.IsCentered)
                        {
                            PlayerController.Instance.CrossFadeAnimation("Extend", 0.3f);
                            return;
                        }
                    }
                }
                return;
            }
            else if (Main.settings.LaidbackFlips)
            {
                if (_flipDetected && !PlayerController.Instance.IsCurrentAnimationPlaying("Extend"))
                {
                    if (PlayerController.Instance.inputController.LeftStick.IsCentered && PlayerController.Instance.inputController.RightStick.IsCentered)
                    {
                        PlayerController.Instance.CrossFadeAnimation("Extend", 0.3f);
                    }
                }
            }
        }

        bool _manualling;
        bool _noseManualling;

        public override void OnManualUpdate(StickInput p_popStick, StickInput p_flipStick)
        {
            if (_rightOn || _leftOn)
            {
                PlayerController.Instance.AnimSetManual(true, Mathf.Lerp(PlayerController.Instance.AnimGetManualAxis(), p_popStick.ForwardDir, Time.deltaTime * 10f));
                _manualling = true;
                _noseManualling = false;
                _popStick = p_popStick;
                _flipStick = p_flipStick;
            }
        }

        public override void OnManualExit()
        {
            PlayerController.Instance.AnimSetManual(false, PlayerController.Instance.AnimGetManualAxis());
            _manualling = false;
        }

        public override void OnNoseManualUpdate(StickInput p_popStick, StickInput p_flipStick)
        {
            if (_rightOn || _leftOn)
            {
                PlayerController.Instance.AnimSetNoseManual(true, Mathf.Lerp(PlayerController.Instance.AnimGetManualAxis(), p_popStick.ForwardDir, Time.deltaTime * 10f));
                _popStick = p_popStick;
                _flipStick = p_flipStick;
                _noseManualling = true;
                _manualling = false;
            }
        }

        public override void OnNoseManualExit()
        {
            PlayerController.Instance.AnimSetNoseManual(false, PlayerController.Instance.AnimGetManualAxis());
            _noseManualling = false;
        }

        private void PredictedNextState()
        {
            if (!!_leftOn || !_rightOn)
            {
                PlayerController.Instance.ForceBail();
                return;
            }
            if (_manualling || _noseManualling)
            {
                PlayerController.Instance.SetRightIKRotationWeight(1f);
                PlayerController.Instance.SetLeftIKRotationWeight(1f);
                PlayerController.Instance.SetMaxSteeze(0f);
                PlayerController.Instance.SetLeftIKLerpTarget(0f);
                PlayerController.Instance.SetRightIKLerpTarget(0f);
                PlayerController.Instance.animationController.ScaleAnimSpeed(1f);
                PlayerController.Instance.boardController.ResetAll();
                PlayerController.Instance.AnimRelease(false);
                PlayerController.Instance.SetBoardToMaster();
                PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
                PlayerController.Instance.AnimOllieTransition(false);
                PlayerController.Instance.AnimSetupTransition(false);
                object[] args = new object[]
                {
                _popStick,
                _flipStick,
                !_noseManualling,
                _inAirTurnDelta,
                true,
                true
                };
                base.DoTransition(typeof(Custom_Manualling), args);
                return;
            }
            PlayerController.Instance.SetMaxSteeze(0f);
            PlayerController.Instance.SetLeftIKLerpTarget(0f);
            PlayerController.Instance.SetRightIKLerpTarget(0f);
            PlayerController.Instance.boardController.ResetAll();
            PlayerController.Instance.AnimRelease(false);
            PlayerController.Instance.SetBoardToMaster();
            PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
            PlayerController.Instance.AnimOllieTransition(false);
            PlayerController.Instance.AnimSetupTransition(false);
            PlayerController.Instance.ResetAnimationsAfterImpact();
            PlayerController.Instance.AnimLandedEarly(true);
            PlayerController.Instance.AnimSetManual(false, PlayerController.Instance.AnimGetManualAxis());
            _manualling = false;
            PlayerController.Instance.AnimSetNoseManual(false, PlayerController.Instance.AnimGetManualAxis());
            _noseManualling = false;
            base.DoTransition(typeof(Custom_Impact), null);
        }

        private void TransitionToNextState()
        {
            if (!_leftOn || !_rightOn)
            {
                PlayerController.Instance.ForceBail();
                return;
            }
            PlayerController.Instance.SetRightIKRotationWeight(1f);
            PlayerController.Instance.SetLeftIKRotationWeight(1f);
            if (_manualling || _noseManualling)
            {
                PlayerController.Instance.SetMaxSteeze(0f);
                PlayerController.Instance.SetLeftIKLerpTarget(0f);
                PlayerController.Instance.SetRightIKLerpTarget(0f);
                PlayerController.Instance.animationController.ScaleAnimSpeed(1f);
                PlayerController.Instance.boardController.ResetAll();
                PlayerController.Instance.AnimRelease(false);
                PlayerController.Instance.SetBoardToMaster();
                PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
                PlayerController.Instance.AnimOllieTransition(false);
                PlayerController.Instance.AnimSetupTransition(false);
                object[] args = new object[]
                {
                _popStick,
                _flipStick,
                !_noseManualling,
                _inAirTurnDelta,
                true,
                true
                };
                base.DoTransition(typeof(Custom_Manualling), args);
                return;
            }
            PlayerController.Instance.SetMaxSteeze(0f);
            PlayerController.Instance.SetLeftIKLerpTarget(0f);
            PlayerController.Instance.SetRightIKLerpTarget(0f);
            PlayerController.Instance.boardController.ResetAll();
            PlayerController.Instance.AnimRelease(false);
            PlayerController.Instance.SetBoardToMaster();
            PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
            PlayerController.Instance.AnimOllieTransition(false);
            PlayerController.Instance.AnimSetupTransition(false);
            PlayerController.Instance.ResetAnimationsAfterImpact();
            PlayerController.Instance.AnimLandedEarly(true);
            PlayerController.Instance.AnimSetManual(false, PlayerController.Instance.AnimGetManualAxis());
            _manualling = false;
            PlayerController.Instance.AnimSetNoseManual(false, PlayerController.Instance.AnimGetManualAxis());
            _noseManualling = false;
            base.DoTransition(typeof(Custom_Impact), null);
        }
    }
}