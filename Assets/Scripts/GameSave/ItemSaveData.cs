namespace GameSave
{
    [System.Serializable]
    public struct ItemSaveData
    {
        /// <summary>
        /// Unique identifier of the item
        /// </summary>
        public int id;
        /// <summary>
        /// How many quantity of this item slot has
        /// </summary>
        public int quantity;

        public ItemSaveData(int id, int quantity)
        {
            this.id = id;
            this.quantity = quantity;
        }
    }
}
