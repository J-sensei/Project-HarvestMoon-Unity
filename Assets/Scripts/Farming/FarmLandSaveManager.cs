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

        private void LoadSaveData(List<FarmSaveData> saves)
        {
            for(int i = 0; i < saves.Count; i++)
            {
                _farmLands[i].Load(saves[i]);
                _farmSaves[i] = saves[i];
            }
        }

        /// <summary>
        /// Save current data into the static variable
        /// </summary>
        public void OnDestroy()
        {
            SaveData = new List<FarmSaveData>(_farmSaves); // Save to SaveData static variables to hold it throught out the game session
        }

        /// <summary>
        /// Save a farm land data
        /// </summary>
        /// <param name="saveData"></param>
        public void Save(FarmSaveData saveData)
        {
            if(saveData.id < 0 || saveData.id > _farmLands.Count)
            {
                Debug.LogWarning("[Farm Land Save Manager] Invalid save data id: " + saveData.id);
                return;
            }

            _farmSaves[saveData.id] = saveData;
        }
    }
}
