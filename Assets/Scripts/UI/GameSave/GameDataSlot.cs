using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;
using DG.Tweening;
using TMPro;
using GameSave;

namespace UI.GameSave
{
    public abstract class GameDataSlot : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        [Header("Configuration")]
        [SerializeField] private string filename;
        protected string Filename { get { return filename; } }

        [Header("Color")]
        [SerializeField] private Color idleColor = Color.white;
        [SerializeField] private Color hoverColor = Color.white;
        [SerializeField] private float tweenDuration = 0.15f;

        [Header("Reference")]
        [SerializeField] private Image image;

        [Header("Text Details")]
        [Tooltip("Title of the save including save name and order number")]
        [SerializeField] private TextMeshProUGUI title;
        [Tooltip("In game date time")]
        [SerializeField] private TextMeshProUGUI dateTime;
        [Tooltip("Playtime")]
        [SerializeField] private TextMeshProUGUI playTime;

        private void Awake()
        {
            if(image == null)
                image = GetComponent<Image>();
        }

        private void OnDestroy()
        {
            DOTween.Kill(image);
        }

        public void SetFilename(string filename) => this.filename = filename;

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.menuClick, 1);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            image.DOColor(hoverColor, tweenDuration);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.menuSelect, 1);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            image.DOColor(idleColor, tweenDuration);
        }

        public void Reset()
        {
            if(image != null)
                image.color = idleColor;
        }

        public void LoadSaveDetails()
        {
            if (GameSaveManager.Instance.SaveExist(filename))
            {
                GameSaveData data = GameSaveManager.Instance.Load(filename);
                title.text = filename;
                dateTime.text = data.gameTime.Preview();
                playTime.text = "Playtime: " + GameStateManager.GetPlayTimeString(data.playtime);
            }
            else
            {
                title.text = "Empty Save File";
                dateTime.text = "Date Time";
                playTime.text = "Playtime: 00:00:00";
            }
        }
    }
}
