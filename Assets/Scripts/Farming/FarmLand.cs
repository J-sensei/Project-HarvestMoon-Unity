using GameDateTime;
using Interactable;
using Inventory;
using UnityEngine;
using Utilities;
using QuickOutline;

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
    public class FarmLand : MonoBehaviour, ITimeChecker, IInteractable
    {
        [Header("Configuration")]
        [SerializeField] FarmLandConfig config;
        [SerializeField] private FarmLandState currentState;
        [Tooltip("Dirt that need to hoe before plant the seed on it")]
        [SerializeField] private GameObject dirt;
        [Tooltip("Dirt that are ready to plant seed on it")]
        [SerializeField] private GameObject hoeDirt;
        [Tooltip("Gameobject to show when the farm land is selected")]
        [SerializeField] private GameObject selectObject;
        [SerializeField] private Outline outline;
        private Renderer _dirtRenderer;

        [Header("Yield")]
        [Tooltip("Determine if any crop is grow on it, empty if its null")]
        [SerializeField] private Crop crop;
        [SerializeField] private Transform cropTransform;

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

            if(cropTransform == null)
            {
                cropTransform = transform.GetChild(3);
            }

            if (outline == null) outline = GetComponent<Outline>();
        }

        private void Start()
        {
            _dirtRenderer = hoeDirt.GetComponent<Renderer>();
            SwitchState(currentState);

            StartCoroutine(OutlineHelper.InitializeOutline(outline));

            // Add listener to the game time manager to get call when game time is update
            GameTimeManager.Instance.AddListener(this);
        }

        /// <summary>
        /// Change the state of the farm land
        /// </summary>
        /// <param name="state"></param>
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
                    _dirtRenderer.material = config.soil;
                    break;
                case FarmLandState.Watered:
                    dirt.SetActive(false);
                    hoeDirt.SetActive(true);
                    _dirtRenderer.material = config.water;
                    break;
            }
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
        /// Remove any crop in the farmland
        /// </summary>
        public void RemoveCrop()
        {
            if(crop != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.plantRemoveAudio);
                Destroy(crop.gameObject);
            }
        }

        /// <summary>
        /// Plant crop into the land
        /// </summary>
        /// <param name="seedData">Seed that are going to plant</param>
        /// <returns></returns>
        public bool Plant(SeedData seedData)
        {
            if(currentState != FarmLandState.Soil && crop == null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.plantAudio);
                crop = Instantiate(seedData.cropPrefab, cropTransform);
                crop.Initialize(seedData, cropTransform);
                crop.UpdateCropPrefab();
                return true;
            }
            else
            {
                Debug.Log("[Farm Land] Not able to plant as condition are not valid");
                return false;
            }
        }

        /// <summary>
        /// Check if the farmland is ready to plant the crop
        /// </summary>
        /// <returns></returns>
        public bool CanPlant()
        {
            return currentState != FarmLandState.Soil;
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
                // If crop is available, do something with it
                // Only want to grow the crop if player remember to water it
                if (crop != null)
                {
                    crop.Grow();
                }

                SwitchState(FarmLandState.Farmland); // Back to farm land for the player to water again
            }
            else if(currentState != FarmLandState.Watered)
            {
                if (crop != null)
                {
                    crop.Wilt();
                }
            }
        }
        #endregion

        #region IInteractable
        public void OnSelect(bool v)
        {
            selectObject.SetActive(v);
            if(outline != null)
            {
                outline.enabled = v;
            }
        }
        public void Interact()
        {
            // TODO: Find something to put it here      
        }

        public InteractableType GetInteractableType()
        {
            return InteractableType.Farm;
        }
        #endregion
    }
}
