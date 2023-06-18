using SceneTransition;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using UI;
using GameDateTime;
using Farming;
using Inventory;
using Inventory.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Container")]
    [SerializeField] private CanvasGroup mainMenuContainer;
    [SerializeField] private CanvasGroup settingContainer;


    [Header("Reset Target")]
    [SerializeField] private GameButton settingBackButton;

    [Header("Tween")]
    [SerializeField] private float tweenDuration = 0.15f;

    private void Awake()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        yield return new WaitForEndOfFrame();
        settingContainer.gameObject.SetActive(false);
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

    public void QuitGame()
    {
        Application.Quit();
    }
}
