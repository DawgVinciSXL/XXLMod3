using SkaterXL.Core;
using UnityEngine;
using XXLMod3.Controller;
using XXLMod3.Core;

namespace XXLMod3.PlayerStates
{
    public class Custom_Released : PlayerState_OnBoard
    {
        private bool _backwardsSet;
        private bool _bothCaught;
        private bool _canManual;
        private bool _catchRegistered;
        private bool _caught;
        private bool _caughtLeft;
        private bool _caughtRight;
        private bool _caughtThisIteration;
        private bool _completedRotationAfterCatch;
        private bool _flipDetected;
        private bool _forwardLoad;
        private bool highestFound;
        private bool isExitingState;
        private bool _isRespawning;
        private bool _leftCaughtFirst;
        private bool _leftCentered;
        private bool _manualling;
        private bool _noseManualling;
        private bool _popDirEqualCatchDir;
        private bool _potentialFlip;
        private bool _predictedCollision;
        private bool _rightCaughtFirst;
        private bool _rightCentered;
        private bool _timerEnded;
        private bool _wasGrinding;

        private float _boardHeighest;
        private float _flip;
        private float _flipVel;
        private float _highestSkaterY;
        private float _inAirTurnDelta;
        private float _invertVel;
        private float _leftCenteredTimer;
        private float _leftForwardDir;
        private float _leftToeAxis;
        private float _lMagnitude;
        private float _popDir;
        private float _popVel;
        private float _rightCenteredTimer;
        private float _rightForwardDir;
        private float _rightToeAxis;
        private float _rMagnitude;
        private float _skaterY;
        private float _timer;
        private float _toeAxis;

        private PlayerController.SetupDir _setupDir;

        private StickInput _flipStick;
        private StickInput _popStick;

        private Vector2 _initialFlipDir = Vector2.zero;
        private int _flipFrameCount;
        private int _flipFrameMax;
        private float _augmentedLeftAngle;
        private float _augmentedRightAngle;
        private float _flipWindowTimer;

        public Custom_Released(StickInput p_popStick, StickInput p_flipStick, bool p_forwardLoad, float p_invertVel, PlayerController.SetupDir p_setupDir, float p_inAirTurnDelta)
        {
            _popStick = p_popStick;
            _flipStick = p_flipStick;
            _forwardLoad = p_forwardLoad;
            _invertVel = p_invertVel;
            _setupDir = p_setupDir;
            _inAirTurnDelta = p_inAirTurnDelta;
        }

        public Custom_Released(StickInput p_popStick, StickInput p_flipStick, Vector2 p_initialFlipDir, float p_flipVel, float p_popVel, float p_toeAxis, float p_popDir, bool p_flipDetected, float p_flip, bool p_forwardLoad, float p_invertVel, PlayerController.SetupDir p_setupDir, bool p_catchRegistered, bool p_leftCaughtFirst, bool p_rightCaughtFirst, float p_inAirTurnDelta)
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
            _catchRegistered = p_catchRegistered;
            _leftCaughtFirst = p_leftCaughtFirst;
            _rightCaughtFirst = p_rightCaughtFirst;
            _inAirTurnDelta = p_inAirTurnDelta;
        }

        public Custom_Released(StickInput p_popStick, StickInput p_flipStick, Vector2 p_initialFlipDir, float p_flipVel, float p_popVel, float p_toeAxis, float p_popDir, bool p_flipDetected, float p_flip, bool p_forwardLoad, float p_invertVel, PlayerController.SetupDir p_setupDir, bool p_wasGrinding, bool p_catchRegistered, bool p_leftCaughtFirst, bool p_rightCaughtFirst, float p_inAirTurnDelta)
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
            _wasGrinding = p_wasGrinding;
            _catchRegistered = p_catchRegistered;
            _leftCaughtFirst = p_leftCaughtFirst;
            _rightCaughtFirst = p_rightCaughtFirst;
            _inAirTurnDelta = p_inAirTurnDelta;
        }

        public override void Enter()
        {
            PlayerController.Instance.currentStateEnum = PlayerController.CurrentState.Release;
            XXLController.CurrentState = CurrentState.Released;
            PlayerController.Instance.cameraController.NeedToSlowLerpCamera = true;
            EventManager.Instance.OnReleased(true, true);
            PlayerController.Instance.ToggleFlipTrigger(true);
            _skaterY = PlayerController.Instance.skaterController.skaterTransform.position.y;
            _highestSkaterY = _skaterY;
            if (PlayerController.Instance.inputController.turningMode != InputController.TurningMode.FastLeft && PlayerController.Instance.inputController.turningMode != InputController.TurningMode.FastRight)
            {
                PlayerController.Instance.SetTurningMode(InputController.TurningMode.InAir);
            }
        }

        public override void Exit()
        {
            XXLController.Instance.ResetFlips();
            XXLController.Instance.IsInHardcorePop = false;
            PlayerController.Instance.cameraController.NeedToSlowLerpCamera = false;
            PlayerController.Instance.ToggleFlipTrigger(false);
            PlayerController.Instance.AnimOllieTransition(false);
            PlayerController.Instance.AnimSetRollOff(false);
            PlayerController.Instance.SetIKLerpSpeed(8f);
            PlayerController.Instance.AnimSetNoComply(false);
            PlayerController.Instance.SetMaxSteeze(0f);
            PlayerController.Instance.SetLeftIKLerpTarget(0f);
            PlayerController.Instance.SetRightIKLerpTarget(0f);
            if (EventManager.Instance.IsInAir)
            {
                EventManager.Instance.ExitAir();
            }
        }

        public override void OnPredictedCollisionEvent()
        {
            PredictedNextState();
        }

        public override void OnAllWheelsDown()
        {
            TransitionToNextState();
        }

        public override void OnCollisionStayEvent(Collision c)
        {
            TransitionToNextState();
        }

        public override void OnCollisionEnterEvent(Vector3 _impulse, bool _isBoard, Collision c)
        {
            if (_isBoard)
            {
                SoundManager.Instance.PlayBoardHit(_impulse.magnitude);
            }
            TransitionToNextState();
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

        public override bool IsInReleaseState()
        {
            return true;
        }

        private void PredictedNextState()
        {
            _predictedCollision = true;
            isExitingState = true;
            if (!_caught)
            {
                PlayerController.Instance.ForceBail();
                return;
            }
            CatchBoth();
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
                _popDirEqualCatchDir
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
            isExitingState = true;
            if (!_caught)
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
                _popDirEqualCatchDir
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

        public override void Update()
        {
            if (XXLController.Instance.IsLateFlip)
            {
                PlayerController.Instance.CrossFadeAnimation("Steeze Control", 0.1f);
            }
            HandleBoardControllerUpVector(); ///////////
            HandleHippieOllie();
            if(_caught || _bothCaught || _caughtLeft || _caughtRight)
            {
                HandleLateflip();
                XXLController.Instance.FlipDetected = false;
            }
            SoundManager.Instance.SetRollingVolumeFromRPS(PlayerController.Instance.GetSurfaceTag(PlayerController.Instance.boardController.GetSurfaceTagString()), PlayerController.Instance.boardController._rollSoundSpeed);
            _skaterY = PlayerController.Instance.skaterController.skaterTransform.position.y;
            if (_skaterY > _highestSkaterY)
            {
                _highestSkaterY = _skaterY;
                _boardHeighest = PlayerController.Instance.boardController.boardTransform.position.y;
            }
            else if (!highestFound)
            {
                highestFound = true;
            }
            base.Update();
            if (XXLController.Instance.IsInHardcorePop && (!_caught || !_bothCaught || !_caughtLeft || !_caughtRight))
            {
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
                PlayerController.Instance.CrossFadeAnimation("Extend", 0.1f);
            }
            if(Main.settings.CatchMode == CatchMode.Manual || Main.settings.CatchMode == CatchMode.Realistic)
            {
                if(Main.settings.CatchMode == CatchMode.Realistic)
                {
                    if(PlayerController.Instance.inputController.player.GetButtonDown("Left Stick Button") || PlayerController.Instance.inputController.player.GetButtonDown("Right Stick Button"))
                    {
                        if (!CatchUpRealisticAngleCheck())
                        {
                            PlayerController.Instance.CrossFadeAnimation("Fall", 0.2f);
                            PlayerController.Instance.ForceBail();
                        }
                    }
                }
                if (!_caughtThisIteration && (_caught || _bothCaught || _caughtLeft || _caughtRight))
                {
                    XXLController.Instance.IsInHardcorePop = false;
                    _caughtThisIteration = true;
                    PlayerController.Instance.ToggleFlipColliders(false);
                }
                return;
            }
            if (!_timerEnded)
            {
                if (_timer > 0.15f && CatchForwardAngleCheck() && CatchUpAngleCheck())
                {
                    _timerEnded = true;
                }
                if (_timer < 0.3f)
                {
                    _timer += Time.deltaTime;
                }
                else
                {
                    _timerEnded = true;
                }
            }
            else if (!_caught && _catchRegistered)
            {
                CatchBoth();
            }
            if (!_caught && PlayerController.Instance.DistanceToBoardTarget() > 0.4f)
            {
                PlayerController.Instance.ForceBail();
            }
            if (!_caughtThisIteration && (_caught || _bothCaught || _caughtLeft || _caughtRight))
            {
                if (XXLController.Instance.IsInHardcorePop)
                {
                    XXLController.Instance.IsInHardcorePop = false;
                }
                _caughtThisIteration = true;
                PlayerController.Instance.ToggleFlipColliders(false);
            }
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
            PlayerController.Instance.SetRotationTarget();
            if (!isExitingState)
            {
                PlayerController.Instance.comController.UpdateCOM();
                if (!_caught)
                {
                    PlayerController.Instance.SetMaxSteeze(1f);
                    PlayerController.Instance.SetBoardTargetPosition(0f);
                    PlayerController.Instance.SetFrontPivotRotation(0f);
                    PlayerController.Instance.SetBackPivotRotation(0f);
                    PlayerController.Instance.SetPivotForwardRotation(0f, 0f);
                    PlayerController.Instance.SetPivotSideRotation(0f);
                    PlayerController.Instance.FlipTrickRotation();
                }
                else
                {
                    if (!_caughtLeft || !_caughtRight)
                    {
                        PlayerController.Instance.RotateToCatchRotation();
                    }
                    else
                    {
                        PlayerController.Instance.SnapRotation((_lMagnitude + _rMagnitude) / 2f);
                    }
                    if (!_completedRotationAfterCatch && Quaternion.Angle(PlayerController.Instance.boardController.boardTransform.rotation, PlayerController.Instance.boardController.currentCatchRotationTarget) < 10f)
                    {
                        _completedRotationAfterCatch = true;
                    }
                }
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
        }

        public override bool Popped()
        {
            return true;
        }

        private bool CatchForwardAngleCheck()
        {
            float num = Vector3.Angle(Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardTransform.forward, PlayerController.Instance.skaterController.skaterTransform.up), PlayerController.Instance.skaterController.skaterTransform.forward);
            return num < 35f || num > 145f;
        }

        private bool CatchUpAngleCheck()
        {
            return Vector3.Angle(Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardTransform.up, PlayerController.Instance.skaterController.skaterTransform.forward), PlayerController.Instance.skaterController.skaterTransform.up) < 130f;
        }

        public override StickInput GetPopStick()
        {
            return _popStick;
        }

        public override void OnLeftStickCenteredUpdate()
        {
            if (!_leftCentered)
            {
                _leftCentered = true;
                if (_rightCentered)
                {
                    _rightCaughtFirst = true;
                    _catchRegistered = true;
                }
            }
        }

        public override void OnRightStickCenteredUpdate()
        {
            if (!_rightCentered)
            {
                _rightCentered = true;
                if (_leftCentered)
                {
                    _leftCaughtFirst = true;
                    _catchRegistered = true;
                }
            }
        }

        public override void OnStickUpdate(StickInput p_leftStick, StickInput p_rightStick)
        {
            if (_isRespawning)
            {
                return;
            }
            _lMagnitude = p_leftStick.rawInput.pos.magnitude;
            _rMagnitude = p_rightStick.rawInput.pos.magnitude;
            PlayerController.Instance.SetLeftIKOffset(p_leftStick.ToeAxis, p_leftStick.ForwardDir, p_leftStick.PopDir, p_leftStick.IsPopStick, true, PlayerController.Instance.GetAnimReleased());
            PlayerController.Instance.SetRightIKOffset(p_rightStick.ToeAxis, p_rightStick.ForwardDir, p_rightStick.PopDir, p_rightStick.IsPopStick, true, PlayerController.Instance.GetAnimReleased());
        }

        public override void OnStickFixedUpdate(StickInput p_leftStick, StickInput p_rightStick)
        {
            if (_isRespawning)
            {
                return;
            }
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
                _leftToeAxis = (_caughtLeft ? p_leftStick.ToeAxis : 0f);
                _rightToeAxis = (_caughtRight ? p_rightStick.ToeAxis : 0f);
                _leftForwardDir = (_caughtLeft ? p_leftStick.ForwardDir : 0f);
                _rightForwardDir = (_caughtRight ? p_rightStick.ForwardDir : 0f);
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
                                PlayerController.Instance.SetFrontPivotRotation(_leftToeAxis);
                                PlayerController.Instance.SetBackPivotRotation(_rightToeAxis);
                                PlayerController.Instance.SetPivotForwardRotation((p_leftStick.ForwardDir + p_rightStick.ForwardDir) * 0.7f, 15f);
                                PlayerController.Instance.SetPivotSideRotation(_leftToeAxis - _rightToeAxis);
                                return;
                            }
                            PlayerController.Instance.SetFrontPivotRotation(-_rightToeAxis);
                            PlayerController.Instance.SetBackPivotRotation(-_leftToeAxis);
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

        public override void OnManualUpdate(StickInput p_popStick, StickInput p_flipStick)
        {
            if (_caughtLeft || _caughtRight)
            {
                ForceCatchBoth();
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
            if (_caughtLeft || _caughtRight)
            {
                ForceCatchBoth();
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

        public override void OnGrindDetected()
        {
            if (!Main.settings.Grinds)
            {
                return;
            }

            if (_caught || _catchRegistered)
            {
                PlayerController.Instance.AnimSetManual(false, PlayerController.Instance.AnimGetManualAxis());
                PlayerController.Instance.AnimSetNoseManual(false, PlayerController.Instance.AnimGetManualAxis());
                base.DoTransition(typeof(Custom_Grinding), null);
            }
        }

        public override bool CanGrind()
        {
            if (!Main.settings.Grinds)
            {
                return false;
            }

            return _caught || _catchRegistered;
        }

        private void CatchLeft()
        {
            PlayerController.Instance.boardController.LeaveFlipMode();
            _caughtLeft = true;
            PlayerController.Instance.SetCatchForwardRotation();
            SoundManager.Instance.PlayCatchSound();
            bool boardBackwards = PlayerController.Instance.GetBoardBackwards();
            PlayerController.Instance.SetBoardBackwards();
            PlayerController.Instance.CorrectHandIKRotation(PlayerController.Instance.GetBoardBackwards());
            if (!_backwardsSet)
            {
                _popDirEqualCatchDir = (boardBackwards == PlayerController.Instance.GetBoardBackwards());
            }
            _backwardsSet = true;
            PlayerController.Instance.boardController.ResetAll();
            _flipDetected = false;
            _caught = true;
            PlayerController.Instance.SetIKLerpSpeed(4f);
            PlayerController.Instance.SetLeftIKLerpTarget(0f, 0f);
            PlayerController.Instance.SetLeftSteezeWeight(0f);
            PlayerController.Instance.SetMaxSteezeLeft(0f);
        }

        private void CatchRight()
        {
            PlayerController.Instance.boardController.LeaveFlipMode();
            _caughtRight = true;
            PlayerController.Instance.SetCatchForwardRotation();
            SoundManager.Instance.PlayCatchSound();
            bool boardBackwards = PlayerController.Instance.GetBoardBackwards();
            PlayerController.Instance.SetBoardBackwards();
            PlayerController.Instance.CorrectHandIKRotation(PlayerController.Instance.GetBoardBackwards());
            if (!_backwardsSet)
            {
                _popDirEqualCatchDir = (boardBackwards == PlayerController.Instance.GetBoardBackwards());
            }
            _backwardsSet = true;
            PlayerController.Instance.boardController.ResetAll();
            _flipDetected = false;
            _caught = true;
            PlayerController.Instance.SetIKLerpSpeed(4f);
            PlayerController.Instance.SetRightIKLerpTarget(0f, 0f);
            PlayerController.Instance.SetRightSteezeWeight(0f);
            PlayerController.Instance.SetMaxSteezeRight(0f);
        }

        private void CatchBoth()
        {
            PlayerController.Instance.boardController.LeaveFlipMode();
            if (!_caught)
            {
                PlayerController.Instance.SetCatchForwardRotation();
            }
            _flipDetected = false;
            _caught = true;
            _caughtLeft = true;
            _caughtRight = true;
            if (!_bothCaught)
            {
                SoundManager.Instance.PlayCatchSound();
                _bothCaught = true;
                bool boardBackwards = PlayerController.Instance.GetBoardBackwards();
                PlayerController.Instance.SetBoardBackwards();
                PlayerController.Instance.CorrectHandIKRotation(PlayerController.Instance.GetBoardBackwards());
                if (!_backwardsSet)
                {
                    _popDirEqualCatchDir = (boardBackwards == PlayerController.Instance.GetBoardBackwards());
                }
                _backwardsSet = true;
                EventManager.Instance.OnCatched(true, true);
            }
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
            PlayerController.Instance.CrossFadeAnimation("Extend", 0.15f);
            PlayerController.Instance.OnExtendAnimEnter();
            PlayerController.Instance.AnimRelease(false);
        }

        private void ForceCatchBoth()
        {
            if (!_caughtRight || !_caughtLeft)
            {
                EventManager.Instance.OnCatched(true, true);
            }
            _caughtRight = true;
            _caughtLeft = true;
            _flipDetected = false;
            PlayerController.Instance.SetRightIKLerpTarget(0f);
            PlayerController.Instance.SetLeftIKLerpTarget(0f);
            PlayerController.Instance.SetRightIKRotationWeight(1f);
            PlayerController.Instance.SetLeftIKRotationWeight(1f);
            PlayerController.Instance.boardController.ResetAll();
            PlayerController.Instance.SetMaxSteeze(0f);
            PlayerController.Instance.AnimCaught(true);
            PlayerController.Instance.SetIKLerpSpeed(4f);
            PlayerController.Instance.CrossFadeAnimation("Extend", 0.15f);
            PlayerController.Instance.OnExtendAnimEnter();
            PlayerController.Instance.AnimRelease(false);
            PlayerController.Instance.OnExtendAnimEnter();
        }

        public override void OnStickPressed(bool p_right)
        {
            if (p_right)
            {
                if (!_caughtRight)
                {
                    _caughtRight = true;
                    EventManager.Instance.OnCatched(_caughtRight, _caughtLeft);
                }
                if (!_caught)
                {
                    PlayerController.Instance.boardController.LeaveFlipMode();
                    PlayerController.Instance.SetCatchForwardRotation();
                    SoundManager.Instance.PlayCatchSound();
                    bool boardBackwards = PlayerController.Instance.GetBoardBackwards();
                    PlayerController.Instance.SetBoardBackwards();
                    PlayerController.Instance.CorrectHandIKRotation(PlayerController.Instance.GetBoardBackwards());
                    if (!_backwardsSet)
                    {
                        _popDirEqualCatchDir = (boardBackwards == PlayerController.Instance.GetBoardBackwards());
                    }
                    _backwardsSet = true;
                    PlayerController.Instance.boardController.ResetAll();
                    _flipDetected = false;
                    _caught = true;
                    PlayerController.Instance.SetIKLerpSpeed(4f);
                    PlayerController.Instance.SetRightIKLerpTarget(0f, 0f);
                    PlayerController.Instance.SetRightSteezeWeight(0f);
                    PlayerController.Instance.SetMaxSteezeRight(0f);
                }
            }
            else
            {
                if (!_caughtLeft)
                {
                    _caughtLeft = true;
                    EventManager.Instance.OnCatched(_caughtRight, _caughtLeft);
                }
                if (!_caught)
                {
                    PlayerController.Instance.boardController.LeaveFlipMode();
                    PlayerController.Instance.SetCatchForwardRotation();
                    SoundManager.Instance.PlayCatchSound();
                    bool boardBackwards2 = PlayerController.Instance.GetBoardBackwards();
                    PlayerController.Instance.SetBoardBackwards();
                    PlayerController.Instance.CorrectHandIKRotation(PlayerController.Instance.GetBoardBackwards());
                    if (!_backwardsSet)
                    {
                        _popDirEqualCatchDir = (boardBackwards2 == PlayerController.Instance.GetBoardBackwards());
                    }
                    _backwardsSet = true;
                    PlayerController.Instance.boardController.ResetAll();
                    _flipDetected = false;
                    _caught = true;
                    PlayerController.Instance.SetIKLerpSpeed(4f);
                    PlayerController.Instance.SetLeftIKLerpTarget(0f, 0f);
                    PlayerController.Instance.SetLeftSteezeWeight(0f);
                    PlayerController.Instance.SetMaxSteezeLeft(0f);
                }
            }
            if (_caughtLeft && _caughtRight)
            {
                if (!_bothCaught)
                {
                    SoundManager.Instance.PlayCatchSound();
                    _bothCaught = true;
                }
                PlayerController.Instance.SetRightIKRotationWeight(1f);
                PlayerController.Instance.SetLeftIKRotationWeight(1f);
                PlayerController.Instance.SetMaxSteeze(0f);
                PlayerController.Instance.AnimCaught(true);
                PlayerController.Instance.SetIKLerpSpeed(4f);
                PlayerController.Instance.CrossFadeAnimation("Extend", 0.15f);
                PlayerController.Instance.OnExtendAnimEnter();
                PlayerController.Instance.AnimRelease(false);
                PlayerController.Instance.OnExtendAnimEnter();
            }
            if (_bothCaught && Main.settings.StompCatch)
            {
                PlayerController.Instance.skaterController.skaterRigidbody.AddForce(-PlayerController.Instance.skaterController.skaterTransform.up * Main.settings.StompCatchForce, ForceMode.Impulse);
            }
        }

        public override void OnNextState()
        {
            object[] args = new object[]
            {
            _inAirTurnDelta
            };
            base.DoTransition(typeof(Custom_InAir), args);
        }

        public override void OnLBDown()
        {
            if (!Main.settings.Grabs)
            {
                return;
            }

            if (!Main.settings.GrabDelay && CatchGrabForwardAngleCheck() && CatchUpGrabAngleCheck())
            {
                PlayerController.Instance.SetRightIKLerpTarget(1f, 1f);
                PlayerController.Instance.SetRightSteezeWeight(0f);
                PlayerController.Instance.SetMaxSteezeRight(0f);
                PlayerController.Instance.SetLeftIKLerpTarget(1f, 1f);
                PlayerController.Instance.SetLeftSteezeWeight(0f);
                PlayerController.Instance.SetMaxSteezeLeft(0f);
                object[] args = new object[]
                {
                true,
                                _inAirTurnDelta
                };
                base.DoTransition(typeof(Custom_Grabs), args);
                return;
            }

            if (_caught && _completedRotationAfterCatch)
            {
                CatchBoth();
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

            if (!Main.settings.GrabDelay && CatchGrabForwardAngleCheck() && CatchUpGrabAngleCheck())
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
                return;
            }

            if (_caught && _completedRotationAfterCatch)
            {
                CatchBoth();
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
            if (!Main.settings.GrabDelay && CatchGrabForwardAngleCheck() && CatchUpGrabAngleCheck())
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
                return;
            }

            if (_caught && _completedRotationAfterCatch)
            {
                CatchBoth();
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
            if (!Main.settings.GrabDelay && CatchGrabForwardAngleCheck() && CatchUpGrabAngleCheck())
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
                return;
            }

            if (_caught && _completedRotationAfterCatch)
            {
                CatchBoth();
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

        private bool CanGrab()
        {
            return PlayerController.Instance.IsCurrentAnimationPlaying("Extend") || PlayerController.Instance.IsCurrentAnimationPlaying("Steeze Control");
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


        private bool CatchUpGrabAngleCheck()
        {
            return Vector3.Angle(Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardTransform.up, PlayerController.Instance.skaterController.skaterTransform.forward), PlayerController.Instance.skaterController.skaterTransform.up) < 70f;
        }

        private bool CatchGrabForwardAngleCheck()
        {
            float num = Vector3.Angle(Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardTransform.forward, PlayerController.Instance.skaterController.skaterTransform.up), PlayerController.Instance.skaterController.skaterTransform.forward);
            return num < 25f || num > 155f;
        }

        private bool CatchUpRealisticAngleCheck()
        {
            return Vector3.Angle(Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardTransform.up, PlayerController.Instance.skaterController.skaterTransform.forward), PlayerController.Instance.skaterController.skaterTransform.up) < 90f;
        }

        private void HandleBoardControllerUpVector()
        {
            if (!Main.settings.Primos)
            {
                PlayerController.Instance.boardController.SetBoardControllerUpVector(PlayerController.Instance.skaterController.skaterTransform.up);
                return;
            }

            if (_caught)
            {
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

        private void HandleLateflip()
        {
            PlayerController.Instance.boardController.ReferenceBoardRotation();
            PlayerController.Instance.FixTargetNormal();
            PlayerController.Instance.AnimOllieTransition(true);

            if (_flipDetected)
            {
                PlayerController.Instance.ResetAllAnimations();
                PlayerController.Instance.SetRightIKWeight(1);
                PlayerController.Instance.SetLeftIKWeight(1);
                object[] args = new object[]
                {
                _popStick,
                _flipStick,
                _initialFlipDir,
                0,
                0,
                _toeAxis,
                0,
                _flipDetected,
                _flip,
                -1f,
                true,
                0,
                _setupDir,
                _augmentedLeftAngle,
                _augmentedRightAngle,
                true,
                false
                };
                base.DoTransition(typeof(Custom_Lateflip), args);
                return;
            }
        }

        public override void OnFlipStickUpdate()
        {
            if (Main.settings.Lateflips)
            {
                //if(!PlayerController.Instance.inputController.player.GetButton("Left Stick Button") || !PlayerController.Instance.inputController.player.GetButton("Left Stick Button"))
                //{
                //    float num = 0f;
                //    PlayerController.Instance.OnFlipStickUpdate(ref _flipDetected, ref _potentialFlip, ref _initialFlipDir, ref _flipFrameCount, ref _flipFrameMax, ref _toeAxis, ref _flipVel, ref _popVel, ref _popDir, ref _flip, _flipStick, false, false, ref _invertVel, _popStick.IsRightStick ? _augmentedLeftAngle : _augmentedRightAngle, false, _forwardLoad, ref _flipWindowTimer);
                //}

                float num = 0f;
                if (PlayerController.Instance.inputController.player.GetButton("Right Stick Button"))
                {
                    PlayerController.Instance.OnFlipStickUpdate(ref _flipDetected, ref _potentialFlip, ref _initialFlipDir, ref _flipFrameCount, ref _flipFrameMax, ref _toeAxis, ref _flipVel, ref _popVel, ref _popDir, ref _flip, _popStick, true, false, ref _invertVel, _popStick.IsRightStick ? _augmentedLeftAngle : _augmentedRightAngle, false, _forwardLoad, ref num);

                    PlayerController.Instance.OnFlipStickUpdate(ref _flipDetected, ref _potentialFlip, ref _initialFlipDir, ref _flipFrameCount, ref _flipFrameMax, ref _toeAxis, ref _flipVel, ref _popVel, ref _popDir, ref _flip, _flipStick, true, false, ref _invertVel, _popStick.IsRightStick ? _augmentedLeftAngle : _augmentedRightAngle, false, _forwardLoad, ref num);
                }
                else if (PlayerController.Instance.inputController.player.GetButton("Left Stick Button"))
                {
                    PlayerController.Instance.OnFlipStickUpdate(ref _flipDetected, ref _potentialFlip, ref _initialFlipDir, ref _flipFrameCount, ref _flipFrameMax, ref _toeAxis, ref _flipVel, ref _popVel, ref _popDir, ref _flip, _popStick, true, false, ref _invertVel, _popStick.IsRightStick ? _augmentedLeftAngle : _augmentedRightAngle, false, _forwardLoad, ref num);

                    PlayerController.Instance.OnFlipStickUpdate(ref _flipDetected, ref _potentialFlip, ref _initialFlipDir, ref _flipFrameCount, ref _flipFrameMax, ref _toeAxis, ref _flipVel, ref _popVel, ref _popDir, ref _flip, _flipStick, true, false, ref _invertVel, _popStick.IsRightStick ? _augmentedLeftAngle : _augmentedRightAngle, false, _forwardLoad, ref num);
                }
            }
        }
    }
}