using Farming;
using GameDateTime;
using GameSave;
using SceneTransition;
using System;
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
    private bool _hasTempSceneData = false;
    public bool HasTempSceneData { get { return _hasTempSceneData; } }
    private float _playTime = 0f;
    public float PlayTime
    {
        get
        {
            return _playTime;
        }
    }
    public string PlayTimeString
    {
        get
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(_playTime);

            return ((int)timeSpan.TotalHours).ToString("D2") + ":" + timeSpan.Minutes.ToString("D2") + ":" + timeSpan.Seconds.ToString("D2");
        }
    }
    public static string GetPlayTimeString(float playtime)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(playtime);

        return ((int)timeSpan.TotalHours).ToString("D2") + ":" + timeSpan.Minutes.ToString("D2") + ":" + timeSpan.Seconds.ToString("D2");
    }

    public Action OnPlayTimeRecord { get; set; }
    private Coroutine _recordTimeCoroutine;

    private IEnumerator RecordTimeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            _playTime += 1;
            OnPlayTimeRecord?.Invoke();
            Debug.Log("Playtime: " + PlayTimeString);
        }
    }

    public void StopRecordTime()
    {
        if(_recordTimeCoroutine != null)
        {
            StopCoroutine(_recordTimeCoroutine);
        }
    }

    public void RecordTime()
    {
        _playTime = 0f;
        if(_recordTimeCoroutine != null)
        {
            StopCoroutine(_recordTimeCoroutine);
        }
        _recordTimeCoroutine = StartCoroutine(RecordTimeRoutine());
    }

    public void LoadRecordTime(float time)
    {
        _playTime = time;
        if (_recordTimeCoroutine != null)
        {
            StopCoroutine(_recordTimeCoroutine);
        }
        StartCoroutine(RecordTimeRoutine());
    }

    public void SaveTempData(TempSceneData tempSceneData)
    {
        _TempSceneData = tempSceneData;
        _hasTempSceneData = true;
    }

    public SceneLocation GetLastLocation()
    {
        return _TempSceneData.location;
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
    /// To ensure game state manager is subcribe to
    /// </summary>
    public void Ensure()
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

        // Only happen when game fresh start
        if(GameTimeManager.Instance != null && !GameTimeManager.Instance.ExistListener(this))
        {
            Debug.Log("initialize gamestatemanager");
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
