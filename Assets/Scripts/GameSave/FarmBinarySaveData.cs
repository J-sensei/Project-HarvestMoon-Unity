using Farming;

namespace GameSave
{
    [System.Serializable]
    /// <summary>
    /// Save data for a farm land works for binary file format
    /// </summary>
    public struct FarmBinarySaveData
    {
        public int id;
        public FarmLandState state;
        public CropBinarySaveData crop;

        public FarmBinarySaveData(FarmSaveData data)
        {
            id = data.id;
            state = data.state;

            if(data.crop.seedData != null)
            {
                crop.seedId = data.crop.seedData.id;
                crop.growDay = data.crop.growDay;
                crop.state = data.crop.state;
            }
            else
            {
                crop.seedId = -1;
                crop.growDay = 0;
                crop.state = 0;
            }
        }

        /// <summary>
        /// Farm with no crop planted
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        public FarmBinarySaveData(int id, FarmLandState state)
        {
            this.id = id;
            this.state = state;
            crop.seedId = -1;
            crop.growDay = 0;
            crop.state = 0;
        }

        /// <summary>
        /// Farm with crop planted
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="crop"></param>
        public FarmBinarySaveData(int id, FarmLandState state, CropBinarySaveData crop)
        {
            this.id = id;
            this.state = state;
            this.crop = crop;
        }
    }

    [System.Serializable]
    /// <summary>
    /// Save data for crop that planted in the farm land
    /// </summary>
    public struct CropBinarySaveData
    {
        public int seedId;
        public int growDay;
        public CropState state;

        public CropBinarySaveData(int seedId, int growDay, CropState state)
        {
            this.seedId = seedId;
            this.growDay = growDay;
            this.state = state;
        }
    }
}
