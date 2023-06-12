using Farming;
using GameDateTime;
using GameSave;
using Inventory;
using SceneTransition;
using UI.UIScreen;
using UnityEngine.EventSystems;

namespace UI.GameSave
{
    public class GameLoadSlot : GameDataSlot
    {
        public override void OnPointerClick(PointerEventData eventData)
        {
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

            base.OnPointerClick(eventData);

            SceneTransitionManager.Instance.SetSceneLocation(SceneLocation.MainMenu);
            SceneTransitionManager.Instance.SwitchScene(SceneLocation.Home); // Always back to home after load a save
        }
    }
}