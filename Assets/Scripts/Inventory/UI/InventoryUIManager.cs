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
        //[Tooltip("Game object that holds multiple inventory slot of tools (Deprecated, put all item in item holder)")]
        //[SerializeField] private GameObject toolHolder;
        [Tooltip("Game object that holds multiple inventory slot of items")]
        [SerializeField] private GameObject itemHolder;

        /// <summary>
        /// Tools slots
        /// </summary>
        //private InventorySlot[] _toolSlots;
        /// <summary>
        /// Item slots
        /// </summary>
        private InventorySlot[] _itemSlots;

        [Tooltip("Slot that show current equip item")]
        [SerializeField] private EquipInventorySlot equipInventorySlot;

        protected override void AwakeSingleton()
        {
            // Initialize the slots
            //_toolSlots = toolHolder.GetComponentsInChildren<InventorySlot>();
            _itemSlots = itemHolder.GetComponentsInChildren<InventorySlot>();

            if(equipInventorySlot == null)
            {
                equipInventorySlot = GetComponentInChildren<EquipInventorySlot>();
            }
        }

        //private void OnDestroy()
        //{
        //    // Delete inventory UI controls if the InventoryUIManager is destroyed
        //    _inputControls.UI.Inventory.started -= ToggleInventoryUI;
        //    _inputControls.UI.Inventory.Disable();
        //}

        //private void OnEnable()
        //{
        //    _inputControls.UI.Inventory.started += ToggleInventoryUI;
        //    _inputControls.UI.Inventory.Enable();
        //}

        //private void OnDisable()
        //{
        //    _inputControls.UI.Inventory.started -= ToggleInventoryUI;
        //    _inputControls.UI.Inventory.Disable();
        //}

        private void Start()
        {
            UpdateInventoryUI();
            for(int i = 0; i < _itemSlots.Length; i++)
            {
                //_toolSlots[i].SetId(i);
                _itemSlots[i].SetId(i);
            }
            //ToggleInventory(false);
            GameMenu.Instance.InitializeGameMenu();
        }

        /// <summary>
        /// Update the inventory UI to show the correct data to the player
        /// </summary>
        public void UpdateInventoryUI()
        {
            // Get the item data
            //ItemData[] tools = InventoryManager.Instance.Tools;
            ItemSlot[] items = InventoryManager.Instance.ItemSlots;

            // Update the inventory slot using the items data
            //UpdateInventory(tools, _toolSlots);
            UpdateInventory(items, _itemSlots);

            // Update Equip Item
            ItemSlot equipItem = InventoryManager.Instance.HoldingItemSlot;
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
        private void UpdateInventory(ItemSlot[] data, InventorySlot[] slots)
        {
            // Assume that item data and slot has same length, this will not give any trouble
            //Debug.Log(data + "    " + slots);
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
            GameMenu.Instance.ToggleInventory(v);

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
        /// Reset the inventory slots to prevent any ui bug
        /// </summary>
        public void ResetInventorySlots()
        {
            foreach(InventorySlot slot in _itemSlots)
            {
                slot.ResetSlotUI();
            }
            equipInventorySlot.ResetSlotUI();
        }
    }
}
