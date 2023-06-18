using Inventory.UI;
using UI.Tab;
using UI.Tooltip;
using UnityEngine;
using Utilities;
using DG.Tweening;
using GameDateTime;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class GameMenu : Singleton<GameMenu>
{
    [Tooltip("Tab group to switch the tab when game menu is called to open/close")]
    [SerializeField] private TabGroup tabGroup;

    private bool _disableMenu = false;

    /// <summary>
    /// Controls map
    /// </summary>
    private InputControls _inputControls;

    private CanvasGroup _canvasGroup;
    private Vector3 _originalPosition;
    private float _duration = 0.15f;
    private float _tweenMoveDistance = 40f;

    [SerializeField] private UnityEvent onClose;
    protected override void AwakeSingleton()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _originalPosition = transform.position;

        // Binding inventory key
        _inputControls = new();
        _inputControls.UI.Character.started += ToggleCharacter;
        _inputControls.UI.Inventory.started += ToggleInventory;
        _inputControls.UI.Setting.started += ToggleSetting;
        _inputControls.UI.Game.started += ToggleGame;
        _inputControls.UI.Enable();
    }

    private void ToggleCharacter(InputAction.CallbackContext context) => ToggleGameMenu(!gameObject.activeSelf, 0);
    private void ToggleInventory(InputAction.CallbackContext context) => ToggleGameMenu(!gameObject.activeSelf, 1);
    private void ToggleSetting(InputAction.CallbackContext context) => ToggleGameMenu(!gameObject.activeSelf, 2);
    private void ToggleGame(InputAction.CallbackContext context) => ToggleGameMenu(!gameObject.activeSelf, 3);

    public void ToggleGameMenu(bool v, int tabIndex = 0)
    {
        if (_disableMenu) return;
        TooltipManager.Instance.Hide(); // Reset the tooltip tp solve any tooltip ui bug (not active/deactive correctly)
        if (!v) // Close
        {
            InventoryUIManager.Instance.ResetInventorySlots();
            AudioManager.Instance.PlaySFX(AudioManager.Instance.menuClose, 1);
            GameManager.Instance.Player.Enable();
            GameTimeManager.Instance.PauseTime(false); // Resume time ticking
            tabGroup.Close();
            onClose?.Invoke();

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
            tabGroup.SetTab(tabIndex); // 0 => First Tab
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
        ToggleGameMenu(v, 1);
    }

    public void CloseGameMenu() => ToggleGameMenu(false);

    public void InitializeGameMenu()
    {
        TooltipManager.Instance.Hide();
        InventoryUIManager.Instance.ResetInventorySlots();
        GameManager.Instance.Player.Enable();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Disable game menu and prevent player open it
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public void DisableGameMenu(bool v)
    {
        if (v && gameObject.activeSelf)
        {
            ToggleGameMenu(false);
        }

        _disableMenu = v;
    }
}
