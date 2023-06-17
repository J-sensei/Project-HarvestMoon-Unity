using SceneTransition;
using StateMachine.Player;
using UI.GameSave;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utilities
{
    /// <summary>
    /// Help to initialize the game properly
    /// </summary>
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField] private SceneLocation defaultScene = SceneLocation.Home;

        [Header("Initialize Objects")]
        [Header("UI")]
        [Tooltip("Intiialize Game Menu")]
        [SerializeField] private GameMenu gameMenu;
        [SerializeField] private GameSaveUI gameSaveUI;

        [Header("OTher")]
        [Tooltip("Initialize Player object")]
        [SerializeField] private PlayerStateMachine player;
        [Tooltip("Initialize event system")]
        [SerializeField] private EventSystem eventSystem;

        private void Start()
        {
            if(gameMenu != null && !gameMenu.gameObject.activeSelf)
            {
                gameMenu.gameObject.SetActive(true);

                // Audio
                AudioSetting[] audioSettins = gameMenu.gameObject.GetComponentsInChildren<AudioSetting>();
                foreach(AudioSetting audioSetting in audioSettins)
                {
                    audioSetting.InitializeVolume();
                }
            }

            if(gameSaveUI != null && !gameSaveUI.gameObject.activeSelf)
            {
                gameSaveUI.gameObject.SetActive(true);
                gameSaveUI.gameObject.SetActive(false);
            }

            // Spawn player to the scene
            if (player != null && GameManager.Instance.Player == null)
            {
                Transform spawnPos = StartLocationManager.Instance.GetTransform(defaultScene);
                Instantiate(player, spawnPos.position, spawnPos.rotation);
                GameManager.Instance.Camera.UpdateTargetAndInitialize(GameManager.Instance.Player.transform);
            }

            // Incase
            Ensure();
        }

        /// <summary>
        /// Ensure everything is properly initialize
        /// </summary>
        public void Ensure()
        {
            if (eventSystem != null && GameObject.FindObjectOfType<EventSystem>() == null)
            {
                Instantiate(eventSystem, Vector3.zero, Quaternion.identity);
            }

            if (GameManager.Instance.Player == null)
            {
                Debug.Log("[Game Initializer] Trying to reintialize player:" + GameManager.Instance.Player);
            }

            if (GameManager.Instance.Camera == null)
            {
                Debug.Log("[Game Initializer] Trying to reintialize main camera:" + GameManager.Instance.Camera);
            }
        }
    }

}