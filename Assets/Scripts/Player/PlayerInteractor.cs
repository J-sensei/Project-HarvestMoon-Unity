using Farming;
using Item;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Enable interaction between player and interactable objects
    /// </summary>
    public class PlayerInteractor : MonoBehaviour
    {
        [Tooltip("Valid maximum distance for the ray cast to happen")]
        [SerializeField] private float maxInteractionRay = 1f;

        #region Farm Interaction

        private FarmLand _selectedFarmLand;
        /// <summary>
        /// Current selected farm land
        /// </summary>
        public FarmLand SelectedFarmLand { get { return _selectedFarmLand; } }
        /// <summary>
        /// Detect whether player is selecting any interactable object
        /// </summary>
        public bool Selecting { get; private set; }
        #endregion

        #region Item Interactioin
        private PickableItem _selectedItem;
        public PickableItem SelectedItem { get { return _selectedItem; } }
        private Crop _selectedCrop;
        public Crop SelectedCrop { get { return _selectedCrop; } }
        #endregion
        private void Update()
        {
            // Draw raycast below
            RaycastHit hit;
            if(Physics.Raycast(transform.position, Vector3.down, out hit, maxInteractionRay))
            {
                Debug.Log("[Player Interactor] Hit: " + hit.collider.name);
                OnInteractableHit(hit);
            }
        }

        private void OnInteractableHit(RaycastHit hit)
        {
            Collider collider = hit.collider; // Get the collider reference

            // TODO: Detect other interactable item
            if(collider.TryGetComponent<PickableItem>(out PickableItem item))
            {
                OnSelectItem(item);
                Selecting = true;

                if (_selectedFarmLand != null)
                {
                    _selectedFarmLand.OnSelect(false);
                    _selectedFarmLand = null;
                }

                if (_selectedCrop != null)
                {
                    _selectedCrop.OnSelect(false);
                    _selectedCrop = null;
                }
            }
            else if (collider.TryGetComponent<Crop>(out Crop crop))
            {
                OnSelectCrop(crop);
                Selecting = true;

                if (_selectedFarmLand != null)
                {
                    _selectedFarmLand.OnSelect(false);
                    _selectedFarmLand = null;
                }

                if (_selectedItem != null)
                {
                    Debug.Log("Selected Item: " + _selectedItem);
                    _selectedItem.OnSelect(false);
                    _selectedItem = null;
                }
            }
            else if(collider.TryGetComponent<FarmLand>(out FarmLand farm)) // Detect farm land
            {
                //Debug.Log("Detecting Land: " + farm.name + " State: " + farm.CurrentState.ToString());
                OnSelectFarmLand(farm);
                Selecting = true;

                if (_selectedItem != null)
                {
                    Debug.Log("Selected Item: " + _selectedItem);
                    _selectedItem.OnSelect(false);
                    _selectedItem = null;
                }

                if (_selectedCrop != null)
                {
                    _selectedCrop.OnSelect(false);
                    _selectedCrop = null;
                }
            } 
            else
            {
                // As player not step on the farm land, if there is any farm land should set to null
                if(_selectedFarmLand != null)
                {
                    _selectedFarmLand.OnSelect(false);
                    _selectedFarmLand = null;
                }

                if(_selectedItem != null)
                {
                    Debug.Log("Selected Item: " + _selectedItem);
                    _selectedItem.OnSelect(false);
                    _selectedItem = null;
                }

                if(_selectedCrop != null)
                {
                    _selectedCrop.OnSelect(false);
                    _selectedCrop = null;
                }

                Selecting = false;
            }
        }

        /// <summary>
        /// Toggle selection of the farm land
        /// </summary>
        /// <param name="land"></param>
        private void OnSelectFarmLand(FarmLand land)
        {
            // If its not null, need to deselect the previous farm land
            if(_selectedFarmLand != null)
            {
                _selectedFarmLand.OnSelect(false);
            }

            // Set the farm land
            _selectedFarmLand = land;
            _selectedFarmLand.OnSelect(true);
        }

        private void OnSelectItem(PickableItem item)
        {
            // If its not null, need to deselect the previous item
            if (_selectedItem != null)
            {
                _selectedItem.OnSelect(false);
            }

            // Set the item
            _selectedItem = item;
            _selectedItem.OnSelect(true);
        }

        private void OnSelectCrop(Crop item)
        {
            // If its not null, need to deselect the previous crop
            if (_selectedCrop != null)
            {
                _selectedCrop.OnSelect(false);
            }

            // Set the crop
            _selectedCrop = item;
            _selectedCrop.OnSelect(true);
        }
    }
}
