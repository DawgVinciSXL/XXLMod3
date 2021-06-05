using Dreamteck.Splines;
using FSMHelper;
using SkaterXL.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using XXLMod3.Controller;
using XXLMod3.Core;

namespace XXLMod3.PlayerStates
{
    public class Custom_InAir : PlayerState_OnBoard
    {
        private bool _bumpOut;
        private bool _canEnterCoping;
        private bool _canGrind = true;
        private bool _caughtBoth = true;
        private bool _caughtLeft = true;
        private bool _caughtRight = true;
        private bool _colliding;
        private bool _delayGrab;
        private bool _footCollidersDisabled;
        private bool _highestFound;
        private bool _manualling;
        private bool _noseManualling;
        private bool _popped;
        private bool _predictedCollision;
        private bool _wasGrinding;
        private bool _wasManualling;
        private bool _wasPowersliding;

        private float _boardCenteredTimer;
        private float _boardHeighest;
        private float _bumpOutTimer;
        private float _canGrindTimer;
        private float _delayGrabTimer;
        private float _footColliderTimer;
        private float _grindExitTimer;
        private float _highestSkaterY;
        private float _inAirTurnDelta;
        private float _leftForwardDir;
        private float _leftToeAxis;
        private float _manualTimer;
        private float _rightForwardDir;
        private float _rightToeAxis;
        private float _skaterY;
        private float _timeInState;

        private SplineComputer _spline;

        private StickInput _flipStick;
        private StickInput _popStick;

        private Vector2 _leftStick = Vector2.zero;
        private Vector2 _rightStick = Vector2.zero;

        public override void SetupDefinition(ref FSMStateType stateType, ref List<Type> children)
        {
            stateType = FSMStateType.Type_OR;
        }

        public Custom_InAir()
        {
        }

        public Custom_InAir(float p_inAirTurnDelta)
        {
            _inAirTurnDelta = p_inAirTurnDelta;
        }

        public Custom_InAir(bool p_wasPowersliding)
        {
            _wasPowersliding = p_wasPowersliding;
            _canEnterCoping = true;
        }

        public Custom_InAir(int p_popped, float p_inAirTurnDelta)
        {
            _popped = true;
            if (p_popped == 2)
            {
                _delayGrab = true;
            }
            _inAirTurnDelta = p_inAirTurnDelta;
        }

        public Custom_InAir(bool p_wasGrinding, float p_inAirTurnDelta)
        {
            _wasGrinding = p_wasGrinding;
            _inAirTurnDelta = p_inAirTurnDelta;
        }

        public Custom_InAir(bool p_wasGrinding, int p_popped, float p_inAirTurnDelta)
        {
            _wasGrinding = p_wasGrinding;
            _popped = true;
            _inAirTurnDelta = p_inAirTurnDelta;
        }

        public Custom_InAir(bool p_wasGrinding, bool p_caughtLeft, bool p_caughtRight, bool p_popped, float p_inAirTurnDelta)
        {
            _popped = p_popped;
            _wasGrinding = p_wasGrinding;
            _caughtLeft = p_caughtLeft;
            _caughtRight = p_caughtRight;
            if (!_caughtRight && _caughtLeft)
            {
                PlayerController.Instance.SetRightIKLerpTarget(1f, 1f);
                PlayerController.Instance.SetRightSteezeWeight(1f);
                PlayerController.Instance.SetMaxSteezeRight(1f);
                _caughtBoth = false;
            }
            if (!_caughtLeft && _caughtRight)
            {
                PlayerController.Instance.SetLeftIKLerpTarget(1f, 1f);
                PlayerController.Instance.SetLeftSteezeWeight(1f);
                PlayerController.Instance.SetMaxSteezeLeft(1f);
                _caughtBoth = false;
            }
            _inAirTurnDelta = p_inAirTurnDelta;
        }

        public Custom_InAir(bool p_wasGrinding, bool p_canGrind, float p_inAirTurnDelta)
        {
            _wasGrinding = p_wasGrinding;
            _canGrind = p_canGrind;
            _inAirTurnDelta = p_inAirTurnDelta;
            _canEnterCoping = true;
        }

        public Custom_InAir(bool p_wasGrinding, bool p_canGrind, bool p_bumpOut, float p_inAirTurnDelta)
        {
            _bumpOut = p_bumpOut;
            _wasGrinding = p_wasGrinding;
            _canGrind = p_canGrind;
            _inAirTurnDelta = p_inAirTurnDelta;
            _canEnterCoping = true;
        }

        public Custom_InAir(bool p_wasGrinding, bool p_canGrind, float p_inAirTurnDelta, bool p_wasManualling)
        {
            _wasGrinding = p_wasGrinding;
            _canGrind = p_canGrind;
            _inAirTurnDelta = p_inAirTurnDelta;
            _wasManualling = p_wasManualling;
            _canEnterCoping = true;
        }

        public Custom_InAir(bool p_wasGrinding, bool p_canGrind, SplineComputer p_spline, float p_inAirTurnDelta)
        {
            _wasGrinding = p_wasGrinding;
            _canGrind = p_canGrind;
            _spline = p_spline;
            _inAirTurnDelta = p_inAirTurnDelta;
        }

        public override void Enter()
        {
            PlayerController.Instance.currentStateEnum = PlayerController.CurrentState.InAir;
            XXLController.CurrentState = CurrentState.InAir;
            PlayerController.Instance.cameraController.NeedToSlowLerpCamera = true;
            PlayerController.Instance.ToggleFlipColliders(false);
            _skaterY = PlayerController.Instance.skaterController.skaterTransform.position.y;
            _highestSkaterY = _skaterY;
            _boardHeighest = PlayerController.Instance.boardController.boardTransform.position.y;
            if (PlayerController.Instance.skaterController.rightFootCollider.isTrigger || PlayerController.Instance.skaterController.leftFootCollider.isTrigger)
            {
                _footCollidersDisabled = true;
            }
            PlayerController.Instance.skaterController.InitializeSkateRotation();
            if (PlayerController.Instance.inputController.turningMode != InputController.TurningMode.FastLeft && PlayerController.Instance.inputController.turningMode != InputController.TurningMode.FastRight)
            {
                if (PlayerController.Instance.movementMaster == PlayerController.MovementMaster.Skater)
                {
                    PlayerController.Instance.SetTurningMode(InputController.TurningMode.InAir);
                }
                else if (PlayerController.Instance.movementMaster == PlayerController.MovementMaster.Board)
                {
                    PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
                }
            }
            EventManager.Instance.EnterAir();
        }

        public override void Exit()
        {
            PlayerController.Instance.cameraController.NeedToSlowLerpCamera = false;
            PlayerController.Instance.AnimOllieTransition(false);
            PlayerController.Instance.AnimSetRollOff(false);
            PlayerController.Instance.AnimSetNoComply(false);
            PlayerController.Instance.skaterController.InitializeSkateRotation();
            PlayerController.Instance.skaterController.skaterRigidbody.angularVelocity = Vector3.zero;
        }

        public override void Update()
        {
            HandleBoardControllerUpVector(); //////////////
            HandleHippieOllie();
            _timeInState += Time.deltaTime;
            if (_canEnterCoping && _timeInState > 0.2f)
            {
                _canEnterCoping = false;
            }
            SoundManager.Instance.SetRollingVolumeFromRPS(PlayerController.Instance.GetSurfaceTag(PlayerController.Instance.boardController.GetSurfaceTagString()), PlayerController.Instance.boardController._rollSoundSpeed);
            if (_caughtRight && _caughtLeft)
            {
                PlayerController.Instance.LerpKneeIkWeight();
            }
            else
            {
                PlayerController.Instance.LerpLeftKneeIkWeight();
                PlayerController.Instance.LerpRightKneeIkWeight();
            }
            _skaterY = PlayerController.Instance.skaterController.skaterTransform.position.y;
            if (_skaterY > _highestSkaterY)
            {
                _highestSkaterY = _skaterY;
                _boardHeighest = PlayerController.Instance.boardController.boardTransform.position.y;
            }
            else if (!_highestFound)
            {
                _highestFound = true;
            }
            UpdateTimers();
            if (PlayerController.Instance.movementMaster == PlayerController.MovementMaster.Board)
            {
                PlayerController.Instance.SetTurningMode(InputController.TurningMode.InAir);
                PlayerController.Instance.SetSkaterToMaster();
            }
            Vector3 vector = SkaterXL.Core.Mathd.LocalAngularVelocity(PlayerController.Instance.skaterController.skaterRigidbody);
            _inAirTurnDelta += 57.29578f * vector.y * Time.deltaTime;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            PlayerController.Instance.ScalePlayerCollider();
            PlayerController.Instance.comController.UpdateCOM();
            if ((PlayerController.Instance.boardController.boardRigidbody.position - PlayerController.Instance.boardController.boardTargetPosition.position).magnitude > 0.5f)
            {
                PlayerController.Instance.ForceBail();
            }
            PlayerController.Instance.boardController.ApplyOnBoardMaxRoll(_colliding, 90f);
            PlayerController.Instance.SetRotationTarget();
            PlayerController.Instance.SnapRotation();
            if (PlayerController.Instance.boardController.triggerManager.IsColliding && PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude > PlayerController.Instance.VelocityOnPop.magnitude)
            {
                Vector3 velocity = PlayerController.Instance.boardController.boardRigidbody.velocity.normalized * PlayerController.Instance.VelocityOnPop.magnitude;
                PlayerController.Instance.boardController.boardRigidbody.velocity = velocity;
            }
            if (PlayerController.Instance.boardController.triggerManager.IsColliding && PlayerController.Instance.skaterController.skaterRigidbody.velocity.magnitude > PlayerController.Instance.VelocityOnPop.magnitude)
            {
                Vector3 velocity2 = PlayerController.Instance.skaterController.skaterRigidbody.velocity.normalized * PlayerController.Instance.VelocityOnPop.magnitude;
                PlayerController.Instance.skaterController.skaterRigidbody.velocity = velocity2;
            }
        }

        private void UpdateTimers()
        {
            if (_wasGrinding)
            {
                _grindExitTimer += Time.deltaTime;
                if (_grindExitTimer > (Main.settings.ManualDelay ? 0.2f : 0f))
                {
                    _wasGrinding = false;
                    _grindExitTimer = 0f;
                }
            }
            if (_delayGrab)
            {
                _delayGrabTimer += Time.deltaTime;
                if (_delayGrabTimer > 0.2f)
                {
                    _delayGrab = false;
                }
            }
            if (_wasManualling)
            {
                _manualTimer += Time.deltaTime;
                if (_manualTimer > (Main.settings.ManualDelay ? 0.3f : 0f))
                {
                    _wasManualling = false;
                }
            }
            if (!_canGrind)
            {
                _canGrindTimer += Time.deltaTime;
                if (_canGrindTimer > (Main.settings.GrindBumpDelay ? 0.3f : 0.001f))
                {
                    _canGrind = true;
                    _canGrindTimer = 0f;
                }
            }
            if (_bumpOut)
            {
                _bumpOutTimer += Time.deltaTime;
                if (!Main.settings.GrindBumpDelay)
                {
                    if (_bumpOutTimer > 0.3f)
                    {
                        _bumpOut = false;
                        _bumpOutTimer = 0f;
                    }
                }
                else
                {
                    if (_bumpOutTimer > 0.5f)
                    {
                        _bumpOut = false;
                        _bumpOutTimer = 0f;
                    }
                }
            }
        }

        public override bool IsInAirState()
        {
            return true;
        }

        public override bool Popped()
        {
            return _popped;
        }

        public override void OnStickFixedUpdate(StickInput p_leftStick, StickInput p_rightStick)
        {
            if (_caughtLeft && _caughtRight)
            {
                switch (SettingsManager.Instance.controlType)
                {
                    case ControlType.Same:
                        if (SettingsManager.Instance.stance == Stance.Regular)
                        {
                            PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
                            PlayerController.Instance.SetFrontPivotRotation(p_rightStick.ToeAxis);
                            PlayerController.Instance.SetBackPivotRotation(p_leftStick.ToeAxis);
                            PlayerController.Instance.SetPivotForwardRotation((p_leftStick.ForwardDir + p_rightStick.ForwardDir) * 0.7f, 15f);
                            PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                            return;
                        }
                        PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_rightStick.rawInput.pos.x) - Mathf.Abs(p_leftStick.rawInput.pos.x));
                        PlayerController.Instance.SetFrontPivotRotation(-p_leftStick.ToeAxis);
                        PlayerController.Instance.SetBackPivotRotation(-p_rightStick.ToeAxis);
                        PlayerController.Instance.SetPivotForwardRotation((p_leftStick.ForwardDir + p_rightStick.ForwardDir) * 0.7f, 15f);
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
                                PlayerController.Instance.SetPivotForwardRotation((p_leftStick.ForwardDir + p_rightStick.ForwardDir) * 0.7f, 15f);
                                PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                                return;
                            }
                            PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_rightStick.rawInput.pos.x) - Mathf.Abs(p_leftStick.rawInput.pos.x));
                            PlayerController.Instance.SetFrontPivotRotation(-p_leftStick.ToeAxis);
                            PlayerController.Instance.SetBackPivotRotation(-p_rightStick.ToeAxis);
                            PlayerController.Instance.SetPivotForwardRotation((p_leftStick.ForwardDir + p_rightStick.ForwardDir) * 0.7f, 15f);
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
                                PlayerController.Instance.SetPivotForwardRotation((p_leftStick.ForwardDir + p_rightStick.ForwardDir) * 0.7f, 15f);
                                PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                                return;
                            }
                            PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
                            PlayerController.Instance.SetFrontPivotRotation(-p_rightStick.ToeAxis);
                            PlayerController.Instance.SetBackPivotRotation(-p_leftStick.ToeAxis);
                            PlayerController.Instance.SetPivotForwardRotation((p_leftStick.ForwardDir + p_rightStick.ForwardDir) * 0.7f, 15f);
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
                                PlayerController.Instance.SetPivotForwardRotation((p_leftStick.ForwardDir + p_rightStick.ForwardDir) * 0.7f, 15f);
                                PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                                return;
                            }
                            PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_rightStick.rawInput.pos.x) - Mathf.Abs(p_leftStick.rawInput.pos.x));
                            PlayerController.Instance.SetFrontPivotRotation(-p_leftStick.ToeAxis);
                            PlayerController.Instance.SetBackPivotRotation(-p_rightStick.ToeAxis);
                            PlayerController.Instance.SetPivotForwardRotation((p_leftStick.ForwardDir + p_rightStick.ForwardDir) * 0.7f, 15f);
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
                                PlayerController.Instance.SetPivotForwardRotation((p_leftStick.ForwardDir + p_rightStick.ForwardDir) * 0.7f, 15f);
                                PlayerController.Instance.SetPivotSideRotation(p_rightStick.ToeAxis - p_leftStick.ToeAxis);
                                return;
                            }
                            PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
                            PlayerController.Instance.SetFrontPivotRotation(-p_rightStick.ToeAxis);
                            PlayerController.Instance.SetBackPivotRotation(-p_leftStick.ToeAxis);
                            PlayerController.Instance.SetPivotForwardRotation((p_leftStick.ForwardDir + p_rightStick.ForwardDir) * 0.7f, 15f);
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
                _leftToeAxis = (_caughtLeft ? (p_leftStick.ToeAxis * 1.5f) : 0f);
                _rightToeAxis = (_caughtRight ? (p_rightStick.ToeAxis * 1.5f) : 0f);
                _leftForwardDir = (_caughtLeft ? (p_leftStick.ForwardDir * 2f) : 0f);
                _rightForwardDir = (_caughtRight ? (p_rightStick.ForwardDir * 2f) : 0f);
                switch (SettingsManager.Instance.controlType)
                {
                    case ControlType.Same:
                        if (SettingsManager.Instance.stance == Stance.Regular)
                        {
                            PlayerController.Instance.SetFrontPivotRotation(_rightToeAxis);
                            PlayerController.Instance.SetBackPivotRotation(_leftToeAxis);
                            PlayerController.Instance.SetPivotForwardRotation((_leftForwardDir + _rightForwardDir) * 0.7f, 15f);
                            PlayerController.Instance.SetPivotSideRotation(_leftToeAxis - _rightToeAxis);
                            return;
                        }
                        PlayerController.Instance.SetFrontPivotRotation(-_leftToeAxis);
                        PlayerController.Instance.SetBackPivotRotation(-_rightToeAxis);
                        PlayerController.Instance.SetPivotForwardRotation((p_leftStick.ForwardDir + p_rightStick.ForwardDir) * 0.7f, 15f);
                        PlayerController.Instance.SetPivotSideRotation(_leftToeAxis - _rightToeAxis);
                        return;
                    case ControlType.Swap:
                        if (!PlayerController.Instance.IsSwitch)
                        {
                            if (SettingsManager.Instance.stance == Stance.Regular)
                            {
                                PlayerController.Instance.SetFrontPivotRotation(_rightToeAxis);
                                PlayerController.Instance.SetBackPivotRotation(_leftToeAxis);
                                PlayerController.Instance.SetPivotForwardRotation((_leftForwardDir + _rightForwardDir) * 0.7f, 15f);
                                PlayerController.Instance.SetPivotSideRotation(_leftToeAxis - _rightToeAxis);
                                return;
                            }
                            PlayerController.Instance.SetFrontPivotRotation(_leftToeAxis);
                            PlayerController.Instance.SetBackPivotRotation(_rightToeAxis);
                            PlayerController.Instance.SetPivotForwardRotation((_leftForwardDir + _rightForwardDir) * 0.7f, 15f);
                            PlayerController.Instance.SetPivotSideRotation(_leftToeAxis - _rightToeAxis);
                            return;
                        }
                        else
                        {
                            if (SettingsManager.Instance.stance == Stance.Regular)
                            {
                                PlayerController.Instance.SetFrontPivotRotation(_leftToeAxis);
                                PlayerController.Instance.SetBackPivotRotation(_rightToeAxis);
                                PlayerController.Instance.SetPivotForwardRotation((_leftForwardDir + _rightForwardDir) * 0.7f, 15f);
                                PlayerController.Instance.SetPivotSideRotation(_leftToeAxis - _rightToeAxis);
                                return;
                            }
                            PlayerController.Instance.SetFrontPivotRotation(_rightToeAxis);
                            PlayerController.Instance.SetBackPivotRotation(_leftToeAxis);
                            PlayerController.Instance.SetPivotForwardRotation((_leftForwardDir + _rightForwardDir) * 0.7f, 15f);
                            PlayerController.Instance.SetPivotSideRotation(_leftToeAxis - _rightToeAxis);
                            return;
                        }
                        break;
                    case ControlType.Simple:
                        if (!PlayerController.Instance.IsSwitch)
                        {
                            if (SettingsManager.Instance.stance == Stance.Regular)
                            {
                                PlayerController.Instance.SetFrontPivotRotation(_rightToeAxis);
                                PlayerController.Instance.SetBackPivotRotation(_leftToeAxis);
                                PlayerController.Instance.SetPivotForwardRotation((_leftForwardDir + _rightForwardDir) * 0.7f, 15f);
                                PlayerController.Instance.SetPivotSideRotation(_leftToeAxis - _rightToeAxis);
                                return;
                            }
                            PlayerController.Instance.SetFrontPivotRotation(-_leftToeAxis);
                            PlayerController.Instance.SetBackPivotRotation(-_rightToeAxis);
                            PlayerController.Instance.SetPivotForwardRotation((p_leftStick.ForwardDir + p_rightStick.ForwardDir) * 0.7f, 15f);
                            PlayerController.Instance.SetPivotSideRotation(_leftToeAxis - _rightToeAxis);
                            return;
                        }
                        else
                        {
                            if (SettingsManager.Instance.stance == Stance.Regular)
                            {
                                PlayerController.Instance.SetFrontPivotRotation(_rightToeAxis);
                                PlayerController.Instance.SetBackPivotRotation(_leftToeAxis);
                                PlayerController.Instance.SetPivotForwardRotation((_leftForwardDir + _rightForwardDir) * 0.7f, 15f);
                                PlayerController.Instance.SetPivotSideRotation(_rightToeAxis - _leftToeAxis);
                                return;
                            }
                            PlayerController.Instance.SetFrontPivotRotation(-_rightToeAxis);
                            PlayerController.Instance.SetBackPivotRotation(-_leftToeAxis);
                            PlayerController.Instance.SetPivotForwardRotation((p_leftStick.ForwardDir + p_rightStick.ForwardDir) * 0.7f, 15f);
                            PlayerController.Instance.SetPivotSideRotation(_rightToeAxis - _leftToeAxis);
                            return;
                        }
                        break;
                    default:
                        return;
                }
            }
        }

        public override bool LeftFootOff()
        {
            return !_caughtLeft;
        }

        public override bool RightFootOff()
        {
            return !_caughtRight;
        }

        public override void OnStickPressed(bool p_right)
        {
            if (p_right)
            {
                if (_caughtRight && _caughtBoth)
                {
                    PlayerController.Instance.SetRightIKLerpTarget(1f, 1f);
                    PlayerController.Instance.SetRightSteezeWeight(1f);
                    PlayerController.Instance.SetMaxSteezeRight(1f);
                    PlayerController.Instance.SetRightKneeIKTargetWeight(0.2f);
                    _caughtRight = false;
                    _caughtBoth = false;
                    EventManager.Instance.OnReleased(!_caughtRight, !_caughtLeft);
                    return;
                }
                if (!_caughtRight && _caughtLeft)
                {
                    SoundManager.Instance.PlayCatchSound();
                    PlayerController.Instance.SetRightIKLerpTarget(0f, 0f);
                    PlayerController.Instance.SetRightSteezeWeight(0f);
                    PlayerController.Instance.SetMaxSteezeRight(0f);
                    PlayerController.Instance.SetRightKneeIKTargetWeight(1f);
                    _caughtRight = true;
                    _caughtBoth = true;
                    EventManager.Instance.OnCatched(_caughtRight, _caughtLeft);
                    return;
                }
            }
            else
            {
                if (_caughtLeft && _caughtBoth)
                {
                    PlayerController.Instance.SetLeftIKLerpTarget(1f, 1f);
                    PlayerController.Instance.SetLeftSteezeWeight(1f);
                    PlayerController.Instance.SetMaxSteezeLeft(1f);
                    PlayerController.Instance.SetLeftKneeIKTargetWeight(0.2f);
                    _caughtLeft = false;
                    _caughtBoth = false;
                    EventManager.Instance.OnReleased(!_caughtRight, !_caughtLeft);
                    return;
                }
                if (_caughtRight && !_caughtLeft)
                {
                    SoundManager.Instance.PlayCatchSound();
                    PlayerController.Instance.SetLeftIKLerpTarget(0f, 0f);
                    PlayerController.Instance.SetLeftSteezeWeight(0f);
                    PlayerController.Instance.SetMaxSteezeLeft(0f);
                    PlayerController.Instance.SetLeftKneeIKTargetWeight(1f);
                    _caughtLeft = true;
                    _caughtBoth = true;
                    EventManager.Instance.OnCatched(_caughtRight, _caughtLeft);
                }
            }
        }

        public override void OnStickUpdate(StickInput p_leftStick, StickInput p_rightStick)
        {
            _leftStick = new Vector2(p_leftStick.ToeAxis, p_leftStick.ForwardDir);
            _rightStick = new Vector2(p_rightStick.ToeAxis, p_rightStick.ForwardDir);
            PlayerController.Instance.SetLeftIKOffset(p_leftStick.ToeAxis, p_leftStick.ForwardDir, p_leftStick.PopDir, p_leftStick.IsPopStick, true, false);
            PlayerController.Instance.SetRightIKOffset(p_rightStick.ToeAxis, p_rightStick.ForwardDir, p_rightStick.PopDir, p_rightStick.IsPopStick, true, false);
        }

        public override void OnManualUpdate(StickInput p_popStick, StickInput p_flipStick)
        {
            if (!_wasGrinding)
            {
                PlayerController.Instance.AnimSetManual(true, Mathf.Lerp(PlayerController.Instance.AnimGetManualAxis(), p_popStick.ForwardDir, Time.deltaTime * 10f));
                _manualling = true;
                _noseManualling = false;
                _popStick = p_popStick;
                _flipStick = p_flipStick;
            }
        }

        public override void OnNoseManualUpdate(StickInput p_popStick, StickInput p_flipStick)
        {
            if (!_wasGrinding)
            {
                PlayerController.Instance.AnimSetNoseManual(true, Mathf.Lerp(PlayerController.Instance.AnimGetManualAxis(), p_popStick.ForwardDir, Time.deltaTime * 10f));
                _popStick = p_popStick;
                _flipStick = p_flipStick;
                _noseManualling = true;
                _manualling = false;
            }
        }

        public override void OnManualExit()
        {
            _manualling = false;
            PlayerController.Instance.AnimSetManual(false, PlayerController.Instance.AnimGetManualAxis());
        }

        public override void OnNoseManualExit()
        {
            _noseManualling = false;
            PlayerController.Instance.AnimSetNoseManual(false, PlayerController.Instance.AnimGetManualAxis());
        }

        public override void OnGrindDetected()
        {
            if (!Main.settings.Grinds)
            {
                return;
            }

            if (PlayerController.Instance.WasOnCoping)
            {
                return;
            }
            if (_canGrind && !_bumpOut)
            {
                base.DoTransition(typeof(Custom_Grinding), null);
            }
        }

        public override bool CanGrind()
        {
            if (!Main.settings.Grinds)
            {
                return false;
            }
            return _canGrind;
        }

        public override void OnPredictedCollisionEvent()
        {
            PredictedNextState();
        }

        public override void OnFirstWheelDown()
        {
            if (_wasGrinding)
            {
                SwitchToBoardMaster();
                return;
            }
            TransitionToNextState();
        }

        public override void OnAllWheelsDown()
        {
            TransitionToNextState();
        }

        public override void OnCollisionEnterEvent(Vector3 _impulse, bool _isBoard, Collision c)
        {
            _colliding = true;
            if (_isBoard)
            {
                SoundManager.Instance.PlayBoardHit(_impulse.magnitude);
            }
            if (CanTransitionToNextState(c))
            {
                TransitionToNextState();
            }
        }

        public override void OnCollisionStayEvent(Collision c)
        {
            _colliding = true;
            if (CanTransitionToNextState(c))
            {
                TransitionToNextState();
            }
        }

        private bool CanTransitionToNextState(Collision c)
        {
            return !_wasGrinding && !_bumpOut;
        }

        private void SwitchToBoardMaster()
        {
            PlayerController.Instance.SetBoardToMaster();
            PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
        }

        public override bool IsInImpactState()
        {
            return _predictedCollision;
        }

        private void CatchBoth()
        {
            PlayerController.Instance.SetRightIKLerpTarget(0f, 0f);
            PlayerController.Instance.SetRightSteezeWeight(0f);
            PlayerController.Instance.SetMaxSteezeRight(0f);
            PlayerController.Instance.boardController.ResetAll();
            PlayerController.Instance.SetCatchForwardRotation();
            PlayerController.Instance.SetLeftIKLerpTarget(0f, 0f);
            PlayerController.Instance.SetLeftSteezeWeight(0f);
            PlayerController.Instance.SetMaxSteezeLeft(0f);
            _caughtLeft = true;
            _caughtRight = true;
            _caughtBoth = true;
            PlayerController.Instance.SetRightIKRotationWeight(1f);
            PlayerController.Instance.SetLeftIKRotationWeight(1f);
            PlayerController.Instance.SetMaxSteeze(0f);
        }

        private void PredictedNextState()
        {
            _predictedCollision = true;
            CatchBoth();
            if (!_wasManualling && (_manualling || _noseManualling))
            {
                object[] args = new object[]
                {
                _popStick,
                _flipStick,
                !_noseManualling,
                _inAirTurnDelta
                };
                base.DoTransition(typeof(Custom_Manualling), args);
            }
        }

        private void TransitionToNextState()
        {
            CatchBoth();
            if (!_wasManualling && (_manualling || _noseManualling))
            {
                object[] args = new object[]
                {
                _popStick,
                _flipStick,
                !_noseManualling,
                _inAirTurnDelta
                };
                base.DoTransition(typeof(Custom_Manualling), args);
                return;
            }
            _manualling = false;
            _noseManualling = false;
            PlayerController.Instance.AnimSetManual(false, PlayerController.Instance.AnimGetManualAxis());
            PlayerController.Instance.AnimSetNoseManual(false, PlayerController.Instance.AnimGetManualAxis());
            object[] args2 = new object[]
            {
            _canGrind
            };
            PlayerController.Instance.Impact();
            base.DoTransition(typeof(Custom_Impact), args2);
        }

        public override void OnLBDown()
        {
            if (!Main.settings.Grabs)
            {
                return;
            }
            if (!_delayGrab)
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
            if (!_delayGrab)
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
            if (!_delayGrab)
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
            if (!_delayGrab)
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
    }
}