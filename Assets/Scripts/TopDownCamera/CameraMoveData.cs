using UnityEngine;

namespace TopDownCamera
{
    [System.Serializable]
    public struct CameraMoveData
    {
        public Transform target;
        public float height;
        public float distance;
        public float angle;
        public float smoothTime;
        public float stayTime;
        public bool fade;
    }
}
