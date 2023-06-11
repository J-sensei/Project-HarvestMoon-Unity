using GameDateTime;
using System;
using System.Collections;
using System.Collections.Generic;
using UI.UIScreen;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace SceneTransition
{
    public class SceneTransitionManager : Singleton<SceneTransitionManager>
    {
        [Tooltip("Ensure game is properly initialize every scene load")]
        [SerializeField] private GameInitializer gameInitializer;

        [SerializeField] private SceneLocation _currentLocation = SceneLocation.Farm;
        public SceneLocation CurrentLocation { get { return _currentLocation; } }
        private List<GameObject> _holdingObjects = new();
        private AsyncOperation _operation;
        public List<GameObject> HoldingObjects { 
            get
            {
                return _holdingObjects;
            } 
        }

        protected override void AwakeSingleton()
        {
            SceneManager.sceneLoaded += OnLocationLoad;
        }

        public void SwitchScene(SceneLocation location)
        {
            GameTimeManager.Instance.PauseTime(true);
            FadeScreenManager.Instance.FadePanel.FadeOut(() =>
            {
                FadeScreenManager.Instance.Loading(true); // Scene loading start
                StartCoroutine(LoadSceneAsync(location.ToString()));
            });
            AddHoldObject(GameManager.Instance.Player.transform);
            GameManager.Instance.Player.Disable();
        }


        IEnumerator LoadSceneAsync(string sceneName)
        {
            _operation = SceneManager.LoadSceneAsync(sceneName);
            _operation.allowSceneActivation = true;
            while (!_operation.isDone)
            {
                float progress = Mathf.Clamp01(_operation.progress / 0.9f);
                yield return null;
            }
        }

        public void AddHoldObject(Transform tr)
        {
            tr.parent = transform;
            _holdingObjects.Add(tr.gameObject);
        }

        public void OnLocationLoad(Scene scene, LoadSceneMode mode)
        {
            gameInitializer.Ensure(); // To ensure game initialize properly

            // Fade screen on start
            if (FadeScreenManager.Instance.FadePanel.FadeOnStart)
            {
                FadeScreenManager.Instance.FadePanel.OnStart.AddListener(() => FadeScreenManager.Instance.Loading(false)); // Scene loading start
                FadeScreenManager.Instance.FadePanel.FadeIn();
            }
            GameTimeManager.Instance.LoadSunTransform();

            // Load scene location
            SceneLocation oldLocation = _currentLocation;
            SceneLocation location = (SceneLocation)Enum.Parse(typeof(SceneLocation), scene.name);

            if (oldLocation == location) return; // If location same then no need to do anything

            // Change player position unload it
            Transform startPoint = StartLocationManager.Instance.GetTransform(oldLocation);
            for(int i = 0; i < _holdingObjects.Count; i++)
            {
                if (_holdingObjects[i] == null || _holdingObjects[i].transform == null)
                {
                    Debug.Log("[Scene Transition Manager] Holding Object (" + _holdingObjects[i].name + "::" + i + ") is null");
                    continue;
                }
                //Debug.Log("Transform: " + _holdingObjects[i].transform + " Old Location: " + oldLocation.ToString());
                _holdingObjects[i].transform.position = startPoint.position;
                _holdingObjects[i].transform.rotation = startPoint.rotation;
                _holdingObjects[i].transform.parent = null;
            }
            _holdingObjects.Clear();
            //Debug.Log(GameManager.Instance.Player);
            //Debug.Log(GameManager.Instance.Camera);
            GameManager.Instance.Camera.UpdateTargetAndInitialize(GameManager.Instance.Player.transform);
            GameTimeManager.Instance.PauseTime(false); // Resume the time pause

            StartCoroutine(EnablePlayer());
            _currentLocation = location;
        }

        private IEnumerator EnablePlayer()
        {
            yield return new WaitForSeconds(0.1f);
            GameManager.Instance.Player.Enable();
        }
    }
}
