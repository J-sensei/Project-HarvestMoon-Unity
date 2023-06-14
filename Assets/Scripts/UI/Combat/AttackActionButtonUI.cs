using Combat;
using UnityEngine.EventSystems;

namespace UI.Combat
{
    public class AttackActionButtonUI : CombatActionButtonUI
    {
        public override void OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Left)
            {
                CombatManager.Instance.PlayerAttack();
                CombatUIManager.Instance.ToggleActionUI(false);
            }

            base.OnPointerClick(eventData);
        }
    }
}
