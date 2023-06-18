using Combat;
using UnityEngine.EventSystems;

namespace UI.Combat
{
    public class SkipActionButtonUI : CombatActionButtonUI
    {
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                CombatManager.Instance.Skip();      
                CombatUIManager.Instance.ToggleActionUI(false);
            }

            base.OnPointerClick(eventData);
        }
    }
}
