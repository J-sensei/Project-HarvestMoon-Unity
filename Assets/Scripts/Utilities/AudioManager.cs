using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Manage different audio sources and play audio clips, Singleton so it can be accessing in all the scenes
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource[] audioSources;
        private int _audioToggle = 0;
        [SerializeField] private AudioSource sfxSource;

        [Header("Background Music")]
        [SerializeField] private float musicStartTime = 0f;
        [SerializeField] private float musicEndTime = 100f;
        public float initialSkip = 160;
        [SerializeField] private AudioClip musicClip;

        [Header("Farming Audio")]
        [SerializeField] public AudioClip wateringAudio;
        [SerializeField] public AudioClip hoeAudio;
        [SerializeField] public AudioClip plantAudio;

        private double _musicDuration;
        private double _goalTime = 0;
        bool test = false;
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
            _goalTime = AudioSettings.dspTime;
            audioSources[0].clip = clip;
            audioSources[0].PlayScheduled(_goalTime);
            audioSources[0].time = initialSkip;

            _musicDuration = (double)clip.samples / clip.frequency;
            _goalTime = _goalTime + (musicEndTime - initialSkip);
            audioSources[0].SetScheduledEndTime(_goalTime);

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
            Debug.Log("Schedule loop");
            audioSources[_audioToggle].clip = musicClip;
            audioSources[_audioToggle].PlayScheduled(_goalTime);
            audioSources[_audioToggle].time = musicStartTime;

            _goalTime = _goalTime + (musicEndTime - musicStartTime);
            audioSources[_audioToggle].SetScheduledEndTime(_goalTime);

            _audioToggle = 1 - _audioToggle;
        }

        protected override void AwakeSingleton()
        {
            // Extra checking incase music or sfx source is null
            //if(musicSource == null)
            //{
            //    musicSource = GetComponentInChildren<AudioSource>();
            //    Debug.LogWarning("[Audio Manager] Music source was null, assigned children component. IS NULL: " + (musicSource == null));
            //}

            if(sfxSource == null)
            {
                sfxSource = GetComponentInChildren<AudioSource>();
                Debug.LogWarning("[Audio Manager] SFX source was null, assigned children component. IS NULL: " + (sfxSource == null));
            }
        }
    }

}