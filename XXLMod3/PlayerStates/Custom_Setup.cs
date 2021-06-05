using Dreamteck.Splines;
using FSMHelper;
using System;
using System.Collections.Generic;
using UnityEngine;
using XXLMod3.Controller;
using XXLMod3.Core;

namespace XXLMod3.PlayerStates
{
    public class Custom_Setup : PlayerState_OnBoard
    {
        private bool _canEnterCopingGrind = true;
        private bool _canGrind;
        private bool _canNoComply;
        private bool _colliding;
        private bool _flipDetected;
        private bool _footCollidersDisabled;
        private bool _forwardLoad;
        private bool _mongo;
        private bool _noComply;
        private bool _potentialFlip;
        private bool _windupFoleyDelay;

        private int _flipFrameCount;
        private int _flipFrameMax = 3;

        private float _augmentedLeftAngle;
        private float _augmentedRightAngle;
        private float _copingTimeLimit = 0.25f;
        private float _copingTimer;
        private float _doubleSetupTimer;
        private float _flip;
        private float _flipVel;
        private float _flipWindowTimer;
        private float _footColliderTimer;
        private float _invertVel;
        private float _noComplyTimer;
        private float _popDir;
        private float _popStrength;
        private float _popStrengthTarget;
        private float _popVel;
        private float _prevWindUpTarget;
        private float _setupBlend;
        private float _setupHeight = 0.79f;
        private float _setupHeightLow = 0.684f;
        private float _setupHeightHigh = 0.79f;
        private float _toeAxis;
        private float _triggerVel;
        private float _windupFoleyTimer;
        private float _windUpLerp;

        private PlayerController.SetupDir _setupDir;

        private StickInput _flipStick;
        private StickInput _popStick;

        private Vector2 _initialFlipDir = Vector2.zero;
        private Vector2 _leftStick = Vector2.zero;
        private Vector2 _rightStick = Vector2.zero;

        private float _babyPopTimer;
        private float _highPopForce;
        private float _popForce;

        public override void SetupDefinition(ref FSMStateType stateType, ref List<Type> children)
        {
            stateType = FSMStateType.Type_OR;
        }

        public Custom_Setup(StickInput p_popStick, StickInput p_flipStick, bool p_forwardLoad, PlayerController.SetupDir p_setupDir)
        {
            _popStick = p_popStick;
            _flipStick = p_flipStick;
            _forwardLoad = p_forwardLoad;
            _setupDir = p_setupDir;
        }

        public Custom_Setup(StickInput p_popStick, StickInput p_flipStick, bool p_forwardLoad, PlayerController.SetupDir p_setupDir, bool p_mongo)
        {
            _popStick = p_popStick;
            _flipStick = p_flipStick;
            _forwardLoad = p_forwardLoad;
            _setupDir = p_setupDir;
            _mongo = p_mongo;
        }

        public override void Enter()
        {
            PlayerController.Instance.currentStateEnum = PlayerController.CurrentState.Setup;
            XXLController.CurrentState = CurrentState.Setup;
            PlayerController.Instance.AnimSetRollOff(false);
            PlayerController.Instance.ToggleFlipColliders(false);
            PlayerController.Instance.OnEnterSetupState();
            PlayerController.Instance.animationController.SetValue("EndImpact", false);
            SoundManager.Instance.PlayShoeMovementSound();
            SoundManager.Instance.PlayMovementFoleySound(0.3f, true);
            if (PlayerController.Instance.skaterController.rightFootCollider.isTrigger || PlayerController.Instance.skaterController.leftFootCollider.isTrigger)
            {
                _footCollidersDisabled = true;
            }
            PlayerController.Instance.SetArmWeights(0.4f, 0.8f, 0.5f);
            PlayerController.Instance.AnimSetPush(false);
            PlayerController.Instance.AnimSetMongo(false);
            PlayerController.Instance.AnimSetupTransition(true);
            PlayerController.Instance.boardController.ResetAll();
            PlayerController.Instance.SetTurnMultiplier(0.5f);
            PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
            PlayerController.Instance.AnimSetupTransition(true);
            PlayerController.Instance.AnimPopInterruptedTransitions(false);
            PlayerController.Instance.OnEndImpact();
            PlayerController.Instance.AnimLandedEarly(false);
            PlayerController.Instance.CrossFadeAnimation("Setup", 0.1f);
        }

        public override StickInput GetPopStick()
        {
            return _popStick;
        }

        public override void Exit()
        {
            PlayerController.Instance.SetArmWeights(0.2f, 0.4f, 0.15f);
            PlayerController.Instance.OnExitSetupState();
            PlayerController.Instance.AnimOllieTransition(false);
        }

        public override void Update()
        {
            base.Update();
            PlayerController.Instance.animationController.skaterAnim.GetBoneTransform(HumanBodyBones.LeftToes).localRotation = Quaternion.Slerp(PlayerController.Instance.animationController.skaterAnim.GetBoneTransform(HumanBodyBones.LeftToes).localRotation, StanceController.Instance.LTIndicator.transform.localRotation, 2f);
            PlayerController.Instance.animationController.skaterAnim.GetBoneTransform(HumanBodyBones.RightToes).localRotation = Quaternion.Slerp(PlayerController.Instance.animationController.skaterAnim.GetBoneTransform(HumanBodyBones.RightToes).localRotation, StanceController.Instance.RTIndicator.transform.localRotation, 2f);
            _popForce = PopForce() * BabyPop(); //////
            AdvancedPop(); ///////
            SoundManager.Instance.SetRollingVolumeFromRPS(PlayerController.Instance.GetSurfaceTag(PlayerController.Instance.boardController.GetSurfaceTagString()), PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude);
            if (_canEnterCopingGrind)
            {
                _copingTimer += Time.deltaTime;
                if (_copingTimer >= _copingTimeLimit)
                {
                    _canEnterCopingGrind = false;
                }
            }
            if (!PlayerController.Instance.IsGrounded() && !_colliding)
            {
                PlayerController.Instance.SetTurningMode(InputController.TurningMode.InAir);
                PlayerController.Instance.SetSkaterToMaster();
                PlayerController.Instance.AnimSetRollOff(true);
                PlayerController.Instance.CrossFadeAnimation("Extend", 0.3f);
                PlayerController.Instance.skaterController.skaterRigidbody.angularVelocity = Vector3.zero;
                PlayerController.Instance.OnExtendAnimEnter();
                bool flag = false;
                if (Mathf.Abs(Vector3.Dot(PlayerController.Instance.boardController.LastGroundNormalWhenAllDown, Vector3.up)) < 0.3f)
                {
                    flag = true;
                }
                if (flag)
                {
                    PlayerController.Instance.skaterController.skaterRigidbody.velocity = Vector3.ProjectOnPlane(PlayerController.Instance.skaterController.skaterRigidbody.velocity, PlayerController.Instance.boardController.LastGroundNormalWhenAllDown);
                }
                Vector3 force = PlayerController.Instance.skaterController.PredictLanding(PlayerController.Instance.boardController.boardRigidbody.velocity);
                if (!flag)
                {
                    PlayerController.Instance.skaterController.skaterRigidbody.AddForce(force, ForceMode.Impulse);
                }
                EventManager.Instance.EnterAir();
                object[] args = new object[]
                {
                false,
                false,
                0f
                };
                base.DoTransition(typeof(Custom_InAir), args);
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
            if (PlayerController.Instance.boardController.AllDown)
            {
                PlayerController.Instance.CacheRidingTransforms();
            }
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

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            PlayerController.Instance.ScalePlayerCollider();
            PlayerController.Instance.boardController.SetBoardControllerUpVector(PlayerController.Instance.skaterController.skaterTransform.up);
            PlayerController.AutoRampTags autoRampTags = PlayerController.Instance.AutoPumpAndRevertCheck();
            if (autoRampTags != PlayerController.AutoRampTags.AutoRevert && autoRampTags == PlayerController.AutoRampTags.AutoPumpAndRevert)
            {
                PlayerController.Instance.AutoPump();
            }
            PlayerController.Instance.ApplyFriction();
            PlayerController.Instance.SetRotationTarget();
            PlayerController.Instance.LimitAngularVelocity(5f);
            PlayerController.Instance.SkaterRotation(true, false);
            PlayerController.Instance.boardController.DoBoardLean();
            PlayerController.Instance.comController.UpdateCOM(_setupHeight, 2);
            PlayerController.Instance.boardController.ApplyOnBoardMaxRoll(_colliding, 60f);
            _colliding = false;
        }

        public override float GetAugmentedAngle(StickInput p_stick)
        {
            if (p_stick.IsRightStick)
            {
                return _augmentedRightAngle;
            }
            return _augmentedLeftAngle;
        }

        public override void OnPopStickCentered()
        {
            PlayerController.Instance.AnimSetupTransition(false);
            base.DoTransition(typeof(Custom_Riding), null);
        }

        public override void OnStickUpdate(StickInput p_leftStick, StickInput p_rightStick)
        {
            _leftStick = new Vector2(p_leftStick.ToeAxis, p_leftStick.ForwardDir);
            _rightStick = new Vector2(p_rightStick.ToeAxis, p_rightStick.ForwardDir);
            PlayerController.Instance.SetLeftIKOffset(p_leftStick.ToeAxis, p_leftStick.ForwardDir, p_leftStick.PopDir, p_leftStick.IsPopStick, false, false);
            PlayerController.Instance.SetRightIKOffset(p_rightStick.ToeAxis, p_rightStick.ForwardDir, p_rightStick.PopDir, p_rightStick.IsPopStick, false, false);
        }

        public override void OnPopStickUpdate()
        {
            PlayerController.Instance.OnPopStartCheck(PlayerController.Instance.IsGrounded(), _popStick, ref _setupDir, _forwardLoad, PlayerController.Instance.popThreshold, ref _invertVel, _popStick.IsRightStick ? _augmentedRightAngle : _augmentedLeftAngle, ref _popVel, false);
        }

        public override void OnNextState()
        {
            _highPopForce = _popForce * (Main.settings.HighPopForceMult + 0.65f);
            PlayerController.Instance.boardController.ReferenceBoardRotation();
            PlayerController.Instance.FixTargetNormal();
            PlayerController.Instance.SetTargetToMaster();
            PlayerController.Instance.AnimOllieTransition(true);
            if (_windUpLerp > 0.1f && _popStrength > 0.1f)
            {
                PlayerController.Instance.SetTurnMultiplier(_windUpLerp);
                PlayerController.Instance.SetTurningMode(InputController.TurningMode.FastRight);
            }
            else if (_windUpLerp < -0.1f && _popStrength > 0.1f)
            {
                PlayerController.Instance.SetTurnMultiplier(Mathf.Abs(_windUpLerp));
                PlayerController.Instance.SetTurningMode(InputController.TurningMode.FastLeft);
            }
            else
            {
                PlayerController.Instance.SetTurnMultiplier(1f);
                PlayerController.Instance.SetTurningMode(InputController.TurningMode.InAir);
            }
            float num = Mathf.Lerp(_popForce, _highPopForce, _popStrength);
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
                num,
                _forwardLoad,
                _invertVel,
                _setupDir,
                _augmentedLeftAngle,
                _augmentedRightAngle
                };
                base.DoTransition(typeof(Custom_BeginPop), args);
                return;
            }
            object[] args2 = new object[]
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
            base.DoTransition(typeof(Custom_BeginPop), args2);
        }

        public override void OnFlipStickUpdate()
        {
            if (_flipStick.SetupDir < -0.1f)
            {
                if (PlayerController.Instance.inputController.turningMode == InputController.TurningMode.Grounded)
                {
                    if (Main.settings.LockTurningWhileWindUp)
                    {
                        PlayerController.Instance.SetTurnMultiplier(0f);
                    }
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
                    if (Main.settings.LockTurningWhileWindUp)
                    {
                        PlayerController.Instance.SetTurnMultiplier(0.5f);
                    }
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
            float num = _setupHeightHigh - _setupHeightLow;
            _setupHeight = _setupHeightHigh - _setupBlend * num;
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

        public override bool IsInSetupState()
        {
            return true;
        }

        public override bool IsOnGroundState()
        {
            return false;
        }

        public override void OnCopingCheck(SplineComputer _spline)
        {
            if (_canEnterCopingGrind)
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



        private float BabyPop()
        {
            if (Main.settings.BabyPop)
            {
                _babyPopTimer += Time.deltaTime;
                if (_babyPopTimer <= 0.1f)
                {
                    return Main.settings.BabyPopForceMult;
                }
                return 1f;
            }
            return 1f;
        }

        private float PopForce()
        {
            if(Main.settings.IndividualPopForce)
            {
                if (!PlayerController.Instance.IsSwitch)
                {
                    if (!_forwardLoad)
                    {
                        return Main.settings.DefaultPopForce;
                    }
                    else
                    {
                        return Main.settings.NolliePopForce;
                    }
                }
                else if (!_forwardLoad)
                {
                    return Main.settings.FakiePopForce;
                }
                else
                {
                    return Main.settings.SwitchPopForce;
                }
            }
            else
            {
                return Main.settings.DefaultPopForce;
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
    }
}
