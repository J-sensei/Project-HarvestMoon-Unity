using Farming;
using GameDateTime;
using GameSave;
using Inventory;
using SceneTransition;
using UI.UIScreen;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.PointerEventData;

namespace UI.GameSave
{
    public class GameLoadSlot : GameDataSlot
    {
        public static string LoadFilename { get; set; }
        public static bool RequestLoad { get; set; } = false;
        [SerializeField] private bool requestLoad = false;

        /// <summary>
        /// Request to load the save file (Use because main menu don't have enough game objects to load need to wait until home scene is loaded only to load back the save)
        /// </summary>
        /// <param name="filename"></param>
        public static void RequestLoadFile(string filename)
        {
            UnityEngine.Debug.Log("Request to load: " + filename);
            GameMenu.Instance?.ToggleGameMenu(false);
            GameManager.Instance?.Player?.Disable();
            // Load Game Save Data
            GameSaveData saveData = GameSaveManager.Instance.Load(filename);

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
            GameStateManager.Instance.LoadRecordTime(saveData.playtime);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != InputButton.Left) return;
            base.OnPointerClick(eventData);

            if (!GameSaveManager.Instance.SaveExist(Filename))
            {
                return;
            }

            if (requestLoad)
            {
                // Usually this place will place in main menu, so need try to enable back the shortcut so later in game wont experience shortcuts disabled
                if (GameMenu.Instance != null)
                {
                    GameMenu.Instance.EnableShortcuts();
                }
                GameLoadSlot.LoadFilename = Filename;
                GameLoadSlot.RequestLoad = true;
                SceneTransitionManager.Instance.SetSceneLocation(SceneLocation.MainMenu);
                SceneTransitionManager.Instance.SwitchScene(SceneLocation.Home); // Always back to home after load a save
                return;
            }
            GameMenu.Instance?.ToggleGameMenu(false);
            FadeScreenManager.Instance.Loading(true);
            GameManager.Instance?.Player?.Disable();
            // Load Game Save Data
            GameSaveData saveData = GameSaveManager.Instance.Load(Filename);

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
            GameStateManager.Instance.LoadRecordTime(saveData.playtime);

            SceneTransitionManager.Instance.SetSceneLocation(SceneLocation.MainMenu);
            SceneTransitionManager.Instance.SwitchScene(SceneLocation.Home); // Always back to home after load a save
        }
    }
}