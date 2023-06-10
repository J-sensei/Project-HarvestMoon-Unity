using Inventory.UI;
using UnityEngine;
using Utilities;

namespace Inventory
{
    /// <summary>
    /// Manage all the items available that player holds
    /// </summary>
    public class InventoryManager : Singleton<InventoryManager>
    {
        protected override void AwakeSingleton()
        {
            // If there is no items set, then initialize it all empty
            if (_itemSlots == null || _itemSlots.Length == 0)
                _itemSlots = new ItemSlot[itemSlot];

            for (int i = 0; i < _itemSlots.Length; i++)
            {
                Debug.Log(_itemSlots[i]);
            }
        }

        [Header("Inventory Slot")]
        [Tooltip("Maximum items player can holds")]
        [SerializeField] private int itemSlot = 8;

        [Header("Inventory")]
        [Tooltip("Items stored in the inventory")]
        [SerializeField] private ItemSlot[] _itemSlots;

        /// <summary>
        /// Items that player is holding
        /// </summary>
        public ItemSlot[] ItemSlots { get { return _itemSlots; } }

        /// <summary>
        /// Item holding by the player
        /// </summary>
        [SerializeField] private ItemSlot _holdingItemSlot; // Exposed for debug purposes
        /// <summary>
        /// Item holding by the player
        /// </summary>
        public ItemSlot HoldingItemSlot { get { return _holdingItemSlot; } }

        public ItemData GetHoldingItem()
        {
            if (_holdingItemSlot == null) 
                return null;
            else
                return _holdingItemSlot.ItemData;
        }

        /// <summary>
        /// Equip the item to the player
        /// </summary>
        /// <param name="slotId">Which slot should take out</param>
        /// <param name="itemType">Which type of the slot should be interact</param>
        public void Equip(int slotId, ItemType itemType)
        {
            // Cache
            ItemSlot itemToEquip = InventoryManager.Instance._itemSlots[slotId];
            _itemSlots[slotId] = null; // Take out from the inventory

            // Replace the holding item to the corresponding inventory id if the type is same
            // Else just put it back to corresponding inventory and take the item out from current inventory
            if (_holdingItemSlot != null)
            {
                PutBackItem(_holdingItemSlot); // Put back to its corresponding inventory
            }

            // Change holding slot to inventory slot
            _holdingItemSlot = itemToEquip;

            // Update changes of the UI
            InventoryUIManager.Instance.UpdateInventoryUI();
        }

        /// <summary>
        /// Pickup the item to the holding item slot
        /// </summary>
        /// <param name="item"></param>
        public void Pickup(ItemData item)
        {
            ItemSlot itemSlot = new ItemSlot(item);
            if (_holdingItemSlot != null && _holdingItemSlot.Stackable(itemSlot))
            {
                _holdingItemSlot.Stack(itemSlot);
            }
            else
            {
                // Do somthing with the new item
                PutBackItem(_holdingItemSlot);
                _holdingItemSlot = itemSlot;
            }

            InventoryUIManager.Instance.UpdateInventoryUI();
        }

        /// <summary>
        /// Unload the current holding item
        /// </summary>
        public void Unload()
        {
            if(_holdingItemSlot == null)
            {
                Debug.Log("[Inventory Manager] Cannot unload as holding item is null!");
            }

            _holdingItemSlot = null;
            InventoryUIManager.Instance.UpdateInventoryUI();
        }

        /// <summary>
        /// Check current item type of the current holding item
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool CheckHoldingItemType(ItemType type)
        {
            if (_holdingItemSlot == null) return false; // False if player not holding anything

            if (_holdingItemSlot.ItemData.type == type) // Type is matching
                return true;
            else 
                return false;
        }

        /// <summary>
        /// Unquip the item from player to the inventory
        /// </summary>
        public void Unequip(ItemSlot item)
        {
            if (item == null) return; // If there is nothing in the equip slot, just do nothing
            if (PutBackItem(item))
            {
                _holdingItemSlot = null; // Player are not holding anything if successfully putting the item back to the inventory
            }

            // Update changes of the UI
            InventoryUIManager.Instance.UpdateInventoryUI();
        }

        /// <summary>
        /// Put the item back to the inventory, it will find the first empty slot. Return false if unable to put it back
        /// </summary>
        /// <param name="itemSlot">Is item successfully put into the inventory</param>
        /// <returns></returns>
        private bool PutBackItem(ItemSlot itemSlot)
        {
            if (itemSlot == null) return false;

            for(int i = 0; i < _itemSlots.Length; i++)
            {
                if (_itemSlots[i] != null && _itemSlots[i].Stackable(itemSlot))
                {
                    ItemSlot s = _itemSlots[i].Stack(itemSlot);
                    if (s == null) return true;
                }
                else if(_itemSlots[i] == null)
                {
                    _itemSlots[i] = itemSlot;
                    return true;
                }
            }
            return false;
        }
    }
}
