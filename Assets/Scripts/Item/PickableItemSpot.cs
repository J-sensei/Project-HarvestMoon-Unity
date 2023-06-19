using UnityEngine;

namespace Item
{
    public struct ItemSpotData
    {
        public bool[] items;
        public ItemSpotData(bool[] items)
        {
            this.items = items;
        }
    }

    public class PickableItemSpot : MonoBehaviour
    {
        [SerializeField] private PickableItem[] items;
        [SerializeField] private ParticleSystem indicator;


        public void CheckItem()
        {
            bool result = false;
            for(int i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                {

                }
            }
        }

        public ItemSpotData SaveData
        {
            get
            {
                bool[] temp = new bool[items.Length];
                for(int i = 0; i < items.Length; i++)
                {
                    if (items[i] != null)
                        temp[i] = true;
                    else
                        temp[i] = false;
                }
                return new ItemSpotData(temp);
            }
        }

        public void Load(ItemSpotData save)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (!save.items[i])
                    Destroy(items[i]);
            }
        }
    }
}
