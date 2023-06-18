using DG.Tweening;
using SceneTransition;
using UI.GameSave;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Menu for game content menu in the game menu
    /// </summary>
    public class GameContentMenu : MonoBehaviour
    {
        [Header("Sub Contents")]
        [SerializeField] private CanvasGroup masterView;
        [SerializeField] private CanvasGroup loadGameView;

        [Header("Items")]
        [SerializeField] private GameButton[] gameButtons;
        [SerializeField] private GameButton loadViewCloseButton;
        [SerializeField] private GameDataSlot[] saveSlots;

        [Header("Tween")]
        [SerializeField] private float tweenDuration = 0.15f;

        private void Awake()
        {
            loadGameView.gameObject.SetActive(false);
            for(int i = 0; i < saveSlots.Length; i++)
            {
                saveSlots[i].SetFilename("Save" + (i + 1).ToString());
            }
        }

        public void OpenMasterView()
        {
            masterView.gameObject.SetActive(true);
            masterView.alpha = 0f;
            masterView.DOFade(1f, tweenDuration);
            loadGameView.alpha = 1f;
            loadGameView.DOFade(0f, tweenDuration).OnComplete(() =>
            {
                loadGameView.gameObject.SetActive(false);
            });

            ResetLoadGameView();
        }

        public void OpenLoadGameView()
        {
            masterView.alpha = 1f;
            masterView.DOFade(0f, tweenDuration).OnComplete(() =>
            {
                masterView.gameObject.SetActive(false);
            });

            loadGameView.gameObject.SetActive(true);
            loadGameView.alpha = 0f;
            loadGameView.DOFade(1f, tweenDuration);

            // TODO: Load preview save
            for (int i = 0; i < saveSlots.Length; i++)
            {
                saveSlots[i].LoadSaveDetails();
            }

            ResetMasterView();
        }

        public void Reset()
        {
            ResetMasterView();
            ResetLoadGameView();
        }

        public void ResetMasterView()
        {
            for(int i = 0; i < gameButtons.Length; i++)
            {
                if(gameButtons[i] != null)
                    gameButtons[i].ResetUI();
            }
        }

        public void ResetLoadGameView()
        {
            for (int i = 0; i < saveSlots.Length; i++)
            {
                if(saveSlots[i] != null)
                    saveSlots[i].Reset();
            }

            if(loadGameView != null)
                loadViewCloseButton.ResetUI();
        }

        public void MainMenu()
        {
            GameMenu.Instance.DisableShortcuts();
            GameMenu.Instance.ToggleGameMenu(false); // Makesure game menu are close
            GameStateManager.Instance.StopRecordTime();
            SceneTransitionManager.Instance.DontEnablePlayer = true;
            SceneTransitionManager.Instance.SwitchScene(SceneLocation.MainMenu);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
