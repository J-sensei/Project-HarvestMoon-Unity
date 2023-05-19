using UnityEngine;

namespace Farming
{
    [CreateAssetMenu(fileName = "New Farm Land Config", menuName = "Farm/New Farm Land Config")]
    public class FarmLandConfig : ScriptableObject
    {
        [Tooltip("Material to display when the land is soil")]
        public Material soil;
        [Tooltip("Material to display when the land is watered")]
        public Material water;
    }
}
