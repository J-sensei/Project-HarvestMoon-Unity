using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utilities;

namespace Inventory.UI
{
    /// <summary>
    /// Inventory UI manager help to render and interact the inventory of the player (Attached to the Inventory UI game object)
    /// </summary>
    public class InventoryUIManager : Singleton<InventoryUIManager>
    {
        [Header("Inventory System")]
        [Tooltip("Game object that holds multiple inventory slot of tools")]
        [SerializeField] private GameObject toolHolder;
        [Tooltip("Game object that holds multiple inventory slot of items")]
        [SerializeField] private GameObject itemHolder;

        /// <summary>
        /// Tools slots
        /// </summary>
        [SerializeField] private InventorySlot[] _toolSlots;
        /// <summary>
        /// Item slots
        /// </summary>
        [SerializeField] private InventorySlot[] _itemSlots;

        [Tooltip("Slot that show current equip item")]
        [SerializeField] private EquipInventorySlot equipInventorySlot;

        [Header("Item Info")]
        [SerializeField] private TMP_Text itemNameText;
        [SerializeField] private TMP_Text itemDescriptionText;

        protected override void AwakeSingleton()
        {
            _toolSlots = toolHolder.GetComponentsInChildren<InventorySlot>();
            _itemSlots = itemHolder.GetComponentsInChildren<InventorySlot>();

            if(equipInventorySlot == null)
            {
                equipInventorySlot = GetComponentInChildren<EquipInventorySlot>();
            }
        }

        private void Start()
        {
            UpdateInventoryUI();
            for(int i = 0; i < _toolSlots.Length; i++)
            {
                _toolSlots[i].SetId(i);
                _itemSlots[i].SetId(i);
            }
        }

        public void UpdateInventoryUI()
        {
            // Get the item data
            ItemData[] tools = InventoryManager.Instance.Tools;
            ItemData[] items = InventoryManager.Instance.Items;

            // Update the inventory slot using the items data
            UpdateInventory(tools, _toolSlots);
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

        
        private void UpdateInventory(ItemData[] data, InventorySlot[] slots)
        {
            // Assume that item data and slot has same length, this will not give any trouble
            for(int i = 0; i < slots.Length; i++)
            {
                slots[i].Display(data[i]); // Update the item display in the inventory slot
            }
        }


        public void ToggleInventory(bool v)
        {
            // Toggle inventory
            gameObject.SetActive(v);

            if (v)
            {
                UpdateInventoryUI();
            }
        }

        public void UpdateItemInfo(ItemData data)
        {
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
