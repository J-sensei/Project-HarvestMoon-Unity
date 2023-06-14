using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UI
{
    [ExecuteInEditMode]
    public class ProgressBar : MonoBehaviour
    {
        #if UNITY_EDITOR
        [MenuItem("GameObject/UI/Linear Progress Bar")]
        public static void AddLinearProgressBar()
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("UI/Progress Bar/Linear Progress Bar"));
            go.transform.SetParent(Selection.activeGameObject.transform, false);
        }
        #endif

        [Header("Values")]
        [Tooltip("Minimum value consider as 'zero' in the progress bar")]
        [SerializeField] private int minValue = 0;
        [Tooltip("Maximum value of the progress bar")]
        [SerializeField] private int maxValue = 100;
        [Tooltip("Current value of the progress bar")]
        [SerializeField] private int currentValue = 100;

        [Header("Images")]
        [SerializeField] private Image mask;
        [SerializeField] private Image fill;
        [SerializeField] private Color color;

        private void Update()
        {
            if (Application.isEditor)
            {
                UpdateFill();
            }
        }

        public void UpdateValues(int minValue, int maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;

            UpdateFill();
        }

        public void UpdateValue(int currentValue)
        {
            this.currentValue = currentValue;
            UpdateFill();
        }

        private void UpdateFill()
        {
            if (mask == null) return;
            
            float currentOffset = currentValue - minValue;
            float maxOffset = maxValue - minValue;

            float fillAmount = (float)currentOffset / (float)maxOffset;
            mask.fillAmount = fillAmount;

            UpdateFillColor();
        }

        private void UpdateFillColor()
        {
            if (fill == null) return;
            fill.color = color;
        }
    }
}