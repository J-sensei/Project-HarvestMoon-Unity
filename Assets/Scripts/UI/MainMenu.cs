using SceneTransition;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using UI;
using GameDateTime;
using Farming;
using Inventory;
using Inventory.UI;
using UI.GameSave;

public class MainMenu : MonoBehaviour
{
    [Header("Container")]
    [SerializeField] private CanvasGroup mainMenuContainer;
    [SerializeField] private CanvasGroup settingContainer;
    [SerializeField] private CanvasGroup loadGameContainer;

    [Header("Reset Target")]
    [SerializeField] private GameButton settingBackButton;
    [SerializeField] private GameButton loadGameBackButton;
    [SerializeField] private GameLoadSlot[] loadSlots;

    [Header("Tween")]
    [SerializeField] private float tweenDuration = 0.15f;

    private void Awake()
    {
        StartCoroutine(Initialize());

        for(int i = 0; i < loadSlots.Length; i++)
        {
            loadSlots[i].SetFilename("Save" + (i + 1).ToString());
        }
    }

    private IEnumerator Initialize()
    {
        yield return new WaitForEndOfFrame();
        settingContainer.gameObject.SetActive(false);
        loadGameContainer.gameObject.SetActive(false);
    }

    public void StartNewGame()
    {
        // Reset
        // Only happen when player already play the game and comeback to main menu from game
        if (GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.PauseTime(true);
            GameTimeManager.Instance.Reset();
        }
        if(InventoryManager.Instance != null)
        {
            InventoryManager.Instance.Reset();
            InventoryUIManager.Instance.UpdateInventoryUI();
        }

        if(GameMenu.Instance != null)
        {
            GameMenu.Instance.EnableShortcuts();
        }
        FarmLandSaveManager.Reset();
        GameStateManager.Instance.RecordTime();
        
        SceneTransitionManager.Instance.SwitchScene(SceneLocation.Home);
    }

    public void OpenSetting()
    {
        settingContainer.gameObject.SetActive(true);
        settingContainer.alpha = 0f;
        settingContainer.DOFade(1f, tweenDuration);

        mainMenuContainer.alpha = 1f;
        mainMenuContainer.DOFade(0f, tweenDuration).OnComplete(() =>
        {
            mainMenuContainer.gameObject.SetActive(false);
        });
    }

    public void CloseSetting()
    {
        settingContainer.alpha = 1f;
        settingContainer.DOFade(0f, tweenDuration).OnComplete(() => settingContainer.gameObject.SetActive(false));

        mainMenuContainer.gameObject.SetActive(true);
        mainMenuContainer.alpha = 0f;
        mainMenuContainer.DOFade(1f, tweenDuration);
        settingBackButton?.ResetUI();
    }

    public void OpenLoadGame()
    {
        loadGameContainer.gameObject.SetActive(true);
        loadGameContainer.alpha = 0f;
        loadGameContainer.DOFade(1f, tweenDuration);

        mainMenuContainer.alpha = 1f;
        mainMenuContainer.DOFade(0f, tweenDuration).OnComplete(() =>
        {
            mainMenuContainer.gameObject.SetActive(false);
        });

        for(int i = 0; i < loadSlots.Length; i++)
        {
            loadSlots[i].LoadSaveDetails();
        }
    }

    public void CloseLoadGame()
    {
        loadGameContainer.alpha = 1f;
        loadGameContainer.DOFade(0f, tweenDuration).OnComplete(() => loadGameContainer.gameObject.SetActive(false));

        mainMenuContainer.gameObject.SetActive(true);
        mainMenuContainer.alpha = 0f;
        mainMenuContainer.DOFade(1f, tweenDuration);

        for (int i = 0; i < loadSlots.Length; i++)
        {
            loadSlots[i].Reset();
        }
        loadGameBackButton?.ResetUI();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
