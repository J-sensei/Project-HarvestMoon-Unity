using SceneTransition;
using StateMachine.Player;
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
        [SerializeField] private GameMenu gameMenu;
        [SerializeField] private PlayerStateMachine player;
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

            if (player != null && GameManager.Instance.Player == null)
            {
                Transform spawnPos = StartLocationManager.Instance.GetTransform(defaultScene);
                Instantiate(player, spawnPos.position, spawnPos.rotation);
                GameManager.Instance.Camera.UpdateTarget(GameManager.Instance.Player.transform);
            }
        }

        /// <summary>
        /// Ensure everything is properly initialize
        /// </summary>
        public void Ensure()
        {
            if(eventSystem != null && GameObject.FindObjectOfType<EventSystem>() == null)
            {
                Instantiate(eventSystem, Vector3.zero, Quaternion.identity);
            }
        }
    }

}