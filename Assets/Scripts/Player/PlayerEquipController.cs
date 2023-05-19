using Inventory;
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
                    // If the data is a tool
                    if(data is ToolData)
                    {
                        ToolData toolData = (ToolData)data;
                        if (toolData.itemPrefab != null)
                        {
                            if (_currentTool != null) DeleteTool(); // IF there is any item just delete it

                            _currentTool = Instantiate(toolData.itemPrefab, attachPoint);
                            _currentTool.transform.parent = _currentTool.transform;
                            _currentItemName = toolData.name;
                        }
                        else
                        {
                            Debug.LogWarning("[Player Equip Controller] Tool item prefab is null!");
                        }
                    }
                    else
                    {
                        // TODO: Item on hand

                        DeleteTool();
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
    }
}

