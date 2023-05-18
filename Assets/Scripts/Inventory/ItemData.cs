using UnityEngine;

namespace Inventory
{
    public enum ItemType
    {
        /// <summary>
        /// Special item where player can equip can do something with it
        /// </summary>
        Tool, 
        /// <summary>
        /// Normal item
        /// </summary>
        Item
    }

    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/New Item Data")]
    /// <summary>
    /// Item data of an item
    /// </summary>
    public class ItemData : ScriptableObject
    {
        public string itemName = "Item";
        public string description = "";
        public ItemType type = ItemType.Tool;

        public Sprite thumnail;
        public GameObject itemPrefab;
    }
}
