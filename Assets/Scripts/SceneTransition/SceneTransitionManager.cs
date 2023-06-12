using GameDateTime;
using Item;
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
        [Header("Scene Transition Manager")]
        [Tooltip("Ensure game is properly initialize every scene load")]
        [SerializeField] private GameInitializer gameInitializer;
        [Tooltip("Which scene location belong to in this scene (Refer to scene name)")]
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

        /// <summary>
        /// Explictly set the scene location (Only use in load game function)
        /// </summary>
        /// <param name="scene"></param>
        public void SetSceneLocation(SceneLocation scene) => _currentLocation = scene;

        protected override void AwakeSingleton()
        {
            SceneManager.sceneLoaded += OnLocationLoad;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnLocationLoad;
        }

        /// <summary>
        /// Switch to the scene to the location available in the game
        /// </summary>
        /// <param name="location"></param>
        public void SwitchScene(SceneLocation location)
        {
            // In case there is any left over pickable item set to DontDestroy (Player hold before)
            // Need to destroy to prevent it stay change player switch scene
            PickableItem[] items = GameObject.FindObjectsOfType<PickableItem>();
            foreach(PickableItem item in items)
            {
                Destroy(item.gameObject);
            }

            GameTimeManager.Instance.PauseTime(true); // Pause the time when loading scene
            // Fade out transition
            FadeScreenManager.Instance.FadePanel.FadeOut(() =>
            {
                FadeScreenManager.Instance.Loading(true); // Scene loading start
                StartCoroutine(LoadSceneAsync(location.ToString())); // Start to load the scene after fade finish
            });
            AddHoldObject(GameManager.Instance.Player.transform); // Add player instance to move it to other scene (DontDestroyObject)
            GameManager.Instance.Player.Disable();
        }

        /// <summary>
        /// Load the scene asynchronys
        /// </summary>
        /// <param name="sceneName">Name of the scene (Refer to scene project folder and inside the build settings)</param>
        /// <returns></returns>
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

        /// <summary>
        /// Make the object as dont destroy as assign it sa child of the transition manager and detach it after scene is loaded
        /// </summary>
        /// <param name="tr"></param>
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
                //Debug.Log("Transform: " + _holdingObjects[i].transform.position + " Old Location: " + oldLocation.ToString() + " Start Pos: " + startPoint.position);
                _holdingObjects[i].transform.position = startPoint.position;
                _holdingObjects[i].transform.rotation = startPoint.rotation;
                _holdingObjects[i].transform.parent = null;
            }
            _holdingObjects.Clear();
            GameManager.Instance.Camera.UpdateTargetAndInitialize(GameManager.Instance.Player.transform);
            GameTimeManager.Instance.PauseTime(false); // Resume the time pause

            StartCoroutine(EnablePlayer());
            _currentLocation = location;
        }
        private IEnumerator EnablePlayer()
        {
            yield return new WaitForSeconds(0.1f);
            GameManager.Instance.Player.Enable();
            //Debug.Log("Player Pos: " + GameManager.Instance.Player.transform.position);
        }
    }
}
