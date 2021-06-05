using Dreamteck.Splines;
using SkaterXL.Core;
using SkaterXL.TrickDetection;
using UnityEngine;
using XXLMod3.Core;
using XXLMod3.Controller;

namespace XXLMod3.PlayerStates
{
    public class Custom_Darkslide : PlayerState_OnBoard
    {
        private Vector2 _initialFlipDir = Vector2.zero;
        private float _popForce = 2f;
        private Vector3 _lastVelocity = Vector3.zero;
        private int _flipFrameMax = 25;
        private bool _canGrind = true;
        private Vector3 _grindDirection = Vector3.zero;
        private bool _isLeftActuallyLeft = true;
        private float _maxStallVelocity = 4f;
        private float _tweak;
        private StickInput _popStick;
        private StickInput _flipStick;
        private bool _forwardLoad;
        private bool _flipDetected;
        private bool _potentialFlip;
        private float _toeAxis;
        private float _flipVel;
        private float _popVel;
        private float _popDir;
        private float _flip;
        private bool _popped;
        private float _invertVel;
        private PlayerController.SetupDir _setupDir;
        private float _grindTimer;
        private bool _waitDone;
        private float _augmentedLeftAngle;
        private float _augmentedRightAngle;
        private float _initialVelocityMagnitude;
        private int _flipFrameCount;
        private float _leftTrigger;
        private float _rightTrigger;
        private float _triggerDif;
        private float _leftTriggerTimer;
        private float _rightTriggerTimer;
        private float _bothTriggerTimer;
        private SplineComputer _spline;
        private bool _wasSliding;
        private float _flipWindowTimer;
        private float _grindTime;
        private bool _grindDetected;
        private bool _overrideSpline;
        private bool _coping;
        private bool _grindingBackwards;
        private bool _goingForward;
        private bool _leftTriggerHasBeenReleased;
        private bool _rightTriggerHasBeenReleased;
        private float _timeInState;
        private Vector3 _vel;
        private Vector3 _angVel;
        private bool _bothTriggersPressed;
        private bool _leftTriggerPressed;
        private bool _rightTriggerPressed;
        private bool _bothReleased;
        private float _horizontalTarget;
        private float _horizontalLerp;
        private float _verticalTarget;
        private float _verticalLerp;
        private bool _bluntStart;
        private float _playerTurn;

        public Custom_Darkslide()
        {
            SoundManager.Instance.PlayGrindSound(PlayerController.Instance.boardController.isSliding ? 1 : PlayerController.Instance.boardController.GetGrindSoundInt(), Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardRigidbody.velocity, Vector3.up).magnitude);
        }

        public Custom_Darkslide(SplineComputer p_spline)
        {
            SoundManager.Instance.PlayGrindSound(PlayerController.Instance.boardController.isSliding ? 1 : PlayerController.Instance.boardController.GetGrindSoundInt(), Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardRigidbody.velocity, Vector3.up).magnitude);
            this._overrideSpline = true;
            this._spline = p_spline;
        }

        public Custom_Darkslide(
          SplineComputer p_spline,
          bool p_coping,
          float p_horizontalTarget,
          float p_verticalTarget,
          float p_horizontalLerp,
          float p_verticalLerp)
        {
            this._horizontalTarget = p_horizontalTarget;
            this._verticalTarget = p_verticalTarget;
            this._horizontalLerp = p_horizontalLerp;
            this._verticalLerp = p_verticalLerp;
            this._coping = true;
            this._overrideSpline = true;
            this._spline = p_spline;
        }

        public override void Enter()
        {
            PlayerController.Instance.currentStateEnum = PlayerController.CurrentState.Grinding;
            XXLController.CurrentState = CurrentState.Grinding;
            PlayerController.Instance.cameraController.IsInGrindState = true;
            PlayerController.Instance.cameraController.NeedToSlowLerpCamera = this._overrideSpline;
            PlayerController.Instance.cameraController.IsInCopingState = this._overrideSpline;
            if ((Object)this._spline == (Object)null)
                this._spline = PlayerController.Instance.boardController.triggerManager.spline;
            PlayerController.Instance.ToggleFlipColliders(false);
            //PlayerController.Instance.SetBoardPhysicsMaterial(PlayerController.FrictionType.Grind);
            foreach (Collider collider in PlayerController.Instance.boardController.boardTransform.GetComponentsInChildren<Collider>())
            {
                collider.material = XXLController.GrindPhysicsMaterial;
            }
            this._grindDirection = PlayerController.Instance.boardController.triggerManager.grindDirection;
            this._lastVelocity = PlayerController.Instance.VelocityOnPop;
            Vector3 velocity = PlayerController.Instance.boardController.boardRigidbody.velocity;
            velocity.y = 0.0f;
            PlayerController.Instance.boardController.boardRigidbody.velocity = velocity;
            PlayerController.Instance.SetIKRigidbodyKinematic(false);
            PlayerController.Instance.boardController.ResetAll();
            this._initialVelocityMagnitude = this._lastVelocity.magnitude;
            this._wasSliding = PlayerController.Instance.boardController.isSliding;
            SplineResult p_splineResult = this._spline.Evaluate(this._spline.Project(PlayerController.Instance.boardController.boardTransform.position, 3, 0.0, 1.0));
            if (!this._coping)
            {
                HandlePopOutDirection();
                PlayerController.Instance.boardController.triggerManager.grindDetection.grindSide = PlayerController.Instance.IsBacksideGrind(p_splineResult) ? GrindSide.Backside : GrindSide.Frontside;
            }
            else
            {
                this.CorrectVelocity(PlayerController.Instance.boardController.boardRigidbody);
                this.CorrectVelocity(PlayerController.Instance.boardController.backTruckRigidbody);
                this.CorrectVelocity(PlayerController.Instance.boardController.frontTruckRigidbody);
            }
            if (!PlayerController.Instance.IsRightSameSide())
                this._isLeftActuallyLeft = false;
            PlayerController.Instance.cameraController.SetCameraSide(PlayerController.Instance.boardController.triggerManager.sideEnteredGrind == TriggerManager.SideEnteredGrind.Left ? (this._isLeftActuallyLeft ? 0.0f : 1f) : (this._isLeftActuallyLeft ? 1f : 0.0f), 1f);
            this._tweak = PlayerController.Instance.GetLastTweakAxis();
            PlayerController.Instance.SetGrindTweakAxis(this._tweak);
            PlayerController.Instance.SetTurnMultiplier(Main.settings.GrindTurnSpeed);
            PlayerController.Instance.SetTurningMode(InputController.TurningMode.None);
            PlayerController.Instance.AnimGrindTransition(true);
            if (!this._coping)
                PlayerController.Instance.CrossFadeAnimation("Grinds", 0.2f);
            PlayerController.Instance.AnimOllieTransition(false);
            PlayerController.Instance.AnimSetupTransition(false);
            PlayerController.Instance.AnimRelease(false);
            PlayerController.Instance.SetBoardToMaster();
            PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
            PlayerController.Instance.SetLeftIKLerpTarget(0.0f);
            PlayerController.Instance.SetRightIKLerpTarget(0.0f);
            PlayerController.Instance.animationController.ScaleAnimSpeed(1f);
            this._vel = PlayerController.Instance.boardController.boardRigidbody.velocity;
            this._angVel = PlayerController.Instance.boardController.boardRigidbody.angularVelocity;
            if (!SkaterXL.Core.Mathd.Vector3IsInfinityOrNan(PlayerController.Instance.GetGrindContactPosition()))
                PlayerController.Instance.SetBoardCenterOfMass(PlayerController.Instance.boardController.boardTransform.InverseTransformPoint(PlayerController.Instance.GetGrindContactPosition()));
            PlayerController.Instance.boardController.boardRigidbody.angularVelocity = this._angVel;
            if (!this._coping && !this.VelocityCheckNew(this._grindDirection))
            {
                PlayerController.Instance.playerSM.OnGrindEndedSM();
            }
            else
            {
                this._goingForward = (double)Vector3.Dot(PlayerController.Instance.boardController.boardRigidbody.velocity, this._grindDirection) >= 0.0;
                EventManager.Instance.EnterGrind(this._spline ?? PlayerController.Instance.boardController.triggerManager.spline);
            }
        }

        public override void Update()
        {
            base.Update();
            DoStance();
            XXLController.GrindPhysicsMaterial.dynamicFriction = GrindFriction;
            DetermineGrind();
            HandleCrouch();
            HandleStall();
            OneFootGrind();
            XXLController.Instance.GrindPopOutSidewayForce = SidewayPopForce;
            XXLController.Instance.GrindStabilizer = Stabilizer;
            this._timeInState += Time.deltaTime;
            this._canGrind = !((Object)this._spline != (Object)PlayerController.Instance.boardController.triggerManager.spline);
            SoundManager.Instance.SetRollingVolumeFromRPS(PlayerController.Instance.GetSurfaceTag(PlayerController.Instance.boardController.GetSurfaceTagString()), PlayerController.Instance.boardController._rollSoundSpeed);
            if (!this._waitDone)
            {
                this._grindTimer += Time.deltaTime;
                if ((double)this._grindTimer > 0.400000005960464)
                {
                    PlayerController.Instance.AnimSetGrinding(true);
                    this._waitDone = true;
                }
            }
            this._grindTime += Time.deltaTime;
            SoundManager.Instance.SetGrindVolume(PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude);
            if (!PlayerController.Instance.IsCurrentGrindMetal())
            {
                if (this._wasSliding && !PlayerController.Instance.boardController.isSliding)
                {
                    SoundManager.Instance.StopGrindSound(!SkaterXL.Core.Mathd.IsInfinityOrNaN(this._lastVelocity.magnitude) ? this._lastVelocity.magnitude : 0.0f);
                    SoundManager instance = SoundManager.Instance;
                    int p_grindType = PlayerController.Instance.boardController.isSliding ? 1 : PlayerController.Instance.boardController.GetGrindSoundInt();
                    Vector3 vector3 = Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardRigidbody.velocity, Vector3.up);
                    double magnitude = (double)vector3.magnitude;
                    instance.PlayGrindSound(p_grindType, (float)magnitude);
                    vector3 = PlayerController.Instance.boardController.boardRigidbody.velocity;
                    this._initialVelocityMagnitude = vector3.magnitude;
                    this._wasSliding = false;
                }
                else if (!this._wasSliding && PlayerController.Instance.boardController.isSliding)
                {
                    SoundManager.Instance.StopGrindSound(!SkaterXL.Core.Mathd.IsInfinityOrNaN(this._lastVelocity.magnitude) ? this._lastVelocity.magnitude : 0.0f);
                    SoundManager instance = SoundManager.Instance;
                    int p_grindType = PlayerController.Instance.boardController.isSliding ? 1 : PlayerController.Instance.boardController.GetGrindSoundInt();
                    Vector3 vector3 = Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardRigidbody.velocity, Vector3.up);
                    double magnitude = (double)vector3.magnitude;
                    instance.PlayGrindSound(p_grindType, (float)magnitude);
                    vector3 = PlayerController.Instance.boardController.boardRigidbody.velocity;
                    this._initialVelocityMagnitude = vector3.magnitude;
                    this._wasSliding = true;
                }
            }
            this.Lean();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            PlayerController.Instance.ScalePlayerCollider();
            PlayerController.Instance.boardController.SetBoardControllerUpVector(PlayerController.Instance.skaterController.skaterTransform.up);
            PlayerController.Instance.boardController.GroundY = Mathf.Lerp(PlayerController.Instance.boardController.GroundY, PlayerController.Instance.boardController.triggerManager.grindContactSplinePosition.position.y, Time.deltaTime * (this._coping ? 1f : 10f));
            this._vel = PlayerController.Instance.boardController.boardRigidbody.velocity;
            this._angVel = PlayerController.Instance.boardController.boardRigidbody.angularVelocity;
            if ((double)(PlayerController.Instance.GetGrindContactPosition() - PlayerController.Instance.boardController.boardRigidbody.worldCenterOfMass).magnitude > 0.0500000007450581)
                PlayerController.Instance.SetBoardCenterOfMass(PlayerController.Instance.boardController.boardTransform.InverseTransformPoint(PlayerController.Instance.GetGrindContactPosition()));
            PlayerController.Instance.boardController.boardRigidbody.angularVelocity = this._angVel;
            this._grindDirection = PlayerController.Instance.boardController.triggerManager.grindDirection;
            this.CheckGrindingBackwards();
            PlayerController.Instance.BoardGrindRotation = Quaternion.Slerp(PlayerController.Instance.boardController.boardRigidbody.rotation, PlayerController.Instance.GetBoardBackwards() ? PlayerController.Instance.boardController.triggerManager.grindOffset.rotation : PlayerController.Instance.boardOffsetRoot.rotation, Time.fixedDeltaTime * 60f);
            PlayerController.Instance.LockAngularVelocity(PlayerController.Instance.BoardGrindRotation);
            this._lastVelocity = PlayerController.Instance.boardController.boardRigidbody.velocity;
            PlayerController.Instance.SetRotationTarget();
            PlayerController.Instance.SkaterRotation(PlayerController.Instance.PlayerGrindRotation, 1f);
            PlayerController.Instance.comController.UpdateCOM(DefaultCrouchAmount, 0);
        }

        private void CheckGrindingBackwards()
        {
            Vector3 to = this._goingForward ? this._grindDirection : -this._grindDirection;
            if ((double)this._vel.magnitude < 0.100000001490116 || (double)Vector3.Dot(this._vel, to.normalized) < 0.5)
                to = Vector3.Project(PlayerController.Instance.cameraController._actualCam.forward, this._grindDirection).normalized;
            if (PlayerController.Instance.boardController.triggerManager.grindDetection.grindType.IsSlide())
                this._grindingBackwards = (double)Mathf.Abs(Vector3.Angle(Vector3.ProjectOnPlane(PlayerController.Instance.boardController.GetClosestBoardForward(), PlayerController.Instance.boardController.triggerManager.grindUp), to)) > (PlayerController.Instance.IsSwitch ? 45.0 : 135.0);
            else
                this._grindingBackwards = (double)Mathf.Abs(Vector3.Angle(Vector3.ProjectOnPlane(PlayerController.Instance.boardController.GetClosestBoardForward(), PlayerController.Instance.boardController.triggerManager.grindUp), to)) > (PlayerController.Instance.IsSwitch ? 75.0 : 105.0);
        }

        public override void Exit()
        {
            PlayerController.Instance.cameraController.IsInGrindState = false;
            PlayerController.Instance.cameraController.IsInCopingState = false;
            PlayerController.Instance.cameraController.NeedToSlowLerpCamera = false;
            PlayerController.Instance.SetBoardPhysicsMaterial(PlayerController.FrictionType.Default);
            PlayerController.Instance.comController.UpdateCOM(0.89f, 0);
            PlayerController.Instance.skaterController.InitializeSkateRotation();
            PlayerController.Instance.skaterController.skaterRigidbody.angularVelocity = Vector3.zero;
            PlayerController.Instance.boardController.triggerManager.spline = (SplineComputer)null;
            SoundManager.Instance.StopGrindSound(!SkaterXL.Core.Mathd.IsInfinityOrNaN(this._lastVelocity.magnitude) ? this._lastVelocity.magnitude : 0.0f);
            PlayerController.Instance.ResetBoardCenterOfMass();
            PlayerController.Instance.ResetBackTruckCenterOfMass();
            PlayerController.Instance.ResetFrontTruckCenterOfMass();
            SoundManager.Instance.StopPowerslideSound(1, Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardRigidbody.velocity, Vector3.up).magnitude);
            PlayerController.Instance.cameraController.SetCameraSide(PlayerController.Instance.boardController.triggerManager.sideEnteredGrind == TriggerManager.SideEnteredGrind.Left ? (this._isLeftActuallyLeft ? 0.0f : 1f) : (this._isLeftActuallyLeft ? 1f : 0.0f), 0.0f);
            if (!this._popped)
                PlayerController.Instance.AnimSetPopStrength(0.0f);
            EventManager.Instance.ExitGrind();
        }

        private bool VelocityCheckNew(Vector3 _grindDirection)
        {
            return (double)Vector3.ProjectOnPlane(Vector3.ProjectOnPlane(PlayerController.Instance.VelocityOnPop, Vector3.up), Vector3.ProjectOnPlane(_grindDirection, Vector3.up)).magnitude < (double)this._maxStallVelocity;
        }

        public override void SetSpline(SplineComputer p_spline)
        {
            if (this._overrideSpline)
                return;
            this._spline = p_spline;
        }

        public override bool IsCurrentSpline(SplineComputer p_spline)
        {
            return this._overrideSpline || (Object)this._spline == (Object)p_spline;
        }

        public override Vector3 GetClosestSplinePos()
        {
            return this._spline.position;
        }

        public override bool IsGrinding()
        {
            return true;
        }

        public override bool IsInGrindStateOnCoping()
        {
            return this._overrideSpline;
        }

        private void Lean()
        {
            if ((double)this._leftTrigger <= 0.300000011920929 || (double)this._rightTrigger <= 0.300000011920929)
                return;
            this._bothTriggersPressed = true;
            this._bothReleased = false;
            this._bothTriggerTimer += Time.deltaTime;
        }

        public override float GetAugmentedAngle(StickInput p_stick)
        {
            if ((double)p_stick.PopToeSpeed < 10.0)
            {
                if (p_stick.IsRightStick)
                {
                    if (p_stick.IsPopStick)
                    {
                        PlayerController.Instance.DebugAugmentedAngles(p_stick.augmentedInput.pos, true);
                        this._augmentedRightAngle = PlayerController.Instance.GetAngleToAugment(p_stick.rawInput.pos, true);
                        return PlayerController.Instance.GetAngleToAugment(p_stick.rawInput.pos, true);
                    }
                    if ((Object)PlayerController.Instance.playerSM.GetPopStickSM() == (Object)null)
                    {
                        PlayerController.Instance.DebugAugmentedAngles(p_stick.augmentedInput.pos, true);
                        this._augmentedRightAngle = PlayerController.Instance.GetAngleToAugment(p_stick.rawInput.pos, true);
                        return PlayerController.Instance.GetAngleToAugment(p_stick.rawInput.pos, true);
                    }
                    PlayerController.Instance.DebugAugmentedAngles(p_stick.augmentedInput.pos, false);
                    this._augmentedRightAngle = PlayerController.Instance.GetAngleToAugment(p_stick.rawInput.pos, false);
                    return PlayerController.Instance.GetAngleToAugment(p_stick.rawInput.pos, false);
                }
                if (p_stick.IsPopStick)
                {
                    PlayerController.Instance.DebugAugmentedAngles(p_stick.augmentedInput.pos, false);
                    this._augmentedLeftAngle = PlayerController.Instance.GetAngleToAugment(p_stick.rawInput.pos, false);
                    return PlayerController.Instance.GetAngleToAugment(p_stick.rawInput.pos, false);
                }
                if ((Object)PlayerController.Instance.playerSM.GetPopStickSM() == (Object)null)
                {
                    PlayerController.Instance.DebugAugmentedAngles(p_stick.augmentedInput.pos, false);
                    this._augmentedLeftAngle = PlayerController.Instance.GetAngleToAugment(p_stick.rawInput.pos, false);
                    return PlayerController.Instance.GetAngleToAugment(p_stick.rawInput.pos, false);
                }
                PlayerController.Instance.DebugAugmentedAngles(p_stick.augmentedInput.pos, true);
                this._augmentedLeftAngle = PlayerController.Instance.GetAngleToAugment(p_stick.rawInput.pos, true);
                return PlayerController.Instance.GetAngleToAugment(p_stick.rawInput.pos, true);
            }
            return p_stick.IsRightStick ? this._augmentedRightAngle : this._augmentedLeftAngle;
        }

        public override StickInput GetPopStick()
        {
            return this._popStick;
        }

        public override void OnStickUpdate(StickInput p_leftStick, StickInput p_rightStick)
        {
            if (SettingsManager.Instance.stance == SkaterXL.Core.Stance.Regular)
            {
                switch (SettingsManager.Instance.controlType)
                {
                    case ControlType.Same:
                        if (PlayerController.Instance.CanOllieOutOfGrind() && PlayerController.Instance.CanNollieOutOfGrind())
                        {
                            if ((double)p_leftStick.PopToeSpeed > 8.0)
                            {
                                if ((Object)this._popStick != (Object)p_leftStick)
                                    this._popStick = p_leftStick;
                                this._flipStick = p_rightStick;
                            }
                            if ((double)p_rightStick.PopToeSpeed > 8.0)
                            {
                                if ((Object)this._popStick != (Object)p_rightStick)
                                    this._popStick = p_rightStick;
                                this._flipStick = p_leftStick;
                                break;
                            }
                            break;
                        }
                        if (PlayerController.Instance.CanOllieOutOfGrind())
                        {
                            if ((Object)this._popStick != (Object)p_rightStick)
                                this._popStick = p_rightStick;
                            this._flipStick = p_leftStick;
                            break;
                        }
                        if (PlayerController.Instance.CanNollieOutOfGrind())
                        {
                            if ((Object)this._popStick != (Object)p_leftStick)
                                this._popStick = p_leftStick;
                            this._flipStick = p_rightStick;
                            break;
                        }
                        break;
                    case ControlType.Swap:
                        if (!PlayerController.Instance.IsSwitch)
                        {
                            if (PlayerController.Instance.CanOllieOutOfGrind() && PlayerController.Instance.CanNollieOutOfGrind())
                            {
                                if ((double)p_leftStick.PopToeSpeed > 8.0)
                                {
                                    if ((Object)this._popStick != (Object)p_leftStick)
                                        this._popStick = p_leftStick;
                                    this._flipStick = p_rightStick;
                                }
                                if ((double)p_rightStick.PopToeSpeed > 8.0)
                                {
                                    if ((Object)this._popStick != (Object)p_rightStick)
                                        this._popStick = p_rightStick;
                                    this._flipStick = p_leftStick;
                                    break;
                                }
                                break;
                            }
                            if (PlayerController.Instance.CanOllieOutOfGrind())
                            {
                                if ((Object)this._popStick != (Object)p_rightStick)
                                    this._popStick = p_rightStick;
                                this._flipStick = p_leftStick;
                                break;
                            }
                            if (PlayerController.Instance.CanNollieOutOfGrind())
                            {
                                if ((Object)this._popStick != (Object)p_leftStick)
                                    this._popStick = p_leftStick;
                                this._flipStick = p_rightStick;
                                break;
                            }
                            break;
                        }
                        if (PlayerController.Instance.CanOllieOutOfGrind() && PlayerController.Instance.CanNollieOutOfGrind())
                        {
                            if ((double)p_leftStick.PopToeSpeed > 8.0)
                            {
                                if ((Object)this._popStick != (Object)p_leftStick)
                                    this._popStick = p_leftStick;
                                this._flipStick = p_rightStick;
                            }
                            if ((double)p_rightStick.PopToeSpeed > 8.0)
                            {
                                if ((Object)this._popStick != (Object)p_rightStick)
                                    this._popStick = p_rightStick;
                                this._flipStick = p_leftStick;
                                break;
                            }
                            break;
                        }
                        if (PlayerController.Instance.CanOllieOutOfGrind())
                        {
                            if ((Object)this._popStick != (Object)p_leftStick)
                                this._popStick = p_leftStick;
                            this._flipStick = p_rightStick;
                            break;
                        }
                        if (PlayerController.Instance.CanNollieOutOfGrind())
                        {
                            if ((Object)this._popStick != (Object)p_rightStick)
                                this._popStick = p_rightStick;
                            this._flipStick = p_leftStick;
                            break;
                        }
                        break;
                    case ControlType.Simple:
                        if (!PlayerController.Instance.IsSwitch)
                        {
                            if (PlayerController.Instance.CanOllieOutOfGrind() && PlayerController.Instance.CanNollieOutOfGrind())
                            {
                                if ((double)p_leftStick.PopToeSpeed > 8.0)
                                {
                                    if ((Object)this._popStick != (Object)p_leftStick)
                                        this._popStick = p_leftStick;
                                    this._flipStick = p_rightStick;
                                }
                                if ((double)p_rightStick.PopToeSpeed > 8.0)
                                {
                                    if ((Object)this._popStick != (Object)p_rightStick)
                                        this._popStick = p_rightStick;
                                    this._flipStick = p_leftStick;
                                    break;
                                }
                                break;
                            }
                            if (PlayerController.Instance.CanOllieOutOfGrind())
                            {
                                if ((Object)this._popStick != (Object)p_rightStick)
                                    this._popStick = p_rightStick;
                                this._flipStick = p_leftStick;
                                break;
                            }
                            if (PlayerController.Instance.CanNollieOutOfGrind())
                            {
                                if ((Object)this._popStick != (Object)p_leftStick)
                                    this._popStick = p_leftStick;
                                this._flipStick = p_rightStick;
                                break;
                            }
                            break;
                        }
                        if (PlayerController.Instance.CanOllieOutOfGrind() && PlayerController.Instance.CanNollieOutOfGrind())
                        {
                            if ((double)p_leftStick.PopToeSpeed > 8.0)
                            {
                                if ((Object)this._popStick != (Object)p_leftStick)
                                    this._popStick = p_leftStick;
                                this._flipStick = p_rightStick;
                            }
                            if ((double)p_rightStick.PopToeSpeed > 8.0)
                            {
                                if ((Object)this._popStick != (Object)p_rightStick)
                                    this._popStick = p_rightStick;
                                this._flipStick = p_leftStick;
                                break;
                            }
                            break;
                        }
                        if (PlayerController.Instance.CanOllieOutOfGrind())
                        {
                            if ((Object)this._popStick != (Object)p_rightStick)
                                this._popStick = p_rightStick;
                            this._flipStick = p_leftStick;
                            break;
                        }
                        if (PlayerController.Instance.CanNollieOutOfGrind())
                        {
                            if ((Object)this._popStick != (Object)p_leftStick)
                                this._popStick = p_leftStick;
                            this._flipStick = p_rightStick;
                            break;
                        }
                        break;
                }
            }
            else
            {
                switch (SettingsManager.Instance.controlType)
                {
                    case ControlType.Same:
                        if (PlayerController.Instance.CanOllieOutOfGrind() && PlayerController.Instance.CanNollieOutOfGrind())
                        {
                            if ((double)p_leftStick.PopToeSpeed > 8.0)
                            {
                                if ((Object)this._popStick != (Object)p_leftStick)
                                    this._popStick = p_leftStick;
                                this._flipStick = p_rightStick;
                            }
                            if ((double)p_rightStick.PopToeSpeed > 8.0)
                            {
                                if ((Object)this._popStick != (Object)p_rightStick)
                                    this._popStick = p_rightStick;
                                this._flipStick = p_leftStick;
                                break;
                            }
                            break;
                        }
                        if (PlayerController.Instance.CanOllieOutOfGrind())
                        {
                            if ((Object)this._popStick != (Object)p_leftStick)
                                this._popStick = p_leftStick;
                            this._flipStick = p_rightStick;
                            break;
                        }
                        if (PlayerController.Instance.CanNollieOutOfGrind())
                        {
                            if ((Object)this._popStick != (Object)p_rightStick)
                                this._popStick = p_rightStick;
                            this._flipStick = p_leftStick;
                            break;
                        }
                        break;
                    case ControlType.Swap:
                        if (!PlayerController.Instance.IsSwitch)
                        {
                            if (PlayerController.Instance.CanOllieOutOfGrind() && PlayerController.Instance.CanNollieOutOfGrind())
                            {
                                if ((double)p_leftStick.PopToeSpeed > 8.0)
                                {
                                    if ((Object)this._popStick != (Object)p_leftStick)
                                        this._popStick = p_leftStick;
                                    this._flipStick = p_rightStick;
                                }
                                if ((double)p_rightStick.PopToeSpeed > 8.0)
                                {
                                    if ((Object)this._popStick != (Object)p_rightStick)
                                        this._popStick = p_rightStick;
                                    this._flipStick = p_leftStick;
                                    break;
                                }
                                break;
                            }
                            if (PlayerController.Instance.CanOllieOutOfGrind())
                            {
                                if ((Object)this._popStick != (Object)p_leftStick)
                                    this._popStick = p_leftStick;
                                this._flipStick = p_rightStick;
                                break;
                            }
                            if (PlayerController.Instance.CanNollieOutOfGrind())
                            {
                                if ((Object)this._popStick != (Object)p_rightStick)
                                    this._popStick = p_rightStick;
                                this._flipStick = p_leftStick;
                                break;
                            }
                            break;
                        }
                        if (PlayerController.Instance.CanOllieOutOfGrind() && PlayerController.Instance.CanNollieOutOfGrind())
                        {
                            if ((double)p_leftStick.PopToeSpeed > 8.0)
                            {
                                if ((Object)this._popStick != (Object)p_leftStick)
                                    this._popStick = p_leftStick;
                                this._flipStick = p_rightStick;
                            }
                            if ((double)p_rightStick.PopToeSpeed > 8.0)
                            {
                                if ((Object)this._popStick != (Object)p_rightStick)
                                    this._popStick = p_rightStick;
                                this._flipStick = p_leftStick;
                                break;
                            }
                            break;
                        }
                        if (PlayerController.Instance.CanOllieOutOfGrind())
                        {
                            if ((Object)this._popStick != (Object)p_rightStick)
                                this._popStick = p_rightStick;
                            this._flipStick = p_leftStick;
                            break;
                        }
                        if (PlayerController.Instance.CanNollieOutOfGrind())
                        {
                            if ((Object)this._popStick != (Object)p_leftStick)
                                this._popStick = p_leftStick;
                            this._flipStick = p_rightStick;
                            break;
                        }
                        break;
                    case ControlType.Simple:
                        if (!PlayerController.Instance.IsSwitch)
                        {
                            if (PlayerController.Instance.CanOllieOutOfGrind() && PlayerController.Instance.CanNollieOutOfGrind())
                            {
                                if ((double)p_leftStick.PopToeSpeed > 8.0)
                                {
                                    if ((Object)this._popStick != (Object)p_leftStick)
                                        this._popStick = p_leftStick;
                                    this._flipStick = p_rightStick;
                                }
                                if ((double)p_rightStick.PopToeSpeed > 8.0)
                                {
                                    if ((Object)this._popStick != (Object)p_rightStick)
                                        this._popStick = p_rightStick;
                                    this._flipStick = p_leftStick;
                                    break;
                                }
                                break;
                            }
                            if (PlayerController.Instance.CanOllieOutOfGrind())
                            {
                                if ((Object)this._popStick != (Object)p_leftStick)
                                    this._popStick = p_leftStick;
                                this._flipStick = p_rightStick;
                                break;
                            }
                            if (PlayerController.Instance.CanNollieOutOfGrind())
                            {
                                if ((Object)this._popStick != (Object)p_rightStick)
                                    this._popStick = p_rightStick;
                                this._flipStick = p_leftStick;
                                break;
                            }
                            break;
                        }
                        if (PlayerController.Instance.CanOllieOutOfGrind() && PlayerController.Instance.CanNollieOutOfGrind())
                        {
                            if ((double)p_leftStick.PopToeSpeed > 8.0)
                            {
                                if ((Object)this._popStick != (Object)p_leftStick)
                                    this._popStick = p_leftStick;
                                this._flipStick = p_rightStick;
                            }
                            if ((double)p_rightStick.PopToeSpeed > 8.0)
                            {
                                if ((Object)this._popStick != (Object)p_rightStick)
                                    this._popStick = p_rightStick;
                                this._flipStick = p_leftStick;
                                break;
                            }
                            break;
                        }
                        if (PlayerController.Instance.CanOllieOutOfGrind())
                        {
                            if ((Object)this._popStick != (Object)p_leftStick)
                                this._popStick = p_leftStick;
                            this._flipStick = p_rightStick;
                            break;
                        }
                        if (PlayerController.Instance.CanNollieOutOfGrind())
                        {
                            if ((Object)this._popStick != (Object)p_rightStick)
                                this._popStick = p_rightStick;
                            this._flipStick = p_leftStick;
                            break;
                        }
                        break;
                }
            }
            if ((Object)this._popStick != (Object)null && (Object)this._flipStick != (Object)null)
            {
                this._forwardLoad = (double)PlayerController.Instance.GetNollie(this._popStick.IsRightStick) > 0.100000001490116;
                PlayerController.Instance.OnPopStartCheck(true, this._popStick, ref this._setupDir, this._forwardLoad, 15f, ref this._invertVel, this._popStick.IsRightStick ? this._augmentedRightAngle : this._augmentedLeftAngle, ref this._popVel, true);
                PlayerController.Instance.OnFlipStickUpdate(ref this._flipDetected, ref this._potentialFlip, ref this._initialFlipDir, ref this._flipFrameCount, ref this._flipFrameMax, ref this._toeAxis, ref this._flipVel, ref this._popVel, ref this._popDir, ref this._flip, this._flipStick, false, false, ref this._invertVel, this._popStick.IsRightStick ? this._augmentedLeftAngle : this._augmentedRightAngle, false, this._forwardLoad, ref this._flipWindowTimer);
            }
            PlayerController.Instance.SetLeftIKOffset(p_leftStick.ToeAxis, p_leftStick.ForwardDir, p_leftStick.PopDir, p_leftStick.IsPopStick, true, false);
            PlayerController.Instance.SetRightIKOffset(p_rightStick.ToeAxis, p_rightStick.ForwardDir, p_rightStick.PopDir, p_rightStick.IsPopStick, true, false);
        }

        public override void OnStickFixedUpdate(StickInput p_leftStick, StickInput p_rightStick)
        {
            this.RotateBoard(p_leftStick, p_rightStick);
            switch (SettingsManager.Instance.controlType)
            {
                case ControlType.Same:
                    if (SettingsManager.Instance.stance == Stance.Regular)
                    {
                        PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
                        PlayerController.Instance.SetFrontPivotRotation(p_rightStick.ToeAxis);
                        PlayerController.Instance.SetBackPivotRotation(p_leftStick.ToeAxis);
                        PlayerController.Instance.SetPivotForwardRotation((float)(((double)p_leftStick.ForwardDir + (double)p_rightStick.ForwardDir) * 0.699999988079071), 15f);
                        PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                        break;
                    }
                    PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_rightStick.rawInput.pos.x) - Mathf.Abs(p_leftStick.rawInput.pos.x));
                    PlayerController.Instance.SetFrontPivotRotation(-p_leftStick.ToeAxis);
                    PlayerController.Instance.SetBackPivotRotation(-p_rightStick.ToeAxis);
                    PlayerController.Instance.SetPivotForwardRotation((float)(((double)p_leftStick.ForwardDir + (double)p_rightStick.ForwardDir) * 0.699999988079071), 15f);
                    PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                    break;
                case ControlType.Swap:
                    if (!PlayerController.Instance.IsSwitch)
                    {
                        if (SettingsManager.Instance.stance == Stance.Regular)
                        {
                            PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
                            PlayerController.Instance.SetFrontPivotRotation(p_rightStick.ToeAxis);
                            PlayerController.Instance.SetBackPivotRotation(p_leftStick.ToeAxis);
                            PlayerController.Instance.SetPivotForwardRotation((float)(((double)p_leftStick.ForwardDir + (double)p_rightStick.ForwardDir) * 0.699999988079071), 15f);
                            PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                            break;
                        }
                        PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_rightStick.rawInput.pos.x) - Mathf.Abs(p_leftStick.rawInput.pos.x));
                        PlayerController.Instance.SetFrontPivotRotation(-p_leftStick.ToeAxis);
                        PlayerController.Instance.SetBackPivotRotation(-p_rightStick.ToeAxis);
                        PlayerController.Instance.SetPivotForwardRotation((float)(((double)p_leftStick.ForwardDir + (double)p_rightStick.ForwardDir) * 0.699999988079071), 15f);
                        PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                        break;
                    }
                    if (SettingsManager.Instance.stance == Stance.Regular)
                    {
                        PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_rightStick.rawInput.pos.x) - Mathf.Abs(p_leftStick.rawInput.pos.x));
                        PlayerController.Instance.SetFrontPivotRotation(p_leftStick.ToeAxis);
                        PlayerController.Instance.SetBackPivotRotation(p_rightStick.ToeAxis);
                        PlayerController.Instance.SetPivotForwardRotation((float)(((double)p_leftStick.ForwardDir + (double)p_rightStick.ForwardDir) * 0.699999988079071), 15f);
                        PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                        break;
                    }
                    PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
                    PlayerController.Instance.SetFrontPivotRotation(-p_rightStick.ToeAxis);
                    PlayerController.Instance.SetBackPivotRotation(-p_leftStick.ToeAxis);
                    PlayerController.Instance.SetPivotForwardRotation((float)(((double)p_leftStick.ForwardDir + (double)p_rightStick.ForwardDir) * 0.699999988079071), 15f);
                    PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                    break;
                case ControlType.Simple:
                    if (!PlayerController.Instance.IsSwitch)
                    {
                        if (SettingsManager.Instance.stance == Stance.Regular)
                        {
                            PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
                            PlayerController.Instance.SetFrontPivotRotation(p_rightStick.ToeAxis);
                            PlayerController.Instance.SetBackPivotRotation(p_leftStick.ToeAxis);
                            PlayerController.Instance.SetPivotForwardRotation((float)(((double)p_leftStick.ForwardDir + (double)p_rightStick.ForwardDir) * 0.699999988079071), 15f);
                            PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                            break;
                        }
                        PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_rightStick.rawInput.pos.x) - Mathf.Abs(p_leftStick.rawInput.pos.x));
                        PlayerController.Instance.SetFrontPivotRotation(-p_leftStick.ToeAxis);
                        PlayerController.Instance.SetBackPivotRotation(-p_rightStick.ToeAxis);
                        PlayerController.Instance.SetPivotForwardRotation((float)(((double)p_leftStick.ForwardDir + (double)p_rightStick.ForwardDir) * 0.699999988079071), 15f);
                        PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                        break;
                    }
                    if (SettingsManager.Instance.stance == Stance.Regular)
                    {
                        PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
                        PlayerController.Instance.SetFrontPivotRotation(p_rightStick.ToeAxis);
                        PlayerController.Instance.SetBackPivotRotation(p_leftStick.ToeAxis);
                        PlayerController.Instance.SetPivotForwardRotation((float)(((double)p_leftStick.ForwardDir + (double)p_rightStick.ForwardDir) * 0.699999988079071), 15f);
                        PlayerController.Instance.SetPivotSideRotation(p_rightStick.ToeAxis - p_leftStick.ToeAxis);
                        break;
                    }
                    PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
                    PlayerController.Instance.SetFrontPivotRotation(-p_rightStick.ToeAxis);
                    PlayerController.Instance.SetBackPivotRotation(-p_leftStick.ToeAxis);
                    PlayerController.Instance.SetPivotForwardRotation((float)(((double)p_leftStick.ForwardDir + (double)p_rightStick.ForwardDir) * 0.699999988079071), 15f);
                    PlayerController.Instance.SetPivotSideRotation(p_rightStick.ToeAxis - p_leftStick.ToeAxis);
                    break;
            }
        }

        public override void OnNextState()
        {
            if (!((Object)this._popStick != (Object)null) || !((Object)this._flipStick != (Object)null))
                return;
            this._popped = true;
            PlayerController.Instance.AnimSetNollie(PlayerController.Instance.GetNollie(this._popStick.IsRightStick));
            PlayerController.Instance.AnimSetPopStrength(0.0f);
            PlayerController.Instance.AnimGrindTransition(false);
            PlayerController.Instance.boardController.ReferenceBoardRotation();
            PlayerController.Instance.SetTurnMultiplier(1.2f);
            PlayerController.Instance.SetTurningMode(InputController.TurningMode.InAir);
            PlayerController.Instance.FixTargetNormal();
            PlayerController.Instance.SetTargetToMaster();
            PlayerController.Instance.AnimOllieTransition(true);
            if (this._potentialFlip)
                this.DoTransition(typeof(Custom_BeginPop), new object[17]
                {
        (object) this._popStick,
        (object) this._flipStick,
        (object) this._initialFlipDir,
        (object) this._flipVel,
        (object) this._popVel,
        (object) this._toeAxis,
        (object) this._popDir,
        (object) this._flipDetected,
        (object) this._flip,
        (object) this._popForce,
        (object) true,
        (object) this._forwardLoad,
        (object) this._invertVel,
        (object) this._setupDir,
        (object) this._augmentedLeftAngle,
        (object) this._augmentedRightAngle,
        (object) (this._grindingBackwards ? 1 : 0)
                });
            else
                this.DoTransition(typeof(Custom_BeginPop), new object[13]
                {
        (object) this._popStick,
        (object) this._flipStick,
        (object) this._popForce,
        (object) true,
        (object) this._forwardLoad,
        (object) this._invertVel,
        (object) this._setupDir,
        (object) this._augmentedLeftAngle,
        (object) this._augmentedRightAngle,
        (object) this._popVel,
        (object) this._toeAxis,
        (object) this._popDir,
        (object) (this._grindingBackwards ? 1 : 0)
                });
        }

        private void CorrectVelocity(Rigidbody p_rb)
        {
            Vector3 vector3_1 = Vector3.Project(p_rb.velocity, this._grindDirection);
            Vector3 vector3_2 = new Vector3(vector3_1.x, p_rb.velocity.y, vector3_1.z).normalized * vector3_1.magnitude;
            Vector3 vector3_3 = (double)Vector3.Angle(vector3_2, this._grindDirection) < 90.0 ? this._grindDirection : -this._grindDirection;
            if (SkaterXL.Core.Mathd.Vector3IsInfinityOrNan(vector3_2))
                return;
            if ((double)Vector3.ProjectOnPlane(vector3_2, PlayerController.Instance.boardController.triggerManager.grindUp).magnitude < 0.5)
                p_rb.velocity = vector3_2;
            else
                p_rb.velocity = vector3_3 * vector3_2.magnitude;
        }

        public override void LeftTriggerPressed()
        {
            this._leftTriggerPressed = true;
        }

        public override void LeftTriggerHeld(float p_value, InputController.TurningMode p_turningMode)
        {
            this.RotatePlayer(-p_value);
            this._leftTrigger = p_value;
            this._leftTriggerTimer += Time.deltaTime;
        }

        public override void LeftTriggerReleased()
        {
            this._leftTrigger = 0.0f;
            this._leftTriggerHasBeenReleased = true;
            if ((double)this._bothTriggerTimer == 0.0 && this._leftTriggerPressed && (double)this._leftTriggerTimer < 0.300000011920929)
            {
                if (PlayerController.Instance.boardController.triggerManager.sideEnteredGrind != (this._isLeftActuallyLeft ? TriggerManager.SideEnteredGrind.Left : TriggerManager.SideEnteredGrind.Right))
                {
                    PlayerController.Instance.boardController.triggerManager.sideEnteredGrind = this._isLeftActuallyLeft ? TriggerManager.SideEnteredGrind.Left : TriggerManager.SideEnteredGrind.Right;
                    PlayerController.Instance.cameraController.SetCameraSide(PlayerController.Instance.boardController.triggerManager.sideEnteredGrind == TriggerManager.SideEnteredGrind.Left ? (this._isLeftActuallyLeft ? 0.0f : 1f) : (this._isLeftActuallyLeft ? 1f : 0.0f), 1f);
                }
                else if (SettingsManager.Instance.bumpOutType == SettingsManager.BumpOutType.Triggers)
                    this.FallOutOfGrind(false);
            }
            this._leftTriggerPressed = false;
            this._leftTriggerTimer = 0.0f;
        }

        public override void RightTriggerPressed()
        {
            this._rightTriggerPressed = true;
        }

        public override void RightTriggerHeld(float p_value, InputController.TurningMode p_turningMode)
        {
            this.RotatePlayer(p_value);
            this._rightTrigger = p_value;
            this._rightTriggerTimer += Time.deltaTime;
        }

        public override void RightTriggerReleased()
        {
            this._rightTrigger = 0.0f;
            this._rightTriggerHasBeenReleased = true;
            if ((double)this._bothTriggerTimer == 0.0 && this._rightTriggerPressed && (double)this._rightTriggerTimer < 0.300000011920929)
            {
                if (PlayerController.Instance.boardController.triggerManager.sideEnteredGrind != (this._isLeftActuallyLeft ? TriggerManager.SideEnteredGrind.Right : TriggerManager.SideEnteredGrind.Left))
                {
                    PlayerController.Instance.boardController.triggerManager.sideEnteredGrind = this._isLeftActuallyLeft ? TriggerManager.SideEnteredGrind.Right : TriggerManager.SideEnteredGrind.Left;
                    PlayerController.Instance.cameraController.SetCameraSide(PlayerController.Instance.boardController.triggerManager.sideEnteredGrind == TriggerManager.SideEnteredGrind.Left ? (this._isLeftActuallyLeft ? 0.0f : 1f) : (this._isLeftActuallyLeft ? 1f : 0.0f), 1f);
                }
                else if (SettingsManager.Instance.bumpOutType == SettingsManager.BumpOutType.Triggers)
                    this.FallOutOfGrind(true);
            }
            this._rightTriggerPressed = false;
            this._rightTriggerTimer = 0.0f;
        }

        public override void BothTriggersReleased(InputController.TurningMode turningMode)
        {
            base.BothTriggersReleased(turningMode);
            if (!this._bothTriggersPressed)
                return;
            if (!this._bothReleased && (double)this._bothTriggerTimer < 0.300000011920929 && PlayerController.Instance.boardController.triggerManager.sideEnteredGrind != TriggerManager.SideEnteredGrind.Center)
            {
                PlayerController.Instance.boardController.triggerManager.sideEnteredGrind = TriggerManager.SideEnteredGrind.Center;
                PlayerController.Instance.cameraController.SetCameraSide(PlayerController.Instance.boardController.triggerManager.sideEnteredGrind == TriggerManager.SideEnteredGrind.Left ? (this._isLeftActuallyLeft ? 0.0f : 1f) : (this._isLeftActuallyLeft ? 1f : 0.0f), 0.0f);
            }
            this._bothTriggerTimer = 0.0f;
            this._bothReleased = true;
            this._bothTriggersPressed = false;
        }

        private void RotateBoard(StickInput p_leftStick, StickInput p_rightStick)
        {
            this._horizontalTarget = (float)(-(double)p_rightStick.ToeAxis * 44.0 + (double)p_leftStick.ToeAxis * TorqueModifier);
            this._horizontalLerp = Mathf.Lerp(this._horizontalLerp, this._horizontalTarget, Time.deltaTime * PlayerController.Instance.horizontalSpeed);
            Vector3 velocity = PlayerController.Instance.boardController.boardRigidbody.velocity;
            float num1 = Mathf.Abs(Vector3.Dot(Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardTransform.forward, PlayerController.Instance.skaterController.skaterTransform.up), this._grindDirection));
            bool flag = PlayerController.Instance.TwoWheelsDown() && (double)num1 < 0.400000005960464;
            float num2 = flag ? 21f : MaxAngleModifier;
            if (!this._bluntStart & flag)
            {
                this._bluntStart = true;
                SoundManager.Instance.PlayPowerslideSound(PlayerController.Instance.GetSurfaceTag("Surface_Wood"), Vector3.ProjectOnPlane(velocity, Vector3.up).magnitude / 2f, 1);
            }
            else if (this._bluntStart && !flag)
            {
                this._bluntStart = false;
                SoundManager.Instance.StopPowerslideSound(1, Vector3.ProjectOnPlane(velocity / 2f, Vector3.up).magnitude);
            }
            this._verticalTarget = (float)((double)p_rightStick.ForwardDir * (double)num2 + (double)p_leftStick.ForwardDir * (double)num2);
            this._verticalLerp = Mathf.Lerp(this._verticalLerp, this._verticalTarget, Time.deltaTime * PlayerController.Instance.verticalSpeed);
        }

        private bool Blunt()
        {
            return PlayerController.Instance.boardController.triggerManager.grindDetection.grindType == SkaterXL.Core.GrindType.BsBluntSlide || PlayerController.Instance.boardController.triggerManager.grindDetection.grindType == SkaterXL.Core.GrindType.BsNoseBluntSlide || PlayerController.Instance.boardController.triggerManager.grindDetection.grindType == SkaterXL.Core.GrindType.FsBluntSlide || PlayerController.Instance.boardController.triggerManager.grindDetection.grindType == SkaterXL.Core.GrindType.FsNoseBluntSlide;
        }

        private void RotatePlayer(float p_value)
        {
            this._playerTurn = (float)((double)p_value * (double)Time.deltaTime * 100.0);
            PlayerController.Instance.GrindRotatePlayerHorizontal(this._playerTurn);
        }

        public override void OnCopingCheck(SplineComputer _spline)
        {
            if (this._overrideSpline)
                return;
            this._overrideSpline = true;
        }

        public override bool CanGrind()
        {
            return this._canGrind;
        }

        public override void OnGrindStay()
        {
        }

        public override void OnGrindEnded()
        {
            PlayerController.Instance.skaterController.InitializeSkateRotation();
            PlayerController.Instance.boardController.boardRigidbody.velocity = this._lastVelocity;
            PlayerController.Instance.boardController.backTruckRigidbody.velocity = this._lastVelocity;
            PlayerController.Instance.boardController.frontTruckRigidbody.velocity = this._lastVelocity;
            PlayerController.Instance.boardController.boardRigidbody.angularVelocity = Vector3.zero;
            PlayerController.Instance.boardController.backTruckRigidbody.angularVelocity = Vector3.zero;
            PlayerController.Instance.boardController.frontTruckRigidbody.angularVelocity = Vector3.zero;
            PlayerController.Instance.skaterController.skaterRigidbody.angularVelocity = Vector3.zero;
            if (PlayerController.Instance.TwoWheelsDown())
            {
                PlayerController.Instance.AnimGrindTransition(false);
                PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
                PlayerController.Instance.SetBoardToMaster();
                if (!PlayerController.Instance.IsRespawning)
                    PlayerController.Instance.CrossFadeAnimation("Riding", 0.5f);
                this.DoTransition(typeof(Custom_Riding), new object[1]
                {
        (object) true
                });
            }
            else
            {
                PlayerController.Instance.AnimGrindTransition(false);
                PlayerController.Instance.SetTurningMode(InputController.TurningMode.InAir);
                PlayerController.Instance.SetSkaterToMaster();
                if (!PlayerController.Instance.IsRespawning)
                    PlayerController.Instance.CrossFadeAnimation("Extend", 0.5f);
                PlayerController.Instance.OnExtendAnimEnter();
                PlayerController.Instance.skaterController.skaterRigidbody.AddForce(PlayerController.Instance.skaterController.PredictLanding(PlayerController.Instance.skaterController.skaterRigidbody.velocity), ForceMode.Impulse);
                EventManager.Instance.EnterAir();
                this.DoTransition(typeof(Custom_InAir), new object[3]
                {
        (object) true,
        (object) ((double) this._timeInState > 0.200000002980232),
        (object) 0.0f
                });
            }
        }

        public override void OnStickPressed(bool right)
        {
            if (SettingsManager.Instance.bumpOutType != SettingsManager.BumpOutType.Sticks)
                return;
            this.FallOutOfGrind(right);
        }

        private void FallOutOfGrind(bool right)
        {
            if (!BumpOut)
            {
                return;
            }
            if (PlayerController.Instance.IsTransition(!this._isLeftActuallyLeft ^ right))
            {
                EventManager.Instance.EnterAir();
                this.DoTransition(typeof(Custom_ExitCoping), new object[3]
                {
        (object) this._spline,
        (object) this._grindDirection,
        (object) (this._isLeftActuallyLeft ? (right ? 1 : 0) : (right ? 0 : 1))
                });
            }
            else
            {
                PlayerController.Instance.skaterController.InitializeSkateRotation();
                PlayerController.Instance.boardController.boardRigidbody.velocity = this._lastVelocity;
                PlayerController.Instance.boardController.backTruckRigidbody.velocity = this._lastVelocity;
                PlayerController.Instance.boardController.frontTruckRigidbody.velocity = this._lastVelocity;
                PlayerController.Instance.boardController.boardRigidbody.angularVelocity = Vector3.zero;
                PlayerController.Instance.boardController.backTruckRigidbody.angularVelocity = Vector3.zero;
                PlayerController.Instance.boardController.frontTruckRigidbody.angularVelocity = Vector3.zero;
                PlayerController.Instance.skaterController.skaterRigidbody.angularVelocity = Vector3.zero;
                PlayerController.Instance.AnimGrindTransition(false);
                PlayerController.Instance.SetTurningMode(InputController.TurningMode.InAir);
                PlayerController.Instance.SetSkaterToMaster();
                if (!PlayerController.Instance.IsRespawning)
                    PlayerController.Instance.CrossFadeAnimation("Extend", 0.5f);
                Vector3 vector3 = Vector3.ProjectOnPlane(PlayerController.Instance.GetGrindRight(), Vector3.up).normalized;
                if (!this._isLeftActuallyLeft)
                    vector3 = -vector3;
                Vector3 rhs1 = right ? vector3 : -vector3;
                bool flag = false;
                Vector3 lhs1 = PlayerController.Instance.boardController.backTruckRigidbody.position - PlayerController.Instance.boardController.boardRigidbody.position;
                Vector3 lhs2 = PlayerController.Instance.boardController.frontTruckRigidbody.position - PlayerController.Instance.boardController.boardRigidbody.position;
                Vector3 lhs3 = PlayerController.Instance.boardController.backTruckRigidbody.position - PlayerController.Instance.GetGrindContactPosition();
                Vector3 lhs4 = PlayerController.Instance.boardController.frontTruckRigidbody.position - PlayerController.Instance.GetGrindContactPosition();
                Vector3 rhs2 = PlayerController.Instance.boardController.boardRigidbody.position - PlayerController.Instance.GetGrindContactPosition();
                if ((double)lhs3.sqrMagnitude > (double)lhs4.sqrMagnitude)
                {
                    if ((double)Vector3.Dot(lhs3, rhs2) > 0.0 && (double)Vector3.Dot(lhs3, rhs1) < 0.0 && (double)Vector3.Dot(lhs1, Vector3.up) < 0.0399999991059303)
                        flag = true;
                }
                else if ((double)Vector3.Dot(lhs4, rhs2) > 0.0 && (double)Vector3.Dot(lhs4, rhs1) < 0.0 && (double)Vector3.Dot(lhs2, Vector3.up) < 0.0399999991059303)
                    flag = true;
                float num = flag ? BumpOutPopForce + 0.55f : BumpOutPopForce;
                Vector3 force = PlayerController.Instance.skaterController.PredictLanding(PlayerController.Instance.skaterController.skaterRigidbody.velocity + PlayerController.Instance.skaterController.skaterTransform.up * num + rhs1 * 1.2f);
                if ((double)PlayerController.Instance.boardController.trajectory.PredictedGroundPoint.y >= (double)PlayerController.Instance.GetGrindContactPosition().y - 0.200000002980232)
                    num = BumpOutPopForce + 0.75f;
                PlayerController.Instance.skaterController.skaterRigidbody.AddForce(PlayerController.Instance.skaterController.skaterTransform.up * num + rhs1 * BumpOutSidewayForce, ForceMode.Impulse);
                PlayerController.Instance.skaterController.skaterRigidbody.AddForce(force, ForceMode.Impulse);
                EventManager.Instance.EnterAir();
                this.DoTransition(typeof(Custom_InAir), new object[4]
                {
        (object) true,
        (object) false,
        (object) true,
        (object) 0.0f
                });
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

        public bool BumpOut = true;
        public float BumpOutPopForce = 1.25f;
        public float BumpOutSidewayForce = 1.2f;
        public CrouchMode CurrentCrouchMode;
        public float CrouchAmount = 0.95f;
        public float DefaultCrouchAmount = 0.95f;
        public float GrindFriction = 0.25f;
        public float MaxAngleModifier = 15f;
        public float PopThreshold = 15f;
        public float SidewayPopForce = 0.75f;
        public bool Stabilizer = true;
        public float TorqueModifier = 44f;

        private void DetermineGrind()
        {
            if (Main.settings.AdvancedGrinding)
            {
                switch (PlayerController.Instance.boardController.triggerManager.grindDetection.grindType)
                {
                    case GrindType.BsBluntSlide:
                        UpdateGrind(Main.settings._bluntSlideSettings);
                        break;
                    case GrindType.FsBluntSlide:
                        UpdateGrind(Main.settings._bluntSlideSettings);
                        break;
                    case GrindType.BsBoardSlide:
                        UpdateGrind(Main.settings._boardSlideSettings);
                        break;
                    case GrindType.FsBoardSlide:
                        UpdateGrind(Main.settings._boardSlideSettings);
                        break;
                    case GrindType.BsCrook:
                        UpdateGrind(Main.settings._crookSettings);
                        break;
                    case GrindType.FsCrook:
                        UpdateGrind(Main.settings._crookSettings);
                        break;
                    case GrindType.BsFeeble:
                        UpdateGrind(Main.settings._feebleSettings);
                        break;
                    case GrindType.FsFeeble:
                        UpdateGrind(Main.settings._feebleSettings);
                        break;
                    case GrindType.BsFiftyFifty:
                        UpdateGrind(Main.settings._fiftyFiftySettings);
                        break;
                    case GrindType.FsFiftyFifty:
                        UpdateGrind(Main.settings._fiftyFiftySettings);
                        break;
                    case GrindType.BsFiveO:
                        UpdateGrind(Main.settings._fiveOSettings);
                        break;
                    case GrindType.FsFiveO:
                        UpdateGrind(Main.settings._fiveOSettings);
                        break;
                    case GrindType.BsLipSlide:
                        UpdateGrind(Main.settings._lipslideSettings);
                        break;
                    case GrindType.FsLipSlide:
                        UpdateGrind(Main.settings._lipslideSettings);
                        break;
                    case GrindType.BsLosi:
                        UpdateGrind(Main.settings._losiSettings);
                        break;
                    case GrindType.FsLosi:
                        UpdateGrind(Main.settings._losiSettings);
                        break;
                    case GrindType.BsNoseBluntSlide:
                        UpdateGrind(Main.settings._nosebluntSettings);
                        break;
                    case GrindType.FsNoseBluntSlide:
                        UpdateGrind(Main.settings._nosebluntSettings);
                        break;
                    case GrindType.BsNoseGrind:
                        UpdateGrind(Main.settings._nosegrindSettings);
                        break;
                    case GrindType.FsNoseGrind:
                        UpdateGrind(Main.settings._nosegrindSettings);
                        break;
                    case GrindType.BsNoseSlide:
                        UpdateGrind(Main.settings._noseslideSettings);
                        break;
                    case GrindType.FsNoseSlide:
                        UpdateGrind(Main.settings._noseslideSettings);
                        break;
                    case GrindType.BsOverCrook:
                        UpdateGrind(Main.settings._overcrookSettings);
                        break;
                    case GrindType.FsOverCrook:
                        UpdateGrind(Main.settings._overcrookSettings);
                        break;
                    case GrindType.BsSalad:
                        UpdateGrind(Main.settings._saladSettings);
                        break;
                    case GrindType.FsSalad:
                        UpdateGrind(Main.settings._saladSettings);
                        break;
                    case GrindType.BsSmith:
                        UpdateGrind(Main.settings._smithSettings);
                        break;
                    case GrindType.FsSmith:
                        UpdateGrind(Main.settings._smithSettings);
                        break;
                    case GrindType.BsSuski:
                        UpdateGrind(Main.settings._suskiSettings);
                        break;
                    case GrindType.FsSuski:
                        UpdateGrind(Main.settings._suskiSettings);
                        break;
                    case GrindType.BsTailSlide:
                        UpdateGrind(Main.settings._tailslideSettings);
                        break;
                    case GrindType.FsTailSlide:
                        UpdateGrind(Main.settings._tailslideSettings);
                        break;
                    case GrindType.BsWilly:
                        UpdateGrind(Main.settings._willySettings);
                        break;
                    case GrindType.FsWilly:
                        UpdateGrind(Main.settings._willySettings);
                        break;
                }
            }
            else
            {
                UpdateGrind(Main.settings._generalGrindSettings);
            }
        }

        private void HandleCrouch()
        {
            switch (CurrentCrouchMode)
            {
                case CrouchMode.Auto:
                    DefaultCrouchAmount = Mathf.MoveTowards(DefaultCrouchAmount, CrouchAmount, 1f);
                    break;
                case CrouchMode.LB:
                    if (PlayerController.Instance.inputController.player.GetButton("LB"))
                    {
                        DefaultCrouchAmount = Mathf.MoveTowards(DefaultCrouchAmount, CrouchAmount, 1f);
                        return;
                    }
                    DefaultCrouchAmount = Mathf.MoveTowards(DefaultCrouchAmount, 0.95f, 1f);
                    break;
                case CrouchMode.RB:
                    if (PlayerController.Instance.inputController.player.GetButton("RB"))
                    {
                        DefaultCrouchAmount = Mathf.MoveTowards(DefaultCrouchAmount, CrouchAmount, 1f);
                        return;
                    }
                    DefaultCrouchAmount = Mathf.MoveTowards(DefaultCrouchAmount, 0.95f, 1f);
                    break;
                case CrouchMode.Off:
                    DefaultCrouchAmount = 0.95f;
                    break;
            }
        }

        private void HandlePopOutDirection()
        {
            switch (Main.settings.PopOutDirection)
            {
                case Core.PopOutDirection.Default:
                    PlayerController.Instance.boardController.triggerManager.sideEnteredGrind = (PlayerController.Instance.IsRightSideOfGrind() ? TriggerManager.SideEnteredGrind.Right : TriggerManager.SideEnteredGrind.Left);
                    break;
                case Core.PopOutDirection.Opposite:
                    PlayerController.Instance.boardController.triggerManager.sideEnteredGrind = (PlayerController.Instance.IsRightSideOfGrind() ? TriggerManager.SideEnteredGrind.Left : TriggerManager.SideEnteredGrind.Right);
                    break;
                case Core.PopOutDirection.Straight:
                    PlayerController.Instance.boardController.triggerManager.sideEnteredGrind = (PlayerController.Instance.IsRightSideOfGrind() ? TriggerManager.SideEnteredGrind.Center : TriggerManager.SideEnteredGrind.Center);
                    break;
            }
        }

        private void HandleStall()
        {
            if (Main.settings.InstantStallMode == Core.InstantStallMode.LB && PlayerController.Instance.inputController.player.GetButton("LB"))
            {
                PlayerController.Instance.boardController.boardRigidbody.velocity = new Vector3(0, 0, 0);
            }

            if (Main.settings.InstantStallMode == Core.InstantStallMode.RB && PlayerController.Instance.inputController.player.GetButton("RB"))
            {
                PlayerController.Instance.boardController.boardRigidbody.velocity = new Vector3(0, 0, 0);
            }
        }

        private void OneFootGrind()
        {
            switch (Main.settings.GrindOneFootMode)
            {
                case OneFootMode.Bumper:
                    if (PlayerController.Instance.inputController.player.GetButton("LB"))
                    {
                        PlayerController.Instance.SetLeftIKLerpTarget(0.5f, 1f);
                        PlayerController.Instance.SetLeftSteezeWeight(1f);
                        PlayerController.Instance.SetMaxSteezeLeft(1f);
                        PlayerController.Instance.SetLeftKneeIKTargetWeight(0.3f);
                    }

                    else if (PlayerController.Instance.inputController.player.GetButton("RB"))
                    {
                        PlayerController.Instance.SetRightIKLerpTarget(0.5f, 1f);
                        PlayerController.Instance.SetRightSteezeWeight(1f);
                        PlayerController.Instance.SetMaxSteezeRight(1f);
                        PlayerController.Instance.SetRightKneeIKTargetWeight(0.3f);
                    }

                    else
                    {
                        ExitOneFootGrind();
                    }
                    break;
                case OneFootMode.Buttons:
                    if (PlayerController.Instance.inputController.player.GetButton("A"))
                    {
                        PlayerController.Instance.SetRightIKLerpTarget(0.5f, 1f);
                        PlayerController.Instance.SetRightSteezeWeight(1f);
                        PlayerController.Instance.SetMaxSteezeRight(1f);
                        PlayerController.Instance.SetRightKneeIKTargetWeight(0.3f);
                        return;
                    }

                    else if (PlayerController.Instance.inputController.player.GetButton("X"))
                    {
                        PlayerController.Instance.SetLeftIKLerpTarget(0.5f, 1f);
                        PlayerController.Instance.SetLeftSteezeWeight(1f);
                        PlayerController.Instance.SetMaxSteezeLeft(1f);
                        PlayerController.Instance.SetLeftKneeIKTargetWeight(0.3f);
                        return;
                    }
                    ExitOneFootGrind();
                    break;
            }

        }

        private void ExitOneFootGrind()
        {
            PlayerController.Instance.SetRightIKLerpTarget(0f, 0f);
            PlayerController.Instance.SetRightSteezeWeight(0f);
            PlayerController.Instance.SetMaxSteezeRight(0f);
            PlayerController.Instance.SetRightKneeIKTargetWeight(0f);

            PlayerController.Instance.SetLeftIKLerpTarget(0f, 0f);
            PlayerController.Instance.SetLeftSteezeWeight(0f);
            PlayerController.Instance.SetMaxSteezeLeft(0f);
            PlayerController.Instance.SetLeftKneeIKTargetWeight(0f);
        }

        private void UpdateGrind(BaseGrindSettings GrindSettings)
        {
            BumpOut = GrindSettings.BumpOut;
            BumpOutPopForce = GrindSettings.BumpOutPopForce;
            BumpOutSidewayForce = GrindSettings.BumpOutSidewayForce;
            CrouchAmount = GrindSettings.CrouchAmount;
            CurrentCrouchMode = GrindSettings.CrouchMode;
            GrindFriction = GrindSettings.Friction;
            MaxAngleModifier = GrindSettings.MaxAngleModifier;
            _popForce = GrindSettings.PopForce;
            PopThreshold = GrindSettings.PopThreshold;
            SidewayPopForce = GrindSettings.SidewayPopForce;
            Stabilizer = GrindSettings.Stabilizer;
            TorqueModifier = GrindSettings.TorqueModifier;
        }

        private void DoStance()
        {
            switch (PlayerController.Instance.boardController.triggerManager.grindDetection.grindType)
            {
                case GrindType.BsBluntSlide:
                    DoStance(StanceController.Instance.BSBluntslideFeet);
                    break;
                case GrindType.FsBluntSlide:
                    DoStance(StanceController.Instance.BSBluntslideFeet);
                    break;
                case GrindType.BsBoardSlide:
                    DoStance(StanceController.Instance.BSBoardslideFeet);
                    break;
                case GrindType.FsBoardSlide:
                    DoStance(StanceController.Instance.BSBoardslideFeet);
                    break;
                case GrindType.BsCrook:
                    DoStance(StanceController.Instance.BSCrookFeet);
                    break;
                case GrindType.FsCrook:
                    DoStance(StanceController.Instance.BSCrookFeet);
                    break;
                case GrindType.BsFeeble:
                    DoStance(StanceController.Instance.BSFeebleFeet);
                    break;
                case GrindType.FsFeeble:
                    DoStance(StanceController.Instance.BSFeebleFeet);
                    break;
                case GrindType.BsFiftyFifty:
                    DoStance(StanceController.Instance.BSFiftyFiftyFeet);
                    break;
                case GrindType.FsFiftyFifty:
                    DoStance(StanceController.Instance.BSFiftyFiftyFeet);
                    break;
                case GrindType.BsFiveO:
                    DoStance(StanceController.Instance.BSFiveOFeet);
                    break;
                case GrindType.FsFiveO:
                    DoStance(StanceController.Instance.BSFiveOFeet);
                    break;
                case GrindType.BsLipSlide:
                    DoStance(StanceController.Instance.BSLipslideFeet);
                    break;
                case GrindType.FsLipSlide:
                    DoStance(StanceController.Instance.BSLipslideFeet);
                    break;
                case GrindType.BsLosi:
                    DoStance(StanceController.Instance.BSLosiFeet);
                    break;
                case GrindType.FsLosi:
                    DoStance(StanceController.Instance.BSLosiFeet);
                    break;
                case GrindType.BsNoseBluntSlide:
                    DoStance(StanceController.Instance.BSNosebluntFeet);
                    break;
                case GrindType.FsNoseBluntSlide:
                    DoStance(StanceController.Instance.BSNosebluntFeet);
                    break;
                case GrindType.BsNoseGrind:
                    DoStance(StanceController.Instance.BSNosegrindFeet);
                    break;
                case GrindType.FsNoseGrind:
                    DoStance(StanceController.Instance.BSNosegrindFeet);
                    break;
                case GrindType.BsNoseSlide:
                    DoStance(StanceController.Instance.BSNoseslideFeet);
                    break;
                case GrindType.FsNoseSlide:
                    DoStance(StanceController.Instance.BSNoseslideFeet);
                    break;
                case GrindType.BsOverCrook:
                    DoStance(StanceController.Instance.BSOvercrookFeet);
                    break;
                case GrindType.FsOverCrook:
                    DoStance(StanceController.Instance.BSOvercrookFeet);
                    break;
                case GrindType.BsSalad:
                    DoStance(StanceController.Instance.BSSaladFeet);
                    break;
                case GrindType.FsSalad:
                    DoStance(StanceController.Instance.BSSaladFeet);
                    break;
                case GrindType.BsSmith:
                    DoStance(StanceController.Instance.BSSmithFeet);
                    break;
                case GrindType.FsSmith:
                    DoStance(StanceController.Instance.BSSmithFeet);
                    break;
                case GrindType.BsSuski:
                    DoStance(StanceController.Instance.BSSuskiFeet);
                    break;
                case GrindType.FsSuski:
                    DoStance(StanceController.Instance.BSSuskiFeet);
                    break;
                case GrindType.BsTailSlide:
                    DoStance(StanceController.Instance.BSTailslideFeet);
                    break;
                case GrindType.FsTailSlide:
                    DoStance(StanceController.Instance.BSTailslideFeet);
                    break;
                case GrindType.BsWilly:
                    DoStance(StanceController.Instance.BSWillyFeet);
                    break;
                case GrindType.FsWilly:
                    DoStance(StanceController.Instance.BSWillyFeet);
                    break;
            }
        }

        private void DoStance(CustomFeetObject Stance)
        {
            if (PlayerController.Instance.inputController.player.GetButton("Left Stick Button") && Main.settings.UseSpecialInGrindState)
            {
                StanceController.Instance.SetFreeFootMovementLeft(false, true);
                StanceController.Instance.DoLeftFootTransition(StanceController.Instance.OnButtonFeet);
            }
            else
            {
                StanceController.Instance.SetFreeFootMovementLeft(true, false);
                StanceController.Instance.DoLeftFootTransition(Stance);
            }
            if (PlayerController.Instance.inputController.player.GetButton("Right Stick Button") && Main.settings.UseSpecialInGrindState)
            {
                StanceController.Instance.SetFreeFootMovementRight(false, true);
                StanceController.Instance.DoRightFootTransition(StanceController.Instance.OnButtonFeet);
            }
            else
            {
                StanceController.Instance.SetFreeFootMovementRight(true, false);
                StanceController.Instance.DoRightFootTransition(Stance);
            }
        }
    }
}
