using Inventory;
using UnityEngine;

namespace Item
{
    /// <summary>
    /// Item that can be pick and lift by the player
    /// </summary>
    public class PickableItem : MonoBehaviour
    {
        [SerializeField] private ItemData itemData;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private Canvas canvas;
        [Tooltip("Collider to detect the raycast of player interactor")]
        [SerializeField] private Collider itemCollider;
        [Tooltip("Outline to show the item is selected")]
        [SerializeField] private Outline outline;

        public ItemData ItemData { get { return itemData; } }
        private Vector3 _attachPos;
        private Quaternion _attachRot;

        private void Awake()
        {
            if(canvas == null)
            {
                canvas = GetComponentInChildren<Canvas>();
                canvas.gameObject.SetActive(false);
            }
            else
            {
                canvas.gameObject.SetActive(false);
            }

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
                //if (outline == null) outline = GetComponentInChildren<Outline>();
            }

            outline.enabled = false;
        }

        /// <summary>
        /// Select and hightlight the pickable item
        /// </summary>
        /// <param name="v"></param>
        public void OnSelect(bool v)
        {
            //if(canvas != null)
            //{
            //    canvas.gameObject.SetActive(v);
            //}
            //else
            //{
            //    Debug.LogWarning("[Pickable Item] Trying to show canvas but canvas is null");
            //}
            outline.enabled = v;
        }

        public void OnPickup()
        {
            Destroy(gameObject);
        }

        public void OnHold()
        {
            if(rb != null)
            {
                rb.gameObject.transform.localPosition = _attachPos;
                rb.gameObject.transform.localRotation = _attachRot;
                rb.isKinematic = true;
                Debug.Log("Yes its holding by the player");
            }
            else
            {
                Debug.Log("[Pickable Item]" + name + " rigidbody is not defined!");
            }

            if(itemCollider != null)
            {
                itemCollider.enabled = false;
            }
            else
            {
                Debug.Log("[Pickable Item]" + name + " item collider is not defined!");
            }
        }

        public void OnThrow()
        {
            if (rb != null)
            {
                rb.isKinematic = false;
                Debug.Log("Item rigidbody kinematic is disable back");
            }
            else
            {
                Debug.Log("[Pickable Item]" + name + " rigidbody is not defined!");
            }

            if (itemCollider != null)
            {
                itemCollider.enabled = true;
            }
            else
            {
                Debug.Log("[Pickable Item]" + name + " item collider is not defined!");
            }
        }
    }
}
