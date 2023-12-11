using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Car
{
    public class SimpleCarController: CarController
    {
        
        public InputActionReference movementControl;

        private Rigidbody _rigidbody;

        private bool _isBraking;
        private Vector2 _controls;
        
        public override bool IsBraking => _isBraking;
        public override Vector2 Controls => _controls;

        public void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void Update()
        {
            _controls = movementControl.action.ReadValue<Vector2>();
            _isBraking = _controls.y == 0;
        }
    }
}