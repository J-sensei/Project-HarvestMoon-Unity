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

        [SerializeField] private bool fade = true;
        public bool Fade { get { return fade; } }

        protected override void AwakeSingleton()
        {
           
        }
    }
}
