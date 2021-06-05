using System;
using UnityEngine;

namespace XXLMod3.Core
{
    [Serializable]
    public class SaveStance
    {
        [Serializable]
        public struct SerializableVector3
        {
            public float x;
            public float y;
            public float z;

            public SerializableVector3(float rX, float rY, float rZ)
            {
                x = rX;
                y = rY;
                z = rZ;
            }

            public static implicit operator Vector3(SerializableVector3 rValue)
            {
                return new Vector3(rValue.x, rValue.y, rValue.z);
            }

            public static implicit operator SerializableVector3(Vector3 rValue)
            {
                return new SerializableVector3(rValue.x, rValue.y, rValue.z);
            }
        }

        [Serializable]
        public struct SerializableQuaternion
        {
            public float x;
            public float y;
            public float z;
            public float w;

            public SerializableQuaternion(float rX, float rY, float rZ, float rW)
            {
                x = rX;
                y = rY;
                z = rZ;
                w = rW;
            }

            public static implicit operator Quaternion(SerializableQuaternion rValue)
            {
                return new Quaternion(rValue.x, rValue.y, rValue.z, rValue.w);
            }

            public static implicit operator SerializableQuaternion(Quaternion rValue)
            {
                return new SerializableQuaternion(rValue.x, rValue.y, rValue.z, rValue.w);
            }
        }

        public bool Active;

        public float lfPosSpeed;
        public float lfRotSpeed;
        public float rfPosSpeed;
        public float rfRotSpeed;

        public SerializableVector3 LeftFootPosition;
        public SerializableVector3 RightFootPosition;

        public SerializableQuaternion LeftFootRotation;
        public SerializableQuaternion RightFootRotation;

        public SerializableQuaternion LeftToeRotation;
        public SerializableQuaternion RightToeRotation;

        public SaveStance(bool _active, float _lfPosSpeed, float _lfRotSpeed, float _rfPosSpeed, float _rfRotSpeed, Vector3 lfPos, Vector3 rfPos, Quaternion lfRot, Quaternion rfRot, Quaternion ltRot, Quaternion rtRot)
        {
            Active = _active;

            lfPosSpeed = _lfPosSpeed;
            lfRotSpeed = _lfRotSpeed;
            rfPosSpeed = _rfPosSpeed;
            rfRotSpeed = _rfRotSpeed;

            LeftFootPosition = lfPos;
            RightFootPosition = rfPos;

            LeftFootRotation = lfRot;
            RightFootRotation = rfRot;

            LeftToeRotation = ltRot;
            RightToeRotation = rtRot;
        }
    }
}
