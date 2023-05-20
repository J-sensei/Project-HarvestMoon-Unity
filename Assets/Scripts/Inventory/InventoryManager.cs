using Inventory.UI;
using UnityEngine;
using UnityEngine.InputSystem;
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
            if(_tools.Length == 0)
                _tools = new ItemData[toolsSlot];

            if(_items.Length == 0)
                _items = new ItemData[itemSlot];
        }

        [Header("Inventory Slot")]
        [Tooltip("Maximum tools player can holds")]
        [SerializeField] private int toolsSlot = 8;
        [Tooltip("Maximum items player can holds")]
        [SerializeField] private int itemSlot = 8;

        [Header("Inventory")]
        [SerializeField] private ItemData[] _tools;
        [SerializeField] private ItemData[] _items;

        /// <summary>
        /// Tools that player is holding
        /// </summary>
        public ItemData[] Tools { get { return _tools; } }
        /// <summary>
        /// Items that player is holding
        /// </summary>
        public ItemData[] Items { get { return _items; } }

        /// <summary>
        /// Item holding by the player
        /// </summary>
        [SerializeField] private ItemData _holdingItem;
        /// <summary>
        /// Item holding by the player
        /// </summary>
        public ItemData HoldingItem { get { return _holdingItem; } }

        /// <summary>
        /// Equip the item to the player
        /// </summary>
        /// <param name="slotId">Which slot should take out</param>
        /// <param name="itemType">Which type of the slot should be interact</param>
        public void Equip(int slotId, ItemType itemType)
        {
            if(itemType == ItemType.Item)
            {
                // Cache
                ItemData itemToEquip = InventoryManager.Instance._items[slotId];

                // Replace the holding item to the corresponding inventory id if the type is same
                // Else just put it back to corresponding inventory and take the item out from current inventory
                if (_holdingItem != null && _holdingItem.type != ItemType.Item)
                {
                    PutBackItem(_holdingItem); // Put back to its corresponding inventory
                    _items[slotId] = null; // Take out from the inventory
                }
                else
                {
                    _items[slotId] = _holdingItem; // Replace if the item type is same
                }

                // Change holding slot to inventory slot
                _holdingItem = itemToEquip;
            }
            else
            {
                // Cache
                ItemData itemToEquip = InventoryManager.Instance._tools[slotId];


                if (_holdingItem != null && _holdingItem.type != ItemType.Tool)
                {
                    PutBackItem(_holdingItem); // Put back to its own item
                    _tools[slotId] = null;
                }
                else
                {
                    _tools[slotId] = _holdingItem;
                }


                // Change holding slot to inventory slot
                _holdingItem = itemToEquip;
            }

            // Update changes of the UI
            InventoryUIManager.Instance.UpdateInventoryUI();
        }

        public void Pickup(ItemData item)
        {
            // Do somthing with the new item
            PutBackItem(_holdingItem);
            _holdingItem = item;

            InventoryUIManager.Instance.UpdateInventoryUI();
        }

        public void Unload()
        {
            if(_holdingItem == null)
            {
                Debug.Log("[Inventory Manager] Cannot unload as holding item is null!");
            }

            _holdingItem = null;
            InventoryUIManager.Instance.UpdateInventoryUI();
        }

        public bool CheckHoldingItemType(ItemType type)
        {
            if (_holdingItem == null) return false;

            if (_holdingItem.type == type) 
                return true;
            else 
                return false;
        }

        /// <summary>
        /// Unquip the item from player to the inventory
        /// </summary>
        public void Unequip(ItemData item)
        {
            if (item == null) return; // If there is nothing in the equip slot, just do nothing
            if (PutBackItem(item))
            {
                _holdingItem = null; // Player are not holding anything if successfully putting the item back to the inventory
            }

            // Update changes of the UI
            InventoryUIManager.Instance.UpdateInventoryUI();
        }

        /// <summary>
        /// Put the item back to the inventory, it will find the first empty slot. Return false if unable to put it back
        /// </summary>
        /// <param name="item">Is item successfully put into the inventory</param>
        /// <returns></returns>
        private bool PutBackItem(ItemData item)
        {
            if (item == null) return false;
            if(item.type == ItemType.Item)
            {
                for(int i = 0; i < _items.Length; i++)
                {
                    if (_items[i] == null)
                    {
                        _items[i] = item;
                        return true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < _tools.Length; i++)
                {
                    if (_tools[i] == null)
                    {
                        _tools[i] = item;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
