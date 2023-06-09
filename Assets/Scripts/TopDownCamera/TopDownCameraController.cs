using UnityEngine;

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
            float angleMove = Input.mouseScrollDelta.y;
            angleMove = Mathf.Clamp(angleMove, -1, 1);

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
