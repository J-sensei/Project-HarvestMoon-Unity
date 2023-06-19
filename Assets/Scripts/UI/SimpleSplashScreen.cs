using UnityEngine;
using DG.Tweening;

namespace UI
{
    /// <summary>
    /// A simple splash screen data
    /// </summary>
    [System.Serializable]
    public struct SimpleSplashScreenData
    {
        /// <summary>
        /// Reference to the target, canvas group can be use to do fade in out
        /// </summary>
        public CanvasGroup canvasGroup;
        /// <summary>
        /// How long the splash screen will stay
        /// </summary>
        public float stayTime;
    }
    /// <summary>
    /// Simple splash screen will fade out when start
    /// </summary>
    public class SimpleSplashScreen : MonoBehaviour
    {
        [SerializeField] private Canvas splashScreenCanvas;
        [SerializeField] private SimpleSplashScreenData[] screens;
        [SerializeField] bool startSplashScreen = true;
        [SerializeField] private float tweenDuration = 0.15f;
        private float _timer;
        private int _index = 0; // Always start at 1

        private void Awake()
        {
            // Hide all screens first
            for(int i = 0; i < screens.Length; i++)
            {
                screens[i].canvasGroup.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            // Fade in the first screen
            if (startSplashScreen && screens.Length > 0)
            {
                splashScreenCanvas.gameObject.SetActive(true);
                screens[_index].canvasGroup.gameObject.SetActive(true);
                screens[_index].canvasGroup.alpha = 0f;
                screens[_index].canvasGroup.DOFade(1f, tweenDuration);
            }
            else
            {
                splashScreenCanvas.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (!startSplashScreen) return;
            _timer += Time.deltaTime; // Add the timer

            if (_timer >= screens[_index].stayTime) // Siwtch out this screen
            {
                _timer = 0; // Reset the timer
                if(_index + 1< screens.Length) // Check if next screen is available
                {
                    // Hide previous
                    SimpleSplashScreenData temp = screens[_index]; // Reference to to run the onComplete function
                    screens[_index].canvasGroup.DOFade(0f, tweenDuration).OnComplete(() =>
                    {
                        temp.canvasGroup.gameObject.SetActive(false);
                    });

                    _index++;

                    // Show next
                    screens[_index].canvasGroup.gameObject.SetActive(true);
                    screens[_index].canvasGroup.alpha = 0f;
                    screens[_index].canvasGroup.DOFade(1f, tweenDuration);
                }
                else
                {
                    // Hide last splash screen
                    screens[_index].canvasGroup.DOFade(0f, tweenDuration).OnComplete(() =>
                    {
                        screens[_index].canvasGroup.gameObject.SetActive(false);
                    });

                    // Close canvas
                    splashScreenCanvas.GetComponent<CanvasGroup>().DOFade(0f, tweenDuration).OnComplete(() =>
                    {
                        splashScreenCanvas.gameObject.SetActive(false);
                    });
                    startSplashScreen = false; // Stop splash screen
                }
            }
        }
    }

}