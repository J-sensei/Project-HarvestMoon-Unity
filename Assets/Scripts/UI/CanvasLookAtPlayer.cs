using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Rotate the canvas to look at the main camera
    /// </summary>
    public class CanvasLookAtPlayer : MonoBehaviour
    {
        private Transform _camera;

        private void Start()
        {
            _camera = Camera.main.transform;
        }

        private void LateUpdate()
        {
            transform.LookAt(_camera);
        }
    }

}