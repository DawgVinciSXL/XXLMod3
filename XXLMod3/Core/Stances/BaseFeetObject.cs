using UnityEngine;

namespace XXLMod3.Core
{
    public class BaseFeetObject
    {
        public virtual GameObject LeftFootPos { get; set; }
        public virtual GameObject LeftFootRot { get; set; }
        public virtual GameObject LeftToeRot { get; set; }

        public virtual GameObject RightFootPos { get; set; }
        public virtual GameObject RightFootRot { get; set; }
        public virtual GameObject RightToeRot { get; set; }

        public virtual BaseStanceSettings StanceSettings { get; set; }
    }
}
