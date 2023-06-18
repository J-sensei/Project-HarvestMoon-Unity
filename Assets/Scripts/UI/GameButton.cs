using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using Utilities;

namespace UI
{
    public class GameButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("Reference")]
        [SerializeField] private Image background;

        [Header("Color")]
        [SerializeField] private Color idleColor = Color.white;
        [SerializeField] private Color hoverColor = Color.white;
        [SerializeField] private Color pressedColor = Color.white;

        [Header("Tween")]
        [SerializeField] private float tweenDuration = 0.15f;
        [SerializeField] private float scaleMultiplier = 1.2f;

        [Header("Event")]
        [SerializeField] private UnityEvent onClick;
        
        private void Awake()
        {
            if(background == null)
            {
                background = GetComponent<Image>();
            }
        }

        private void OnDestroy()
        {
            DOTween.Kill(background);
            DOTween.Kill(transform);
        }

        public void ResetUI()
        {
            background.color = idleColor;
            transform.localScale = Vector3.one;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Left)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.menuClick, 1);
                onClick?.Invoke();
            }
            //background.DOColor(pressedColor, tweenDuration);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            background?.DOColor(hoverColor, tweenDuration);
            transform?.DOScale(Vector3.one * scaleMultiplier, tweenDuration);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.menuSelect, 1);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            background?.DOColor(idleColor, tweenDuration);
            transform?.DOScale(Vector3.one, tweenDuration);
        }
    }

}