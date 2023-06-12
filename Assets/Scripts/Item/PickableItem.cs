using Interactable;
using Inventory;
using UnityEngine;
using Utilities;
using QuickOutline;

namespace Item
{
    /// <summary>
    /// Item that can be pick and lift by the player
    /// </summary>
    public class PickableItem : MonoBehaviour, IInteractable
    {
        [Tooltip("Item Data of this pickable item")]
        [SerializeField] private ItemData itemData;
        [Tooltip("Rigidbody of the object")]
        [SerializeField] private Rigidbody rb;
        [Tooltip("Collider to detect the raycast of player interactor, disable when player holding it")]
        [SerializeField] private Collider[] itemColliders;
        [Tooltip("Outline to show the item is selected")]
        [SerializeField] private Outline outline;

        [Header("Physics")]
        [Tooltip("Distance before hit something on the ground to determine is grounded or not")]
        [SerializeField] private float groundedDistance = 0.1f;

        /// <summary>
        /// Item data of the pickable item object
        /// </summary>
        public ItemData ItemData { get { return itemData; } }
        /// <summary>
        /// Orignal attached transform position (Use to reset the transform)
        /// </summary>
        private Vector3 _attachPos;
        /// <summary>
        /// Original attached transform rotation (Use to reset the transform)
        /// </summary>
        private Quaternion _attachRot;

        /// <summary>
        /// Is the item attaching is player
        /// </summary>
        public bool Attaching { get; private set; } = false;

        private void Awake()
        {
            if(rb == null)
            {
                rb = GetComponentInChildren<Rigidbody>();
                if (rb == null)
                    rb = GetComponent<Rigidbody>();
                _attachPos = rb.transform.localPosition;
                _attachRot = rb.transform.localRotation;
            }
            else
            {
                _attachPos = rb.transform.localPosition;
                _attachRot = rb.transform.localRotation;
            }

            if(outline == null)
            {
                outline = GetComponent<Outline>();
            }

            outline.enabled = false;
            StartCoroutine(OutlineHelper.InitializeOutline(outline));
        }

        private void FixedUpdate()
        {
            if(rb && !rb.isKinematic)
            {
                if(!Physics.Raycast(transform.position, -Vector3.up, groundedDistance))
                {
                    rb.drag = 1;
                    rb.angularDrag = 1;
                }
            }
        }

        /// <summary>
        /// When player request to hold the item
        /// </summary>
        public void OnHold()
        {
            if(rb != null)
            {
                rb.gameObject.transform.localPosition = _attachPos;
                rb.gameObject.transform.localRotation = _attachRot;
                rb.isKinematic = true;
            }
            else
            {
                Debug.Log("[Pickable Item]" + name + " rigidbody is not defined!");
            }

            if(itemColliders != null)
            {
                foreach (Collider collider in itemColliders)
                    collider.enabled = false;
            }
            else
            {
                Debug.Log("[Pickable Item]" + name + " item collider is not defined!");
            }

            Attaching = true;
        }

        public void OnThrow(float drag = float.MaxValue, float angularDrag = float.MaxValue)
        {
            if (rb != null)
            {
                rb.drag = drag;
                rb.angularDrag = angularDrag;
                rb.isKinematic = false;
            }
            else
            {
                Debug.Log("[Pickable Item]" + name + " rigidbody is not defined!");
            }

            if (itemColliders != null)
            {
                foreach(Collider collider in itemColliders)
                    collider.enabled = true;
            }
            else
            {
                Debug.Log("[Pickable Item]" + name + " item collider is not defined!");
            }

            Attaching = false;
        }

        public void Interact()
        {
            InventoryManager.Instance.Pickup(itemData); // Update the holding item of the inventory manager
            Destroy(gameObject); // Destroy the object as its already picked up by the player
        }

        public InteractableType GetInteractableType()
        {
            switch (itemData.type)
            {
                case ItemType.Tool:
                    return InteractableType.Tool;
                case ItemType.Item:
                    return InteractableType.Item;
                default:
                    throw new UnityException("[Pickable Item] Invalid Item Type");
            }
        }
        public void OnSelect(bool v)
        {
            if (outline != null)
                outline.enabled = v;
        }

        public ItemData GetItemData()
        {
            return itemData;
        }
    }
}
