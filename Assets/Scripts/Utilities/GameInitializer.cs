using SceneTransition;
using StateMachine.Player;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Help to initialize the game properly
    /// </summary>
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField] private SceneLocation defaultScene = SceneLocation.Home;
        [SerializeField] private GameMenu gameMenu;
        [SerializeField] private PlayerStateMachine player;

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
                Instantiate(player, StartLocationManager.Instance.GetTransform(defaultScene).position, Quaternion.identity);
                GameManager.Instance.Camera.UpdateTarget(GameManager.Instance.Player.transform);
            }
        }
    }

}