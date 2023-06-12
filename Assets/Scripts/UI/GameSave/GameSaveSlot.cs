using GameSave;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.PointerEventData;

namespace UI.GameSave
{
    public class GameSaveSlot : GameDataSlot
    {
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != InputButton.Left) return;
            base.OnPointerClick(eventData);

            // Save Game
            //UnityEngine.Debug.Log(GameSaveManager.Instance.ExportSaveData());
            GameSaveData saveData = GameSaveManager.Instance.ExportSaveData();
            GameSaveManager.Instance.Save(saveData);
        }
    }
}
