using Inventory.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        protected override void AwakeSingleton()
        {
            if(_tools.Length == 0)
                _tools = new ItemData[toolsSlot];

            if(_items.Length == 0)
                _items = new ItemData[itemSlot];
        }

        [Header("Inventory Slot")]
        [SerializeField] private int toolsSlot = 8;
        [SerializeField] private int itemSlot = 8;

        [Header("Inventory")]
        [SerializeField] private ItemData[] _tools;
        [SerializeField] private ItemData[] _items;

        public ItemData[] Tools { get { return _tools; } }
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

                // Replace inventory to the current holding item
                //_items[slotId] = _holdingItem;
                //if (!PutBackItem(_holdingItem))
                //{
                //    _items[slotId] = null;
                //}

                if (_holdingItem != null && _holdingItem.type != ItemType.Item)
                {
                    PutBackItem(_holdingItem); // Put back to its corresponding inventory
                    _items[slotId] = null; // Take out from the inventory
                }
                else
                {
                    _items[slotId] = _holdingItem;
                }

                // Change holding slot to inventory slot
                _holdingItem = itemToEquip;
            }
            else
            {
                // Cache
                ItemData itemToEquip = InventoryManager.Instance._tools[slotId];

                // Replace inventory to the current holding item
                //_tools[slotId] = _holdingItem;
                //if (!PutBackItem(_holdingItem))
                //{
                //    _tools[slotId] = null;
                //}

                //// As the holding time is store back to its corresponding inventory, should make the tools slot null
                //if(_holdingItem != null && _holdingItem.type != ItemType.Tool)
                //{
                //    _tools[slotId] = null;
                //}

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

        /// <summary>
        /// Unquip the item from player to the inventory
        /// </summary>
        public void Unequip(ItemData item)
        {
            if (item == null) return;
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
