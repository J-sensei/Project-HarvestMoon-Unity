using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

namespace UI.UIScreen
{
    public class FadePanel : MonoBehaviour
    {
        [Tooltip("Reference of the image that block the screen")]
        [SerializeField] private Image image;
        [Tooltip("Fade the panel out when start the fade panel")]
        [SerializeField] private bool fadeOnStart = true;
        [Tooltip("Duration of the fade")]
        [SerializeField] private float fadeDuration = 2f;
        [Tooltip("Color of the panel")]
        [SerializeField] private Color fadeColor;

        public float OriginalFadeDuration { get; private set; }
        public float FadeDuration { get { return fadeDuration; } set { fadeDuration = value; } }
        private CanvasGroup _canvasGroup;

        public UnityEvent OnStart;
        /// <summary>
        /// Event to trigegr when fade is finish
        /// </summary>
        public UnityEvent OnFinish;
        public bool Finish { get; private set; } = false;
        public bool FadeOnStart { get { return fadeOnStart; } }


        private void Awake()
        {
            OriginalFadeDuration = fadeDuration;
            if (image == null)
            {
                image = GetComponent<Image>();
            }
            image.gameObject.SetActive(false); // Disable the image
            _canvasGroup = image.GetComponent<CanvasGroup>();
        }

        /// <summary>
        /// Black to transparent fade
        /// </summary>
        public void FadeIn(TweenCallback callback = null)
        {
            image.gameObject.SetActive(true);
            Finish = false;
            Fade(1, 0, callback);
        }

        /// <summary>
        /// Transparent to transparent fade
        /// </summary>
        public void FadeOut(TweenCallback callback = null)
        {
            image.gameObject.SetActive(true);
            Finish = false;
            Fade(0, 1, callback);
        }

        /// <summary>
        /// Fade out then fade in
        /// </summary>
        /// <param name="callback">Callback function when fading is finish</param>
        public void FadeOutIn(TweenCallback callback = null)
        {
            image.gameObject.SetActive(true);
            Finish = false;
            Fade(0, 1, () => Fade(1, 0, callback)); // Fade out then when fade out finish call fade in
        }

        /// <summary>
        /// Fade the panel using tween
        /// </summary>
        /// <param name="alphaIn">Initial Value</param>
        /// <param name="alphaOut">Final Value</param>
        /// <param name="callback">Callback function when tween is completed</param>
        public void Fade(float alphaIn, float alphaOut, TweenCallback callback = null)
        {
            // Start events invoke
            OnStart?.Invoke();
            OnStart.RemoveAllListeners();

            _canvasGroup.alpha = alphaIn;
            _canvasGroup.DOFade(alphaOut, fadeDuration).SetEase(Ease.Linear).OnComplete(() => {
                // Finish events invoke
                OnFinish?.Invoke();
                OnFinish.RemoveAllListeners();

                callback?.Invoke();
                Finish = true;

                if (alphaOut == 1)
                {
                    image.gameObject.SetActive(true);
                }
                else
                {
                    image.gameObject.SetActive(false);
                }
            });
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
}