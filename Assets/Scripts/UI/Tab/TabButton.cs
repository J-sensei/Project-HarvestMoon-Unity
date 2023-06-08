using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UI.Tooltip;
using Utilities;

namespace UI.Tab
{
    [RequireComponent(typeof(Image))]
    public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        [SerializeField] private TabGroup tabGroup;
        [SerializeField] private Image background;

        [Header("Events")]
        public UnityEvent OnTabSelected;
        public UnityEvent OnTabDeselected;

        [Header("Description")]
        [SerializeField] private string description;

        private void Awake()
        {
            if(tabGroup == null) tabGroup = GetComponentInParent<TabGroup>();
            if (background == null) background = GetComponent<Image>();

            tabGroup.Add(this);
        }

        public void SetSprite(Sprite sprite)
        {
            if(sprite != null)
            {
                background.sprite = sprite;
            }
        }
        public void SetColor(Color color) => background.color = color;

        public void OnPointerClick(PointerEventData eventData)
        {
            tabGroup.OnTabSelect(this);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.menuClick);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            tabGroup.OnTabEnter(this);
            if (!string.IsNullOrEmpty(description))
            {
                TooltipManager.Instance.Show(description);
            }
            AudioManager.Instance.PlaySFX(AudioManager.Instance.menuSelect);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tabGroup.OnTabExit(this);
            if (!string.IsNullOrEmpty(description))
            {
                TooltipManager.Instance.Hide();
            }
        }

        public void Select()
        {
            OnTabSelected?.Invoke();
        }
        public void Deselect()
        {
            OnTabDeselected?.Invoke();
        }
    }
}
