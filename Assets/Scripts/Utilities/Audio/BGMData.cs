using UnityEngine;

namespace Utilities.Audio
{
    /// <summary>
    /// Background music data for a BGM to loop perfectly
    /// </summary>
    [CreateAssetMenu(fileName = "New BGM Data", menuName = "Audio/BGM Data")]
    public class BGMData : ScriptableObject
    {
        [Header("Audio")]
        [Tooltip("Audio clip of the BGM")]
        public AudioClip clip;

        [Header("Data")]
        [Tooltip("Unique Identifier of the bgm, all bgm scriptable object should have different id")]
        public int id = 0;
        [Tooltip("Name of the bgm for identify purposes, all bgm scriptable object should have different name")]
        public string bgmName = "BGM";

        [Header("Background Music Settings")]
        [Tooltip("Start time after a loop (seconds)")]
        public float startTime = 0f;
        [Tooltip("Time to start the loop (seconds)")]
        public float endTime = 100f;
        [Tooltip("Skip time when the BGM first play (seconds)")]
        public float initialSkip = 0f;
    }
}
