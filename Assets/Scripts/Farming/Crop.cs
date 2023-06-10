using Interactable;
using Inventory;
using QuickOutline;
using UnityEngine;
using Utilities;

namespace Farming
{
    public enum CropState
    {
        Grow,
        Harvest,
        /// <summary>
        /// Plant die because not watered it
        /// </summary>
        Wilted
    }

    /// <summary>
    /// A crop to grow on the farm land
    /// </summary>
    public class Crop : MonoBehaviour, IInteractable
    {
        [SerializeField] private SeedData seed;
        [Tooltip("Game objects to represent number of life cycle required to grow the crop")]
        [SerializeField] private GameObject[] cropStateObjects;
        [Tooltip("Game Object to represent when the crop wilted")]
        [SerializeField] private GameObject wiltedCrop;
        [Tooltip("Current day to grow the crop")]
        [SerializeField] private ItemData yieldItem;
        public ItemData YieldItem { get { return yieldItem; } }
        [Tooltip("Growing day")]
        [SerializeField] private int day = 1;
        [SerializeField] private Collider detectCollider;

        [SerializeField] private CropState currentState = CropState.Grow;
        private GrowData[] _growData;
        private GameObject _currentDisplayObject;
        private Outline _outline;

        private void Start()
        {
            if(detectCollider == null)
            {
                detectCollider = GetComponent<BoxCollider>();
            }
            if (currentState != CropState.Harvest) detectCollider.enabled = false; // Prevent player detect the unharvestable crop
        }

        /// <summary>
        /// Initialze to plant to crop
        /// </summary>
        /// <param name="seed"></param>
        /// <param name="transform"></param>
        public void Initialize(SeedData seed, Transform transform)
        {
            this.seed = seed;
            yieldItem = seed.crop;
            cropStateObjects = new GameObject[seed.grows.Length];
            for (int i = 0; i < seed.grows.Length; i++)
            {
                cropStateObjects[i] = seed.grows[i].displayPrefab;
            }

            this.transform.position = transform.position;
            _growData = seed.grows;
        }

        /// <summary>
        /// Update to decide which gameobject to show based on the current state
        /// </summary>
        public void UpdateCropPrefab()
        {
            if (day > _growData.Length) return; // No need to instantiate / destroy the display prefab as plant is fullly grown
            if(currentState == CropState.Wilted)
            {
                InstantiatePlant(wiltedCrop);
                return;
            }

            GameObject go = null;
            for (int i = 0; i < _growData.Length; i++)
            {
                if(day >= _growData[i].day)
                {
                    go = _growData[i].displayPrefab;
                }
                else
                {
                    break;
                }
            }

            InstantiatePlant(go);
        }

        /// <summary>
        /// Instantiate the plant game object for display purposes
        /// </summary>
        /// <param name="go"></param>
        private void InstantiatePlant(GameObject go)
        {
            if(go == null)
            {
                Debug.LogWarning("[Crop] Unable to instantiate null game object. GameObject name: " + name);
                return;
            }

            if(_currentDisplayObject != null)
            {
                Destroy(_currentDisplayObject);
            }
            _currentDisplayObject = Instantiate(go, this.transform);
        }

        /// <summary>
        /// Grow the plant into next state
        /// </summary>
        public void Grow()
        {
            if (currentState == CropState.Wilted) return;

            day++;
            UpdateCropPrefab();
            if(day >= _growData[_growData.Length - 1].day)
            {
                currentState = CropState.Harvest; // Crop are ready to harvest
                detectCollider.enabled = true; // Allow player to detect the crop to harvest it
                _outline = _currentDisplayObject.GetComponent<Outline>(); // Get the outline for the current display object
                StartCoroutine(OutlineHelper.InitializeOutline(_outline));
            }
        }

        public void Wilt()
        {
            currentState = CropState.Wilted;
            detectCollider.enabled = false;
            UpdateCropPrefab();
        }

        /// <summary>
        /// When the plant is ready to harvest
        /// </summary>
        private void Harvest()
        {
            if (seed.regrowable)
            {
                day -= seed.dayToRegrow;
                currentState = CropState.Grow; // Back to grow state
                detectCollider.enabled = false; // Disable not allow player to detect the crop to harvest it
                UpdateCropPrefab(); // Update the display game object
            }
            else
            {
                // Destroy the object if its not regrowable
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// When player interact with fully grown plant
        /// </summary>
        public void Interact()
        {
            InventoryManager.Instance.Pickup(YieldItem); // Get the yield item
            Harvest(); // Then harvest the crop
        }

        public InteractableType GetInteractableType()
        {
            return InteractableType.Crop;
        }
        public void OnSelect(bool v)
        {
            if (currentState == CropState.Harvest && _outline != null)
                _outline.enabled = v;
        }
    }
}