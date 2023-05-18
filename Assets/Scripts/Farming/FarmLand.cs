using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public class FarmLand : MonoBehaviour
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
            }
        }
    }
}
