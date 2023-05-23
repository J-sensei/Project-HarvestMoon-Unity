using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace GameDateTime
{
    [System.Serializable]
    public struct SkyboxTime
    {
        public Material material;
        public float time;
    }

    /// <summary>
    /// Manage in game time
    /// </summary>
    public class GameTimeManager : Singleton<GameTimeManager>
    {
        [Tooltip("How many second pass to increate the in game time")]
        [SerializeField] private float timeScale = 1f;
        [Tooltip("Set the initial game time")]
        [SerializeField] private GameTime gameTime;
        [Tooltip("Pause the clock tick")]
        [SerializeField] private bool pause = false;
        [SerializeField] private SkyboxTime[] skyboxTimes;

        [Header("Sun")]
        [SerializeField] private Transform sunTransform;
        private float _sunMoveAngle;
        private Quaternion _targetSunAngle;

        List<ITimeChecker> timeCheckerListeners = new();
        /// <summary>
        /// Flag to false when player sleep or its 2am
        /// </summary>
        private bool _updateNewDay = false;

        private SkyboxTime currentSkybox;
        private float skyboxTransitionTime = 1f;
        private float skyboxTimer = 0f;

        protected override void AwakeSingleton()
        {
            gameTime = new(0, Season.Spring, 1, 6, 0); // 6am

            float degreeInHour = 360 / GameTime.MAX_HOUR;
            _sunMoveAngle = degreeInHour / GameTime.MAX_MINUTE;
            if(sunTransform == null)
            {
                sunTransform = GameObject.Find("Directional Light").GetComponent<Transform>();
            }

            StartCoroutine(UpdateTime()); // Start the time update
            sunTransform.rotation = _targetSunAngle; // Rotate the sun instantly
            currentSkybox = skyboxTimes[0];
        }

        private void Update()
        {
            // Smooth rotate the sun (Directional light)
            sunTransform.rotation = Quaternion.Slerp(sunTransform.rotation, _targetSunAngle, Time.deltaTime);
        }

        /// <summary>
        /// Update the game time tick
        /// </summary>
        /// <returns></returns>
        private IEnumerator UpdateTime()
        {
            while (true)
            {
                if(!pause)
                    Tick(); // Put before wait for second to prevent any delay

                yield return new WaitForSeconds(1 / timeScale);
            }
        }
        
        /// <summary>
        /// Increase game time
        /// </summary>
        public void Tick()
        {
            gameTime.IncreaseTime();
            if(gameTime.Hour >= 5 && !_updateNewDay) // 5AM Reset, Player already sleep
            {
                _updateNewDay = true;
                // Inform listeners new day start
                foreach (ITimeChecker listener in timeCheckerListeners)
                {
                    listener.NewDay(gameTime);
                }
            }
            else if(gameTime.Hour == 0 && _updateNewDay) // Auto flag update new day, not neccessary but it can be update the new day if no sleep feature is implemented
            {
                _updateNewDay = false;
            }

            // Inform listeners about the clock time
            foreach(ITimeChecker listener in timeCheckerListeners)
            {
                listener.ClockUpdate(gameTime);
            }

            // Update skybox
            if(skyboxTimes != null)
            {
                foreach(SkyboxTime skyboxTime in skyboxTimes)
                {
                    if(gameTime.Hour == skyboxTime.time && skyboxTime.time != currentSkybox.time)
                    {
                        currentSkybox = skyboxTime;
                        RenderSettings.skybox = currentSkybox.material; // TODO: Make it smoothly change the texture
                        //skyboxTimer = 0;
                        break;
                    }
                }
            }

            //if (skyboxTimer < skyboxTransitionTime) skyboxTimer += Time.deltaTime;
            //RenderSettings.skybox.Lerp(RenderSettings.skybox, currentSkybox.material, skyboxTimer / skyboxTransitionTime);


            UpdateSunTransform(); // Calculate target sun rotation
        }

        /// <summary>
        /// Player sleep
        /// </summary>
        public void Sleep()
        {
            _updateNewDay = false;
            gameTime.Reset();
        }

        private void UpdateSunTransform()
        {
            int minutes = GameTime.HoursToMinutes(gameTime.Hour) + gameTime.Minute;
            float sunAngle = _sunMoveAngle * minutes - 90;

            _targetSunAngle = Quaternion.Euler(sunAngle, 0, 0);
        }

        /// <summary>
        /// Add listner to the time checker when the time is update
        /// </summary>
        /// <param name="listener"></param>
        public void AddListener(ITimeChecker listener)
        {
            timeCheckerListeners.Add(listener);
        }
        /// <summary>
        /// Remove listner to the time checker when the time is update
        /// </summary>
        /// <param name="listener"></param>
        public void RemoveListener(ITimeChecker listener)
        {
            timeCheckerListeners.Remove(listener);
        }

        public void RemoveAllListeners()
        {
            timeCheckerListeners.Clear();
        }

        /// <summary>
        /// Toggle the time pause
        /// </summary>
        public void TogglePause()
        {
            PauseTime(!pause);
        }

        /// <summary>
        /// Pause the game time, prevent it to counting
        /// </summary>
        /// <param name="v"></param>
        public void PauseTime(bool v)
        {
            pause = v;
        }
    }
}
