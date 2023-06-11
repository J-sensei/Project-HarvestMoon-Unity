using UnityEngine;

namespace Environment
{
    public class Windmill : MonoBehaviour
    {
        [SerializeField] private Transform rotateTransform;
        [SerializeField] private float rotateSpeed = 1f;
        [SerializeField] private Vector3 rotateAxis = Vector3.zero;

        private void Update()
        {
            if(rotateTransform != null)
            {
                rotateTransform.Rotate(rotateAxis * rotateSpeed * Time.deltaTime);
            }
        }
    }
}
