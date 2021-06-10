using System;
using System.Collections.Generic;
using FSMHelper;
using RootMotion.Dynamics;
using UnityEngine;
using XXLMod3.Controller;
using XXLMod3.Core;

namespace XXLMod3.PlayerStates
{
    public class Custom_Bailed : PlayerState_OffBoard
    {
        private bool _isExiting;
        private bool _isGrounded;

        private float _fallBlend;
        private float _fallTarget;
        private float _hipForwardAngle;
        private float _hipForwardLerp;
        private float _hipUp;
        private float _hipUpLerp;
        private float _hipXAngle;
        private float _hipYAngle;
        private float _hipXLerp;
        private float _hipYLerp;

        private int _frames;

        private RaycastHit[] _rayCasts = new RaycastHit[1];
        private Rigidbody _hips;

        public override void SetupDefinition(ref FSMStateType stateType, ref List<Type> children)
        {
            stateType = FSMStateType.Type_OR;
        }

        public override void Enter()
        {
            PlayerController.Instance.currentStateEnum = PlayerController.CurrentState.Bailed;
            XXLController.CurrentState = CurrentState.Bailed;
            XXLController.Instance.FlipDetected = false;
            if (Main.settings.BailRespawnAt)
            {
                Utils.InvokeMethod(PlayerController.Instance.respawn, "SetSpawnPos");
            }
            if (Main.settings.BetterBails)
            {
                XXLController.Instance.SetMuscleWeight();
            }
            PlayerController.Instance.respawn.puppetMaster.DisableTeleport();
            _hips = PlayerController.Instance.respawn.puppetMaster.muscles[0].rigidbody;
            EventManager.Instance.OnBail();
            PlayerController.Instance.respawn.puppetMaster.Kill();
            PlayerController.Instance.ToggleFlipColliders(false);
            GroundLogic();
            PlayerController.Instance.SetBoardPhysicsMaterial(PlayerController.FrictionType.Bail);
            PlayerController.Instance.RagdollLayerChange(true);
            _fallTarget = 1f;
            _fallBlend = 1f;
            PlayerController.Instance.animationController.SetValue("FallBlend", _fallBlend);
            PlayerController.Instance.skaterController.rightFootCollider.isTrigger = false;
            PlayerController.Instance.skaterController.leftFootCollider.isTrigger = false;
            PlayerController.Instance.SetKneeIKTargetWeight(0f);
            PlayerController.Instance.DoBailDelay();
            PlayerController.Instance.skaterController.skaterRigidbody.useGravity = true;
            _hips.velocity = PlayerController.Instance.skaterController.skaterRigidbody.velocity;
            PlayerController.Instance.respawn.behaviourPuppet.defaults.minMappingWeight = 1f;
            PlayerController.Instance.respawn.behaviourPuppet.masterProps.normalMode = BehaviourPuppet.NormalMode.Active;
            InitializeBailAnimInfo();
            XXLController.Instance.ActivateSlowMotion(Main.settings.SlowMotionBails, Main.settings.SlowMotionBailSpeed);
        }

        public override void Exit()
        {
            XXLController.Instance.ResetTime(Main.settings.SlowMotionBails);
            _isExiting = true;
            if (Main.settings.BetterBails)
            {
                XXLController.Instance.ResetMuscleWeight();
            }
            PlayerController.Instance.RagdollLayerChange(false);
            PlayerController.Instance.SetKneeBendWeightManually(1f);
            PlayerController.Instance.respawn.puppetMaster.pinWeight = 1f;
            PlayerController.Instance.respawn.puppetMaster.muscleWeight = 1f;
            PlayerController.Instance.respawn.behaviourPuppet.defaults.minMappingWeight = 0f;
            PlayerController.Instance.respawn.behaviourPuppet.masterProps.normalMode = BehaviourPuppet.NormalMode.Unmapped;
            PlayerController.Instance.SetBoardPhysicsMaterial(PlayerController.FrictionType.Default);
        }

        public override void Update()
        {
            if (_isExiting)
            {
                return;
            }
            base.Update();
            if (Main.settings.BailControls)
            {
                if (InputController.Instance.player.GetButton("RB"))
                {
                    PlayerController.Instance.respawn.behaviourPuppet.puppetMaster.muscles[2].rigidbody.AddForce(0, Main.settings.BailUpForce, 0f, ForceMode.Impulse);
                }
                if (InputController.Instance.player.GetButton("LB"))
                {
                    PlayerController.Instance.respawn.behaviourPuppet.puppetMaster.muscles[12].rigidbody.AddForce(0, -Main.settings.BailDownForce, 0f, ForceMode.Impulse);
                    PlayerController.Instance.respawn.behaviourPuppet.puppetMaster.muscles[15].rigidbody.AddForce(0, -Main.settings.BailDownForce, 0f, ForceMode.Impulse);
                }
                if(InputController.Instance.player.GetAxis("LT") > 0.1f)
                {
                    PlayerController.Instance.respawn.behaviourPuppet.puppetMaster.muscles[6].rigidbody.AddForce(0, Main.settings.BailArmForce, 0f, ForceMode.Impulse);
                }
                if (InputController.Instance.player.GetAxis("RT") > 0.1f)
                {
                    PlayerController.Instance.respawn.behaviourPuppet.puppetMaster.muscles[9].rigidbody.AddForce(0, Main.settings.BailArmForce, 0f, ForceMode.Impulse);
                }
                if (InputController.Instance.player.GetButton("Left Stick Button"))
                {
                    PlayerController.Instance.respawn.behaviourPuppet.puppetMaster.muscles[12].rigidbody.AddForce(0, Main.settings.BailLegForce, 0f, ForceMode.Impulse);
                }
                if (InputController.Instance.player.GetButton("Right Stick Button"))
                {
                    PlayerController.Instance.respawn.behaviourPuppet.puppetMaster.muscles[15].rigidbody.AddForce(0, Main.settings.BailLegForce, 0f, ForceMode.Impulse);
                }
            }
            PlayerController.Instance.LerpKneeIkWeight();
            UpdateBailAnimInfo();
            if (Physics.RaycastNonAlloc(_hips.position, -Vector3.up, _rayCasts, 1f, PlayerController.Instance.boardController._layers) > 0)
            {
                _fallTarget = 0f;
                PlayerController.Instance.respawn.puppetMaster.pinWeight = 0.01f;
                PlayerController.Instance.respawn.puppetMaster.muscleWeight = 0.1f;
            }
            else
            {
                _fallTarget = 1f;
            }
            _fallBlend = Mathf.Lerp(_fallBlend, _fallTarget, Time.fixedDeltaTime * 10f);
            PlayerController.Instance.animationController.SetValue("FallBlend", _fallBlend);
        }

        private void InitializeBailAnimInfo()
        {
            Vector3 velocity = _hips.velocity;
            velocity.y = 0f;
            Vector3 vector = Quaternion.AngleAxis(90f, Vector3.up) * velocity;
            Vector3 from = Vector3.ProjectOnPlane(_hips.transform.up + -_hips.transform.right, Vector3.up);
            Vector3 from2 = Vector3.ProjectOnPlane(_hips.transform.up, vector);
            _hipYAngle = Vector3.SignedAngle(from, velocity, Vector3.up);
            _hipXAngle = Vector3.SignedAngle(from2, velocity, vector);
            _hipUp = Vector3.Angle(-_hips.transform.right, Vector3.up);
            _hipForwardAngle = Vector3.SignedAngle(_hips.transform.up, Vector3.up, -_hips.transform.right);
            _hipUpLerp = _hipUp;
            _hipForwardLerp = _hipForwardAngle;
            _hipXLerp = _hipXAngle;
            _hipYLerp = _hipYAngle;
            PlayerController.Instance.animationController.SetValue("hipX", _hipXAngle);
            PlayerController.Instance.animationController.SetValue("hipY", _hipYAngle);
            PlayerController.Instance.animationController.SetValue("hipUp", _hipUp);
            PlayerController.Instance.animationController.SetValue("hipForward", _hipForwardAngle);
            PlayerController.Instance.animationController.SetValue("bailVel", Vector3.ProjectOnPlane(_hips.velocity, Vector3.up).magnitude);
        }

        private void UpdateBailAnimInfo()
        {
            Vector3 velocity = _hips.velocity;
            velocity.y = 0f;
            Vector3 vector = Quaternion.AngleAxis(90f, Vector3.up) * velocity;
            Vector3 from = Vector3.ProjectOnPlane(_hips.transform.up + -_hips.transform.right, Vector3.up);
            Vector3 from2 = Vector3.ProjectOnPlane(_hips.transform.up, vector);
            _hipYAngle = Vector3.SignedAngle(from, velocity, Vector3.up);
            _hipXAngle = Vector3.SignedAngle(from2, velocity, vector);
            _hipUp = Vector3.Angle(-_hips.transform.right, Vector3.up);
            _hipForwardAngle = Vector3.SignedAngle(_hips.transform.up, Vector3.up, -_hips.transform.right);
            _hipUpLerp = Mathf.MoveTowards(_hipUpLerp, _hipUp, Time.deltaTime * 50f);
            _hipForwardLerp = Mathf.MoveTowards(_hipForwardLerp, _hipForwardAngle, Time.deltaTime * 50f);
            _hipXLerp = Mathf.MoveTowards(_hipXLerp, _hipXAngle, Time.deltaTime * 50f);
            _hipYLerp = Mathf.MoveTowards(_hipYLerp, _hipYAngle, Time.deltaTime * 50f);
            PlayerController.Instance.animationController.SetValue("hipX", _hipXLerp);
            PlayerController.Instance.animationController.SetValue("hipY", _hipYLerp);
            PlayerController.Instance.animationController.SetValue("hipUp", _hipUpLerp);
            PlayerController.Instance.animationController.SetValue("hipForward", _hipForwardAngle);
            PlayerController.Instance.animationController.SetValue("bailVel", Vector3.ProjectOnPlane(_hips.velocity, Vector3.up).magnitude);
        }

        public override void FixedUpdate()
        {
            if (_isExiting)
            {
                return;
            }
            base.FixedUpdate();
            GroundLogic();
        }

        private void GroundLogic()
        {
            if (PlayerController.Instance.IsGrounded())
            {
                if (!_isGrounded)
                {
                    _isGrounded = true;
                    PlayerController.Instance.SetBoardPhysicsMaterial(PlayerController.FrictionType.Default);
                }
                Vector3 vector = PlayerController.Instance.boardController.boardTransform.InverseTransformDirection(PlayerController.Instance.boardController.boardRigidbody.angularVelocity);
                vector.y = Mathf.Lerp(vector.y, 0f, Time.deltaTime * 5f);
                PlayerController.Instance.boardController.boardRigidbody.angularVelocity = PlayerController.Instance.boardController.boardTransform.TransformDirection(vector);
                PlayerController.Instance.ApplyFriction();
                SoundManager.Instance.SetRollingVolumeFromRPS(PlayerController.Instance.GetSurfaceTag(PlayerController.Instance.boardController.GetSurfaceTagString()), PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude);
                return;
            }
            if (_isGrounded)
            {
                _isGrounded = false;
                PlayerController.Instance.SetBoardPhysicsMaterial(PlayerController.FrictionType.Bail);
            }
            SoundManager.Instance.SetRollingVolumeFromRPS(PlayerController.Instance.GetSurfaceTag(PlayerController.Instance.boardController.GetSurfaceTagString()), PlayerController.Instance.boardController._rollSoundSpeed);
        }

        public override void OnCollisionEnterEvent(Vector3 _impulse, bool _isBoard, Collision c)
        {
            SoundManager.Instance.PlayBoardHit(_impulse.magnitude);
        }

        public override bool IsInBailState()
        {
            return true;
        }

        public override void OnRespawn()
        {
            base.OnRespawn();
        }
    }
}