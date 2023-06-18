using Entity;
using GameDateTime;
using GameSave;
using Item;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI.GameSave;
using UI.UIScreen;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;
using Utilities.Audio;

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
        /// Determine if scene transition to go to battle scene
        /// </summary>
        public bool Combat { get; set; } = false;
        public bool DontEnablePlayer { get; set; } = false;

        [Header("Indoor locations")]
        [SerializeField] private SceneLocation[] indoorLocations = { SceneLocation.Home };
        /// <summary>
        /// Check if player is currently indoor
        /// </summary>
        public bool Indoor
        {
            get
            {
                return indoorLocations.Contains(_currentLocation);
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

            if(GameTimeManager.Instance != null)
                GameTimeManager.Instance.PauseTime(true); // Pause the time when loading scene

            FadeScreenManager.Instance.FadePanel.FadeDuration = 0.5f;
            // Fade out transition
            FadeScreenManager.Instance.FadePanel.FadeOut(() =>
            {
                FadeScreenManager.Instance.Loading(true); // Scene loading start
                StartCoroutine(LoadSceneAsync(location.ToString())); // Start to load the scene after fade finish
            });

            if(!Combat && GameManager.Instance.Player != null)
                AddHoldObject(GameManager.Instance.Player.transform); // Add player instance to move it to other scene (DontDestroyObject)

            if(GameManager.Instance.Player != null)
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
            if(GameStateManager.Instance != null)
            {
                GameStateManager.Instance.Ensure();
            }

            // Request Load
            if (GameLoadSlot.RequestLoad)
            {
                GameLoadSlot.RequestLoad = false;
                GameLoadSlot.RequestLoadFile(GameLoadSlot.LoadFilename);
            }

            if (!Combat)
            {
                if (gameInitializer != null)
                    gameInitializer.Ensure(); // To ensure game initialize properly
                else
                    Debug.LogWarning("[Screen Transition Manager] Game Initializer is null");
            }

            // Fade screen on start
            if (FadeScreenManager.Instance.FadePanel.FadeOnStart)
            {
                FadeScreenManager.Instance.FadePanel.FadeDuration = 0.5f;
                FadeScreenManager.Instance.FadePanel.OnStart.AddListener(() => FadeScreenManager.Instance.Loading(false)); // Scene loading start
                FadeScreenManager.Instance.FadePanel.FadeIn(() => {
                    if (GameTimeManager.Instance != null)
                        GameTimeManager.Instance.LoadSunTransform();
                    else
                        Debug.LogWarning("[Scene Transition Manager] Game Time Manager is null");
                });
            }

            // Change BGM
            if (BGMPlayer.Instance != null && BGMPlayer.Instance.BGMData != null)
            {
                AudioManager.Instance.PlayMusic(BGMPlayer.Instance.BGMData, BGMPlayer.Instance.Fade);
            }

            // Load scene location
            SceneLocation oldLocation = _currentLocation;
            SceneLocation location = (SceneLocation)Enum.Parse(typeof(SceneLocation), scene.name);
            if (oldLocation == location) return; // If location same then no need to do anything


            Transform startPoint = null;
            if (StartLocationManager.Instance != null)
            {
                startPoint = StartLocationManager.Instance.GetTransform(oldLocation);
            }
            else
            {
                Debug.LogWarning("[Screen Transition Manager] Start Location Manager is null");
            }

            // Change player position unload it
            if (startPoint != null)
            {
                for (int i = 0; i < _holdingObjects.Count; i++)
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
            }
            else
            {
                Debug.LogWarning("[Scene Transition Manager] Start Point is null and not able to find in this scene: " + location.ToString());
            }

            _holdingObjects.Clear();

            if (!Combat)
            {
                if (GameStateManager.Instance.HasTempSceneData)
                {
                    TempSceneData data = GameStateManager.Instance.LoadTempData();
                    var player = gameInitializer.SpawnPlayer(data.playerPosition);
                    player.Disable();
                    player.GetComponent<PlayerStatus>().Load(data.playerStatus);

                    GameStateManager.Instance.HasTempSceneData = false;
                }

                // Set Camera
                if(GameManager.Instance.Camera != null)
                    GameManager.Instance.Camera.UpdateTargetAndInitialize(GameManager.Instance.Player.transform); // Set Camera, TODO: Dont run this if enter to combat scene
                // Set Game time
                if (GameTimeManager.Instance != null)
                {
                    GameTimeManager.Instance.PauseTime(false); // Resume the time pause

                    if(!DontEnablePlayer)
                        StartCoroutine(EnablePlayer());
                }
            }

            _currentLocation = location;

            if (GameTimeManager.Instance != null)
                GameTimeManager.Instance.LoadSunTransform(); // Update sun transform

            Combat = false;
            DontEnablePlayer = false;
        }

        /// <summary>
        /// Enable player back to control
        /// </summary>
        /// <returns></returns>
        private IEnumerator EnablePlayer()
        {
            yield return new WaitForSeconds(0.1f);
            GameManager.Instance.Player.Enable();
            //Debug.Log("Player Pos: " + GameManager.Instance.Player.transform.position);
        }
    }
}
