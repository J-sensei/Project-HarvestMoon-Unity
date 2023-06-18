using DG.Tweening;
using GameDateTime;
using UI.Tooltip;
using UnityEngine;
using Utilities;

namespace UI.GameSave
{
    public class GameSaveUI : Singleton<GameSaveUI>
    {
        private CanvasGroup _canvasGroup;
        private Vector3 _originalPosition;
        [Tooltip("Tween move animation distance")]
        [SerializeField] private float _tweenMoveDistance = 40f;
        [Tooltip("Tween duration")]
        [SerializeField] private float _duration = 0.15f;
        [SerializeField] private GameSaveSlot[] saveSlots;
        protected override void AwakeSingleton()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _originalPosition = transform.position;

            for (int i = 0; i < saveSlots.Length; i++)
            {
                saveSlots[i].SetFilename("Save" + (i + 1).ToString()); // Load the save details
            }
        }

        public void ToggleGameMenu(bool v)
        {
            TooltipManager.Instance.Hide(); // Reset the tooltip tp solve any tooltip ui bug (not active/deactive correctly)
            if (!v) // Close
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.menuClose, 1);
                GameManager.Instance.Player.Enable();
                GameTimeManager.Instance.PauseTime(false); // Resume time ticking

                // Tween
                Vector3 pos = _originalPosition;
                pos.y -= _tweenMoveDistance;

                transform.DOMove(pos, _duration);

                _canvasGroup.alpha = 1f;
                _canvasGroup.DOFade(0f, _duration).OnComplete(() =>
                {
                    TooltipManager.Instance.Hide(); // Reset the tooltip to solve any tooltip ui bug (not active/deactive correctly)
                    gameObject.SetActive(v);
                });
            }
            else // Open
            {
                for(int i = 0; i < saveSlots.Length; i++)
                {
                    saveSlots[i].LoadSaveDetails(); // Load the save details
                }

                AudioManager.Instance.PlaySFX(AudioManager.Instance.menuOpen, 1);
                GameManager.Instance.Player.Disable();
                GameTimeManager.Instance.PauseTime(true); // Stop time ticking

                // Tween
                gameObject.SetActive(v);
                Vector3 pos = _originalPosition;
                pos.y += _tweenMoveDistance;
                transform.position = pos;

                transform.DOMove(_originalPosition, _duration);

                _canvasGroup.alpha = 0f;
                _canvasGroup.DOFade(1f, _duration);
            }
        }
    }
}
