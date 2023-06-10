using GameDateTime;
using UnityEngine;

namespace Environment
{
    public class Lamp : MonoBehaviour, ITimeChecker
    {
        [SerializeField] private GameObject[] lightSources;

        private void Start()
        {
            // Add listener to the game time manager to get call when game time is update
            GameTimeManager.Instance.AddListener(this);
        }

        #region ITimeChecker
        public void ClockUpdate(GameTime gameTime)
        {
            if((gameTime.Hour >= 19 && gameTime.Hour <= 23) || (gameTime.Hour >= 0 && gameTime.Hour <= 8))
            {
                foreach(GameObject lightSource in lightSources)
                {
                    if(lightSource != null)
                        lightSource.SetActive(true);
                }
            }
            else
            {
                foreach (GameObject lightSource in lightSources)
                {
                    if (lightSource != null)
                        lightSource.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Update the farm land status when new day is start (Water dry out, fruit grow up)
        /// </summary>
        /// <param name="gameTime"></param>
        public void NewDay(GameTime gameTime)
        {

        }
        #endregion
    }
}
