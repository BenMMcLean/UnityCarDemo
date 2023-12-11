using UnityEngine;
using UnityEngine.Serialization;

namespace Car
{
    public class CarConfiguration: MonoBehaviour
    {
        
        public float suspensionRestDistance;
        public float suspensionMaxDistance;
        public float springStrength;
        public float springDamper;
        
        [FormerlySerializedAs("tyreGripCurve")] public AnimationCurve frontTyreGripCurve;
        public AnimationCurve rearTyreGripCurve;
        public float tyreMass;

        public AnimationCurve accelerationCurve;
        public float maxForwardVelocity;
        public float torqueMultiplier;
    }
}