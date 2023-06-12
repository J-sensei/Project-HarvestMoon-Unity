using GameDateTime;
using GameSave;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Farming
{
    public class FarmLandSaveManager : Singleton<FarmLandSaveManager>
    {
        /// <summary>
        /// Save data for the farms
        /// </summary>
        private static List<FarmSaveData> SaveData = null;
        public static List<FarmSaveData> CurrentSaveData
        {
            get
            {
                return SaveData;
            }
        }
        public static void Load(FarmSaveData[] farmSaveDatas)
        {
            SaveData = new List<FarmSaveData>(farmSaveDatas);

            // Update the save data if instance is created (When play is in farm scene)
            if(Instance != null)
            {
                Instance.LoadSaveData(SaveData);
            }
        }

        [Header("Farm Lands")]
        [SerializeField] private GameObject farm;
        [SerializeField] private List<FarmLand> _farmLands = new();

        [Header("Save Data")]
        [SerializeField] private List<FarmSaveData> _farmSaves = new();
        //[SerializeField] private List<CropSaveData> cropSaves = new();

        protected override void AwakeSingleton()
        {
            // Initialize farm and farm save data
            if(farm != null)
            {
                FarmLand[] farms = farm.GetComponentsInChildren<FarmLand>();
                foreach(FarmLand land in farms)
                {
                    _farmLands.Add(land);
                    land.ID = _farmLands.Count - 1;

                    // Create save data
                    _farmSaves.Add(new FarmSaveData(land.SaveData));
                }
            }
            else
            {
                Debug.LogWarning("[Farm Land Save Manager] Unable to initialize farm land as farm object is not found");
            }
        }

        private void Start()
        {
            // Load save data if SaveData exist
            if(SaveData != null)
            {
                LoadSaveData(SaveData);
            }
        }

        /// <summary>
        /// Update farm land state outside of the farm scene
        /// </summary>
        public static void UpdateFarmLandState(GameTime gameTime)
        {
            if(SaveData != null)
            {
                for (int i = 0; i < SaveData.Count; i++)
                {
                    FarmSaveData saveData = SaveData[i]; // Get the save data struct (This step is essential to change the structure data)
                    saveData.NewDay(gameTime);
                    SaveData[i] = saveData; // Put back the updated data
                }
            }
        }

        private void LoadSaveData(List<FarmSaveData> saves)
        {
            for(int i = 0; i < saves.Count; i++)
            {
                Debug.Log("FarmSave(" + saves[i].id +"): " + saves[i].state.ToString());
                _farmLands[i].Load(saves[i]);
                _farmSaves[i] = saves[i];
            }
        }

        /// <summary>
        /// Save current data into the static variable
        /// </summary>
        public void OnDestroy()
        {
            
        }

        /// <summary>
        /// Save a farm land data
        /// </summary>
        /// <param name="saveData"></param>
        public void Save(FarmSaveData saveData)
        {
            if(saveData.id < 0 || saveData.id > _farmLands.Count)
            {
                return;
            }

            _farmSaves[saveData.id] = saveData;

            // Save to static variable to keep the changes
            if(SaveData == null)
            {
                SaveData = new List<FarmSaveData>(_farmSaves);
            }
            else
            {
                SaveData[saveData.id] = saveData;
            }
        }
    }
}
