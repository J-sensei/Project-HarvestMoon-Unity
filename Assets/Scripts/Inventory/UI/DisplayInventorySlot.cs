using UnityEngine.EventSystems;

namespace Inventory.UI
{
    /// <summary>
    /// An individual item slot for equip slot to show the current item that player equiping
    /// </summary>
    public class DisplayInventorySlot : InventorySlot
    {
        public override void OnPointerClick(PointerEventData eventData)
        {
            InventoryUIManager.Instance.ToggleInventory();
        }
    }
}
