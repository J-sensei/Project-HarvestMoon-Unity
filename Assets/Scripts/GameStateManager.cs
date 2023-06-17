using Farming;
using GameDateTime;
using GameSave;
using SceneTransition;
using System.Collections;
using UnityEngine;
using Utilities;

/// <summary>
/// Update game elements that required ITimeChecker allow to change its state even without playing on the scene
/// </summary>
public class GameStateManager : Singleton<GameStateManager>, ITimeChecker
{
    private static TempSceneData _TempSceneData;
    public static TempSceneData TempSceneData { get { return _TempSceneData; } }
    public bool _hasTempSceneData = false;

    public void SaveTempData(TempSceneData tempSceneData)
    {
        _TempSceneData = tempSceneData;
        _hasTempSceneData = true;
    }

    public TempSceneData LoadTempData()
    {
        if (_hasTempSceneData)
        {
            _hasTempSceneData = false;
            return _TempSceneData;
        }
        else
        {
            return new TempSceneData();
        }
    }

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
            FarmLandSaveManager.UpdateFarmLandState(gameTime);
        }
    }
}
