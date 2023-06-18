using UnityEngine;
using DG.Tweening;

namespace UI.Tab.Content
{
    public class SettingTabContent : TabContent
    {
        [SerializeField] private CanvasGroup view;

        [Header("Tween")]
        [SerializeField] private float tweenDuration = 0.15f;
        [SerializeField] private float tweenDistance = 120f;

        private Vector3 _viewPos;

        private void Awake()
        {
            _viewPos = view.transform.localPosition;
        }

        public override void Open()
        {
            base.Open();

            view.transform.position = new Vector3(_viewPos.x, _viewPos.y + tweenDistance, _viewPos.z);
            view.transform.DOLocalMove(_viewPos, tweenDuration);

            view.alpha = 0f;
            view.DOFade(1f, tweenDuration);
        }

        public override void Close()
        {
            view.transform.DOLocalMove(new Vector3(_viewPos.x, _viewPos.y + tweenDistance, _viewPos.z), tweenDuration);

            view.alpha = 1f;
            view.DOFade(0f, tweenDuration).OnComplete(() => base.Close());
        }
    }
}
