using UI.Tooltip;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AudioSetting : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    [SerializeField] private string mixerParameter = "Music";
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider slider;

    private void Start()
    {
        slider.minValue = 0.0001f;

        InitializeVolume();
    }

    public void InitializeVolume()
    {

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipManager.Instance.Show((slider.value * 100).ToString("F0") + "%");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.Hide();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        TooltipManager.Instance.UpdateTooltip((slider.value * 100).ToString("F0") + "%");
    }
}
