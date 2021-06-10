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
    public class Custom_Manualling : PlayerState_OnBoard
    {
        private bool _animBegun;
        private bool _canEnterCopingGrind = true;
        private bool _canGrind;
        private bool _canPop;
        private bool _colliding;
        private bool _delayExit;
        private bool _flipDetected;
        private bool _forwardLoad;
        private bool _hasLanded;
        private bool _impacting;
        private bool _manual;
        private bool _popDirEqualsCatchDir;
        private bool _potentialFlip;
        private bool _wasFlipping;
        private bool _wasManuallingBeforeInAir;
        private bool _wasPowersliding;

        private float _animTimer;
        private float _augmentedLeftAngle;
        private float _augmentedRightAngle;
        private float backTruckDampCache;
        private float backTruckSpringCache;
        private float _boardAngleToGround;
        private float _copingTimeLimit = 0.32f;
        private float _copingTimer;
        private float _delayTimer;
        private float _flip;
        private float _flipVel;
        private float _flipWindowTimer;
        private float frontTruckDampCache;
        private float frontTruckSpringCache;
        private float _impactTimer;
        private float _inAirTimer;
        private float _inAirTurnDelta;
        private float _invertVel;
        private float _manualAxis;
        private float _manualStrength;
        private float _popDir;
        private float _popForce = 2.5f;
        private float _popWait;
        private float _popVel;
        private float _powerslideTimer;
        private float _toeAxis;

        private int _flipFrameCount;
        private int _flipFrameMax = 20;
        private int _manualSign = 1;

        private ManualType _manualType;

        private PlayerController.SetupDir _setupDir;

        private StickInput _flipStick;
        private StickInput _popStick;

        private Vector2 _initialFlipDir = Vector2.zero;
        private Vector2 _leftStick = Vector2.zero;
        private Vector2 _rightStick = Vector2.zero;

        public override void SetupDefinition(ref FSMStateType stateType, ref List<Type> children)
        {
            stateType = FSMStateType.Type_OR;
        }

        private ManualStance ManualStance;

        public Custom_Manualling(StickInput p_popStick, StickInput p_flipStick, bool p_manual, float p_inAirTurnDelta)
        {
            _inAirTurnDelta = p_inAirTurnDelta;
            _popStick = p_popStick;
            _flipStick = p_flipStick;
            _manual = p_manual;
            if (!PlayerController.Instance.IsSwitch)
            {
                if (_manual)
                {
                    _manualSign = -1;
                }
                else
                {
                    _manualSign = 1;
                }
            }
            else if (_manual)
            {
                _manualSign = 1;
            }
            else
            {
                _manualSign = -1;
            }
            if (PlayerController.Instance.IsAnimSwitch == PlayerController.Instance.IsSwitch)
            {
                if (!PlayerController.Instance.IsAnimSwitch)
                {
                    if (_manual)
                    {
                        _manualType = ManualType.Manual;
                        ManualStance = ManualStance.Manual;
                        return;
                    }
                    _manualType = ManualType.NoseManual;
                    ManualStance = ManualStance.NoseManual;
                    return;
                }
                else
                {
                    if (_manual)
                    {
                        _manualType = ManualType.Manual;
                        ManualStance = ManualStance.ManualSwitch;
                        return;
                    }
                    _manualType = ManualType.NoseManual;
                    ManualStance = ManualStance.NoseManualSwitch;
                    return;
                }
            }
            else if (!PlayerController.Instance.IsAnimSwitch)
            {
                if (_manual)
                {
                    _manualType = ManualType.NoseManual;
                    ManualStance = ManualStance.NoseManual;
                    return;
                }
                _manualType = ManualType.Manual;
                ManualStance = ManualStance.Manual;
                return;
            }
            else
            {
                if (_manual)
                {
                    _manualType = ManualType.NoseManual;
                    ManualStance = ManualStance.NoseManualSwitch;
                    return;
                }
                _manualType = ManualType.Manual;
                ManualStance = ManualStance.ManualSwitch;
                return;
            }
        }

        public Custom_Manualling(StickInput p_popStick, StickInput p_flipStick, bool p_manual, float p_inAirTurnDelta, int p_state)
        {
            _wasPowersliding = true;
            _inAirTurnDelta = p_inAirTurnDelta;
            _popStick = p_popStick;
            _flipStick = p_flipStick;
            _manual = (_popStick.ForwardDir <= 0f);
            if (!PlayerController.Instance.IsSwitch)
            {
                if (_manual)
                {
                    _manualSign = -1;
                }
                else
                {
                    _manualSign = 1;
                }
            }
            else if (_manual)
            {
                _manualSign = -1;
            }
            else
            {
                _manualSign = 1;
            }
            if (PlayerController.Instance.IsAnimSwitch == PlayerController.Instance.IsSwitch)
            {
                if (!PlayerController.Instance.IsAnimSwitch)
                {
                    if (_manual)
                    {
                        _manualType = ManualType.Manual;
                        ManualStance = ManualStance.Manual;
                        return;
                    }
                    _manualType = ManualType.NoseManual;
                    ManualStance = ManualStance.NoseManual;
                    return;
                }
                else
                {
                    if (_manual)
                    {
                        _manualType = ManualType.Manual;
                        ManualStance = ManualStance.ManualSwitch;
                        return;
                    }
                    _manualType = ManualType.NoseManual;
                    ManualStance = ManualStance.NoseManualSwitch;
                    return;
                }
            }
            else if (!PlayerController.Instance.IsAnimSwitch)
            {
                if (_manual)
                {
                    _manualType = ManualType.NoseManual;
                    ManualStance = ManualStance.NoseManual;
                    return;
                }
                _manualType = ManualType.Manual;
                ManualStance = ManualStance.Manual;
                return;
            }
            else
            {
                if (_manual)
                {
                    _manualType = ManualType.NoseManual;
                    ManualStance = ManualStance.NoseManualSwitch;
                    return;
                }
                _manualType = ManualType.Manual;
                ManualStance = ManualStance.ManualSwitch;
                return;
            }
        }

        public Custom_Manualling(StickInput p_popStick, StickInput p_flipStick, bool p_manual, float p_inAirTurnDelta, bool p_wasManuallingBeforeInAir)
        {
            _inAirTurnDelta = p_inAirTurnDelta;
            _popStick = p_popStick;
            _flipStick = p_flipStick;
            _manual = p_manual;
            _wasManuallingBeforeInAir = p_wasManuallingBeforeInAir;
            if (!PlayerController.Instance.IsSwitch)
            {
                if (_manual)
                {
                    _manualSign = -1;
                }
                else
                {
                    _manualSign = 1;
                }
            }
            else if (_manual)
            {
                _manualSign = 1;
            }
            else
            {
                _manualSign = -1;
            }
            if (PlayerController.Instance.IsAnimSwitch == PlayerController.Instance.IsSwitch)
            {
                if (!PlayerController.Instance.IsAnimSwitch)
                {
                    if (_manual)
                    {
                        _manualType = ManualType.Manual;
                        ManualStance = ManualStance.Manual;
                        return;
                    }
                    _manualType = ManualType.NoseManual;
                    ManualStance = ManualStance.NoseManual;
                    return;
                }
                else
                {
                    if (_manual)
                    {
                        _manualType = ManualType.Manual;
                        ManualStance = ManualStance.ManualSwitch;
                        return;
                    }
                    _manualType = ManualType.NoseManual;
                    ManualStance = ManualStance.NoseManualSwitch;
                    return;
                }
            }
            else if (!PlayerController.Instance.IsAnimSwitch)
            {
                if (_manual)
                {
                    _manualType = ManualType.NoseManual;
                    ManualStance = ManualStance.NoseManual;
                    return;
                }
                _manualType = ManualType.Manual;
                ManualStance = ManualStance.Manual;
                return;
            }
            else
            {
                if (_manual)
                {
                    _manualType = ManualType.NoseManual;
                    ManualStance = ManualStance.NoseManualSwitch;
                    return;
                }
                _manualType = ManualType.Manual;
                ManualStance = ManualStance.ManualSwitch;
                return;
            }
        }

        public Custom_Manualling(StickInput p_popStick, StickInput p_flipStick, bool p_manual, float p_inAirTurnDelta, bool p_wasFlipping, bool p_popDirEqualsCatchDir)
        {
            _popDirEqualsCatchDir = p_popDirEqualsCatchDir;
            _wasFlipping = p_wasFlipping;
            _inAirTurnDelta = p_inAirTurnDelta;
            _popStick = p_popStick;
            _flipStick = p_flipStick;
            _manual = p_manual;
            if (!PlayerController.Instance.IsSwitch)
            {
                if (_manual)
                {
                    _manualSign = -1;
                }
                else
                {
                    _manualSign = 1;
                }
            }
            else if (_manual)
            {
                _manualSign = 1;
            }
            else
            {
                _manualSign = -1;
            }
            if (PlayerController.Instance.IsAnimSwitch == PlayerController.Instance.IsSwitch)
            {
                if (!PlayerController.Instance.IsAnimSwitch)
                {
                    if (_manual)
                    {
                        _manualType = ManualType.Manual;
                        ManualStance = ManualStance.Manual;
                        return;
                    }
                    _manualType = ManualType.NoseManual;
                    ManualStance = ManualStance.NoseManual;
                    return;
                }
                else
                {
                    if (_manual)
                    {
                        _manualType = ManualType.Manual;
                        ManualStance = ManualStance.ManualSwitch;
                        return;
                    }
                    _manualType = ManualType.NoseManual;
                    ManualStance = ManualStance.NoseManualSwitch;
                    return;
                }
            }
            else if (!PlayerController.Instance.IsAnimSwitch)
            {
                if (_manual)
                {
                    _manualType = ManualType.NoseManual;
                    ManualStance = ManualStance.NoseManual;
                    return;
                }
                _manualType = ManualType.Manual;
                ManualStance = ManualStance.Manual;
                return;
            }
            else
            {
                if (_manual)
                {
                    _manualType = ManualType.NoseManual;
                    ManualStance = ManualStance.NoseManualSwitch;
                    return;
                }
                _manualType = ManualType.Manual;
                ManualStance = ManualStance.ManualSwitch;
                return;
            }
        }

        public override void Enter()
        {
            PlayerController.Instance.currentStateEnum = PlayerController.CurrentState.Manual;
            XXLController.CurrentState = CurrentState.Manual;
            PlayerController.Instance.ToggleFlipColliders(false);
            PlayerController.Instance.BoardFreezedAfterRespawn = false;
            if (_colliding || PlayerController.Instance.IsGrounded())
            {
                PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
                PlayerController.Instance.SetBoardToMaster();
            }
            else
            {
                _impacting = true;
            }
            PlayerController.Instance.boardController.boardRigidbody.angularVelocity = Vector3.zero;
            PlayerController.Instance.skaterController.skaterRigidbody.angularVelocity = Vector3.zero;
            PlayerController.Instance.boardController.ResetAll();
            SetManualTruckSprings();
            SetCenterOfMass();
            PlayerController.Instance.SetTurnMultiplier(1f);
            if (PlayerController.Instance.IsGrounded())
            {
                _manualStrength = 1f;
                PlayerController.Instance.SetManualStrength(1f);
            }
            else
            {
                _manualStrength = 0f;
                PlayerController.Instance.SetManualStrength(0f);
            }
            PlayerController.Instance.OnImpact();
            XXLController.ManualType = ManualStance;
            XXLController.Instance.ActivateSlowMotion(Main.settings.SlowMotionManuals, Main.settings.SlowMotionManualSpeed);
        }

        public override void Exit()
        {
            XXLController.Instance.ResetTime(Main.settings.SlowMotionManuals);
            SoundManager.Instance.StopPowerslideSound(1, PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude); ///////////
            if (isBraking)
            {
                PlayerController.Instance.SetBoardPhysicsMaterial(PlayerController.FrictionType.Default);
            }
            PlayerController.Instance.VelocityOnPop = PlayerController.Instance.boardController.boardRigidbody.velocity;
            UnsetManualTruckSprings();
            PlayerController.Instance.AnimSetManual(false, PlayerController.Instance.AnimGetManualAxis());
            PlayerController.Instance.AnimSetNoseManual(false, PlayerController.Instance.AnimGetManualAxis());
            PlayerController.Instance.ResetBoardCenterOfMass();
            PlayerController.Instance.ResetBackTruckCenterOfMass();
            PlayerController.Instance.ResetFrontTruckCenterOfMass();
        }

        public override void Update()
        {
            base.Update();
            AdvancedPop();
            HandleCrouch(); ///////////
            SoundManager.Instance.SetRollingVolumeFromRPS(PlayerController.Instance.GetSurfaceTag(PlayerController.Instance.boardController.GetSurfaceTagString()), PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude);
            if (_canEnterCopingGrind)
            {
                _copingTimer += Time.deltaTime;
                if (_copingTimer >= _copingTimeLimit)
                {
                    _canEnterCopingGrind = false;
                }
            }
            if (_wasPowersliding)
            {
                _powerslideTimer += Time.deltaTime;
                if (_powerslideTimer > 0.7f)
                {
                    _wasPowersliding = false;
                }
            }
            _boardAngleToGround = Vector3.Angle(PlayerController.Instance.boardController.boardTransform.up, PlayerController.Instance.GetGroundNormal());
            _boardAngleToGround *= (float)_manualSign;
            _boardAngleToGround = Mathf.Clamp(_boardAngleToGround, -30f, 30f);
            _boardAngleToGround /= 30f;
            _manualAxis = Mathf.Lerp(_manualAxis, _boardAngleToGround, Time.deltaTime * 10f);
            PlayerController.Instance.AnimSetManualAxis(_manualAxis);
            _manualStrength = Mathf.Clamp(_manualStrength + Time.deltaTime * 2f, 0f, 1f);
            PlayerController.Instance.SetManualStrength(_manualStrength);
            if (!_hasLanded && PlayerController.Instance.IsGrounded())
            {
                EventManager.Instance.EnterManual(_manualType);
            }
            if (PlayerController.Instance.IsGrounded() || _colliding)
            {
                _hasLanded = true;
            }
            if (_hasLanded)
            {
                if (PlayerController.Instance.IsGrounded())
                {
                    if (!_animBegun)
                    {
                        _animTimer += Time.deltaTime;
                        if (_animTimer > 0.1f)
                        {
                            PlayerController.Instance.CrossFadeAnimation("Manual", 0.2f);
                            _animBegun = true;
                        }
                    }
                    _inAirTimer = 0f;
                }
                else
                {
                    _inAirTimer = 0f;
                }
            }
            if (_popWait < 0.2f)
            {
                if (PlayerController.Instance.IsGrounded())
                {
                    _popWait += Time.deltaTime;
                }
            }
            else
            {
                PlayerController.Instance.ResetAfterGrinds();
                _canPop = true;
            }
            if (_delayExit)
            {
                _delayTimer += Time.deltaTime;
                if (_delayTimer > 0.2f)
                {
                    PlayerController.Instance.AnimSetManual(false, PlayerController.Instance.AnimGetManualAxis());
                    PlayerController.Instance.AnimSetNoseManual(false, PlayerController.Instance.AnimGetManualAxis());
                    base.DoTransition(typeof(Custom_Riding), null);
                    return;
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            PlayerController.Instance.ScalePlayerCollider();
            if (_colliding || PlayerController.Instance.IsGrounded())
            {
                if (PlayerController.Instance.movementMaster != PlayerController.MovementMaster.Board)
                {
                    PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
                    PlayerController.Instance.SetBoardToMaster();
                }
                if (_impacting)
                {
                    _impactTimer += Time.deltaTime;
                    if (_impactTimer > (Main.settings.ManualPopDelay ? 1f : 0f))
                    {
                        _impacting = false;
                    }
                }
            }
            PlayerController.Instance.boardController.SetBoardControllerUpVector(PlayerController.Instance.skaterController.skaterTransform.up);
            PlayerController.Instance.comController.UpdateCOM(DefaultCrouchAmount, 0);
            PlayerController.Instance.SetRotationTarget();
            if (PlayerController.Instance.IsGrounded())
            {
                PlayerController.Instance.ApplyFriction();
                PlayerController.Instance.ManualRotation(GetManualTarget(_popStick.ForwardDir <= 0.1f, _popStick.ForwardDir, -_flipStick.PopDir, _flipStick.ToeAxis));
            }
            else
            {
                PlayerController.Instance.SnapRotation();
            }
            PlayerController.Instance.SkaterRotation(true, true);
            PlayerController.Instance.ReduceImpactBounce();
            _colliding = false;
        }

        public Quaternion GetManualTarget(bool p_manual, float p_manualAxis, float p_secondaryAxis, float p_swivel)
        {
            float num = (Mathf.Abs(p_manualAxis) - 0.5f) * 15f + p_secondaryAxis * Main.settings.ManualMaxAngle;
            Vector3 vector = (!PlayerController.Instance.boardController.IsBoardBackwards) ? PlayerController.Instance.boardController.boardTransform.forward : (-PlayerController.Instance.boardController.boardTransform.forward);
            vector = Vector3.ProjectOnPlane(vector, PlayerController.Instance.boardController.LerpedGroundNormal);
            Vector3 vector2 = Vector3.Cross(vector, PlayerController.Instance.boardController.LerpedGroundNormal);
            Vector3 upwards = Quaternion.AngleAxis(15f + num, p_manual ? vector2 : (-vector2)) * PlayerController.Instance.boardController.LerpedGroundNormal;
            Vector3 forward = Quaternion.AngleAxis(15f + num, p_manual ? vector2 : (-vector2)) * vector;
            Vector3 forward2 = Quaternion.AngleAxis(15f + num, p_manual ? vector2 : (-vector2)) * -vector;

            HandleBraking(num);

            if (PlayerController.Instance.boardController.IsBoardBackwards)
            {
                return Quaternion.LookRotation(forward2, upwards);
            }
            return Quaternion.LookRotation(forward, upwards);
        }

        public override void OnStickUpdate(StickInput p_leftStick, StickInput p_rightStick)
        {
            _leftStick = new Vector2(p_leftStick.ToeAxis, p_leftStick.ForwardDir);
            _rightStick = new Vector2(p_rightStick.ToeAxis, p_rightStick.ForwardDir);
        }

        public override void OnStickFixedUpdate(StickInput p_leftStick, StickInput p_rightStick)
        {
            PowerslideCheck(p_leftStick, p_rightStick);
            switch (SettingsManager.Instance.controlType)
            {
                case ControlType.Same:
                    if (SettingsManager.Instance.stance == Stance.Regular)
                    {
                        PlayerController.Instance.SetFrontPivotRotation(p_rightStick.ToeAxis);
                        PlayerController.Instance.SetBackPivotRotation(p_leftStick.ToeAxis);
                        PlayerController.Instance.SetPivotForwardRotation((p_leftStick.ForwardDir + p_rightStick.ForwardDir) * 0.7f, 15f);
                        PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                        return;
                    }
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
                            PlayerController.Instance.SetFrontPivotRotation(p_rightStick.ToeAxis);
                            PlayerController.Instance.SetBackPivotRotation(p_leftStick.ToeAxis);
                            PlayerController.Instance.SetPivotForwardRotation((p_leftStick.ForwardDir + p_rightStick.ForwardDir) * 0.7f, 15f);
                            PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                            return;
                        }
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
                            PlayerController.Instance.SetFrontPivotRotation(p_leftStick.ToeAxis);
                            PlayerController.Instance.SetBackPivotRotation(p_rightStick.ToeAxis);
                            PlayerController.Instance.SetPivotForwardRotation((p_leftStick.ForwardDir + p_rightStick.ForwardDir) * 0.7f, 15f);
                            PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                            return;
                        }
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
                            PlayerController.Instance.SetFrontPivotRotation(p_rightStick.ToeAxis);
                            PlayerController.Instance.SetBackPivotRotation(p_leftStick.ToeAxis);
                            PlayerController.Instance.SetPivotForwardRotation((p_leftStick.ForwardDir + p_rightStick.ForwardDir) * 0.7f, 15f);
                            PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                            return;
                        }
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
                            PlayerController.Instance.SetFrontPivotRotation(p_rightStick.ToeAxis);
                            PlayerController.Instance.SetBackPivotRotation(p_leftStick.ToeAxis);
                            PlayerController.Instance.SetPivotForwardRotation((p_leftStick.ForwardDir + p_rightStick.ForwardDir) * 0.7f, 15f);
                            PlayerController.Instance.SetPivotSideRotation(p_rightStick.ToeAxis - p_leftStick.ToeAxis);
                            return;
                        }
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

        private void PowerslideCheck(StickInput l, StickInput r)
        {
            if (((l.ToeAxis > Main.settings.ManualRevertSensitivity && r.ToeAxis < -Main.settings.ManualRevertSensitivity) || (l.ToeAxis < -Main.settings.ManualRevertSensitivity && r.ToeAxis > Main.settings.ManualRevertSensitivity)) && PlayerController.Instance.boardController.Grounded && ((l.ToeAxis > Main.settings.ManualRevertSensitivity && r.ToeAxis < -Main.settings.ManualRevertSensitivity && ((l.ForwardDir > Main.settings.ManualRevertSensitivity && r.ForwardDir > Main.settings.ManualRevertSensitivity && PlayerController.Instance.boardController.FrontTwoDown) || (l.ForwardDir < -Main.settings.ManualRevertSensitivity && r.ForwardDir < -Main.settings.ManualRevertSensitivity && PlayerController.Instance.boardController.BackTwoDown))) || (l.ToeAxis < -Main.settings.ManualRevertSensitivity && r.ToeAxis > Main.settings.ManualRevertSensitivity && ((l.ForwardDir > Main.settings.ManualRevertSensitivity && r.ForwardDir > Main.settings.ManualRevertSensitivity && PlayerController.Instance.boardController.FrontTwoDown) || (l.ForwardDir < -Main.settings.ManualRevertSensitivity && r.ForwardDir < -Main.settings.ManualRevertSensitivity && PlayerController.Instance.boardController.BackTwoDown)))) && !_wasPowersliding)
            {
                object[] args = new object[]
                {
                4
                };
                base.DoTransition(typeof(Custom_Powerslide), args);
                return;
            }
        }

        private void TransitionToInAir()
        {
            PlayerController.Instance.OnImpact();
            PlayerController.Instance.skaterController.skaterRigidbody.angularVelocity = Vector3.zero;
            PlayerController.Instance.AnimSetManual(false, PlayerController.Instance.AnimGetManualAxis());
            PlayerController.Instance.AnimSetNoseManual(false, PlayerController.Instance.AnimGetManualAxis());
            PlayerController.Instance.CrossFadeAnimation("Extend", 0.3f);
            PlayerController.Instance.OnExtendAnimEnter();
            PlayerController.Instance.SetTurningMode(InputController.TurningMode.InAir);
            PlayerController.Instance.SetSkaterToMaster();
            Vector3 force = PlayerController.Instance.skaterController.PredictLandingLight(PlayerController.Instance.skaterController.skaterRigidbody.velocity, 0.2f);
            PlayerController.Instance.skaterController.skaterRigidbody.AddForce(force, ForceMode.Impulse);
            SkaterXL.Core.PopType popType = PlayerController.Instance.IsSwitch ? (_manual ? SkaterXL.Core.PopType.Ollie : SkaterXL.Core.PopType.Nollie) : (_manual ? SkaterXL.Core.PopType.Switch : SkaterXL.Core.PopType.Fakie);
            EventManager.Instance.EnterAir(popType, 0f);
            object[] args = new object[]
            {
            false,
            false,
            0f,
            _canPop
            };
            base.DoTransition(typeof(Custom_InAir), args);
        }

        private void SetManualTruckSprings()
        {
            frontTruckSpringCache = PlayerController.Instance.boardController.frontTruckJoint.angularXDrive.positionSpring;
            frontTruckDampCache = PlayerController.Instance.boardController.frontTruckJoint.angularXDrive.positionDamper;
            backTruckSpringCache = PlayerController.Instance.boardController.backTruckJoint.angularXDrive.positionSpring;
            backTruckDampCache = PlayerController.Instance.boardController.backTruckJoint.angularXDrive.positionDamper;
            JointDrive angularXDrive = default(JointDrive);
            angularXDrive.positionDamper = 1f;
            angularXDrive.positionSpring = 20f;
            angularXDrive.maximumForce = PlayerController.Instance.boardController.frontTruckJoint.angularXDrive.maximumForce;
            PlayerController.Instance.boardController.frontTruckJoint.angularXDrive = angularXDrive;
            PlayerController.Instance.boardController.backTruckJoint.angularXDrive = angularXDrive;
        }

        private void UnsetManualTruckSprings()
        {
            JointDrive angularXDrive = default(JointDrive);
            angularXDrive.positionDamper = frontTruckDampCache;
            angularXDrive.positionSpring = frontTruckSpringCache;
            angularXDrive.maximumForce = PlayerController.Instance.boardController.frontTruckJoint.angularXDrive.maximumForce;
            PlayerController.Instance.boardController.frontTruckJoint.angularXDrive = angularXDrive;
            angularXDrive.positionDamper = backTruckDampCache;
            angularXDrive.positionSpring = backTruckSpringCache;
            angularXDrive.maximumForce = PlayerController.Instance.boardController.backTruckJoint.angularXDrive.maximumForce;
            PlayerController.Instance.boardController.backTruckJoint.angularXDrive = angularXDrive;
        }

        private void SetCenterOfMass()
        {
            _manual = ((!PlayerController.Instance.GetBoardBackwards()) ? (_popStick.ForwardDir <= 0f) : (_popStick.ForwardDir > 0f));
            Vector3 position = _manual ? PlayerController.Instance.boardController.backTruckCoM.position : PlayerController.Instance.boardController.frontTruckCoM.position;
            PlayerController.Instance.SetBoardCenterOfMass(PlayerController.Instance.boardController.boardTransform.InverseTransformPoint(position));
            PlayerController.Instance.SetBackTruckCenterOfMass(PlayerController.Instance.boardController.backTruckRigidbody.transform.InverseTransformPoint(position));
            PlayerController.Instance.SetFrontTruckCenterOfMass(PlayerController.Instance.boardController.frontTruckRigidbody.transform.InverseTransformPoint(position));
        }

        public override void OnPopStickUpdate()
        {
            if (_canPop && PlayerController.Instance.IsGrounded())
            {
                _forwardLoad = (PlayerController.Instance.GetNollie(_popStick.IsRightStick) > 0.1f);
                PlayerController.Instance.AnimSetNollie(PlayerController.Instance.GetNollie(_popStick.IsRightStick));
                PlayerController.Instance.OnPopStartCheck(true, _popStick, ref _setupDir, _forwardLoad, 10f, ref _invertVel, 0f, ref _popVel, false);
            }
        }

        public override void OnNextState()
        {
            _popForce = Main.settings.ManualPopForce;
            PlayerController.Instance.AnimSetPopStrength(0f);
            PlayerController.Instance.boardController.ReferenceBoardRotation();
            PlayerController.Instance.SetTurnMultiplier(1.2f);
            PlayerController.Instance.SetTurningMode(InputController.TurningMode.InAir);
            PlayerController.Instance.FixTargetNormal();
            PlayerController.Instance.SetTargetToMaster();
            PlayerController.Instance.AnimOllieTransition(true);
            if (_potentialFlip)
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
                _popForce,
                false,
                _forwardLoad,
                _invertVel,
                _setupDir,
                _augmentedLeftAngle,
                _augmentedRightAngle,
                true
                };
                base.DoTransition(typeof(Custom_BeginPop), args);
                return;
            }
            object[] args2 = new object[]
            {
            _popStick,
            _flipStick,
            _popForce,
            false,
            _forwardLoad,
            _invertVel,
            _setupDir,
            _augmentedLeftAngle,
            _augmentedRightAngle,
            _popVel,
            _toeAxis,
            _popDir,
            true
            };
            base.DoTransition(typeof(Custom_BeginPop), args2);
        }

        public override void OnFlipStickUpdate()
        {
            if (_canPop && PlayerController.Instance.IsGrounded())
            {
                PlayerController.Instance.OnFlipStickUpdate(ref _flipDetected, ref _potentialFlip, ref _initialFlipDir, ref _flipFrameCount, ref _flipFrameMax, ref _toeAxis, ref _flipVel, ref _popVel, ref _popDir, ref _flip, _flipStick, false, true, ref _invertVel, _popStick.IsRightStick ? _augmentedLeftAngle : _augmentedRightAngle, false, _forwardLoad, ref _flipWindowTimer);
            }
        }

        public override void OnCollisionStayEvent(Collision c)
        {
            _colliding = true;
        }

        public override bool IsOnGroundState()
        {
            return true;
        }

        public override StickInput GetPopStick()
        {
            return _popStick;
        }

        public override void OnPredictedCollisionEvent()
        {
            if (Vector3.Angle(Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardTransform.up, PlayerController.Instance.skaterController.skaterTransform.forward), PlayerController.Instance.skaterController.skaterTransform.up) > 90f)
            {
                PlayerController.Instance.ForceBail();
            }
            PlayerController.Instance.ResetAllAnimations();
            PlayerController.Instance.SetBoardToMaster();
            PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
        }

        public override void OnCollisionEnterEvent(Vector3 _impulse, bool _isBoard, Collision c)
        {
            if (_isBoard)
            {
                SoundManager.Instance.PlayBoardHit(_impulse.magnitude);
            }
            _colliding = true;
            if (Vector3.Angle(Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardTransform.up, PlayerController.Instance.skaterController.skaterTransform.forward), PlayerController.Instance.skaterController.skaterTransform.up) > 90f)
            {
                PlayerController.Instance.ForceBail();
            }
            PlayerController.Instance.ResetAllAnimations();
            PlayerController.Instance.SetBoardToMaster();
            PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
        }

        public override void OnImpactUpdate()
        {
        }

        public override void OnWheelsLeftGround()
        {
        }

        public override void OnManualExit()
        {
            ExitManual(true);
        }

        public override void OnNoseManualExit()
        {
            ExitManual(false);
        }

        private void ExitManual(bool _manual)
        {
            if (!PlayerController.Instance.IsGrounded() && !_colliding)
            {
                TransitionToInAir();
                return;
            }
            if (_manual)
            {
                PlayerController.Instance.AnimSetManual(false, PlayerController.Instance.AnimGetManualAxis());
            }
            else
            {
                PlayerController.Instance.AnimSetNoseManual(false, PlayerController.Instance.AnimGetManualAxis());
            }
            PlayerController.Instance.CrossFadeAnimation("Riding", 0.35f);
            if (_impacting)
            {
                object[] args = new object[]
                {
                4
                };
                base.DoTransition(typeof(Custom_Riding), args);
                return;
            }
            base.DoTransition(typeof(Custom_Riding), null);
        }

        public override bool IgnoreManualExitAttempt()
        {
            return _wasPowersliding && _popStick.PopToeVector.magnitude > 0.2f;
        }

        public override void BothTriggersReleased(InputController.TurningMode p_turningMode)
        {
            PlayerController.Instance.RemoveTurnTorque(0.3f, p_turningMode);
        }

        public override void OnFirstWheelDown()
        {
        }

        public override void OnCopingCheck(SplineComputer _spline)
        {
            PlayerController.Instance.CheckForCopingGrind(_spline, _leftStick, _rightStick);
        }

        public override void TransitionToEnterCopingState(Transform _targetPos, Quaternion _targetRot, SplineComputer _spline, Vector3 _grindDir)
        {
            if (!Main.settings.Grinds)
            {
                return;
            }

            if (PlayerController.Instance.WasOnCoping)
            {
                return;
            }
            object[] args = new object[]
            {
            _targetPos,
            _targetRot,
            _spline,
            _grindDir
            };
            base.DoTransition(typeof(Custom_EnterCoping), args);
        }

        public override void OnGrindDetected()
        {
            if (!Main.settings.Grinds)
            {
                return;
            }

            if (!_canPop)
            {
                base.DoTransition(typeof(Custom_Grinding), null);
                return;
            }
        }

        public override bool CanGrind()
        {
            if (!Main.settings.Grinds)
            {
                return false;
            }
            return !_canPop;
        }

        public override bool CanRideIntoGrinds()
        {
            if (!Main.settings.Grinds)
            {
                return false;
            }
            return !_canPop;
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

        float DefaultCrouchAmount = 1f;
        bool isBraking;

        private void HandleBraking(float manualAngle)
        {
            if (!Main.settings.ManualBraking)
            {
                return;
            }

            if (manualAngle > 16f)
            {
                isBraking = true;
                PlayerController.Instance.SetBoardPhysicsMaterial(PlayerController.FrictionType.Powerslide);
            }
            else
            {
                isBraking = false;
                PlayerController.Instance.SetBoardPhysicsMaterial(PlayerController.FrictionType.Default);
            }

            if (PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude > 0.3f && isBraking)
            {
                SoundManager.Instance.PlayPowerslideSound(PlayerController.Instance.GetSurfaceTag("Surface_Concrete"), PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude, 1);
            }
            else
            {
                SoundManager.Instance.StopPowerslideSound(1, PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude);
            }
        }

        private void HandleCrouch()
        {
            switch (Main.settings.ManualCrouchMode)
            {
                case CrouchMode.Auto:
                    DefaultCrouchAmount = Mathf.MoveTowards(DefaultCrouchAmount, Main.settings.ManualCrouchAmount, 1f);
                    break;
                case CrouchMode.LB:
                    if (PlayerController.Instance.inputController.player.GetButton("LB"))
                    {
                        DefaultCrouchAmount = Mathf.MoveTowards(DefaultCrouchAmount, Main.settings.ManualCrouchAmount, 1f);
                        return;
                    }
                    DefaultCrouchAmount = Mathf.MoveTowards(DefaultCrouchAmount, 1f, 0.5f);
                    break;
                case CrouchMode.RB:
                    if (PlayerController.Instance.inputController.player.GetButton("RB"))
                    {
                        DefaultCrouchAmount = Mathf.MoveTowards(DefaultCrouchAmount, Main.settings.ManualCrouchAmount, 0.5f);
                        return;
                    }
                    DefaultCrouchAmount = Mathf.MoveTowards(DefaultCrouchAmount, 1f, 0.5f);
                    break;
                case CrouchMode.Off:
                    break;
            }
        }

        private void AdvancedPop()
        {
            if (Main.enabled && Main.settings.AdvancedPop == Core.AdvancedPop.Bumper)
            {
                if (PlayerController.Instance.inputController.player.GetButtonDown("LB") && !PlayerController.Instance.inputController.player.GetButtonDown("RB"))
                {
                    XXLController.Instance.LeftPop = !XXLController.Instance.LeftPop;
                    XXLController.Instance.RightPop = false;
                }
                else if (PlayerController.Instance.inputController.player.GetButtonDown("RB") && !PlayerController.Instance.inputController.player.GetButtonDown("LB"))
                {
                    XXLController.Instance.RightPop = !XXLController.Instance.RightPop;
                    XXLController.Instance.LeftPop = false;
                }
            }

            if (Main.enabled && Main.settings.AdvancedPop == Core.AdvancedPop.Sticks)
            {
                if (PlayerController.Instance.inputController.player.GetButtonDown("Left Stick Button") && !PlayerController.Instance.inputController.player.GetButtonDown("Right Stick Button"))
                {
                    XXLController.Instance.LeftPop = !XXLController.Instance.LeftPop;
                    XXLController.Instance.RightPop = false;
                }
                else if (PlayerController.Instance.inputController.player.GetButtonDown("Right Stick Button") && !PlayerController.Instance.inputController.player.GetButtonDown("Left Stick Button"))
                {
                    XXLController.Instance.RightPop = !XXLController.Instance.RightPop;
                    XXLController.Instance.LeftPop = false;
                }
            }
        }


        public override void OnStickPressed(bool right)
        {
            BabyPop(right);
        }

        private void BabyPop(bool right)
        {
            if (!Main.settings.ManualBabyPop)
            {
                return;
            }

            _forwardLoad = (PlayerController.Instance.GetNollie(_popStick.IsRightStick) > 0.1f);
            PlayerController.Instance.skaterController.InitializeSkateRotation();
            PlayerController.Instance.SetTurningMode(InputController.TurningMode.InAir);
            PlayerController.Instance.SetSkaterToMaster();

            float num = Main.settings.ManualBabyPopForce;
            EventManager.Instance.EnterAir();
            object[] args = new object[]
                {
                _popStick,
                _flipStick,
                num,
                _forwardLoad,
                _invertVel,
                _setupDir,
                _augmentedLeftAngle,
                _augmentedRightAngle,
                _popVel,
                _toeAxis,
                _popDir
                };
            this.DoTransition(typeof(Custom_BeginPop), args);
        }
    }
}