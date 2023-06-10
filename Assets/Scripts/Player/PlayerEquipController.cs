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
            // TODO: Optimise this to call when player is picking, equip, unequip item
            // Check data for current holding item
            ItemData data = InventoryManager.Instance.HoldingItem; // Constantly check the current holding item and responded to it

            // Player is not holding anything
            if(data == null)
            {
                CheckPlayerPicking();
                DeleteTool(); // Delete holding item if any
                return;
            }

            /**
             * If current item hold is null then we should instantiate it
             * If different time is switch to hand, we also need to sinatantiate the new item on player hand
             */
            if ((_currentItem == null || !_currentItemName.Equals(data.name)))
            {
                // TODO: Add seed and item data
                // If the data is a tool
                if (data.type == ItemType.Tool)
                {
                    CheckPlayerPicking();
                    if (data.itemPrefab != null)
                    {
                        if (_currentItem != null) 
                            DeleteTool(); // If there is any item just delete it

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
                    if (data.itemPrefab != null)
                    {
                        if (_currentItem != null) 
                            DeleteTool(); // If there is any item just delete it

                        // Translate player to lift state
                        _currentItem = Instantiate(data.itemPrefab, itemAttachPoint);
                        _currentItem.GetComponent<PickableItem>().OnHold();
                        _currentItem.transform.parent = _currentItem.transform;
                        _currentItemName = data.name;

                        // Transition to lift state
                        // Checking to prevent player go lift again even when player already inside lift state
                        if (player.CurrentState != player.StateFactory.Lift())
                        {
                            player.PickingItem = true;
                            player.SwitchState(player.StateFactory.Lift());
                        }
                    }
                    else
                    {
                        DeleteTool();
                    }
                }
            }
        }

        /// <summary>
        /// Check if player change the item when character is picking up the item
        /// </summary>
        private void CheckPlayerPicking()
        {
            // Fixed when player is picking item and take out the item from inventory
            if (player.PickingItem)
            {
                player.SwitchState(player.StateFactory.Grounded());
                player.PickingItem = false;
            }

            if (player.DroppingItem)
            {
                player.SwitchState(player.StateFactory.Grounded());
                player.DroppingItem = false;
            }
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

        /// <summary>
        /// Detach current carrying item from the player
        /// </summary>
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

