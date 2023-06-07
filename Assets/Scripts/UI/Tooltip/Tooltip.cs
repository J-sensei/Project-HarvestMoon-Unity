using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace UI.Tooltip
{
    [ExecuteInEditMode]
    public class Tooltip : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        [SerializeField] private LayoutElement layoutElement;
        [SerializeField] private int characterWrapLimit;

        [Header("Mouse offset")]
        [SerializeField] private Vector2 pivotOffset = Vector2.zero;

        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            if(layoutElement == null) layoutElement = GetComponent<LayoutElement>();
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Update()
        {
            if (Application.isEditor && titleText && descriptionText && layoutElement)
            {
                int titleLength = titleText.text.Length;
                int descriptionLength = descriptionText.text.Length;

                layoutElement.enabled = (titleLength > characterWrapLimit || descriptionLength > characterWrapLimit);
            }

            Vector2 mPosition = Input.mousePosition;
            float pivotX = mPosition.x / Screen.width;
            float pivotY = mPosition.y / Screen.height;

            _rectTransform.pivot = new Vector2(pivotX + pivotOffset.x, pivotY + pivotOffset.y);
            transform.position = mPosition;
        }

        public void SetText(string description = "", string title = "")
        {
            if (string.IsNullOrEmpty(title))
            {
                titleText.gameObject.SetActive(false);
            }
            else
            {
                titleText.gameObject.SetActive(true);
                titleText.text = title;
            }

            descriptionText.text = description;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _canvasGroup.alpha = 0f;
            _canvasGroup.DOFade(1f, 0.5f);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
