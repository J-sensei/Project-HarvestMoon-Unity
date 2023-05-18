using UnityEngine;

namespace Farming
{
    [CreateAssetMenu(fileName = "New Farm Land Config", menuName = "Farm/New Farm Land Config")]
    public class FarmLandConfig : ScriptableObject
    {
        public Material soil;
        public Material water;
    }
}
