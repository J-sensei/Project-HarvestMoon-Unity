using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Utilities;

namespace UI.Combat
{
    public class CombatActionButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [SerializeField] private Image background;
        [SerializeField] private Color idleColor = Color.white;
        [SerializeField] private Color hoverColor = Color.white;
        [SerializeField] private Color clickColor = Color.white;

        [Header("TWeen")]
        [SerializeField] private float tweenDuration = 0.15f;

        private Vector3 _originalPos;
        private bool _hovering;

        private void Awake()
        {
            _originalPos = background.transform.localPosition;    
        }

        public void ResetUI()
        {
            background.color = idleColor;
            background.transform.localPosition = _originalPos;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            background.transform.DOLocalMove(Vector3.zero, tweenDuration);
            background.DOColor(hoverColor, tweenDuration);
            _hovering = true;
            AudioManager.Instance.PlaySFX(AudioManager.Instance.menuSelect, 1);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            background.transform.DOLocalMove(_originalPos, tweenDuration);
            background.DOColor(idleColor, tweenDuration);
            _hovering = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Left)
            {
                background.DOColor(clickColor, tweenDuration);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if(_hovering)
                background.DOColor(hoverColor, tweenDuration);
            else
                background.DOColor(idleColor, tweenDuration);
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.menuClick, 1);
        }
    }

}