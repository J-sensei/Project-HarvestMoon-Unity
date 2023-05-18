using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(fileName = "New Seed", menuName = "Inventory/New Seed Data")]
    /// <summary>
    /// Item data of a seed
    /// </summary>
    public class SeedData : ItemData
    {
        [Tooltip("Days required to grow the plant")]
        public int growDay = 1;

        [Header("Yield")]
        [Tooltip("The crop can be collected by the player when the plant is fully grown")]
        public ItemData crop;
    }

}