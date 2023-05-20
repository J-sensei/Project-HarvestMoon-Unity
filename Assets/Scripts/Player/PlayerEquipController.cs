using Inventory;
using Item;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Attach relative game object to the player when equip something
    /// </summary>
    public class PlayerEquipController : MonoBehaviour
    {
        [Tooltip("Attach Point for the tools on hand")]
        [SerializeField] private Transform attachPoint;
        [SerializeField] private Transform itemAttachPoint;

        /// <summary>
        /// Current game object that is instantiate and attached to the player
        /// </summary>
        private GameObject _currentTool;
        /// <summary>
        /// Current name of the item
        /// </summary>
        private string _currentItemName;
        private void Update()
        {
            // Check holding item
            ItemData data = InventoryManager.Instance.HoldingItem;
            if (data != null)
            {
                /**
                 * If current item hold is null then we should instantiate it
                 * If different time is switch to hand, we also need to sinatantiate the new item on player hand
                 */
                if ((_currentTool == null || !_currentItemName.Equals(data.name)))
                {
                    // TODO: Add seed and item data
                    // If the data is a tool
                    if(data.type == ItemType.Tool)
                    {
                        if (data.itemPrefab != null)
                        {
                            if (_currentTool != null) DeleteTool(); // IF there is any item just delete it

                            _currentTool = Instantiate(data.itemPrefab, attachPoint);
                            _currentTool.GetComponent<PickableItem>().OnHold();
                            _currentTool.transform.parent = _currentTool.transform;
                            _currentItemName = data.name;
                        }
                        else
                        {
                            Debug.LogWarning("[Player Equip Controller] Tool item prefab is null!");
                        }
                    }
                    else
                    {
                        if(data.itemPrefab != null)
                        {
                            if (_currentTool != null) DeleteTool(); // IF there is any item just delete it

                            // TODO: Translate player to lift state
                            _currentTool = Instantiate(data.itemPrefab, itemAttachPoint);
                            _currentTool.GetComponent<PickableItem>().OnHold();
                            _currentTool.transform.parent = _currentTool.transform;
                            _currentItemName = data.name;
                        }
                        else
                        {
                            DeleteTool();
                        }
                    }
                }
            }
            else
            {
                DeleteTool();
            }
        }

        private void DeleteTool()
        {
            if (_currentTool != null)
            {
                Destroy(_currentTool.gameObject);
                _currentTool = null;
                _currentItemName = "";
            }
        }

        public void DetachItem()
        {
            if(_currentTool != null)
            {
                _currentTool.GetComponent<PickableItem>().OnThrow();
                _currentTool.transform.parent = null;
                _currentTool = null;
                InventoryManager.Instance.Unload();
            }
            else
            {
                Debug.LogWarning("[Player Equip Controller] Trying to detach null object!");
            }
        }
    }
}

