using Farming;
using Inventory;

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

        public bool HasCrop()
        {
            return crop.seedData != null;
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
    }
}
