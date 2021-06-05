using System;

namespace XXLMod3.Core
{
    [Serializable]
    public class BaseGrindSettings
    {
        public virtual float AnimationSpeed { get; set; }
        public virtual bool BumpOut { get; set; }
        public virtual float BumpOutPopForce { get; set; }
        public virtual float BumpOutSidewayForce { get; set; }
        public virtual CrouchMode CrouchMode { get; set; }
        public virtual float CrouchAmount { get; set; }
        public virtual float Friction { get; set; }
        public virtual float MaxAngleModifier { get; set; }
        public virtual float PopForce { get; set; }
        public virtual bool PopOut { get; set; }
        public virtual float PopThreshold { get; set; }
        public virtual float SidewayPopForce { get; set; }
        public virtual bool Stabilizer { get; set; }
        public virtual float TorqueModifier { get; set; }
    }
}
