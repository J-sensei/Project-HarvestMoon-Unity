using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace TopDownCamera
{
    [RequireComponent(typeof(Camera))]
    public class TopDownCameraController : MonoBehaviour
    {
        /// <summary>
        /// Unity new input system (Class is genenerated by the InputAction)
        /// </summary>
        private InputControls _inputControls;
        private TopDownCamera _camera;

        [SerializeField] private float rotateMultiplier = 10f;
        [SerializeField] private float rotateSmoothTime = 10f;
        private float _rotateVel;

        private void Awake()
        {
            _inputControls = new();
            //_inputControls.Camera.MousePosition.performed += OnMouseMove;

            _camera = GetComponent<TopDownCamera>();
        }

        private void Update()
        {
            if (Mouse.current.leftButton.isPressed)
            {
                Vector3 angleMove = Mouse.current.delta.ReadValue().normalized;

                _camera.Angle = Mathf.SmoothDamp(_camera.Angle, _camera.Angle + (angleMove.x * rotateMultiplier), ref _rotateVel, rotateSmoothTime);
                if(_camera.Angle < 0)
                {
                    _camera.Angle = 360f;
                } 
                else if(_camera.Angle > 360f)
                {
                    _camera.Angle = 0;
                }
            }

        }
    }
}
