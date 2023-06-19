using Combat;
using UnityEngine.EventSystems;

namespace UI.Combat
{
    public class DefenseActionButtonUI : CombatActionButtonUI
    {
        public override void OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Left)
            {
                CombatManager.Instance.PlayerDefense();
                CombatUIManager.Instance.ToggleActionUI(false);
            }

            base.OnPointerClick(eventData);
        }
    }
}
