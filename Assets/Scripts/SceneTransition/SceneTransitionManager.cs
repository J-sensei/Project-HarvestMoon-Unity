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
        private SceneLocation _currentLocation;
        private List<GameObject> _holdingObjects = new();
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
            FadeScreenManager.Instance.Loading(true);
            FadeScreenManager.Instance.FadePanel.FadeOut(() =>
            {
                StartCoroutine(LoadSceneAsync(location.ToString()));
            });
            AddHoldObject(GameManager.Instance.Player.transform);
            //SceneManager.LoadScene(location.ToString());
            GameManager.Instance.Player.Disable();

            //AsyncOperation operation = null;
            //FadeScreenManager.Instance.FadePanel.FadeDuration = 1f;
            //FadeScreenManager.Instance.FadePanel.OnStart.AddListener(() =>
            //{
            //    FadeScreenManager.Instance.Loading(true);
            //    GameManager.Instance.Player.Disable();
            //    //operation = SceneManager.LoadSceneAsync(location.ToString(), LoadSceneMode.Additive);
            //    //operation.allowSceneActivation = false;
            //});

            //FadeScreenManager.Instance.FadePanel.FadeOut(() =>
            //{
            //    StartCoroutine(LoadSceneAsync(location.ToString()));
            //});
        }

        //private IEnumerator WaitLevelLoaded(AsyncOperation operation)
        //{

        //    // Loading finish
        //    GameManager.Instance.Player.Enable();
        //    FadeScreenManager.Instance.Loading(false);
        //    operation.allowSceneActivation = true;
        //}

        IEnumerator LoadSceneAsync(string sceneName)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            //operation.allowSceneActivation = false;
            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                //Debug.Log("Scene Loading Progress: " + progress * 100 + "%");
                yield return null;
            }

            //GameManager.Instance.Player.Enable();
            //FadeScreenManager.Instance.Loading(false);
            //operation.allowSceneActivation = true;
        }

        public void AddHoldObject(Transform tr)
        {
            tr.parent = transform;
            _holdingObjects.Add(tr.gameObject);
        }

        public void OnLocationLoad(Scene scene, LoadSceneMode mode)
        {
            // Fade screen on start
            if (FadeScreenManager.Instance.FadePanel.FadeOnStart)
            {
                FadeScreenManager.Instance.FadePanel.OnStart.AddListener(() => FadeScreenManager.Instance.Loading(false));
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
                _holdingObjects[i].transform.parent = null;
                _holdingObjects[i].transform.position = startPoint.position;
            }
            _holdingObjects.Clear();
            GameManager.Instance.Camera.UpdateTarget(GameManager.Instance.Player.transform);
            StartCoroutine(EnablePlayer());
            _currentLocation = location;
        }

        private IEnumerator EnablePlayer()
        {
            yield return new WaitForSeconds(0.5f);
            GameManager.Instance.Player.Enable();
            GameTimeManager.Instance.PauseTime(false);
        }
    }
}
