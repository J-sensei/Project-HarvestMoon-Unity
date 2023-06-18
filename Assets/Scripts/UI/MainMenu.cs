using SceneTransition;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void StartNewGame()
    {
        SceneTransitionManager.Instance.SwitchScene(SceneLocation.Home);
    }
}
