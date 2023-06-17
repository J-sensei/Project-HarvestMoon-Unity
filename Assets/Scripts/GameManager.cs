using StateMachine.Player;
using UnityEngine;
using Utilities;
using SceneTransition;
using GameSave;
using Entity.Enemy;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private PlayerStateMachine player;
    public PlayerStateMachine Player 
    { 
        get 
        {
            // Keep player reference available
            if (player == null)
            {
                GameObject go = GameObject.FindGameObjectWithTag(TagCollection.PLAYER_TAG);
                if(go != null)
                {
                    player = go.GetComponent<PlayerStateMachine>();
                }
            }
            return player;
        } 
    }

    [SerializeField] private TopDownCamera.TopDownCamera _topDownCamera;
    public TopDownCamera.TopDownCamera Camera
    {
        get
        {
            if(_topDownCamera == null)
            {
                //_topDownCamera = UnityEngine.Camera.main.GetComponent<TopDownCamera.TopDownCamera>();
                _topDownCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<TopDownCamera.TopDownCamera>();
            }
            return _topDownCamera;
        }
    }

    public void EnterCombat()
    {
        // Pause enemy
        Enemy[] enemies = GameObject.FindObjectsOfType<Enemy>();
        foreach(Enemy enemy in enemies)
        {
            enemy.Pause = true;
        }

        // TODO: Save current scene && player position
        // Save temp data
        TempSceneData temp = new(SceneTransitionManager.Instance.CurrentLocation, Player.transform.position);
        GameStateManager.Instance.SaveTempData(temp);

        GameMenu.Instance.DisableGameMenu(true);
        GameUIManager.Instance.DisableMenu(true);

        // Go to combat scene
        SceneTransitionManager.Instance.Combat = true;
        SceneTransitionManager.Instance.SwitchScene(SceneLocation.Combat);
    }

    public void ExitCombat()
    {
        // Enable back the UI
        GameMenu.Instance.DisableGameMenu(false);
        GameUIManager.Instance.DisableMenu(false);

        // TODO: Load back the scene and put back the player to original position
        SceneLocation data = GameStateManager.Instance.GetLastLocation();
        SceneTransitionManager.Instance.SwitchScene(data);
    }

    protected override void AwakeSingleton()
    {
        if (player == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag(TagCollection.PLAYER_TAG);
            if (go != null)
            {
                player = go.GetComponent<PlayerStateMachine>();
            }
        }

        if(_topDownCamera == null)
        {
            _topDownCamera = UnityEngine.Camera.main.GetComponent<TopDownCamera.TopDownCamera>();
        }
    }
}
