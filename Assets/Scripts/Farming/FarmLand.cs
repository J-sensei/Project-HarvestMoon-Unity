using GameDateTime;
using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Farming
{
    public enum FarmLandState 
    {
        /// <summary>
        /// Original dirt that are required to hoe before plant
        /// </summary>
        Soil,
        /// <summary>
        /// Dirt land that are ready to plant 
        /// </summary>
        Farmland,
        /// <summary>
        /// Dirt that are watered
        /// </summary>
        Watered
    }

    /// <summary>
    /// A farming land that are able to hoe, water and plant seed on it
    /// </summary>
    public class FarmLand : MonoBehaviour, ITimeChecker
    {
        [SerializeField] FarmLandConfig config;
        [SerializeField] private FarmLandState currentState;
        [Tooltip("Dirt that need to hoe before plant the seed on it")]
        [SerializeField] private GameObject dirt;
        [Tooltip("Dirt that are ready to plant seed on it")]
        [SerializeField] private GameObject hoeDirt;
        [Tooltip("Gameobject to show when the farm land is selected")]
        [SerializeField] private GameObject selectObject;
        private Renderer dirtRenderer;

        public FarmLandState CurrentState { get { return currentState; } }

        private void Awake()
        {
            // Getting the reference based on the game object placement.
            // Missplacement of the prefab will result wrong reference to the variables.

            if(dirt == null)
            {
                dirt = transform.GetChild(0).gameObject;
            }

            if(hoeDirt == null)
            {
                hoeDirt = transform.GetChild(1).gameObject;
            }

            if(selectObject == null)
            {
                selectObject = transform.GetChild(2).gameObject;
            }
        }

        private void Start()
        {
            dirtRenderer = hoeDirt.GetComponent<Renderer>();
            SwitchState(currentState);

            // Add listener to the game time manager to get call when game time is update
            GameTimeManager.Instance.AddListener(this);
        }

        public void SwitchState(FarmLandState state)
        {
            currentState = state;

            // Change the material / game object
            switch (currentState)
            {
                case FarmLandState.Soil:
                    dirt.SetActive(true);
                    hoeDirt.SetActive(false);
                    break;
                case FarmLandState.Farmland:
                    dirt.SetActive(false);
                    hoeDirt.SetActive(true);
                    dirtRenderer.material = config.soil;
                    break;
                case FarmLandState.Watered:
                    dirt.SetActive(false);
                    hoeDirt.SetActive(true);
                    dirtRenderer.material = config.water;
                    break;
            }
        }

        /// <summary>
        /// Show/Hide select effect of the farm land
        /// </summary>
        /// <param name="v">flag</param>
        public void OnSelect(bool v)
        {
            selectObject.SetActive(v);
        }

        /// <summary>
        /// Water the land, if its a farmland
        /// </summary>
        public void Water()
        {
            if(currentState == FarmLandState.Farmland)
            {
                SwitchState(FarmLandState.Watered);
                AudioManager.Instance.PlaySFX(AudioManager.Instance.wateringAudio);
            }
        }

        /// <summary>
        /// Hoe the land, if its a soil
        /// </summary>
        public void Hoe()
        {
            if(currentState == FarmLandState.Soil)
            {
                SwitchState(FarmLandState.Farmland);
                AudioManager.Instance.PlaySFX(AudioManager.Instance.hoeAudio);
            }
        }

        /// <summary>
        /// Check if player using current tool to farm
        /// </summary>
        /// <returns>Is player able to farm the land</returns>
        public bool CheckTool(ToolData.ToolType toolType)
        {
            if (currentState == FarmLandState.Farmland && toolType == ToolData.ToolType.WateringCan)
            {
                return true;
            }
            else if (currentState == FarmLandState.Soil && toolType == ToolData.ToolType.Hoe)
            {
                return true;
            }

            return false;
        }

        #region ITimeChecker
        public void ClockUpdate(GameTime gameTime)
        {

        }

        /// <summary>
        /// Update the farm land status when new day is start (Water dry out, fruit grow up)
        /// </summary>
        /// <param name="gameTime"></param>
        public void NewDay(GameTime gameTime)
        {
            // Make the water dry out in the next day
            if (currentState == FarmLandState.Watered)
            {
                SwitchState(FarmLandState.Farmland); // Back to farm land for the player to water again
            }
        }
        #endregion
    }
}
