using System;
using UnityEngine;

namespace XXLMod3.Core
{
    [Serializable]
    public class BaseStanceSettings
    {
        public virtual bool Active { get; set; }
        public virtual float lfPosSpeed { get; set; }
        public virtual float lfRotSpeed { get; set; }
        public virtual Quaternion lfRot { get; set; }
        public virtual Vector3 lfPos { get; set; }
        public virtual Quaternion ltRot { get; set; }

        public virtual float rfPosSpeed { get; set; }
        public virtual float rfRotSpeed { get; set; }
        public virtual Quaternion rfRot { get; set; }
        public virtual Vector3 rfPos { get; set; }
        public virtual Quaternion rtRot { get; set; }

        public BaseStanceSettings()
        {
        }

        public BaseStanceSettings(bool p_active, float p_lfPosSpeed, float p_lfRotSpeed, float p_rfPosSpeed, float p_rfRotSpeed, Quaternion p_lfRot, Quaternion p_rfRot, Vector3 p_lfPos, Vector3 p_rfPos, Quaternion p_ltRot, Quaternion p_rtRot)
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