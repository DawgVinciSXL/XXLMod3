using Dreamteck.Splines;
using FSMHelper;
using SkaterXL.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using XXLMod3.Controller;
using XXLMod3.Core;

namespace XXLMod3.PlayerStates
{
    public class Custom_ExitCoping : PlayerState_OnBoard
    {
        private bool _backTruckOverTransition;
        private bool _frontTruckOverTransition;
        private bool _isBoardSlide;
        private bool _grindSoundStopped;

        private double _maxPercentage;
        private double _minPercentage;
        private double percent;

        private float _backTruckAboveCoping;
        private float _backTruckOnExitSide;
        private float _boardFwdDotGrindDir;
        private float _boardFwdDotLandTangent;
        private float _boardFwdExitDirAngle;
        private float _boardSplinePointDiffY;
        private float _boardUpDotLandNormal;
        private float _com;
        private float _currentBoardCOM;
        private float _exitToAirTimer;
        private float _frontTruckAboveCoping;
        private float _frontTruckOnExitSide;
        private float _initialHeight;
        private float _initialVelSquared;
        private float _leftForwardDir;
        private float _leftToeAxis;
        private float _leftTrigger;
        private float _playerTurn;
        private float _rightForwardDir;
        private float _rightToeAxis;
        private float _rightTrigger;
        private float _sideFactor;
        private float _skaterUpLandNormalAngle;
        private float targetGrindAnimY;
        private float _timer;
        private float _turnSpeed;

        private int transitionLayerMask;

        private Quaternion _skaterRot = Quaternion.identity;
        private Quaternion _targetBoardRot = Quaternion.identity;
        private Quaternion _targetSkaterRot = Quaternion.identity;

        private RaycastHit[] _rayCasts = new RaycastHit[1];

        private SplineComputer _spline;
        private SplineResult _splineResult;

        private TriggerManager.SideEnteredGrind _side;

        private Vector3 _exitDirection;
        private Vector3 _grindContact;
        private Vector3 _grindContactOffset = Vector3.zero;
        private Vector3 _grindDir;
        private Vector3 _grindUp;
        private Vector3 _initialVel;
        private Vector3 _landNormal;
        private Vector3 _landTangent;
        private Vector3 _lastSkaterPosition;

        public TweakValues tweakValues = TweakValues.Tuned;

        [Serializable]
        public struct TweakValues
        {
            public static TweakValues Tuned
            {
                get
                {
                    return new TweakValues
                    {
                        exitTime = 1.2f,
                        initialCOM = 1f,
                        dropInSpeed = 0.6f,
                        skaterPIDRotationSpeed = 0.4f,
                        skaterPIDLookDirectionSlerpSpeed = 60f,
                        boardCOMSpeed = 10f,
                        exitToAirDelay = 0.25f,
                        popOutVelocity = 0.2f,
                        popOutTorqueAngleFactor = 3f,
                        popOutTorqueOffExitFactor = 1f,
                        popOutOffset = 0.1f,
                        popOutVelocityFalloff = 1f,
                        popOutTorque = 15000f,
                        maxTorqueFactor = 2f,
                        popOutTorqueFalloff = 0.0001f,
                        pushBoardToTransitionForce = 100f,
                        pushBoardToTransitionVelocity = 1f,
                        skaterRotVelocityInfluence = 0.2f,
                        boardOverCopingAngle = 40f,
                        grindAnimSpeed = 5f,
                        turnSpeed = 2f,
                        turnSpeedGain = 1000f
                    };
                }
            }

            public override string ToString()
            {
                return string.Concat(new string[]
                {
                "\r\n            exitTime = ",
                this.exitTime.ToString("0.######", CultureInfo.InvariantCulture),
                "f,\r\n            initialCOM = ",
                this.initialCOM.ToString("0.######", CultureInfo.InvariantCulture),
                "f,\r\n\r\n            dropInSpeed = ",
                this.dropInSpeed.ToString("0.######", CultureInfo.InvariantCulture),
                "f,\r\n            skaterPIDRotationSpeed = ",
                this.skaterPIDRotationSpeed.ToString("0.######", CultureInfo.InvariantCulture),
                "f,\r\n            skaterPIDLookDirectionSlerpSpeed = ",
                this.skaterPIDLookDirectionSlerpSpeed.ToString("0.######", CultureInfo.InvariantCulture),
                "f,\r\n            boardCOMSpeed = ",
                this.boardCOMSpeed.ToString("0.######", CultureInfo.InvariantCulture),
                "f,\r\n            exitToAirDelay = ",
                this.exitToAirDelay.ToString("0.######", CultureInfo.InvariantCulture),
                "f,\r\n            popOutVelocity = ",
                this.popOutVelocity.ToString("0.######", CultureInfo.InvariantCulture),
                "f,\r\n            popOutTorqueAngleFactor = ",
                this.popOutTorqueAngleFactor.ToString("0.######", CultureInfo.InvariantCulture),
                "f,\r\n            popOutTorqueOffExitFactor = ",
                this.popOutTorqueOffExitFactor.ToString("0.######", CultureInfo.InvariantCulture),
                "f,\r\n            popOutOffset = ",
                this.popOutOffset.ToString("0.######", CultureInfo.InvariantCulture),
                "f,\r\n            popOutVelocityFalloff = ",
                this.popOutVelocityFalloff.ToString("0.######", CultureInfo.InvariantCulture),
                "f,\r\n            popOutTorque = ",
                this.popOutTorque.ToString("0.######", CultureInfo.InvariantCulture),
                "f,\r\n            maxTorqueFactor = ",
                this.maxTorqueFactor.ToString("0.######", CultureInfo.InvariantCulture),
                "f,\r\n            popOutTorqueFalloff = ",
                this.popOutTorqueFalloff.ToString("0.######", CultureInfo.InvariantCulture),
                "f,\r\n            pushBoardToTransitionForce = ",
                this.pushBoardToTransitionForce.ToString("0.######", CultureInfo.InvariantCulture),
                "f,\r\n            pushBoardToTransitionVelocity = ",
                this.pushBoardToTransitionVelocity.ToString("0.######", CultureInfo.InvariantCulture),
                "f,\r\n            skaterRotVelocityInfluence = ",
                this.skaterRotVelocityInfluence.ToString("0.######", CultureInfo.InvariantCulture),
                "f,\r\n            boardOverCopingAngle = ",
                this.boardOverCopingAngle.ToString("0.######", CultureInfo.InvariantCulture),
                "f,\r\n            grindAnimSpeed = ",
                this.grindAnimSpeed.ToString("0.######"),
                "f,\r\n\r\n            turnSpeed = ",
                this.turnSpeed.ToString("0.######"),
                "f,\r\n            turnSpeedGain = ",
                this.turnSpeedGain.ToString("0.######"),
                "f"
                });
            }

            public float boardCOMSpeed;
            public float boardOverCopingAngle;
            public float dropInSpeed;
            public float exitTime;
            public float exitToAirDelay;
            public float grindAnimSpeed;
            public float initialCOM;
            public float maxTorqueFactor;
            public float popOutOffset;
            public float popOutTorque;
            public float popOutTorqueAngleFactor;
            public float popOutTorqueFalloff;
            public float popOutTorqueOffExitFactor;
            public float popOutVelocity;
            public float popOutVelocityFalloff;
            public float pushBoardToTransitionForce;
            public float pushBoardToTransitionVelocity;
            public float skaterPIDLookDirectionSlerpSpeed;
            public float skaterPIDRotationSpeed;
            public float skaterRotVelocityInfluence;
            public float turnSpeed;
            public float turnSpeedGain;
        }

        private Transform backTruck
        {
            get
            {
                return PlayerController.Instance.boardController.backTruckCoM;
            }
        }

        private Transform frontTruck
        {
            get
            {
                return PlayerController.Instance.boardController.frontTruckCoM;
            }
        }

        private float _exitTime
        {
            get
            {
                return this.tweakValues.exitTime;
            }
        }

        private bool backTruckColliding
        {
            get
            {
                return PlayerController.Instance.boardController.triggerManager.backTruckCollision.isColliding;
            }
        }

        private bool frontTruckColliding
        {
            get
            {
                return PlayerController.Instance.boardController.triggerManager.frontTruckCollision.isColliding;
            }
        }

        private bool boardColliding
        {
            get
            {
                return PlayerController.Instance.boardController.triggerManager.boardCollision.isColliding;
            }
        }

        private bool BoardTrigger
        {
            get
            {
                return PlayerController.Instance.boardController.triggerManager.activeGrinds[0];
            }
        }

        private bool TailTrigger
        {
            get
            {
                return PlayerController.Instance.boardController.triggerManager.activeGrinds[1];
            }
        }

        private bool NoseTrigger
        {
            get
            {
                return PlayerController.Instance.boardController.triggerManager.activeGrinds[2];
            }
        }

        private bool BackTruckTrigger
        {
            get
            {
                return PlayerController.Instance.boardController.triggerManager.activeGrinds[3];
            }
        }

        private bool FrontTruckTrigger
        {
            get
            {
                return PlayerController.Instance.boardController.triggerManager.activeGrinds[4];
            }
        }

        public override void SetupDefinition(ref FSMStateType stateType, ref List<Type> children)
        {
            stateType = FSMStateType.Type_OR;
        }

        public Custom_ExitCoping()
        {
        }


        private float CalculatePotentialEnergy(float referenceHeight)
        {
            float num = PlayerController.Instance.comController.COMRigidbody.position.y - referenceHeight;
            return Physics.gravity.magnitude * num;
        }

        private void CalculateGrindVectors()
        {
            this.percent = this._spline.Project(PlayerController.Instance.GetBoardCenterOfMass(), 3, 0.0, 1.0);
            this._splineResult = this._spline.Evaluate(this.percent);
            this._grindContact = this._splineResult.position + this._grindContactOffset;
            this._grindDir = this._splineResult.direction;
            this._grindUp = this._splineResult.normal;
            this._exitDirection = this._sideFactor * Vector3.Cross(this._grindDir, Vector3.up).normalized;
        }

        private void CalculateDotProducts()
        {
            this._boardSplinePointDiffY = PlayerController.Instance.GetBoardCenterOfMass().y - this._grindContact.y;
            this._boardFwdDotLandTangent = Vector3.Dot(PlayerController.Instance.boardController.GetClosestBoardForward(), this._landTangent);
            this._boardFwdDotGrindDir = Vector3.Dot(PlayerController.Instance.boardController.GetClosestBoardForward(), this._grindDir);
            this._boardFwdExitDirAngle = Vector3.Angle(this._exitDirection, Vector3.ProjectOnPlane(PlayerController.Instance.boardController.GetClosestBoardForward(), this._grindUp));
            this._boardUpDotLandNormal = Vector3.Dot(PlayerController.Instance.boardController.boardTransform.up, this._landNormal);
            this._skaterUpLandNormalAngle = Vector3.Angle(PlayerController.Instance.skaterController.skaterTransform.up, this._landNormal);
            this._backTruckOnExitSide = Vector3.Dot(this._exitDirection, this.backTruck.position - this._grindContact);
            this._frontTruckOnExitSide = Vector3.Dot(this._exitDirection, this.frontTruck.position - this._grindContact);
            this._backTruckAboveCoping = Vector3.Dot(Vector3.up, this.backTruck.position - this._grindContact);
            this._frontTruckAboveCoping = Vector3.Dot(Vector3.up, this.frontTruck.position - this._grindContact);
            this._isBoardSlide = (Mathf.Abs(this._boardFwdDotGrindDir) < 0.5f && this.BoardTrigger && !this.backTruckColliding && !this.frontTruckColliding && !this.TailTrigger && !this.NoseTrigger);
        }

        public Custom_ExitCoping(SplineComputer p_spline, Vector3 p_grindDir, int p_side)
        {
            this.transitionLayerMask = LayerMask.GetMask(new string[]
            {
            "Default"
            });
            this._com = this.tweakValues.initialCOM;
            this._initialVel = PlayerController.Instance.boardController.boardRigidbody.velocity;
            this._spline = p_spline;
            this._grindDir = p_grindDir;
            this._side = (TriggerManager.SideEnteredGrind)p_side;
            switch (this._side)
            {
                case TriggerManager.SideEnteredGrind.Left:
                    this._sideFactor = 1f;
                    break;
                case TriggerManager.SideEnteredGrind.Right:
                    this._sideFactor = -1f;
                    break;
                case TriggerManager.SideEnteredGrind.Center:
                    this._sideFactor = 0f;
                    break;
            }
            if (this._spline == null)
            {
                return;
            }
            this._minPercentage = this._spline.Travel(0.0, 0.1f, Spline.Direction.Forward);
            this._maxPercentage = this._spline.Travel(1.0, 0.1f, Spline.Direction.Backward);
            this.CalculateGrindVectors();
            this.FindLandNormal();
            this.CalculateDotProducts();
        }

        public override void Enter()
        {
            PlayerController.Instance.currentStateEnum = PlayerController.CurrentState.ExitCoping;
            XXLController.CurrentState = CurrentState.ExitCoping;
            PlayerController.Instance.cameraController.NeedToSlowLerpCamera = true;
            if (!(this._spline == null))
            {
                if (Vector3.Angle(this._landNormal, Vector3.up) < 10f && PlayerController.Instance.boardController.Grounded)
                {
                    this.ExitCopingToRiding();
                }
                SoundManager.Instance.PlayGrindSound(PlayerController.Instance.boardController.isSliding ? 1 : PlayerController.Instance.boardController.GetGrindSoundInt(), Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardRigidbody.velocity, Vector3.up).magnitude);
                this._initialVelSquared = PlayerController.Instance.boardController.boardRigidbody.velocity.sqrMagnitude;
                this._initialHeight = PlayerController.Instance.comController.COMRigidbody.position.y;
                this._lastSkaterPosition = PlayerController.Instance.skaterController.skaterTransform.position;
                this.SetCenterOfMass();
                return;
            }
            if (PlayerController.Instance.boardController.Grounded)
            {
                this.ExitCopingToRiding();
                return;
            }
            this.ExitCopingToInAir();
        }

        public override void Exit()
        {
            PlayerController.Instance.cameraController.NeedToSlowLerpCamera = false;
            this.StopGrindSound();
            PlayerController.Instance.WasOnCoping = true;
            PlayerController.Instance.DisableWasOnCopingDelayed();
            this.SetBoardCenterOfMass(0.5f);
            base.Exit();
        }

        private void SetCenterOfMass()
        {
            Vector3 rhs = this.frontTruck.position - this.backTruck.position;
            float boardCenterOfMass = Mathf.Clamp01(Vector3.Dot(this._grindContact - this.backTruck.position, rhs) / Mathf.Pow(rhs.magnitude, 2f));
            this.SetBoardCenterOfMass(boardCenterOfMass);
        }

        private void SetBoardCenterOfMass(float backToFrontAxis)
        {
            this._currentBoardCOM = backToFrontAxis;
            Vector3 boardCenterOfMass = Vector3.Lerp(PlayerController.Instance.boardController.backTruckCoM.position, PlayerController.Instance.boardController.frontTruckCoM.position, Mathf.Clamp01(backToFrontAxis));
            this.SetBoardCenterOfMass(boardCenterOfMass);
        }

        private void SetBoardCenterOfMass(Vector3 _pos)
        {
            PlayerController.Instance.SetBoardCenterOfMass(PlayerController.Instance.boardController.boardTransform.InverseTransformPoint(_pos));
            PlayerController.Instance.SetBackTruckCenterOfMass(PlayerController.Instance.boardController.backTruckRigidbody.transform.InverseTransformPoint(_pos));
            PlayerController.Instance.SetFrontTruckCenterOfMass(PlayerController.Instance.boardController.frontTruckRigidbody.transform.InverseTransformPoint(_pos));
        }

        private void FindLandNormal()
        {
            Ray ray = new Ray(this._grindContact + this._exitDirection + Vector3.up * 0.15f, -this._exitDirection + Vector3.down * 0.4f);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit, 3f, this.transitionLayerMask))
            {
                Debug.DrawRay(ray.origin, ray.direction, Color.yellow, 5f, false);
                Debug.DrawRay(raycastHit.point, raycastHit.normal, Color.green, 10f, false);
                this._landNormal = raycastHit.normal;
                this._landTangent = Vector3.Cross(this._grindDir, this._landNormal) * this._sideFactor;
                if (Vector3.Angle(this._landNormal, Vector3.up) > 10f)
                {
                    Ray ray2 = new Ray(raycastHit.point, -this._landTangent);
                    float distance;
                    if (new Plane(this._grindUp, this._grindContact).Raycast(ray2, out distance))
                    {
                        Vector3 point = ray2.GetPoint(distance);
                        Vector3 lhs = point - this._grindContact;
                        if (Vector3.Dot(lhs, this._exitDirection) > 0f && lhs.magnitude < 0.5f)
                        {
                            this._grindContactOffset = Vector3.ProjectOnPlane(point - this._splineResult.position, this._grindDir);
                            this._grindContact = point;
                        }
                    }
                }
            }
            this._backTruckOverTransition = false;
            this._frontTruckOverTransition = false;
            Vector3 a = this._landNormal;
            if (Physics.Raycast(this.backTruck.position, -this._landNormal, out raycastHit, 10f, this.transitionLayerMask) && Vector3.Angle(raycastHit.normal, this._landNormal) < 20f && raycastHit.point.y < this._grindContact.y)
            {
                this._backTruckOverTransition = true;
                a = raycastHit.normal;
            }
            if (Physics.Raycast(this.frontTruck.position, -this._landNormal, out raycastHit, 10f, this.transitionLayerMask) && Vector3.Angle(raycastHit.normal, this._landNormal) < 20f && raycastHit.point.y < this._grindContact.y)
            {
                this._frontTruckOverTransition = true;
                this._landNormal = Vector3.Slerp(a, raycastHit.normal, 0.5f);
            }
        }

        public override void Update()
        {
            SoundManager.Instance.SetRollingVolumeFromRPS(PlayerController.Instance.GetSurfaceTag(PlayerController.Instance.boardController.GetSurfaceTagString()), PlayerController.Instance.IsGrounded() ? PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude : PlayerController.Instance.boardController._rollSoundSpeed);
            if (this.backTruckColliding || this.frontTruckColliding || this.boardColliding)
            {
                this._exitToAirTimer = 0f;
            }
            else if (PlayerController.Instance.boardController.Grounded)
            {
                this._exitToAirTimer = 0f;
            }
            else
            {
                this._exitToAirTimer += Time.fixedDeltaTime;
            }
            if (!this._spline.isClosed && (this.percent < this._minPercentage || this.percent > this._maxPercentage))
            {
                this.ExitCopingToInAir();
            }
            if (this._turnSpeed < this.tweakValues.turnSpeed)
            {
                this._turnSpeed = Mathf.Clamp(this._turnSpeed + Time.deltaTime * this.tweakValues.turnSpeedGain, 0f, this.tweakValues.turnSpeed);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this._timer += Time.fixedDeltaTime;
            PlayerController.Instance.ScalePlayerCollider();
            this.SetGrindAnim();
            this.CalculateGrindVectors();
            this.FindLandNormal();
            this.CalculateDotProducts();
            Debug.DrawRay(this._grindContact, this._grindUp, Color.white, 10f, false);
            Debug.DrawRay(this._grindContact, this._landNormal, Color.white, 10f, false);
            this.PopOutTorque();
            this.PIDPosition();
            this.PIDRotation();
            this.AdjustCOM();
            this.CheckForExit();
        }

        private void SetGrindAnim()
        {
            if (this._backTruckAboveCoping < 0f && this._frontTruckAboveCoping < 0f)
            {
                this.targetGrindAnimY = 0f;
            }
            else if (this._boardFwdExitDirAngle < 90f)
            {
                this.targetGrindAnimY = 2f;
            }
            else
            {
                this.targetGrindAnimY = 2f;
            }
            Vector2 vector = PlayerController.Instance.boardController.triggerManager.grindDetection.LerpGrindAnimationY(this.targetGrindAnimY, this.tweakValues.grindAnimSpeed);
            PlayerController.Instance.AnimSetGrindBlend(vector.x, vector.y);
        }

        private void CheckForExit()
        {
            if (this._timer > this._exitTime)
            {
                if (PlayerController.Instance.boardController.TwoDown && this._boardUpDotLandNormal > 0.5f && (this._backTruckOnExitSide > 0f || this._backTruckAboveCoping < 0f) && (this._frontTruckOnExitSide > 0f || this._frontTruckAboveCoping < 0f))
                {
                    this.ExitCopingToImpact();
                }
                PlayerController.Instance.ForceBail();
            }
            if (!this.backTruckColliding && !this.frontTruckColliding && !this.boardColliding)
            {
                this.StopGrindSound();
                if (!PlayerController.Instance.boardController.Grounded && this._exitToAirTimer > this.tweakValues.exitToAirDelay)
                {
                    this.ExitCopingToInAir();
                }
                if (this._backTruckAboveCoping < 0f && this._frontTruckAboveCoping < 0f)
                {
                    if (PlayerController.Instance.boardController.Grounded)
                    {
                        this._landNormal = PlayerController.Instance.GetGroundNormal();
                        this._landTangent = Vector3.Cross(this._grindDir, this._landNormal) * (float)((this._side == TriggerManager.SideEnteredGrind.Right) ? -1 : 1);
                    }
                    if (PlayerController.Instance.boardController.AllDown)
                    {
                        this.ExitCopingToRiding();
                        return;
                    }
                    if (PlayerController.Instance.boardController.TwoDown && this._boardUpDotLandNormal > 0.8f)
                    {
                        this.ExitCopingToImpact();
                    }
                }
            }
        }

        private void AdjustCOM()
        {
            if (!PlayerController.Instance.boardController.AllDown)
            {
                float num = Vector3.Dot(PlayerController.Instance.skaterController.skaterTransform.position - this._lastSkaterPosition, PlayerController.Instance.skaterController.skaterTransform.up);
                this._com = Mathf.Clamp(this._com - num, 0.7f, 1.1f);
                this._lastSkaterPosition = PlayerController.Instance.skaterController.skaterTransform.position;
            }
            else
            {
                this._com = Mathf.MoveTowards(this._com, 1.1f, 15f * Time.fixedDeltaTime);
            }
            PlayerController.Instance.comController.UpdateCOM(this._com, 0);
            if (!Mathf.Approximately(this._currentBoardCOM, 0.5f) && ((this._backTruckOnExitSide > 0f && this._frontTruckOnExitSide > 0f) || (this._backTruckAboveCoping < 0f && this._frontTruckAboveCoping < 0f)))
            {
                this.SetBoardCenterOfMass(0.5f);
            }
        }

        private void PIDRotation()
        {
            Vector3 vector = Quaternion.AngleAxis(this._playerTurn * this._turnSpeed, PlayerController.Instance.skaterController.skaterTransform.up) * PlayerController.Instance.skaterController.skaterTransform.forward;
            vector = Vector3.Slerp(vector, Vector3.ProjectOnPlane(PlayerController.Instance.boardController.GetClosestBoardForward(), PlayerController.Instance.skaterController.skaterTransform.up), this.tweakValues.skaterPIDLookDirectionSlerpSpeed * Time.fixedDeltaTime);
            Quaternion p_rot = Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.ProjectOnPlane(PlayerController.Instance.skaterController.skaterTransform.up, this._grindDir), this._landNormal, this._grindDir) * this.tweakValues.dropInSpeed, this._grindDir) * Quaternion.LookRotation(vector, PlayerController.Instance.skaterController.skaterTransform.up);
            PlayerController.Instance.SkaterRotation(p_rot, this.tweakValues.skaterPIDRotationSpeed);
            PlayerController.Instance.SetRotationTarget();
            PlayerController.Instance.boardController.SnapRotation();
        }

        private void PopOutTorque()
        {
            if (this._skaterUpLandNormalAngle > 45f && this._isBoardSlide)
            {
                return;
            }
            float num = 0.1f;
            float num2 = Mathf.Min(this._backTruckOnExitSide, this._frontTruckOnExitSide);
            float num3 = Mathf.Max(this._backTruckAboveCoping, this._backTruckAboveCoping);
            float num4 = Mathf.Clamp(-num2, 0.01f, num);
            float num5 = this.tweakValues.popOutTorqueOffExitFactor * num4 / num;
            if ((num2 < -0.1f || num3 < -0.1f) && !((this._boardFwdExitDirAngle < 90f) ? this.TailTrigger : this.NoseTrigger))
            {
                Vector3 vector = this._landNormal;
                if (!this._backTruckOverTransition && !this._frontTruckOverTransition)
                {
                    vector = Quaternion.AngleAxis(this.tweakValues.boardOverCopingAngle, this._grindDir * this._sideFactor) * vector;
                }
                float num6 = this.tweakValues.popOutTorqueAngleFactor * Mathf.Abs(Vector3.SignedAngle(PlayerController.Instance.boardController.boardTransform.up, vector, this._grindDir) / 90f);
                num5 += num6;
            }
            num5 *= Mathf.Pow(Mathf.Clamp((90f - this._skaterUpLandNormalAngle) / 90f, 0.1f, 0.6f) / 0.6f, 2f);
            num5 = Mathf.Clamp(num5, 0f, this.tweakValues.maxTorqueFactor);
            if (Mathf.Abs(num5) > 0.001f)
            {
                Vector3 a = num5 * Vector3.Project(this._grindDir.normalized, PlayerController.Instance.boardController.boardTransform.right) * this.tweakValues.popOutTorque * this._sideFactor;
                PlayerController.Instance.boardController.boardRigidbody.AddTorque(a * Time.fixedDeltaTime);
            }
        }

        private void PIDPosition()
        {
            if (this._backTruckOverTransition && this._frontTruckOverTransition && PlayerController.Instance.IsGrounded() && Vector3.Dot(PlayerController.Instance.boardController.boardRigidbody.velocity, this._landNormal) < 0f)
            {
                return;
            }
            float num = Mathf.Max(this._backTruckAboveCoping, this._backTruckAboveCoping);
            Vector3 posError;
            if (PlayerController.Instance.IsGrounded() && this._backTruckAboveCoping < 0f && this._frontTruckAboveCoping < 0f && this._backTruckOverTransition && this._frontTruckOverTransition && Vector3.Dot(PlayerController.Instance.GetGroundNormal(), this._landNormal) > 0.9f)
            {
                posError = (Vector3.ProjectOnPlane(PlayerController.Instance.boardController.boardRigidbody.velocity.normalized, this._landNormal) - this._landNormal * this.tweakValues.pushBoardToTransitionVelocity + Physics.gravity * Time.fixedDeltaTime) * Time.fixedDeltaTime;
            }
            else
            {
                Vector3 b = PlayerController.Instance.boardController.boardTransform.position - PlayerController.Instance.boardController.boardTransform.up * 0.04f;
                float num2 = Vector3.Dot(this._grindContact - b, this._exitDirection);
                Vector3 vector = (Vector3.Project(PlayerController.Instance.boardController.boardRigidbody.velocity, Physics.gravity) + Physics.gravity * Time.fixedDeltaTime) * Time.fixedDeltaTime;
                if (this._isBoardSlide)
                {
                    vector *= Mathf.Clamp(90f - this._skaterUpLandNormalAngle / 90f, this._timer / 0.8f, 1f);
                }
                posError = (num2 + Mathf.Clamp((num + 0.05f) / 0.3f, (float)(this._backTruckOverTransition ? 0 : 1) + (this._frontTruckOverTransition ? 0f : 1f) * 0.25f, 1f) * this.tweakValues.popOutOffset) * this._exitDirection + vector;
            }
            PlayerController.Instance.boardController.PIDPosition(PlayerController.Instance.boardController.boardRigidbody.velocity, posError, Mathf.Clamp01(100f - this._boardUpDotLandNormal / 100f) * this.tweakValues.popOutVelocity);
        }

        private void StopGrindSound()
        {
            if (!this._grindSoundStopped)
            {
                SoundManager.Instance.StopGrindSound((!SkaterXL.Core.Mathd.IsInfinityOrNaN(PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude)) ? PlayerController.Instance.boardController.boardRigidbody.velocity.magnitude : 0f);
                this._grindSoundStopped = true;
            }
        }

        private void ExitCopingToImpact()
        {
            PlayerController.Instance.Impact();
            PlayerController.Instance.AnimGrindTransition(false);
            if (!PlayerController.Instance.IsRespawning)
            {
                PlayerController.Instance.CrossFadeAnimation("Riding", 0.5f);
            }
            PlayerController.Instance.AnimSetManual(false, PlayerController.Instance.AnimGetManualAxis());
            PlayerController.Instance.AnimSetNoseManual(false, PlayerController.Instance.AnimGetManualAxis());
            object[] args = new object[]
            {
            false
            };
            base.DoTransition(typeof(Custom_Impact), args);
        }

        private void ExitCopingToRiding()
        {
            PlayerController.Instance.AnimGrindTransition(false);
            PlayerController.Instance.SetTurningMode(InputController.TurningMode.Grounded);
            PlayerController.Instance.SetBoardToMaster();
            if (!PlayerController.Instance.IsRespawning)
            {
                PlayerController.Instance.CrossFadeAnimation("Riding", 0.5f);
            }
            object[] args = new object[]
            {
            22
            };
            base.DoTransition(typeof(Custom_Riding), args);
        }

        private void ExitCopingToInAir()
        {
            PlayerController.Instance.skaterController.InitializeSkateRotation();
            PlayerController.Instance.AnimGrindTransition(false);
            PlayerController.Instance.SetTurningMode(InputController.TurningMode.InAir);
            PlayerController.Instance.SetSkaterToMaster();
            if (!PlayerController.Instance.IsRespawning)
            {
                PlayerController.Instance.CrossFadeAnimation("Extend", 0.5f);
            }
            PlayerController.Instance.OnExtendAnimEnter();
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

        public override bool IsInCopingExitState()
        {
            return true;
        }

        public override void LeftTriggerHeld(float p_value, InputController.TurningMode p_turningMode)
        {
            this.RotatePlayer(-p_value);
            this._leftTrigger = p_value;
        }

        public override void LeftTriggerReleased()
        {
            this._leftTrigger = 0f;
        }

        public override void RightTriggerHeld(float p_value, InputController.TurningMode p_turningMode)
        {
            this.RotatePlayer(p_value);
            this._rightTrigger = p_value;
        }

        public override void RightTriggerReleased()
        {
            this._rightTrigger = 0f;
        }

        private void RotatePlayer(float p_value)
        {
            this._playerTurn = p_value * Time.deltaTime * 300f;
        }

        public override void BothTriggersReleased(InputController.TurningMode turningMode)
        {
            this._playerTurn = 0f;
        }

        public override void OnStickFixedUpdate(StickInput p_leftStick, StickInput p_rightStick)
        {
            PlayerController.Instance.SetLeftIKOffset(p_leftStick.ToeAxis, p_leftStick.ForwardDir, p_leftStick.PopDir, p_leftStick.IsPopStick, true, false);
            PlayerController.Instance.SetRightIKOffset(p_rightStick.ToeAxis, p_rightStick.ForwardDir, p_rightStick.PopDir, p_rightStick.IsPopStick, true, false);
            float num = (this._boardFwdExitDirAngle - 90f) / 90f * Mathf.Clamp01(1f - Mathf.Pow(this._timer / this.tweakValues.popOutTorqueFalloff, 2f));
            switch (SettingsManager.Instance.controlType)
            {
                case ControlType.Same:
                    if (SettingsManager.Instance.stance == Stance.Regular)
                    {
                        PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
                        PlayerController.Instance.SetFrontPivotRotation(p_rightStick.ToeAxis);
                        PlayerController.Instance.SetBackPivotRotation(p_leftStick.ToeAxis);
                        PlayerController.Instance.SetPivotForwardRotation((num + p_leftStick.ForwardDir + p_rightStick.ForwardDir) * 0.7f, 15f);
                        PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                        return;
                    }
                    PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_rightStick.rawInput.pos.x) - Mathf.Abs(p_leftStick.rawInput.pos.x));
                    PlayerController.Instance.SetFrontPivotRotation(-p_leftStick.ToeAxis);
                    PlayerController.Instance.SetBackPivotRotation(-p_rightStick.ToeAxis);
                    PlayerController.Instance.SetPivotForwardRotation((num + p_leftStick.ForwardDir + p_rightStick.ForwardDir) * 0.7f, 15f);
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
                            PlayerController.Instance.SetPivotForwardRotation((num + p_leftStick.ForwardDir + p_rightStick.ForwardDir) * 0.7f, 15f);
                            PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                            return;
                        }
                        PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_rightStick.rawInput.pos.x) - Mathf.Abs(p_leftStick.rawInput.pos.x));
                        PlayerController.Instance.SetFrontPivotRotation(-p_leftStick.ToeAxis);
                        PlayerController.Instance.SetBackPivotRotation(-p_rightStick.ToeAxis);
                        PlayerController.Instance.SetPivotForwardRotation((num + p_leftStick.ForwardDir + p_rightStick.ForwardDir) * 0.7f, 15f);
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
                            PlayerController.Instance.SetPivotForwardRotation((num + p_leftStick.ForwardDir + p_rightStick.ForwardDir) * 0.7f, 15f);
                            PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                            return;
                        }
                        PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
                        PlayerController.Instance.SetFrontPivotRotation(-p_rightStick.ToeAxis);
                        PlayerController.Instance.SetBackPivotRotation(-p_leftStick.ToeAxis);
                        PlayerController.Instance.SetPivotForwardRotation((num + p_leftStick.ForwardDir + p_rightStick.ForwardDir) * 0.7f, 15f);
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
                            PlayerController.Instance.SetPivotForwardRotation((num + p_leftStick.ForwardDir + p_rightStick.ForwardDir) * 0.7f, 15f);
                            PlayerController.Instance.SetPivotSideRotation(p_leftStick.ToeAxis - p_rightStick.ToeAxis);
                            return;
                        }
                        PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_rightStick.rawInput.pos.x) - Mathf.Abs(p_leftStick.rawInput.pos.x));
                        PlayerController.Instance.SetFrontPivotRotation(-p_leftStick.ToeAxis);
                        PlayerController.Instance.SetBackPivotRotation(-p_rightStick.ToeAxis);
                        PlayerController.Instance.SetPivotForwardRotation((num + p_leftStick.ForwardDir + p_rightStick.ForwardDir) * 0.7f, 15f);
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
                            PlayerController.Instance.SetPivotForwardRotation((num + p_leftStick.ForwardDir + p_rightStick.ForwardDir) * 0.7f, 15f);
                            PlayerController.Instance.SetPivotSideRotation(p_rightStick.ToeAxis - p_leftStick.ToeAxis);
                            return;
                        }
                        PlayerController.Instance.SetBoardTargetPosition(Mathf.Abs(p_leftStick.rawInput.pos.x) - Mathf.Abs(p_rightStick.rawInput.pos.x));
                        PlayerController.Instance.SetFrontPivotRotation(-p_rightStick.ToeAxis);
                        PlayerController.Instance.SetBackPivotRotation(-p_leftStick.ToeAxis);
                        PlayerController.Instance.SetPivotForwardRotation((num + p_leftStick.ForwardDir + p_rightStick.ForwardDir) * 0.7f, 15f);
                        PlayerController.Instance.SetPivotSideRotation(p_rightStick.ToeAxis - p_leftStick.ToeAxis);
                        return;
                    }
                    break;
                default:
                    return;
            }
        }

        public override void OnRespawn()
        {
            base.OnRespawn();
        }
    }
}