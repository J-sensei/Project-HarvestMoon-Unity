using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.PointerEventData;

namespace Inventory.UI
{
    /// <summary>
    /// An individual item slot for equip slot to show the current item that player equiping
    /// </summary>
    public class DisplayInventorySlot : InventorySlot
    {
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != InputButton.Left) return;
            InventoryUIManager.Instance.ToggleInventory();
        }
    }
}
