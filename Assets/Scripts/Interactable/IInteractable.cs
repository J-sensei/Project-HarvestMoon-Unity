namespace Interactable
{
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
    }
}
