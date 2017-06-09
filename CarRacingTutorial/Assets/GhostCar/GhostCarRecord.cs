using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.GhostCar
{
    [Serializable()]
    public class GhostCarRecord
    {
        public float PastTime { get; set; }        
        public SerializableVector3 BuggyPosition { get; set; }
        public SerializableQuaternion BuggyRotation { get; set; }

        public SerializableQuaternion WheelFrontLeftRotation { get; set; }
        public SerializableQuaternion WheelFrontRightRotation { get; set; }
        public SerializableQuaternion WheelBackLeftRotation { get; set; }
        public SerializableQuaternion WheelBackRightRotation { get; set; }        
    }

    [Serializable()]
    public struct SerializableVector3
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }        
    }

    [Serializable()]
    public struct SerializableQuaternion
    {
        public float w { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }
}
