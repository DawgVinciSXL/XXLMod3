using UnityEngine;
using XXLMod3.Controller;
using XXLMod3.Core;

namespace XXLMod3.PlayerStates
{
    public class Custom_PrimoSetup : PlayerState_OnBoard
    {
        StickInput _popStick;
        StickInput _flipStick;
        bool _forwardLoad;
        PlayerController.SetupDir _setupDir;

        private bool _flipDetected;
        private bool _potentialFlip;
        float setupHeight = 1.35f;

        private Vector2 _initialFlipDir = Vector2.zero;
        private Vector3 _velocityOnBeginPop = Vector3.zero;
        private Vector2 _leftStick = Vector2.zero;
        private Vector2 _rightStick = Vector2.zero;

        private int _flipFrameCount;
        private int _flipFrameMax = 25;

        private float _augmentedLeftAngle;
        private float _augmentedRightAngle;
        private float _invertVel;
        private float _toeAxis;
        private float _flip;
        private float _flipVel;
        private float _popDir;
        private float _popForce = Main.settings.PrimoPopForce;
        private float _popVel;
        private float _popStrengthTarget;
        private float _doubleSetupTimer;
        private float _popStrength;
        private float _setupBlend;
        private float setupHeightHigh = 1.45f;
        private float setupHeightLow = 1.35f;
        private float _flipWindowTimer;
        private bool _windupFoleyDelay;
        private float _windupFoleyTimer;
        private float _triggerVel;
        private float _prevWindUpTarget;
        private float _windUpLerp;

        public Custom_PrimoSetup(StickInput p_popStick, StickInput p_flipStick, bool p_forwardLoad, PlayerController.SetupDir p_setupDir)
        {
            _popStick = p_popStick;
            _flipStick = p_flipStick;
            _forwardLoad = p_forwardLoad;
            _setupDir = p_setupDir;
        }

        public override void Enter()
        {
            XXLController.CurrentState = CurrentState.PrimoSetup;
            //PlayerController.Instance.boardController.boardRigidbody.angularVelocity = Vector3.zero;
            PlayerController.Instance.SetBoardPhysicsMaterial(PlayerController.FrictionType.Brake);
            PlayerController.Instance.animationController.ScaleAnimSpeed(1f);
            PlayerController.Instance.AnimSetRollOff(false);
            PlayerController.Instance.ToggleFlipColliders(false);
            PlayerController.Instance.OnEnterSetupState();
            PlayerController.Instance.animationController.SetValue("EndImpact", false);
            SoundManager.Instance.PlayShoeMovementSound();
            PlayerController.Instance.SetArmWeights(0.4f, 0.8f, 0.5f);
            PlayerController.Instance.AnimSetupTransition(true);
            PlayerController.Instance.boardController.ResetAll();
            PlayerController.Instance.SetTurnMultiplier(1f);
            PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
            PlayerController.Instance.AnimSetupTransition(true);
            PlayerController.Instance.AnimPopInterruptedTransitions(false);
            PlayerController.Instance.OnEndImpact();
            PlayerController.Instance.AnimLandedEarly(false);
            PlayerController.Instance.CrossFadeAnimation("Setup", 0.1f);
            XXLController.Instance.ActivateSlowMotion(Main.settings.SlowMotionPrimos, Main.settings.SlowMotionPrimoSpeed);
        }

        public override void Update()
        {
            XXLController.Instance.PressureFlip();
            PlayerController.Instance.CacheRidingTransforms();
            if (PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude > 0.3f)
            {
                SoundManager.Instance.PlayPowerslideSound(PlayerController.Instance.GetSurfaceTag("Surface_Concrete"), PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude, 1);
                return;
            }
            SoundManager.Instance.StopPowerslideSound(1, PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            PlayerController.Instance.comController.UpdateCOM(setupHeight, 2);
            PlayerController.Instance.SetRotationTarget();
            PlayerController.Instance.LimitAngularVelocity(5f);
            //PlayerController.Instance.SnapRotation();
            PlayerController.Instance.SetRotationTarget();
            PlayerController.Instance.SkaterRotation(true, false);
        }

        public override void Exit()
        {
            XXLController.Instance.ResetTime(Main.settings.SlowMotionPrimos);
            SoundManager.Instance.StopPowerslideSound(1, PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude);
            PlayerController.Instance.SetBoardPhysicsMaterial(PlayerController.FrictionType.Default);
            PlayerController.Instance.SetArmWeights(0.2f, 0.4f, 0.15f);
            PlayerController.Instance.OnExitSetupState();
            PlayerController.Instance.AnimOllieTransition(false);
        }

        public override float GetAugmentedAngle(StickInput p_stick)
        {
            if (p_stick.IsRightStick)
            {
                return _augmentedRightAngle;
            }
            return _augmentedLeftAngle;
        }

        public override StickInput GetPopStick()
        {
            return _popStick;
        }

        public override void OnFlipStickUpdate()
        {
            if (_flipStick.SetupDir < -0.1f)
            {
                if (PlayerController.Instance.inputController.turningMode == InputController.TurningMode.Grounded)
                {
                    PlayerController.Instance.SetTurningMode(InputController.TurningMode.PreWind);
                }
                _popStrengthTarget = Mathf.Clamp(1.1f * Mathf.Abs(_flipStick.SetupDir), -1f, 1f);
                if (Mathf.Abs(PlayerController.Instance.GetWindUp()) > 0.2f)
                {
                    _doubleSetupTimer += Time.deltaTime;
                }
            }
            else
            {
                if (PlayerController.Instance.inputController.turningMode == InputController.TurningMode.PreWind)
                {
                    PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
                }
                _popStrengthTarget = 0f;
                _doubleSetupTimer = 0f;
            }
            if (_popStrength < _popStrengthTarget)
            {
                _popStrength = _popStrengthTarget;
            }
            else
            {
                _popStrength = Mathf.Lerp(_popStrength, _popStrengthTarget, Time.deltaTime * 5f);
            }
            _setupBlend = Mathf.Lerp(_setupBlend, _popStrength, Time.deltaTime * 10f);
            float num = setupHeightHigh - setupHeightLow;
            setupHeight = setupHeightHigh - _setupBlend * num;
            if (_windupFoleyDelay)
            {
                _windupFoleyTimer += Time.deltaTime;
                if (_windupFoleyTimer > 0.3f)
                {
                    _windupFoleyTimer = 0f;
                    _windupFoleyDelay = false;
                }
            }
            if (_doubleSetupTimer > 0.2f)
            {
                float num2 = PlayerController.Instance.GetWindUp();
                _triggerVel = Mathf.Abs(num2 - _prevWindUpTarget) / Time.deltaTime;
                if (Mathf.Abs(num2) > Mathf.Abs(_prevWindUpTarget) && _triggerVel > 10f && !_windupFoleyDelay)
                {
                    _windupFoleyDelay = true;
                    SoundManager.Instance.PlayMovementFoleySound(0.3f, true);
                }
                _prevWindUpTarget = num2;
                num2 = Mathf.Clamp(num2, -_setupBlend, _setupBlend);
                if (Mathf.Abs(_windUpLerp) < Mathf.Abs(num2))
                {
                    _windUpLerp = Mathf.Lerp(_windUpLerp, num2, Time.deltaTime * 10f);
                }
                else
                {
                    _windUpLerp = Mathf.Lerp(_windUpLerp, num2, Time.deltaTime * 2f);
                }
            }
            PlayerController.Instance.AnimSetWindUp(_windUpLerp);
            PlayerController.Instance.AnimSetPopStrength(_popStrength);
            PlayerController.Instance.AnimSetSetupBlend(_setupBlend);
            PlayerController.Instance.OnFlipStickUpdate(ref _flipDetected, ref _potentialFlip, ref _initialFlipDir, ref _flipFrameCount, ref _flipFrameMax, ref _toeAxis, ref _flipVel, ref _popVel, ref _popDir, ref _flip, _flipStick, false, true, ref _invertVel, _popStick.IsRightStick ? _augmentedLeftAngle : _augmentedRightAngle, false, _forwardLoad, ref _flipWindowTimer);
        }

        public override void OnPopStickCentered()
        {
            PlayerController.Instance.AnimSetupTransition(false);
            base.DoTransition(typeof(Custom_Primo), null);
        }

        public override void OnPopStickUpdate()
        {
            PlayerController.Instance.OnPopStartCheck(true, _popStick, ref _setupDir, _forwardLoad, 15f, ref _invertVel, _popStick.IsRightStick ? _augmentedRightAngle : _augmentedLeftAngle, ref _popVel, false);
        }

        public override void OnStickUpdate(StickInput p_leftStick, StickInput p_rightStick)
        {
            _leftStick = new Vector2(p_leftStick.ToeAxis, p_leftStick.ForwardDir);
            _rightStick = new Vector2(p_rightStick.ToeAxis, p_rightStick.ForwardDir);
            PlayerController.Instance.SetLeftIKOffset(p_leftStick.ToeAxis, p_leftStick.ForwardDir, p_leftStick.PopDir, p_leftStick.IsPopStick, false, false);
            PlayerController.Instance.SetRightIKOffset(p_rightStick.ToeAxis, p_rightStick.ForwardDir, p_rightStick.PopDir, p_rightStick.IsPopStick, false, false);
        }

        public override void OnNextState()
        {
            PlayerController.Instance.boardController.ReferenceBoardRotation();
            PlayerController.Instance.FixTargetNormal();
            PlayerController.Instance.SetTargetToMaster();
            PlayerController.Instance.AnimOllieTransition(true);
            float num = Mathf.Lerp(PlayerController.Instance.popForce, PlayerController.Instance.highPopForce, 1);
            if (_flipDetected)
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
                _forwardLoad,
                _invertVel,
                _setupDir,
                _augmentedLeftAngle,
                _augmentedRightAngle,
                false,
                false,
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
            _forwardLoad,
            _invertVel,
            _setupDir,
            _augmentedLeftAngle,
            _augmentedRightAngle,
            _popVel,
            _toeAxis,
            _popDir,
             false,
                false,
                true
            };
            base.DoTransition(typeof(Custom_BeginPop), args2);
            return;
        }

        public override bool IsInSetupState()
        {
            return true;
        }
    }
}