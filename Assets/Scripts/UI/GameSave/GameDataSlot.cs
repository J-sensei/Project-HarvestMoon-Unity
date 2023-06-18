using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;
using DG.Tweening;

namespace UI.GameSave
{
    public abstract class GameDataSlot : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        [SerializeField] private Color idleColor = Color.white;
        [SerializeField] private Color hoverColor = Color.white;
        [SerializeField] private float tweenDuration = 0.15f;

        [SerializeField] private Image image;
        private void Awake()
        {
            if(image == null)
                image = GetComponent<Image>();
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.menuClick, 1);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            image.DOColor(hoverColor, tweenDuration);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.menuSelect, 1);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            image.DOColor(idleColor, tweenDuration);
        }

        public void Reset()
        {
            if(image != null)
                image.color = idleColor;
        }
    }
}
