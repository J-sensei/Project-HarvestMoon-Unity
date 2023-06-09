using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using UI.Tooltip;
using Utilities;

namespace Inventory.UI
{
    // IPointerEnterHandler - will detect mouse enter event to the inventory slot
    // IPointerExitHandler - will detect mouse enter exit to the inventory slot
    // IPointerClickHandler - detect mouse click the inventory

    /// <summary>
    /// An individual item slot of the inventory UI
    /// </summary>
    public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        /// <summary>
        /// Current item in the inventory slot
        /// </summary>
        private ItemData _item;
        protected ItemData Item { get { return _item; } }
        /// <summary>
        /// Unique identifier of the inventory slot to find the corresponding slot in Inventory Manager
        /// </summary>
        private int _id = -1;

        [Header("Configuration")]
        [Tooltip("Does this inventory slot allow player to click and do something")]
        [SerializeField] protected bool mouseEvent = true;
        [Tooltip("Which item type should this slot to correspond to? Invalid for EquipInventorySlot")]
        [SerializeField] private ItemType itemType = ItemType.Tool;

        [Tooltip("Reference to the image responsible to display the item thumbnail")]
        [SerializeField] private Image background;
        [SerializeField] private Image image;
        [Tooltip("Use to search for item thumbnail object is its null")]
        [SerializeField] private string itemThumbnailName = "Item Thumbnail";

        [Header("Color")]
        [SerializeField] private Color idleColor = Color.white;
        [SerializeField] private Color hoverColor = Color.white;
        [SerializeField] private Color activeColor = Color.white;

        [Header("UI Tween")]
        [SerializeField] private float hoverScaleMultiplier = 1.35f;
        [SerializeField] private float tweenDuration = 0.05f;
        private Vector3 _scale;

        private bool _mousePointing = false;

        private void Awake()
        {
            // Try to get the item thumbnail children image reference if its null
            if(image == null)
                image = transform.Find(itemThumbnailName).GetComponent<Image>();

            if (background == null)
                background = GetComponent<Image>();

            _scale = transform.localScale;
        }

        /// <summary>
        /// To update an display item to the inventory slot UI
        /// </summary>
        /// <param name="item"></param>
        public void Display(ItemData item)
        {
            // We want to set the item if its not null and the thumbnail also not null
            if(item != null)
            {
                if (item.thumnail != null)
                    image.sprite = item.thumnail;
                else
                    Debug.LogWarning("[Inventory Slot] Item thumbnail is null!");

                _item = item;
                image.gameObject.SetActive(true);

                // Update the tooltip if the slot is currently having the tooltip showing
                if(_mousePointing)
                {
                    TooltipManager.Instance.Show(_item.description, _item.name);
                }
            }
            else
            {
                _item = null; // As item is null, put it as null
                image.gameObject.SetActive(false); // Active false for the image not to display anything is the item is empty

                // Hide tooltip is showing anything
                if(_mousePointing && TooltipManager.Instance.ShowingTooltip)
                {
                    TooltipManager.Instance.Hide();
                }
            }
        }

        /// <summary>
        /// Set the unique identifier for the inventory slot, as later will use it to take in/out item in InventoryManager
        /// </summary>
        /// <param name="id"></param>
        public void SetId(int id)
        {
            _id = id;
        }

        public void ResetSlotUI()
        {
            transform.localScale = _scale;
            _mousePointing = false;
            background.color = idleColor;
            image.color = idleColor;
        }

        #region Mouse Events
        public void OnPointerEnter(PointerEventData eventData)
        {
            background.DOColor(hoverColor, tweenDuration).SetEase(Ease.Linear);
            image.DOColor(hoverColor, tweenDuration).SetEase(Ease.Linear);
            transform.localScale = _scale * hoverScaleMultiplier;

            if(_item != null)
            {
                TooltipManager.Instance.Show(_item.description, _item.name);
            }
            else
            {
                TooltipManager.Instance.Hide();
            }
            _mousePointing = true;
            AudioManager.Instance.PlaySFX(AudioManager.Instance.menuSelect);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            background.DOColor(idleColor, tweenDuration).SetEase(Ease.Linear);
            image.DOColor(idleColor, tweenDuration).SetEase(Ease.Linear);
            transform.localScale = _scale;

            TooltipManager.Instance.Hide();
            _mousePointing = false;
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (mouseEvent)
                InventoryManager.Instance.Equip(_id, itemType);

            AudioManager.Instance.PlaySFX(AudioManager.Instance.menuClick);
        }
        #endregion
    }
}
