using DG.Tweening;
using UI.Tooltip;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    /// <summary>
    /// Clickable UI for character icon at top left character UI
    /// </summary>
    public class CharacterUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private float hoverScaleMultiplier = 1.2f;
        [SerializeField] private float tweenDuration = 0.15f;
        [SerializeField] private Color hoverColor = Color.white;

        private RectTransform _rectTransform;
        private Image _image;
        private CanvasGroup _canvasGroup;
        private Color _color;
        private Vector2 _scale;
        private float _alpha;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _image = GetComponent<Image>();

            _color = _image.color;
            _scale = _rectTransform.localScale;
            _canvasGroup = GetComponent<CanvasGroup>();
            _alpha = _canvasGroup.alpha;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(!GameMenu.Instance.gameObject.activeSelf)
                GameMenu.Instance.ToggleGameMenu(true);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _image.DOColor(hoverColor, tweenDuration).SetEase(Ease.Linear);
            _canvasGroup.alpha = _alpha;
            _canvasGroup.DOFade(1f, tweenDuration).SetEase(Ease.Linear);
            transform.localScale = _scale * hoverScaleMultiplier;

            TooltipManager.Instance.Show("Open character menu");
            AudioManager.Instance.PlaySFX(AudioManager.Instance.menuSelect);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _image.DOColor(_color, tweenDuration).SetEase(Ease.Linear);
            _canvasGroup.alpha = 1f;
            _canvasGroup.DOFade(_alpha, tweenDuration).SetEase(Ease.Linear);
            transform.localScale = _scale;
            TooltipManager.Instance.Hide();
        }
    }
}
