using FSMHelper;
using System.Collections.Generic;
using UnityEngine;
using XXLMod3.Controller;

namespace XXLMod3.PlayerStates
{
    public class Custom_Primo : PlayerState_OnBoard
    {
        private Vector2 _leftStick = Vector2.zero;
        private Vector2 _rightStick = Vector2.zero;

        private PlayerController.SetupDir _setupDir;

        private bool IsRightSide;

        bool IsExitingToSetup;

        public Custom_Primo()
        {
        }

        public override void SetupDefinition(ref FSMStateType stateType, ref List<System.Type> children)
        {
            stateType = FSMStateType.Type_OR;
        }

        public override void Enter()
        {
            XXLController.CurrentState = Core.CurrentState.Primo;
            foreach (Collider collider in PlayerController.Instance.boardController.boardTransform.GetComponentsInChildren<Collider>())
            {
                collider.material = XXLController.PrimoPhysicsMaterial;
            }
            XXLController.PrimoPhysicsMaterial.dynamicFriction = Main.settings.PrimoFriction;
            PlayerController.Instance.animationController.ScaleAnimSpeed(0.5f);
            PlayerController.Instance.CrossFadeAnimation("Grinds", 0.2f);
            PlayerController.Instance.SetTurnMultiplier(10f);
            PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
            //if (Vector3.Angle(Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardTransform.right, PlayerController.Instance.skaterController.skaterTargetTransform.forward), PlayerController.Instance.skaterController.skaterTargetTransform.up) > 170f)
            //{
            //    IsRightSide = true;
            //}
            //else if (Vector3.Angle(Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardTransform.right, PlayerController.Instance.skaterController.skaterTargetTransform.forward), PlayerController.Instance.skaterController.skaterTargetTransform.up) < 10f)
            //{
            //    IsRightSide = false;
            //}
            if (PlayerController.Instance.movementMaster != PlayerController.MovementMaster.Board)
            {
                PlayerController.Instance.movementMaster = PlayerController.MovementMaster.Board;
            }
            XXLController.Instance.ActivateSlowMotion(Main.settings.SlowMotionPrimos, Main.settings.SlowMotionPrimoSpeed);
        }

        public override void Exit()
        {
            XXLController.Instance.ResetTime(Main.settings.SlowMotionPrimos);
            if (!IsExitingToSetup)
            {
                PlayerController.Instance.SetBoardPhysicsMaterial(PlayerController.FrictionType.Default);
                SoundManager.Instance.StopPowerslideSound(1, PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude);
            }
        }

        public override void Update()
        {
            base.Update();
            PlayerController.Instance.AnimSetGrindBlend(0f, 0f);
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
            Vector3 vector = SkaterXL.Core.Mathd.LocalAngularVelocity(PlayerController.Instance.skaterController.skaterRigidbody);
            PlayerController.Instance.comController.UpdateCOM(1.5f, 1);
            PlayerController.Instance.SnapRotation();
            PlayerController.Instance.SetRotationTarget();
            PlayerController.Instance.SkaterRotation(true, false);
        }

        public override void OnBrakeHeld()
        {
            DoTransition(typeof(Custom_Riding), null);
        }

        public override void OnBrakePressed()
        {
            DoTransition(typeof(Custom_Riding), null);
        }

        public override void OnStickUpdate(StickInput p_leftStick, StickInput p_rightStick)
        {
            if (PlayerController.Instance.IsExitingPinState || PlayerController.Instance.respawn.respawning)
            {
                return;
            }
            _leftStick = new Vector2(p_leftStick.ToeAxis, p_leftStick.ForwardDir);
            _rightStick = new Vector2(p_rightStick.ToeAxis, p_rightStick.ForwardDir);
            SetupCheck(p_leftStick, p_rightStick);
        }

        private void SetupCheck(StickInput p_leftStick, StickInput p_rightStick)
        {
            if (p_leftStick.SetupDir > 0.8f || (new Vector2(p_leftStick.ToeAxis, p_leftStick.SetupDir).magnitude > 0.8f && p_leftStick.SetupDir > 0.325f))
            {
                IsExitingToSetup = true;
                PlayerController.Instance.AnimSetNollie(PlayerController.Instance.GetNollie(false));
                PlayerController.Instance.SetupDirection(p_leftStick, ref _setupDir);
                object[] args = new object[]
                {
            p_leftStick,
            p_rightStick,
            p_rightStick.ForwardDir > 0.2f,
            _setupDir
                };
                base.DoTransition(typeof(Custom_PrimoSetup), args);
                return;
            }
            if (p_rightStick.AugmentedSetupDir > 0.8f || (new Vector2(p_rightStick.ToeAxis, p_rightStick.SetupDir).magnitude > 0.8f && p_rightStick.SetupDir > 0.325f))
            {
                IsExitingToSetup = true;
                PlayerController.Instance.AnimSetNollie(PlayerController.Instance.GetNollie(true));
                PlayerController.Instance.SetupDirection(p_rightStick, ref _setupDir);
                object[] args2 = new object[]
                {
            p_rightStick,
            p_leftStick,
            p_leftStick.ForwardDir > 0.2f,
            _setupDir
                };
                base.DoTransition(typeof(Custom_PrimoSetup), args2);
                return;
            }
            PlayerController.Instance.AnimSetupTransition(false);
        }
    }
}