using UnityEngine;

namespace Utilities.Audio
{
    public class BGMPlayer : Singleton<BGMPlayer>
    {
        [SerializeField] private BGMData bgmData;
        /// <summary>
        /// BGM Data contain within this scene
        /// </summary>
        public BGMData BGMData { get { return bgmData; } }

        protected override void AwakeSingleton()
        {
           
        }
    }
}
