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
        [Header("Details")]
        [Tooltip("Unique Identifier of the item, make sure its not -1 and unique from other items")]
        public int id = -1;
        [Tooltip("Name of the item")]
        public string itemName = "Item";
        [Tooltip("Description of the item")]
        public string description = "";
        [Tooltip("Type of the item, use for inventory checking and item use case checking when player is trying to using it")]
        public ItemType type = ItemType.Tool;

        [Header("Render")]
        [Tooltip("Thumbnail to display in the inventory")]
        public Sprite thumnail;
        [Tooltip("3D Model of the object for the player to equip on hand")]
        public GameObject itemPrefab;

        [Header("Item Properties")]
        [Tooltip("Can this item stack with same item?")]
        public bool stacktable = true;
        [Tooltip("Will this item descrease the quantity after consumed")]
        public bool consumable = false;
    }
}
