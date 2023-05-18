using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(fileName = "New Tool", menuName = "Inventory/New Tool Data")]
    /// <summary>
    /// Item data of a tool
    /// </summary>
    public class ToolData : ItemData
    {
        public enum ToolType
        {
            Hoe, WateringCan, Axe, Pickaxe
        }

        public ToolType toolType;
    }

}