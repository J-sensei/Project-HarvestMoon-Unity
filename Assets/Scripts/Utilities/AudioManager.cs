using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Manage different audio sources and play audio clips, Singleton so it can be accessing in all the scenes
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Test Audios and Musics")]
        [SerializeField] AudioClip musicClip;

        /// <summary>
        /// Play sound effect audio clip
        /// </summary>
        /// <param name="clip"></param>
        public void PlaySFX(AudioClip clip)
        {
            sfxSource.PlayOneShot(clip);
        }

        public void PlaySFX(AudioSource source, AudioClip clip)
        {
            source.clip = clip;
            source.loop = false;
            source.PlayOneShot(clip);
        }

        public void PlayMusic(AudioClip clip)
        {
            musicSource.clip = clip;
            musicSource.Play();
        }

        private void Start()
        {
            PlayMusic(musicClip); // Test play music at the begining
        }

        protected override void AwakeSingleton()
        {
            // Extra checking incase music or sfx source is null
            if(musicSource == null)
            {
                musicSource = GetComponentInChildren<AudioSource>();
                Debug.LogWarning("[Audio Manager] Music source was null, assigned children component. IS NULL: " + (musicSource == null));
            }

            if(sfxSource == null)
            {
                sfxSource = GetComponentInChildren<AudioSource>();
                Debug.LogWarning("[Audio Manager] SFX source was null, assigned children component. IS NULL: " + (sfxSource == null));
            }
        }
    }

}