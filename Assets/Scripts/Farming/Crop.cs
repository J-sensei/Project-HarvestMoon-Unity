using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Farming
{
    public enum CropState
    {
        Grow,
        Harvest
    }

    /// <summary>
    /// A crop to grow on the farm land
    /// </summary>
    public class Crop : MonoBehaviour
    {
        [SerializeField] private SeedData seed;
        [Tooltip("Game objects to represent number of life cycle required to grow the crop")]
        [SerializeField] private GameObject[] cropStateObjects;
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

        //public Crop(SeedData seed, Transform transform)
        //{
        //    this.seed = seed;
        //    yieldItem = seed.crop;
        //    cropStateObjects = new GameObject[seed.grows.Length];
        //    for(int i = 0; i < seed.grows.Length; i++)
        //    {
        //        cropStateObjects[i] = seed.grows[i].displayPrefab;
        //    }

        //    this.transform.position = transform.position;
        //    _growData = seed.grows;
        //}

        private void Start()
        {
            if(detectCollider == null)
            {
                detectCollider = GetComponent<BoxCollider>();
            }
            if (currentState != CropState.Harvest) detectCollider.enabled = false;
        }

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

        public void UpdateCropPrefab()
        {
            if (day > _growData.Length) return; // No need to instantiate / destroy the display prefab as plant is fullly grown

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


        private void InstantiatePlant(GameObject go)
        {
            if(go == null)
            {
                Debug.LogWarning("[Crop] Unable to instantiate null game object");
                return;
            }

            if(_currentDisplayObject != null)
            {
                Destroy(_currentDisplayObject);
            }
            _currentDisplayObject = Instantiate(go, this.transform);
        }

        public void Grow()
        {
            day++;
            UpdateCropPrefab();
            if(day >= _growData[_growData.Length - 1].day)
            {
                currentState = CropState.Harvest; // Crop are ready to harvest
                detectCollider.enabled = true; // Allow player to detect the crop to harvest it
                _outline = _currentDisplayObject.GetComponent<Outline>();
            }
        }

        public void OnSelect(bool v)
        {
            if (currentState == CropState.Harvest)
                _outline.enabled = v;
        }

        /// <summary>
        /// When the plant is ready to harvest
        /// </summary>
        public void Harvest()
        {
            // Just destroy the object
            Destroy(gameObject);
        }


    }

}