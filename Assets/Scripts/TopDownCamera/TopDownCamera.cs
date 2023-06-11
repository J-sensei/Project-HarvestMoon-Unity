using UnityEngine;

namespace TopDownCamera
{
    public class TopDownCamera : MonoBehaviour
    {
        [Tooltip("Target transform position")]
        [SerializeField] private Transform target;
        [SerializeField] private float height = 10f;
        [SerializeField] private float distance = 20f;

        [Tooltip("Angle to rotate the camera")]
        [Range(0f, 360f)]
        [SerializeField] private float angle = 45f;
        [SerializeField] private float smoothTime = 0.5f;

        public Transform Target { get { return target; } }
        public float Height 
        {
            get { return height; }
            set { height = value; }
        }
        public float Distance 
        { 
            get { return distance; }
            set { distance = value; }
        }
        public float Angle
        {
            get { return angle; }
            set { angle = value; }
        }

        private Vector3 _refVelocity;

        // Camera Collision
        [Header("Camera Collision")]
        private Vector3 _camDirection;
        private float _camDistance;
        [SerializeField] private float _minCamDistance = 0.5f;
        [SerializeField] private float _maxCamDistance = 5f;
        private Transform _camTransform;
        private bool _blocking = false;

        private void Start()
        {
            _camTransform = transform;
            _camDirection = _camTransform.localPosition.normalized;
            _camDistance = _maxCamDistance;

            InitializeCameraPos(); // Instantly teleport to the target position
        }

        private void Update()
        {
            //CheckCameraOcclusionAndCollision(_camTransform);
            CameraHandler();
        }

        private void LateUpdate()
        {
            //CheckCameraOcclusionAndCollision(_camTransform);
        }

        public void UpdateTarget(Transform target) => this.target = target;

        /// <summary>
        /// Update target transform and immediately update the camera position to the target
        /// </summary>
        /// <param name="target"></param>
        public void UpdateTargetAndInitialize(Transform target)
        {
            UpdateTarget(target);
            InitializeCameraPos();
        }

        /// <summary>
        /// Instantly move camera to the target position
        /// </summary>
        protected virtual void InitializeCameraPos()
        {
            if (target == null)
            {
                Debug.LogWarning("[TopDownCamera] Target is null");
                return;
            }

            // Build world position vector
            Vector3 worldPos = (Vector3.forward * -distance) + (Vector3.up * height);

            // Camera rotation
            Vector3 rotatedVector = Quaternion.AngleAxis(angle, Vector3.up) * worldPos;

            // Move our position
            Vector3 targetPos = target.position;
            //targetPos.y = 0;
            Vector3 finalPos = targetPos + rotatedVector;

            // Smooth values
            transform.position = finalPos;
            transform.LookAt(targetPos);
        }

        /// <summary>
        /// Smoothly move camera to the target
        /// </summary>
        protected virtual void CameraHandler()
        {
            if (target == null)
            {
                Debug.LogWarning("[TopDownCamera] Target is null");
                return;
            }

            // Build world position vector
            float dist = distance;
            Vector3 worldPos = (Vector3.forward * -dist) + (Vector3.up * height);
            // Debug.DrawLine(target.position, worldPos, Color.red);

            // Camera rotation
            Vector3 rotatedVector = Quaternion.AngleAxis(angle, Vector3.up) * worldPos;
            // Debug.DrawLine(target.position, rotatedVector, Color.green);

            // Move our position
            Vector3 targetPos = target.position;
            //targetPos.y = 0;
            Vector3 finalPos = targetPos + rotatedVector;

            // Smooth values
            transform.position = Vector3.SmoothDamp(transform.position, finalPos, ref _refVelocity, smoothTime);
            transform.LookAt(targetPos);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.35f);
            if (target != null)
            {
                Gizmos.DrawLine(transform.position, target.position);
                Gizmos.DrawSphere(target.position, 0.5f);
            }

            Gizmos.DrawSphere(transform.position, 0.5f);
        }

        private void CheckCameraOcclusionAndCollision(Transform camTransform)
        {
            float distance = 2f;
            Vector3 desiredPos = transform.TransformPoint(_camDirection * _maxCamDistance);
            RaycastHit hit;
            if(Physics.Raycast(transform.position, target.position, out hit, distance))
            {
                // If any objects in between
                //_camDistance = Mathf.Clamp(hit.distance, _minCamDistance, _maxCamDistance);
                _camDistance = distance * 1.5f;
                _blocking = true;
            }
            else
            {
                _camDistance = _maxCamDistance;
                _blocking = false;
            }

            Debug.DrawRay(transform.position, (target.position - transform.position).normalized * distance, Color.red, 0f, false);

            //_camTransform.localPosition = _camDirection * _camDistance;
        }
    }
}
