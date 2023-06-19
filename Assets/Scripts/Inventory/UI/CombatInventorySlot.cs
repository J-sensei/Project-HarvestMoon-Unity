using Combat;
using UI.Combat;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.PointerEventData;
using DG.Tweening;
using UI.Tooltip;
using Utilities;

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
                InventoryManager.Instance.Consume(ID);
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

        public override void OnPointerEnter(PointerEventData eventData)
        {
            Background.DOColor(HoverColor, TweenDuration).SetEase(Ease.Linear);
            Image.DOColor(HoverColor, TweenDuration).SetEase(Ease.Linear);
            transform.localScale = _scale * HoverScaleMultiplier;

            if (Item != null)
            {
                TooltipManager.Instance.Show("Damage: " + Item.damage.ToString(), Item.name);
            }
            else
            {
                TooltipManager.Instance.Hide();
            }
            _mousePointing = true;
            AudioManager.Instance.PlaySFX(AudioManager.Instance.menuSelect);
        }
    }
}
