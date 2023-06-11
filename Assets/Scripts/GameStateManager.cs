using Farming;
using GameDateTime;
using SceneTransition;
using System.Collections;
using UnityEngine;
using Utilities;

/// <summary>
/// Update game elements that required ITimeChecker allow to change its state even without playing on the scene
/// </summary>
public class GameStateManager : Singleton<GameStateManager>, ITimeChecker
{
    protected override void AwakeSingleton()
    {
        StartCoroutine(Initialize());
    }

    /// <summary>
    /// Make sure the game time manager is initialized before subcribe to the ITimeChecker
    /// </summary>
    /// <returns></returns>
    private IEnumerator Initialize()
    {
        yield return new WaitForEndOfFrame();
        if(GameTimeManager.Instance != null)
        {
            GameTimeManager.Instance.AddListener(this);
        }
        
    }

    public void ClockUpdate(GameTime gameTime)
    {
        
    }

    public void NewDay(GameTime gameTime)
    {
        // When current scene is not farm, update the farm land data
        if(SceneTransitionManager.Instance.CurrentLocation != SceneLocation.Farm)
        {
            FarmLandSaveManager.Instance.UpdateFarmLandState(gameTime);
        }
    }
}
