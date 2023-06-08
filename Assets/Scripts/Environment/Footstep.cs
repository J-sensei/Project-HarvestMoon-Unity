using UnityEngine;

namespace Environment
{
    /// <summary>
    /// Type of ground available in the game
    /// </summary>
    public enum GroundType
    {
        Default,
        Dirt,
        Water,
    }

    /// <summary>
    /// Footstep audios structure to tied the audios with the ground type
    /// </summary>
    [System.Serializable]
    public struct FootstepAudio
    {
        /// <summary>
        /// Type of the ground
        /// </summary>
        public GroundType type;
        /// <summary>
        /// Audio clips corresponding to the ground type
        /// </summary>
        public AudioClip[] footsteps;
    }

}
