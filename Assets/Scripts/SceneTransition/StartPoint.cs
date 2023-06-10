using UnityEngine;

namespace SceneTransition
{
    [System.Serializable]
    public struct StartPoint
    {
        public SceneLocation enterFrom;
        public Transform spawnPoint;
    }
}
