using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace UI.Tab.Content
{
    public class CharacterTabContent : TabContent
    {
        [SerializeField] private Image characterImage;
        [SerializeField] private Image characterBackground;
        [SerializeField] private TextMeshProUGUI titleText;

        [Header("Tween")]
        [SerializeField] private float tweenDuration = 0.15f;
        [SerializeField] private float tweenDistance = -40f;
        [SerializeField] private float titleTweenDistance = 100f;

        [SerializeField] private Vector3[] _originalPositions;
        [SerializeField] private CanvasGroup[] _canvasGroups;

        private void Awake()
        {
            //if(_originalPositions == null || _originalPositions.Length == 0)
            //{
                _originalPositions = new Vector3[3];
                _originalPositions[0] = characterImage.transform.localPosition; // 0 => Character Image
                _originalPositions[1] = characterBackground.transform.localPosition; // 1 => Character background
                _originalPositions[2] = titleText.transform.localPosition; // 2 => Title text
            //}

            _canvasGroups = new CanvasGroup[3];
            _canvasGroups[0] = characterImage.GetComponent<CanvasGroup>();
            _canvasGroups[1] = characterBackground.GetComponent<CanvasGroup>();
            _canvasGroups[2] = titleText.GetComponent<CanvasGroup>();
        }

        public override void Open()
        {
            base.Open();
            characterImage.transform.localPosition = new Vector3(_originalPositions[0].x + tweenDistance, _originalPositions[0].y, _originalPositions[0].z);
            characterBackground.transform.localPosition = new Vector3(_originalPositions[1].x + tweenDistance, _originalPositions[1].y, _originalPositions[1].z);
            titleText.transform.localPosition = new Vector3(_originalPositions[2].x, _originalPositions[2].y + titleTweenDistance, _originalPositions[2].z);

            characterImage.transform.DOLocalMove(_originalPositions[0], tweenDuration);
            characterBackground.transform.DOLocalMove(_originalPositions[1], tweenDuration);
            titleText.transform.DOLocalMove(_originalPositions[2], tweenDuration);

            _canvasGroups[0].alpha = 0f;
            _canvasGroups[1].alpha = 0f;
            _canvasGroups[2].alpha = 0f;
            _canvasGroups[0].DOFade(1f, tweenDuration);
            _canvasGroups[1].DOFade(1f, tweenDuration);
            _canvasGroups[2].DOFade(1f, tweenDuration);
        }

        public override void Close()
        {
            characterImage.transform.DOLocalMove(new Vector3(_originalPositions[0].x + tweenDistance, _originalPositions[0].y, _originalPositions[0].z), tweenDuration);
            characterBackground.transform.DOLocalMove(new Vector3(_originalPositions[1].x + tweenDistance, _originalPositions[1].y, _originalPositions[1].z), tweenDuration);
            titleText.transform.DOLocalMove(new Vector3(_originalPositions[2].x, _originalPositions[2].y + titleTweenDistance, _originalPositions[2].z), tweenDuration);

            _canvasGroups[0].alpha = 1f;
            _canvasGroups[1].alpha = 1f;
            _canvasGroups[0].DOFade(0f, tweenDuration).OnComplete(() =>
            {
                base.Close();
            });
            _canvasGroups[1].DOFade(0f, tweenDuration);
            _canvasGroups[2].DOFade(0f, tweenDuration);
        }
    }

}