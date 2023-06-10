using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Animated sprites and put into the image ui element
    /// </summary>
    public class SpriteAnimationUI : MonoBehaviour
    {
        [SerializeField] private Image image;
        [Tooltip("Sprites to animate")]
        [SerializeField] private Sprite[] sprites;
        [Tooltip("Time elapsed for each frame")]
        [SerializeField] private float animationSpeed = 0.2f;

        private int _spriteIndex;
        private Coroutine _coroutine;
        private bool _isFinish;

        public bool IsFinish { get { return _isFinish; } }

        private void Awake()
        {
            if(image == null)
            {
                image = GetComponent<Image>();
            }
        }

        public void Play()
        {
            if(sprites == null || sprites.Length == 0)
            {
                UnityEngine.Debug.LogWarning("[Sprite Animation UI] Sprite Animation UI cannot play as sprites is empty or null");
                return;
            }

            _isFinish = false;
            _coroutine = StartCoroutine(Animating());
        }

        public void Stop()
        {
            _isFinish = true;
            StopCoroutine(Animating());
        }

        private IEnumerator Animating()
        {
            yield return new WaitForSeconds(animationSpeed);
            // Reset frame
            if(_spriteIndex >= sprites.Length)
            {
                _spriteIndex = 0;
            }

            image.sprite = sprites[_spriteIndex];
            _spriteIndex++;
            if (!_isFinish)
            {
                _coroutine = StartCoroutine(Animating());
            }
        }
    }
}