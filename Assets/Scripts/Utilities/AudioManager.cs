using UnityEngine;
using Utilities.Audio;

namespace Utilities
{
    /// <summary>
    /// Manage different audio sources and play audio clips, Singleton so it can be accessing in all the scenes
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        [Header("Audio Sources")]
        [Tooltip("Audio Source for BGM")]
        [SerializeField] private AudioSource[] bgmSources;
        /// <summary>
        /// Number to toggle between index for bgm sources
        /// </summary>
        private int _audioToggle = 0;
        [Tooltip("Audio Source for SFX")]
        [SerializeField] private AudioSource[] sfxSources;

        [Header("Background Music")]
        [SerializeField] private float musicStartTime = 0f;
        [SerializeField] private float musicEndTime = 100f;
        public float initialSkip = 160;
        [SerializeField] private AudioClip musicClip;
        [SerializeField] private BGMData[] bgmList;

        [Header("SFX Audios")]
        [Header("Farming Audio")]
        [SerializeField] public AudioClip wateringAudio;
        [SerializeField] public AudioClip hoeAudio;
        [SerializeField] public AudioClip plantAudio;
        [SerializeField] public AudioClip plantRemoveAudio;

        [Header("UI")]
        [Header("Menu Audio")]
        [SerializeField] public AudioClip menuOpen;
        [SerializeField] public AudioClip menuClose;
        [SerializeField] public AudioClip menuSelect;
        [SerializeField] public AudioClip menuClick;

        private double _musicDuration;
        private double _goalTime = 0;

        /// <summary>
        /// Play sound effect audio clip on channel 0
        /// </summary>
        /// <param name="clip"></param>
        public void PlaySFX(AudioClip clip) => PlaySFXClip(clip, 0);

        /// <summary>
        /// Player sound effect audio clip on target channel providing that target audio source is exist
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="channel"></param>
        public void PlaySFX(AudioClip clip, int channel)
        {
            if(channel < 0 || channel >= sfxSources.Length)
            {
                Debug.LogWarning("[Audio Manager] Invalid channel index: " + channel);
                return;
            }

            PlaySFXClip(clip, channel);
        }

        private void PlaySFXClip(AudioClip clip, int channel)
        {

            sfxSources[channel].clip = clip;
            sfxSources[channel].loop = false;
            sfxSources[channel].Play();
        }

        public void PlayMusic(AudioClip clip)
        {
            _goalTime = AudioSettings.dspTime;
            bgmSources[0].clip = clip;
            bgmSources[0].PlayScheduled(_goalTime);
            bgmSources[0].time = initialSkip;

            _musicDuration = (double)clip.samples / clip.frequency;
            _goalTime = _goalTime + (musicEndTime - initialSkip);
            bgmSources[0].SetScheduledEndTime(_goalTime);

            _audioToggle = 1 - _audioToggle;
        }

        private void Start()
        {
            PlayMusic(musicClip); // Test play music at the begining
        }


        private void Update()
        {
            if(AudioSettings.dspTime > _goalTime - 1)
            {
                PlayScheduleClip();
            }
        }

        private void PlayScheduleClip()
        {
            Debug.Log("[Audio Manager] Schedule loop");
            bgmSources[_audioToggle].clip = musicClip;
            bgmSources[_audioToggle].PlayScheduled(_goalTime);
            bgmSources[_audioToggle].time = musicStartTime;

            _goalTime = _goalTime + (musicEndTime - musicStartTime);
            bgmSources[_audioToggle].SetScheduledEndTime(_goalTime);

            _audioToggle = 1 - _audioToggle;
        }

        protected override void AwakeSingleton()
        {
            // Extra checking incase music or sfx source is null
            if(sfxSources == null || sfxSources.Length == 0)
            {
                sfxSources = transform.Find("SFX Audio Source").GetComponentsInChildren<AudioSource>();

                if(sfxSources == null || sfxSources.Length == 0)
                    Debug.LogWarning("[Audio Manager] SFX sources is null");
            }

            if(bgmSources == null || bgmSources.Length == 0)
            {
                bgmSources = transform.Find("Music Audio Source").GetComponentsInChildren<AudioSource>();

                if (bgmSources == null || bgmSources.Length == 0)
                    Debug.LogWarning("[Audio Manager] BGM sources is null");
            }
        }
    }

}