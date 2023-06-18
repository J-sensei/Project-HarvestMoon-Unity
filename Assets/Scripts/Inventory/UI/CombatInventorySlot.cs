using Combat;
using UI.Combat;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.PointerEventData;

namespace Inventory.UI
{
    /// <summary>
    /// Inventory slot for player to select what item to use in combat
    /// </summary>
    public class CombatInventorySlot : InventorySlot
    {
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != InputButton.Left) return;

            // TODO: Use the item if valid
            if(Item != null)
            {
                CombatManager.Instance.PlayerThrow(Item);
                BattleInventoryManager.Instance.Toggle(false);
                CombatUIManager.Instance.ToggleActionUI(false);
            }
            else
            {
                // TODO: Invalid SFX
                Debug.Log("Item is null!!");
            }
        }
    }
}
