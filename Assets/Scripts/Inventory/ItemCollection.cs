using UnityEngine;

namespace Inventory
{
    /// <summary>
    /// Database for item to retrieve it back
    /// </summary>
    [CreateAssetMenu(fileName = "New Item Collection", menuName = "Inventory/New Item Collection")]
    public class ItemCollection : ScriptableObject
    {
        [Tooltip("Items available in the game")]
        public ItemData[] items;
    }
}
