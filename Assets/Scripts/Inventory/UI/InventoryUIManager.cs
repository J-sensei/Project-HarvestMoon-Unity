using TMPro;
using UnityEngine;
using Utilities;
using UnityEngine.InputSystem;

namespace Inventory.UI
{
    /// <summary>
    /// Inventory UI manager help to render and interact the inventory of the player (Attached to the Inventory UI game object)
    /// </summary>
    public class InventoryUIManager : Singleton<InventoryUIManager>
    {
        [Header("Inventory System")]
        [Tooltip("Game object that holds multiple inventory slot of tools (Deprecated, put all item in item holder)")]
        [SerializeField] private GameObject toolHolder;
        [Tooltip("Game object that holds multiple inventory slot of items")]
        [SerializeField] private GameObject itemHolder;

        /// <summary>
        /// Tools slots
        /// </summary>
        private InventorySlot[] _toolSlots;
        /// <summary>
        /// Item slots
        /// </summary>
        private InventorySlot[] _itemSlots;

        [Tooltip("Slot that show current equip item")]
        [SerializeField] private EquipInventorySlot equipInventorySlot;

        [Header("Item Info")]
        [Tooltip("Text to display the item name")]
        [SerializeField] private TMP_Text itemNameText;
        [Tooltip("Text to display the item description")]
        [SerializeField] private TMP_Text itemDescriptionText;

        /// <summary>
        /// Controls map
        /// </summary>
        private InputControls _inputControls;

        protected override void AwakeSingleton()
        {
            // Initialize the slots
            //_toolSlots = toolHolder.GetComponentsInChildren<InventorySlot>();
            _itemSlots = itemHolder.GetComponentsInChildren<InventorySlot>();

            if(equipInventorySlot == null)
            {
                equipInventorySlot = GetComponentInChildren<EquipInventorySlot>();
            }

            // Binding inventory key
            _inputControls = new();
            _inputControls.UI.Inventory.started += ToggleInventoryUI;
            _inputControls.UI.Inventory.Enable();
        }

        /// <summary>
        /// Toggle the invnetory UI to show the player
        /// </summary>
        /// <param name="context"></param>
        private void ToggleInventoryUI(InputAction.CallbackContext context)
        {
            Instance.ToggleInventory();
        }

        private void OnDestroy()
        {
            // Delete inventory UI controls if the InventoryUIManager is destroyed
            _inputControls.UI.Inventory.started -= ToggleInventoryUI;
            _inputControls.UI.Inventory.Disable();
        }

        private void Start()
        {
            UpdateInventoryUI();
            for(int i = 0; i < _itemSlots.Length; i++)
            {
                //_toolSlots[i].SetId(i);
                _itemSlots[i].SetId(i);
            }
            ToggleInventory(false);
        }

        /// <summary>
        /// Update the inventory UI to show the correct data to the player
        /// </summary>
        public void UpdateInventoryUI()
        {
            // Get the item data
            //ItemData[] tools = InventoryManager.Instance.Tools;
            ItemData[] items = InventoryManager.Instance.Items;

            // Update the inventory slot using the items data
            //UpdateInventory(tools, _toolSlots);
            UpdateInventory(items, _itemSlots);

            // Update Equip Item
            ItemData equipItem = InventoryManager.Instance.HoldingItem;
            GameUIManager.Instance.UpdateEquipedItem(equipItem);

            if(equipInventorySlot != null)
            {
                equipInventorySlot.Display(equipItem);
            }
            else
            {
                Debug.LogWarning("[Inventory UI Manager] Missing Equip Inventory Slot, please assign it!");
            }
        }

        
        /// <summary>
        /// Update inventory slots in the inventory UI
        /// </summary>
        /// <param name="data"></param>
        /// <param name="slots"></param>
        private void UpdateInventory(ItemData[] data, InventorySlot[] slots)
        {
            // Assume that item data and slot has same length, this will not give any trouble
            Debug.Log(data + "    " + slots);
            for(int i = 0; i < slots.Length; i++)
            {
                slots[i].Display(data[i]); // Update the item display in the inventory slot
            }
        }

        /// <summary>
        /// Show / Hide the inventory
        /// </summary>
        /// <param name="v"></param>
        public void ToggleInventory(bool v)
        {
            // Toggle inventory
            GameMenu.Instance.ToggleGameMenu(v);

            if (v)
            {
                UpdateInventoryUI();
            }
        }

        public void ToggleInventory()
        {
            ToggleInventory(!GameMenu.Instance.gameObject.activeSelf);
        }

        /// <summary>
        /// Update the item information when user if hovering it
        /// </summary>
        /// <param name="data"></param>
        public void UpdateItemInfo(ItemData data)
        {
            return;
            if(data == null)
            {
                itemNameText.text = "";
                itemDescriptionText.text = "";
                return;
            }

            if(itemNameText != null)
            {
                itemNameText.text = data.itemName;
            }
            else
            {
                Debug.LogWarning("[Inventory UI Manager] Item name text is null");
            }

            if (itemDescriptionText != null)
            {
                itemDescriptionText.text = data.description;
            }
            else
            {
                Debug.LogWarning("[Inventory UI Manager] Item description text is null");
            }
        }
    }
}
