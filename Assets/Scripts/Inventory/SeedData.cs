using Farming;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    [System.Serializable]
    public struct GrowData
    {
        [Tooltip("The corresponding day to show the crop grow")]
        public int day;
        [Tooltip("Prefab game object to display on farm land")]
        public GameObject displayPrefab;
    }

    [CreateAssetMenu(fileName = "New Seed", menuName = "Inventory/New Seed Data")]
    /// <summary>
    /// Item data of a seed
    /// </summary>
    public class SeedData : ItemData
    {
        [Header("Crop Grow")]
        [Tooltip("Days required to grow the plant")]
        public int growDay = 1;
        [Tooltip("Define how the crop should grow when update the grow logic, last day should be the harvest day for the plant")]
        public GrowData[] grows;
        [Tooltip("Crop monobehavior object will put on the farm land")]
        public Crop cropPrefab;

        [Header("Yield")]
        [Tooltip("The crop can be collected by the player when the plant is fully grown")]
        public ItemData crop;
    }

}