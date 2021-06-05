namespace XXLMod3.Core
{
    public class CustomLegSettings : BaseLegSettings
    {
        public override bool Active { get => base.Active; set => base.Active = value; }
        public override float LeftLegX { get => base.LeftLegX; set => base.LeftLegX = value; }
        public override float LeftLegY { get => base.LeftLegY; set => base.LeftLegY = value; }
        public override float LeftLegZ { get => base.LeftLegZ; set => base.LeftLegZ = value; }
        public override float RightLegX { get => base.RightLegX; set => base.RightLegX = value; }
        public override float RightLegY { get => base.RightLegY; set => base.RightLegY = value; }
        public override float RightLegZ { get => base.RightLegZ; set => base.RightLegZ = value; }

        public CustomLegSettings()
        {
        }

        public CustomLegSettings(bool active, float leftLegX, float leftLegY, float leftLegZ, float rightLegX, float rightLegY, float rightLegZ)
        {
            Active = active;
            LeftLegX = leftLegX;
            LeftLegY = leftLegY;
            LeftLegZ = leftLegZ;
            RightLegX = rightLegX;
            RightLegY = rightLegY;
            RightLegZ = rightLegZ;
        }
    }
}