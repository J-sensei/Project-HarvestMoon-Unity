using Inventory;

namespace Interactable
{
    /// <summary>
    /// Type of the IInteractable objects
    /// </summary>
    public enum InteractableType
    {
        /// <summary>
        /// Tool that user can equip to the hand
        /// </summary>
        Tool,
        /// <summary>
        /// Tools that user can pickup and lifting including crops
        /// </summary>
        Item,
        /// <summary>
        /// Crop that ready to harvest
        /// </summary>
        Crop,
        /// <summary>
        /// Farm Land to harvest / plant the crop
        /// </summary>
        Farm,
        /// <summary>
        /// Environmental objects to interact with it such as bed and crafting workshop
        /// </summary>
        Environmental,
    }

    /// <summary>
    /// Object that is interactable to the player
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// On select behavior of the object
        /// </summary>
        /// <param name="v"></param>
        public void OnSelect(bool v);

        /// <summary>
        /// What to do when player is trying to interact with it
        /// </summary>
        public void Interact();

        /// <summary>
        /// Define the type of the IInteractable object
        /// </summary>
        /// <returns></returns>
        public InteractableType GetInteractableType();
        /// <summary>
        /// Return item data of the interactable object, if any, return null if dont have item data
        /// </summary>
        /// <returns></returns>
        public ItemData GetItemData();
    }
}
