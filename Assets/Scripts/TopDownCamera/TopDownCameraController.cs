using UnityEngine;
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

        private float rotateValue = 0f;

        private void Awake()
        {
            _inputControls = new();
            //_inputControls.Camera.MousePosition.performed += OnMouseMove;
            _inputControls.Camera.RotateLeft.started += OnRotateLeft;
            _inputControls.Camera.RotateLeft.performed += OnRotateLeft;
            _inputControls.Camera.RotateLeft.canceled += OnRotateLeft;

            _inputControls.Camera.RotateRight.started += OnRotateRight;
            _inputControls.Camera.RotateRight.performed += OnRotateRight;
            _inputControls.Camera.RotateRight.canceled += OnRotateRight;
            _inputControls.Camera.Enable();

            _camera = GetComponent<TopDownCamera>();
        }

        private void OnDestroy()
        {
            _inputControls.Camera.RotateLeft.started -= OnRotateLeft;
            _inputControls.Camera.RotateLeft.performed -= OnRotateLeft;
            _inputControls.Camera.RotateLeft.canceled -= OnRotateLeft;

            _inputControls.Camera.RotateRight.started -= OnRotateRight;
            _inputControls.Camera.RotateRight.performed -= OnRotateRight;
            _inputControls.Camera.RotateRight.canceled -= OnRotateRight;
            _inputControls.Camera.Disable();
        }

        private void OnRotateLeft(InputAction.CallbackContext context)
        {
            if (context.ReadValueAsButton())
            {
                rotateValue = -1f;
            }
            else
            {
                rotateValue = 0f;
            }
        }

        private void OnRotateRight(InputAction.CallbackContext context)
        {
            if (context.ReadValueAsButton())
            {
                rotateValue = 1f;
            }
            else
            {
                rotateValue = 0f;
            }
        }

        private void Update()
        {
            //float angleMove = Input.mouseScrollDelta.y;
            //angleMove = Mathf.Clamp(angleMove, -1, 1);
            float angleMove = Mathf.Clamp(rotateValue, -1, 1);

            _camera.Angle = Mathf.SmoothDamp(_camera.Angle, _camera.Angle + (angleMove * rotateMultiplier), ref _rotateVel, rotateSmoothTime);
            
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
