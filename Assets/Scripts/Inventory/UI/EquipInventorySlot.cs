using UnityEngine.EventSystems;
using Utilities;

namespace Inventory.UI
{
    /// <summary>
    /// An individual item slot for equip slot to show the current item that player equiping
    /// </summary>
    public class EquipInventorySlot : InventorySlot
    {
        public override void OnPointerClick(PointerEventData eventData)
        {
            if(this.mouseEvent)
                InventoryManager.Instance.Unequip(this.ItemSlot);

            AudioManager.Instance.PlaySFX(AudioManager.Instance.menuClick);
        }
    }
}
