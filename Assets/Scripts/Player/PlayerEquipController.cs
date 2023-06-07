using Inventory;
using Item;
using StateMachine.Player;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Attach relative game object to the player when equip something
    /// </summary>
    public class PlayerEquipController : MonoBehaviour
    {
        [Header("State Machine")]
        [Tooltip("Player State Machine reference to allow switching state")]
        [SerializeField] private PlayerStateMachine player;

        [Header("Transform points")]
        [Tooltip("Attach Point holding a tool in a hand")]
        [SerializeField] private Transform attachPoint;
        [Tooltip("Attach point to lift the item")]
        [SerializeField] private Transform itemAttachPoint;

        /// <summary>
        /// Current game object that is instantiate and attached to the player
        /// </summary>
        private GameObject _currentItem;
        /// <summary>
        /// Current name of the item (Use to compare and detect item changes)
        /// </summary>
        private string _currentItemName;

        private void Update()
        {
            // Check data for current holding item
            ItemData data = InventoryManager.Instance.HoldingItem;

            // Player is not holding anything
            if(data == null)
            {
                DeleteTool(); // Delete holding item if any
                return;
            }

            //if (data != null)
            //{
                /**
                 * If current item hold is null then we should instantiate it
                 * If different time is switch to hand, we also need to sinatantiate the new item on player hand
                 */
                if ((_currentItem == null || !_currentItemName.Equals(data.name)))
                {
                    // TODO: Add seed and item data
                    // If the data is a tool
                    if(data.type == ItemType.Tool)
                    {
                        if (data.itemPrefab != null)
                        {
                            if (_currentItem != null) DeleteTool(); // IF there is any item just delete it

                            _currentItem = Instantiate(data.itemPrefab, attachPoint);
                            _currentItem.GetComponent<PickableItem>().OnHold();
                            _currentItem.transform.parent = _currentItem.transform;
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
                            if (_currentItem != null) DeleteTool(); // IF there is any item just delete it

                            // TODO: Translate player to lift state
                            _currentItem = Instantiate(data.itemPrefab, itemAttachPoint);
                            _currentItem.GetComponent<PickableItem>().OnHold();
                            _currentItem.transform.parent = _currentItem.transform;
                            _currentItemName = data.name;

                            // Transition to lift state
                            player.PickingItem = true;
                            player.SwitchState(player.StateFactory.Lift());
                        }
                        else
                        {
                            DeleteTool();
                        }
                    }
                }
            //}
            //else
            //{
            //    DeleteTool();
            //}
        }

        /// <summary>
        /// Delete current holding item if its not null
        /// </summary>
        private void DeleteTool()
        {
            if (_currentItem != null)
            {
                Destroy(_currentItem.gameObject);
                _currentItem = null;
                _currentItemName = "";
            }
        }

        public void DetachItem()
        {
            if(_currentItem != null)
            {
                _currentItem.GetComponent<PickableItem>().OnThrow();
                _currentItem.transform.parent = null;
                _currentItem = null;
                InventoryManager.Instance.Unload();
            }
            else
            {
                Debug.LogWarning("[Player Equip Controller] Trying to detach null object!");
            }
        }
    }
}

