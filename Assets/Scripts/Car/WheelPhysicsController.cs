using System;
using UnityEngine;

namespace Car
{
    public class WheelPhysicsController : MonoBehaviour
    {

        private Rigidbody _carRigidbody;
        private CarConfiguration _configuration;
        private CarController _controller;
        private GameObject _sphere;
        
        public bool driveWheel;
        public bool brakeWheel;
        public bool steerWheel;
        public bool frontWheel;
        public bool leftWheel;
    
        void Start()
        {
            var parent = transform.parent;
            _carRigidbody = parent.GetComponent<Rigidbody>();
            _configuration = parent.GetComponent<CarConfiguration>();
            _controller = parent.GetComponent<CarController>();
            // Time.timeScale = 0.1f;
        }

        private RaycastHit hit;

        private void Update()
        {
            if (!steerWheel) return;
            transform.rotation = _carRigidbody.transform.rotation * Quaternion.Euler(
                0, 
                90 + Mathf.LerpAngle(-20, 20, (_controller.Controls.x + 1)/2), 
                0
            );
        }

        void FixedUpdate()
        {
            hit = _test();
            if (hit.collider == null) return;
            _suspension(hit.distance);
            _steering(_steeringTransform());
            if (driveWheel && !_controller.IsBraking && _controller.Controls.y != 0)
                _acceleration(_controller.Controls.y);
            if (brakeWheel && _controller.IsBraking)
                _brake(1f);
        }

        private Vector3 _steeringTransform()
        {
            var direction = Vector3.Dot(transform.right, _carRigidbody.GetPointVelocity(transform.position));
            return direction > 0 ? -transform.right : transform.right;
        }

        private RaycastHit _test()
        {
            Transform wheelTransform = transform;
            Vector3 down = -wheelTransform.up;
            Vector3 position = wheelTransform.position;
            RaycastHit hit;
            
            Ray ray = new Ray(position, down);
            Physics.Raycast(ray, out hit, maxDistance: _configuration.suspensionMaxDistance);
            
            Debug.DrawRay(position, down * _configuration.suspensionMaxDistance);

            return hit;
        }

        private void _suspension(float tyreDistance)
        {
            Vector3 tyrePosition = transform.position;
            Vector3 springDir = transform.up;
            Vector3 tyreWorldVelocity = _carRigidbody.GetPointVelocity(tyrePosition);
            float offset = _configuration.suspensionRestDistance - tyreDistance;

            float velocity = Vector3.Dot(springDir, tyreWorldVelocity);
            float force = (offset * _configuration.springStrength) - (velocity * _configuration.springDamper);
            
            _carRigidbody.AddForceAtPosition(springDir * force, tyrePosition);
        }

        private void _steering(Vector3 steeringDirection)
        {
            var tyreTransform = transform;
            Vector3 tyreWorldVelocity = _carRigidbody.GetPointVelocity(tyreTransform.position);
            float steeringVelocity = Vector3.Dot(steeringDirection, tyreWorldVelocity);
            
            float desiredVelocityChange = -steeringVelocity * 
                                          (frontWheel ? _configuration.frontTyreGripCurve : _configuration.rearTyreGripCurve)
                                          .Evaluate(steeringVelocity);
            float desiredAccelerationChange = desiredVelocityChange / Time.fixedDeltaTime;
            var appliedForce = desiredAccelerationChange * _configuration.tyreMass * steeringDirection;
            
            Debug.DrawRay(tyreTransform.position, desiredVelocityChange * steeringDirection, Color.red);
            Debug.DrawRay(tyreTransform.position, steeringDirection * steeringVelocity, Color.blue);
            Debug.DrawRay(tyreTransform.position, tyreWorldVelocity, frontWheel ? Color.green : Color.magenta);
            
            _carRigidbody.AddForceAtPosition(appliedForce, tyreTransform.position);
        }

        private void _acceleration(float magnitude)
        {
            Vector3 tyrePosition = transform.position;
            Vector3 accelerationDirection = transform.forward;

            float normalizedSpeed = _normalizedSpeed();
            print($"Speed = {normalizedSpeed}, {_carRigidbody.velocity.magnitude}");
            if (normalizedSpeed >= 1) return;
            
            float torque = _configuration.accelerationCurve.Evaluate(normalizedSpeed) * magnitude * _configuration.torqueMultiplier;
            Debug.DrawRay(tyrePosition, accelerationDirection * torque, Color.yellow);
            _carRigidbody.AddForceAtPosition(accelerationDirection * torque, tyrePosition);
        }

        private float _normalizedSpeed()
        {
            float carSpeed = Vector3.Dot(_carRigidbody.transform.forward, _carRigidbody.velocity);
            return Mathf.Clamp01(Mathf.Abs(carSpeed) / _configuration.maxForwardVelocity);
        }

        private void _brake(float magnitude)
        {
            _steering(-transform.forward * magnitude * 0.5f);
        }
    }
}
