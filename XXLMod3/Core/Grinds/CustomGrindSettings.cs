using System;

namespace XXLMod3.Core
{
    [Serializable]
    public class CustomGrindSettings : BaseGrindSettings
    {
        public override float AnimationSpeed { get; set; } = 1f;
        public override bool BumpOut { get; set; } = true;
        public override float BumpOutPopForce { get; set; } = 1.25f;
        public override float BumpOutSidewayForce { get; set; } = 1.2f;
        public override CrouchMode CrouchMode { get; set; } = CrouchMode.Off;
        public override float CrouchAmount { get; set; } = 0.95f;
        public override float Friction { get; set; } = 0.25f;
        public override float MaxAngleModifier { get; set; } = 15f;
        public override float PopForce { get; set; } = 2f;
        public override bool PopOut { get; set; } = true;
        public override float PopThreshold { get; set; } = 15f;
        public override float SidewayPopForce { get; set; } = 0.75f;
        public override bool Stabilizer { get; set; } = true;
        public override float TorqueModifier { get; set; } = 44f;

        public CustomGrindSettings()
        {
        }

        public CustomGrindSettings(float _animationSpeed, bool _bumpOut, float _bumpOutPopForce, float _bumpOutSidewayForce, CrouchMode _crouchMode, float _crouchAmount, float _friction,
            float _maxAngleModifier, float _popForce, bool _popOut, float _popThreshold, float _sidewayPopForce, bool _stabilizer, float _torqueModifier)
        {
            AnimationSpeed = _animationSpeed;
            BumpOut = _bumpOut;
            BumpOutPopForce = _bumpOutPopForce;
            BumpOutSidewayForce = _bumpOutSidewayForce;
            CrouchMode = _crouchMode;
            CrouchAmount = _crouchAmount;
            Friction = _friction;
            MaxAngleModifier = _maxAngleModifier;
            PopForce = _popForce;
            PopOut = _popOut;
            PopThreshold = _popThreshold;
            SidewayPopForce = _sidewayPopForce;
            Stabilizer = _stabilizer;
            TorqueModifier = _torqueModifier;
        }
    }
}