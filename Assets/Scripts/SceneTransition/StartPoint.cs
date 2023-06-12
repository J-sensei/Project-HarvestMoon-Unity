using UnityEngine;

namespace SceneTransition
{
    [System.Serializable]
    public struct StartPoint
    {
        /// <summary>
        /// Which scene of the plauer from to be able to assign this spawn point
        /// </summary>
        [Tooltip("Which scene of the plauer from to be able to assign this spawn point")]
        public SceneLocation enterFrom;
        /// <summary>
        /// Spawn transform when the player enter the scene
        /// </summary>
        [Tooltip("Spawn transform when the player enter the scene")]
        public Transform spawnPoint;
    }
}
