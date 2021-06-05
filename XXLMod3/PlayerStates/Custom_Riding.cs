using Dreamteck.Splines;
using FSMHelper;
using System;
using System.Collections.Generic;
using UnityEngine;
using XXLMod3.Controller;
using XXLMod3.Core;

namespace XXLMod3.PlayerStates
{
    public class Custom_Riding : PlayerState_OnBoard
    {
        private bool _bumpAnimIsPlaying;
        private bool _bumpOutToRiding;
        private bool _canEnterCopingGrind = true;
        private bool _canGrind;
        private bool _canPumpLeftStick;
        private bool _canPumpRightStick;
        private bool _centerOfMassChanged;
        private bool _colliding;
        private bool _firstFrame = true;
        private bool _footCollidersDisabled;
        private bool _impactFromManual;
        private bool _lockInAirFromPushing;
        private bool _lockPushing;
        private bool _lockSetupFromPowerslide;
        private bool _lockTurningFromPowerslide;
        private bool _pivoting;
        private bool _powerslideActionAttempt;
        private bool _pumping;
        private bool _setIkLerpSpeed;
        private bool _startCopingTimer;
        private bool _sticksCentered;
        private bool _switchPivot;
        private bool _wasAutoReverting;
        private bool _wasGrinding;
        private bool _wasOnCoping;
        private bool _wasPowersliding;
        private bool _wasReverting;

        private float _animPivot;
        private float _animPivotTurn;
        private float _animPivotTurnSignLerp;
        private float _animPivotTurnTarget;
        private float _animSwitchPivot;
        private float backTruckDampCache;
        private float backTruckSpringCache;
        private float _bumpOutTimer;
        private float _bumpOutTimerMax = 0.3f;
        private float _copingTimeLimit = 0.32f;
        private float _copingTimeout;
        private float _copingTimer;
        private float _currentSine;
        private float _difSine;
        private float frontTruckDampCache;
        private float frontTruckSpringCache;
        private float _footColliderTimer;
        private float _inAirTimer;
        private float _leftTriggerValue;
        private float _lockInAirFromPushingTimer;
        private float _lockPushTimer;
        private float _lockTimer;
        private float _manualImpactTimer;
        private float _pivotTurnLerp;
        private float _prevPumpDelta;
        private float _prevSine;
        private float _pumpCom = 1.04196f;
        private float _pumpForce = 0.45f;
        private float _pumpLeft;
        private float _pumpMultiplier;
        private float _pumpResetTime = 0.5f;
        private float _pumpRight;
        private float _pumpTarget = 1.04196f;
        private float _pumpTargetLow = 0.7f;
        private float _pumpTargetHigh = 1.04196f;
        private float _pumpTimer;
        private float _revertTimer;
        private float _setupLockTimer;
        private float _rightTriggerValue;
        private float _timeInState;

        private PlayerController.SetupDir _setupDir;

        private Vector2 _leftStick = Vector2.zero;
        private Vector2 _rightStick = Vector2.zero;

        private Vector3 _cachedForward;
        private Vector3 _cachedRight;

        public override void SetupDefinition(ref FSMStateType stateType, ref List<Type> children)
        {
            stateType = FSMStateType.Type_OR;
        }

        public Custom_Riding()
        {
            Init();
        }

        public Custom_Riding(int p_state)
        {
            if (p_state != 4)
            {
                switch (p_state)
                {
                    case 13:
                        _lockPushing = true;
                        _lockInAirFromPushing = true;
                        break;
                    case 18:
                        _wasReverting = true;
                        _wasPowersliding = true;
                        _lockTurningFromPowerslide = true;
                        break;
                    case 19:
                        _wasReverting = true;
                        _wasPowersliding = true;
                        _lockTurningFromPowerslide = true;
                        _lockSetupFromPowerslide = true;
                        break;
                    case 20:
                        _wasReverting = true;
                        _wasAutoReverting = true;
                        _wasPowersliding = true;
                        _lockTurningFromPowerslide = true;
                        break;
                    case 21:
                        _bumpOutToRiding = true;
                        break;
                    case 22:
                        _wasGrinding = true;
                        _wasOnCoping = true;
                        Init();
                        break;
                }
            }
            else
            {
                _impactFromManual = true;
            }
            Init();
        }

        public Custom_Riding(bool p_wasGrinding)
        {
            _wasGrinding = p_wasGrinding;
            Init();
        }

        public Custom_Riding(bool p_isPumping, float p_pumpTime, float p_pumpMultiplier, float p_prevPumpDelta)
        {
            _pumping = p_isPumping;
            _pumpTimer = p_pumpTime;
            _pumpMultiplier = p_pumpMultiplier;
            _prevPumpDelta = p_prevPumpDelta;
            Init();
        }

        private void Init()
        {
            PlayerController.Instance.animationController.ScaleAnimSpeed(1f);
            PlayerController.Instance.SetTurnMultiplier(1f);
            PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
            if (PlayerController.Instance.movementMaster != PlayerController.MovementMaster.Board)
            {
                PlayerController.Instance.movementMaster = PlayerController.MovementMaster.Board;
            }
        }

        public override void Enter()
        {
            PlayerController.Instance.currentStateEnum = PlayerController.CurrentState.Riding;
            XXLController.CurrentState = CurrentState.Riding;
            XXLController.Instance.ResetTime(true);
            PlayerController.Instance.cameraController.NeedToSlowLerpCamera = false;
            PlayerController.Instance.AnimSetRollOff(false);
            EventManager.Instance.EndTrickCombo(false, false);
            PlayerController.Instance.ToggleFlipColliders(false);
            SoundManager.Instance.PlayShoeMovementSound();
            PlayerController.Instance.ResetBoardCenterOfMass();
            PlayerController.Instance.ResetBackTruckCenterOfMass();
            PlayerController.Instance.ResetFrontTruckCenterOfMass();
            if (PlayerController.Instance.skaterController.rightFootCollider.isTrigger || PlayerController.Instance.skaterController.leftFootCollider.isTrigger)
            {
                _footCollidersDisabled = true;
            }
            PlayerController.Instance.skaterController.InitializeSkateRotation();
        }

        public override void Update()
        {
            base.Update();
            _timeInState += Time.deltaTime;
            SoundManager.Instance.SetRollingVolumeFromRPS(PlayerController.Instance.GetSurfaceTag(PlayerController.Instance.boardController.GetSurfaceTagString()), PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude);
            if (PlayerController.Instance.boardController.AllDown)
            {
                PlayerController.Instance.CacheRidingTransforms();
            }
            PlayerController.Instance.UpdateBoardAngle();
            Timers();
            UpdatePivotAnimValues();
            if (PlayerController.Instance.boardController.AllDown)
            {
                _switchPivot = PlayerController.Instance.IsSwitch;
            }
            if (!_setIkLerpSpeed && PlayerController.Instance.IsCurrentAnimationPlaying("Riding"))
            {
                PlayerController.Instance.SetIKLerpSpeed(8f);
                _setIkLerpSpeed = true;
            }
            if (!PlayerController.Instance.IsGrounded() && !PlayerController.Instance.IsRespawning && !_colliding && !PlayerController.Instance.playerSM.IsInState(typeof(PlayerState_Tutorial)))
            {
                _inAirTimer += Time.deltaTime;
                PlayerController.Instance.SetTurningMode(InputController.TurningMode.InAir);
                PlayerController.Instance.SetSkaterToMaster();
                PlayerController.Instance.AnimSetRollOff(true);
                PlayerController.Instance.CrossFadeAnimation("Extend", 0.3f);
                PlayerController.Instance.skaterController.skaterRigidbody.angularVelocity = Vector3.zero;
                if (!_lockInAirFromPushing)
                {
                    PlayerController.Instance.OnExtendAnimEnter();
                }
                else
                {
                    PlayerController.Instance.ikController.ForceLeftLerpValue(0f);
                    PlayerController.Instance.ikController.ForceRightLerpValue(0f);
                    PlayerController.Instance.OnExtendAnimEnter(true);
                }
                Vector3 force = PlayerController.Instance.skaterController.PredictLanding(PlayerController.Instance.skaterController.skaterRigidbody.velocity);
                PlayerController.Instance.skaterController.skaterRigidbody.AddForce(force, ForceMode.Impulse);
                EventManager.Instance.EnterAir();
                object[] args = new object[]
                {
                false,
                false,
                0f
                };
                base.DoTransition(typeof(Custom_InAir), args);
                return;
            }
            if (PlayerController.Instance.IsRespawning && !PlayerController.Instance.IsCurrentAnimationPlaying("Riding"))
            {
                PlayerController.Instance.animationController.ForceAnimation("Riding");
            }
            _inAirTimer = 0f;
            if (PlayerController.Instance.IsCurrentAnimationPlaying("Falling"))
            {
                PlayerController.Instance.animationController.ForceAnimation("Riding");
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            PlayerController.AutoRampTags autoRampTags = PlayerController.Instance.AutoPumpAndRevertCheck();
            if (autoRampTags == PlayerController.AutoRampTags.AutoPumpAndRevert)
            {
                PlayerController.Instance.AutoPump();
            }
            PlayerController.Instance.ScalePlayerCollider();
            PlayerController.Instance.ApplyFriction();
            PlayerController.Instance.boardController.SetBoardControllerUpVector(PlayerController.Instance.skaterController.skaterTransform.up);
            PlayerController.Instance.SetRotationTarget();
            PlayerController.Instance.LimitAngularVelocity(5f);
            PlayerController.Instance.SkaterRotation(true, false);
            PlayerController.Instance.boardController.ApplyOnBoardMaxRoll(_colliding, 80f);
            PlayerController.Instance.boardController.DoBoardLean();
            if (_wasReverting || _impactFromManual)
            {
                PlayerController.Instance.comController.UpdateCOM(1.04196f, 1);
            }
            else
            {
                PlayerController.Instance.comController.UpdateCOM(_pumpCom - (PlayerController.Instance.IsGrounded() ? 0f : 0.125f), 0);
            }
            if ((!_colliding && !_firstFrame) || !PlayerController.Instance.IsGrounded())
            {
                PlayerController.Instance.SnapRotation(PlayerController.Instance.boardController.currentRotationTarget, 0.7f, 1f, 0.7f);
            }
            if (_firstFrame)
            {
                _firstFrame = false;
            }
            _colliding = false;
        }

        private void Timers()
        {
            if (_bumpOutToRiding)
            {
                _bumpOutTimer += Time.deltaTime;
                if (_bumpOutTimer > 1f)
                {
                    _bumpOutToRiding = false;
                }
            }
            if (_lockInAirFromPushing)
            {
                _lockInAirFromPushingTimer += Time.deltaTime;
                if (_lockInAirFromPushingTimer > 0.5f)
                {
                    _lockInAirFromPushing = false;
                }
            }
            if (_lockPushing)
            {
                _lockPushTimer += Time.deltaTime;
                if (_lockPushTimer > 0.5f || (!PlayerController.Instance.inputController.player.GetButtonDown(0) && !PlayerController.Instance.inputController.player.GetButtonDown(3)))
                {
                    _lockPushing = false;
                }
            }
            if (_impactFromManual)
            {
                _manualImpactTimer += Time.deltaTime;
                if (_manualImpactTimer > 0.5f)
                {
                    _impactFromManual = false;
                }
            }
            if (_wasReverting)
            {
                _revertTimer += Time.deltaTime;
                if (_revertTimer > 1f)
                {
                    _wasReverting = false;
                }
            }
            if (_wasOnCoping)
            {
                _copingTimeout += Time.deltaTime;
                if (_copingTimeout > 1f)
                {
                    _wasOnCoping = false;
                    _copingTimeout = 0f;
                }
            }
            if (_startCopingTimer)
            {
                _copingTimer += Time.deltaTime;
                if (_copingTimer >= _copingTimeLimit)
                {
                    _startCopingTimer = false;
                }
            }
            if (_footCollidersDisabled)
            {
                _footColliderTimer += Time.deltaTime;
                if (_footColliderTimer > 0.3f)
                {
                    PlayerController.Instance.skaterController.rightFootCollider.isTrigger = false;
                    PlayerController.Instance.skaterController.leftFootCollider.isTrigger = false;
                    _footCollidersDisabled = false;
                }
            }
            if (_lockTurningFromPowerslide)
            {
                _lockTimer += Time.deltaTime;
                if (_lockTimer >= 0.15f)
                {
                    _lockTurningFromPowerslide = false;
                    if (_wasAutoReverting)
                    {
                        _wasPowersliding = false;
                    }
                }
            }
            if (_lockSetupFromPowerslide)
            {
                _setupLockTimer += Time.deltaTime;
                if (_setupLockTimer >= 0.3f)
                {
                    _lockSetupFromPowerslide = false;
                }
            }
        }

        public override bool WasReverting()
        {
            return _wasReverting;
        }

        public override void OnCollisionStayEvent(Collision c)
        {
            _colliding = true;
        }

        public override void OnCollisionEnterEvent(Vector3 _impulse, bool _isBoard, Collision c)
        {
            if (_isBoard)
            {
                SoundManager.Instance.PlayBoardHit(_impulse.magnitude);
            }
            _colliding = true;
            float num = Mathf.Abs(Vector3.Dot(c.impulse.normalized, PlayerController.Instance.skaterController.skaterTransform.forward));
            if (_impulse.magnitude > 1f && !_bumpAnimIsPlaying && num > 0.7f)
            {
                PlayerController.Instance.animationController.SetValue("BumpImpact", Mathf.Clamp(_impulse.magnitude, 1f, 3f));
                PlayerController.Instance.CrossFadeAnimation("Bump", 0.1f);
                _bumpAnimIsPlaying = true;
            }
        }

        public override void Exit()
        {
            XXLController.Instance.ResetFlips();
            PlayerController.Instance.VelocityOnPop = PlayerController.Instance.boardController.boardRigidbody.velocity;
            PlayerController.Instance.animationController.SetValue("Pumping", false);
            if (_centerOfMassChanged)
            {
                ResetCenterOfMass();
            }
        }

        private void UpdatePivotAnimValues()
        {
            _animPivot = Mathf.MoveTowards(_animPivot, _pivoting ? 1f : 0f, Time.deltaTime * (_pivoting ? 5f : 2f));
            _animSwitchPivot = Mathf.MoveTowards(_animSwitchPivot, _switchPivot ? 1f : 0f, Time.deltaTime * (_pivoting ? 5f : 2f));
            _animPivotTurnTarget = _rightTriggerValue - _leftTriggerValue;
            _animPivotTurnSignLerp = Mathf.MoveTowards(_animPivotTurnSignLerp, _animPivotTurnTarget, Time.deltaTime * 20f);
            float target = Mathf.Clamp(PlayerController.Instance.boardController.boardTransform.InverseTransformDirection(PlayerController.Instance.boardController.boardRigidbody.angularVelocity).y, -3f, 3f) / 3f;
            _pivotTurnLerp = Mathf.MoveTowards(_pivotTurnLerp, target, Time.deltaTime * 3f);
            _animPivotTurn = _pivotTurnLerp;
            PlayerController.Instance.animationController.SetValue("Pivot", _animPivot);
            PlayerController.Instance.animationController.SetValue("SwitchPivot", _animSwitchPivot);
            PlayerController.Instance.animationController.SetValue("PivotTurn", _animPivotTurn);
            PlayerController.Instance.SetIKLerpSpeed(8f);
        }

        public override void OnWheelsLeftGround()
        {
        }

        public override void OnPushButtonPressed(bool p_mongo)
        {
            if (_lockPushing || (PlayerController.Instance.inputController.player.GetButtonDown(0) && PlayerController.Instance.inputController.player.GetButtonDown(3)))
            {
                return;
            }
            if (!PlayerController.Instance.IsRespawning)
            {
                PlayerController.Instance.CacheRidingTransforms();
                object[] args = new object[]
                {
                p_mongo
                };
                base.DoTransition(typeof(Custom_Pushing), args);
                return;
            }
        }

        public override void OnPushButtonHeld(bool p_mongo)
        {
            if (_lockPushing || (PlayerController.Instance.inputController.player.GetButtonDown(0) && PlayerController.Instance.inputController.player.GetButtonDown(3)))
            {
                return;
            }
            if (!PlayerController.Instance.IsRespawning)
            {
                PlayerController.Instance.CacheRidingTransforms();
                object[] args = new object[]
                {
                p_mongo
                };
                base.DoTransition(typeof(Custom_Pushing), args);
                return;
            }
        }

        public override void OnBrakePressed()
        {
            if (!PlayerController.Instance.LockBrakes)
            {
                PlayerController.Instance.AnimSetBraking(true);
                object[] args = new object[]
                {
                _pumpCom - (PlayerController.Instance.IsGrounded() ? 0f : 0.125f)
                };
                base.DoTransition(typeof(Custom_Braking), args);
            }
        }

        public override void OnBrakeHeld()
        {
            if (!PlayerController.Instance.LockBrakes)
            {
                PlayerController.Instance.AnimSetBraking(true);
                object[] args = new object[]
                {
                _pumpCom - (PlayerController.Instance.IsGrounded() ? 0f : 0.125f)
                };
                base.DoTransition(typeof(Custom_Braking), args);
            }
        }

        public override void OnStickUpdate(StickInput p_leftStick, StickInput p_rightStick)
        {
            if (PlayerController.Instance.IsExitingPinState || PlayerController.Instance.respawn.respawning)
            {
                return;
            }
            _leftStick = new Vector2(p_leftStick.ToeAxis, p_leftStick.ForwardDir);
            _rightStick = new Vector2(p_rightStick.ToeAxis, p_rightStick.ForwardDir);
            if (_leftStick.magnitude < 0.1f && _rightStick.magnitude < 0.1f)
            {
                _sticksCentered = true;
                _startCopingTimer = false;
            }
            else
            {
                if (_sticksCentered)
                {
                    if (!_startCopingTimer)
                    {
                        _copingTimer = 0f;
                    }
                    _startCopingTimer = true;
                }
                _sticksCentered = false;
            }
            PowerslideCheck(p_leftStick, p_rightStick);
            SetupCheck(p_leftStick, p_rightStick);
            PumpingCheck(p_leftStick, p_rightStick);
            if (!_lockTurningFromPowerslide)
            {
                PlayerController.Instance.SetLeftIKOffset(p_leftStick.ToeAxis, p_leftStick.ForwardDir, p_leftStick.PopDir, p_leftStick.IsPopStick, false, false);
                PlayerController.Instance.SetRightIKOffset(p_rightStick.ToeAxis, p_rightStick.ForwardDir, p_rightStick.PopDir, p_rightStick.IsPopStick, false, false);
            }
            PlayerController.Instance.SetBoardTargetPosition(0f);
            PlayerController.Instance.SetFrontPivotRotation(0f);
            PlayerController.Instance.SetBackPivotRotation(0f);
            PlayerController.Instance.SetPivotForwardRotation(0f, 40f);
            PlayerController.Instance.SetPivotSideRotation(0f);
        }

        private void SetCenterOfMass()
        {
            _pivoting = true;
            SetManualTruckSprings();
            bool flag = !PlayerController.Instance.GetBoardBackwards();
            if (_switchPivot)
            {
                flag = !flag;
            }
            Vector3 position = flag ? PlayerController.Instance.boardController.backTruckCoM.position : PlayerController.Instance.boardController.frontTruckCoM.position;
            PlayerController.Instance.SetBoardCenterOfMass(PlayerController.Instance.boardController.boardTransform.InverseTransformPoint(position));
            PlayerController.Instance.SetBackTruckCenterOfMass(PlayerController.Instance.boardController.backTruckRigidbody.transform.InverseTransformPoint(position));
            PlayerController.Instance.SetFrontTruckCenterOfMass(PlayerController.Instance.boardController.frontTruckRigidbody.transform.InverseTransformPoint(position));
            _centerOfMassChanged = true;
        }

        private void ResetCenterOfMass()
        {
            _pivoting = false;
            UnsetManualTruckSprings();
            PlayerController.Instance.ResetBoardCenterOfMass();
            PlayerController.Instance.ResetBackTruckCenterOfMass();
            PlayerController.Instance.ResetFrontTruckCenterOfMass();
            _centerOfMassChanged = false;
        }

        public Quaternion GetManualTarget(bool p_manual, float p_manualAxis, float p_secondaryAxis, float p_swivel)
        {
            float num = (Mathf.Abs(p_manualAxis) - 0.5f) * 10f + p_secondaryAxis * 5f;
            Vector3 vector = (!PlayerController.Instance.boardController.IsBoardBackwards) ? PlayerController.Instance.boardController.boardTransform.forward : (-PlayerController.Instance.boardController.boardTransform.forward);
            vector = Vector3.ProjectOnPlane(vector, PlayerController.Instance.boardController.LerpedGroundNormal);
            Vector3 vector2 = Vector3.Cross(vector, PlayerController.Instance.boardController.LerpedGroundNormal);
            Vector3 upwards = Quaternion.AngleAxis(10f + num, p_manual ? vector2 : (-vector2)) * PlayerController.Instance.boardController.LerpedGroundNormal;
            Vector3 forward = Quaternion.AngleAxis(10f + num, p_manual ? vector2 : (-vector2)) * vector;
            Vector3 forward2 = Quaternion.AngleAxis(10f + num, p_manual ? vector2 : (-vector2)) * -vector;
            if (PlayerController.Instance.boardController.IsBoardBackwards)
            {
                return Quaternion.LookRotation(forward2, upwards);
            }
            return Quaternion.LookRotation(forward, upwards);
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

        public override void LeftTriggerHeld(float value, InputController.TurningMode turningMode)
        {
            _leftTriggerValue = value;
            if (!_lockTurningFromPowerslide)
            {
                if (PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude < 0.5f && !_powerslideActionAttempt)
                {
                    if (!_centerOfMassChanged)
                    {
                        SetCenterOfMass();
                    }
                    PlayerController.Instance.ManualRotation(GetManualTarget(!_switchPivot, 0.05f, 0.05f, 0f));
                }
                else if (_centerOfMassChanged)
                {
                    ResetCenterOfMass();
                }
                PlayerController.Instance.TurnLeft(value, turningMode);
                return;
            }
            PlayerController.Instance.RemoveTurnTorque(PlayerController.Instance.spinDeccelerate, turningMode);
        }

        public override void RightTriggerHeld(float value, InputController.TurningMode turningMode)
        {
            _rightTriggerValue = value;
            if (!_lockTurningFromPowerslide)
            {
                if (PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude < 0.5f && !_powerslideActionAttempt)
                {
                    if (!_centerOfMassChanged)
                    {
                        SetCenterOfMass();
                    }
                    PlayerController.Instance.ManualRotation(GetManualTarget(!_switchPivot, 0.05f, 0.05f, 0f));
                }
                else if (_centerOfMassChanged)
                {
                    ResetCenterOfMass();
                }
                PlayerController.Instance.TurnRight(value, turningMode);
                return;
            }
            PlayerController.Instance.RemoveTurnTorque(PlayerController.Instance.spinDeccelerate, turningMode);
        }

        public override void BothTriggersReleased(InputController.TurningMode turningMode)
        {
            if (_lockTurningFromPowerslide)
            {
                _lockTurningFromPowerslide = false;
            }
            if (_centerOfMassChanged)
            {
                ResetCenterOfMass();
            }
            _wasPowersliding = false;
            PlayerController.Instance.RemoveTurnTorque(PlayerController.Instance.spinDeccelerate, turningMode);
        }

        protected virtual void PowerslideCheck(StickInput l, StickInput r)
        {
            if ((l.ToeAxis > 0.1f && r.ToeAxis < -0.1f) || (l.ToeAxis < -0.1f && r.ToeAxis > 0.1f))
            {
                _powerslideActionAttempt = true;
                if (PlayerController.Instance.boardController.AllDown && PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude > 0.7f && !_wasPowersliding && !_lockTurningFromPowerslide)
                {
                    base.DoTransition(typeof(Custom_Powerslide), null);
                    return;
                }
            }
            else
            {
                _powerslideActionAttempt = false;
                if (_wasPowersliding)
                {
                    _wasPowersliding = false;
                }
            }
        }

        protected virtual void SetupCheck(StickInput p_leftStick, StickInput p_rightStick)
        {
            if (PlayerController.Instance.IsExitingPinState || PlayerController.Instance.WasOnCoping || _lockTurningFromPowerslide || _lockSetupFromPowerslide || _impactFromManual || _bumpOutToRiding)
            {
                return;
            }
            if (p_leftStick.SetupDir > 0.8f || (new Vector2(p_leftStick.ToeAxis, p_leftStick.SetupDir).magnitude > 0.8f && p_leftStick.SetupDir > 0.325f))
            {
                PlayerController.Instance.AnimSetNollie(PlayerController.Instance.GetNollie(false));
                PlayerController.Instance.SetupDirection(p_leftStick, ref _setupDir);
                object[] args = new object[]
                {
                p_leftStick,
                p_rightStick,
                p_leftStick.ForwardDir > 0.2f,
                _setupDir
                };
                base.DoTransition(typeof(Custom_Setup), args);
                return;
            }
            if (p_rightStick.AugmentedSetupDir > 0.8f || (new Vector2(p_rightStick.ToeAxis, p_rightStick.SetupDir).magnitude > 0.8f && p_rightStick.SetupDir > 0.325f))
            {
                PlayerController.Instance.AnimSetNollie(PlayerController.Instance.GetNollie(true));
                PlayerController.Instance.SetupDirection(p_rightStick, ref _setupDir);
                object[] args2 = new object[]
                {
                p_rightStick,
                p_leftStick,
                p_rightStick.ForwardDir > 0.2f,
                _setupDir
                };
                base.DoTransition(typeof(Custom_Setup), args2);
                return;
            }
            _lockSetupFromPowerslide = false;
            PlayerController.Instance.AnimSetupTransition(false);
        }

        protected virtual void PumpingCheck(StickInput p_leftStick, StickInput p_rightStick)
        {
            if (Mathf.Abs(p_leftStick.ToeAxis) < 0.8f && p_leftStick.SetupDir < -0.1f)
            {
                if (Mathf.Abs(p_leftStick.SetupDir) > _pumpLeft)
                {
                    _pumpLeft = 1f;
                }
                _canPumpLeftStick = true;
            }
            if (Mathf.Abs(p_rightStick.ToeAxis) < 0.8f && p_rightStick.SetupDir < -0.1f)
            {
                if (Mathf.Abs(p_rightStick.SetupDir) > _pumpRight)
                {
                    _pumpRight = 1f;
                }
                _canPumpRightStick = true;
            }
            if (!_pumping)
            {
                if (_canPumpLeftStick && !_canPumpRightStick)
                {
                    _pumpTarget = Mathf.Lerp(_pumpTargetHigh, _pumpTargetLow, _pumpLeft);
                    if (p_leftStick.PopToeVel.y > 2f)
                    {
                        PlayerController.Instance.animationController.ScaleAnimSpeed(1f);
                        _difSine = 0f;
                        _cachedForward = PlayerController.Instance.boardController.GetClosestBoardForwardToVelocity();
                        _cachedRight = -Vector3.Cross(_cachedForward, PlayerController.Instance.boardController.LastGroundNormalWhenGrounded);
                        _pumpMultiplier = _pumpLeft;
                        _pumping = true;
                        PlayerController.Instance.animationController.SetValue("Pumping", true);
                    }
                }
                else if (_canPumpRightStick && !_canPumpLeftStick)
                {
                    _pumpTarget = Mathf.Lerp(_pumpTargetHigh, _pumpTargetLow, _pumpRight);
                    if (p_rightStick.PopToeVel.y > 2f)
                    {
                        PlayerController.Instance.animationController.ScaleAnimSpeed(1f);
                        _difSine = 0f;
                        _cachedForward = PlayerController.Instance.boardController.GetClosestBoardForwardToVelocity();
                        _cachedRight = -Vector3.Cross(_cachedForward, PlayerController.Instance.boardController.LastGroundNormalWhenGrounded);
                        _pumpMultiplier = _pumpRight;
                        _pumping = true;
                        PlayerController.Instance.animationController.SetValue("Pumping", true);
                    }
                }
                else if (_canPumpRightStick && _canPumpLeftStick)
                {
                    _pumpTarget = Mathf.Lerp(_pumpTargetHigh, _pumpTargetLow, Mathf.Clamp((_pumpLeft * 2f + _pumpRight * 2f) * 0.5f, 0f, 1.5f));
                    if (p_rightStick.PopToeVel.y > 2f || p_leftStick.PopToeVel.y > 2f)
                    {
                        PlayerController.Instance.animationController.ScaleAnimSpeed(1f);
                        _difSine = 0f;
                        _cachedForward = PlayerController.Instance.boardController.GetClosestBoardForwardToVelocity();
                        _cachedRight = -Vector3.Cross(_cachedForward, PlayerController.Instance.boardController.LastGroundNormalWhenGrounded);
                        _pumpMultiplier = (_pumpLeft * 2f + _pumpRight * 2f) * 0.5f;
                        _pumpMultiplier = Mathf.Clamp(_pumpMultiplier, 0f, 1.2f);
                        _pumping = true;
                        PlayerController.Instance.animationController.SetValue("Pumping", true);
                    }
                }
                else
                {
                    _pumpTarget = _pumpTargetHigh;
                }
                PlayerController.Instance.animationController.SetValue("Pump", Mathf.Lerp(PlayerController.Instance.animationController.skaterAnim.GetFloat("Pump"), 0f, Time.deltaTime));
            }
            else
            {
                _pumpTarget = _pumpTargetHigh;
                _prevSine = _currentSine;
                Vector3 to = Vector3.ProjectOnPlane(PlayerController.Instance.boardController.GetClosestBoardForwardToVelocity(), _cachedRight);
                float num = Vector3.SignedAngle(_cachedForward, to, _cachedRight);
                Mathf.Clamp(num, 0f, 180f);
                _currentSine = Mathf.Sin(num * 3.14159274f / 180f);
                _difSine += Mathf.Abs(_currentSine - _prevSine);
                PlayerController.Instance.animationController.SetValue("Pump", Mathf.Lerp(PlayerController.Instance.animationController.skaterAnim.GetFloat("Pump"), _difSine, Time.deltaTime * 10f));
                float p_force = PlayerController.Instance.pumpCurve.Evaluate(_pumpTimer) * _difSine * PlayerController.Instance._pumpPotential * _pumpForce;
                PlayerController.Instance.AddPumpForce(p_force);
                _pumpTimer += Time.deltaTime;
                if (_pumpTimer >= _pumpResetTime)
                {
                    PlayerController.Instance.animationController.SetValue("Pumping", false);
                    ResetPumpParameters();
                    _pumping = false;
                }
            }
            _pumpCom = Mathf.Lerp(_pumpCom, _pumpTarget, (!_pumping) ? (Time.deltaTime * 50f) : (Time.deltaTime * 60f));
        }

        private void ResetPumpParameters()
        {
            _pumpTimer = 0f;
            _pumpLeft = 0f;
            _pumpRight = 0f;
        }

        public override void OnLeftStickCenteredUpdate()
        {
            _canPumpLeftStick = false;
            _pumpLeft = 0f;
        }

        public override void OnRightStickCenteredUpdate()
        {
            _canPumpRightStick = false;
            _pumpRight = 0f;
        }

        public override void OnManualEnter(StickInput p_popStick, StickInput p_flipStick)
        {
            if (PlayerController.Instance.respawn.respawning)
            {
                return;
            }
            PlayerController.Instance.SetManualStrength(1f);
            PlayerController.Instance.AnimSetManual(true, Mathf.Lerp(PlayerController.Instance.AnimGetManualAxis(), p_popStick.ForwardDir, Time.deltaTime * 10f));
            object[] args = new object[]
            {
            p_popStick,
            p_flipStick,
            true,
            0f
            };
            base.DoTransition(typeof(Custom_Manualling), args);
        }

        public override void OnNoseManualEnter(StickInput p_popStick, StickInput p_flipStick)
        {
            if (PlayerController.Instance.respawn.respawning)
            {
                return;
            }
            PlayerController.Instance.AnimSetNoseManual(true, Mathf.Lerp(PlayerController.Instance.AnimGetManualAxis(), p_popStick.ForwardDir, Time.deltaTime * 10f));
            object[] args = new object[]
            {
            p_popStick,
            p_flipStick,
            false,
            0f
            };
            base.DoTransition(typeof(Custom_Manualling), args);
        }

        public override bool IsOnGroundState()
        {
            return true;
        }

        public override bool CapsuleEnabled()
        {
            return true;
        }

        public override void OnCopingCheck(SplineComputer _spline)
        {
            if (!Main.settings.Grinds)
            {
                return;
            }

            if (!PlayerController.Instance.WasOnCoping && _canEnterCopingGrind && !_bumpOutToRiding)
            {
                PlayerController.Instance.CheckForCopingGrind(_spline, _leftStick, _rightStick);
            }
        }

        public override void TransitionToEnterCopingState(Transform _targetPos, Quaternion _targetRot, SplineComputer _spline, Vector3 _grindDir)
        {
            if (!Main.settings.Grinds)
            {
                return;
            }

            if (PlayerController.Instance.WasOnCoping || _bumpOutToRiding)
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
    }
}