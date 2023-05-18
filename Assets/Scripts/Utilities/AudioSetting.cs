using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSetting : MonoBehaviour
{
    [SerializeField] private string mixerParameter = "Music";
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider slider;

    private void Start()
    {
        slider.minValue = 0.0001f;

        if (PlayerPrefs.HasKey(mixerParameter))
        {
            LoadVolume();
        }
        else
        {
            SetVolume();
        }

    }

    public void SetVolume()
    {
        float volume = slider.value;
        mixer.SetFloat(mixerParameter, Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(mixerParameter, volume);
    }

    private void LoadVolume()
    {
        slider.value = PlayerPrefs.GetFloat(mixerParameter);
        SetVolume();
    }
}
