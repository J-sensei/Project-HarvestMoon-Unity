using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FadePanel : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private bool fadeOnStart = true;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private Color fadeColor;

    public UnityEvent OnFinish;
    public bool Finish { get; private set; } = false;


    private void Awake()
    {
        image.gameObject.SetActive(false);
        if(image == null)
        {
            image = GetComponent<Image>();
        }
    }

    private void Start()
    {
        if (fadeOnStart)
        {
            FadeIn();
        }
    }

    /// <summary>
    /// Black to transparent fade
    /// </summary>
    public void FadeIn()
    {
        image.gameObject.SetActive(true);
        Finish = false;
        Fade(1, 0);
    }

    /// <summary>
    /// Transparent to transparent fade
    /// </summary>
    public void FadeOut()
    {
        image.gameObject.SetActive(true);
        Finish = false;
        Fade(0, 1);
    }

    public void Fade(float alphaIn, float alphaOut)
    {
        StartCoroutine(FadeRoutine(alphaIn, alphaOut));
    }

    private IEnumerator FadeRoutine(float alphaIn, float alphaOut)
    {
        float timer = 0;
        while (timer <= fadeDuration)
        {
            Color color = fadeColor;
            color.a = Mathf.Lerp(alphaIn, alphaOut, (timer / fadeDuration));
            image.color = color;

            timer += Time.deltaTime;
            yield return null; // Wait for one frame
        }

        // Make sure color will to go alpha out
        Color colorFinal = fadeColor;
        colorFinal.a = alphaOut;
        image.color = colorFinal;
        Finish = true;
        OnFinish?.Invoke();
        OnFinish.RemoveAllListeners();

        if(alphaOut == 1)
        {
            image.gameObject.SetActive(true);
        }
        else
        {
            image.gameObject.SetActive(false);
        }
    }
}
