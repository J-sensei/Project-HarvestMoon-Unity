using GameSave;
using UnityEngine.EventSystems;

namespace UI.GameSave
{
    public class GameSaveSlot : GameDataSlot
    {
        public override void OnPointerClick(PointerEventData eventData)
        {
            // Save Game
            //UnityEngine.Debug.Log(GameSaveManager.Instance.ExportSaveData());
            GameSaveData saveData = GameSaveManager.Instance.ExportSaveData();
            GameSaveManager.Instance.Save(saveData);

            base.OnPointerClick(eventData);
        }
    }
}
