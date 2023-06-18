using Inventory.UI;
using UnityEngine.EventSystems;

namespace UI.Combat
{
    public class ItemActionButtonUI : CombatActionButtonUI
    {
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                //TODO: Open UI
                BattleInventoryManager.Instance.Toggle(true);
                
                CombatUIManager.Instance.ToggleActionUI(false);
            }

            base.OnPointerClick(eventData);
        }
    }
}
