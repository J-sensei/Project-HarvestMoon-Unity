using System.Collections;
using UnityEngine;
using Utilities.Audio;

namespace Utilities
{
    /// <summary>
    /// Manage different audio sources and play audio clips, Singleton so it can be accessing in all the scenes
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        /// <summary>
        /// Early time to set schedule play to loop the BGM (in seconds)
        /// </summary>
        private const double SCHEDULE_EARLY = 5f;
        private const float FADE_DURATION = 2f;

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
        private BGMData _currentBGM = null;

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

        public void PlayMusic(BGMData bgmData)
        {
            StopAllCoroutines();
            for (int i = 0; i < bgmSources.Length; i++)
            {
                if (bgmSources[i].clip != null && bgmSources[i].isPlaying)
                {
                    StartCoroutine(Fade(false, bgmSources[i], FADE_DURATION, 0f)); // Fade out
                    _audioToggle = 1 - i;
                }
                else
                {
                    bgmSources[i].Stop(); // Clear any bgm are playing right now
                }
            }
            _goalTime = AudioSettings.dspTime;
            bgmSources[_audioToggle].clip = bgmData.clip;
            bgmSources[_audioToggle].PlayScheduled(_goalTime);
            bgmSources[_audioToggle].time = bgmData.initialSkip;

            _musicDuration = (double)bgmData.clip.samples / bgmData.clip.frequency;
            _goalTime = _goalTime + (bgmData.endTime - bgmData.initialSkip);
            bgmSources[_audioToggle].SetScheduledEndTime(_goalTime); // Set the next bgm loop

            bgmSources[_audioToggle].volume = 0f;
            StartCoroutine(Fade(true, bgmSources[_audioToggle], FADE_DURATION, 1f));

            _audioToggle = 1 - _audioToggle; // Toggle audio source
            _currentBGM = bgmData;
        }

        private void Start()
        {
            //PlayMusic(musicClip); // Test play music at the begining
            //PlayMusic(bgmList[0]);
        }


        private void Update()
        {
            if(AudioSettings.dspTime > _goalTime - SCHEDULE_EARLY)
            {
                if(_currentBGM != null)
                {
                    PlayScheduleClip(_currentBGM);
                }
                else
                {
                    Debug.LogWarning("[Audio Manager] Current BGM is null");
                }
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

        private void PlayScheduleClip(BGMData bgmData)
        {
            bgmSources[_audioToggle].clip = bgmData.clip;
            bgmSources[_audioToggle].PlayScheduled(_goalTime);
            bgmSources[_audioToggle].time = bgmData.startTime;

            _goalTime = _goalTime + (bgmData.endTime - bgmData.startTime);
            bgmSources[_audioToggle].SetScheduledEndTime(_goalTime);

            _audioToggle = 1 - _audioToggle;
        }

        private IEnumerator Fade(bool fadeIn, AudioSource source, float duration, float targetVolume)
        {
            float time = 0f;
            float startVol = source.volume;
            while(time < duration)
            {
                time += Time.deltaTime;
                source.volume = Mathf.Lerp(startVol, targetVolume, time / duration);
                yield return null;
            }

            if(targetVolume == 0f)
            {
                source.Stop();
                source.volume = 1f;
                source.clip = null;
            }
            yield break;
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