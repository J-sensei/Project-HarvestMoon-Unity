using Farming;
using GameDateTime;
using GameSave;
using Inventory;
using SceneTransition;
using UI.UIScreen;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.PointerEventData;

namespace UI.GameSave
{
    public class GameLoadSlot : GameDataSlot
    {
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != InputButton.Left) return;
            base.OnPointerClick(eventData);

            if (!GameSaveManager.Instance.SaveExist(Filename))
            {
                return;
            }
            GameMenu.Instance.ToggleGameMenu(false);
            FadeScreenManager.Instance.Loading(true);
            GameManager.Instance.Player.Disable();
            // Load Game Save Data
            GameSaveData saveData = GameSaveManager.Instance.Load();

            // Load data
            // Load into inventory manager
            ItemSlot[] itemSlots = GameSaveManager.SerializeArray(saveData.items);
            ItemSlot holdingItemSlot = GameSaveManager.SerializeData(saveData.holdingSlot);

            InventoryManager.Instance.Load(itemSlots, holdingItemSlot);

            // Load farm
            if (saveData.farmSaveDatas != null)
                FarmLandSaveManager.Load(GameSaveManager.SerializeArray(saveData.farmSaveDatas));

            // Load Game Time
            GameTimeManager.Instance.Load(saveData.gameTime);

            SceneTransitionManager.Instance.SetSceneLocation(SceneLocation.MainMenu);
            SceneTransitionManager.Instance.SwitchScene(SceneLocation.Home); // Always back to home after load a save
        }
    }
}