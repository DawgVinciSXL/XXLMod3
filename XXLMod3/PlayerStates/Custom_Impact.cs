using FSMHelper;
using System;
using System.Collections.Generic;
using UnityEngine;
using XXLMod3.Controller;
using XXLMod3.Core;

namespace XXLMod3.PlayerStates
{
    public class Custom_Impact : PlayerState_OnBoard
    {
        private bool _applyFrictionIncrease;
        private bool _canGrind = true;
        private bool _canPumpLeftStick;
        private bool _canPumpRightStick;
        private bool _colliding;
        private bool _correctBoard = true;
        private bool _disablePumping;
        private bool _foundLowest;
        private bool _foundLowestCom;
        private bool _impactFinished;
        private bool _landed;
        private bool _leftHasCentered;
        private bool _pumpCheck;
        private bool _pumping;
        private bool _rightHasCentered;
        private bool _transitioningToPowerslide;
        private bool _waitForPowerSlideCheck = true;

        private float _currentSine;
        private float _difSine;
        private float _friction = 1f;
        private float _frictionIncreaseRate = 3f;
        private float _frictionIncreaseTimer;
        private float _frictionMax = 0.3f;
        private float _grindTimer;
        private float _impactForce;
        private float _impactTimer;
        private float _inAirTimer;
        private float _maxGrindTimer = 0.3f;
        private float _maxImpactTime = 0.3f;
        private float _prevPumpDelta;
        private float _prevSine;
        private float _pumpCom = 1.04196f;
        private float _pumpForce = 0.6f;
        private float _pumpLeft;
        private float _pumpMultiplier;
        private float _pumpResetTime = 0.5f;
        private float _pumpRight;
        private float _pumpTarget = 1.04196f;
        private float _pumpTargetLow = 0.7f;
        private float _pumpTargetHigh = 1.04196f;
        private float _pumpTimer;
        private float _rollOffWait;
        private float _totalTimeInState;

        private int groundedFrameCount;
        private int groundedFrameMax = 1;

        private PlayerController.SetupDir _setupDir;

        private RaycastHit _hit;
        private RaycastHit[] _rayCasts = new RaycastHit[0];

        private Vector3 _cachedForward;
        private Vector3 _cachedRight;
        private Vector3 _velocityOnImpact = Vector3.zero;

        public Custom_Impact()
        {
        }

        public Custom_Impact(bool p_canGrind)
        {
            _canGrind = p_canGrind;
        }

        public Custom_Impact(Vector3 p_inAirVelocity)
        {
        }

        public override void SetupDefinition(ref FSMStateType p_stateType, ref List<Type> children)
        {
            p_stateType = FSMStateType.Type_OR;
        }

        public override void Enter()
        {
            PlayerController.Instance.currentStateEnum = PlayerController.CurrentState.Impact;
            XXLController.CurrentState = CurrentState.Impact;
            _maxImpactTime = Main.settings.PopDelay ? 0.3f : 0f; ///////////////
            _velocityOnImpact = Vector3.ProjectOnPlane(PlayerController.Instance.skaterController.GetAverageSkaterVelocity(), PlayerController.Instance.skaterController.skaterTransform.up);
            PlayerController.Instance.ScalePlayerCollider();
            PlayerController.Instance.AnimSetRollOff(false);
            PlayerController.Instance.ToggleFlipColliders(false);
            SoundManager.Instance.PlayMovementFoleySound(0.3f, false);
            PlayerController.Instance.animationController.SetValue("NollieImpact", PlayerController.Instance.animationController.skaterAnim.GetFloat("Nollie"));
            PlayerController.Instance.animationController.ScaleAnimSpeed(1f);
            float num = Mathf.Clamp(Mathf.Abs(PlayerController.Instance.comController.COMRigidbody.velocity.y), 0f, 17f);
            if (num > 6f)
            {
                num -= 6f;
                num /= 11f;
                float num2 = num * 5.5f;
                float p_speed = Mathf.Clamp(1f + num2, 1f, 1f + num2);
                PlayerController.Instance.animationController.ScaleAnimSpeed(p_speed);
            }
            float impactForce = (Mathf.Clamp(Mathf.Abs(PlayerController.Instance.comController.COMRigidbody.velocity.y), 3.5f, 7.2f) - 3.5f) / 3.7f;
            _impactForce = impactForce;
            PlayerController.Instance.animationController.SetValue("Impact", _impactForce);
            if (!PlayerController.Instance.IsRespawning)
            {
                PlayerController.Instance.skaterController.InitializeSkateRotation();
                PlayerController.Instance.AnimSetNoComply(false);
                PlayerController.Instance.boardController.ResetAll();
                PlayerController.Instance.AnimSetManual(false, PlayerController.Instance.AnimGetManualAxis());
                PlayerController.Instance.AnimSetNoseManual(false, PlayerController.Instance.AnimGetManualAxis());
                PlayerController.Instance.SetTurnMultiplier(1f);
                PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
                PlayerController.Instance.AnimSetGrinding(false);
                PlayerController.Instance.ResetAnimationsAfterImpact();
                PlayerController.Instance.OnImpact();
                if (!PlayerController.Instance.IsCurrentAnimationPlaying("Impact"))
                {
                    PlayerController.Instance.CrossFadeAnimation(Main.settings.RandomImpactAnimations ? GetRandomAnimation(animations) : "Impact", 0.1f);
                    Main.modEntry.Logger.Log(GetRandomAnimation(animations));
                }
                PlayerController.Instance.SetKneeBendWeightManually(0f);
            }

            CheckForHighImpact();
        }

        string[] animations = { "Grinds", "Manual", "Impact", "Braking" };

        private string GetRandomAnimation(string[] strings)
        {
            System.Random rand = new System.Random();
            int index = rand.Next(strings.Length);
            string randomString = ($"{strings[index]}");
            return randomString;
        }

        public override void Exit()
        {
            PlayerController.Instance.animationController.ScaleAnimSpeed(1f);
            PlayerController.Instance.SetKneeIKTargetWeight(1f);
            if (EventManager.Instance.IsInAir)
            {
                EventManager.Instance.ExitAir();
            }
        }

        public override bool IsInImpactState()
        {
            return true;
        }

        private void CheckForAutoRevert()
        {
            if (!Main.settings.AutoRevert)
            {
                return;
            }

            if (PlayerController.Instance.boardController.Grounded && Vector3.Angle(PlayerController.Instance.boardController.GroundNormal, Vector3.up) > 10f)
            {
                float num = Mathf.Clamp(Vector3.Angle(PlayerController.Instance.boardController.GroundNormal, Vector3.up), 0f, 45f) / 45f;
                _transitioningToPowerslide = true;
                PlayerController.Instance.boardController.AutoPumpLerpedNormal = PlayerController.Instance.boardController.GroundNormal;
                object[] args = new object[]
                {
                PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude * num
                };
                base.DoTransition(typeof(Custom_Powerslide), args);
            }
        }

        public override void Update()
        {
            base.Update();
            if (CanPrimoCatch() && Main.settings.Primos && XXLController.Instance.CanPrimoCatch)
            {
                PlayerController.Instance.SetRightIKLerpTarget(0f, 0f);
                PlayerController.Instance.SetRightSteezeWeight(0f);
                PlayerController.Instance.SetMaxSteezeRight(0f);
                PlayerController.Instance.boardController.ResetAll();
                PlayerController.Instance.SetLeftIKLerpTarget(0f, 0f);
                PlayerController.Instance.SetLeftSteezeWeight(0f);
                PlayerController.Instance.SetMaxSteezeLeft(0f);
                PlayerController.Instance.SetRightIKRotationWeight(1f);
                PlayerController.Instance.SetLeftIKRotationWeight(1f);
                PlayerController.Instance.SetMaxSteeze(0f);
                PlayerController.Instance.SetIKLerpSpeed(4f);
                PlayerController.Instance.AnimCaught(true);
                PlayerController.Instance.AnimRelease(false);
                DoTransition(typeof(Custom_Primo), null);
                return;
            }

            if (PlayerController.Instance.boardController.AllDown)
            {
                PlayerController.Instance.CacheRidingTransforms();
            }
            SoundManager.Instance.SetRollingVolumeFromRPS(PlayerController.Instance.GetSurfaceTag(PlayerController.Instance.boardController.GetSurfaceTagString()), PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude);
            _totalTimeInState += Time.deltaTime;
            if (_totalTimeInState > 5f)
            {
                PlayerController.Instance.ForceBail();
            }
            if (_totalTimeInState > 0.5f && EventManager.Instance.IsInCombo)
            {
                EventManager.Instance.EndTrickCombo(false, false);
            }
            if (!_foundLowest && PlayerController.Instance.skaterController.skaterRigidbody.velocity.y >= 0f)
            {
                _foundLowest = true;
            }
            if (!_foundLowestCom && PlayerController.Instance.comController.COMRigidbody.velocity.y >= 0f)
            {
                _foundLowestCom = true;
            }
            if (!_applyFrictionIncrease)
            {
                _frictionIncreaseTimer += Time.deltaTime;
                if (_frictionIncreaseTimer > 0.07f)
                {
                    _applyFrictionIncrease = true;
                }
            }
            if (_grindTimer < _maxGrindTimer)
            {
                _grindTimer += Time.deltaTime;
            }
            else
            {
                _canGrind = false;
            }
            if (!_impactFinished)
            {
                if (_impactTimer < _maxImpactTime)
                {
                    _impactTimer += Time.deltaTime;
                }
                else
                {
                    _impactFinished = true;
                }
            }
            if (!_colliding && !PlayerController.Instance.IsGrounded() && !PlayerController.Instance.IsRespawning && !PlayerController.Instance.boardController.triggerManager.IsColliding)
            {
                _inAirTimer += Time.deltaTime;
                if (_inAirTimer > 0.2f)
                {
                    PlayerController.Instance.SetTurningMode(InputController.TurningMode.InAir);
                    PlayerController.Instance.SetSkaterToMaster();
                    PlayerController.Instance.AnimSetRollOff(true);
                    PlayerController.Instance.CrossFadeAnimation("Extend", 0.3f);
                    PlayerController.Instance.OnExtendAnimEnter();
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
                }
            }
            else if (PlayerController.Instance.IsGrounded())
            {
                _rollOffWait = 0f;
            }
            if (PlayerController.Instance.IsCurrentAnimationPlaying("Riding") && _totalTimeInState > 0.3f)
            {
                if (!_pumping)
                {
                    base.DoTransition(typeof(Custom_Riding), null);
                }
                else
                {
                    object[] array = new object[]
                    {
                    _pumping,
                    _pumpTimer,
                    _pumpMultiplier,
                    _prevPumpDelta
                    };
                    base.DoTransition(typeof(Custom_Riding), null);
                }
            }
            if (PlayerController.Instance.AngleToBoardTargetRotation() > 89f)
            {
                PlayerController.Instance.ForceBail();
            }
            if (PlayerController.Instance.boardController.AllDown)
            {
                _landed = true;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            PlayerController.Instance.ScalePlayerCollider();
            PlayerController.Instance.boardController.SetBoardControllerUpVector(PlayerController.Instance.skaterController.skaterTransform.up);
            PlayerController.Instance.comController.UpdateCOM(1.04196f, 1);
            HandleFriction();
            PlayerController.AutoRampTags autoRampTags = PlayerController.Instance.AutoPumpAndRevertCheck();
            if (autoRampTags != PlayerController.AutoRampTags.AutoRevert)
            {
                if (autoRampTags == PlayerController.AutoRampTags.AutoPumpAndRevert)
                {
                    CheckForAutoRevert();
                    PlayerController.Instance.AutoPump();
                }
            }
            else
            {
                CheckForAutoRevert();
            }
            PlayerController.Instance.SetRotationTarget();
            PlayerController.Instance.ReduceImpactBounce();
            PlayerController.Instance.boardController.ApplyOnBoardMaxRoll(_colliding, 60f);
            PlayerController.Instance.SkaterRotation(true, false);
            PlayerController.Instance.boardController.DoBoardLean();
            Quaternion.LookRotation(PlayerController.Instance.boardController.boardTransform.forward, PlayerController.Instance.GetGroundNormal());
            if (PlayerController.Instance.IsGrounded())
            {
                Vector3 lhs = Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardTransform.forward, PlayerController.Instance.skaterController.skaterTransform.up);
                if (_velocityOnImpact.magnitude > 0.7f && _totalTimeInState > 0.05f && _totalTimeInState < 0.2f && Mathf.Abs(Vector3.Dot(lhs, _velocityOnImpact)) < 0.4f)
                {
                    PlayerController.Instance.ForceBail();
                }
                if (PlayerController.Instance.boardController.AllDown)
                {
                    PlayerController.Instance.skaterController.skaterRigidbody.angularVelocity = Vector3.zero;
                    if (groundedFrameCount < groundedFrameMax)
                    {
                        groundedFrameCount++;
                    }
                }
            }
            if (!PlayerController.Instance.boardController.AllDown)
            {
                if (Physics.Raycast(PlayerController.Instance.skaterController.skaterTransform.position, -Vector3.up, out _hit, 1.1f, PlayerController.Instance.boardController._layers))
                {
                    PlayerController.Instance.ApplyWeightOnBoard();
                }
                PlayerController.Instance.SnapRotation(PlayerController.Instance.boardController.currentRotationTarget);
            }
            if (PlayerController.Instance.boardController.boardRigidbody.velocity.y > 0f && !PlayerController.Instance.boardController.AllDown)
            {
                Vector3 velocity = PlayerController.Instance.boardController.boardRigidbody.velocity;
                velocity.y = 0f;
                PlayerController.Instance.boardController.boardRigidbody.velocity = velocity;
            }
            _colliding = false;
        }

        private void HandleFriction()
        {
            _friction = Mathf.MoveTowards(_friction, _frictionMax, Time.deltaTime * _frictionIncreaseRate);
            PlayerController.Instance.ApplyFriction(_friction);
        }

        public override void OnCollisionStayEvent(Collision c)
        {
            _colliding = true;
        }

        public override void OnCollisionEnterEvent(Vector3 _impulse, bool _isBoard, Collision c)
        {
            _colliding = true;
        }

        public override void OnStickUpdate(StickInput p_leftStick, StickInput p_rightStick)
        {
            PumpingCheck(p_leftStick, p_rightStick);
            PowerslideCheck(p_leftStick, p_rightStick);
            if ((((double)p_leftStick.ToeAxis > 0.1 && (double)p_rightStick.ToeAxis < -0.1) || (p_leftStick.ToeAxis < -0.1f && p_rightStick.ToeAxis > 0.1f)) && (double)p_leftStick.PopToeVector.magnitude < 0.3)
            {
                float magnitude = p_rightStick.PopToeVector.magnitude;
            }
            PlayerController.Instance.SetLeftIKOffset(p_leftStick.ToeAxis, p_leftStick.ForwardDir, p_leftStick.PopDir, p_leftStick.IsPopStick, true, false);
            PlayerController.Instance.SetRightIKOffset(p_rightStick.ToeAxis, p_rightStick.ForwardDir, p_rightStick.PopDir, p_rightStick.IsPopStick, true, false);
            PlayerController.Instance.SetBoardTargetPosition(0f);
            PlayerController.Instance.SetFrontPivotRotation(0f);
            PlayerController.Instance.SetBackPivotRotation(0f);
            PlayerController.Instance.SetPivotForwardRotation(0f, 40f);
            PlayerController.Instance.SetPivotSideRotation(0f);
            if (_impactFinished && !PlayerController.Instance.WasOnCoping && _totalTimeInState > 0.1f)
            {
                if (_leftHasCentered && (p_leftStick.SetupDir > 0.8f || (new Vector2(p_leftStick.ToeAxis, p_leftStick.SetupDir).magnitude > 0.8f && p_leftStick.SetupDir > 0.325f)))
                {
                    EventManager.Instance.EndTrickCombo(false, false);
                    PlayerController.Instance.AnimSetSetupBlend(0f);
                    PlayerController.Instance.animationController.SetValue("EndImpact", true);
                    PlayerController.Instance.AnimSetNollie(PlayerController.Instance.GetNollie(false));
                    PlayerController.Instance.CrossFadeAnimation("Setup", 0.2f);
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
                if (_rightHasCentered && (p_rightStick.AugmentedSetupDir > 0.8f || (new Vector2(p_rightStick.ToeAxis, p_rightStick.SetupDir).magnitude > 0.8f && p_rightStick.SetupDir > 0.325f)))
                {
                    EventManager.Instance.EndTrickCombo(false, false);
                    PlayerController.Instance.AnimSetSetupBlend(0f);
                    PlayerController.Instance.animationController.SetValue("EndImpact", true);
                    PlayerController.Instance.AnimSetNollie(PlayerController.Instance.GetNollie(true));
                    PlayerController.Instance.CrossFadeAnimation("Setup", 0.2f);
                    PlayerController.Instance.SetupDirection(p_rightStick, ref _setupDir);
                    object[] args2 = new object[]
                    {
                    p_rightStick,
                    p_leftStick,
                    p_rightStick.ForwardDir > 0.2f,
                    _setupDir
                    };
                    base.DoTransition(typeof(Custom_Setup), args2);
                }
            }
        }

        private void PowerslideCheck(StickInput l, StickInput r)
        {
            if (PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude > 0.3f && PowerslideGroundCheck(l, r))
            {
                _transitioningToPowerslide = true;
                object[] args = new object[]
                {
                true
                };
                base.DoTransition(typeof(Custom_Powerslide), args);
            }
        }

        private bool PowerslideGroundCheck(StickInput l, StickInput r)
        {
            return (PlayerController.Instance.boardController.AllDown && ((l.ToeAxis > 0.3f && r.ToeAxis < -0.3f) || (l.ToeAxis < -0.3f && r.ToeAxis > 0.3f))) || (PlayerController.Instance.boardController.Grounded && ((l.ToeAxis > 0.3f && r.ToeAxis < -0.3f && ((l.ForwardDir > 0.3f && r.ForwardDir > 0.3f && PlayerController.Instance.boardController.FrontTwoDown) || (l.ForwardDir < -0.3f && r.ForwardDir < -0.3f && PlayerController.Instance.boardController.BackTwoDown))) || (l.ToeAxis < -0.3f && r.ToeAxis > 0.3f && ((l.ForwardDir > 0.3f && r.ForwardDir > 0.3f && PlayerController.Instance.boardController.FrontTwoDown) || (l.ForwardDir < -0.3f && r.ForwardDir < -0.3f && PlayerController.Instance.boardController.BackTwoDown)))));
        }

        private bool AngleCheck(float _limit, bool _greaterThan)
        {
            if (!_greaterThan)
            {
                return DotProduct() < _limit;
            }
            return DotProduct() > _limit;
        }

        private float DotProduct()
        {
            return Mathf.Abs(Vector3.Dot(Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardRigidbody.velocity.normalized, PlayerController.Instance.boardController.GroundNormal).normalized, PlayerController.Instance.boardController.boardRigidbody.transform.forward));
        }

        private void PumpingCheck(StickInput p_leftStick, StickInput p_rightStick)
        {
            if (!_pumpCheck)
            {
                if (p_leftStick.SetupDir < -0.1f || p_rightStick.SetupDir < -0.1f)
                {
                    _disablePumping = true;
                }
                _pumpCheck = true;
            }
            if (_disablePumping)
            {
                return;
            }
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
            if (!_pumping && PlayerController.Instance.IsGrounded())
            {
                if (_canPumpLeftStick && !_canPumpRightStick)
                {
                    _pumpTarget = Mathf.Lerp(_pumpTargetHigh, _pumpTargetLow, _pumpLeft);
                    if (p_leftStick.PopToeVel.y > 2f)
                    {
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
                    ResetPumpParameters();
                    _pumping = false;
                    PlayerController.Instance.animationController.SetValue("Pumping", false);
                }
            }
            _pumpCom = Mathf.Lerp(_pumpCom, _pumpTarget, (!_pumping) ? (Time.deltaTime * 50f) : (Time.deltaTime * 5f));
        }

        private void ResetPumpParameters()
        {
            _pumpTimer = 0f;
            _pumpLeft = 0f;
            _pumpRight = 0f;
        }

        public override void OnLeftStickCenteredUpdate()
        {
            if (!_leftHasCentered)
            {
                _leftHasCentered = true;
            }
            if (!_pumping)
            {
                _canPumpLeftStick = false;
                _pumpLeft = 0f;
            }
        }

        public override void OnRightStickCenteredUpdate()
        {
            if (!_rightHasCentered)
            {
                _rightHasCentered = true;
            }
            if (!_pumping)
            {
                _canPumpRightStick = false;
                _pumpRight = 0f;
            }
        }

        public override void OnPushButtonPressed(bool p_mongo)
        {
            if (!PlayerController.Instance.IsRespawning)
            {
                object[] args = new object[]
                {
                p_mongo
                };
                base.DoTransition(typeof(Custom_Pushing), args);
            }
        }

        public override void OnBrakePressed()
        {
            if (!PlayerController.Instance.LockBrakes)
            {
                PlayerController.Instance.AnimSetBraking(true);
                base.DoTransition(typeof(Custom_Braking), null);
            }
        }

        public override void OnBrakeHeld()
        {
            if (!PlayerController.Instance.LockBrakes)
            {
                PlayerController.Instance.AnimSetBraking(true);
                base.DoTransition(typeof(Custom_Braking), null);
            }
        }

        public override void OnAllWheelsDown()
        {
            PlayerController.Instance.AnimSetRollOff(false);
        }

        public override void OnFirstWheelDown()
        {
        }

        public override void OnImpactUpdate()
        {
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
            if (_canGrind && !_landed)
            {
                base.DoTransition(typeof(Custom_Grinding), null);
            }
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

            return _canGrind && !_landed;
        }

        public override void OnEndImpact()
        {
            if (!_pumping)
            {
                base.DoTransition(typeof(Custom_Riding), null);
                return;
            }
            object[] array = new object[]
            {
            _pumping,
            _pumpTimer,
            _pumpMultiplier,
            _prevPumpDelta
            };
            base.DoTransition(typeof(Custom_Riding), null);
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


        bool numberGenerated;

        private bool CanPrimoCatch()
        {
            float num = Vector3.Angle(PlayerController.Instance.boardController.boardTransform.right, PlayerController.Instance.skaterController.skaterTransform.up);
            bool b = num < 15f || num > 165f; ////10,170
            bool b2 = false;
            if (b)
            {
                float num2 = Vector3.Angle(PlayerController.Instance.boardController.boardTransform.up, PlayerController.Instance.skaterController.skaterTransform.up);
                b2 = num2 < 100f || num2 > 80f; /////95,85f
            }
            if (b && b2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void CheckForHighImpact()
        {
            if (Main.settings.RealisticDrops)
            {
                float impact = Mathf.Abs(PlayerController.Instance.comController.COMRigidbody.velocity.y) * 1f;
                if (impact > 10)
                {
                    System.Random random = new System.Random();
                    int rand = random.Next(0, 2);
                    if (numberGenerated == false)
                    {
                        numberGenerated = true;
                        if (rand == 1)
                        {
                            PlayerController.Instance.ForceBail();
                        }
                    }
                }
                //Impact Vel between 7 and 10
                else if (impact > 7 && impact < 10)
                {
                    System.Random random = new System.Random();
                    int rand = random.Next(0, 5);
                    if (numberGenerated == false)
                    {
                        numberGenerated = true;
                        if (rand == 2)
                        {
                            PlayerController.Instance.ForceBail();
                        }
                    }
                }
                //Impact Vel between 4 and 7
                else if (impact > 4 && impact < 7)
                {
                    System.Random random = new System.Random();
                    int rand = random.Next(0, 20);
                    if (numberGenerated == false)
                    {
                        numberGenerated = true;
                        if (rand == 2)
                        {
                            PlayerController.Instance.ForceBail();
                        }
                    }
                }
                //Impact Vel between 2 and 4
                else if (impact > 2 && impact < 4)
                {
                    System.Random random = new System.Random();
                    int rand = random.Next(0, 1000);
                    if (numberGenerated == false)
                    {
                        numberGenerated = true;
                        if (rand == 2)
                        {
                            PlayerController.Instance.ForceBail();
                        }
                    }
                }
            }
        }
    }
}
