using Farming;
using GameDateTime;
using Inventory;
using UnityEngine;

namespace GameSave
{
    [System.Serializable]
    /// <summary>
    /// Save data for a farm land
    /// </summary>
    public struct FarmSaveData
    {
        public int id;
        public FarmLandState state;
        public CropSaveData crop;

        public FarmSaveData(FarmSaveData data)
        {
            id = data.id;
            state = data.state;
            crop = data.crop;
        }

        /// <summary>
        /// Farm with no crop planted
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        public FarmSaveData(int id, FarmLandState state)
        {
            this.id = id;
            this.state = state;
            crop.seedData = null;
            crop.growDay = 0;
            crop.state = 0;
        }

        /// <summary>
        /// Farm with crop planted
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="crop"></param>
        public FarmSaveData(int id, FarmLandState state, CropSaveData crop)
        {
            this.id = id;
            this.state = state;
            this.crop = crop;
        }

        /// <summary>
        /// Check if this farm land has crop planted to it
        /// </summary>
        /// <returns></returns>
        public bool HasCrop()
        {
            return crop.seedData != null;
        }

        /// <summary>
        /// Make changes to the data to update the farm and crop state
        /// </summary>
        /// <param name="gameTime"></param>
        public void NewDay(GameTime gameTime)
        {
            // Make the water dry out in the next day
            if (state == FarmLandState.Watered)
            {
                // If crop is available, do something with it
                // Only want to grow the crop if player remember to water it
                if (HasCrop())
                {
                    crop.Grow();
                }
                state = FarmLandState.Farmland;
            }
            else if (state != FarmLandState.Watered)
            {
                if (HasCrop())
                {
                    crop.Wilt();
                }
            }
        }
    }

    [System.Serializable]
    /// <summary>
    /// Save data for crop that planted in the farm land
    /// </summary>
    public struct CropSaveData
    {
        public SeedData seedData;
        public int growDay;
        public CropState state;

        public CropSaveData(SeedData seedData, int growDay, CropState state)
        {
            this.seedData = seedData;
            this.growDay = growDay;
            this.state = state;
        }

        public void Grow()
        {
            if (state == CropState.Wilted) return; // No need to grow anymore as already wilted

            growDay++; // Grow up by one day
            if (growDay >= seedData.grows[seedData.grows.Length - 1].day) // Check if the plant are fully grown
            {
                state = CropState.Harvest; // Crop are ready to harvest
            }
        }

        public void Wilt()
        {
            state = CropState.Wilted;
        }
    }
}
