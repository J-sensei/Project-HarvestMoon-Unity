using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

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

        private void Start()
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
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            tabGroup.OnTabEnter(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tabGroup.OnTabExit(this);
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
