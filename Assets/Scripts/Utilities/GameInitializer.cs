using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Help to initialize the game properly
    /// </summary>
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField] private GameMenu gameMenu;

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
        }
    }

}