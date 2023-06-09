using Inventory.UI;
using UI.Tab;
using UI.Tooltip;
using UnityEngine;
using Utilities;
using DG.Tweening;
using GameDateTime;

public class GameMenu : Singleton<GameMenu>
{
    [Tooltip("Tab group to switch the tab when game menu is called to open/close")]
    [SerializeField] private TabGroup tabGroup;

    private CanvasGroup _canvasGroup;
    private Vector3 _originalPosition;
    private float _duration = 0.15f;
    private float _tweenMoveDistance = 40f;
    protected override void AwakeSingleton()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _originalPosition = transform.position;
    }

    public void ToggleGameMenu(bool v)
    {
        TooltipManager.Instance.Hide(); // Reset the tooltip tp solve any tooltip ui bug (not active/deactive correctly)
        if (!v) // Close
        {
            InventoryUIManager.Instance.ResetInventorySlots();
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
        else if (v && tabGroup)  // Open
        {
            tabGroup.SetTab(0); // 0 => First Tab
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
        else
        {
            Debug.LogWarning("[Game Menu] Tab Group is null!");
        }
    }

    public void ToggleInventory(bool v)
    {
        ToggleGameMenu(v);
        if (v && tabGroup)
        {
            tabGroup.SetTab(1); // 1 => Inventory Tab
        }
    }

    public void CloseGameMenu()
    {
        TooltipManager.Instance.Hide();
        InventoryUIManager.Instance.ResetInventorySlots();
        GameManager.Instance.Player.Enable();
        gameObject.SetActive(false);
    }
}
