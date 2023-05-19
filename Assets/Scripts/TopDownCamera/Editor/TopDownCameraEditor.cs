using UnityEngine;
using UnityEditor;

namespace TopDownCamera
{
    [CustomEditor(typeof(TopDownCamera))]
    public class TopDownCameraEditor : Editor
    {
        private TopDownCamera _camera;

        private void OnEnable()
        {
            _camera = (TopDownCamera)target;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }

        private void OnSceneGUI()
        {
            if (_camera.Target == null) return;

            Transform cameraTarget = _camera.Target;

            // Tell the distance by drawing the circle
            Handles.color = new Color(1f, 0f, 0f, 0.15f);
            Handles.DrawSolidDisc(cameraTarget.position, Vector3.up, _camera.Distance);
            Handles.color = new Color(1f, 1f, 0f, 1f);
            Handles.DrawWireDisc(cameraTarget.position, Vector3.up, _camera.Distance);

            // Adjust distance
            Handles.color = new Color(0f, 0f, 1f, 1f);
            _camera.Distance = Handles.ScaleSlider(_camera.Distance, cameraTarget.position, -cameraTarget.forward, Quaternion.identity, _camera.Distance, 1f);
            _camera.Distance = Mathf.Clamp(_camera.Distance, 1f, float.MaxValue);

            // Adjust height
            Handles.color = new Color(0f, 1f, 0f, 1f);
            _camera.Height = Handles.ScaleSlider(_camera.Height, cameraTarget.position, Vector3.up, Quaternion.identity, _camera.Height, 1f);
            _camera.Height = Mathf.Clamp(_camera.Height, 1f, float.MaxValue);

            // Labels
            GUIStyle labelStyle = new();
            labelStyle.fontSize = 20;
            labelStyle.normal.textColor = Color.white;
            labelStyle.fontStyle = FontStyle.Normal;
            labelStyle.alignment = TextAnchor.UpperCenter;

            Handles.Label(cameraTarget.position + (-cameraTarget.forward * _camera.Distance), "Distance", labelStyle);
            Handles.Label(cameraTarget.position + (Vector3.up * _camera.Height), "Height", labelStyle);
        }
    }
}
