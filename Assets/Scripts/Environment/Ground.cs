using UnityEngine;

namespace Environment
{
    /// <summary>
    /// Ground object, put inside the ground environmental objects
    /// </summary>
    public class Ground : MonoBehaviour
    {
        [Tooltip("Which type of this ground is")]
        [SerializeField] private GroundType type = GroundType.Default;
        /// <summary>
        /// Type of the ground
        /// </summary>
        public GroundType Type { get { return type; } }
    }
}
