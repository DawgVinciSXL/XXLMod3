using FSMHelper;
using SkaterXL.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using XXLMod3.Controller;
using XXLMod3.Core;

namespace XXLMod3.PlayerStates
{
    public class Custom_Pushing : PlayerState_OnBoard
    {
        private bool _canPushWithButton;
        private bool _colliding;
        private bool _mongo;
        private bool _pushButtonHeld;
        private bool _pushButtonPressed;
        private bool _pushing;

        private float _holdTimer;
        private float _pushPower;
        private float _pushTimer;
        private float _timeInState;
        private float _timeSincePush;

        private int _pushCount;

        private PlayerController.SetupDir _setupDir;

        private StickInput _noComplyStick;
        private StickInput _pushStick;

        public override void SetupDefinition(ref FSMStateType p_stateType, ref List<Type> children)
        {
            p_stateType = FSMStateType.Type_OR;
        }

        public Custom_Pushing(bool p_mongo)
        {
            _mongo = p_mongo;
        }

        public Custom_Pushing(StickInput p_pushStick, StickInput p_noComplyStick)
        {
            _pushStick = p_pushStick;
            _noComplyStick = p_noComplyStick;
        }

        public override void Enter()
        {
            PlayerController.Instance.currentStateEnum = PlayerController.CurrentState.Pushing;
            XXLController.CurrentState = CurrentState.Pushing;
            PlayerController.Instance.AnimSetRollOff(false);
            EventManager.Instance.OnPushing(_mongo);
            PlayerController.Instance.ToggleFlipColliders(false);
            PlayerController.Instance.CacheRidingTransforms();
            if (SettingsManager.Instance.stance == Stance.Regular)
            {
                if (!_mongo)
                {
                    if (!PlayerController.Instance.IsSwitch)
                    {
                        PlayerController.Instance.skaterController.rightFootCollider.isTrigger = true;
                        PlayerController.Instance.SetRightIKWeight(0f);
                        PlayerController.Instance.SetRightIKLerpTarget(1f);
                        PlayerController.Instance.SetLeftIKLerpTarget(0f);
                    }
                    else
                    {
                        PlayerController.Instance.skaterController.leftFootCollider.isTrigger = true;
                        PlayerController.Instance.SetLeftIKWeight(0f);
                        PlayerController.Instance.SetLeftIKLerpTarget(1f);
                        PlayerController.Instance.SetRightIKLerpTarget(0f);
                    }
                }
                else if (!PlayerController.Instance.IsSwitch)
                {
                    PlayerController.Instance.skaterController.leftFootCollider.isTrigger = true;
                    PlayerController.Instance.SetLeftIKWeight(0f);
                    PlayerController.Instance.SetLeftIKLerpTarget(1f);
                    PlayerController.Instance.SetRightIKLerpTarget(0f);
                }
                else
                {
                    PlayerController.Instance.skaterController.rightFootCollider.isTrigger = true;
                    PlayerController.Instance.SetRightIKWeight(0f);
                    PlayerController.Instance.SetRightIKLerpTarget(1f);
                    PlayerController.Instance.SetLeftIKLerpTarget(0f);
                }
            }
            else if (!_mongo)
            {
                if (!PlayerController.Instance.IsSwitch)
                {
                    PlayerController.Instance.skaterController.leftFootCollider.isTrigger = true;
                    PlayerController.Instance.SetLeftIKWeight(0f);
                    PlayerController.Instance.SetLeftIKLerpTarget(1f);
                    PlayerController.Instance.SetRightIKLerpTarget(0f);
                }
                else
                {
                    PlayerController.Instance.skaterController.rightFootCollider.isTrigger = true;
                    PlayerController.Instance.SetRightIKWeight(0f);
                    PlayerController.Instance.SetRightIKLerpTarget(1f);
                    PlayerController.Instance.SetLeftIKLerpTarget(0f);
                }
            }
            else if (!PlayerController.Instance.IsSwitch)
            {
                PlayerController.Instance.skaterController.rightFootCollider.isTrigger = true;
                PlayerController.Instance.SetRightIKWeight(0f);
                PlayerController.Instance.SetRightIKLerpTarget(1f);
                PlayerController.Instance.SetLeftIKLerpTarget(0f);
            }
            else
            {
                PlayerController.Instance.skaterController.leftFootCollider.isTrigger = true;
                PlayerController.Instance.SetLeftIKWeight(0f);
                PlayerController.Instance.SetLeftIKLerpTarget(1f);
                PlayerController.Instance.SetRightIKLerpTarget(0f);
            }
            if (_mongo)
            {
                PlayerController.Instance.AnimSetMongo(true);
                PlayerController.Instance.CrossFadeAnimation("MongoRidingToPush", 0.15f);
            }
            else
            {
                PlayerController.Instance.AnimSetPush(true);
                PlayerController.Instance.CrossFadeAnimation("RidingToPush", 0.15f);
            }
            PlayerController.Instance.SetTurnMultiplier(1f);
            PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
            StanceController.IsMongoPushing = _mongo;
        }

        public override void Exit()
        {
            PlayerController.Instance.animationController.ScaleAnimSpeed(1f);
            PlayerController.Instance.SetRightIKLerpTarget(0f);
            PlayerController.Instance.SetRightIKWeight(1f);
            PlayerController.Instance.SetLeftIKLerpTarget(0f);
            PlayerController.Instance.SetLeftIKWeight(1f);
            PlayerController.Instance.AnimSetPush(false);
            PlayerController.Instance.AnimSetMongo(false);
        }

        public override void Update()
        {
            base.Update();
            _timeSincePush += Time.deltaTime;
            _timeInState += Time.deltaTime;
            PlayerController.Instance.CacheRidingTransforms();
            SoundManager.Instance.SetRollingVolumeFromRPS(PlayerController.Instance.GetSurfaceTag(PlayerController.Instance.boardController.GetSurfaceTagString()), PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude);
            if (_pushing)
            {
                _pushTimer += Time.deltaTime;
                if (_pushTimer > 0.25f)
                {
                    _pushing = false;
                    _pushTimer = 0f;
                }
            }
            if (!PlayerController.Instance.IsGrounded() && !PlayerController.Instance.IsRespawning && !_colliding)
            {
                ExitToRiding();
                PlayerController.Instance.CrossFadeAnimation("Extend", 0.1f);
            }
            if (_timeSincePush > 0.7f)
            {
                ExitToRiding();
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
            PlayerController.Instance.boardController.SetBoardControllerUpVector(PlayerController.Instance.skaterController.skaterTransform.up);
            PlayerController.Instance.comController.UpdateCOM(Main.settings.PushCrouchAmount, 1);
            PlayerController.Instance.ApplyFriction();
            PlayerController.Instance.SetRotationTarget();
            PlayerController.Instance.SkaterRotation(true, false);
            PlayerController.Instance.boardController.ApplyOnBoardMaxRoll(_colliding, 60f);
            PlayerController.Instance.boardController.DoBoardLean();
            PlayerController.Instance.SetBoardTargetPosition(0f);
            PlayerController.Instance.SetFrontPivotRotation(0f);
            PlayerController.Instance.SetBackPivotRotation(0f);
            PlayerController.Instance.SetPivotForwardRotation(0f, 40f);
            PlayerController.Instance.SetPivotSideRotation(0f);
            _colliding = false;
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
        }

        public override bool IsPushing()
        {
            return true;
        }

        public override bool IsOnGroundState()
        {
            return true;
        }

        public override void ExitPushing()
        {
            ExitToRiding();
        }

        public override void OnStickUpdate(StickInput p_leftStick, StickInput p_rightStick)
        {
            if (p_leftStick.SetupDir > 0.8f || (new Vector2(p_leftStick.ToeAxis, p_leftStick.SetupDir).magnitude > 0.8f && p_leftStick.SetupDir > 0.325f))
            {
                PlayerController.Instance.SetRightIKWeight(1f);
                PlayerController.Instance.SetRightIKLerpTarget(0f);
                PlayerController.Instance.SetLeftIKWeight(1f);
                PlayerController.Instance.SetLeftIKLerpTarget(0f);
                PlayerController.Instance.AnimSetNollie(PlayerController.Instance.GetNollie(false));
                PlayerController.Instance.SetupDirection(p_leftStick, ref _setupDir);
                PlayerController.Instance.AnimSetPush(false);
                PlayerController.Instance.AnimSetMongo(false);
                PlayerController.Instance.AnimSetupTransition(true);
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
                PlayerController.Instance.SetRightIKWeight(1f);
                PlayerController.Instance.SetRightIKLerpTarget(0f);
                PlayerController.Instance.SetLeftIKWeight(1f);
                PlayerController.Instance.SetLeftIKLerpTarget(0f);
                PlayerController.Instance.AnimSetNollie(PlayerController.Instance.GetNollie(true));
                PlayerController.Instance.SetupDirection(p_rightStick, ref _setupDir);
                PlayerController.Instance.AnimSetupTransition(true);
                PlayerController.Instance.AnimSetPush(false);
                PlayerController.Instance.AnimSetMongo(false);
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
            if (SettingsManager.Instance.stance == Stance.Regular)
            {
                if (!_mongo)
                {
                    PlayerController.Instance.SetLeftIKOffset(0f, 0f, 0f, p_leftStick.IsPopStick, false, false);
                    PlayerController.Instance.SetRightIKOffset(0f, 0f, 0f, p_rightStick.IsPopStick, false, false);
                    return;
                }
                PlayerController.Instance.SetLeftIKOffset(0f, 0f, 0f, p_leftStick.IsPopStick, false, false);
                PlayerController.Instance.SetRightIKOffset(0f, 0f, 0f, p_rightStick.IsPopStick, false, false);
                return;
            }
            else
            {
                if (!_mongo)
                {
                    PlayerController.Instance.SetLeftIKOffset(0f, 0f, 0f, p_leftStick.IsPopStick, false, false);
                    PlayerController.Instance.SetRightIKOffset(0f, 0f, 0f, p_rightStick.IsPopStick, false, false);
                    return;
                }
                PlayerController.Instance.SetLeftIKOffset(0f, 0f, 0f, p_leftStick.IsPopStick, false, false);
                PlayerController.Instance.SetRightIKOffset(0f, 0f, 0f, p_rightStick.IsPopStick, false, false);
                return;
            }
        }

        private void ExitToRiding()
        {
            PlayerController.Instance.animationController.ScaleAnimSpeed(1f);
            PlayerController.Instance.SetIKLerpSpeed(1f);
            PlayerController.Instance.SetLeftIKLerpTarget(0f);
            PlayerController.Instance.SetRightIKLerpTarget(0f);
            PlayerController.Instance.SetRightIKWeight(1f);
            PlayerController.Instance.SetLeftIKWeight(1f);
            object[] args = new object[]
            {
            13
            };
            base.DoTransition(typeof(Custom_Riding), args);
        }

        public override void OnPushButtonPressed(bool p_mongo)
        {
            _timeSincePush = 0f;
            if (p_mongo != _mongo)
            {
                ExitToRiding();
            }
            _pushCount++;
            _pushButtonPressed = true;
            if ((float)_pushCount > 2f)
            {
                PlayerController.Instance.animationController.ScaleAnimSpeed(1.25f);
            }
        }

        public override void OnPushButtonHeld(bool p_mongo)
        {
            _timeSincePush = 0f;
            if (p_mongo != _mongo)
            {
                ExitToRiding();
            }
            _pushButtonPressed = true;
            bool canPushWithButton = _canPushWithButton;
            if (!_pushButtonHeld)
            {
                _holdTimer += Time.deltaTime;
                if (_holdTimer > 0.2f)
                {
                    PlayerController.Instance.animationController.ScaleAnimSpeed(1.25f);
                    _pushButtonHeld = true;
                    _pushPower = Mathf.Clamp(_pushPower, 0.8f, 1f);
                }
            }
            _pushPower += Time.deltaTime;
        }

        public override void OnPushButtonReleased()
        {
            _holdTimer = 0f;
            if (_pushButtonHeld)
            {
                _pushButtonPressed = false;
            }
        }

        public override void OnPushEnd()
        {
            if (!_pushButtonPressed)
            {
                ExitToRiding();
                return;
            }
            _pushCount = 0;
            _pushButtonPressed = false;
        }

        public override void OnPushLastCheck()
        {
            _canPushWithButton = true;
        }

        public override void OnPush()
        {
            if (!_pushing)
            {
                _pushPower = Mathf.Clamp(_pushPower, (_pushCount > 2) ? 0.9f : Main.settings.FirstPushForce, 16f);
                PlayerController.Instance.AddPushForce(PlayerController.Instance.GetPushForce() * 1.8f * _pushPower);
                _pushPower = 0f;
                _pushing = true;
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
    }
}