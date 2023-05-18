using Farming;
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
        private void Update()
        {
            // Draw raycast below
            RaycastHit hit;
            if(Physics.Raycast(transform.position, Vector3.down, out hit, maxInteractionRay))
            {
                OnInteractableHit(hit);
            }
        }

        private void OnInteractableHit(RaycastHit hit)
        {
            Collider collider = hit.collider; // Get the collider reference

            // TODO: Detect other interactable item
            // Detect farm land
            if(collider.TryGetComponent<FarmLand>(out FarmLand farm))
            {
                //Debug.Log("Detecting Land: " + farm.name + " State: " + farm.CurrentState.ToString());
                OnSelectFarmLand(farm);
                Selecting = true;
            } 
            else
            {
                // As player not step on the farm land, if there is any farm land should set to null
                if(_selectedFarmLand != null)
                {
                    _selectedFarmLand.OnSelect(false);
                    _selectedFarmLand = null;
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
    }
}
