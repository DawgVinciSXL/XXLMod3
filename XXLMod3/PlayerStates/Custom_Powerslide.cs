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
    public class Custom_Powerslide : PlayerState_OnBoard
    {
        private bool _animTriggered;
        private bool _autoRevert;
        private bool _backTruckDown;
        private bool _canRevert;
        private bool _centerOfMassSet;
        private bool _colliding;
        private bool _exitToInAir;
        private bool _exitToManual;
        private bool _forceRevert;
        private bool _frontTruckDown;
        private bool _isExiting;
        private bool _leftTriggerHeld;
        private bool _manual;
        private bool _revertAnimTriggered;
        private bool _revertTriggered;
        private bool _revertWindowEnded;
        private bool _rightTriggerHeld;
        private bool _toFakie;
        private bool _toRidingAnim;
        private bool _wasFakie;
        private bool _wasInImpactState;
        private bool _wasManualling;

        private float _autoPumpLerpSpeed = 5f;
        private float _curNormalAngle;
        private float _currentFrictionMultiplier = 1f;
        private float _dot;
        private float _impactTimer;
        private float _inAirTimer;
        private float _lForwardDir;
        private float _lToeAxis;
        private float _manualTimer;
        private float _maxFrictionMultiplier = 0.935f;
        private float _minimumVelocity;
        private float _powerslide;
        private float _powerslideRotationSpeed = 60f;
        private float _prevNormalAngle;
        private float _ratchetSpeed = 3f;
        private float _rForwardDir;
        private float _rToeAxis;
        private float _signedAngle;
        private float _skaterDot;
        private float _tempPowerslide;
        private float _timeInState;

        private int _negation = 1;

        private string _backTruckTag = "Surface_Concrete";
        private string _frontTruckTag = "Surface_Concrete";
        private string _prevBackTruckTag = "Surface_Concrete";
        private string _prevFrontTruckTag = "Surface_Concrete";

        private Quaternion _targetRot = Quaternion.identity;

        private StickInput _leftStickInput;
        private StickInput _rightStickInput;

        private Vector2 _leftStick = Vector2.zero;
        private Vector2 _rightStick = Vector2.zero;

        private Vector3 _groundNormal = Vector3.up;
        private Vector3 _intendedForward = Vector3.zero;
        private Vector3 _prevLerpedNormal = Vector3.up;

        public override void SetupDefinition(ref FSMStateType p_stateType, ref List<Type> children)
        {
            p_stateType = FSMStateType.Type_OR;
        }

        public Custom_Powerslide()
        {
            PlayerController.Instance.animationController.SetValue("Powersliding", true);
            PlayerController.Instance.CrossFadeAnimation("Powersliding", 0.2f);
            PlayerController.Instance.SetKneeBendWeightManually(0.2f);
            _animTriggered = true;
            _autoPumpLerpSpeed = PlayerController.Instance.autoPumpLerpSpeed;
            _prevLerpedNormal = PlayerController.Instance.boardController.AutoPumpLerpedNormal;
            PlayerController.Instance.boardController.AutoPumpLerpedNormal = Vector3.MoveTowards(PlayerController.Instance.boardController.AutoPumpLerpedNormal, PlayerController.Instance.boardController.GroundNormal, _autoPumpLerpSpeed * Time.deltaTime);
            _curNormalAngle = Vector3.Angle(PlayerController.Instance.boardController.AutoPumpLerpedNormal, Vector3.up);
            _prevNormalAngle = _curNormalAngle;
        }

        public Custom_Powerslide(bool _fromImpactState)
        {
            PlayerController.Instance.SetKneeBendWeightManually(0.2f);
            _autoPumpLerpSpeed = PlayerController.Instance.autoPumpLerpSpeed;
            _prevLerpedNormal = PlayerController.Instance.boardController.AutoPumpLerpedNormal;
            PlayerController.Instance.boardController.AutoPumpLerpedNormal = Vector3.MoveTowards(PlayerController.Instance.boardController.AutoPumpLerpedNormal, PlayerController.Instance.boardController.GroundNormal, _autoPumpLerpSpeed * Time.deltaTime);
            _curNormalAngle = Vector3.Angle(PlayerController.Instance.boardController.AutoPumpLerpedNormal, Vector3.up);
            _prevNormalAngle = _curNormalAngle;
            _wasInImpactState = _fromImpactState;
        }

        public Custom_Powerslide(int p_state)
        {
            if (p_state == 4)
            {
                _wasManualling = true;
            }
            PlayerController.Instance.SetKneeBendWeightManually(0.2f);
            _autoPumpLerpSpeed = PlayerController.Instance.autoPumpLerpSpeed;
            _prevLerpedNormal = PlayerController.Instance.boardController.AutoPumpLerpedNormal;
            PlayerController.Instance.boardController.AutoPumpLerpedNormal = Vector3.MoveTowards(PlayerController.Instance.boardController.AutoPumpLerpedNormal, PlayerController.Instance.boardController.GroundNormal, _autoPumpLerpSpeed * Time.deltaTime);
            _curNormalAngle = Vector3.Angle(PlayerController.Instance.boardController.AutoPumpLerpedNormal, Vector3.up);
            _prevNormalAngle = _curNormalAngle;
        }

        public Custom_Powerslide(float p_velMag)
        {
            _minimumVelocity = p_velMag;
            _autoRevert = true;
            _wasInImpactState = true;
        }

        public override void Enter()
        {
            PlayerController.Instance.currentStateEnum = PlayerController.CurrentState.Powerslide;
            XXLController.CurrentState = CurrentState.Powerslide;
            PlayerController.Instance.DisableArmPhysics();
            PlayerController.Instance.cameraController.NeedToSlowLerpCamera = true;
            PlayerController.Instance.ScalePlayerCollider();
            PlayerController.Instance.ToggleFlipColliders(false);
            _groundNormal = PlayerController.Instance.GetGroundNormal();
            HandlePowerslideTagsAndSound();
            PlayerController.Instance.skaterController.skaterRigidbody.angularVelocity = Vector3.zero;
            PlayerController.Instance.SetTurningMode(InputController.TurningMode.Powerslide);
            PlayerController.Instance.SetTurnMultiplier(20f);
            _negation = ((!PlayerController.Instance.GetBoardBackwards()) ? 1 : -1);
            PlayerController.Instance.SetBoardPhysicsMaterial(PlayerController.FrictionType.Powerslide);
            EventManager.Instance.EnterPowerSlide();
            XXLController.Instance.ActivateSlowMotion(Main.settings.SlowMotionReverts, Main.settings.SlowMotionRevertSpeed);
        }

        public override void Exit()
        {
            XXLController.Instance.ResetTime(Main.settings.SlowMotionReverts);
            PlayerController.Instance.EnableArmPhysics();
            PlayerController.Instance.cameraController.NeedToSlowLerpCamera = false;
            _isExiting = true;
            SoundManager.Instance.StopPowerslideSound(1, Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardRigidbody.velocity, Vector3.up).magnitude);
            SoundManager.Instance.StopPowerslideSound(2, Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardRigidbody.velocity, Vector3.up).magnitude);
            PlayerController.Instance.SetBoardPhysicsMaterial(PlayerController.FrictionType.Default);
            PlayerController.Instance.VelocityOnPop = PlayerController.Instance.boardController.boardRigidbody.velocity;
            PlayerController.Instance.SetKneeIKTargetWeight(1f);
            PlayerController.Instance.SetTurnMultiplier(1f);
            PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
            if (!_exitToManual)
            {
                PlayerController.Instance.ResetBackTruckCenterOfMass();
                PlayerController.Instance.ResetFrontTruckCenterOfMass();
                PlayerController.Instance.ResetBoardCenterOfMass();
                if (!_exitToInAir || _toRidingAnim)
                {
                    PlayerController.Instance.CrossFadeAnimation("Riding", 0.3f);
                }
                PlayerController.Instance.animationController.SetValue("Powersliding", false);
            }
            EventManager.Instance.ExitPowerSlide();
        }

        public override void Update()
        {
            base.Update();
            PlayerController.Instance.UpdateBoardAngle();
            RunImpactTimer();
            _timeInState += Time.deltaTime;
            if (_autoRevert)
            {
                PlayerController.Instance.CacheRidingTransforms();
            }
            SoundManager.Instance.SetPowerslideVolume(PlayerController.Instance.boardController.AllDown ? 2 : 1, PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            PlayerController.Instance.ScalePlayerCollider();
            if (_autoRevert)
            {
                UpdateAutoPumpLerpedNormal();
            }
            HandlePowerslideTagsAndSound();
            _groundNormal = Vector3.Lerp(_groundNormal, PlayerController.Instance.GetGroundNormal(), Time.deltaTime * 50f);
            PlayerController.Instance.comController.UpdateCOM(Main.settings.PowerslideCrouchAmount, 1);
            if (!_revertTriggered)
            {
                if (Mathf.Abs(_tempPowerslide) > 0.1f)
                {
                    _currentFrictionMultiplier = Mathf.MoveTowards(_currentFrictionMultiplier, _maxFrictionMultiplier, Time.deltaTime);
                    PlayerController.Instance.SetRotationTarget();
                    PlayerController.Instance.boardController.SetBoardControllerUpVector(_groundNormal);
                    PlayerController.Instance.SkaterRotation(true, false, 1f, PlayerController.Instance.skaterController.skaterTransform.forward);
                    PlayerController.Instance.PowerslideRotation(0.3f);
                }
                else if (_autoRevert && PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude < _minimumVelocity)
                {
                    PlayerController.Instance.boardController.boardRigidbody.velocity = PlayerController.Instance.boardController.boardRigidbody.velocity.normalized * _minimumVelocity;
                }
            }
            ListenForRevert();
            ListenForInAirStateChange();
            VelocityChecks();
            _colliding = false;
        }

        private void HandlePowerslideTagsAndSound()
        {
            _frontTruckTag = PlayerController.Instance.boardController.GetSurfaceTagString(true);
            if (_frontTruckTag == "")
            {
                _frontTruckTag = "Surface_Concrete";
                if (_frontTruckDown)
                {
                    _frontTruckDown = false;
                    SoundManager.Instance.StopPowerslideSound(1, Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardRigidbody.velocity, Vector3.up).magnitude);
                }
            }
            else if (!_frontTruckDown || _prevFrontTruckTag != _frontTruckTag)
            {
                _frontTruckDown = true;
                SoundManager.Instance.StopPowerslideSound(1, Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardRigidbody.velocity, Vector3.up).magnitude);
                SoundManager.Instance.PlayPowerslideSound(PlayerController.Instance.GetSurfaceTag(_frontTruckTag), Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardRigidbody.velocity, Vector3.up).magnitude, 1);
            }
            _prevFrontTruckTag = _frontTruckTag;
            _backTruckTag = PlayerController.Instance.boardController.GetSurfaceTagString(false);
            if (_backTruckTag == "")
            {
                _backTruckTag = "Surface_Concrete";
                if (_backTruckDown)
                {
                    _backTruckDown = false;
                    SoundManager.Instance.StopPowerslideSound(2, Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardRigidbody.velocity, Vector3.up).magnitude);
                }
            }
            else if (!_backTruckDown || _prevBackTruckTag != _backTruckTag)
            {
                _backTruckDown = true;
                SoundManager.Instance.StopPowerslideSound(2, Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardRigidbody.velocity, Vector3.up).magnitude);
                SoundManager.Instance.PlayPowerslideSound(PlayerController.Instance.GetSurfaceTag(_backTruckTag), Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardRigidbody.velocity, Vector3.up).magnitude, 2);
            }
            _prevBackTruckTag = _backTruckTag;
        }

        private void VelocityChecks()
        {
            Vector3 vector = Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardRigidbody.velocity, _groundNormal);
            if (_timeInState >= 0.1f)
            {
                if (AngularVelocityCheck() && AngleCheck(0.7f))
                {
                    TransitionToRiding(18);
                }
                if (!_revertTriggered && AngleCheck(0.99f) && vector.magnitude < 3f)
                {
                    TransitionToRiding(18);
                }
            }
            if (vector.magnitude < 1f)
            {
                TransitionToRiding(0);
            }
        }

        private bool ImpactAngleCheck()
        {
            Vector3 vector = Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardRigidbody.velocity.normalized, PlayerController.Instance.skaterController.skaterTransform.up);
            Vector3 vector2 = Vector3.ProjectOnPlane(PlayerController.Instance.boardController.GetClosestBoardForwardToVelocity(), PlayerController.Instance.skaterController.skaterTransform.up);
            float f = Vector3.Dot(PlayerController.Instance.skaterController.skaterTransform.forward, vector);
            float f2 = Vector3.Dot(vector2, vector);
            Vector3.Angle(vector2, PlayerController.Instance.skaterController.skaterTransform.forward);
            Vector3.Angle(vector2, vector);
            float f3 = Vector3.Dot(_intendedForward, vector);
            return Mathf.Abs(f2) > 0.8f && (Mathf.Abs(f) <= 0.94f || Mathf.Abs(f3) >= 0.5f) && (Mathf.Abs(f) <= 0.95f || Mathf.Abs(f2) <= 0.8f);
        }

        private void TransitionToRiding(int _state)
        {
            PlayerController.Instance.skaterController.skaterRigidbody.angularVelocity = Vector3.zero;
            PlayerController.Instance.boardController.boardRigidbody.angularVelocity = Vector3.zero;
            PlayerController.Instance.boardController.backTruckRigidbody.angularVelocity = Vector3.zero;
            PlayerController.Instance.boardController.frontTruckRigidbody.angularVelocity = Vector3.zero;
            if (_state == 0)
            {
                base.DoTransition(typeof(Custom_Riding), null);
                return;
            }
            if (_wasInImpactState)
            {
                _state = 19;
            }
            object[] args = new object[]
            {
            _state
            };
            base.DoTransition(typeof(Custom_Riding), args);
        }

        private bool AngleCheck(float _limit)
        {
            return Mathf.Abs(Vector3.Dot(Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardRigidbody.velocity.normalized, _groundNormal).normalized, PlayerController.Instance.boardController.boardRigidbody.transform.forward)) > _limit;
        }

        private bool ManualAngleCheck(float _limit)
        {
            Vector3 rhs = Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardRigidbody.velocity.normalized, PlayerController.Instance.skaterController.skaterTransform.up);
            return Mathf.Abs(Vector3.Dot(Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardTransform.forward, PlayerController.Instance.skaterController.skaterTransform.up), rhs)) > _limit;
        }

        private float SkaterDot()
        {
            Vector3 rhs = Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardRigidbody.velocity.normalized, PlayerController.Instance.skaterController.skaterTransform.up);
            return Vector3.Dot(PlayerController.Instance.skaterController.skaterTransform.forward, rhs);
        }

        private bool AngularVelocityCheck()
        {
            return Mathf.Abs(PlayerController.Instance.boardController.boardTransform.InverseTransformDirection(PlayerController.Instance.boardController.boardRigidbody.angularVelocity).y) < 0.2f;
        }

        private void RunImpactTimer()
        {
            if (_wasManualling)
            {
                _manualTimer += Time.deltaTime;
                if (_manualTimer > 0.4f)
                {
                    _wasManualling = false;
                }
            }
            if (_wasInImpactState)
            {
                _impactTimer += Time.deltaTime;
                if (_impactTimer > 0.15f)
                {
                    _revertWindowEnded = true;
                }
                if (_impactTimer > 0.3f)
                {
                    _wasInImpactState = false;
                }
            }
        }

        private void UpdateAutoPumpLerpedNormal()
        {
            _prevNormalAngle = _curNormalAngle;
            PlayerController.Instance.boardController.AutoPumpLerpedNormal = Vector3.MoveTowards(PlayerController.Instance.boardController.AutoPumpLerpedNormal, PlayerController.Instance.boardController.GroundNormal, _autoPumpLerpSpeed * Time.deltaTime);
            _curNormalAngle = Vector3.Angle(PlayerController.Instance.boardController.AutoPumpLerpedNormal, Vector3.up);
        }

        private void SpawnCube(Vector3 _pos, Vector3 _normal)
        {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.transform.localScale = new Vector3(0.02f, 2f, 0.02f);
            gameObject.transform.position = _pos;
            gameObject.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(Vector3.forward, _normal), _normal);
            UnityEngine.Object.Destroy(gameObject, 5f);
        }

        public override void OnStickFixedUpdate(StickInput p_leftStick, StickInput p_rightStick)
        {
            _leftStickInput = p_leftStick;
            _rightStickInput = p_rightStick;
            _leftStick = new Vector2(p_leftStick.ToeAxis, p_leftStick.ForwardDir);
            _rightStick = new Vector2(p_rightStick.ToeAxis, p_rightStick.ForwardDir);
            if (!_forceRevert)
            {
                ListenForPowerslideInput(p_leftStick, p_rightStick);
            }
            _intendedForward = Quaternion.AngleAxis(_tempPowerslide * 80f, PlayerController.Instance.skaterController.skaterTransform.up) * PlayerController.Instance.skaterController.skaterTransform.forward;
            _lForwardDir = p_leftStick.ForwardDir * 0.8f;
            _rForwardDir = p_rightStick.ForwardDir * 0.8f;
            if (Mathf.Abs(p_leftStick.ToeAxis) > Mathf.Abs(_lToeAxis) || _revertTriggered)
            {
                _lToeAxis = p_leftStick.ToeAxis;
            }
            else
            {
                _lToeAxis = Mathf.MoveTowards(_lToeAxis, p_leftStick.ToeAxis, Time.deltaTime * _ratchetSpeed);
            }
            if (Mathf.Abs(p_rightStick.ToeAxis) > Mathf.Abs(_rToeAxis) || _revertTriggered)
            {
                _rToeAxis = p_rightStick.ToeAxis;
            }
            else
            {
                _rToeAxis = Mathf.MoveTowards(_rToeAxis, p_rightStick.ToeAxis, Time.deltaTime * _ratchetSpeed);
            }
            if ((_lForwardDir <= 0.1f || _rForwardDir <= 0.1f) && (_lForwardDir >= -0.1f || _rForwardDir >= -0.1f))
            {
                _lForwardDir = 0f;
                _rForwardDir = 0f;
                if (_centerOfMassSet)
                {
                    PlayerController.Instance.ResetBackTruckCenterOfMass();
                    PlayerController.Instance.ResetFrontTruckCenterOfMass();
                    PlayerController.Instance.ResetBoardCenterOfMass();
                    _centerOfMassSet = false;
                }
            }
            else if (!_centerOfMassSet && !_isExiting)
            {
                SetCenterOfMass(_lForwardDir, _rForwardDir);
                _centerOfMassSet = true;
            }
            if (_revertTriggered || (!_revertTriggered && Mathf.Abs(_powerslide) < 0.1f))
            {
                return;
            }
            switch (SettingsManager.Instance.controlType)
            {
                case ControlType.Same:
                    if (SettingsManager.Instance.stance == Stance.Regular)
                    {
                        PlayerController.Instance.SetBoardTargetPosition(0f);
                        PlayerController.Instance.SetFrontPivotRotation(0f);
                        PlayerController.Instance.SetBackPivotRotation(0f);
                        PlayerController.Instance.SetPivotForwardRotation(_revertTriggered ? 0f : (_lForwardDir + _rForwardDir), _powerslideRotationSpeed * 0.2f);
                        PlayerController.Instance.SetPivotSideRotation(_revertTriggered ? 0f : (_lToeAxis - _rToeAxis), _powerslideRotationSpeed);
                        return;
                    }
                    PlayerController.Instance.SetBoardTargetPosition(0f);
                    PlayerController.Instance.SetFrontPivotRotation(0f);
                    PlayerController.Instance.SetBackPivotRotation(0f);
                    PlayerController.Instance.SetPivotForwardRotation(_revertTriggered ? 0f : (_lForwardDir + _rForwardDir), _powerslideRotationSpeed * 0.2f);
                    PlayerController.Instance.SetPivotSideRotation(_revertTriggered ? 0f : (_lToeAxis - _rToeAxis), _powerslideRotationSpeed);
                    return;
                case ControlType.Swap:
                    if (!PlayerController.Instance.IsSwitch)
                    {
                        if (SettingsManager.Instance.stance == Stance.Regular)
                        {
                            PlayerController.Instance.SetBoardTargetPosition(0f);
                            PlayerController.Instance.SetFrontPivotRotation(0f);
                            PlayerController.Instance.SetBackPivotRotation(0f);
                            PlayerController.Instance.SetPivotForwardRotation(_revertTriggered ? 0f : (_lForwardDir + _rForwardDir), _powerslideRotationSpeed * 0.2f);
                            PlayerController.Instance.SetPivotSideRotation(_revertTriggered ? 0f : (_lToeAxis - _rToeAxis), _powerslideRotationSpeed);
                            return;
                        }
                        PlayerController.Instance.SetBoardTargetPosition(0f);
                        PlayerController.Instance.SetFrontPivotRotation(0f);
                        PlayerController.Instance.SetBackPivotRotation(0f);
                        PlayerController.Instance.SetPivotForwardRotation(_revertTriggered ? 0f : (_lForwardDir + _rForwardDir), _powerslideRotationSpeed * 0.2f);
                        PlayerController.Instance.SetPivotSideRotation(_revertTriggered ? 0f : (_lToeAxis - _rToeAxis), _powerslideRotationSpeed);
                        return;
                    }
                    else
                    {
                        if (SettingsManager.Instance.stance == Stance.Regular)
                        {
                            PlayerController.Instance.SetBoardTargetPosition(0f);
                            PlayerController.Instance.SetFrontPivotRotation(0f);
                            PlayerController.Instance.SetBackPivotRotation(0f);
                            PlayerController.Instance.SetPivotForwardRotation(_revertTriggered ? 0f : (_lForwardDir + _rForwardDir), _powerslideRotationSpeed * 0.2f);
                            PlayerController.Instance.SetPivotSideRotation(_revertTriggered ? 0f : (_lToeAxis - _rToeAxis), _powerslideRotationSpeed);
                            return;
                        }
                        PlayerController.Instance.SetBoardTargetPosition(0f);
                        PlayerController.Instance.SetFrontPivotRotation(0f);
                        PlayerController.Instance.SetBackPivotRotation(0f);
                        PlayerController.Instance.SetPivotForwardRotation(_revertTriggered ? 0f : (_lForwardDir + _rForwardDir), _powerslideRotationSpeed * 0.2f);
                        PlayerController.Instance.SetPivotSideRotation(_revertTriggered ? 0f : (_lToeAxis - _rToeAxis), _powerslideRotationSpeed);
                        return;
                    }
                    break;
                case ControlType.Simple:
                    if (!PlayerController.Instance.IsSwitch)
                    {
                        if (SettingsManager.Instance.stance == Stance.Regular)
                        {
                            PlayerController.Instance.SetBoardTargetPosition(0f);
                            PlayerController.Instance.SetFrontPivotRotation(0f);
                            PlayerController.Instance.SetBackPivotRotation(0f);
                            PlayerController.Instance.SetPivotForwardRotation(_revertTriggered ? 0f : (_lForwardDir + _rForwardDir), _powerslideRotationSpeed * 0.2f);
                            PlayerController.Instance.SetPivotSideRotation(_revertTriggered ? 0f : (_lToeAxis - _rToeAxis), _powerslideRotationSpeed);
                            return;
                        }
                        PlayerController.Instance.SetBoardTargetPosition(0f);
                        PlayerController.Instance.SetFrontPivotRotation(0f);
                        PlayerController.Instance.SetBackPivotRotation(0f);
                        PlayerController.Instance.SetPivotForwardRotation(_revertTriggered ? 0f : (_lForwardDir + _rForwardDir), _powerslideRotationSpeed * 0.2f);
                        PlayerController.Instance.SetPivotSideRotation(_revertTriggered ? 0f : (_lToeAxis - _rToeAxis), _powerslideRotationSpeed);
                        return;
                    }
                    else
                    {
                        if (SettingsManager.Instance.stance == Stance.Regular)
                        {
                            PlayerController.Instance.SetBoardTargetPosition(0f);
                            PlayerController.Instance.SetFrontPivotRotation(0f);
                            PlayerController.Instance.SetBackPivotRotation(0f);
                            PlayerController.Instance.SetPivotForwardRotation(_revertTriggered ? 0f : (_lForwardDir + _rForwardDir), _powerslideRotationSpeed * 0.2f);
                            PlayerController.Instance.SetPivotSideRotation(_revertTriggered ? 0f : (_rToeAxis - _lToeAxis), _powerslideRotationSpeed);
                            return;
                        }
                        PlayerController.Instance.SetBoardTargetPosition(0f);
                        PlayerController.Instance.SetFrontPivotRotation(0f);
                        PlayerController.Instance.SetBackPivotRotation(0f);
                        PlayerController.Instance.SetPivotForwardRotation(_revertTriggered ? 0f : (_lForwardDir + _rForwardDir), _powerslideRotationSpeed * 0.2f);
                        PlayerController.Instance.SetPivotSideRotation(_revertTriggered ? 0f : (_rToeAxis - _lToeAxis), _powerslideRotationSpeed);
                        return;
                    }
                    break;
                default:
                    return;
            }
        }

        private void SetCenterOfMass(float l_ForwardDir, float r_ForwardDir)
        {
            _manual = ((!PlayerController.Instance.GetBoardBackwards()) ? ((l_ForwardDir + r_ForwardDir) / 2f <= 0f) : ((l_ForwardDir + r_ForwardDir) / 2f > 0f));
            Vector3 position = _manual ? PlayerController.Instance.boardController.backTruckCoM.position : PlayerController.Instance.boardController.frontTruckCoM.position;
            PlayerController.Instance.SetBoardCenterOfMass(PlayerController.Instance.boardController.boardTransform.InverseTransformPoint(position));
            PlayerController.Instance.SetBackTruckCenterOfMass(PlayerController.Instance.boardController.backTruckRigidbody.transform.InverseTransformPoint(position));
            PlayerController.Instance.SetFrontTruckCenterOfMass(PlayerController.Instance.boardController.frontTruckRigidbody.transform.InverseTransformPoint(position));
        }

        private void ListenForPowerslideInput(StickInput p_leftStick, StickInput p_rightStick)
        {
            if ((p_leftStick.ToeAxis < -0.1f && p_rightStick.ToeAxis > 0.1f) || (p_leftStick.ToeAxis > 0.1f && p_rightStick.ToeAxis < -0.1f))
            {
                _canRevert = true;
                if (!_wasInImpactState && !_animTriggered && !PlayerController.Instance.IsCurrentAnimationPlaying("Powersliding") && !_revertTriggered)
                {
                    PlayerController.Instance.animationController.SetValue("Powersliding", true);
                    PlayerController.Instance.CrossFadeAnimation("Powersliding", 0.2f);
                    _animTriggered = true;
                }
                _tempPowerslide = (p_leftStick.ToeAxis - p_rightStick.ToeAxis) / 2f;
            }
            else
            {
                _tempPowerslide = 0f;
                if (_canRevert)
                {
                    if (!_revertTriggered)
                    {
                        PlayerController.Instance.SetBoardPhysicsMaterial(PlayerController.FrictionType.Default);
                    }
                    _revertTriggered = true;
                }
            }
            if (_wasInImpactState && !ImpactAngleCheck() && !_animTriggered)
            {
                PlayerController.Instance.animationController.SetValue("Powersliding", true);
                PlayerController.Instance.CrossFadeAnimation("Powersliding", 0.3f);
                _animTriggered = true;
            }
            if (_canRevert && Mathf.Abs(_tempPowerslide) > 0.3f && _wasInImpactState && ImpactAngleCheck())
            {
                TransitionToRiding(19);
            }
            if (Mathf.Abs(_tempPowerslide) > Mathf.Abs(_powerslide))
            {
                _powerslide = _tempPowerslide;
                return;
            }
            _powerslide = Mathf.MoveTowards(_powerslide, _tempPowerslide, Time.deltaTime);
        }

        private void ListenForRevert()
        {
            Vector3 vector = (!PlayerController.Instance.GetBoardBackwards()) ? PlayerController.Instance.boardController.boardTransform.forward : (-PlayerController.Instance.boardController.boardTransform.forward);
            _signedAngle = Vector3.SignedAngle(PlayerController.Instance.boardController.boardRigidbody.velocity.normalized, vector, PlayerController.Instance.GetGroundNormal());
            _dot = Vector3.Dot(PlayerController.Instance.boardController.boardRigidbody.velocity.normalized, vector);
            _skaterDot = Vector3.Dot(PlayerController.Instance.boardController.boardRigidbody.velocity.normalized, PlayerController.Instance.skaterController.skaterTransform.forward);
            if (_dot > 0f)
            {
                _toFakie = false;
            }
            else
            {
                _toFakie = true;
            }
            if (_revertTriggered)
            {
                Revert(1.2f, false);
                if (!_revertAnimTriggered)
                {
                    if (_toFakie && !_wasFakie)
                    {
                        if (_signedAngle > 0f)
                        {
                            PlayerController.Instance.CrossFadeAnimation("R_Com_Bs_Revert",(Main.settings.FixedRevertAnimation ? 1f : 0.2f));
                        }
                        else
                        {
                            PlayerController.Instance.CrossFadeAnimation("R_Com_Fs_Revert", (Main.settings.FixedRevertAnimation ? 1f : 0.2f));
                        }
                    }
                    else if (!_toFakie && _wasFakie)
                    {
                        if (_signedAngle > 0f)
                        {
                            PlayerController.Instance.CrossFadeAnimation("R_Com_Bs_Switch_Revert", (Main.settings.FixedRevertAnimation ? 1f : 0.2f));
                        }
                        else
                        {
                            PlayerController.Instance.CrossFadeAnimation("R_Com_Fs_Switch_Revert", (Main.settings.FixedRevertAnimation ? 1f : 0.2f));
                        }
                    }
                    else
                    {
                        PlayerController.Instance.skaterController.InitializeSkateRotation();
                        _toRidingAnim = true;
                        PlayerController.Instance.CrossFadeAnimation("Riding", 0.2f);
                    }
                    PlayerController.Instance.SetKneeBendWeightManually(0.2f);
                    _revertAnimTriggered = (Main.settings.FixedRevertAnimation ? false : true);
                    return;
                }
            }
            else
            {
                if (Mathf.Abs(_tempPowerslide) <= 0.1f)
                {
                    PlayerController.AutoRampTags autoRampTags = PlayerController.Instance.AutoPumpAndRevertCheck();
                    if (autoRampTags != PlayerController.AutoRampTags.AutoRevert && autoRampTags == PlayerController.AutoRampTags.AutoPumpAndRevert)
                    {
                        PlayerController.Instance.AutoPump();
                    }
                    Revert(1f, true);
                    return;
                }
                if (_skaterDot > 0f)
                {
                    _wasFakie = false;
                    return;
                }
                _wasFakie = true;
            }
        }

        private void Revert(float _speed, bool _autoCorrect)
        {
            Vector3 target = Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardRigidbody.velocity.normalized, _groundNormal);
            Vector3 a = Vector3.MoveTowards(Vector3.ProjectOnPlane(PlayerController.Instance.boardController.GetClosestBoardForwardToVelocity(), _groundNormal), target, Time.deltaTime * 30f);
            Vector3 vector = PlayerController.Instance.boardController.boardTransform.up;
            if (!_autoCorrect)
            {
                vector = Vector3.Lerp(vector, _groundNormal, Time.deltaTime * 20f);
            }
            else
            {
                vector = _groundNormal;
            }
            if (!_toFakie)
            {
                _targetRot = Quaternion.LookRotation((float)_negation * a, vector);
            }
            else
            {
                _targetRot = Quaternion.LookRotation((float)_negation * -a, vector);
            }
            PlayerController.Instance.RevertRotation(_targetRot, _speed);
            PlayerController.Instance.SkaterRotation(true, false);
            PlayerController.Instance.boardController.ResetTweakValues();
            if (AngleCheck(0.99f))
            {
                TransitionToRiding(18);
            }
        }

        private void ListenForInAirStateChange()
        {
            if (!_colliding && !PlayerController.Instance.IsGrounded() && !PlayerController.Instance.IsRespawning && !PlayerController.Instance.boardController.triggerManager.IsColliding)
            {
                _inAirTimer += Time.deltaTime;
                if (_inAirTimer > 0.1f)
                {
                    PlayerController.Instance.SetTurningMode(InputController.TurningMode.InAir);
                    PlayerController.Instance.SetSkaterToMaster();
                    PlayerController.Instance.CrossFadeAnimation("Extend", 0.3f);
                    PlayerController.Instance.OnExtendAnimEnter();
                    Vector3 force = PlayerController.Instance.skaterController.PredictLanding(PlayerController.Instance.skaterController.skaterRigidbody.velocity);
                    PlayerController.Instance.skaterController.skaterRigidbody.AddForce(force, ForceMode.Impulse);
                    _exitToInAir = true;
                    EventManager.Instance.EnterAir();
                    object[] args = new object[]
                    {
                    true
                    };
                    base.DoTransition(typeof(Custom_InAir), args);
                    return;
                }
            }
            else if (PlayerController.Instance.IsGrounded())
            {
                _inAirTimer = 0f;
            }
        }

        public override void LeftTriggerHeld(float value, InputController.TurningMode turningMode)
        {
            base.LeftTriggerHeld(value * (_revertTriggered ? 5f : 1.75f), turningMode);
            _leftTriggerHeld = true;
        }

        public override void LeftTriggerReleased()
        {
            base.LeftTriggerReleased();
            _leftTriggerHeld = false;
        }

        public override void RightTriggerHeld(float value, InputController.TurningMode turningMode)
        {
            base.RightTriggerHeld(value * (_revertTriggered ? 5f : 1.75f), turningMode);
            _rightTriggerHeld = true;
        }

        public override void RightTriggerReleased()
        {
            base.RightTriggerReleased();
            _rightTriggerHeld = false;
        }

        public override void BothTriggersReleased(InputController.TurningMode turningMode)
        {
            base.BothTriggersReleased(turningMode);
        }

        public override void OnCollisionStayEvent(Collision c)
        {
            _colliding = true;
        }

        public override void OnCollisionEnterEvent(Vector3 _impulse, bool _isBoard, Collision c)
        {
            _colliding = true;
            if (_isBoard)
            {
                SoundManager.Instance.PlayBoardHit(_impulse.magnitude);
            }
        }

        public override void OnManualEnter(StickInput p_popStick, StickInput p_flipStick)
        {
            if (ManualAngleCheck(0.8f) && !_wasManualling)
            {
                PlayerController.Instance.SetManualStrength(1f);
                PlayerController.Instance.AnimSetManual(true, p_popStick.ForwardDir);
                PlayerController.Instance.CrossFadeAnimation("Manual", 0.2f);
                _exitToManual = true;
                object[] args = new object[]
                {
                p_popStick,
                p_flipStick,
                true,
                0f,
                0
                };
                base.DoTransition(typeof(Custom_Manualling), args);
                return;
            }
        }

        public override void OnNoseManualEnter(StickInput p_popStick, StickInput p_flipStick)
        {
            if (ManualAngleCheck(0.8f) && !_wasManualling)
            {
                PlayerController.Instance.SetManualStrength(1f);
                PlayerController.Instance.AnimSetNoseManual(true, p_popStick.ForwardDir);
                PlayerController.Instance.CrossFadeAnimation("Manual", 0.2f);
                _exitToManual = true;
                object[] args = new object[]
                {
                p_popStick,
                p_flipStick,
                false,
                0f,
                0
                };
                base.DoTransition(typeof(Custom_Manualling), args);
                return;
            }
        }

        public override void OnManualUpdate(StickInput p_popStick, StickInput p_flipStick)
        {
            if (ManualAngleCheck(0.8f) && !_wasManualling)
            {
                PlayerController.Instance.SetManualStrength(1f);
                PlayerController.Instance.AnimSetManual(true, Mathf.Lerp(PlayerController.Instance.AnimGetManualAxis(), p_popStick.ForwardDir, Time.deltaTime * 10f));
                PlayerController.Instance.CrossFadeAnimation("Manual", 0.2f);
                _exitToManual = true;
                object[] args = new object[]
                {
                p_popStick,
                p_flipStick,
                true,
                0f,
                0
                };
                base.DoTransition(typeof(Custom_Manualling), args);
                return;
            }
        }

        public override void OnNoseManualUpdate(StickInput p_popStick, StickInput p_flipStick)
        {
            if (ManualAngleCheck(0.8f) && !_wasManualling)
            {
                PlayerController.Instance.SetManualStrength(1f);
                PlayerController.Instance.AnimSetNoseManual(true, Mathf.Lerp(PlayerController.Instance.AnimGetManualAxis(), p_popStick.ForwardDir, Time.deltaTime * 10f));
                PlayerController.Instance.CrossFadeAnimation("Manual", 0.2f);
                _exitToManual = true;
                object[] args = new object[]
                {
                p_popStick,
                p_flipStick,
                false,
                0f,
                0
                };
                base.DoTransition(typeof(Custom_Manualling), args);
                return;
            }
        }

        public override bool IgnoreManualExitAttempt()
        {
            return true;
        }

        public override bool IsOnGroundState()
        {
            return true;
        }

        public override bool CanGrind()
        {
            if (!Main.settings.Grinds)
            {
                return false;
            }
            return _timeInState <= 0.2f;
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
            if (_timeInState <= 0.2f)
            {
                base.DoTransition(typeof(Custom_Grinding), null);
            }
        }

        public override void OnCopingCheck(SplineComputer _spline)
        {
            if (!Main.settings.Grinds)
            {
                return;
            }

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

        public override bool IsInPowerslideState()
        {
            return true;
        }
    }
}