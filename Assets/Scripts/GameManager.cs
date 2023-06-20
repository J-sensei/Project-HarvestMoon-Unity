using StateMachine.Player;
using UnityEngine;
using Utilities;
using SceneTransition;
using GameSave;
using Entity.Enemy;
using Entity;
using Combat;
using System.Linq;

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

    public Enemy[] Enemies
    {
        get
        {
            return GameObject.FindObjectsOfType<Enemy>();
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

    public void PauseEnemies()
    {
        foreach (Enemy enemy in Enemies)
        {
            enemy.Disable();
        }
    }

    public void UnPauseEnemies()
    {
        foreach (Enemy enemy in Enemies)
        {
            enemy.Enable();
        }
    }

    public void EnterCombat()
    {
        // Pause enemy
        PauseEnemies();

        // Get enemies that havnt touches the player
        Vector3[] enemiesPos = Enemies.Where(x => !x.Combat).Select(x => x.transform.position).ToArray();

        // Save temp data
        TempSceneData temp = new(SceneTransitionManager.Instance.CurrentLocation, Player.transform.position, Player.GetComponent<PlayerStatus>().StatusSave, enemiesPos);
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

        // Update player status
        TempSceneData tempData = GameStateManager.Instance.LoadTempData();
        tempData.playerStatus = ((PlayerStatus)CombatManager.Instance.Player.CharacterStatus).StatusSave;
        GameStateManager.Instance.SaveTempData(tempData);

        // TODO: Load back the scene and put back the player to original position
        SceneLocation data = GameStateManager.Instance.GetLastLocation();
        SceneTransitionManager.Instance.SwitchScene(data);
    }

    /// <summary>
    /// GO back to main menu
    /// </summary>
    public void MainMenu()
    {
        GameMenu.Instance.DisableShortcuts();
        GameMenu.Instance.ToggleGameMenu(false); // Makesure game menu are close
        GameStateManager.Instance.StopRecordTime();
        SceneTransitionManager.Instance.DontEnablePlayer = true;
        SceneTransitionManager.Instance.SwitchScene(SceneLocation.MainMenu);
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
