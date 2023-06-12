using DG.Tweening;
using Inventory.UI;
using UnityEngine;

namespace UI.Tab.Content
{
    public class InventoryTabContent : TabContent
    {
        [SerializeField] private CanvasGroup titleCanvas;
        [SerializeField] private CanvasGroup scrollView;

        [Header("Tween")]
        [SerializeField] private float tweenDuration = 0.15f;
        [SerializeField] private float rotateAngle = 6.5f;

        private Quaternion _titleRot;
        private Quaternion _scrollRot;
        private void Awake()
        {
            _titleRot = titleCanvas.transform.rotation;
            _scrollRot = scrollView.transform.rotation;
        }

        public override void Open()
        {
            base.Open();

            titleCanvas.transform.rotation = Quaternion.Euler(0f, 0f, -rotateAngle);
            scrollView.transform.rotation = Quaternion.Euler(0f, 0f, rotateAngle);

            titleCanvas.transform.DORotate(_titleRot.eulerAngles, tweenDuration);
            scrollView.transform.DORotate(_scrollRot.eulerAngles, tweenDuration);

            titleCanvas.alpha = 0f;
            scrollView.alpha = 0f;
            titleCanvas.DOFade(1f, tweenDuration);
            scrollView.DOFade(1f, tweenDuration);
        }

        public override void Close()
        {
            InventoryUIManager.Instance.ResetInventorySlots();
            titleCanvas.transform.DORotate(Quaternion.Euler(0f, 0f, -rotateAngle).eulerAngles, tweenDuration);
            scrollView.transform.DORotate(Quaternion.Euler(0f, 0f, rotateAngle).eulerAngles, tweenDuration);

            titleCanvas.alpha = 1f;
            scrollView.alpha = 1f;
            titleCanvas.DOFade(0f, tweenDuration).OnComplete(() => base.Close());
            scrollView.DOFade(0f, tweenDuration);
        }
    }
}
