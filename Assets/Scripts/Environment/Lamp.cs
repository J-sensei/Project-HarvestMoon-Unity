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

        private void OnDestroy()
        {
            // Make sure remove the ITimeChecker listener to prevent any null reference exception
            GameTimeManager.Instance.RemoveListener(this);
        }

        #region ITimeChecker
        public void ClockUpdate(GameTime gameTime)
        {
            // 5pm to 11pm or 12am to 7am
            if((gameTime.Hour >= 17 && gameTime.Hour <= 23) || (gameTime.Hour >= 0 && gameTime.Hour <= 7))
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
