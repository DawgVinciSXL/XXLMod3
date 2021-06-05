using UnityEngine;

namespace XXLMod3.Core
{
    public class CustomFeetObject : BaseFeetObject
    {
        public override GameObject LeftFootPos { get => base.LeftFootPos; set => base.LeftFootPos = value; }
        public override GameObject LeftFootRot { get => base.LeftFootRot; set => base.LeftFootRot = value; }
        public override GameObject LeftToeRot { get => base.LeftToeRot; set => base.LeftToeRot = value; }
        public override GameObject RightFootPos { get => base.RightFootPos; set => base.RightFootPos = value; }
        public override GameObject RightFootRot { get => base.RightFootRot; set => base.RightFootRot = value; }
        public override GameObject RightToeRot { get => base.RightToeRot; set => base.RightToeRot = value; }
        public override BaseStanceSettings StanceSettings { get => base.StanceSettings; set => base.StanceSettings = value; }
    }
}
