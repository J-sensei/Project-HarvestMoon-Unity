using GameDateTime;
using UI.UIScreen;
using UnityEngine;
using Utilities;
using QuickOutline;
using Inventory;

namespace Interactable
{
    public class Bed : MonoBehaviour, IInteractable
    {
        [SerializeField] private Outline outline;

        private bool _sleeping = false;

        private void Start()
        {
            if(outline == null)
            {
                outline = GetComponent<Outline>();
            }
            outline.enabled = false;

            // For some reason this can make the setting applied
            StartCoroutine(OutlineHelper.InitializeOutline(outline));
        }

        #region IInteractable
        public void OnSelect(bool v)
        {
            outline.enabled = v;
        }

        public InteractableType GetInteractableType()
        {
            return InteractableType.Environmental;
        }

        public void Interact()
        {
            if (_sleeping) return; // Prevent player spamming bed to cause weird bahvior when sleeping

            _sleeping = true; // Player start to sleep

            // Callback when fade first started
            FadeScreenManager.Instance.FadePanel.OnStart.AddListener(() =>
            {
                GameManager.Instance.Player.Disable(); // Prevent player moving when sleeping
                GameTimeManager.Instance.PauseTime(true); // Pause the time
            });

            // Update the date time when fade out is finish
            FadeScreenManager.Instance.FadePanel.OnFinish.AddListener(() =>
            {
                GameTimeManager.Instance.Sleep(); // Called sleep to past the date time to next day
            });

            FadeScreenManager.Instance.FadePanel.FadeOutIn(() =>
            {
                // Callback when fade in is finish
                GameTimeManager.Instance.PauseTime(false); // Unpause the time
                GameManager.Instance.Player.Enable(); // Allow player to move after finish sleep
                _sleeping = false; // Player finish sleep
            });
        }

        public ItemData GetItemData()
        {
            return null;
        }
        #endregion
    }
}
