using Farming;
using Interactable;
using Item;
using UnityEngine;

namespace Player
{
    /*
     * Interactor is using raycast shooting down from top to bottom to detect any item has interactable components to it
     * Interactor has its own layer, any layer physics disable to the interactor will not work!
     * Check Unity Editor to see if the physics between interactor and the interactable object has proper physics layer
     */
    /// <summary>
    /// Enable interaction between player and interactable objects
    /// </summary>
    public class PlayerInteractor : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Valid maximum distance for the ray cast to happen")]
        [SerializeField] private float maxInteractionRay = 1f;


        [Header("Debug")]
        [Tooltip("Show debug properties fro the player interactor")]
        [SerializeField] private bool debug = false;
        /// <summary>
        /// Line color for the debug ray
        /// </summary>
        private Color _debugLineColor = Color.white;

        public bool Selecting { get; private set; }

        #region Item Interaction
        private IInteractable _selectedInteractable;
        public IInteractable SelectedtInteractable { get { return _selectedInteractable; } }
        #endregion
        private void Update()
        {
            // Draw raycast below
            RaycastHit hit;
            if(Physics.Raycast(transform.position, Vector3.down, out hit, maxInteractionRay))
            {
                //Debug.Log("[Player Interactor] Hit: " + hit.collider.name);
                OnInteractableHit(hit);
            }
            else
            {
                Deselect();
            }

            if (debug)
            {
                // Draw ray to show how the ray cast work
                Debug.DrawRay(transform.position, Vector3.down * maxInteractionRay, _debugLineColor, 0f, false);
            }
        }

        /// <summary>
        /// Call when raycast is hitting something
        /// </summary>
        /// <param name="hit"></param>
        private void OnInteractableHit(RaycastHit hit)
        {
            Debug.Log("Hit: " + hit.collider.name);
            _debugLineColor = Color.red; // Update the color to red to indicate something is selected for debug line
            Collider collider = hit.collider; // Get the collider reference

            // Hitting a interactable object
            if (collider.TryGetComponent(out IInteractable interactable))
            {
                OnSelectInteractable(interactable);
                Selecting = true;
            }
            else // Not hitting anything valid item
            {
                Deselect();
            }
        }

        /// <summary>
        /// Deselect interactable object
        /// </summary>
        private void Deselect()
        {
            if (_selectedInteractable != null)
            {
                _selectedInteractable.OnSelect(false);
                _selectedInteractable = null;
            }

            Selecting = false;
            _debugLineColor = Color.white;
        }

        /// <summary>
        /// Select interactable objects
        /// </summary>
        /// <param name="interactable">Object that inherites IInteractable</param>
        private void OnSelectInteractable(IInteractable interactable)
        {
            // If its not null, deselect the previous interactable
            if (_selectedInteractable != null)
            {
                _selectedInteractable.OnSelect(false);
            }

            // Set the new interactable object
            _selectedInteractable = interactable;
            _selectedInteractable.OnSelect(true);
        }
    }
}
