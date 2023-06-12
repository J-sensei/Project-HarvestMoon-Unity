using Inventory;
using QuickOutline;
using UI.GameSave;
using UnityEngine;
using Utilities;

namespace Interactable
{
    /// <summary>
    /// A place for the player to save
    /// </summary>
    public class SaveBench : MonoBehaviour, IInteractable
    {
        [SerializeField] private Outline outline;

        private void Start()
        {
            if (outline == null)
            {
                outline = GetComponent<Outline>();
            }
            outline.enabled = false;

            // For some reason this can make the setting applied
            StartCoroutine(OutlineHelper.InitializeOutline(outline));
        }

        #region IInteractable
        public InteractableType GetInteractableType()
        {
            return InteractableType.Environmental;
        }

        public void Interact()
        {
            GameSaveUI.Instance.ToggleGameMenu(true);
        }

        public void OnSelect(bool v)
        {
            if(outline != null)
                outline.enabled = v;
        }

        public ItemData GetItemData()
        {
            return null;
        }
        #endregion
    }

}