using System;
using System.Collections.Generic;
using FSMHelper;
using SkaterXL.Core;
using UnityEngine;
using XXLMod3.Controller;
using XXLMod3.Core;

namespace XXLMod3.PlayerStates
{
    public class Custom_Braking : PlayerState_OnBoard
    {
        private bool _colliding;

        private float _comHeight = 1.04196f;

        public override void SetupDefinition(ref FSMStateType stateType, ref List<Type> children)
        {
            stateType = FSMStateType.Type_OR;
        }

        public Custom_Braking()
        {
        }

        public Custom_Braking(float p_comHeight)
        {
            _comHeight = p_comHeight;
        }

        public override void Enter()
        {
            PlayerController.Instance.currentStateEnum = PlayerController.CurrentState.Braking;
            XXLController.CurrentState = CurrentState.Braking;
            EventManager.Instance.OnBraking();
            PlayerController.Instance.ToggleFlipColliders(false);
            PlayerController.Instance.SetBoardPhysicsMaterial(PlayerController.FrictionType.Brake);
            PlayerController.Instance.CrossFadeAnimation("Braking", 0.4f);
        }

        public override void Update()
        {
            base.Update();
            _comHeight = Mathf.Lerp(_comHeight, 1.04196f, Time.deltaTime);
            SoundManager.Instance.SetRollingVolumeFromRPS(PlayerController.Instance.GetSurfaceTag(PlayerController.Instance.boardController.GetSurfaceTagString()), PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            PlayerController.Instance.comController.UpdateCOM(_comHeight, 0);
            PlayerController.Instance.boardController.SetBoardControllerUpVector(PlayerController.Instance.skaterController.skaterTransform.up);
            PlayerController.Instance.ApplyFriction();
            PlayerController.Instance.SetRotationTarget();
            PlayerController.Instance.SkaterRotation(true, false);
            PlayerController.Instance.boardController.ApplyOnBoardMaxRoll(_colliding, 60f);
            PlayerController.Instance.boardController.DoBoardLean();
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

        public override void Exit()
        {
            PlayerController.Instance.AnimSetBraking(false);
            PlayerController.Instance.SetBoardPhysicsMaterial(PlayerController.FrictionType.Default);
        }

        public override void OnStickUpdate(StickInput p_leftStick, StickInput p_rightStick)
        {
            if (SettingsManager.Instance.stance == Stance.Regular)
            {
                PlayerController.Instance.SetLeftIKOffset(0f, 0f, 0f, p_leftStick.IsPopStick, false, false);
                PlayerController.Instance.SetRightIKOffset(0f, 0f, 0f, p_rightStick.IsPopStick, false, false);
                return;
            }
            PlayerController.Instance.SetLeftIKOffset(0f, 0f, 0f, p_leftStick.IsPopStick, false, false);
            PlayerController.Instance.SetRightIKOffset(0f, 0f, 0f, p_rightStick.IsPopStick, false, false);
        }

        public override bool IsInBrakingState()
        {
            return true;
        }

        public override void OnBrakeHeld()
        {
        }

        public override void OnBrakeReleased()
        {
            base.DoTransition(typeof(Custom_Riding), null);
        }

        public override bool IsOnGroundState()
        {
            return true;
        }

        public override void OnRespawn()
        {
            base.OnRespawn();
        }
    }
}
