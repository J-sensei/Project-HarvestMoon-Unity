using System;

namespace Inventory
{
    /// <summary>
    /// An individual item slot
    /// </summary>
    public class ItemSlot
    {
        private const int MAX_QUANTITY = 9; // Maximum quantity for a slot
        private ItemData _itemData;
        private int _quantity;

        public ItemData ItemData { get { return _itemData; } }
        public int Quantity { get { return _quantity; } }

        public ItemSlot(ItemData itemData, int quantity)
        {
            _itemData = itemData;
            _quantity = quantity;
            CheckQuantity();
        }

        public ItemSlot(ItemData itemData)
        {
            _itemData = itemData;
            _quantity = 1;
            CheckQuantity();
        }

        /// <summary>
        /// Check if its same item
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public bool Stackable(ItemSlot slot)
        {
            return _itemData.Equals(slot._itemData) && _quantity < MAX_QUANTITY;
        }

        /// <summary>
        /// Stack into the slot, return back the remain quantity left in slot, null if empty
        /// </summary>
        /// <param name="slot">Slot which to stack into this slot</param>
        /// <returns></returns>
        public ItemSlot Stack(ItemSlot slot)
        {
            // Just in case
            if (!Stackable(slot))
            {
                throw new InvalidOperationException("Slot Item Data are not same");
            }

            while(_quantity < MAX_QUANTITY && slot._quantity > 0)
            {
                _quantity++;
                slot._quantity--;
            }

            if (slot._quantity > 0) return slot;
            else return null;
        }

        /// <summary>
        /// Stack into the item slot
        /// </summary>
        /// <param name="quantity">Amount to add</param>
        public void Add(int quantity = 1)
        {
            _quantity += quantity;
        }

        /// <summary>
        /// Remove the quantity from the item slot
        /// </summary>
        public void Remove(int quantity = 1)
        {
            _quantity -= quantity;
            CheckQuantity();
        }

        /// <summary>
        /// Check and update quantity of the item slot
        /// </summary>
        private void CheckQuantity()
        {
            if (_quantity <= 0 || _itemData == null)
            {
                Empty();
            }
        }

        /// <summary>
        /// Empty the item slot
        /// </summary>
        public void Empty()
        {
            _itemData = null;
            _quantity = 0;
        }
    }
}
