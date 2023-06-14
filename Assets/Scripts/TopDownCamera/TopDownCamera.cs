using System.Collections;
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

        [Header("Shake")]
        [SerializeField] private float shakeDuration = 1f;
        [SerializeField] private AnimationCurve shakeCurve;
        [SerializeField] private bool testShake = false;

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

        private void Start()
        {
            InitializeCameraPos(); // Instantly teleport to the target position
        }

        private void Update()
        {
            //CheckCameraOcclusionAndCollision(_camTransform);
            CameraHandler();

            if (testShake)
            {
                testShake = false;
                StartCoroutine(Shaking());
            }
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

        public void Shake() => StartCoroutine(Shaking());

        private IEnumerator Shaking()
        {
            Vector3 startPos = transform.position;
            float elapsed = 0f;

            while(elapsed < shakeDuration)
            {
                elapsed += Time.deltaTime;
                float strength = shakeCurve.Evaluate(elapsed / shakeDuration);
                transform.position = startPos + Random.insideUnitSphere * strength;
                yield return null;
            }

            transform.position = startPos;
        }
    }
}
