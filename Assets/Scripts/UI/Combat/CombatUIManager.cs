using UnityEngine;
using Utilities;
using DG.Tweening;

namespace UI.Combat
{
    public class CombatUIManager : Singleton<CombatUIManager>
    {
        [Header("Combat essential UI")]
        [SerializeField] private CanvasGroup actionUI;
        [SerializeField] private CanvasGroup combatTurnUI;
        [SerializeField] private CanvasGroup actionDisplayUI;
        [SerializeField] private float actionUIMove = 40f;

        [Header("Result screen")]
        [SerializeField] private CanvasGroup resutlScreenUI;

        [Header("Tween")]
        [SerializeField] private float tweenDuration = 0.15f;

        private Vector3 _actionUIPos;
        private CombatActionButtonUI[] _actionButtons;

        protected override void AwakeSingleton()
        {
            _actionUIPos = actionUI.transform.localPosition;
            actionUI.gameObject.SetActive(false); // Hide on start

            _actionButtons = actionUI.GetComponentsInChildren<CombatActionButtonUI>();
        }

        public void ToggleActionUI(bool v)
        {
            if (!v)
            {
                // Hide
                actionUI.alpha = 1f;
                actionUI.DOFade(0f, tweenDuration);
                actionUI.transform.DOLocalMove(new Vector3(_actionUIPos.x - actionUIMove, _actionUIPos.y, _actionUIPos.z), tweenDuration).OnComplete(() =>
                {
                    foreach (CombatActionButtonUI ui in _actionButtons)
                    {
                        ui.ResetUI();
                    }
                    actionUI.gameObject.SetActive(false);
                });
            }
            else
            {
                // Show
                actionUI.gameObject.SetActive(true);
                actionUI.alpha = 0f;
                actionUI.DOFade(1f, tweenDuration);
                actionUI.transform.localPosition = new Vector3(_actionUIPos.x - actionUIMove, _actionUIPos.y, _actionUIPos.z);
                actionUI.transform.DOLocalMove(_actionUIPos, tweenDuration);
                AudioManager.Instance.PlaySFX(AudioManager.Instance.actionMenuAppear);
            }
        }
    }

}