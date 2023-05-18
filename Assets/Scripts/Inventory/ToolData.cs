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
        /// <summary>
        /// Type of the tool
        /// </summary>
        public enum ToolType
        {
            /// <summary>
            /// Use to tilling the farm land
            /// </summary>
            Hoe, 
            /// <summary>
            /// Use to water the farm land
            /// </summary>
            WateringCan, 
            /// <summary>
            /// Use to chop the tree
            /// </summary>
            Axe, 
            /// <summary>
            /// Use to mine the stone / ore
            /// </summary>
            Pickaxe
        }

        [Tooltip("Tool type use for checking when player is trying to using it")]
        public ToolType toolType;
    }

}