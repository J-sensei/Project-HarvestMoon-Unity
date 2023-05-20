using System.Collections;
using System.Collections.Generic;
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

        private Vector3 refVelocity;
        private void Start()
        {
            CameraHandler();
        }

        private void Update()
        {
            CameraHandler();
        }

        protected virtual void CameraHandler()
        {
            if (target == null)
            {
                Debug.LogWarning("[TopDownCamera] Target is null");
                return;
            }

            // Build world position vector
            Vector3 worldPos = (Vector3.forward * -distance) + (Vector3.up * height);
            // Debug.DrawLine(target.position, worldPos, Color.red);

            // Camera rotation
            Vector3 rotatedVector = Quaternion.AngleAxis(angle, Vector3.up) * worldPos;
            // Debug.DrawLine(target.position, rotatedVector, Color.green);

            // Move our position
            Vector3 targetPos = target.position;
            //targetPos.y = 0;
            Vector3 finalPos = targetPos + rotatedVector;

            // Debug.DrawLine(target.position, finalPos, Color.blue);

            // Smooth values
            transform.position = Vector3.SmoothDamp(transform.position, finalPos, ref refVelocity, smoothTime);
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
    }
}
