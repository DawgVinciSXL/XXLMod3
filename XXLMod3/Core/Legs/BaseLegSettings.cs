namespace XXLMod3.Core
{
    public class BaseLegSettings
    {
        public virtual bool Active { get; set; }

        public virtual float LeftLegX { get; set; }
        public virtual float LeftLegY { get; set; }
        public virtual float LeftLegZ { get; set; }

        public virtual float RightLegX { get; set; }
        public virtual float RightLegY { get; set; }
        public virtual float RightLegZ { get; set; }

        public BaseLegSettings()
        {
        }

        public BaseLegSettings(bool active, float leftLegX, float leftLegY, float leftLegZ, float rightLegX, float rightLegY, float rightLegZ)
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