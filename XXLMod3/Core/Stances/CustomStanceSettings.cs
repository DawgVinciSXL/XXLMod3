using System;
using UnityEngine;

namespace XXLMod3.Core
{
    [Serializable]
    public class CustomStanceSettings : BaseStanceSettings
    {
        public override bool Active { get; set; } = true;
        public override float lfPosSpeed { get; set; } = 1f;
        public override float lfRotSpeed { get; set; } = 100f;
        public override float rfPosSpeed { get; set; } = 1f;
        public override float rfRotSpeed { get; set; } = 100f;

        public override Quaternion lfRot { get; set; }
        public override Quaternion rfRot { get; set; }

        public override Vector3 lfPos { get; set; }
        public override Vector3 rfPos { get; set; }

        public override Quaternion ltRot { get; set; }
        public override Quaternion rtRot { get; set; }

        public CustomStanceSettings()
        {
        }

        public CustomStanceSettings(bool p_active, float p_lfPosSpeed, float p_lfRotSpeed, float p_rfPosSpeed, float p_rfRotSpeed, Quaternion p_lfRot, Quaternion p_rfRot, Vector3 p_lfPos, Vector3 p_rfPos, Quaternion p_ltRot, Quaternion p_rtRot)
        {
            Active = p_active;

            lfPosSpeed = p_lfPosSpeed;
            lfRotSpeed = p_lfRotSpeed;
            rfPosSpeed = p_rfPosSpeed;
            rfRotSpeed = p_rfRotSpeed;

            lfRot = p_lfRot;
            rfRot = p_rfRot;

            lfPos = p_lfPos;
            rfPos = p_rfPos;

            ltRot = p_ltRot;
            rtRot = p_rtRot;
        }
    }
}