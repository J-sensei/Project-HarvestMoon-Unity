using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        [SerializeField] protected bool mouseEvent = true;
        [SerializeField] private ItemType itemType = ItemType.Tool;

        [Tooltip("Reference to the image responsible to display the item thumbnail")]
        [SerializeField] private Image image;
        [Tooltip("Use to search for item thumbnail object is its null")]
        [SerializeField] private string itemThumbnailName = "Item Thumbnail";
        private void Awake()
        {
            // Try to get the item thumbnail children image reference if its null
            if(image == null)
                image = transform.Find(itemThumbnailName).GetComponent<Image>();
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
            }
            else
            {
                _item = null; // As item is null, put it as null
                image.gameObject.SetActive(false); // Active false for the image not to display anything is the item is empty
            }
        }

        // Event to happen when mouse enter the inventory slot
        public void OnPointerEnter(PointerEventData eventData)
        {
            if(mouseEvent)
                InventoryUIManager.Instance.UpdateItemInfo(_item);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(mouseEvent)
                InventoryUIManager.Instance.UpdateItemInfo(null);
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (mouseEvent)
                InventoryManager.Instance.Equip(_id, itemType);
        }

        public void SetId(int id)
        {
            _id = id;
        }
    }
}
