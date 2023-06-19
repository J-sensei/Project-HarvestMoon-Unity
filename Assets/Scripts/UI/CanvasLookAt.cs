using UnityEngine;

namespace UI
{
    public class CanvasLookAt : MonoBehaviour
    {
        [SerializeField] private Transform cameraTransform;

        private void Start()
        {
            if (cameraTransform == null)
            {
                cameraTransform = Camera.main.transform;
            }
        }

        private void LateUpdate()
        {
            transform.LookAt(cameraTransform);
        }
    }

}