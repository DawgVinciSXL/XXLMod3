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
    public class Custom_EnterCoping : PlayerState_OnBoard
    {
        private bool _timerDone;

        private double percent;

        private float _exitTime = 1f;
        private float _horizontalLerp;
        private float _horizontalTarget;
        private float _leftForwardDir;
        private float _leftToeAxis;
        private float _leftTrigger;
        private float _maxTime = 0.5f;
        private float _playerTurn;
        private float _rightForwardDir;
        private float _rightToeAxis;
        private float _rightTrigger;
        private float _timer;
        private float _verticalLerp;
        private float _verticalTarget;
        private float _y;

        private Quaternion _newRot = Quaternion.identity;
        private Quaternion _playerRot = Quaternion.identity;
        private Quaternion _skaterRot = Quaternion.identity;
        private Quaternion _targetRot;

        private SplineComputer _spline;
        private SplineResult _splineResult;

        private Transform _targetTransform;

        private Vector3 _grindDir;
        private Vector3 _grindUp;
        private Vector3 _grindVel;
        private Vector3 _initialVel;
        private Vector3 _lastTargetPos = Vector3.zero;
        private Vector3 _projectedVel = Vector3.zero;
        private Vector3 _rampDir = Vector3.zero;
        private Vector3 _splinePos;
        private Vector3 _targetPos;

        public override void SetupDefinition(ref FSMStateType stateType, ref List<Type> children)
        {
            stateType = FSMStateType.Type_OR;
        }

        public Custom_EnterCoping(Transform p_targetTransform, Quaternion p_targetRot, SplineComputer p_spline, Vector3 p_grindDir)
        {
            _targetTransform = p_targetTransform;
            _targetRot = p_targetRot;
            _spline = p_spline;
            _grindDir = p_grindDir;
            _initialVel = PlayerController.Instance.VelocityOnPop;
            _rampDir = Vector3.ProjectOnPlane(_initialVel, _grindDir);
            _rampDir = Vector3.ProjectOnPlane(_rampDir, _grindUp).normalized;
        }

        public override void Enter()
        {
            PlayerController.Instance.currentStateEnum = PlayerController.CurrentState.EnterCoping;
            XXLController.CurrentState = CurrentState.EnterCoping;
            PlayerController.Instance.cameraController.IsInGrindState = true;
            PlayerController.Instance.cameraController.IsInCopingState = true;
            PlayerController.Instance.cameraController.NeedToSlowLerpCamera = true;
            PlayerController.Instance.ToggleFlipColliders(false);
            SoundManager.Instance.PlayGrindSound(PlayerController.Instance.boardController.isSliding ? 1 : PlayerController.Instance.boardController.GetGrindSoundInt(), Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardRigidbody.velocity, Vector3.up).magnitude);
            PlayerController.Instance.skaterController.skaterRigidbody.angularVelocity = Vector3.zero;
            PlayerController.Instance.skaterController.skaterRigidbody.velocity = Vector3.zero;
            percent = _spline.Project(_targetTransform.position, 3, 0.0, 1.0);
            _splineResult = _spline.Evaluate(percent);
            _lastTargetPos = _splineResult.position;
            PlayerController.Instance.boardController.SetCopingTargetLerp(_targetTransform.position);
            PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
            _projectedVel = Vector3.Project(_initialVel, _splineResult.direction);
            float magnitude = Vector3.Project(_initialVel, _splineResult.direction).magnitude;
            _grindVel = _grindDir * magnitude;
            PlayerController.Instance.boardController.triggerManager.sideEnteredGrind = (PlayerController.Instance.IsRightSideOfStall(_initialVel, _splineResult.right) ? TriggerManager.SideEnteredGrind.Right : TriggerManager.SideEnteredGrind.Left);
            PlayerController.Instance.boardController.triggerManager.grindDetection.grindSide = (PlayerController.Instance.IsBacksideGrind(_splineResult) ? GrindSide.Backside : GrindSide.Frontside);
            PlayerController.Instance.AnimGrindTransition(true);
            PlayerController.Instance.CrossFadeAnimation("Grinds", 0.6f);
            PlayerController.Instance.AnimOllieTransition(false);
            PlayerController.Instance.AnimSetupTransition(false);
            PlayerController.Instance.AnimRelease(false);
        }

        public override void Exit()
        {
            PlayerController.Instance.cameraController.NeedToSlowLerpCamera = false;
            PlayerController.Instance.cameraController.IsInGrindState = false;
            PlayerController.Instance.cameraController.IsInCopingState = false;
        }

        public override void Update()
        {
            base.Update();
            SoundManager.Instance.SetRollingVolumeFromRPS(PlayerController.Instance.GetSurfaceTag(PlayerController.Instance.boardController.GetSurfaceTagString()), PlayerController.Instance.boardController._rollSoundSpeed);
            _timer += Time.deltaTime;
            SoundManager.Instance.SetGrindVolume(PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude);
            CheckForSplineEnd();
            RunTimer();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            MainUpdate();
        }

        private void MainUpdate()
        {
            PlayerController.Instance.boardController.SetBoardControllerUpVector(PlayerController.Instance.skaterController.skaterTransform.up);
            percent = _spline.Project(_targetTransform.position, 3, 0.0, 1.0);
            _splineResult = _spline.Evaluate(percent);
            _grindUp = _splineResult.normal;
            PlayerController.Instance.SetRotationTarget();
            PlayerController.Instance.comController.UpdateCOM(0.95f, 0);
            SetBoardPosition();
            SetBoardRotation();
            SetSkaterRotation();
            if (ReachedPosition() && ReachedRotation())
            {
                TransitionToGrinds();
            }
            PlayerController.Instance.boardController.GroundY = Mathf.Lerp(PlayerController.Instance.boardController.GroundY, PlayerController.Instance.boardController.triggerManager.grindContactSplinePosition.position.y, Time.deltaTime * 0.5f);
        }

        private void SetBoardPosition()
        {
            Vector3 vector = ((_splineResult.position - _lastTargetPos) / Time.deltaTime).normalized * _projectedVel.magnitude;
            if (Vector3.Dot(vector, _projectedVel) < 0f)
            {
                vector = -vector;
            }
            PlayerController.Instance.boardController.LerpCopingTarget();
            float d = Mathf.Clamp((PlayerController.Instance.boardController._copingTargetLerp.position - _splineResult.position).magnitude, 0f, 0.1f) * 0.5f;
            PlayerController.Instance.boardController.PIDPosition(vector, PlayerController.Instance.boardController._copingTargetLerp.position, _splineResult.position + Vector3.up * d, 3f);
            _lastTargetPos = _splineResult.position;
        }

        private void SetBoardRotation()
        {
            PlayerController.Instance.BoardGrindRotation = Quaternion.Slerp(PlayerController.Instance.boardController.boardRigidbody.rotation, PlayerController.Instance.GetBoardBackwards() ? PlayerController.Instance.boardController.triggerManager.grindOffset.rotation : PlayerController.Instance.boardOffsetRoot.rotation, Time.fixedDeltaTime * 90f);
            PlayerController.Instance.boardController.CopingGrindRotation(PlayerController.Instance.BoardGrindRotation, 2f, 200f, 1f);
        }

        private void SetSkaterRotation()
        {
            Vector3 toDirection = Vector3.Lerp(PlayerController.Instance.skaterController.skaterTransform.up, _grindUp, Time.deltaTime * 75f);
            _skaterRot = Quaternion.FromToRotation(PlayerController.Instance.skaterController.skaterTransform.up, toDirection) * PlayerController.Instance.skaterController.skaterTransform.rotation;
            _newRot = Quaternion.AngleAxis(_playerTurn * 10f, PlayerController.Instance.skaterController.skaterTransform.up) * _skaterRot;
            PlayerController.Instance.SkaterRotation(_newRot, 0.95f);
        }

        private void CheckForSplineEnd()
        {
            if (_timer < 0.2f)
            {
                return;
            }
            if ((_splineResult.percent > 0.89999997615814209 || _splineResult.percent < 0.10000000149011612) && IsCloseToEitherEnd(_splineResult.percent > 0.89999997615814209))
            {
                EndCopingGrind();
            }
        }

        private bool IsCloseToEitherEnd(bool p_end)
        {
            Vector3 vector = _spline.Evaluate((double)(p_end ? 1f : 0f)).position - _splineResult.position;
            bool flag = Vector3.Angle(Vector3.Project(PlayerController.Instance.boardController.boardRigidbody.velocity, _splineResult.direction), _splineResult.direction) < 90f;
            return (double)vector.magnitude < 0.05 && ((p_end && flag) || (!p_end && !flag));
        }

        private void RunTimer()
        {
            if (!_timerDone && _timer > _maxTime)
            {
                _timerDone = true;
            }
            if (_timer > _exitTime)
            {
                TransitionToGrinds();
            }
        }

        public override void LeftTriggerHeld(float p_value, InputController.TurningMode p_turningMode)
        {
            RotatePlayer(-p_value);
            _leftTrigger = p_value;
        }

        public override void LeftTriggerReleased()
        {
            _leftTrigger = 0f;
        }

        public override void RightTriggerHeld(float p_value, InputController.TurningMode p_turningMode)
        {
            RotatePlayer(p_value);
            _rightTrigger = p_value;
        }

        public override void RightTriggerReleased()
        {
            _rightTrigger = 0f;
        }

        private void RotatePlayer(float p_value)
        {
            _playerTurn = p_value * Time.deltaTime * 300f;
        }

        public override void BothTriggersReleased(InputController.TurningMode turningMode)
        {
            _playerTurn = 0f;
        }

        public override bool IsInCopingEnterState()
        {
            return true;
        }

        private void TransitionToGrinds()
        {
            PlayerController.Instance.skaterController.skaterRigidbody.velocity = _grindVel;
            PlayerController.Instance.boardController.triggerManager.spline = _spline;
            PlayerController.Instance.boardController.triggerManager.IsColliding = true;
            PlayerController.Instance.boardController.triggerManager.wasColliding = true;
            object[] args = new object[]
            {
            _spline,
            true,
            _horizontalTarget,
            _verticalTarget,
            _horizontalLerp,
            _verticalLerp
            };
            base.DoTransition(typeof(Custom_Grinding), args);
        }

        private bool ReachedPosition()
        {
            return (PlayerController.Instance.boardController._copingTargetLerp.position - _splineResult.position).magnitude < 0.05f && PlayerController.Instance.boardController.triggerManager.IsColliding;
        }

        private bool ReachedRotation()
        {
            return Quaternion.Angle(PlayerController.Instance.boardController.boardTransform.rotation, PlayerController.Instance.BoardGrindRotation) < 10f;
        }

        public override bool CanGrind()
        {
            return true;
        }

        public override void OnStickFixedUpdate(StickInput p_leftStick, StickInput p_rightStick)
        {
            _y = Mathf.Clamp(p_leftStick.rawInput.pos.y + p_rightStick.rawInput.pos.y, -1f, 1f);
            if (!_timerDone)
            {
                PlayerController.Instance.boardController.SetCopingTarget(_y, Vector3.Angle(PlayerController.Instance.boardController.boardTransform.forward, _grindDir));
            }
            RotateBoard(p_leftStick, p_rightStick);
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

        private void RotateBoard(StickInput p_leftStick, StickInput p_rightStick)
        {
            _horizontalTarget = -p_rightStick.ToeAxis * 44f + p_leftStick.ToeAxis * 44f;
            _horizontalLerp = Mathf.Lerp(_horizontalLerp, _horizontalTarget, Time.deltaTime * PlayerController.Instance.horizontalSpeed);
            float num = PlayerController.Instance.TwoWheelsDown() ? 22f : 15f;
            _verticalTarget = p_rightStick.ForwardDir * num + p_leftStick.ForwardDir * num;
            _verticalLerp = Mathf.Lerp(_verticalLerp, _verticalTarget, Time.deltaTime * PlayerController.Instance.verticalSpeed);
            PlayerController.Instance.GrindRotateBoard(_horizontalLerp, _verticalLerp);
        }

        private void EndCopingGrind()
        {
            PlayerController.Instance.AnimGrindTransition(false);
            PlayerController.Instance.SetTurningMode(InputController.TurningMode.InAir);
            PlayerController.Instance.SetSkaterToMaster();
            if (!PlayerController.Instance.IsRespawning)
            {
                PlayerController.Instance.CrossFadeAnimation("Extend", 0.5f);
            }
            PlayerController.Instance.OnExtendAnimEnter();
            float magnitude = PlayerController.Instance.skaterController.skaterRigidbody.velocity.magnitude;
            SoundManager.Instance.StopGrindSound((!SkaterXL.Core.Mathd.IsInfinityOrNaN(PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude)) ? PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude : 0f);
            Vector3 vector = Vector3.ProjectOnPlane(PlayerController.Instance.skaterController.skaterRigidbody.velocity, PlayerController.Instance.GetGrindRight()).normalized * magnitude;
            PlayerController.Instance.skaterController.skaterRigidbody.velocity = Vector3.ClampMagnitude(vector, _grindVel.magnitude);
            Vector3 force = PlayerController.Instance.skaterController.PredictLanding(PlayerController.Instance.skaterController.skaterRigidbody.velocity);
            PlayerController.Instance.skaterController.skaterRigidbody.AddForce(force, ForceMode.Impulse);
            EventManager.Instance.EnterAir();
            object[] args = new object[]
            {
            true,
            false,
            0f
            };
            base.DoTransition(typeof(Custom_InAir), args);
        }

        public override void OnRespawn()
        {
            base.OnRespawn();
        }
    }
}