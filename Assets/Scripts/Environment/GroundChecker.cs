using UnityEngine;
using UnityEngine.Events;

namespace Environment
{
    /// <summary>
    /// Use ray cast to detect ground changes
    /// </summary>
    public class GroundChecker : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Valid maximum distance for the ray cast to happen")]
        [SerializeField] private float maxInteractionRay = 1f;


        [Header("Debug")]
        [Tooltip("Show debug properties fro the player interactor")]
        [SerializeField] private bool debug = false;
        /// <summary>
        /// Line color for the debug ray
        /// </summary>
        private Color _debugLineColor = Color.yellow;

        /// <summary>
        /// Take ground type as parameter to use it as the event
        /// </summary>
        public class GroundCheckEvent : UnityEvent<GroundType> { }
        /// <summary>
        /// Events to fire when ray cast hit the valid ground
        /// </summary>
        public GroundCheckEvent OnGroundCheck = new();

        private void Update()
        {
            // Draw raycast below
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, maxInteractionRay))
            {
                Collider col = hit.collider;
                if(col.TryGetComponent(out Ground ground))
                {
                    _debugLineColor = Color.green;
                    OnGroundCheck?.Invoke(ground.Type);
                }
                else
                {
                    _debugLineColor = Color.yellow;
                }
            }
            else
            {
                _debugLineColor = Color.yellow;
            }

            if (debug)
            {
                // Draw ray to show how the ray cast work
                Debug.DrawRay(transform.position, Vector3.down * maxInteractionRay, _debugLineColor, 0f, false);
            }
        }
    }
}
