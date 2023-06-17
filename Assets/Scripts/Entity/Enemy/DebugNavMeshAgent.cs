using UnityEngine;
using UnityEngine.AI;

namespace Entity.Enemy
{
    /// <summary>
    /// Draw the Nav Mesh Agent properties (For debug purposes)
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class DebugNavMeshAgent : MonoBehaviour
    {
        [Header("Draw")]
        [Tooltip("Draw velocity line of the agent")]
        public bool velocity;
        [Tooltip("Draw desired velocity line of the agent")]
        public bool desiredVelocity;
        [Tooltip("Draw paths of the agent")]
        public bool path;

        /// <summary>
        /// Nav mesh agent reference of the enemy
        /// </summary>
        private NavMeshAgent _agent;

        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        private void OnDrawGizmos()
        {
            if (_agent == null) return;

            if (velocity)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, transform.position + _agent.velocity);
            }

            if (desiredVelocity)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, transform.position + _agent.desiredVelocity);
            }

            if (path)
            {
                Gizmos.color = Color.red;
                NavMeshPath path = _agent.path;
                Vector3 prevCorner = transform.position;
                foreach (Vector3 corner in path.corners)
                {
                    Gizmos.DrawLine(prevCorner, corner);
                    Gizmos.DrawSphere(corner, 0.1f);
                    prevCorner = corner;
                }
            }
        }
    }
}

