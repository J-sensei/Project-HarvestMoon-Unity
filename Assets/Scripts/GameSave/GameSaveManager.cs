using Entity;
using Entity.Enemy;
using Farming;
using GameDateTime;
using Inventory;
using SceneTransition;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Utilities;

namespace GameSave
{
    [System.Serializable]
    public struct GameSaveData
    {
        public FarmBinarySaveData[] farmSaveDatas;
        public ItemSaveData[] items;
        public ItemSaveData holdingSlot;
        public GameTime gameTime;
        public float playtime;
        public PlayerStatusSave playerStatus;

        public GameSaveData(FarmBinarySaveData[] farmSaveDatas, ItemSaveData[] items, ItemSaveData holdingSlot, GameTime gameTime, float playtime, PlayerStatusSave playerStatus)
        {
            this.farmSaveDatas = farmSaveDatas;
            this.items = items;
            this.holdingSlot = holdingSlot;
            this.gameTime = gameTime;
            this.playtime = playtime;
            this.playerStatus = playerStatus;
        }
    }

    /// <summary>
    /// Data to transfer player back to previous scene after finish the combat
    /// </summary>
    [System.Serializable]
    public struct TempSceneData
    {
        public SceneLocation location;
        public Vector3 playerPosition;
        public PlayerStatusSave playerStatus;

        // TODO: Add enemy list
        public Vector3[] enemiesPos;

        public TempSceneData(SceneLocation location, Vector3 playerPosition, PlayerStatusSave playerStatus, Vector3[] enemiesPos)
        {
            this.location = location;
            this.playerPosition = playerPosition;
            this.playerStatus = playerStatus;
            this.enemiesPos = enemiesPos;
        }
    }

    public enum GameSaveLoadType
    {
        JSON,
        BINARY
    }

    public class GameSaveManager : Singleton<GameSaveManager>
    {
        private const string DEFAULT_FILENAME = "Save1";
        private const string FOLDER = "/Saves";
        private const string JSON_EXTENSION = ".json";
        private const string BINARY_EXTENSION = ".cropQuestSave";
        private static ItemCollection ItemCollection;

        [Header("Item")]
        [Tooltip("Item datas")]
        [SerializeField] private ItemCollection itemCollection;

        [Header("Enemy Reference")]
        [SerializeField] private Enemy enemy;
        public Enemy Enemy { get { return enemy; } }

        protected override void AwakeSingleton()
        {
            ItemCollection = itemCollection;
        }

        public void Save(GameSaveData saveData, string filename = null)
        {
            string path = Application.persistentDataPath + FOLDER;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (string.IsNullOrEmpty(filename))
            {
                path += "/" + DEFAULT_FILENAME;
            }
            else
            {
                path += "/" + filename;
            }

            string jsonPath = path + JSON_EXTENSION;
            string binaryPath = path + BINARY_EXTENSION;
            string json = JsonUtility.ToJson(saveData);

            // Save to json
            File.WriteAllText(jsonPath, json); // Create json file for debug purposes

            // Save to binary
            using(FileStream file = File.Create(binaryPath))
            {
                new BinaryFormatter().Serialize(file, saveData);
            }
            //Debug.Log("Json Path: " + jsonPath);
            //Debug.Log("Binary Path: " + binaryPath);
            Debug.Log("[Game Save Manager] Game Save to " + path);
        }

        public bool SaveExist(string filename, GameSaveLoadType type = GameSaveLoadType.BINARY)
        {
            string path = Application.persistentDataPath + FOLDER + "/" + filename;
            if (type == GameSaveLoadType.JSON)
                path += JSON_EXTENSION;
            else
                path += BINARY_EXTENSION;
            if (File.Exists(path))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public GameSaveData Load(string filename = null, GameSaveLoadType type = GameSaveLoadType.BINARY)
        {
            GameSaveData saveData;

            // Get filename
            string name;
            if (string.IsNullOrEmpty(filename)) name = DEFAULT_FILENAME;
            else name = filename;

            // Get filepath
            string path = Application.persistentDataPath + FOLDER + "/" + name;
            if (type == GameSaveLoadType.JSON)
                path += JSON_EXTENSION;
            else
                path += BINARY_EXTENSION;

            if (File.Exists(path))
            {
                // Load Json
                if (type == GameSaveLoadType.JSON)
                {
                    string json = File.ReadAllText(path);
                    saveData = JsonUtility.FromJson<GameSaveData>(json);
                }
                else // Load Binary
                {
                    using(FileStream file = File.Open(path, FileMode.Open))
                    {
                        object data = new BinaryFormatter().Deserialize(file);
                        saveData = (GameSaveData)data; // Cast to save data type
                    }
                }
            }
            else
            {
                Debug.LogWarning("[Game Save Manager] File path not exist: " + path);
                return new GameSaveData();
            }

            return saveData;
        }

        /// <summary>
        /// Get the game save data of the current game state
        /// </summary>
        /// <returns></returns>
        public GameSaveData ExportSaveData()
        {
            FarmBinarySaveData[] farmSaves = null;
            if(FarmLandSaveManager.CurrentSaveData != null)
            {
                farmSaves = GameSaveManager.SerializeArray(FarmLandSaveManager.CurrentSaveData.ToArray());
            }
            ItemSaveData[] items = GameSaveManager.SerializeArray(InventoryManager.Instance.ItemSlots);
            ItemSaveData holdingSlot = GameSaveManager.SerializeData(InventoryManager.Instance.HoldingItemSlot);
            Debug.Log(items.Length);
            GameTime gameTime = GameTimeManager.Instance.GameTime;
            float playtime = GameStateManager.Instance.PlayTime;

            GameSaveData saveData = new GameSaveData(farmSaves, items, holdingSlot, gameTime, playtime, GameManager.Instance.Player.PlayerStatus.StatusSave);
            return saveData;
        }

        #region Serialize
        /// <summary>
        /// Convert item slot to item save data
        /// </summary>
        /// <param name="itemSlot"></param>
        /// <returns></returns>
        public static ItemSaveData SerializeData(ItemSlot itemSlot)
        {
            if (itemSlot == null || itemSlot.EmptyItem())
            {
                return new ItemSaveData(-1, 0);
            }
            return new ItemSaveData(itemSlot.ItemData.id, itemSlot.Quantity);
        }

        /// <summary>
        /// ONvert Farm save data to Farm binary save data
        /// </summary>
        /// <param name="farmData"></param>
        /// <returns></returns>
        public static FarmBinarySaveData SerializeData(FarmSaveData farmData)
        {
            return new FarmBinarySaveData(farmData);
        }

        /// <summary>
        /// Serialize array of item save data
        /// </summary>
        /// <param name="itemSlots"></param>
        /// <returns></returns>
        public static ItemSaveData[] SerializeArray(ItemSlot[] itemSlots)
        {
            return Array.ConvertAll(itemSlots, new Converter<ItemSlot, ItemSaveData>(SerializeData));
        }

        /// <summary>
        /// Serialize array of farm save data
        /// </summary>
        /// <param name="itemSlots"></param>
        /// <returns></returns>
        public static FarmBinarySaveData[] SerializeArray(FarmSaveData[] itemSlots)
        {
            return Array.ConvertAll(itemSlots, new Converter<FarmSaveData, FarmBinarySaveData>(SerializeData));
        }
        #endregion

        #region Deserialize
        public static ItemSlot SerializeData(ItemSaveData saveData)
        {
            ItemSlot item;
            if (saveData.id < 0)
                item = new ItemSlot(null);
            else
                item = new ItemSlot(Array.Find(ItemCollection.items, x => x.id == saveData.id), saveData.quantity);
            return item;
        }

        public static FarmSaveData SerializeData(FarmBinarySaveData saveData)
        {
            CropSaveData crop;

            // Initialize crop data
            if (saveData.crop.seedId < 0)
            {
                crop = new CropSaveData();
            }
            else
            {
                SeedData seed = (SeedData)Array.Find(ItemCollection.items, x => x.id == saveData.crop.seedId);
                if(seed == null)
                {
                    Debug.Log("[Game Save Manager] Failed to serialize farm data (id:" + saveData.id + "), Seed Data is null");
                }
                crop = new CropSaveData(seed, saveData.crop.growDay, saveData.crop.state);
            }

            return new FarmSaveData(saveData.id, saveData.state, crop);
        }

        public static ItemSlot[] SerializeArray(ItemSaveData[] saveDatas)
        {
            return Array.ConvertAll(saveDatas, new Converter<ItemSaveData, ItemSlot>(SerializeData));
        }
        public static FarmSaveData[] SerializeArray(FarmBinarySaveData[] saveDatas)
        {
            return Array.ConvertAll(saveDatas, new Converter<FarmBinarySaveData, FarmSaveData>(SerializeData));
        }
        #endregion
    }
}
